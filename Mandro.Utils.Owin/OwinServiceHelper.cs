using Owin;

namespace Mandro.Utils.Owin
{
    public static class OwinServiceHelper
    {
        public static void UseOwinService(this IAppBuilder appBuilder, OwinService service)
        {
            appBuilder.Use(
                async (context, next) =>
                {
                    if (!await service.HandleAsync(context)) await next();
                });
        }
    }
}