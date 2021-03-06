﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorViewModel.cs" company="Pacific Northwest National Laboratory">
//   2015 Pacific Northwest National Laboratory
// </copyright>
// <author>Christopher Wilkins</author>
// <summary>
//   View model for single color.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reactive;
using System.Windows.Media;
using ReactiveUI;

namespace LcmsSpectator.ViewModels
{
    /// <summary>
    /// View model for single color.
    /// </summary>
    public class ColorViewModel : ReactiveObject
    {
        /// <summary>
        /// The selected color.
        /// </summary>
        private Color selectedColor;

        /// <summary>
        /// A value indicating whether this color view model should be removed.
        /// </summary>
        private bool isRemoveRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorViewModel"/> class.
        /// </summary>
        public ColorViewModel()
        {
            SelectedColor = new Color { A = 255, R = 0, G = 0, B = 0 };

            RemoveCommand = ReactiveCommand.Create(() => IsRemoveRequested = true);
        }

        /// <summary>
        /// Gets or sets the selected color.
        /// </summary>
        public Color SelectedColor
        {
            get => selectedColor;
            set => this.RaiseAndSetIfChanged(ref selectedColor, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this color view model should be removed.
        /// </summary>
        public bool IsRemoveRequested
        {
            get => isRemoveRequested;
            set => this.RaiseAndSetIfChanged(ref isRemoveRequested, value);
        }

        /// <summary>
        /// Gets a command for removing the color.
        /// </summary>
        public ReactiveCommand<Unit, bool> RemoveCommand { get; }
    }
}
