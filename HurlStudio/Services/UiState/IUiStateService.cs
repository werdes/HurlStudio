
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.UiState
{
    public interface IUiStateService
    {
        Task<Model.UiState.UiState?> GetUiStateAsync(bool refresh);
        Model.UiState.UiState? GetUiState(bool refresh);
        Task StoreUiStateAsync();
        void StoreUiState();
        void SetCollectionExplorerState(EditorViewViewModel editorView);
        void SetMainWindowState(MainWindow mainWindow);
        void SetFileHistory(EditorViewViewModel editorView);
        void SetSettingCollapsedState(string id, bool visible);
        void SetSettingSectionCollapsedState(string id, bool visible);
        void SetSettingEnabledState(string id, bool visible);
    }
}
