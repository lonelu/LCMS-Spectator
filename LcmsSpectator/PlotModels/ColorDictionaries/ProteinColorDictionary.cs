﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProteinColorDictionary.cs" company="Pacific Northwest National Laboratory">
//   2015 Pacific Northwest National Laboratory
// </copyright>
// <author>Christopher Wilkins</author>
// <summary>
//   This class maps proteins to an OxyPlot coloring.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;

namespace LcmsSpectator.PlotModels.ColorDictionaries
{
    /// <summary>
    /// This class maps proteins to an OxyPlot coloring.
    /// </summary>
    public class ProteinColorDictionary
    {
        /// <summary>
        /// The dictionary mapping protein names to color indices.
        /// </summary>
        private readonly Dictionary<string, int> colorDictionary;

        /// <summary>
        /// Current protein index.
        /// </summary>
        private int protIndex;

        /// <summary>
        /// Color index offset.
        /// </summary>
        private int offset;

        /// <summary>
        /// Color offset is multiplied by 1/multiplier.
        /// </summary>
        private int multiplier;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProteinColorDictionary"/> class.
        /// </summary>
        /// <param name="numInterpolatedColors">The maximum number of colors for OxyPalette.</param>
        public ProteinColorDictionary(int numInterpolatedColors = 5000)
        {
            colorDictionary = new Dictionary<string, int> { { string.Empty, 0 } };
            protIndex = 1;
            offset = 0;
            multiplier = 2;

            var colors = new[]
                             {
                                 OxyColors.LightGreen, OxyColors.LightBlue, OxyColors.Turquoise, OxyColors.Olive,
                                 OxyColors.Brown, OxyColors.Cyan, OxyColors.Gray, OxyColors.Pink, OxyColors.LightSeaGreen,
                                 OxyColors.Beige
                             };

            OxyPalette = OxyPalette.Interpolate(numInterpolatedColors, colors);
            MaxColors = colors.Length;
        }

        /// <summary>
        /// Gets the palette of OxyColors used to get colors from.
        /// </summary>
        public OxyPalette OxyPalette { get; private set; }

        /// <summary>
        /// Gets the maximum number of colors in this color dictionary.
        /// </summary>
        public int MaxColors { get; private set; }

        /// <summary>
        /// Get an OxyColor from the palette for a specific protein.
        /// </summary>
        /// <param name="proteinName">The name of the protein.</param>
        /// <returns>The OxyColor for the given protein.</returns>
        public OxyColor GetColor(string proteinName)
        {
            return OxyPalette.Colors[GetColorCode(proteinName)];
        }

        /// <summary>
        /// Get a color code for the palette for a specific protein.
        /// </summary>
        /// <param name="proteinName">The name of the protein.</param>
        /// <returns>The index of the OxyColor from the OxyPalette for the given protein.</returns>
        public int GetColorCode(string proteinName)
        {
            if (!colorDictionary.ContainsKey(proteinName))
            {
                int colorIndex;
                do
                {   // do not select the same color as unid color.
                    var r = protIndex++ % MaxColors;
                    colorIndex = Math.Min((r * (OxyPalette.Colors.Count / MaxColors)) + offset, OxyPalette.Colors.Count - 1);
                }
                while (colorIndex == 0);

                colorDictionary.Add(proteinName, colorIndex);

                if (protIndex >= MaxColors)
                {
                    // When we've used up all the primary colors, use the colors midway in between
                    offset = OxyPalette.Colors.Count / Math.Max(multiplier * MaxColors, 1);
                    multiplier *= 2;
                }
            }

            return colorDictionary[proteinName];
        }

        /// <summary>
        /// Set new OxyColors for OxyColor palette.
        /// </summary>
        /// <param name="colors">The colors to create palette from.</param>
        /// <param name="numInterpolatedColors">The maximum number of colors for OxyPalette.</param>
        public void SetColors(IEnumerable<OxyColor> colors, int numInterpolatedColors = 5000)
        {
            var colorArr = colors.ToArray();
            colorDictionary.Clear();
            colorDictionary.Add(string.Empty, 0);
            protIndex = 1;
            offset = 0;
            multiplier = 2;
            OxyPalette = OxyPalette.Interpolate(numInterpolatedColors, colorArr);
            MaxColors = colorArr.Length;
        }
    }
}
