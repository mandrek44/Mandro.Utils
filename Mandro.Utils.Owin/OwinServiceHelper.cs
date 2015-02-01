using Owin;

namespace Mandro.Utils.Owin
{
    public static class OwinServiceHelper
    {
        public static void UseOwinService(this IAppBuilder appBuilder, IOwinHandler service)
        {
	        appBuilder.Use<OwinServiceMiddleware>(service);
        }
    }
}