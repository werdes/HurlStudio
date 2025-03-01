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
        
        /// <summary>
        /// Renames a file by moving it to its new location
        /// </summary>
        /// <param name="fileContainer">The file to be renamed</param>
        /// <returns>true, if the file was renamed</returns>
        Task<bool> RenameFile(HurlFileContainer fileContainer);
        
        /// <summary>
        /// Renames a folder by moving it and its contents to a new location
        /// </summary>
        /// <param name="folderContainer">The folder to be renamed</param>
        /// <returns></returns>
        Task<bool> RenameFolder(HurlFolderContainer folderContainer);
        
        /// <summary>
        /// Renames a collection
        /// </summary>
        /// <param name="collectionContainer">The collection to be renamed</param>
        /// <returns></returns>
        Task<bool> RenameCollection(HurlCollectionContainer collectionContainer);   
        
        /// <summary>
        /// Opens the open documents of the previous session
        /// </summary>
        /// <returns></returns>
        Task OpenInitialDocuments();
        
        /// <summary>
        /// Loads the initial Usersettings for the editor view model
        /// </summary>
        /// <returns></returns>
        Task LoadInitialUserSettings();
        
        /// <summary>
        /// Adds a collection file to user settings 
        /// </summary>
        Task AddCollection();
        
        /// <summary>
        /// Opens a path in an appropriate view
        /// </summary>
        /// <param name="documentPath">The absolute path of the document to be opened</param>
        /// <returns></returns>
        Task OpenPathDocument(string documentPath);
        
        /// <summary>
        /// Opens a collection fileDocument as a document 
        /// </summary>
        /// <param name="fileLocation">The absolute path of the file to be opened</param>
        /// <param name="collectionLocation">The absolute path of the collection which contains the file (<paramref name="fileLocation"/>)</param>
        /// <returns></returns>
        Task OpenFileDocument(string fileLocation, string collectionLocation);
        
        /// <summary>
        /// Opens a file settings document by trying to find a corresponding collection
        /// </summary>
        /// <param name="fileLocation">The absolute path of the file to be opened</param>
        /// <returns></returns>
        Task OpenFileDocument(string fileLocation);
        

        /// <summary>
        /// Opens a folder settings document
        /// </summary>
        /// <param name="folderLocation">The absolute path of the folder to be opened</param>
        /// <param name="collectionLocation">The absolute path of the collection which contains the folder (<paramref name="folderLocation"/>)</param>
        /// <returns></returns>
        Task OpenFolderDocument(string folderLocation, string collectionLocation);
        
        /// <summary>
        /// Opens a folder settings document by trying to find a corresponding collection
        /// </summary>
        /// <param name="folderLocation">The absolute path of the folder to be opened</param>
        /// <returns></returns>
        Task OpenFolderDocument(string folderLocation);
        
        /// <summary>
        /// Opens a collection settings document
        /// </summary>
        /// <param name="collectionLocation">The absolute path of the collection to be opened</param>
        /// <returns></returns>
        Task OpenCollectionDocument(string collectionLocation);
        
        /// <summary>
        /// Opens a environment settings document
        /// </summary>
        /// <param name="environmentLocation">The absolute path of the environment to be opened</param>
        /// <returns></returns>
        Task OpenEnvironmentDocument(string environmentLocation);
        
        /// <summary>
        /// Close a file
        /// -> properly unbind all setting events
        /// </summary>
        /// <param name="fileDocument">The document to be closed</param>
        /// <returns>true, if the file was closed</returns>
        Task<bool> CloseFileDocument(FileDocumentViewModel? fileDocument);
        
        /// <summary>
        /// Saves the given file
        ///  > Save the .hurl doc
        ///  > Save the file specific settings in the collection
        /// </summary>
        /// <param name="fileDocument">The document to save</param>
        /// <returns>true, if the file was properly saved</returns>
        Task<bool> SaveFile(FileDocumentViewModel fileDocument);
        
        /// <summary>
        /// Saves the folder
        ///  > Save the folder specific settings in the collection
        /// </summary>
        /// <param name="folderDocument">The folder to save</param>
        /// <returns>true, if the folder was properly saved</returns>
        Task<bool> SaveFolder(FolderDocumentViewModel folderDocument);
        
        /// <summary>
        /// Saves a collection document
        /// </summary>
        /// <param name="collectionDocument">The collection to be saved</param>
        /// <returns>true, if the collection was properly saved</returns>
        Task<bool> SaveCollection(CollectionDocumentViewModel collectionDocument);
        
        /// <summary>
        /// Saves an environment document
        /// </summary>
        /// <param name="environmentDocument">The environment to be saved</param>
        /// <returns>true, if the environment was properly saved</returns>
        Task<bool> SaveEnvironment(EnvironmentDocumentViewModel environmentDocument);
        
        /// <summary>
        /// Saves the currently opened file
        /// </summary>
        /// <returns>true, if the opened document is either not saveable (no IEditor document) or if it was successfully saved</returns>
        Task<bool> SaveCurrentDocument();
        

        /// <summary>
        /// Called on shutdown of the application
        /// -> saves opened docs in UI state
        /// -> closes all documents
        /// </summary>
        /// <returns></returns>
        Task<bool> Shutdown();
        
        /// <summary>
        /// Deletes a file by moving it to system trash and removing its settings from the collection
        /// </summary>
        /// <param name="fileContainer">The file to be deleted</param>
        /// <returns>true, if the file was either deleted or moved to trash</returns>
        Task<bool> DeleteFile(HurlFileContainer fileContainer);
        
        /// <summary>
        /// Deletes a directory by moving it to system trash and removing its settings from the collection
        /// </summary>
        /// <param name="folderContainer">The folder to be deleted</param>
        /// <returns>true, if the folder was either deleted or moved to trash</returns>
        Task<bool> DeleteFolder(HurlFolderContainer folderContainer);
        
        /// <summary>
        /// Removes a collection from user settings
        /// </summary>
        /// <param name="collectionContainer">The collection to be removed</param>
        /// <returns>true, if the collection was removed from the users' settings</returns>
        Task<bool> RemoveCollection(HurlCollectionContainer collectionContainer);
        
        /// <summary>
        /// Removes an environment from environment directory
        /// </summary>
        /// <param name="environmentContainer">The environment to be deleted</param>
        /// <returns>true, if the environment was deleted (either permanently or to trash)</returns>
        Task<bool> DeleteEnvironment(HurlEnvironmentContainer environmentContainer);
        
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
        
        /// <summary>
        /// Creates a collection with dialog and refreshes the collection explorer
        /// </summary>
        /// <returns>true, if the collection was created</returns>
        Task<bool> CreateCollection();
        
        /// <summary>
        /// Creates an environment and refreshes the environment explorer
        /// </summary>
        /// <returns>true, if the environment was created</returns>
        Task<bool> CreateEnvironment();
        
        /// <summary>
        /// Initializes the editor view model
        /// </summary>
        Task Start();
        
        /// <summary>
        /// Sets the collections and re-binds their events
        /// </summary>
        Task RefreshCollectionExplorerCollections();
        
        /// <summary>
        /// Refreshes a single collection
        /// </summary>
        /// <param name="collectionLocation">The location of the collection to be refreshed</param>
        Task RefreshCollectionExplorerCollection(string collectionLocation);
        
        /// <summary>
        /// Sets the collections and re-binds their events
        /// </summary>
        Task RefreshEnvironmentExplorerEnvironments(string? activeEnvironmentLocation);
        
        /// <summary>
        /// Refreshes a given environment
        /// </summary>
        /// <param name="environmentLocation">The location of the environment to be refreshed</param>
        Task RefreshEnvironmentExplorerEnvironment(string environmentLocation);
        
        /// <summary>
        /// Move to previous dock tab
        /// </summary>
        Task HistoryGoBack();
        
        /// <summary>
        /// Move to next dock tab
        /// </summary>
        Task HistoryGoForward();
    }
}
