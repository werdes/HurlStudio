using Avalonia.Controls;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.EventArgs;
using HurlStudio.Services.Editor;
using HurlStudio.Services.Notifications;
using HurlStudio.UI.Dock;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Tools;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using HurlStudio.Common.Extensions;
using HurlStudio.Utility;
namespace HurlStudio.UI.Controls.Tools
{
    public partial class EnvironmentExplorerTool : ViewModelBasedControl<EnvironmentExplorerToolViewModel>
    {
        private ILogger _log;
        private EditorViewViewModel _editorViewViewModel;
        private EnvironmentExplorerToolViewModel? _viewModel;
        private IEditorService _editorService;
        private INotificationService _notificationService;
        private ICollectionService _collectionService;

        public EnvironmentExplorerTool(ILogger<CollectionExplorerTool> logger, EditorViewViewModel editorViewViewModel, IEditorService editorService, INotificationService notificationService, ICollectionService collectionService)
        {
            this.InitializeComponent();
            _log = logger;
            _editorViewViewModel = editorViewViewModel;
            _editorService = editorService;
            _notificationService = notificationService;
            _collectionService = collectionService;
            _editorViewViewModel.Environments.CollectionChanged += this.On_Environments_CollectionChanged;
        }

        protected override void SetViewModelInstance(EnvironmentExplorerToolViewModel viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// On Environment collection change -> bind new items to local handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Environments_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (HurlEnvironmentContainer environment in e.NewItems)
                {
                    this.BindCollectionEvents(environment);
                }
            }
        }

        /// <summary>
        /// OnInitialized
        /// Bind the SelectionChanged event to a handler that propagates the event to all other collections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_EnvironmentExplorerTool_Initialized(object? sender, EventArgs e)
        {
            foreach (HurlEnvironmentContainer environmentContainer in _editorViewViewModel.Environments)
            {
                this.BindCollectionEvents(environmentContainer);
            }
        }

        /// <summary>
        /// Binds a environments' events to local handlers
        /// </summary>
        /// <param name="environmentContainer"></param>
        private void BindCollectionEvents(HurlEnvironmentContainer environmentContainer)
        {
            environmentContainer.ControlSelectionChanged -= this.On_EnvironmentContainer_ControlSelectionChanged;
            environmentContainer.ControlSelectionChanged += this.On_EnvironmentContainer_ControlSelectionChanged;
        }

        /// <summary>
        /// Propagate the unselect event through all environments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_EnvironmentContainer_ControlSelectionChanged(object? sender, ControlSelectionChangedEventArgs e)
        {
            foreach (HurlEnvironmentContainer environmentContainer in _editorViewViewModel.Environments)
            {
                environmentContainer.Unselect();
            }
        }

        /// <summary>
        /// Refresh the Environments
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_ButtonRefresh_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_viewModel == null) return;
            try
            {
                await _editorService.RefreshEnvironmentExplorerEnvironments(_editorViewViewModel.ActiveEnvironment?.EnvironmentFileLocation);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                _notificationService.Notify(ex);
            }
            finally
            {
                _viewModel.IsEnabled = true;
            }
        }
    }
}
