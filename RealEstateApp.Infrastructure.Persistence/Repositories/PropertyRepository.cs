using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(RealEstateAppContext context) : base(context)
        {
        }
        public async Task<List<string>> GetImagePathsByPropertyTypeIdAsync(int propertyTypeId, CancellationToken cancellationToken = default)
        {
            return await _context.Properties
                .Where(p => p.PropertyTypeId == propertyTypeId)
                .SelectMany(p => p.Images)
                .Select(img => img.ImagePath)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<string>> GetImagePathsBySaleTypeIdAsync(int saleTypeId, CancellationToken cancellationToken = default)
        {
            return await _context.Properties
                .Where(p => p.SaleTypeId == saleTypeId)
                .SelectMany(p => p.Images)
                .Select(img => img.ImagePath)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Property>> GetAvailableByAgentAsync(string agentId, CancellationToken cancellationToken = default)
        {
            return await _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.SaleType)
                .Include(p => p.Images)
                .Where(p => p.AgentId == agentId && p.Status == PropertyStatus.Available)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Property?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.SaleType)
                .Include(p => p.Images)
                .Include(p => p.Improvements)
                .ThenInclude(pi => pi.Improvement)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Properties.AnyAsync(p => p.Code == code, cancellationToken);
        }

        public async Task<Property> CreateWithDetailsAsync(Property property, IEnumerable<int> improvementIds, IEnumerable<PropertyImage> images, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _context.Properties.AddAsync(property, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var img in images)
                {
                    img.PropertyId = property.Id;
                    await _context.PropertyImages.AddAsync(img, cancellationToken);
                }

                foreach (var impId in improvementIds)
                {
                    await _context.PropertyImprovements.AddAsync(new PropertyImprovement
                    {
                        Id = 0,
                        PropertyId = property.Id,
                        ImprovementId = impId
                    }, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return property;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new InvalidOperationException("Error de concurrencia o duplicado en código de propiedad.", ex);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task UpdateWithDetailsAsync(Property property, IEnumerable<int> improvementIds, IEnumerable<int> imageIdsToRemove, IEnumerable<PropertyImage> newImages, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _context.Entry(property).State = EntityState.Modified;

                // Remover imágenes seleccionadas
                if (imageIdsToRemove.Any())
                {
                    var imagesToDelete = await _context.PropertyImages
                        .Where(img => img.PropertyId == property.Id && imageIdsToRemove.Contains(img.Id))
                        .ToListAsync(cancellationToken);

                    _context.PropertyImages.RemoveRange(imagesToDelete);
                }

                // Agregar nuevas imágenes
                foreach (var img in newImages)
                {
                    img.PropertyId = property.Id;
                    await _context.PropertyImages.AddAsync(img, cancellationToken);
                }

                // Sincronizar mejoras
                var existingImprovements = await _context.PropertyImprovements
                    .Where(pi => pi.PropertyId == property.Id)
                    .ToListAsync(cancellationToken);

                _context.PropertyImprovements.RemoveRange(existingImprovements);

                foreach (var impId in improvementIds)
                {
                    await _context.PropertyImprovements.AddAsync(new PropertyImprovement
                    {
                        Id = 0,
                        PropertyId = property.Id,
                        ImprovementId = impId
                    }, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task DeleteAsync(Property property, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
