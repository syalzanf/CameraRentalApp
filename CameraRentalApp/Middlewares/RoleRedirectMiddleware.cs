using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CameraRentalApp.Middlewares
{
    public class RoleRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                // Menghindari redirect loop dengan memeriksa apakah pengguna sudah berada di halaman yang sesuai
                if (context.User.IsInRole("Admin") && !context.Request.Path.StartsWithSegments("/admin/dashboard"))
                {
                    context.Response.Redirect("/admin/dashboard");
                    return;
                }
                else if (context.User.IsInRole("SuperAdmin") && !context.Request.Path.StartsWithSegments("/superAdmin/superadminDashboard"))
                {
                    context.Response.Redirect("/superadmin/superadminDashboard");
                    return;
                }
            }
            await _next(context);
        }
    }
}
