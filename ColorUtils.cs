using System;
using System.Collections.Generic;
using UnityEngine;

namespace DominantColor
{
    public class ColorUtils
    {
        /// <summary>
        /// Get hue histogram for given bitmap.
        /// </summary>
        /// <param name="texture">The bitmap to get the histogram for</param>
        /// <param name="saturationThreshold">The saturation threshold to take into account getting the histogram</param>
        /// <param name="brightnessThreshold">The brightness threshold to take into account getting the histogram</param>
        /// <returns>A dictionary representing the hue histogram. Key: Hue index (0-360). Value: Occurence of the hue.</returns>
        internal static Dictionary<int, uint> GetColorHueHistogram(Texture2D texture, float saturationThreshold,
            float brightnessThreshold)
        {
            Dictionary<int, uint> colorHueHistorgram = new Dictionary<int, uint>();
            for (int i = 0; i <= 360; i++)
            {
                colorHueHistorgram.Add(i, 0);
            }

            for (int i = 0; i < texture.width; ++i)
            {
                for (int j = 0; j < texture.height; ++j)
                {
                    Color clr = texture.GetPixel(i, j);
                    float H, S, V;

                    Color.RGBToHSV(clr, out H, out S, out V);
                    if (S > saturationThreshold && V > brightnessThreshold)
                    {
                        int hue = (int) Math.Round(H * 360, 0);
                        colorHueHistorgram[hue]++;
                    }
                }
            }

            return colorHueHistorgram;
        }

        /// <summary>
        /// Calculate average RGB color for given bitmap
        /// </summary>
        /// <param name="bmp">The bitmap to calculate the average color for.</param>
        /// <returns>Average color</returns>
        internal static Color GetAverageRGBColor(Texture2D bmp)
        {
            float totalRed = 0;
            float totalGreen = 0;
            float totalBlue = 0;

            for (int i = 0; i < bmp.width; ++i)
            {
                for (int j = 0; j < bmp.height; ++j)
                {
                    Color clr = bmp.GetPixel(i, j);
                    totalRed += clr.r;
                    totalGreen += clr.g;
                    totalBlue += clr.b;
                }
            }


            int totalPixels = bmp.width * bmp.height;
            float avgRed = (totalRed / totalPixels);
            float avgGreen = (totalGreen / totalPixels);
            float avgBlue = (totalBlue / totalPixels);
            return new Color(avgRed, avgGreen, avgBlue);
        }

        /// <summary>
        /// Correct out of bound hue index
        /// </summary>
        /// <param name="hue">hue index</param>
        /// <returns>Corrected hue index (within 0-360 boundaries)</returns>
        private static int CorrectHueIndex(int hue)
        {
            int result = hue;
            if (result > 360)
                result = result - 360;
            if (result < 0)
                result = result + 360;
            return result;
        }

        /// <summary>
        /// Get color from HSV (Hue, Saturation, Brightness) combination.
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="value"></param>
        /// <returns>The color</returns>
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            float v = Convert.ToInt32(value) / 255f;
            float p = Convert.ToInt32(value * (1 - saturation)) / 255f;
            float q = Convert.ToInt32(value * (1 - f * saturation)) / 255f;
            float t = Convert.ToInt32(value * (1 - (1 - f) * saturation)) / 255f;

            switch (hi)
            {
                case 0:
                    return new Color(v, t, p);
                case 1:
                    return new Color(q, v, p);
                case 2:
                    return new Color(p, v, t);
                case 3:
                    return new Color(p, q, v);
                case 4:
                    return new Color(t, p, v);
                default:
                    return new Color(v, p, q);
            }
        }

        /// <summary>
        /// Smooth histogram with given smoothfactor. 
        /// </summary>
        /// <param name="colorHueHistogram">The histogram to smooth</param>
        /// <param name="smoothFactor">How many hue neighbouring hue indexes will be averaged by the smoothing algoritme.</param>
        /// <returns>Smoothed hue color histogram</returns>
        internal static Dictionary<int, uint> SmoothHistogram(Dictionary<int, uint> colorHueHistogram, int smoothFactor)
        {
            if (smoothFactor < 0 || smoothFactor > 360)
                throw new ArgumentException("smoothFactor may not be negative or bigger then 360",
                    nameof(smoothFactor));
            if (smoothFactor == 0)
                return new Dictionary<int, uint>(colorHueHistogram);

            Dictionary<int, uint> newHistogram = new Dictionary<int, uint>();
            int totalNrColumns = (smoothFactor * 2) + 1;
            for (int i = 0; i <= 360; i++)
            {
                uint sum = 0;
                uint average = 0;
                for (int x = i - smoothFactor; x <= i + smoothFactor; x++)
                {
                    int hueIndex = CorrectHueIndex(x);
                    sum += colorHueHistogram[hueIndex];
                }

                average = (uint) (sum / totalNrColumns);
                newHistogram[i] = average;
            }

            return newHistogram;
        }
    }
}