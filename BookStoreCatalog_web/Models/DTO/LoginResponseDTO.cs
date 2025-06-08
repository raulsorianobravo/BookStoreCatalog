namespace BookStoreCatalog_web.Models.DTO
{
    public class LoginResponseDTO
    {
        public UserModelDTO User { get; set; }
        public string Token { get; set; }
    }
}
