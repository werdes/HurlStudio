using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace HurlStudio.Utility
{
    public static class StorageUtility
    {
        /// <summary>
        /// Opens a file selection dialog for a single file
        /// </summary>
        /// <param name="storageProvider">window storage provider</param>
        /// <param name="title">localized title of the selection dialog</param>
        /// <param name="allowedTypes"></param>
        /// <returns>A file path or null</returns>
        public static async Task<string?> DisplayOpenFilePickerSingle(IStorageProvider? storageProvider, string title,
            FilePickerFileType[] allowedTypes)
        {
            if (storageProvider == null) return null;
            if (!storageProvider.CanOpen) return null;

            FilePickerOpenOptions filePickerOpenOptions = new FilePickerOpenOptions
            {
                AllowMultiple = false,
                Title = title,
                FileTypeFilter = allowedTypes
            };
            IReadOnlyList<IStorageFile> files =
                await storageProvider.OpenFilePickerAsync(filePickerOpenOptions);

            if (files.Count == 1)
            {
                return files.First().Path.LocalPath;
            }

            return null;
        }

        /// <summary>
        /// Opens a file selection dialog for a multiple folders
        /// </summary>
        /// <param name="storageProvider">window storage provider</param>
        /// <param name="title">localized title of the selection dialog</param>
        /// <param name="allowedTypes"></param>
        /// <returns></returns>
        public static async Task<IReadOnlyList<IStorageFile>?> DisplayOpenFilePickerMulti(
            IStorageProvider? storageProvider, string title, FilePickerFileType[] allowedTypes)
        {
            if (storageProvider == null) return null;
            if (!storageProvider.CanOpen) return null;

            FilePickerOpenOptions filePickerOpenOptions = new FilePickerOpenOptions
            {
                AllowMultiple = false,
                Title = title,
                FileTypeFilter = allowedTypes
            };
            IReadOnlyList<IStorageFile> files =
                await storageProvider.OpenFilePickerAsync(filePickerOpenOptions);

            return files;
        }

        /// <summary>
        /// Opens a folder selection dialog for a single folder
        /// </summary>
        /// <param name="storageProvider">window storage provider</param>
        /// <param name="title">localized title of the selection dialog</param>
        /// <returns>full path of the selected folder, or null</returns>
        public static async Task<string?> DisplayOpenDirectoryPickerSingle(IStorageProvider? storageProvider,
            string title)
        {
            if (storageProvider == null) return null;
            if (!storageProvider.CanOpen) return null;

            FolderPickerOpenOptions folderPickerOpenOptions = new FolderPickerOpenOptions()
            {
                AllowMultiple = false,
                Title = title
            };

            IReadOnlyList<IStorageFolder> folders =
                await storageProvider.OpenFolderPickerAsync(folderPickerOpenOptions);

            return folders.Any() ? folders.First().Path.LocalPath : null;
        }

        /// <summary>
        /// Opens a file selection dialog for a single file
        /// </summary>
        /// <param name="storageProvider">window storage provider</param>
        /// <param name="title">localized title of the selection dialog</param>
        /// <param name="defaultExtension"></param>
        /// <param name="allowedTypes"></param>
        /// <returns>A file path or null</returns>
        public static async Task<string?> DisplaySaveFilePickerSingle(IStorageProvider? storageProvider, string title,
            string defaultExtension, FilePickerFileType[] allowedTypes)
        {
            if (storageProvider == null) return null;
            if (!storageProvider.CanSave) return null;

            FilePickerSaveOptions filePickerSaveOptions = new FilePickerSaveOptions
            {
                DefaultExtension = defaultExtension,
                FileTypeChoices = allowedTypes,
                Title = title,
                ShowOverwritePrompt = true
            };

            IStorageFile? file = await storageProvider.SaveFilePickerAsync(filePickerSaveOptions);
            return file?.Path.LocalPath;
        }
    }
}