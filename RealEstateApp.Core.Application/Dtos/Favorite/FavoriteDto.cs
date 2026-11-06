namespace RealEstateApp.Core.Application.Dtos.Favorite
{
    public class FavoriteDto
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public int PropertyId { get; set; }
    }
}