using HurlStudio.Collections.Model;
using HurlStudio.Model.HurlContainers;
using HurlStudio.Model.HurlFileTemplates;
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
        Task AddCollection();
        Task OpenPathDocument(string documentPath);
        Task OpenFileDocument(string fileLocation, string collectionLocation);
        Task OpenFileDocument(string fileLocation);
        Task OpenFolderDocument(string folderLocation, string collectionLocation);
        Task OpenFolderDocument(string folderLocation);
        Task OpenCollectionDocument(string collectionLocation);
        Task OpenEnvironmentDocument(string environmentLocation);
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
        Task<bool> DeleteEnvironment(HurlEnvironmentContainer environmentContainer, bool deletePermanently);
        
        /// <summary>
        /// Create a new file from template in a collection root
        /// </summary>
        /// <param name="collectionContainer">The collection the file will be created in</param>
        /// <returns>true, if the file was created successfully</returns>
        Task<bool> CreateFile(HurlCollectionContainer collectionContainer);
        
        /// <summary>
        /// Creates a file from a template inside the given folder
        /// </summary>
        /// <param name="folderContainer">The folder the file will be created in</param>
        /// <returns>true, if the file was created successfully</returns>
        Task<bool> CreateFile(HurlFolderContainer folderContainer);
        
        /// <summary>
        /// Creates a folder in a collections' root directory 
        /// </summary>
        /// <param name="rootCollectionContainer">The collection in which the folder will be created</param>
        /// <returns>true, if the folder was successfully created</returns>
        Task<bool> CreateFolder(HurlCollectionContainer rootCollectionContainer);
        
        /// <summary>
        /// Creates a folder inside another folder 
        /// </summary>
        /// <param name="parentFolderContainer">The folder, in which the new folder will be created</param>
        /// <returns>true, if the folder was successfully created</returns>
        Task<bool> CreateFolder(HurlFolderContainer parentFolderContainer);
        Task<bool> CreateCollection();
        Task<bool> CreateEnvironment();
        Task Start();
        Task RefreshCollectionExplorerCollections();
        Task RefreshCollectionExplorerCollection(string collectionLocation);
        Task RefreshEnvironmentExplorerEnvironments(string? activeEnvironmentLocation);
        Task RefreshEnvironmentExplorerEnvironment(string environmentLocation);
        Task HistoryGoBack();
        Task HistoryGoForward();
    }
}
