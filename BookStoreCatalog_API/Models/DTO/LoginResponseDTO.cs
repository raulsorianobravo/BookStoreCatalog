namespace BookStoreCatalog_API.Models.DTO
{
    public class LoginResponseDTO
    {
        public UserModel User { get; set; }
        public string Token {  get; set; }
    }
}
