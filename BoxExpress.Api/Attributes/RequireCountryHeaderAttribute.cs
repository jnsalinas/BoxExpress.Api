// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;
// using System;
// using System.Threading.Tasks;

// namespace BoxExpress.Api.Attributes;

// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
// public class RequireCountryHeaderAttribute : Attribute, IAsyncActionFilter
// {
//     public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//     {
//         var headers = context.HttpContext?.Request?.Headers;
//         if (headers == null)
//         {
//             context.Result = new BadRequestObjectResult("Header X-Country-Id es requerido.");
//             return;
//         }

//         if (!headers.TryGetValue("X-Country-Id", out var values))
//         {
//             context.Result = new BadRequestObjectResult("Header X-Country-Id es requerido.");
//             return;
//         }

//         var value = values.ToString();
//         if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out _))
//         {
//             context.Result = new BadRequestObjectResult("Header X-Country-Id debe ser un entero válido.");
//             return;
//         }

//         await next();
//     }
// }


