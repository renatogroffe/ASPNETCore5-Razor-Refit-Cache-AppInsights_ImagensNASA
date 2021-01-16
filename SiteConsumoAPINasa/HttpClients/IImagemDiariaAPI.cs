using System.Threading.Tasks;
using Refit;
using SiteConsumoAPINasa.Models;

namespace SiteConsumoAPINasa.HttpClients
{
    public interface IImagemDiariaAPI
    {
        [Get("/apod")]
        Task<InfoImagemNASA> GetInfo(string api_key, string date);
    }
}