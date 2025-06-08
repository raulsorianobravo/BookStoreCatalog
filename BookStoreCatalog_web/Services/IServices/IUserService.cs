using BookStoreCatalog_web.Models.DTO;

namespace BookStoreCatalog_web.Services.IServices
{
    public interface IUserService
    {
        Task<T> Login<T>(LoginRequestDTO dto);

        Task<T> Register<T>(RegisterRequestDTO dto);
    }
}
