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
          //  RsaKey rsaKey;
            RsaKeyManagement test;
            JweToken t;
           
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
            //rsaKey = result3.ToString();
            //   string kid =
            //tring kid = jsonObject.get("flx").getAsJsonObject().get("jwk").getAsJsonObject().get("kid").getAsString();

            // byte[] secretKey =< secret key bytes here >;
            

            //snippet from stackoverflow
           // string payload = "";
            var headers = new Dictionary<string, object> { { "kid", kid }, };
            byte[] secretKey = data; //not sure if this is the right key? Or if we should be using the entire key. (this is split)
           // var jweToken = JWT.Encode(payload, secretKey, JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM, extraHeaders: headers);
            
            var payload = new Dictionary<string, object>()
            {
                { "kid", kid },
                { "jwt", result3 }
            };
            RsaKey key = new RsaKey();
            string token = Jose.JWT.Encode(payload, key, JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM);
            //var jweToken = JWT.Encode(payload, keyID, JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM, extraHeaders: headers);
            //JObject jweObject = new JObject(jweToken, payload);
            //jweObject.encrypt(new AESEncrypter(key));
            //String encryptedPayload = jweObject.serialize();

            Console.WriteLine("");
        }
        //  private JWEHeader header;
        private RsaKey rsaKey;
        private IJweAlgorithm header;
        // private JsonConvert.
        // private JsonObject encryptObject = new JsonObject();
    }
}
