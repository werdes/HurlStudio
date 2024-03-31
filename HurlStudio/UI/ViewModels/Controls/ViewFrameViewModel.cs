using HurlStudio.Common.Extensions;
using HurlStudio.Common.UI;
using HurlStudio.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels.Controls
{
    public class ViewFrameViewModel : ViewModelBase
    {
        public ViewModelBase? _selectedViewModel;
        public OrderedObservableCollection<ViewModelBase> _viewModels;

        public ViewFrameViewModel(LoadingViewViewModel loadingViewViewModel, EditorViewViewModel editorViewViewModel) : base(typeof(ViewFrame))
        {
            _viewModels = new OrderedObservableCollection<ViewModelBase>();
            _viewModels.AddIfNotNull(loadingViewViewModel);
            _viewModels.AddIfNotNull(editorViewViewModel);

            _selectedViewModel = loadingViewViewModel;
        }
        
        public ViewModelBase? SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                if(_selectedViewModel != null)
                {
                    _selectedViewModel.IsActive = false;
                }

                _selectedViewModel = value;

                if (_selectedViewModel != null)
                {
                    _selectedViewModel.IsActive = true;
                }

                Notify();
            }
        }

        public OrderedObservableCollection<ViewModelBase> ViewModels
        {
            get => _viewModels;
            set
            {
                _viewModels = value;
                Notify();
            }
        }
    }
}
