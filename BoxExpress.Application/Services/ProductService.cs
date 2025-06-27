using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;

namespace BoxExpress.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return ApiResponse<IEnumerable<ProductDto>>.Success(_mapper.Map<List<ProductDto>>(products));
        }
    }
}
