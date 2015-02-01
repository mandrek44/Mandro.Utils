using System.Threading.Tasks;

using Microsoft.Owin;

namespace Mandro.Utils.Owin
{
	public interface IOwinHandler
	{
		Task<bool> HandleAsync(IOwinContext context);
	}
}