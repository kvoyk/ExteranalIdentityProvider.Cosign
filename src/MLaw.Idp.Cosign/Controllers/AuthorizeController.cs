using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MLaw.Idp.Cosign.IdpSettings;
using MLaw.Idp.Cosign.Models;
using MLaw.Idp.Cosign.Services;
using Newtonsoft.Json.Linq;

namespace MLaw.Idp.Cosign.Controllers
{
    public class AuthorizeController : Controller
    {

        private readonly CosignServer _cosignServer;
        private readonly CosignClient _cosignClient;
        private readonly CosignLoginResultsExtractor _cosignLoginResultsExtractor;
        private readonly TcpBackchannel _tcpBackchannel;
        private readonly UrlBuilder _urlBuilder;
        private readonly PayloadExctractor _payloadExctractor;
        private readonly ILoggedUsersStorage _loggedUsersStorage;
        private readonly ILogger<AuthorizeController> _logger;

        public AuthorizeController(IOptions<IdpSettings.IdpSettings> settings,
            CosignLoginResultsExtractor cosignLoginResultsExtractor,
            TcpBackchannel tcpBackchannel,
            UrlBuilder urlBuilder,
            PayloadExctractor payloadExctractor,
            ILoggedUsersStorage loggedUsersStorage,
            ILogger<AuthorizeController> logger)
        {

            _cosignServer = settings.Value.CosignServer;
            _cosignClient = settings.Value.CosignClient;
            _cosignLoginResultsExtractor = cosignLoginResultsExtractor;
            _tcpBackchannel = tcpBackchannel;
            _urlBuilder = urlBuilder;
            _payloadExctractor = payloadExctractor;
            _loggedUsersStorage = loggedUsersStorage;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string cosignAuthUrl = _urlBuilder.BuildCosignLoginUrl(Request.Scheme, Request.Host.Value, Request.PathBase.Value, Request.Query);
            return Redirect(cosignAuthUrl);
        }


        [HttpGet]
        public async Task<IActionResult> Login()
        {

            CosignLoginResultModel cosignModel = _cosignLoginResultsExtractor.Extract(Request.Query);
            JObject payload = _tcpBackchannel.Send(
                cosignModel.Token,
                _cosignServer.Name,
                _cosignServer.Port,
                _cosignClient.Name,
                _cosignServer.TryCount);

            LoggedinUserModel loggedinUserModel = _payloadExctractor.Exctract(payload);

            string cacheKey = await _loggedUsersStorage.SaveLoginAsync(loggedinUserModel);

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"code", cacheKey},
                {"state", cosignModel.State}
            };
            string redirectUrl = QueryHelpers.AddQueryString(cosignModel.RedirectUrl, parameters);
            return Redirect(redirectUrl);
        }
    }
}