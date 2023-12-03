using Dock.Model.Mvvm.Controls;
using HurlStudio.Model.CollectionContainer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels.Documents
{
    public class FileDocumentViewModel : DocumentBase
    {
        private CollectionFile? _file;

        public FileDocumentViewModel()
        {
            CanFloat = false;
            CanPin = false;
        }

        public CollectionFile? File
        {
            get => _file;
            set
            {
                this.SetProperty(ref _file, value);
                if(_file != null)
                {
                    _file.PropertyChanged += On_File_PropertyChanged;

                    this.Title = Path.GetFileName(_file.Location);
                    Notify(nameof(Title));
                }
            }
        }

        private void On_File_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(File.Location) && _file != null)
            {
                this.Title = Path.GetFileName(_file.Location);
                Notify(nameof(Title));
            }
        }
    }
}
