using System.Collections.Generic;

namespace MLaw.Idp.Cosign.IdpSettings
{
    public class IdpSettings
    {
        public List<ClientSettings> Clients { get; set; }

        public CosignServer CosignServer { get; set; }

        public CosignClient CosignClient { get; set; }
    }
}