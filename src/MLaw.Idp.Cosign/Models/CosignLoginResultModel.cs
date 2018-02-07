namespace MLaw.Idp.Cosign.Models
{
    public class CosignLoginResultModel
    {

        public static CosignLoginResultModel Create(string state, string redirectUrl, string token)
        {
            return new CosignLoginResultModel()
            {
               State=state,
                RedirectUrl =  redirectUrl,
                Token = token

            };

        }



        public string State { get; set; }
        public string RedirectUrl { get; set; }
        public string Token { get; set; }

    }
}
