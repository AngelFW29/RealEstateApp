namespace RealEstateApp.Core.Domain.Common
{
    public record CatalogItemWithCount<T>(T Item, int Count) where T : class;
}
