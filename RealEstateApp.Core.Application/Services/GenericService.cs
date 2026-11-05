using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class GenericService<TDto, TEntity> : IGenericService<TDto>
        where TEntity : BasicEntity<int>
        where TDto : class
    {
        protected readonly IGenericRepository<TEntity> _repository;
        protected readonly IMapper _mapper;

        public GenericService(IGenericRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<TDto> AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            entity.Id = 0;

            var created = await _repository.AddAsync(entity);
            return _mapper.Map<TDto>(created);
        }
        public virtual async Task UpdateAsync(int id, TDto dto)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} {id} not found.");

            _mapper.Map(dto, entity);
            entity.Id = id;

            await _repository.UpdateAsync(entity);
        }
        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} {id} not found.");

            await _repository.DeleteAsync(entity);
        }
        public virtual async Task<TDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity is null ? null : _mapper.Map<TDto>(entity);
        }
        public virtual async Task<List<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<List<TDto>>(entities);
        }
    }
}