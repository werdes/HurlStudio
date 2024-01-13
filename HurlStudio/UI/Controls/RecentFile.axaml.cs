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

            InitializeComponent();
        }

        protected override void SetViewModelInstance(FileHistoryEntry viewModel)
        {
            _viewModel = viewModel;
            this.DataContext = viewModel;
        }

        private void On_RecentFile_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            if (_viewModel == null) return;
            try
            {
                _editorService.OpenFile(_viewModel.Location);
            }
            catch(Exception ex)
            {
                _log.LogCritical(ex, nameof(On_RecentFile_PointerPressed));
            }
        }
    }
}
