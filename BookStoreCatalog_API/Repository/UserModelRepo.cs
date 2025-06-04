using BookStoreCatalog_API.Data;
using BookStoreCatalog_API.Models;
using BookStoreCatalog_API.Models.DTO;
using BookStoreCatalog_API.Repository.IRepository;

namespace BookStoreCatalog_API.Repository
{
    public class UserModelRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;
        public UserModelRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsUserUnique(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username.ToLower() == username.ToLower());
            if (user == null) 
            {
                return true;
            }
            return false;
        }

        public Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel> Register(RegisterRequestDTO registerRequestDTO)
        {
            UserModel user = new UserModel();
            user.Username = registerRequestDTO.Username;
            user.Password = registerRequestDTO.Password;
            user.Email = registerRequestDTO.Email;
            user.Role = registerRequestDTO.Role;
            user.PasswordHash = registerRequestDTO.PasswordHash;
            user.PasswordSalt = registerRequestDTO.PasswordSalt;
            user.Name = registerRequestDTO.Name;
            user.CreatedAT = DateTime.Now;
            user.UpdatedAT = DateTime.Now;
            user.LastUpdatedAT = DateTime.Now;
            user.LastUpdatedBy = DateTime.Now;

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            user.Password = "";
            return user;
        }
    }
}
