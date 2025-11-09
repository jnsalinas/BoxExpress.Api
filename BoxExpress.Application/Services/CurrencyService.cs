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
        private readonly IUserContext _userContext;
        public CurrencyService(ICurrencyRepository repository, IMapper mapper, IUserContext userContext)
        {
            _repository = repository;
            _mapper = mapper;
            _userContext = userContext;
        }
        public async Task<ApiResponse<IEnumerable<CurrencyDto>>> GetAllAsync()
        {
            var countryId = _userContext.CountryId;
            var currency = await _repository.GetByCountryIdAsync(countryId ?? 1);
            List<CurrencyDto> currencies = new List<CurrencyDto>();
            if(currency != null){
                currencies.Add(_mapper.Map<CurrencyDto>(currency));
            }
            return ApiResponse<IEnumerable<CurrencyDto>>.Success(_mapper.Map<List<CurrencyDto>>(currencies));
        }
    }
}
