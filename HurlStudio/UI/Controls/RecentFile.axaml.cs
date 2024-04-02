using Avalonia.Controls;
using HurlStudio.Model.UiState;
using HurlStudio.Services.Editor;
using Microsoft.Extensions.Logging;
using System;

namespace HurlStudio.UI.Controls
{
    public partial class RecentFile : ViewModelBasedControl<FileHistoryEntry>
    {
        private FileHistoryEntry? _viewModel;
        private ILogger _log;
        private IEditorService _editorService;

        public RecentFile(ILogger<RecentFile> logger, IEditorService editorService)
        {
            _editorService = editorService;
            _log = logger;

            this.InitializeComponent();
        }

        protected override void SetViewModelInstance(FileHistoryEntry viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Open the file in a new tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void On_RecentFile_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (_viewModel == null) return;
            if (!e.Pointer.IsPrimary) return;
            
            try
            {
                await _editorService.OpenFile(_viewModel.Location);
            }
            catch(Exception ex)
            {
                _log.LogCritical(ex, nameof(this.On_RecentFile_PointerPressed));
            }
        }
    }
}
