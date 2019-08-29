using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace WavesGraphs.Controls
{
    public partial class ModalHeader : ContentView
    {
        #region Bindable Properties

        public static readonly BindableProperty TitleProperty = BindableProperty
            .Create(nameof(Title), typeof(string), typeof(ModalHeader), default(string));

        public string Title
        {
            get => (string)GetValue(TitleProperty);

            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty ActionIconProperty = BindableProperty
            .Create(nameof(ActionIcon), typeof(string), typeof(ModalHeader), default(string));

        public string ActionIcon
        {
            get => (string)GetValue(ActionIconProperty);

            set => SetValue(ActionIconProperty, value);
        }

        #endregion

        #region Commands

        public static readonly BindableProperty ActionCommandProperty = BindableProperty
            .Create(nameof(ActionCommand), typeof(ICommand), typeof(ModalHeader));

        public ICommand ActionCommand
        {
            get => (ICommand)GetValue(ActionCommandProperty);
            set => SetValue(ActionCommandProperty, value);
        }

        public static readonly BindableProperty CloseCommandProperty = BindableProperty
            .Create(nameof(CloseCommand), typeof(ICommand), typeof(ModalHeader));

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        #endregion

        public ModalHeader()
        {
            InitializeComponent();

            InitializeCommands();
        }

        #region Methods

        void InitializeCommands()
        {
            // Close
            close.TapCommand = new Command(() =>
            {
                if (CloseCommand != null && CloseCommand.CanExecute(null))
                    CloseCommand.Execute(null);
            });

            // Action
            action.TapCommand = new Command(() =>
            {
                if (ActionCommand != null && ActionCommand.CanExecute(null))
                    ActionCommand.Execute(null);
            });
        }

        #endregion

        #region Events

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Title
            if (propertyName == TitleProperty.PropertyName)
            {
                title.Text = Title;
            }

            // ActionIcon
            if (propertyName == ActionIconProperty.PropertyName)
            {
                action.Text = ActionIcon;
            }
        }

        #endregion
    }
}
