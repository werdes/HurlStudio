using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.ViewModels
{
    public interface INavigatableViewModel
    {
        IEnumerable<ViewModelBase> GetNavigationTargets();
    }
}
