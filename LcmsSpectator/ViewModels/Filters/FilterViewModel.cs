﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterViewModel.cs" company="Pacific Northwest National Laboratory">
//   2015 Pacific Northwest National Laboratory
// </copyright>
// <author>Christopher Wilkins</author>
// <summary>
//   A view model for selecting and validating a filter value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive;
using LcmsSpectator.DialogServices;
using ReactiveUI;

namespace LcmsSpectator.ViewModels.Filters
{
    /// <summary>
    /// A view model for selecting and validating a filter value.
    /// </summary>
    public class FilterViewModel : ReactiveObject, IFilter
    {
        /// <summary>
        /// Dialog service for opening dialogs from view model.
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// The function that filters a set of data.
        /// </summary>
        private readonly FilterFunction filter;

        /// <summary>
        /// The function that determines if the filter value is valid.
        /// </summary>
        private readonly Validate validator;

        /// <summary>
        /// A value indicating whether this item is selected
        /// </summary>
        private bool selected;

        /// <summary>
        /// Default constructor to support WPF design-time use
        /// </summary>
        [Obsolete("For WPF Design-time use only.", true)]
        public FilterViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LcmsSpectator.ViewModels.Filters.FilterViewModel"/> class.
        /// Initializes instance of the Filter
        /// </summary>
        /// <param name="name">The name of the filter.</param>
        /// <param name="title">The title of the filter.</param>
        /// <param name="description">The description text for the filter.</param>
        /// <param name="filter">The function that filters a set of data.</param>
        /// <param name="validator">Function that determines if the filter value is valid.</param>
        /// <param name="dialogService">Dialog service for opening dialogs from view model.</param>
        /// <param name="values">The default possible values to filter by.</param>
        /// <param name="defaultValue">The default value to filter by.</param>
        public FilterViewModel(
                               string name,
                               string title,
                               string description,
                               FilterFunction filter,
                               Validate validator,
                               IDialogService dialogService,
                               IEnumerable<string> values = null,
                               string defaultValue = "")
        {
            if (values == null)
            {
                values = new ReactiveList<string>();
            }

            Name = name;
            Title = title;
            Description = description;
            DefaultValues = values;
            this.filter = filter;
            this.validator = validator;
            FilterCommand = ReactiveCommand.Create(FilterImplementation);
            CancelCommand = ReactiveCommand.Create(CancelImplementation);
            Value = defaultValue;
            this.dialogService = dialogService;
            Status = false;
        }

        /// <summary>
        /// Delegate defining interface of function that filters a set of data.
        /// </summary>
        /// <param name="data">The data to filter.</param>
        /// <param name="value">The value of the filter.</param>
        /// <returns>The filtered data.</returns>
        public delegate IEnumerable<object> FilterFunction(IEnumerable<object> data, object value);

        /// <summary>
        /// Delegate defining interface of function that validates a filter value.
        /// </summary>
        /// <param name="value">Value of filter to validate.</param>
        /// <returns>A value indicating whether the filter is valid.</returns>
        public delegate bool Validate(object value);

        /// <summary>
        /// Event that is triggered when filter or cancel are executed.
        /// </summary>
        public event EventHandler ReadyToClose;

        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the title text of the filter.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the description text for the filter
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is selected
        /// </summary>
        public bool Selected
        {
            get => selected;
            set => this.RaiseAndSetIfChanged(ref selected, value);
        }

        /// <summary>
        /// Gets the default possible values to filter by.
        /// </summary>
        public IEnumerable<object> DefaultValues { get; }

        /// <summary>
        /// Gets or sets the selected value for the filter.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets a command that sets status to true if a valid filter has been selected
        /// and triggers the ReadyToClose event.
        /// </summary>
        public ReactiveCommand<Unit, Unit> FilterCommand { get; }

        /// <summary>
        /// Gets a command that sets status to false and triggers the ReadyToClose event.
        /// </summary>
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        /// <summary>
        /// Gets a value indicating whether a valid filter has been selected.
        /// </summary>
        public bool Status { get; private set; }

        /// <summary>
        /// Filter a collection of data.
        /// </summary>
        /// <param name="data">The data to filter.</param>
        /// <returns>The filtered data.</returns>
        public IEnumerable<object> Filter(IEnumerable<object> data)
        {
            return filter(data, Value);
        }

        /// <summary>
        /// Reset the status of this filter.
        /// </summary>
        public void ResetStatus()
        {
            Status = false;
        }

        /// <summary>
        /// Implementation of FilterCommand.
        /// Sets status to true if a valid filter has been selected
        /// and triggers the ReadyToClose event.
        /// </summary>
        private void FilterImplementation()
        {
            if (validator(Value))
            {
                Status = true;
                ReadyToClose?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                dialogService.MessageBox("Invalid filter value.");
            }
        }

        /// <summary>
        /// Implementation of CancelCommand.
        /// Sets status to false and triggers the ReadyToClose event.
        /// </summary>
        private void CancelImplementation()
        {
            Status = false;
            Selected = false;
            ReadyToClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
