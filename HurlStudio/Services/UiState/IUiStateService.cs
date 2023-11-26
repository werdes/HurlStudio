
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Services.UiState
{
    public interface IUiStateService
    {
        Task<Model.UiState.UiState?> GetUiStateAsync(bool refresh);
        Model.UiState.UiState? GetUiState(bool refresh);
        Task StoreUiStateAsync();
        void StoreUiState();
    }
}
