namespace RealEstateApp.Core.Application.ViewModels.Staff
{
    public class StaffUserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Cedula { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
