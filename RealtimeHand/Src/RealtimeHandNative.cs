using System;
using System.Runtime.InteropServices;

namespace RTHand
{
    internal static class RealtimeHandNative 
    {
        
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void VisionHandDetectorCreate(IntPtr session);
        [DllImport("__Internal")]
        public static extern void VisionHandDetectorRelease();
        [DllImport("__Internal")]
        public static extern string Process();
#else
        public static void VisionHandDetectorCreate(IntPtr session) {}
        public static void VisionHandDetectorRelease() {}
        public static string Process() { return null; }
#endif

    }
}
