namespace BookStoreCatalog_web.Models.DTO
{
    public class RegisterRequestDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAT { get; set; }
        public DateTime UpdatedAT { get; set; }
        public DateTime LastUpdatedAT { get; set; }
        public DateTime LastUpdatedBy { get; set; }
    }
}
