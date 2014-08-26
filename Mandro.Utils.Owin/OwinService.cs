using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Owin;

using Newtonsoft.Json;

namespace Mandro.Utils.Owin
{
    public class OwinService : IHttpService
    {
        private readonly Dictionary<string, HttpHandlerCollection> _methodHandlers;

        public OwinService()
        {
            _methodHandlers = new Dictionary<string, HttpHandlerCollection>
                              {
                                  { "GET", new HttpHandlerCollection() }, 
                                  { "POST", new HttpHandlerCollection()}, 
                                  { "DELETE", new HttpHandlerCollection()},
                                  { "PUT", new HttpHandlerCollection()},
                              };
        }

        public HttpHandlerCollection Delete { get { return _methodHandlers["DELETE"]; } }
        public HttpHandlerCollection Get { get { return _methodHandlers["GET"]; } }
        public HttpHandlerCollection Post { get { return _methodHandlers["POST"]; } }
        public HttpHandlerCollection Put { get { return _methodHandlers["PUT"]; } }

        public IHttpService Handles { get { return this; } }

        public async Task<bool> HandleAsync(IOwinContext context)
        {
            if (!_methodHandlers.ContainsKey(context.Request.Method))
            {
                return false;
            }

            var handlers = _methodHandlers[context.Request.Method];
            if (!handlers.ContainsRoute(context.Request.Path.ToString()))
            {
                return false;
            }

            var resposne = await handlers.InvokeHandler(context.Request.Path.ToString(), await context.Request.Body.ReadStringAsync());
            if (resposne != null)
            {
                if (resposne is string) await context.Response.WriteAsync(resposne.ToString());
                else await context.Response.WriteAsync(JsonConvert.SerializeObject(resposne));
            }

            return true;
        }
    }
}