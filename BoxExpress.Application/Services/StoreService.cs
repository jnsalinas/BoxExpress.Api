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
    private readonly IUserRepository _userRepository;

    public StoreService(
        IUnitOfWork unitOfWork,
    IStoreRepository repository,
    IMapper mapper,
    IRoleRepository roleRepository,
    IAuthService authService,
    IUserRepository userRepository)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
        _repository = repository;
        _mapper = mapper;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<StoreDto?>> GetByIdAsync(int storeId)
    {
        StoreDto? store = _mapper.Map<StoreDto?>(await _repository.GetByIdWithDetailsAsync(storeId));
        if (store != null)
        {
            var user = await _userRepository.GetByStoreIdAsync(storeId);
            store.Username = user?.Email;
        }

        return ApiResponse<StoreDto?>.Success(store);
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
                return ApiResponse<AuthResponseDto>.Fail("La tienda ya existe");

            var existingUser = await _userRepository.GetByEmailAsync(createStoreDto.Email);
            if (existingUser != null)
                return ApiResponse<AuthResponseDto>.Fail("El usuario ya existe");

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

    public async Task<bool> ExistsByTokenAsync(string token)
    {
        var (stores, totalCount) = await _repository.GetFilteredAsync(new StoreFilter { ShopifyAccessToken = token });
        return stores.Any();
    }

    public async Task<ApiResponse<bool>> UpdateAsync(int storeId, UpdateStoreDto dto)
    {
        try
        {
            var store = await _repository.GetByIdAsync(storeId);
            if (store == null)
                return ApiResponse<bool>.Fail("Store not found");
            var user = await _userRepository.GetByStoreIdAsync(storeId);
            if (user == null)
                return ApiResponse<bool>.Fail("User not found");

            store.Name = dto.Name ?? store.Name;
            store.ShopifyShopDomain = dto.ShopifyShopDomain ?? store.ShopifyShopDomain;
            user.Email = dto.Username ?? user.Email;
            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = BcryptHelper.Hash(dto.Password);

            await _repository.UpdateAsync(store);
            await _userRepository.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Fail("Error updating store: " + ex.Message);
        }
        return ApiResponse<bool>.Success(true);
    }
}
