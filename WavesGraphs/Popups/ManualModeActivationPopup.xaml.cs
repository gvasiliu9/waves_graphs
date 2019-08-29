using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using WavesGraphs.Controls;
using WavesGraphs.Pages;
using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;

namespace WavesGraphs.Popups
{
    public partial class ManualModeActivationPopup : PopupPage
    {
        RoomView _roomView;

        public ManualModeActivationPopup(RoomView roomView)
        {
            InitializeComponent();

            InitializeEvents();

            _roomView = roomView;
        }

        #region Methods

        void InitializeEvents()
        {
            // Checkbox
            checkbox.CheckedChanged += Checkbox_CheckedChanged;

            // Cancel
            cancelButton.Clicked += CancelButton_Clicked;

            // Activate
            activateButton.Clicked += ActivateButton_Clicked;
        }

        #endregion

        #region Events

        private void Checkbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            //
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
        }

        private async void ActivateButton_Clicked(object sender, EventArgs e)
        {
            _roomView.ActivateManualMode = true;

            await Navigation.PopPopupAsync();
        }

        #endregion
    }
}
