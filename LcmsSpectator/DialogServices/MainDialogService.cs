﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainDialogService.cs" company="Pacific Northwest National Laboratory">
//   2015 Pacific Northwest National Laboratory
// </copyright>
// <author>Christopher Wilkins</author>
// <summary>
//   Dialog services for opening LCMSSpectator dialog boxes from a view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LcmsSpectator.DialogServices
{
    using System;
    using LcmsSpectator.ViewModels;
    using LcmsSpectator.Views;
    
    /// <summary>
    /// Dialog services for opening LCMSSpectator dialog boxes from a view model.
    /// </summary>
    public class MainDialogService : DialogService, IMainDialogService
    {
        /// <summary>
        /// Open a dialog to search for a file on DMS.
        /// </summary>
        /// <param name="dmsLookupViewModel">The view model for the dialog.</param>
        /// <returns>The name of the data set and the name of the job selected.</returns>
        public Tuple<string, string> OpenDmsLookup(DmsLookupViewModel dmsLookupViewModel)
        {
            var dmsLookupDialog = new DmsLookupView { DataContext = dmsLookupViewModel };
            dmsLookupViewModel.ReadyToClose += (o, e) => dmsLookupDialog.Close();
            dmsLookupDialog.ShowDialog();
            Tuple<string, string> data = null;
            if (dmsLookupViewModel.Status)
            {
                var dataSetFolderPath = (dmsLookupViewModel.SelectedDataset == null) ? string.Empty : 
                                    dmsLookupViewModel.SelectedDataset.DatasetFolderPath;
                var jobFolderPath = (dmsLookupViewModel.SelectedJob == null) ? string.Empty : 
                                    dmsLookupViewModel.SelectedJob.JobFolderPath;
                data = new Tuple<string, string>(dataSetFolderPath, jobFolderPath);
            }

            return data;
        }

        /// <summary>
        /// Open a dialog to edit application settings.
        /// </summary>
        /// <param name="settingsViewModel">The view model for the dialog.</param>
        /// <returns>A value indicating whether the user clicked OK on the dialog.</returns>
        public bool OpenSettings(SettingsViewModel settingsViewModel)
        {
            var settingsDialog = new Settings { DataContext = settingsViewModel };
            settingsViewModel.ReadyToClose += (o, e) => settingsDialog.Close();
            settingsDialog.ShowDialog();
            return settingsViewModel.Status;
        }

        /// <summary>
        /// Open a dialog to edit heavy modification settings.
        /// </summary>
        /// <param name="heavyModificationsWindowVm">The view model for the dialog.</param>
        /// <returns>A value indicating whether the user clicked OK on the dialog.</returns>
        public bool OpenHeavyModifications(HeavyModificationsWindowViewModel heavyModificationsWindowVm)
        {
            var heavyModificationsDialog = new HeavyModificationsWindow { DataContext = heavyModificationsWindowVm };
            heavyModificationsWindowVm.ReadyToClose += (o, e) => heavyModificationsDialog.Close();
            heavyModificationsDialog.ShowDialog();
            return heavyModificationsWindowVm.Status;
        }

        /// <summary>
        /// Open a dialog to edit a modification.
        /// </summary>
        /// <param name="customModificationViewModel">The view model for the dialog.</param>
        /// <returns>A value indicating whether the user clicked OK on the dialog.</returns>
        public bool OpenCustomModification(CustomModificationViewModel customModificationViewModel)
        {
            var customModificationDialog = new CustomModificationView { DataContext = customModificationViewModel };
            customModificationViewModel.ReadyToClose += (o, e) => customModificationDialog.Close();
            customModificationDialog.ShowDialog();
            return customModificationViewModel.Status;
        }

        /// <summary>
        /// Open a dialog to manage modifications.
        /// </summary>
        /// <param name="manageModificationsViewModel">The view model for the dialog.</param>
        public void OpenManageModifications(ManageModificationsViewModel manageModificationsViewModel)
        {
            var manageModificationsDialog = new ManageModificationsWindow { DataContext = manageModificationsViewModel };
            manageModificationsDialog.ShowDialog();
        }

        /// <summary>
        /// Open a dialog to select a raw, id, and feature file path to open.
        /// </summary>
        /// <param name="openDataWindowViewModel">The view model for the dialog.</param>
        /// <returns>A value indicating whether the user clicked OK on the dialog.</returns>
        public bool OpenDataWindow(OpenDataWindowViewModel openDataWindowViewModel)
        {
            var openDataDialog = new OpenDataWindow { DataContext = openDataWindowViewModel };
            openDataWindowViewModel.ReadyToClose += (o, e) => openDataDialog.Close();
            openDataDialog.ShowDialog();
            return openDataWindowViewModel.Status;
        }

        /// <summary>
        /// Open a dialog to select a data set.
        /// </summary>
        /// <param name="selectDataViewModel">The view model for the dialog.</param>
        /// <returns>A value indicating whether the user clicked OK on the dialog.</returns>
        public bool OpenSelectDataWindow(SelectDataSetViewModel selectDataViewModel)
        {
            var selectDataDialog = new SelectDataSetView { DataContext = selectDataViewModel };
            selectDataViewModel.ReadyToClose += (o, e) => selectDataDialog.Close();
            selectDataDialog.ShowDialog();
            return selectDataViewModel.Status;
        }

        /// <summary>
        /// Open a dialog to select a filter value.
        /// </summary>
        /// <param name="filterViewModel">The view model for the dialog.</param>
        /// <returns>A value indicating whether the user clicked OK on the dialog.</returns>
        public bool FilterBox(FilterViewModel filterViewModel)
        {
            filterViewModel.ResetStatus();
            var filterDialog = new FilterView
            {
                DataContext = filterViewModel, 
                Title = filterViewModel.Title
            };
            filterViewModel.ReadyToClose += (o, e) => filterDialog.Close();
            filterDialog.ShowDialog();
            return filterViewModel.Status;
        }

        /// <summary>
        /// Open an About Box dialog.
        /// </summary>
        public void OpenAboutBox()
        {
            var dialog = new AboutBox();
            dialog.ShowDialog();
        }

        /// <summary>
        /// Open an Error Map window.
        /// </summary>
        /// <param name="errorMapViewModel">The view model for the dialog.</param>
        public void OpenErrorMapWindow(ErrorMapViewModel errorMapViewModel)
        {
            var errorMapWindow = new ErrorMapWindow { DataContext = errorMapViewModel, Height = 600, Width = 800 };
            errorMapWindow.Show();
        }
    }
}
