
using UnityEngine;

namespace RTHand
{

    public static class ImageCPUHelper
    {

        public static Vector2 ConvertScreenToCPUImage(Matrix4x4 _unityDisplayMatrix, Vector2 _screenNormalizedCoord)
        {
            Vector4 screenNormalized = new Vector4(_screenNormalizedCoord.x, _screenNormalizedCoord.y, 1, 0);
            Vector4 textureCoord = _unityDisplayMatrix.transpose * screenNormalized;
            if (textureCoord.x < 0) textureCoord.x += 1;
            if (textureCoord.y < 0) textureCoord.y += 1;

            Vector2 res = new Vector2(textureCoord.x, textureCoord.y);
            return res;
        }

        public static float GetHumanDistanceFromEnvironment(CPUEnvironmentDepth _depth, CPUHumanStencil _humanStencil, Vector2 _screenPosition, int _rayon = 5)
        {
            int depthX = (int)(_screenPosition.x * _depth.width);
            int depthY = (int)(_screenPosition.y * _depth.height);
            int nbValid = 0;
            int nbZero = 0;

            float[] distances = new float[(_rayon * 2 + 1) * (_rayon * 2 + 1)];

            int i = 0;
            for (int x = -_rayon; x <= _rayon; x++)
            {
                for (int y = -_rayon; y <= _rayon; y++)
                {
                    int idx = (depthX + x) + (depthY + y) * _depth.width;
                    if (idx < 0 || idx >= _depth.pixels.Length)
                    {
                        distances[i++] = 0;
                        continue;
                    }

                    var stencil = _humanStencil.pixels[idx];
                    if (stencil < 128)
                    {
                        distances[i++] = 0;
                        continue;
                    }

                    float dist = _depth.pixels[idx];
                    distances[i++] = dist;

                    if (dist == 0)
                    {
                        nbZero++;
                    }
                    else
                    {
                        nbValid++;
                    }
                }
            }

            float min = 1000;
            float max = -1000;
            for (int d = 0; d < distances.Length; d++)
            {
                var dist = distances[d];
                if (dist == 0) continue;
                if (dist < min) min = dist;
                if (dist > max) max = dist;
            }

            float total = 0;
            nbValid = 0;
            for (int d = 0; d < distances.Length; d++)
            {
                var dist = distances[d];
                if (dist == 0)
                {
                    continue;
                }
                if ((dist - min) < 0.1)
                {
                    total += dist;
                    nbValid++;
                }
            }

            return total / nbValid;
        }
    }
}