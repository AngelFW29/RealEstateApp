using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class ImprovementService : CatalogService<Improvement>, IPropertyImprovementService
    {
        private readonly IImprovementRepository _improvementRepository;
        public ImprovementService(ICatalogRepository<Improvement> repository, IImprovementRepository improvementRepository, IMapper mapper) : base(repository, mapper)
        {
            _improvementRepository = improvementRepository;
        }

        protected override string SingularNameMale => "tipo de mejora";

        protected override string SingularNameFemale => string.Empty;

        protected override bool IsFemale => false;
    }
}
