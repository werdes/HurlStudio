using HurlStudio.Model.HurlContainers;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Documents;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public interface IEditorService
    {
        Task<bool> MoveFileToFolder(HurlFileContainer fileContainer, HurlFolderContainer folderContainer);
        Task<bool> MoveFileToCollection(HurlFileContainer fileContainer, HurlFolderContainer newParentFolderContainer, HurlCollectionContainer collectionContainer);
        Task<bool> MoveFileToCollectionRoot(HurlFileContainer fileContainer, HurlCollectionContainer collectionContainer);
        Task<bool> MoveFolderToCollection(HurlFolderContainer folderContainer, HurlFolderContainer parentFolderContainer, HurlCollectionContainer collectionContainer);
        Task<bool> MoveFolderToFolder(HurlFolderContainer folderContainer, HurlFolderContainer parentFolderContainer);
        Task<bool> MoveFolderToCollectionRoot(HurlFolderContainer folderContainer, HurlCollectionContainer collectionContainer);
        Task<bool> RenameFile(HurlFileContainer fileContainer, string newFileName);
        Task<bool> RenameFolder(HurlFolderContainer folderContainer, string newFolderName);
        Task<bool> RenameCollection(HurlCollectionContainer collectionContainer, string newCollectionName, bool moveFile);   
        Task OpenInitialDocuments();
        Task LoadInitialUserSettings();
        Task OpenPath(string documentPath);
        Task OpenFile(string fileLocation, string collectionLocation);
        Task OpenFile(string fileLocation);
        Task OpenFolder(string folderLocation, string collectionLocation);
        Task OpenFolder(string folderLocation);
        Task OpenCollection(string collectionLocation);
        Task OpenEnvironment(string environmentLocation);
        Task<bool> CloseFileDocument(FileDocumentViewModel? fileDocument);
        Task<bool> SaveFile(FileDocumentViewModel fileDocument);
        Task<bool> SaveFolder(FolderDocumentViewModel folderDocument);
        Task<bool> SaveCollection(CollectionDocumentViewModel collectionDocument);
        Task<bool> SaveEnvironment(EnvironmentDocumentViewModel environmentDocument);
        Task<bool> SaveCurrentDocument();
        Task<bool> Shutdown();
        Task<bool> DeleteFile(HurlFileContainer fileContainer, bool deletePermanently);
        Task<bool> DeleteFolder(HurlFolderContainer folderContainer, bool deletePermanently);
        Task<bool> RemoveCollection(HurlCollectionContainer collectionContainer);
        Task<bool> AddFile(HurlFolderContainer parentFolder, string content);
        Task Start();
        Task RefreshCollectionExplorerCollections();
        Task RefreshCollectionExplorerCollection(string collectionLocation);
        Task RefreshEnvironmentExplorerEnvironments(string? activeEnvironmentLocation);
        Task RefreshEnvironmentExplorerEnvironment(string environmentLocation);
        Task HistoryGoBack();
        Task HistoryGoForward();
    }
}
