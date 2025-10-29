using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Interfaces;

namespace BoxExpress.Application.Services
{
    public class CityService : ICityService
    {
        private readonly IUserContext _userContext;
        private readonly ICityRepository _repository;
        private readonly IMapper _mapper;
        public CityService(ICityRepository repository, IMapper mapper, IUserContext userContext)
        {
            _repository = repository;
            _mapper = mapper;
            _userContext = userContext;
        }
        public async Task<ApiResponse<IEnumerable<CityDto>>> GetAllAsync(CityFilterDto filter)
        {
            filter.CountryId = _userContext.CountryId != null ? _userContext.CountryId : filter.CountryId;
            return ApiResponse<IEnumerable<CityDto>>.Success(_mapper.Map<List<CityDto>>(await _repository.GetFilteredAsync(_mapper.Map<CityFilter>(filter))));
        }
    }
}
