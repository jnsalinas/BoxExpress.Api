using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;

namespace BoxExpress.Application.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _repository;
        private readonly IMapper _mapper;
        public CityService(ICityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ApiResponse<IEnumerable<CityDto>>> GetAllAsync()
        {
            var cities = await _repository.GetAllAsync();
            return ApiResponse<IEnumerable<CityDto>>.Success(_mapper.Map<List<CityDto>>(cities));
        }
    }
}
