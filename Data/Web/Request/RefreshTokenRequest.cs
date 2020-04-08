using Refit;

namespace Pixeval.Data.Web.Request
{
    public class RefreshTokenRequest
    {
        [AliasAs("refresh_token")]
        public string RefreshToken { get; set; }

        [AliasAs("grant_type")]
        public string GrantType => "refresh_token";

        [AliasAs("client_id")]
        public string ClientId => "MOBrBDS8blbauoSck0ZfDbtuzpyT";

        [AliasAs("client_secret")]
        public string ClientSecret => "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";

        [AliasAs("get_secure_url")]
        public string GetSecureUrl => "1";
    }
}