using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;

namespace BoxExpress.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _repository;
        private readonly IMapper _mapper;
        public CountryService(ICountryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ApiResponse<IEnumerable<CountryDto>>> GetAllAsync()
        {
            var cities = await _repository.GetAllAsync();
            return ApiResponse<IEnumerable<CountryDto>>.Success(_mapper.Map<List<CountryDto>>(cities));
        }
    }
}
