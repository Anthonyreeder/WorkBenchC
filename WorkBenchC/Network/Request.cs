using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WorkBenchC.Models;
using System.Web;
using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WorkBenchC.Network
{
    class Request
    {
        /// <summary>
        /// Class for building HTTP Requests, Currently also handling responses though i may create a seperate class for this later if needed.
        /// </summary>
        private HttpWebRequest myWebRequest;
        private CookieContainer cookieContainer = new CookieContainer();
        private BrowserType browser;
        private List<Header> headers = new List<Header>();

        //Contructor, sets the browser to use as this is always needed in a request and won't change.
        public Request(BrowserType browser)
        {
            this.browser = browser;
        }
        //Adds cookies to this requests cookiecontainer
        public void addCookie(string name, string value, string domain)
        {
            Cookie cookie = new Cookie(name, value);
            cookie.Domain = domain;
            cookieContainer.Add(cookie);
        }
        
        //Adds headers to this requests headers
        public void addHeaders(List<Header> _headers)
        {
            foreach (var header in _headers)
            {
                    headers.Add(header);
            }
        }

        //helper set browser,headers,and cookies.
        private void prepareRequest(HttpWebRequest myWebRequest)
        {
            myWebRequest.KeepAlive = true; // = "keep-alive"; 
            var cookies = new List<Cookie>();

            var table = (Hashtable)cookieContainer.GetType().InvokeMember("m_domainTable",
                BindingFlags.NonPublic |
                BindingFlags.GetField |
                BindingFlags.Instance,
                null,
                cookieContainer,
                null);

            foreach (string key in table.Keys)
            {
                var item = table[key];
                var items = (ICollection)item.GetType().GetProperty("Values").GetGetMethod().Invoke(item, null);
                foreach (CookieCollection cc in items)
                {
                    foreach (Cookie cookie in cc)
                    {
                        cookies.Add(cookie);
                    }
                }
            }
            myWebRequest.ContentType = "application/json";
            myWebRequest.CookieContainer = cookieContainer;
            foreach (var header in headers)
            {
                addHeader(header);
            }
            setBrowser(browser);
            myWebRequest.Host = "www.pokemoncenter.com";
        //  myWebRequest.Referer = "https://www.pokemoncenter.com/product/701-09383/25th-celebration-galar-region-pikachu-poke-plush-7-3-4-in2";
            
        }

        //response functions
        //GET request and return page source as string value
        public async Task<string> readStringResponseAsync(string url)
        {

            myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            prepareRequest(myWebRequest);

            try
            {
                using (var response = (HttpWebResponse)await myWebRequest.GetResponseAsync())
                {
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        WebHeaderCollection myWebHeaderCollection = response.Headers;
                        for (int i = 0; i < myWebHeaderCollection.Count; i++)
                        {
                            if (myWebHeaderCollection.GetKey(i) == "Set-Cookie" )
                            {
                                //get RAW header
                                var headerRaw = Newtonsoft.Json.JsonConvert.SerializeObject(myWebHeaderCollection.Get(i));
                                
                                //format JSON and remove the junk backslashes that get added.
                                string headerFormatted = headerRaw.Substring(headerRaw.IndexOf("{"), headerRaw.LastIndexOf("}") - headerRaw.IndexOf("{") + 1).Replace(@"\", "");
                                
                                //deserialize into AuthCookie object
                                AuthCookie deserializedHeaderToAuthCookie = JsonConvert.DeserializeObject<AuthCookie>(headerFormatted);

                                //serialize AuthCookie object
                                var serializeAuthCookie = JsonConvert.SerializeObject(deserializedHeaderToAuthCookie);

                                //encode and add to cookie.
                                addCookie("auth", HttpUtility.UrlEncode(serializeAuthCookie), "www.pokemoncenter.com");
                            }
                        }

                        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                        {
                            var stream = readStream.ReadToEnd();

                            return stream;
                        }
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                return "failed";
            }            
        }

        //this was just hacked together really need more thought into this at some point.
        //as requests become more complicated this may be worth expanding into its own class.
        public async Task<string> postData(string url, Dictionary<string, string> postParameters)
        {
            string postData = "{\"productURI\":\"/carts/items/pokemon/qgqvhkjxgays2mbzgm4dg=/form\",\"quantity\":1}"; //Hardcode payload atm

        //    var last = postParameters.Keys.Last();
            foreach (string key in postParameters.Keys)
            {
                string and = "&";
             //   if (key.Equals(last))
            //    {
           //        and = "";
           //     }
                postData += HttpUtility.UrlEncode(key) + "="
                      + HttpUtility.UrlEncode(postParameters[key]) + and;
            }

            myWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            prepareRequest(myWebRequest);

            //set post data
            myWebRequest.Method = "POST";
            byte[] data = Encoding.ASCII.GetBytes(postData);
            myWebRequest.ContentType = "application/json";
            myWebRequest.ContentLength = data.Length;

            //write to stream
            Stream requestStream = myWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            //response
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myWebRequest.GetResponse();

            Stream responseStream = myHttpWebResponse.GetResponseStream();

            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

            string pageContent = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            responseStream.Close();

            myHttpWebResponse.Close();

            return pageContent;          
        }


        //helpers
        private void addHeader(Header header)
        {
            if (myWebRequest.Headers.AllKeys.Contains(header.getName()))
            {
                Console.WriteLine("Key already added " + header.getName());
            }
            else
            {
                if (header.getName() == "accept")
                    myWebRequest.Accept = header.getValue();
                else
                    myWebRequest.Headers[header.getName()] = header.getValue();
            }
        }
        private void setBrowser(BrowserType browserType)
        {
            myWebRequest.Headers.Add("sec-ch-ua", browserType.getSecchua());
            myWebRequest.UserAgent = browserType.getAgent();
        }
    }
}
