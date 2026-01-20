using UnityEngine;

namespace MagicVillageDash.Settings
{
    public static class GraphicsQualityManager
    {
        public enum QualityLevel
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

        public static void Apply(int level)
        {
            QualitySettings.SetQualityLevel(level, applyExpensiveChanges: true);

            switch ((QualityLevel)level)
            {
                case QualityLevel.Low:
                    ApplyLOD(0.4f);
                    ApplyShadows(ShadowQuality.Disable);
                    ApplyAntiAliasing(0);
                    ApplyVSync(0);
                    ApplyTextureResolution(1); // Half resolution
                    break;

                case QualityLevel.Medium:
                    ApplyLOD(0.75f);
                    ApplyShadows(ShadowQuality.HardOnly);
                    ApplyAntiAliasing(2);
                    ApplyVSync(0);
                    ApplyTextureResolution(0); // Full resolution
                    break;

                case QualityLevel.High:
                default:
                    ApplyLOD(1.2f);
                    ApplyShadows(ShadowQuality.All);
                    ApplyAntiAliasing(4);
                    ApplyVSync(1);
                    ApplyTextureResolution(0);
                    break;
            }
        }

        private static void ApplyLOD(float lodBias)
        {
            QualitySettings.lodBias = lodBias;
        }

        private static void ApplyShadows(ShadowQuality quality)
        {
            QualitySettings.shadows = quality;
        }

        private static void ApplyAntiAliasing(int samples)
        {
            QualitySettings.antiAliasing = samples;
        }

        private static void ApplyVSync(int count)
        {
            QualitySettings.vSyncCount = count;
        }

        private static void ApplyTextureResolution(int masterLimit)
        {
            // 0 = Full, 1 = Half, 2 = Quarter
            //QualitySettings.masterTextureLimit = masterLimit;
            QualitySettings.globalTextureMipmapLimit = masterLimit;
        }
    }
}