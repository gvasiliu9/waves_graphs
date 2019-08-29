using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace WavesGraphs.Modals
{
    public partial class ManualModeModal : ContentView
    {
        #region Commands

        public static readonly BindableProperty CloseCommandProperty = BindableProperty
            .Create(nameof(CloseCommand), typeof(ICommand), typeof(ManualModeModal));

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public static readonly BindableProperty InfoCommandProperty = BindableProperty
            .Create(nameof(InfoCommand), typeof(ICommand), typeof(ManualModeModal));

        public ICommand InfoCommand
        {
            get => (ICommand)GetValue(InfoCommandProperty);
            set => SetValue(InfoCommandProperty, value);
        }

        #endregion

        public ManualModeModal()
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

            // Action
            header.ActionCommand = new Command(() =>
            {
                if (InfoCommand != null && InfoCommand.CanExecute(null))
                    InfoCommand.Execute(null);
            });
        }

        #endregion
    }
}
