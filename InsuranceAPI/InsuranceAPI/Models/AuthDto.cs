namespace InsuranceAPI.Models
{
    public class LoginDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}