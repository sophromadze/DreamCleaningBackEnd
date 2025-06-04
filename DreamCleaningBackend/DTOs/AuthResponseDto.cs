namespace DreamCleaningBackend.DTOs
{
    public class AuthResponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
