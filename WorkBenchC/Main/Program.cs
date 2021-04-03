using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WorkBenchC.Models;
using WorkBenchC.Models.Browsers;
using WorkBenchC.Network;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace WorkBenchC
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create request, assign headers and cookie for DD
            Request request = new Request(new Chrome());
            List<Header> Headers = pokemonCenterHeaders();
            request.addHeaders(Headers);
            request.addCookie("datadome", "2.tOn8t1cr64bWqCMBNH2mBMiOss2I~2lziwm4FSXvgLbI5u2z0bgFeugL41m-4N6U857VR.PZVTiApwtpYUrxr-i9vN5NDbSC7ga2m3Nh", ".pokemoncenter.com");

            //Call GET here to assign SET auth cookie to container
            string setAuthCookie  = request.readStringResponseAsync("https://www.pokemoncenter.com/tpci-ecommweb-api/order?type=status&format=zoom.nodatalinks").Result;

            //ATC JSON 
            string productId = request.readStringResponseAsync("https://www.pokemoncenter.com/product/701-09383").Result;

            //load html in HtmlAgility
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(productId);

            //extract script using HtmlAgility
            var scripts = document.GetElementbyId("__NEXT_DATA__");
            
            //deserialize json
            var routes_list = (JObject)JsonConvert.DeserializeObject(scripts.InnerText);
            var result = routes_list.Descendants()
                 .OfType<JProperty>()
                 .FirstOrDefault(x => x.Name == "addToCartForm")
                 ?.Value;

            //parse ID from JSON
            var id = ParseRawJToken(result, "pokemon/", "/form");

            Dictionary<string, string> post = new Dictionary<string, string>();

            string postAtc = request.postData("https://www.pokemoncenter.com/tpci-ecommweb-api/cart?type=product&format=zoom.nodatalinks", post).Result;
            
            
            Console.ReadLine();
        }

        static List<Header> pokemonCenterHeaders()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("Accept-Encoding", "0"));
            headers.Add(new Header("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            headers.Add(new Header("x-store-scope", "pokemon"));

            return headers;
        }
        static string ParseRawJToken(JToken raw, string start, string end)
        {
            var addToCartUrl = raw.ToString();
            var encodedUrl = addToCartUrl.Substring(
                    addToCartUrl.IndexOf(start),
                    addToCartUrl.IndexOf(end) - addToCartUrl.IndexOf(start)).Substring(start.Length);

            return encodedUrl;
        }
    }
}
