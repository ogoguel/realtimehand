using System;
using UnityEngine;

namespace RTHand
{
    [Serializable]
    public class Joint
    {
        public Vector2 screenPos;
        public Vector2 texturePos;
        public Vector3 worldPos;
        public JointName name;
        public float distance;
        public bool isVisible;
        public float confidence;
    }
}