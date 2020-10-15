using System;
using UnityEngine;

namespace DominantColor
{
    public class DominantRGBColorCalculator : IDominantColorCalculator
    {
        public Color CalculateDominantColor(Texture2D bitmap)
        {
            return ColorUtils.GetAverageRGBColor(bitmap);
        }
    }
}
