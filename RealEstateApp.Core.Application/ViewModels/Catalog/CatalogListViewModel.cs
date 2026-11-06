using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateApp.Core.Application.ViewModels.Catalog
{
    public class CatalogListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PropertyCount { get; set; }
    }
}
