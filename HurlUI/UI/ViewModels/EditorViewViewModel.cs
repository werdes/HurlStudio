using Dock.Model.Controls;
using HurlUI.Collections.Model.Collection;
using HurlUI.Collections.Model.Environment;
using HurlUI.UI.Dock;
using HurlUI.UI.Views;
using Microsoft.Extensions.Logging;
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
        private ILogger _log = null;

        private ObservableCollection<HurlCollection> _collections;
        private ObservableCollection<HurlEnvironment> _environments;
        private IRootDock? _layout;


        public EditorViewViewModel(LayoutFactory layoutFactory, ILogger<EditorViewViewModel> logger) : base(typeof(EditorView))
        {
            this._collections = new ObservableCollection<HurlCollection>();
            this._environments = new ObservableCollection<HurlEnvironment>();

            this._log = logger;
            
        }


        public ObservableCollection<HurlCollection> Collections
        {
            get => this._collections;
            set
            {
                this._collections = value;
                Notify();
            }
        }

        public ObservableCollection<HurlEnvironment> Environments
        {
            get => this._environments;
            set
            {
                this._environments = value;
                Notify();
            }
        }

        //public IRootDock? Layout
        //{
        //    get => this._layout;
        //    set
        //    {
        //        this._layout = value;
        //        Notify();
        //    }
        //}
    }
}
