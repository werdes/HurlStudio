
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
        void SetCollectionExplorerCollapseState(string id, bool collapsed);
        void SetMainWindowState(MainWindow mainWindow);
        void SetFileHistory(EditorViewViewModel editorView);
        void SetSettingCollapsedState(string id, bool visible);
        void RemoveSettingCollapsedState(string id);
        void SetSettingSectionCollapsedState(string id, bool visible);
        void RemoveSettingSectionCollapsedState(string id);
        void SetOpenedDocuments(List<string> openedDocuments);
        void SetActiveDocument(EditorViewViewModel editorViewViewModel);
        void SetCollectionExplorerProportion(double? proportion);
        void SetActiveEnvironment(string environmentFile);
        void SetSettingEnabledState(string id, bool isEnabled);
        void RemoveSettingEnabledState(string id);
    }
}
