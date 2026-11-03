using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Propiedad : BaseEntity
    {
        public string Codigo { get; set; } = default!;   // 6 dígitos, único
        public string AgenteId { get; set; } = default!;
        public int TipoPropiedadId { get; set; }
        public int TipoVentaId { get; set; }
        public decimal Precio { get; set; } // DOP
        public string Descripcion { get; set; } = default!;
        public decimal Tamano { get; set; } // metros
        public int Habitaciones { get; set; }
        public int Banos { get; set; }
        public EstadoPropiedad Estado { get; set; } = EstadoPropiedad.Disponible;
        public DateTime FechaCreacion { get; set; }

        public TipoPropiedad TipoPropiedad { get; set; } = default!;
        public TipoVenta TipoVenta { get; set; } = default!;
        public ICollection<PropiedadImagen> Imagenes { get; set; } = new List<PropiedadImagen>();
        public ICollection<PropiedadMejora> Mejoras { get; set; } = new List<PropiedadMejora>();
        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
        public ICollection<Oferta> Ofertas { get; set; } = new List<Oferta>();
        public ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
    }
}
