﻿using Newtonsoft.Json;
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
using WorkBenchC.Tools;

namespace WorkBenchC
{
    class Program
    {
        /// <summary>
        /// Main used as a test. To be moved to site model when finished.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Create request, assign headers and cookie for DD
            Request request = new Request(new Chrome());
            List<Header> Headers = pokemonCenterHeaders();
            request.addHeaders(Headers);
            request.addCookie("datadome", "GBDGJ9ODTA_Sh6dWKg-v-t7i~3rwkY0gWWOazLWo4EHF8rkYBr8Myx65SK35QLA3VEnDIIRdLjPyWcFtWe9Nlw.hclqEw7L605~~XM6Pgu", ".pokemoncenter.com");

            //Call GET here to assign SET auth cookie to container
            string setAuthCookie  = request.readStringResponseAsync("https://www.pokemoncenter.com/tpci-ecommweb-api/order?type=status&format=zoom.nodatalinks").Result;

            //ATC containing JSON we need 
            string productId = request.readStringResponseAsync("https://www.pokemoncenter.com/product/701-09383").Result;

            //load html in HtmlAgility
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(productId);

            //extract script using HtmlAgility
            var scripts = document.GetElementbyId("__NEXT_DATA__");

            //get ATC value from json
            var idRaw = retrieveValue(scripts.InnerText, "addToCartForm");
            var idFormatted = parseRawJToken(idRaw, "pokemon/", "/form");

            //prepare json for atc request
            Dictionary<string, string> post = new Dictionary<string, string>();
            string postData = "{\"productURI\":\"/carts/items/pokemon/"+ idFormatted + "/form\",\"quantity\":1}"; //Hardcode payload atm

            //atc request
            string postAtc = request.postData("https://www.pokemoncenter.com/tpci-ecommweb-api/cart?type=product&format=zoom.nodatalinks", postData).Result;

            //format address
            AddressFormatted address = pokemonCenterAddress();
            var serializeAddressFormatted = JsonConvert.SerializeObject(address);

            //format address request
            string postAddressFormat = request.postData("https://www.pokemoncenter.com/tpci-ecommweb-api/address?format=zoom.nodatalinks", serializeAddressFormatted).Result;

            //Payment key Id
            string keyIdRaw = request.readStringResponseAsync("https://www.pokemoncenter.com/tpci-ecommweb-api/payment/key?microform=true&locale=en-US").Result;
            var keyIdFormatted = retrieveValue(keyIdRaw, "keyId");

            CyberSourcev2 test = new CyberSourcev2(keyIdFormatted.ToString());

            Console.ReadLine();
        }

        static AddressFormatted pokemonCenterAddress()
        {
            AddressFormatted address = new AddressFormatted();
            Billing billingAddr = new Billing();
            billingAddr.familyName = "reeder";
            billingAddr.givenName = "Ant";
            billingAddr.streetAddress = "828  Big Indian";
            billingAddr.extendedAddress = "New Orleans";
            billingAddr.locality = "New Orleans";
            billingAddr.region = "LA";
            billingAddr.postalCode = "70112";
            billingAddr.countryName = "US";
            billingAddr.phoneNumber = "(504) 550-6828";
            billingAddr.email = "anthonyreeder123@gmail.com";
            Shipping shippingAddr = new Shipping();
            shippingAddr.familyName = "reeder";
            shippingAddr.givenName = "Ant";
            shippingAddr.streetAddress = "828  Big Indian";
            shippingAddr.extendedAddress = "New Orleans";
            shippingAddr.locality = "New Orleans";
            shippingAddr.region = "LA";
            shippingAddr.postalCode = "70112";
            shippingAddr.countryName = "US";
            shippingAddr.phoneNumber = "(504) 550-6828";
            shippingAddr.email = "anthonyreeder123@gmail.com";


            address.billing = billingAddr;
            address.shipping = shippingAddr;

            return address;
        }
        static List<Header> pokemonCenterHeaders()
        {
            List<Header> headers = new List<Header>();
            headers.Add(new Header("Accept-Encoding", "0"));
            headers.Add(new Header("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"));
            headers.Add(new Header("x-store-scope", "pokemon"));

            return headers;
        }
        static string parseRawJToken(JToken raw, string start, string end)
        {
            var addToCartUrl = raw.ToString();
            var encodedUrl = addToCartUrl.Substring(
                    addToCartUrl.IndexOf(start),
                    addToCartUrl.IndexOf(end) - addToCartUrl.IndexOf(start)).Substring(start.Length);

            return encodedUrl;
        }
        static JToken retrieveValue(string source, string value)
        {
            var routes_list = (JObject)JsonConvert.DeserializeObject(source);
            var result = routes_list.Descendants()
                 .OfType<JProperty>()
                 .FirstOrDefault(x => x.Name == value)
                 ?.Value;

            return result;
        }
    }
}
