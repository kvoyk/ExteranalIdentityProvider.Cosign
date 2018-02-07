namespace MLaw.Idp.Cosign.Models
{
    public class IdentityServerRequestModel
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Code { get; set; }
    }
}
