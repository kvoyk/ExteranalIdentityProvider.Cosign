using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MLaw.Idp.Cosign.Models;

namespace MLaw.Idp.Cosign.Services
{
    public class CosignLoginResultsExtractor
    {
        private readonly string _cosignClientName;
        public  CosignLoginResultsExtractor(IOptions<IdpSettings.IdpSettings> settings)
        {
            _cosignClientName = settings.Value.CosignClient.Name;
        }

        public CosignLoginResultModel Extract(IQueryCollection query)
        {
            StringValues states = query["state"];
            string state = states.FirstOrDefault();
            StringValues redirectUrls = query["redirectUrl"];
            string redirectUrl = redirectUrls.FirstOrDefault();
            StringValues codes = query[$"cosign-{_cosignClientName}"];
            string code = codes.FirstOrDefault();
            string token = code.Replace(" ", "+");
            return  CosignLoginResultModel.Create(state,redirectUrl,token);
        }
    }
}