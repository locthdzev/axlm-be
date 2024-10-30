using System.Net;
using System.Security.Claims;
using Repositories.UserRepositories;
using static Data.Enums.Status;

namespace TrainingProgramAPIServices.Middlewares
{
    public class AuthorizeMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserRepository userRepository)
        {
            try
            {
                var requestPath = context.Request.Path;

                if (requestPath.StartsWithSegments("/api/auth/login") || requestPath.StartsWithSegments("/api/auth/reset-password") || requestPath.StartsWithSegments("/api/otp-management"))
                {
                    await _next.Invoke(context);
                    return;
                }

                var userIdentity = context.User.Identity as ClaimsIdentity;
                if (!userIdentity.IsAuthenticated)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                var user = await userRepository.GetUserById(Guid.Parse(userIdentity.FindFirst("userid").Value));

                if (user.Status.Equals(UserStatus.INACTIVE))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ex.ToString());
            }
        }
    }
}