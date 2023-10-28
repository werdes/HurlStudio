using HurlUI.Collections.Model.Collection;
using HurlUI.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.UI.ViewModels
{
    public class EditorViewViewModel : ViewModelBase
    {
        private ObservableCollection<HurlCollection> _collections;

        public ObservableCollection<HurlCollection> Collections
        {
            get => _collections;
            set
            {
                _collections = value;
                Notify();
            }
        }


        public EditorViewViewModel() : base(typeof(EditorView))
        {

        }
    }
}
