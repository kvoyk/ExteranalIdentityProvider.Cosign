using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace MLaw.Idp.Cosign.Handler
{
    public class CosignHandler : IHttpHandler
    {
        private const string IdpReturnUrl = "idpreturnurl";
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            NameValueCollection query = request.QueryString;
            string[] appUrls = query.GetValues(IdpReturnUrl);
            string appUrl = appUrls?.FirstOrDefault();
            if (appUrl == null)
            {
                throw new Exception($"CosignHandler didn't find key '{IdpReturnUrl}' in the query string.");
            }

            string queryString =  string.Join("&", query.AllKeys.Select(a => a + "=" + query[a]));
            context.Response.Redirect($"{appUrl}?{queryString}");
        }
        public bool IsReusable => true;
    }
}
