using System;

using Owin;

namespace Mandro.Utils.Owin
{
    public static class DiagnosticsHelper
    {
        /// <summary>
        /// Logs all exceptions with given logger.
        /// </summary>
        public static void LogExceptions(this IAppBuilder appBuilder, Action<Exception> logger)
        {
            appBuilder.Use(
                async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception e)
                    {
                        logger(e);
                        throw;
                    }
                });
        }

        /// <summary>
        /// Logs all exceptions to stdout.
        /// </summary>
        public static void LogExceptions(this IAppBuilder appBuilder)
        {
            appBuilder.LogExceptions(e => Console.WriteLine(e.ToString()));
        }

        /// <summary>
        /// Writes information about incoming calls to stdout.
        /// </summary>
        public static void TraceCalls(this IAppBuilder appBuilder)
        {
            appBuilder.Use(
                async (context, next) =>
                {
                    Console.WriteLine("{0} {1} {2}", context.Request.Method, context.Request.Path, context.Request.QueryString);
                    await next();
                });
        }
    }
}