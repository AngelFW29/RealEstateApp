using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El usuario o correo es requerido.")]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterUserDto
    {
        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;
        [Required] public string IdCard { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string UserName { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Required] public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public DateTime Expiration { get; set; }
    }

    public class AuthResult
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public AuthResponseDto? Data { get; set; }
    }
}