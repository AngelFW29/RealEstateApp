using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Catalog;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Catalog;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public abstract class CatalogService<T> : ICatalogService where T : TypeEntity
    {
        protected readonly ICatalogRepository<T> _repository;
        protected readonly IMapper _mapper;

        protected abstract string SingularNameMale { get; }
        protected abstract string SingularNameFemale { get; }
        protected abstract bool IsFemale { get; }

        protected string SingularName => IsFemale ? SingularNameFemale : SingularNameMale;

        protected CatalogService(ICatalogRepository<T> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<List<CatalogListViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var itemsWithCount = await _repository.GetAllWithCountAsync(cancellationToken);

            return itemsWithCount.Select(x =>
            {
                var vm = _mapper.Map<CatalogListViewModel>(x.Item);
                vm.PropertyCount = x.Count;
                return vm;
            }).ToList();
        }

        public virtual async Task<SaveCatalogViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity is null ? null : _mapper.Map<SaveCatalogViewModel>(entity);
        }

        public virtual async Task<Result> CreateAsync(SaveCatalogViewModel vm, CancellationToken cancellationToken = default)
        {
            vm.Name = vm.Name?.Trim() ?? string.Empty;
            vm.Description = vm.Description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(vm.Name) || string.IsNullOrWhiteSpace(vm.Description))
            {
                return Result.Fail("Debe completar todos los campos requeridos.", nameof(vm.Name));
            }

            bool nameExists = await _repository.ExistsByNameAsync(vm.Name, null);
            if (nameExists)
            {
                string article = IsFemale ? "una" : "un";
                return Result.Fail($"Ya existe {article} {SingularName} registrado con este nombre.", nameof(vm.Name));
            }

            try
            {
                var entity = (T)_mapper.Map(vm, typeof(SaveCatalogViewModel), typeof(T));
                await _repository.AddAsync(entity);

                string articleDef = IsFemale ? "La" : "El";
                string suffix = IsFemale ? "creada" : "creado";
                return Result.Ok();
            }
            catch (Exception)
            {
                string article = IsFemale ? "una" : "un";
                return Result.Fail($"Ya existe {article} {SingularName} registrado con este nombre.", nameof(vm.Name));
            }
        }

        public virtual async Task<Result> UpdateAsync(int id, SaveCatalogViewModel vm, CancellationToken cancellationToken = default)
        {
            vm.Name = vm.Name?.Trim() ?? string.Empty;
            vm.Description = vm.Description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(vm.Name) || string.IsNullOrWhiteSpace(vm.Description))
            {
                return Result.Fail("Debe completar todos los campos requeridos.", nameof(vm.Name));
            }

            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity is null)
            {
                string articleDef = IsFemale ? "La" : "El";
                string suffix = IsFemale ? "seleccionada" : "seleccionado";
                return Result.Fail($"{articleDef} {SingularName} {suffix} no existe.");
            }

            bool nameExists = await _repository.ExistsByNameAsync(vm.Name, id);
            if (nameExists)
            {
                string article = IsFemale ? "otra" : "otro";
                return Result.Fail($"Ya existe {article} {SingularName} registrado con este nombre.", nameof(vm.Name));
            }

            try
            {
                existingEntity.Name = vm.Name;
                existingEntity.Description = vm.Description;

                await _repository.UpdateAsync(existingEntity);
                return Result.Ok();
            }
            catch (Exception)
            {
                string article = IsFemale ? "otra" : "otro";
                return Result.Fail($"Ya existe {article} {SingularName} registrado con este nombre.", nameof(vm.Name));
            }
        }

        public virtual async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity is null)
            {
                string articleDef = IsFemale ? "La" : "El";
                string suffix = IsFemale ? "seleccionada" : "seleccionado";
                return Result.Fail($"{articleDef} {SingularName} {suffix} no existe.");
            }
            try
            {
                await PerformDeleteAsync(existingEntity, cancellationToken);
                return Result.Ok();
            }
            catch (Exception)
            {
                string articleDef = IsFemale ? "la" : "el";
                return Result.Fail($"No fue posible eliminar {articleDef} {SingularName}. Intente nuevamente más tarde.");
            }
        }

        protected virtual Task PerformDeleteAsync(T entity, CancellationToken cancellationToken)
        {
            return _repository.DeleteAsync(entity);
        }
        public virtual async Task<List<CatalogItemDto>> GetAllApiAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync();
            return _mapper.Map<List<CatalogItemDto>>(items);
        }
        public virtual async Task<Result<CatalogItemDto>> GetByIdApiAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
            {
                string articleDef = IsFemale ? "La" : "El";
                string suffix = IsFemale ? "solicitada" : "solicitado";
                return (Result<CatalogItemDto>)Result<CatalogItemDto>.NotFoundResult($"{articleDef} {SingularName} {suffix} no existe.");
            }

            return Result<CatalogItemDto>.Ok(_mapper.Map<CatalogItemDto>(entity));
        }
        public virtual async Task<Result<CatalogItemDto>> CreateApiAsync(CatalogRequestDto dto, CancellationToken cancellationToken = default)
        {
            dto.Name = dto.Name?.Trim() ?? string.Empty;
            dto.Description = dto.Description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description))
            {
                return Result<CatalogItemDto>.Fail("Los datos enviados no son válidos.");
            }

            bool nameExists = await _repository.ExistsByNameAsync(dto.Name, null);
            if (nameExists)
            {
                string article = IsFemale ? "una" : "un";
                return Result<CatalogItemDto>.Fail($"Ya existe {article} {SingularName} registrado con este nombre.");
            }

            try
            {
                var entity = (T)_mapper.Map(dto, typeof(CatalogRequestDto), typeof(T));
                await _repository.AddAsync(entity);
                return Result<CatalogItemDto>.Ok(_mapper.Map<CatalogItemDto>(entity));
            }
            catch (Exception)
            {
                string article = IsFemale ? "una" : "un";
                return Result<CatalogItemDto>.Fail($"Ya existe {article} {SingularName} registrado con este nombre.");
            }
        }
        public virtual async Task<Result<CatalogItemDto>> UpdateApiAsync(int id, CatalogRequestDto dto, CancellationToken cancellationToken = default)
        {
            dto.Name = dto.Name?.Trim() ?? string.Empty;
            dto.Description = dto.Description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description))
            {
                return Result<CatalogItemDto>.Fail("Los datos enviados no son válidos.");
            }

            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity is null)
            {
                string articleDef = IsFemale ? "La" : "El";
                string suffix = IsFemale ? "solicitada" : "solicitado";
                return (Result<CatalogItemDto>)Result<CatalogItemDto>.NotFoundResult($"{articleDef} {SingularName} {suffix} no existe.");
            }

            bool nameExists = await _repository.ExistsByNameAsync(dto.Name, id);
            if (nameExists)
            {
                string article = IsFemale ? "otra" : "otro";
                return Result<CatalogItemDto>.Fail($"Ya existe {article} {SingularName} registrado con este nombre.");
            }

            try
            {
                existingEntity.Name = dto.Name;
                existingEntity.Description = dto.Description;

                await _repository.UpdateAsync(existingEntity);
                return Result<CatalogItemDto>.Ok(_mapper.Map<CatalogItemDto>(existingEntity));
            }
            catch (Exception)
            {
                string article = IsFemale ? "otra" : "otro";
                return Result<CatalogItemDto>.Fail($"Ya existe {article} {SingularName} registrado con este nombre.");
            }
        }
    }
}