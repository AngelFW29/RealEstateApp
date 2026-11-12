namespace RealEstateApp.Core.Application.Dtos.User
{
    public class UserSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Cedula { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
