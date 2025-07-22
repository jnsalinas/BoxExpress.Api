using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Utilities;
using BoxExpress.Domain.Constants;

namespace BoxExpress.Application.Services;

public class StoreService : IStoreService
{
    private readonly IStoreRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthService _authService;

    public StoreService(
        IUnitOfWork unitOfWork,
    IStoreRepository repository,
    IMapper mapper,
    IRoleRepository roleRepository,
    IAuthService authService)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
        _repository = repository;
        _mapper = mapper;
        _roleRepository = roleRepository;
    }

    public async Task<ApiResponse<StoreDto?>> GetByIdAsync(int storeId)
    {
        return ApiResponse<StoreDto?>.Success(_mapper.Map<StoreDto?>(await _repository.GetByIdWithDetailsAsync(storeId)));
    }

    public async Task<ApiResponse<AuthResponseDto>> AddStoreAsync(CreateStoreDto createStoreDto)
    {
        try
        {
            var existingStore = await _repository.GetFilteredAsync(new StoreFilter
            {
                Name = createStoreDto.StoreName,
            });

            if (existingStore.Stores.Any())
                return ApiResponse<AuthResponseDto>.Fail("Store with this name already exists");

            await _unitOfWork.BeginTransactionAsync();
            var createdAt = DateTime.UtcNow;
            var wallet = await _unitOfWork.Wallets.AddAsync(new Wallet()
            {
                CreatedAt = createdAt,
                Balance = createStoreDto.Balance,
            });

            var store = _mapper.Map<Store>(createStoreDto);
            store.CreatedAt = createdAt;
            store.WalletId = wallet.Id;

            await _unitOfWork.Stores.AddAsync(store);

            Role? role = await _roleRepository.GetByNameAsync(RolConstants.Store);
            if (role == null)
                return ApiResponse<AuthResponseDto>.Fail("Role not found");

            var user = _mapper.Map<User>(createStoreDto);
            user.CreatedAt = createdAt;
            user.StoreId = store.Id;
            user.RoleId = role.Id;
            user.PasswordHash = BcryptHelper.Hash(createStoreDto.Password);

            await _unitOfWork.Users.AddAsync(user);

            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();

            return await _authService.AuthenticateAsync(new LoginDto
            {
                UserName = createStoreDto.Email,
                Password = createStoreDto.Password
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<AuthResponseDto>.Fail("Error creating store: " + ex.Message);
        }
    }
    
    public async Task<ApiResponse<IEnumerable<StoreDto>>> GetAllAsync(StoreFilterDto filter)
    {
        var (stores, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<StoreFilter>(filter));
        return ApiResponse<IEnumerable<StoreDto>>.Success(_mapper.Map<List<StoreDto>>(stores), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<StoreDto?>> GetBalanceSummary()
    {
        return ApiResponse<StoreDto?>.Success(_mapper.Map<StoreDto?>(await _repository.GetBalanceSummary()));
    }
}
