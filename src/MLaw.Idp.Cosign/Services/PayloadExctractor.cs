using MLaw.Idp.Cosign.Models;
using Newtonsoft.Json.Linq;

namespace MLaw.Idp.Cosign.Services
{
    public class PayloadExctractor
    {
        public LoggedinUserModel Exctract(JObject payload)
        {
            string userName = payload.Value<string>("UserId");
            string name = payload.Value<string>("UserId");
            string email = userName.Contains("@") ? userName : $"{userName}@umich.edu";

            return LoggedinUserModel.Create(userName, name , email);
        }
    }
}
