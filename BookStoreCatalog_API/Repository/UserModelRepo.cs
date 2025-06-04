using BookStoreCatalog_API.Data;
using BookStoreCatalog_API.Models;
using BookStoreCatalog_API.Models.DTO;
using BookStoreCatalog_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreCatalog_API.Repository
{
    public class UserModelRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;
        private string secretKey;
        public UserModelRepo(ApplicationDbContext context,IConfiguration configuration)
        {
            _context = context;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
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

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == loginRequestDTO.UserName.ToLower()
                && x.Password == loginRequestDTO.Password);
            if (user == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }
            //JsonWebToken
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO newloginResponseDTO = new LoginResponseDTO();
            newloginResponseDTO.Token = tokenHandler.WriteToken(token);
            newloginResponseDTO.User = user;

            return newloginResponseDTO;
            
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
