using Microsoft.AspNetCore.Mvc.Filters;
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Threading.Tasks;

namespace BoxExpress.Api.Attributes;

public class ShopifyTokenAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly IStoreService _storeService;

    public ShopifyTokenAttribute(IStoreService storeService)
    {
        _storeService = storeService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var token = context.HttpContext.Request.Headers["X-Shopify-Access-Token"].FirstOrDefault();

        if (string.IsNullOrEmpty(token) || !await _storeService.ExistsByTokenAsync(token))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}