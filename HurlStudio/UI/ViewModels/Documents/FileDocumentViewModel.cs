using AvaloniaEdit.Document;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.CollectionContainer;
using HurlStudio.Model.HurlSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels.Documents
{
    public class FileDocumentViewModel : DocumentBase
    {
        private CollectionFile? _file;
        private EditorViewViewModel _editorViewViewModel;
        private TextDocument? _document;

        private ObservableCollection<HurlSettingContainer> _collectionSettingContainers;
        private ObservableCollection<HurlSettingContainer> _fileSettingContainers;
        private ObservableCollection<FolderSettingContainer> _folderSettingContainers;
        private ObservableCollection<HurlSettingContainer> _environmentSettingContainers;

        public FileDocumentViewModel(EditorViewViewModel editorViewViewModel)
        {
            CanFloat = false;
            CanPin = false;
                       

            _editorViewViewModel = editorViewViewModel;

            _collectionSettingContainers = new ObservableCollection<HurlSettingContainer>();
            _fileSettingContainers = new ObservableCollection<HurlSettingContainer>();
            _folderSettingContainers = new ObservableCollection<FolderSettingContainer>();
            _environmentSettingContainers = new ObservableCollection<HurlSettingContainer>();
        }

        public CollectionFile? File
        {
            get => _file;
            set
            {
                this.SetProperty(ref _file, value);
                if (_file != null)
                {
                    _file.PropertyChanged -= On_File_PropertyChanged;
                    _file.PropertyChanged += On_File_PropertyChanged;
                }

                this.ChangeTitle();
            }
        }

        public EditorViewViewModel EditorViewViewModel
        {
            get => _editorViewViewModel;
            set
            {
                _editorViewViewModel = value;
                Notify();
            }
        }

        public TextDocument? Document
        {
            get => _document;
            set
            {
                _document = value;
                if(_document != null)
                {
                    _document.TextChanged -= On_Document_TextChanged;
                    _document.TextChanged += On_Document_TextChanged;
                }

                Notify();
            }
        }

        public ObservableCollection<HurlSettingContainer> CollectionSettingContainers
        {
            get => _collectionSettingContainers;
            set
            {
                _collectionSettingContainers = value;
                Notify();
            }
        }

        public ObservableCollection<HurlSettingContainer> FileSettingContainers
        {
            get => _fileSettingContainers;
            set
            {
                _fileSettingContainers = value;
                Notify();
            }
        }

        public ObservableCollection<FolderSettingContainer> FolderSettingContainers
        {
            get => _folderSettingContainers;
            set
            {
                _folderSettingContainers = value;
                Notify();
            }
        }

        public ObservableCollection<HurlSettingContainer> EnvironmentSettingContainers
        {
            get => _environmentSettingContainers;
            set
            {
                _environmentSettingContainers = value;
                Notify();
            }
        }

        private void ChangeTitle()
        {
            if (_file != null)
            {
                this.Title = Path.GetFileName(_file.Location);
            }
            else
            {
                this.Title = Localization.Localization.Common_Undefined;
            }
            Notify(nameof(Title));
        }

        private void On_File_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(File.Location))
            {
                this.ChangeTitle();
            }
        }

        /// <summary>
        /// On Document text change
        /// -> Reevaluate Redo/Undo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_Document_TextChanged(object? sender, EventArgs e)
        {
            if(_editorViewViewModel != null && _document != null)
            {
                _editorViewViewModel.CanUndo = _document.UndoStack.CanUndo;
                _editorViewViewModel.CanRedo = _document.UndoStack.CanRedo;
            }
        }

    }
}
