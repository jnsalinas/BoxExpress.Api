using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Utilities;

namespace BoxExpress.Application.Services;

public class StoreService : IStoreService
{
    private readonly IStoreRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public StoreService(IUnitOfWork unitOfWork,IStoreRepository repository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<bool>> AddStoreAsync(CreateStoreDto createStoreDto)
    {
        try
        {
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

            var user = _mapper.Map<User>(createStoreDto);
            user.CreatedAt = createdAt;
            user.StoreId = store.Id;
            user.RoleId = 2;
            user.PasswordHash = BcryptHelper.Hash(createStoreDto.Password);
            
            await _unitOfWork.Users.AddAsync(user);

            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();
            
            return ApiResponse<bool>.Success(true, null, "Usuario creado exitosamente");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<bool>.Fail("Error al crear usuario: " + ex.Message);
        }
    }
    public async Task<ApiResponse<IEnumerable<StoreDto>>> GetAllAsync(StoreFilterDto filter) =>
         ApiResponse<IEnumerable<StoreDto>>.Success(_mapper.Map<List<StoreDto>>(await _repository.GetAllAsync()));
}
