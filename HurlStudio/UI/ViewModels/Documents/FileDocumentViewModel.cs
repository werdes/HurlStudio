using AvaloniaEdit.Document;
using Dock.Model.Mvvm.Controls;
using HurlStudio.Common.UI;
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

        private OrderedObservableCollection<HurlSettingSection> _settingSections;

        public FileDocumentViewModel(EditorViewViewModel editorViewViewModel)
        {
            this.CanFloat = false;
            this.CanPin = false;

            _editorViewViewModel = editorViewViewModel;
            _settingSections = new OrderedObservableCollection<HurlSettingSection>();
        }

        public CollectionFile? File
        {
            get => _file;
            set
            {
                this.SetProperty(ref _file, value);
                if (_file != null)
                {
                    _file.PropertyChanged -= this.On_File_PropertyChanged;
                    _file.PropertyChanged += this.On_File_PropertyChanged;
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
                this.Notify();
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
                    _document.TextChanged -= this.On_Document_TextChanged;
                    _document.TextChanged += this.On_Document_TextChanged;
                }

                this.Notify();
            }
        }

        public OrderedObservableCollection<HurlSettingSection> SettingSections
        {
            get => _settingSections;
            set
            {
                _settingSections = value;
                this.Notify();
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

            this.Notify(nameof(this.Title));
        }

        private void On_File_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.File.Location))
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
