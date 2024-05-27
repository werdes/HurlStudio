using HurlStudio.Model.HurlContainers;
using HurlStudio.UI.ViewModels;
using HurlStudio.UI.ViewModels.Documents;
using System.Threading.Tasks;

namespace HurlStudio.Services.Editor
{
    public interface IEditorService
    {
        Task<bool> MoveFileToFolder(HurlFileContainer collectionFile, HurlFolderContainer folder);
        Task<bool> MoveFileToCollection(HurlFileContainer collectionFile, HurlFolderContainer parentFolder, HurlCollectionContainer collection);
        Task<bool> MoveFileToCollectionRoot(HurlFileContainer collectionFile, HurlCollectionContainer collection);
        Task<bool> MoveFolderToCollection(HurlFolderContainer folder, HurlFolderContainer parentFolder, HurlCollectionContainer collection);
        Task<bool> MoveFolderToFolder(HurlFolderContainer folder, HurlFolderContainer parentFolder);
        Task<bool> MoveFolderToCollectionRoot(HurlFolderContainer folder, HurlCollectionContainer collection);
        Task OpenInitialDocuments();
        Task LoadInitialUserSettings();
        Task OpenPath(string documentPath);
        Task OpenFile(string fileLocation, string collectionLocation);
        Task OpenFile(string fileLocation);
        Task OpenFolder(string folderLocation, string collectionLocation);
        Task OpenFolder(string folderLocation);
        Task OpenCollection(string collectionLocation);
        Task<bool> CloseFileDocument(FileDocumentViewModel? fileDocument);
        Task<bool> SaveFile(FileDocumentViewModel fileDocument);
        Task<bool> SaveFolder(FolderDocumentViewModel folderDocument);
        Task<bool> SaveCollection(CollectionDocumentViewModel collectionDocument);
        Task<bool> SaveCurrentDocument();
        Task<bool> Shutdown();
        Task RefreshCollectionExplorerCollections();
    }
}
