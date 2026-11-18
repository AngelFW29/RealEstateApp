using RealEstateApp.Core.Application.Dtos.Agent;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateApp.Core.Application.Services
{
    public class AgentApiService : IAgentApiService
    {
        private readonly IAccountService _accountService;
        private readonly IPropertyRepository _propertyRepository;

        public AgentApiService(IAccountService accountService, IPropertyRepository propertyRepository)
        {
            _accountService = accountService;
            _propertyRepository = propertyRepository;
        }

        public async Task<List<AgentApiDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _accountService.GetUsersByRoleAsync("Agente", cancellationToken);
            if (users == null || users.Count == 0) return new List<AgentApiDto>();

            var agentIds = users.Select(u => u.Id).ToList();
            var counts = await _propertyRepository.CountByAgentIdsAsync(agentIds, cancellationToken);

            return users.Select(u => new AgentApiDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                IsActive = u.IsActive,
                PropertyCount = counts.TryGetValue(u.Id, out var count) ? count : 0
            }).ToList();
        }

        public async Task<Result<AgentApiDto>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _accountService.GetUserWithRoleAsync(id, "Agente", cancellationToken);
            if (user == null)
            {
                return Result<AgentApiDto>.Fail("El agente solicitado no existe.");
            }

            var counts = await _propertyRepository.CountByAgentIdsAsync(new[] { id }, cancellationToken);
            int propertyCount = counts.TryGetValue(id, out var count) ? count : 0;

            var dto = new AgentApiDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                PropertyCount = propertyCount
            };

            return Result<AgentApiDto>.Ok(dto);
        }

        public async Task<Result<List<PropertyDto>>> GetAgentPropertiesAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _accountService.GetUserWithRoleAsync(id, "Agente", cancellationToken);
            if (user == null)
            {
                return Result<List<PropertyDto>>.Fail("El agente solicitado no existe.");
            }

            var properties = await _propertyRepository.GetByAgentIdWithDetailsAsync(id, cancellationToken);
            string agentFullName = $"{user.FirstName} {user.LastName}";

            var propertyDtos = properties.Select(p => new PropertyDto
            {
                Id = p.Id,
                Code = p.Code,
                PropertyTypeId = p.PropertyType!.Id,
                SaleTypeId = p.SaleType!.Id,
                Price = p.Price,
                SizeInMeters = p.SizeInMeters,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Description = p.Description,
                ImprovementNames = p.Improvements?.Select(i => i.Improvement?.Name ?? string.Empty).ToList() ?? new List<string>(),
                AgentName = agentFullName,
                AgentId = p.AgentId,
                Status = p.Status
            }).ToList();

            return  Result<List<PropertyDto>>.Ok(propertyDtos);
        }

        public async Task<Result> ChangeStatusAsync(string id, bool isActive, CancellationToken cancellationToken = default)
        {
            var user = await _accountService.GetUserWithRoleAsync(id, "Agente", cancellationToken);
            if (user == null)
            {
                return Result.Fail("El agente solicitado no existe.");
            }

            var result = await _accountService.SetActiveAsync(id, isActive, cancellationToken);
            if (!result.Success)
            {
                return Result.Fail(result.FirstError ?? "No se pudo actualizar el estado del agente.");
            }

            return Result.Ok();
        }
    }
}
