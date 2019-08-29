using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace WavesGraphs.Controls
{
    public partial class RoomTitle : ContentView
    {
        #region Bindable Properties

        public static readonly BindableProperty TitleProperty = BindableProperty
            .Create(nameof(Title), typeof(string), typeof(RoomTitle), default(string));

        public string Title
        {
            get => (string)GetValue(TitleProperty);

            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty IconProperty = BindableProperty
            .Create(nameof(Icon), typeof(string), typeof(RoomTitle), default(string));

        public string Icon
        {
            get => (string)GetValue(IconProperty);

            set => SetValue(IconProperty, value);
        }

        public static readonly BindableProperty ProfileIconProperty = BindableProperty
            .Create(nameof(ProfileIcon), typeof(string), typeof(RoomTitle), default(string));

        public string ProfileIcon
        {
            get => (string)GetValue(ProfileIconProperty);

            set => SetValue(ProfileIconProperty, value);
        }

        #endregion

        #region Commands

        public static readonly BindableProperty ShowMenuCommandProperty = BindableProperty
            .Create(nameof(ShowMenuCommand), typeof(ICommand), typeof(RoomTitle));

        public ICommand ShowMenuCommand
        {
            get => (ICommand)GetValue(ShowMenuCommandProperty);
            set => SetValue(ShowMenuCommandProperty, value);
        }

        public static readonly BindableProperty ShowProfileModalCommandProperty = BindableProperty
            .Create(nameof(ShowProfileModalCommand), typeof(ICommand), typeof(RoomTitle));

        public ICommand ShowProfileModalCommand
        {
            get => (ICommand)GetValue(ShowProfileModalCommandProperty);
            set => SetValue(ShowProfileModalCommandProperty, value);
        }

        #endregion

        public RoomTitle()
        {
            InitializeComponent();

            InitializeCommands();
        }

        void InitializeCommands()
        {
            // Show menu
            menu.TapCommand = new Command(() =>
            {
                if (ShowMenuCommand != null && ShowMenuCommand.CanExecute(null))
                    ShowMenuCommand.Execute(null);
            });

            // Set profile
            profile.TapCommand = new Command(() =>
            {
                if (ShowProfileModalCommand != null && ShowProfileModalCommand.CanExecute(null))
                    ShowProfileModalCommand.Execute(null);
            });
        }

        #region Events

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Icon
            if (propertyName == IconProperty.PropertyName)
            {
                icon.Text = Icon;
            }

            // Title
            if (propertyName == TitleProperty.PropertyName)
            {
                title.Text = Title;
            }

            // Profile
            if (propertyName == ProfileIconProperty.PropertyName)
            {
                profile.Text = ProfileIcon;
            }
        }

        #endregion
    }
}
