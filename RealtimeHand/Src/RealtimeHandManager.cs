
using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace RTHand
{

    public class RealtimeHandManager : MonoBehaviour
    {
        [SerializeField]
        protected ARSession session;

        [SerializeField]
        protected AROcclusionManager occlusionManager;

        [SerializeField]
        protected ARCameraManager cameraManager;

        public Action<RealtimeHand> HandUpdated;
        public RealtimeHand realtimeHand { get; private set; }

        CPUEnvironmentDepth environmentDepth;
        CPUHumanStencil humanStencil;

        protected void Start()
        {
            realtimeHand = new RealtimeHand();
            cameraManager.frameReceived += OnFrameReceived;
        }

        protected void OnDestroy()
        {
            realtimeHand?.Dispose();
            if (cameraManager != null)
            {
                cameraManager.frameReceived -= OnFrameReceived;
            }
        }

        private void OnFrameReceived(ARCameraFrameEventArgs _arg)
        {
            environmentDepth?.Dispose();
            humanStencil?.Dispose();

            if (!enabled)
            {
                return;
            }

            if (!_arg.displayMatrix.HasValue)
            {
                return;
            }

            if (!realtimeHand.IsInitialized)
            {
                if (ARSession.state == ARSessionState.SessionTracking)
                {
                    realtimeHand.Initialize(session, cameraManager, _arg.displayMatrix.Value);
                }
                return;
            }

            environmentDepth = CPUEnvironmentDepth.Create(occlusionManager);
            if (environmentDepth == null)
            {
                return;
            }
            humanStencil = CPUHumanStencil.Create(occlusionManager);
            if (humanStencil == null)
            {
                return;
            }

            realtimeHand.Process(environmentDepth, humanStencil);

            HandUpdated?.Invoke(realtimeHand);
        }

    }
}