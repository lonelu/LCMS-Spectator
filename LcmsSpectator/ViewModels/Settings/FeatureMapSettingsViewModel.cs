﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LcmsSpectator.Config;
using LcmsSpectator.ViewModels.Colors;

namespace LcmsSpectator.ViewModels.Settings
{
    public class FeatureMapSettingsViewModel
    {
        public FeatureMapSettingsViewModel(FeatureMapSettings featureMapSettings)
        {
            this.FeatureColors = new ColorListViewModel();
            this.FeatureColors.ColorViewModels.AddRange(featureMapSettings.FeatureColors.Select(c => new ColorViewModel { SelectedColor = c }));

            this.IdColors = new ColorListViewModel();
            this.FeatureColors.ColorViewModels.AddRange(featureMapSettings.IdColors.Select(c => new ColorViewModel { SelectedColor = c }));

            this.Ms2ScanColor = featureMapSettings.Ms2ScanColor;
        }

        /// <summary>
        /// Gets the feature map settings for this view model.
        /// </summary>
        public FeatureMapSettings FeatureMapSettings
        {
            get
            {
                return new FeatureMapSettings
                {
                    FeatureColors = this.FeatureColors.ColorViewModels.Select(cvm => cvm.SelectedColor).ToList(),
                    IdColors = this.IdColors.ColorViewModels.Select(cvm => cvm.SelectedColor).ToList(),
                    Ms2ScanColor = this.Ms2ScanColor
                };
            }
        }

        /// <summary>
        /// Gets the view model for the feature color list.
        /// </summary>
        public ColorListViewModel FeatureColors { get; private set; }

        /// <summary>
        /// Gets the view model for the id color list.
        /// </summary>
        public ColorListViewModel IdColors { get; private set; }

        /// <summary>
        /// Gets or sets the color for the MS/MS scans on the feature map.
        /// </summary>
        public Color Ms2ScanColor { get; set; }
    }
}
