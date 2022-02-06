using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace RTHand
{
    public abstract class CPUImageBase : IDisposable
    {
        public int width;
        public int height;
        public abstract void Dispose();
    }

    public class CPUEnvironmentDepth : CPUImageBase
    {
        public NativeArray<float> pixels;

        public override void Dispose()
        {
            if (pixels.IsCreated)
            {
                pixels.Dispose();
            }
        }

        // Factory
        unsafe public static CPUEnvironmentDepth Create(AROcclusionManager _occlusionManager)
        {
            XRCpuImage cpuImage;
            bool res = _occlusionManager.TryAcquireEnvironmentDepthCpuImage(out cpuImage);
            if (!res)
            {
                return null;
            }

            CPUEnvironmentDepth environmentDepth = new CPUEnvironmentDepth();
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
                outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
                outputFormat = TextureFormat.RFloat,
                transformation = XRCpuImage.Transformation.None
            };

            int size = cpuImage.GetConvertedDataSize(conversionParams);
            var buffer = new NativeArray<float>(size, Allocator.Temp);
            cpuImage.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);
            cpuImage.Dispose();

            environmentDepth.width = conversionParams.outputDimensions.x;
            environmentDepth.height = conversionParams.outputDimensions.y;
            environmentDepth.pixels = buffer;
            return environmentDepth;
        }
    }

    public class CPUHumanStencil : CPUImageBase
    {
        public NativeArray<byte> pixels;

        public override void Dispose()
        {
            if (pixels.IsCreated)
            {
                pixels.Dispose();
            }
        }

        // Factory
        unsafe static public CPUHumanStencil Create(AROcclusionManager _occlusionManager)
        {
            XRCpuImage cpuImage;
            bool res = _occlusionManager.TryAcquireHumanStencilCpuImage(out cpuImage);
            if (!res)
            {
                return null;
            }

            CPUHumanStencil humanStencil = new CPUHumanStencil();
            var conversionParams = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
                outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
                outputFormat = TextureFormat.R8,
                transformation = XRCpuImage.Transformation.None
            };

            int size = cpuImage.GetConvertedDataSize(conversionParams);
            var buffer = new NativeArray<byte>(size, Allocator.Temp);
            cpuImage.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);
            cpuImage.Dispose();

            humanStencil.width = conversionParams.outputDimensions.x;
            humanStencil.height = conversionParams.outputDimensions.y;
            humanStencil.pixels = buffer;
            return humanStencil;
        }
    }
}
