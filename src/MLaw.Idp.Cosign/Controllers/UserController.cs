using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MLaw.Idp.Cosign.IdpSettings;
using MLaw.Idp.Cosign.Models;
using MLaw.Idp.Cosign.Services;

namespace MLaw.Idp.Cosign.Controllers
{
    public class UserController : Controller
    {
        private readonly List<ClientSettings> _clients;
        private readonly ILoggedUsersStorage _loggedUsersStorage;
        private readonly ILogger<AuthorizeController> _logger;
        private readonly IHostingEnvironment _environment;

        public UserController(IOptions<IdpSettings.IdpSettings> settings,
            ILoggedUsersStorage loggedUsersStorage,
            ILogger<AuthorizeController> logger,
            IHostingEnvironment environment)
        {
            _clients = settings.Value.Clients;
            _loggedUsersStorage = loggedUsersStorage;
            _logger = logger;
            _environment = environment;
        }
        [HttpPost]
        public async Task<IActionResult> Index([FromForm] IdentityServerRequestModel identityServerRequestModel)
        {

          

            ClientSettings client = _clients.FirstOrDefault(c => c.ClientId == identityServerRequestModel.ClientId);
            if (client == null)
            {
                Response.StatusCode = 400;
                return _environment.IsDevelopment()? Json($"Client not found for clientId {identityServerRequestModel.ClientId}") : Json("Bad request");
            }

            if (client.ClientSecret != identityServerRequestModel.ClientSecret)
            {
                Response.StatusCode = 400;
                return _environment.IsDevelopment() ? Json($"Client secred doesn't match") : Json("Bad request");
            }


            LoggedinUserModel loggedinUserModel = await _loggedUsersStorage.GetLoginAsync(identityServerRequestModel.Code);
            if (loggedinUserModel == null)
            {
                Response.StatusCode = 400;
                return _environment.IsDevelopment() ? Json($"No cache found for key {identityServerRequestModel.Code}") : Json("Bad request");
            }

          

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "name", loggedinUserModel.Name },
                { "nameidentifier", loggedinUserModel.UserName },
                { "email", loggedinUserModel.Email }
            };
            return Json(parameters);
        }
    }
}