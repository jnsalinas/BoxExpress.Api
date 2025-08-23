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
        var hash = context.HttpContext.Request.Headers["X-Shopify-Hmac-Sha256"].FirstOrDefault();

        if (string.IsNullOrEmpty(hash))// || !await _storeService.ExistsByTokenAsync(token))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}