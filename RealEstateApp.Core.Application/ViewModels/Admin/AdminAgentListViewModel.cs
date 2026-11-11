namespace RealEstateApp.Core.Application.ViewModels.Admin
{
    public class AdminAgentListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int PropertyCount { get; set; }
        public bool IsActive { get; set; }
    }
}
