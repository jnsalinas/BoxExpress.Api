using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;

namespace BoxExpress.Application.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _repository;
        private readonly IMapper _mapper;
        public CurrencyService(ICurrencyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ApiResponse<IEnumerable<CurrencyDto>>> GetAllAsync()
        {
            var cities = await _repository.GetAllAsync();
            return ApiResponse<IEnumerable<CurrencyDto>>.Success(_mapper.Map<List<CurrencyDto>>(cities));
        }
    }
}
