namespace Mandro.Utils.Owin
{
    public interface IHttpService
    {
        HttpHandlerCollection Delete { get; }

        HttpHandlerCollection Post { get; }

        HttpHandlerCollection Get { get; }
    }
}