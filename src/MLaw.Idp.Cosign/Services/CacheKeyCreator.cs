using System;

namespace MLaw.Idp.Cosign.Services
{
    public class CacheKeyCreator
    {
        public string CreateKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
