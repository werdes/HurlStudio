using HurlStudio.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.UI.Dock
{
    public interface IExtendedAsyncDockable
    {
        Task<DockableCloseMode> AskAllowClose();
        Task Save();
        Task Discard();
    }
}
