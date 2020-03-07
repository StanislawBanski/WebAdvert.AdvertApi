using AdvertApiModels;
using System.Threading.Tasks;

namespace AdvertApi.Interfaces
{
    public interface IAdvertStorageService
    {
        Task<string> Add(AdvertModel model);

        Task Confirm(ConfirmAdvertModel model);

        Task<bool> CheckHealthAsync();
    }
}
