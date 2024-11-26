using HurlStudio.Collections.Model;
using HurlStudio.Model.HurlFileTemplates;
using HurlStudio.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels.Windows
{
    public class AddCollectionWindowViewModel : ViewModelBase
    {
        private AddCollectionViewViewModel? _addCollectionViewViewModel;
        private HurlCollection? _collection;

        public AddCollectionWindowViewModel() : base(typeof(AddCollectionWindow))
        {
        }

        public AddCollectionViewViewModel? AddCollectionViewViewModel
        {
            get => _addCollectionViewViewModel;
            set
            {
                _addCollectionViewViewModel = value;
                this.Notify();
            }
        }

        public HurlCollection? Collection
        {
            get => _collection;
            set
            {
                _collection = value;
                this.Notify();
            }
        }
    }
}
