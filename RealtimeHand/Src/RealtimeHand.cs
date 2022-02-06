
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


namespace RTHand
{

    public class RealtimeHand : IDisposable
    {
        public bool IsVisible { get; private set; }
        public bool IsInitialized { get; private set; } = false;
        public Dictionary<JointName, Joint> Joints = new Dictionary<JointName, Joint>();

        Camera arCameraCache;
        Matrix4x4 unityDisplayMatrix ;

        public bool Initialize(ARSession _session, ARCameraManager _arCameraManager, Matrix4x4 _unityDisplayMatrix)
        {
            arCameraCache = _arCameraManager.GetComponent<Camera>();
            unityDisplayMatrix = _unityDisplayMatrix;

            if (_session == null || _session.subsystem == null)
            {
                Debug.LogError("Cannot register : invalid sesssion (wait for tracking state");
                return false;
            }

            IntPtr nativeSessionPtr = _session.subsystem.nativePtr;
            RealtimeHandNative.VisionHandDetectorCreate(nativeSessionPtr);

            // Initialisation check

            if (arCameraCache == null)
            {
                Debug.LogError("Missing camera on ARCameraManager");
                return false;
            }

            Joints = new Dictionary<JointName, Joint>();
            foreach (JointName name in Enum.GetValues(typeof(JointName)))
            {
                Joint joint = new Joint();
                joint.name = name;
                Joints.Add(name, joint);
            }

            IsVisible = false;
            IsInitialized = true;
            return true;
        }

        public void Dispose()
        {
            RealtimeHandNative.VisionHandDetectorRelease(); 
        }

        public void Process( CPUEnvironmentDepth _environmentDepth, CPUHumanStencil _humanStencil )
        {
            if (!IsInitialized)
            {
                Debug.LogError("Cannot process as not initialized yet");
                return;
            }

            if (_environmentDepth == null || _humanStencil == null)
            {
                Debug.LogError("Could not retrieve depth images");
                return ;
            }

            if (unityDisplayMatrix == Matrix4x4.zero)
            {
                Debug.LogError("Missing display matrix");
                return;
            }

            // Call Swift to do the heavy work!
            string message = RealtimeHandNative.Process();

            IsVisible = !string.IsNullOrEmpty(message);
            if (!IsVisible)
            {
                return ;
            }

            // Native MessageFormat => strJointName:strX|strY|strConfidence;...

            string[] points = message.Split(';');
            foreach (var point in points)
            {
                string[] kv = point.Split(':');   
                if (kv.Length != 2)
                {
                    continue;
                }

                string strJointName = kv[0];
                if (!Enum.TryParse(strJointName, out JointName jointName))
                {
                    continue;
                }

                string[] coord = kv[1].Split('|');
                if (coord.Length != 3)
                {
                    continue;
                }

                // Extract 2D & Confidence
                string strX = coord[0];
                string strY = coord[1];
                Vector2 screenPos = new Vector2(Convert.ToSingle(strX, CultureInfo.InvariantCulture), Convert.ToSingle(strY, CultureInfo.InvariantCulture));
                Joints[jointName].screenPos = screenPos;
                Joints[jointName].isVisible = true;
                Joints[jointName].texturePos = ImageCPUHelper.ConvertScreenToCPUImage(unityDisplayMatrix,Joints[jointName].screenPos);
                string strConfidence = coord[2];
                float confidence = Convert.ToSingle(strConfidence, CultureInfo.InvariantCulture);
                Joints[jointName].confidence = confidence;

                // Extract distance from environmentDepth
                float distance = ImageCPUHelper.GetHumanDistanceFromEnvironment(_environmentDepth,_humanStencil,Joints[jointName].texturePos);
                Joints[jointName].distance = distance;

                // Calculate 3D Joint position
                var worldPos = arCameraCache.ScreenToWorldPoint(new Vector3(screenPos.x*Screen.width, screenPos.y*Screen.height, distance));
                Joints[jointName].worldPos = worldPos;   
            }
        }

    }
}