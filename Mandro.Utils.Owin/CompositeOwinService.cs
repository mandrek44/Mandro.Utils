using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace Mandro.Utils.Owin
{
	public class CompositeOwinService : IOwinHandler
	{
		private readonly IEnumerable<OwinService> _services;

		public CompositeOwinService(IEnumerable<OwinService> services)
		{
			_services = services;
		}

		public async Task<bool> HandleAsync(IOwinContext context)
		{
			foreach (var service in _services)
				if (await service.HandleAsync(context))
					return true;

			return false;
		}
	}
}