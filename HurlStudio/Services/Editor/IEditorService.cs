using Dock.Model.Core;
using HurlStudio.Collections.Model.Collection;
using HurlStudio.Model.HurlContainers;
using HurlStudio.UI.ViewModels.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
        Task OpenFile(string fileLocation, string collectionLocation);
        Task OpenFile(string fileLocation);
        Task<bool> CloseFileDocument(FileDocumentViewModel? fileDocument);
        Task OpenFolderSettings(HurlFolderContainer folder);
        Task OpenCollectionSettings(HurlCollectionContainer collection);
        Task<bool> SaveFile(FileDocumentViewModel fileDocument);
        Task<bool> SaveCurrentFile();
        Task<bool> Shutdown();
    }
}
