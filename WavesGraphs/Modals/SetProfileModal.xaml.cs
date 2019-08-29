using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using static WavesGraphs.Models.Dashboard.Enums;

namespace WavesGraphs.Modals
{
    public partial class SetProfileModal : ContentView
    {
        #region Commands

        public static readonly BindableProperty CloseCommandProperty = BindableProperty
            .Create(nameof(CloseCommand), typeof(ICommand), typeof(ManualModeModal));

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public static readonly BindableProperty SetProfileCommandProperty = BindableProperty
            .Create(nameof(SetProfileCommand), typeof(ICommand), typeof(SetProfileModal));

        public ICommand SetProfileCommand
        {
            get => (ICommand)GetValue(SetProfileCommandProperty);
            set => SetValue(SetProfileCommandProperty, value);
        }

        #endregion

        public SetProfileModal()
        {
            InitializeComponent();

            InitializeCommands();
        }

        #region Methods

        void InitializeCommands()
        {
            // Close
            header.CloseCommand = new Command(() =>
            {
                if (CloseCommand != null && CloseCommand.CanExecute(null))
                    CloseCommand.Execute(null);
            });

            // Set profile command
            header.ActionCommand = new Command(() =>
            {
                Profile profile = Profile.Health;

                if (SetProfileCommand != null && SetProfileCommand.CanExecute(profile))
                    SetProfileCommand.Execute(profile);
            });
        }

        #endregion
    }
}