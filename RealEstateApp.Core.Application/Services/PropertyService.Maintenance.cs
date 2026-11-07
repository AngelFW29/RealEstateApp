using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public partial class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyTypeRepository _ipropertyTypeRepository;
        private readonly ISaleTypeRepository _saleTypeRepository;
        private readonly IImprovementRepository _improvementRepository;
        private readonly IFileStorage _iFileStorage;

        public async Task<List<PropertyListDto>> GetAvailableByAgentAsync(string agentId, CancellationToken cancellationToken = default)
        {
            var properties = await _propertyRepository.GetAvailableByAgentAsync(agentId, cancellationToken);
            return _mapper.Map<List<PropertyListDto>>(properties);
        }

        public async Task<SavePropertyDto> PrepareSaveDtoAsync(int? id = null, CancellationToken cancellationToken = default)
        {
            var dto = new SavePropertyDto();

            if (id.HasValue && id.Value > 0)
            {
                var property = await _propertyRepository.GetByIdWithDetailsAsync(id.Value, cancellationToken);
                if (property != null)
                {
                    dto.Id = property.Id;
                    dto.PropertyTypeId = property.PropertyTypeId;
                    dto.SaleTypeId = property.SaleTypeId;
                    dto.Price = property.Price;
                    dto.Description = property.Description;
                    dto.SizeInMeters = property.SizeInMeters;
                    dto.Bedrooms = property.Bedrooms;
                    dto.Bathrooms = property.Bathrooms;
                    dto.SelectedImprovementIds = property.Improvements.Select(i => i.ImprovementId).ToList();
                    dto.CurrentImages = property.Images.Select(img => new PropertyImageDto { Id = img.Id, Path = img.ImagePath }).ToList();
                }
            }

            return dto;
        }

        public async Task<Result> CreateAsync(SavePropertyDto dto, string agentId, CancellationToken cancellationToken = default)
        {
            if (dto.NewImages == null || dto.NewImages.Count == 0)
                return Result.Fail("Debe cargar al menos una imagen de la propiedad.");

            if (dto.NewImages.Count > 4)
                return Result.Fail("Solo se permite registrar hasta 4 imágenes por propiedad.");

            if (!await _ipropertyTypeRepository.ExistsByIdAsync(dto.PropertyTypeId, cancellationToken))
                return Result.Fail("El tipo de propiedad seleccionado no es válido.");

            if (!await _saleTypeRepository.ExistsByIdAsync(dto.SaleTypeId, cancellationToken))
                return Result.Fail("El tipo de venta seleccionado no es válido.");

            string code = string.Empty;
            var savedFilePaths = new List<string>();

            try
            {
                code = await GenerateUniqueCodeAsync(cancellationToken);

                foreach (var img in dto.NewImages)
                {
                    string extension = Path.GetExtension(img.FileName);
                    string relativePath = await _iFileStorage.SaveAsync(img.Content, extension, $"properties/{code}", cancellationToken);
                    savedFilePaths.Add(relativePath);
                }

                var property = new Property
                {
                    Id = 0,
                    Code = code,
                    Description = dto.Description?.Trim() ?? string.Empty,
                    Price = dto.Price,
                    SizeInMeters = dto.SizeInMeters,
                    Bedrooms = dto.Bedrooms,
                    Bathrooms = dto.Bathrooms,
                    PropertyTypeId = dto.PropertyTypeId,
                    SaleTypeId = dto.SaleTypeId,
                    AgentId = agentId,
                    Status = PropertyStatus.Available
                };

                var propertyImages = savedFilePaths.Select(p => new PropertyImage { ImagePath = p , Id = 0 }).ToList();

                int attempts = 0;
                bool success = false;
                while (!success && attempts < 5)
                {
                    try
                    {
                        attempts++;
                        property.Code = code;
                        await _propertyRepository.CreateWithDetailsAsync(property, dto.SelectedImprovementIds, propertyImages, cancellationToken);
                        success = true;
                    }
                    catch (Exception)
                    {
                        if (attempts >= 5) throw;
                        code = await GenerateUniqueCodeAsync(cancellationToken);
                    }
                }

                return Result.Ok();
            }
            catch (Exception)
            {
                foreach (var path in savedFilePaths)
                {
                    _iFileStorage.Delete(path);
                }
                return Result.Fail("No fue posible crear la propiedad. Intente nuevamente más tarde.");
            }
        }

        public async Task<Result> UpdateAsync(int id, SavePropertyDto dto, string agentId, CancellationToken cancellationToken = default)
        {
            var existingProperty = await _propertyRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (existingProperty == null)
                return Result.Fail("La propiedad seleccionada no existe.");

            if (existingProperty.AgentId != agentId)
                return Result.Fail("No tiene permisos para modificar esta propiedad.");

            if (existingProperty.Status != PropertyStatus.Available)
                return Result.Fail("No se puede modificar una propiedad que ya fue vendida.");

            var imagesToRemove = dto.ImageIdsToRemove ?? [];
            int currentImagesCount = existingProperty.Images.Count(img => !imagesToRemove.Contains(img.Id));
            int newImagesCount = dto.NewImages?.Count ?? 0;
            int totalImages = currentImagesCount + newImagesCount;

            if (totalImages < 1)
                return Result.Fail("Debe cargar al menos una imagen de la propiedad.");

            if (totalImages > 4)
                return Result.Fail("Solo se permite registrar hasta 4 imágenes por propiedad.");

            var newSavedFilePaths = new List<string>();

            try
            {
                if (dto.NewImages != null && dto.NewImages.Count > 0)
                {
                    foreach (var img in dto.NewImages)
                    {
                        string extension = Path.GetExtension(img.FileName);
                        string relativePath = await _iFileStorage.SaveAsync(img.Content, extension, $"properties/{existingProperty.Code}");
                        newSavedFilePaths.Add(relativePath);
                    }
                }

                existingProperty.Description = dto.Description?.Trim() ?? string.Empty;
                existingProperty.Price = dto.Price;
                existingProperty.SizeInMeters = dto.SizeInMeters;
                existingProperty.Bedrooms = dto.Bedrooms;
                existingProperty.Bathrooms = dto.Bathrooms;
                existingProperty.PropertyTypeId = dto.PropertyTypeId;
                existingProperty.SaleTypeId = dto.SaleTypeId;

                var newPropertyImages = newSavedFilePaths.Select(p => new PropertyImage { ImagePath = p, Id = 0 }).ToList();

                var pathsToDelete = existingProperty.Images
                    .Where(img => imagesToRemove.Contains(img.Id))
                    .Select(img => img.ImagePath)
                    .ToList();

                await _propertyRepository.UpdateWithDetailsAsync(
                    existingProperty,
                    dto.SelectedImprovementIds ?? [],
                    imagesToRemove,
                    newPropertyImages,
                    cancellationToken
                );

                foreach (var path in pathsToDelete)
                {
                    _iFileStorage.Delete(path);
                }

                return Result.Ok();
            }
            catch (Exception)
            {
                foreach (var path in newSavedFilePaths)
                {
                    _iFileStorage.Delete(path);
                }
                return Result.Fail("No fue posible actualizar la propiedad. Intente nuevamente más tarde.");
            }
        }

        public async Task<Result> DeleteAsync(int id, string agentId, CancellationToken cancellationToken = default)
        {
            var property = await _propertyRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (property == null)
                return Result.Fail("La propiedad seleccionada no existe.");

            if (property.AgentId != agentId)
                return Result.Fail("No tiene permisos para eliminar esta propiedad.");

            if (property.Status != PropertyStatus.Available)
                return Result.Fail("No se puede eliminar una propiedad que ya fue vendida.");

            var imagePaths = property.Images.Select(img => img.ImagePath).ToList();

            try
            {
                await _propertyRepository.DeleteAsync(property, cancellationToken);

                foreach (var path in imagePaths)
                {
                    _iFileStorage.Delete(path);
                }

                return Result.Ok();
            }
            catch (Exception)
            {
                return Result.Fail("No fue posible eliminar la propiedad. Intente nuevamente más tarde.");
            }
        }

        private async Task<string> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
        {
            var random = new Random();
            string code;
            do
            {
                code = random.Next(100000, 999999).ToString();
            } while (await _propertyRepository.CodeExistsAsync(code, cancellationToken));

            return code;
        }
    }
}