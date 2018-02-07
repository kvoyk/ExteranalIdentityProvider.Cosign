using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace MLaw.Idp.Cosign.Services
{
    public class TcpBackchannel
    {
        private readonly ILogger _logger;

        public TcpBackchannel(ILogger<TcpBackchannel> logger)
        {
            _logger = logger; 
        }
        public JObject Send(string serviceCookieValue, string cosingDns, int cosignPort, string clientDns, int tryCount)
        {
            X509CertificateCollection certs = GetCertificateCertificateCollection(clientDns,
                StoreName.My,
                StoreLocation.LocalMachine);

            IPHostEntry hostEntry = Dns.GetHostEntry(cosingDns);
            return Send(serviceCookieValue, hostEntry.AddressList, cosignPort, cosingDns, clientDns, certs, tryCount);

        }

        public JObject Send(string serviceCookieValue, IPAddress[] cosignAddresses, int cosignPort,string cosignDns,string clientDns, X509CertificateCollection certs, int tryCount)
        {

            for (int i = 0; i < tryCount; i++)
            {
                //("231 xxx.xxx.xx.xx uniqname UMICH.EDU mtoken two-factor ", true);
                try
                {
                    string receivedData = Connect(serviceCookieValue, cosignAddresses, cosignPort, cosignDns, clientDns, certs);

                    switch (receivedData.Substring(0, 1))
                    {
                        case "2":
                            //Success
                            _logger.LogInformation("Cosign authentication handler. 2-Response from Server: Success.");
                            JObject jobject = Create(receivedData);
                            return jobject;

                        case "4":
                            //Logged out
                            _logger.LogWarning("Cosign authentication handler. Response from Server: 4-Logged out.");
                            break;
                        case "5":
                            //Try a different server
                            _logger.LogWarning("Cosign authentication handler. Response from Server: 5-Try different server.");
                            break;
                        default:
                            _logger.LogWarning("Cosign authentication handler. Response from Server: Undefined.");
                            break;
                    }
                }
                catch (Exception e)
                {
                    continue;
                }

            }
           return new JObject();
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            return false;
        }
        public static X509CertificateCollection GetCertificateCertificateCollection(string subjectName,
            StoreName storeName,
            StoreLocation storeLocation)
        {
            // The following code gets the cert from the keystore
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection =
                store.Certificates.Find(X509FindType.FindBySubjectName,
                    subjectName,
                    false);
            return certCollection;
        }


        public string Connect(string serviceCookieValue, IPAddress[] cosignAddresses, int cosignPort , string cosignDns, string clientDns, X509CertificateCollection certs)
        {
            //string newline = @"\r\n"; //Environment.NewLine;
            string newline = Environment.NewLine;

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid 
            // an exception that occurs when the host IP Address is not compatible with the address family 

            // (typical in the IPv6 case). 
            foreach (IPAddress address in cosignAddresses)
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    tcpClient.Connect(address, cosignPort);
                    if (!tcpClient.Connected) continue;


                    using (NetworkStream networkStream = tcpClient.GetStream())
                    using (StreamReader clearTextReader = new StreamReader(networkStream))
                    using (StreamWriter clearTextWriter = new StreamWriter(networkStream))
                    {
                        string connectResponse = clearTextReader.ReadLine();
                        if (connectResponse == null || !connectResponse.StartsWith("220"))
                        {
                            //throw new InvalidOperationException("Cosign Server did not respond to connection request");
                            continue;
                        }

                        string secureRequestStart = $"STARTTLS 2{newline}";
                        clearTextWriter.Write(secureRequestStart);
                        clearTextWriter.Flush();

                        string startTlsResponse = clearTextReader.ReadLine();
                        if (startTlsResponse == null || !startTlsResponse.StartsWith("220"))
                        {
                            //throw new InvalidOperationException("Cosign Server did not respond to STARTTLS request");
                            continue;
                        }

                        using (SslStream sslStream = new SslStream(tcpClient.GetStream(), false,
                            ValidateServerCertificate,
                            null))
                        {
                            sslStream.AuthenticateAsClient(cosignDns, certs, SslProtocols.Tls12, false);

                            if (
                                !(sslStream.IsEncrypted && sslStream.IsSigned &&
                                  sslStream.IsMutuallyAuthenticated))
                                continue;


                            using (StreamReader secureTextReader = new StreamReader(sslStream))
                            using (StreamWriter secureTextWriter = new StreamWriter(sslStream))
                            {
                                string secureResponse = secureTextReader.ReadLine();
                                if (secureResponse == null || !secureResponse.StartsWith("220"))
                                    continue;
                                string secureRequest =
                                    $"CHECK cosign-{clientDns}={serviceCookieValue}{newline}";

                                secureTextWriter.Write(secureRequest);
                                secureTextWriter.Flush();
                                secureResponse = secureTextReader.ReadLine();
                                return secureResponse;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private JObject Create(string receivedData)
        {
 
            string[] cosignData = receivedData.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            JObject cosignValues = new JObject();
            if (cosignData.Length >= 0)
            {
                cosignValues["IpAddress"] =cosignData[1];
            }

            if (cosignData.Length >= 1)
            {
                cosignValues["UserId"] = cosignData[2].ToLower();
            }

            if (cosignData.Length >= 2)
            {
                cosignValues["Realm"] = cosignData[3].ToLower().Trim(Environment.NewLine.ToCharArray()[0]);
            }

            return cosignValues;
        }

    }
    
}