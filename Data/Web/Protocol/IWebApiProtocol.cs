using System.Net.Http;
using System.Threading.Tasks;
using Pixeval.Data.Web.Request;
using Refit;

namespace Pixeval.Data.Web.Protocol
{
    [Headers("User-Agent: PixivAndroidApp/5.0.64 (Android 6.0)", "Content-Type: application/x-www-form-urlencoded")]
    public interface IWebApiProtocol
    {
        [Post("/setting_user.php")]
        Task<HttpResponseMessage> ToggleR18State([Body(BodySerializationMethod.UrlEncoded)] ToggleR18StateRequest toggleR18StateRequest);
    }
}