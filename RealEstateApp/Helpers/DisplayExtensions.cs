using RealEstateApp.Core.Domain.Common.Enums;
using System.Globalization;

namespace RealEstateApp.Helpers
{
    public static class DisplayExtensions
    {
        private static readonly TimeSpan DominicanOffset = TimeSpan.FromHours(-4);

        public static string ToLocalDisplay(this DateTime utc)
        {
            return new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc))
                .ToOffset(DominicanOffset)
                .ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
        }

        public static string ToBadgeClass(this OfferStatus status)
        {
            return status switch
            {
                OfferStatus.Accepted => "bg-success",
                OfferStatus.Rejected => "bg-danger",
                _ => "bg-warning text-dark"
            };
        }
    }
}
