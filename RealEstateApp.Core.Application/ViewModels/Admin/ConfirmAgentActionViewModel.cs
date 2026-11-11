namespace RealEstateApp.Core.Application.ViewModels.Admin
{
    public class ConfirmAgentActionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
