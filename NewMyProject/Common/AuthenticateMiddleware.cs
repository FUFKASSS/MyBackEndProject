using NewMyProject.Data;

namespace NewMyProject.Common
{
    public class AuthenticateMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticateMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, EfContext EfContext)
        {
            try
            {
                var userId = context.User.Claims.FirstOrDefault(x => 
                                                  x.Type.Equals("Id")).Value;
                var profileId = EfContext.Profiles.FirstOrDefault(x => 
                                                x.Id.ToString() == userId).Id;

                context.Items["profileId"] = profileId;
                context.Items["userId"] = userId;
            }
            catch (Exception ex)
            {

            }
            await _next.Invoke(context);
        }
    }
}
