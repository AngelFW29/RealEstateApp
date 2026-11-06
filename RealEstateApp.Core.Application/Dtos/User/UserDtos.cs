namespace RealEstateApp.Core.Application.Dtos.User
{
    public class UserContactDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
    }

    public class AgentBasicDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
    }
}