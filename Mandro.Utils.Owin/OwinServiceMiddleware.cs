using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace Mandro.Utils.Owin
{
	public class OwinServiceMiddleware
	{
		private readonly Func<IDictionary<string, object>, Task> _next;
		private readonly IOwinHandler _service;

		public OwinServiceMiddleware(Func<IDictionary<string, object>, Task> next, IOwinHandler service)
		{
			_next = next;
			_service = service;
		}

		public async Task Invoke(IDictionary<string, object> environment)
		{
			if (! await _service.HandleAsync(new OwinContext(environment)))
				await _next(environment);
		}
	}
}