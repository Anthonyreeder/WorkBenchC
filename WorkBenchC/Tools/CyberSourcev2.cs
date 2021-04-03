using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jose;

using System.Buffers.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Jose.keys;

namespace WorkBenchC.Tools
{
    class CyberSourcev2
    {
        public CyberSourcev2(String keyID)
        {
            //   String keyBody = new String(Base64.DecodeFromUtf8(
            //decode key using base64 split at point.
            string keyIdSplit = keyID.Split('.')[1];
            byte[] data = Convert.FromBase64String(keyIdSplit);

            string decodedString = Encoding.UTF8.GetString(data); 
            var serializeAddressFormatted = JsonConvert.SerializeObject(decodedString);
            var routes_list2 = (JObject)JsonConvert.DeserializeObject(decodedString);

            var result2 = routes_list2.Descendants()
                 .OfType<JProperty>()
                 .FirstOrDefault(x => x.Name == "kid")
                 ?.Value;

            var result3 = routes_list2.Descendants()
             .OfType<JProperty>()
             .FirstOrDefault(x => x.Name == "jwk")
             ?.Value;
            string kid = result2.ToString();
            string jwk = result3.ToString();


            string payload = "{\"testP\":\"test\"}";
            var headers = new Dictionary<string, object> { { "kid", "test_kid" }, };
           // byte[] secretKey =< secret key bytes here >;
            var jweToken = JWT.Encode(payload, kid, JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM, extraHeaders: headers);



            Console.WriteLine("");
        }
        //  private JWEHeader header;
        private RsaKey rsaKey;
        private IJweAlgorithm header;
        // private JsonConvert.
        // private JsonObject encryptObject = new JsonObject();
    }
}




//kid = the kid references the public key that was used to encrypt the data
//JWK = the key is the public key to which the JWE was encrypted