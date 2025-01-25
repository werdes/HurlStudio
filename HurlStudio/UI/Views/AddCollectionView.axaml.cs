using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using HurlStudio.Collections.Settings;
using HurlStudio.Common;
using HurlStudio.Common.Extensions;
using HurlStudio.Extensions;
using HurlStudio.Model.Enums;
using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.Model.HurlSettings;
using HurlStudio.Model.UserSettings;
using HurlStudio.Services.Editor;
using HurlStudio.Services.HurlFileTemplates;
using HurlStudio.Services.Notifications;
using HurlStudio.Services.UserSettings;
using HurlStudio.UI.Editor;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Windows;
using HurlStudio.UI.Windows;
using HurlStudio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using HurlStudio.Common.Utility;

namespace HurlStudio.UI.Views
{
    public partial class AddCollectionView : ViewBase<AddCollectionViewViewModel>
    {
        private ILogger _log;
        private IConfiguration _configuration;
        private IUserSettingsService _userSettingsService;
        private IEditorService _editorService;
        private INotificationService _notificationService;
        private IHurlFileTemplateService _templateService;
        private ServiceManager<Windows.WindowBase> _windowBuilder;

        public AddCollectionView(AddCollectionViewViewModel viewModel, ILogger<AddCollectionView> logger, IConfiguration configuration, IUserSettingsService userSettingsService, IEditorService editorService, ControlLocator controlLocator, INotificationService notificationService, IHurlFileTemplateService templateService, ServiceManager<Windows.WindowBase> windowBuilder) : base(viewModel, controlLocator)
        {
            _log = logger;
            _configuration = configuration;
            _userSettingsService = userSettingsService;
            _editorService = editorService;
            _notificationService = notificationService;
            _templateService = templateService;
            _windowBuilder = windowBuilder;

            this.InitializeComponent();
        }

        /// <summary>
        /// On View loaded
        /// -> focus search textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_AddCollectionView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Load available template list on initilization and set up editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_AddCollectionView_Initialized(object? sender, System.EventArgs e)
        {
            if (_viewModel == null) return;
            if (_window == null) return;

            try
            {
                // Pull template in from window
                if (_window.GetViewModel() is not AddCollectionWindowViewModel windowViewModel) return;
                _viewModel.Collection = windowViewModel.Collection;
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Adds an empty row to the collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonAddAdditionalLocation_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            if (_viewModel.Collection == null) return;

            try
            {
                _viewModel.Collection.AdditionalLocations.Add(new Collections.Model.Containers.AdditionalLocation(string.Empty, _viewModel.Collection));
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }

        /// <summary>
        /// Sets the collection file path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonSetFileLocation_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_window == null) return;
            if (_viewModel == null) return;
            if (_viewModel.Collection == null) return;

            try
            {
                string? targetPath = await StorageUtility.DisplaySaveFilePickerSingle(_window.StorageProvider, Localization.Localization.View_AddCollectionView_Properties_Path_FilePicker_Title, GlobalConstants.COLLECTION_FILE_EXTENSION, [new FilePickerFileType(Localization.Localization.View_AddCollectionView_Properties_Path_FilePicker_FileType_Name) {
                    Patterns = new[] { $"*{GlobalConstants.COLLECTION_FILE_EXTENSION}" }
                }]);
                if (targetPath == null) return;

                _viewModel.Collection.CollectionFileLocation = targetPath;
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
        }


        /// <summary>
        /// Close the window with the template container attached
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Button_Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            if (_viewModel.Collection == null) return;
            if (_window == null) return;

            try
            {
                if (string.IsNullOrWhiteSpace(_viewModel.Collection.Name)) return;
                if (string.IsNullOrWhiteSpace(_viewModel.Collection.CollectionFileLocation)) return;

                _window.Close(_viewModel.Collection);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }

        /// <summary>
        /// Close the window without attachment -> discard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Button_Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_window == null) return;

            try
            {
                _window.Close(null);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(ex);
                _log.LogException(ex);
            }
        }


        public override void SetWindow(Windows.WindowBase window)
        {
            base.SetWindow(window);
            _controlLocator.Window = window;
        }
    }
}