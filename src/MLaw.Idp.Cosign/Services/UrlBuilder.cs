using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace MLaw.Idp.Cosign.Services
{
    public class UrlBuilder
    {

        private readonly string _cosignServer;
        private readonly string _clientServer;


        public UrlBuilder(IOptions<IdpSettings.IdpSettings> settings)
        {
            _cosignServer = settings.Value.CosignServer.Name;
            _clientServer = settings.Value.CosignClient.Name;
        }

        public string BuildCosignLoginUrl(string idpHttpScheme, string idpHostName, string idpAppName,
            IEnumerable<KeyValuePair<string, StringValues>> query)
        {
            string state = query.Where(x => x.Key == "state").Select(x => x.Value).FirstOrDefault();
            string redirectUrl = query.Where(x => x.Key == "redirectUrl").Select(x => x.Value).FirstOrDefault();
            string idpAppNameAdjustedForIISExpress = idpAppName.StartsWith("/") | string.IsNullOrWhiteSpace(idpAppName) ? idpAppName : $"/{idpAppName}";
            string idpReturnUrl = $"{idpHttpScheme}://{idpHostName}{idpAppNameAdjustedForIISExpress}/authorize/login";
            string cosignRedirectUrl = $"https://{_cosignServer}/?cosign-{_clientServer}";

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"idpreturnurl", idpReturnUrl },
                {"redirectUrl", redirectUrl},
                {"state", state}
            };
            return QueryHelpers.AddQueryString(cosignRedirectUrl, parameters);
        }
    }
}