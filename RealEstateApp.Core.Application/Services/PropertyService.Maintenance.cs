using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Catalog;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public partial class PropertyService : IPropertyService
    {
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

            if (!await _propertyTypeRepository.ExistsByIdAsync(dto.PropertyTypeId, cancellationToken))
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

        // implementaciones de otros métodos de mantenimiento requeridos por el controlador de agentes

        public async Task<Result> CheckCatalogsAsync(CancellationToken cancellationToken = default)
        {
            bool hasPropertyTypes = await _propertyTypeRepository.AnyExistsAsync(cancellationToken);
            bool hasSaleTypes = await _saleTypeRepository.AnyExistsAsync(cancellationToken);
            bool hasImprovements = await _improvementRepository.AnyExistsAsync(cancellationToken);

            if (!hasPropertyTypes || !hasSaleTypes || !hasImprovements)
            {
                return Result.Fail("Debe registrar al menos un tipo de propiedad, un tipo de venta y una mejora antes de crear propiedades.");
            }

            return Result.Ok();
        }

        public async Task PopulateFormAsync(SavePropertyViewModel vm, string agentId, CancellationToken cancellationToken = default)
        {
            var propertyTypes = await _propertyTypeRepository.GetAllAsync();
            var saleTypes = await _saleTypeRepository.GetAllAsync();
            var improvements = await _improvementRepository.GetAllAsync();

            vm.PropertyTypes = propertyTypes.Select(pt => new CatalogListViewModel { Id = pt.Id, Name = pt.Name }).ToList();
            vm.SaleTypes = saleTypes.Select(st => new CatalogListViewModel { Id = st.Id, Name = st.Name }).ToList();
            vm.Improvements = improvements.Select(i => new CatalogListViewModel { Id = i.Id, Name = i.Name }).ToList();
        }

        public async Task<Result<int>> CreateAsync(SavePropertyViewModel vm, IReadOnlyList<ImageUploadDto> images, string agentId, CancellationToken cancellationToken = default)
        {
            if (images == null || images.Count == 0 || images.Count > 4)
            {
                return Result<int>.Fail("Debe subir entre 1 y 4 imágenes.");
            }

            string code = await GenerateUniqueCodeAsync(cancellationToken);

            var property = new Property
            {
                Id = 0,
                Code = code,
                PropertyTypeId = vm.PropertyTypeId!.Value,
                SaleTypeId = vm.SaleTypeId!.Value,
                Price = vm.Price!.Value,
                Description = vm.Description,
                SizeInMeters = ((decimal)vm.Size!.Value),
                Bedrooms = vm.Bedrooms!.Value,
                Bathrooms = vm.Bathrooms!.Value,
                AgentId = agentId,
                Status = PropertyStatus.Available
            };

            var propertyImages = new List<PropertyImage>();
            int order = 1;
            foreach (var img in images)
            {
                string imageUrl = await _iFileStorage.SaveAsync(img.Content, Path.GetExtension(img.FileName), "properties");
                propertyImages.Add(new PropertyImage { ImagePath = imageUrl, IsMain = order == 1, DisplayOrder = order++, Id = 0 });
            }

            var createdProperty = await _propertyRepository.CreateWithDetailsAsync(property, vm.ImprovementIds, propertyImages, cancellationToken);
            return Result<int>.Ok(createdProperty.Id);
        }

        public async Task<Result<SavePropertyViewModel>> GetForEditAsync(int id, string agentId, CancellationToken cancellationToken = default)
        {
            var property = await _propertyRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (property == null)
            {
                return Result<SavePropertyViewModel>.Fail("La propiedad seleccionada no existe.");
            }

            if (property.AgentId != agentId)
            {
                return Result<SavePropertyViewModel>.Fail("No tiene permisos para modificar esta propiedad.");
            }

            if (property.Status == PropertyStatus.Sold)
            {
                return Result<SavePropertyViewModel>.Fail("No se puede editar una propiedad que ya ha sido vendida.");
            }

            var vm = new SavePropertyViewModel
            {
                Id = property.Id,
                Code = property.Code,
                PropertyTypeId = property.PropertyTypeId,
                SaleTypeId = property.SaleTypeId,
                Price = property.Price,
                Description = property.Description,
                Size = ((double)property.SizeInMeters),
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                ImprovementIds = property.Improvements?.Select(i => i.ImprovementId).ToList() ?? new(),
                CurrentImages = property.Images?.Select(img => new PropertyImageViewModel { Id = img.Id, Url = img.ImagePath, Order = img.DisplayOrder }).ToList() ?? new()
            };

            await PopulateFormAsync(vm, agentId, cancellationToken);
            return Result<SavePropertyViewModel>.Ok(vm);
        }

        public async Task<Result> UpdateAsync(SavePropertyViewModel vm, IReadOnlyList<ImageUploadDto> newImages, string agentId, CancellationToken cancellationToken = default)
        {
            var property = await _propertyRepository.GetByIdWithDetailsAsync(vm.Id, cancellationToken);
            if (property == null)
            {
                return Result.Fail("La propiedad seleccionada no existe.");
            }

            if (property.AgentId != agentId)
            {
                return Result.Fail("No tiene permisos para modificar esta propiedad.");
            }

            if (property.Status == PropertyStatus.Sold)
            {
                return Result.Fail("No se puede actualizar una propiedad vendida.");
            }

            int remainingImagesCount = (property.Images?.Count ?? 0) - (vm.ImageIdsToRemove?.Count ?? 0);
            int totalImagesCount = remainingImagesCount + (newImages?.Count ?? 0);

            if (totalImagesCount < 1 || totalImagesCount > 4)
            {
                return Result.Fail("La propiedad debe tener entre 1 y 4 imágenes en total.");
            }

            property.PropertyTypeId = vm.PropertyTypeId!.Value;
            property.SaleTypeId = vm.SaleTypeId!.Value;
            property.Price = vm.Price!.Value;
            property.Description = vm.Description;
            property.SizeInMeters = ((decimal)vm.Size!.Value);
            property.Bedrooms = vm.Bedrooms!.Value;
            property.Bathrooms = vm.Bathrooms!.Value;

            var newPropertyImages = new List<PropertyImage>();
            if (newImages != null)
            {
                int order = (property.Images?.Max(i => (int?)i.DisplayOrder) ?? 0) + 1;
                foreach (var img in newImages)
                {
                    string imageUrl = await _iFileStorage.SaveAsync(img.Content, Path.GetExtension(img.FileName), "properties");
                    newPropertyImages.Add(new PropertyImage { ImagePath = imageUrl, DisplayOrder = order++, Id= 0 });
                }
            }

            await _propertyRepository.UpdateWithDetailsAsync(property, vm.ImprovementIds, vm.ImageIdsToRemove, newPropertyImages, cancellationToken);
            return Result.Ok();
        }

        public async Task<Result<PropertyListViewModel>> GetForDeleteAsync(int id, string agentId, CancellationToken cancellationToken = default)
        {
            var property = await _propertyRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (property == null || property.AgentId != agentId)
            {
                return Result<PropertyListViewModel>.Fail("La propiedad no existe o no tiene permisos.");
            }

            var vm = new PropertyListViewModel
            {
                Id = property.Id,
                Code = property.Code,
                PropertyTypeName = property.PropertyType?.Name ?? string.Empty,
                SaleTypeName = property.SaleType?.Name ?? string.Empty,
                MainImageUrl = property.Images?.FirstOrDefault()?.ImagePath ?? string.Empty,
                Price = property.Price
            };

            return Result<PropertyListViewModel>.Ok(vm);
        }

        public async Task<Result<Result>> Delete(int id, string agentId, CancellationToken cancellationToken = default)
        {
            var property = await _propertyRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (property == null)
            {
                return Result<Result>.Fail("La propiedad no existe.");
            }

            if (property.AgentId != agentId)
            {
                return Result<Result>.Fail("No tiene permisos para eliminar esta propiedad.");
            }

            if (property.Status == PropertyStatus.Sold)
            {
                return Result<Result>.Fail("No se puede eliminar una propiedad que ya ha sido vendida.");
            }

            if (property.Images != null && property.Images.Any())
            {
                var paths = property.Images.Select(i => i.ImagePath).ToList();
                try
                {
                    foreach (var path in paths)
                    {
                        _iFileStorage.Delete(path);
                    }
                }
                catch (Exception)
                {
                    return Result<Result>.Fail("No fue posible eliminar las imágenes de la propiedad. Intente nuevamente más tarde.");
                }
            }

            await _propertyRepository.DeleteAsync(property, cancellationToken);
            return (Result<Result>)Result<Result>.Ok();
        }
    }
}