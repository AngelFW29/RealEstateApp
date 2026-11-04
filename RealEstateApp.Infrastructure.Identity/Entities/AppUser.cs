using Microsoft.AspNet.Identity.CoreCompat;

namespace RealEstateApp.Infrastructure.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        public string Nombre { get; set; } = default!;
        public string Apellido { get; set; } = default!;
        public string? Telefono { get; set; }
        public string? FotoUrl { get; set; }
        public string? Cedula { get; set; }
        public bool Activo { get; set; }
    }
}
