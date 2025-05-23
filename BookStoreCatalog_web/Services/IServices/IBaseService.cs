using BookStoreCatalog_web.Models;

namespace BookStoreCatalog_web.Services.IServices
{
    public interface IBaseService
    {
        public APIResponse responseModel {  get; set; }

        Task<T> SendAsync<T>(APIRequest apiRequest);
    }
}
