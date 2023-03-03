using NewMyProject.DTO.responseDto;
using Microsoft.Extensions.Options;

namespace NewMyProject
{
    public class AuthorizeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthorizationConfig _authorizationConfig;

        public AuthorizeMiddleware(RequestDelegate next, 
                    IOptions<AuthorizationConfig> authorizationConfig)
        {
            _next = next;
            _authorizationConfig = authorizationConfig.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var RoleClaim = context.User.Claims.FirstOrDefault(x =>
                x.Type.Equals(_authorizationConfig.MicrosoftClaimsGateway,
                StringComparison.InvariantCultureIgnoreCase)).Value.ToString();

                if (RoleClaim != null)
                {
                    context.Response.Headers["X-UserRole"] = RoleClaim;
                }
            }catch (Exception ex)
            {

            }
            await _next.Invoke(context);
        }
    }
}
