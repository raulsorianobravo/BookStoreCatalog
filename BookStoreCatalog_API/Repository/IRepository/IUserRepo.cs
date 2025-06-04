using BookStoreCatalog_API.Models;
using BookStoreCatalog_API.Models.DTO;

namespace BookStoreCatalog_API.Repository.IRepository
{
    public interface IUserRepo 
    {
        bool IsUserUnique(string username);

        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);

        Task<UserModel> Register(RegisterRequestDTO registerRequestDTO);
    }
}
