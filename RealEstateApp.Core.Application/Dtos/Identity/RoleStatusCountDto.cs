
namespace RealEstateApp.Core.Application.Dtos.Identity
{
    public class RoleStatusCountDto
    {
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int Count { get; set; }
    }
}
