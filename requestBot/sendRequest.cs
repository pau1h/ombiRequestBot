using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Net;
using System.IO;
using Newtonsoft.Json;
namespace requestBot
{

    public class requestHeaders
    {
        public int theMovieDbId { get; set; }
    }
    class sendRequest
    {
        public static String requestMovie(int ID, String API) //api for ombi, returns the http result as a string
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.1.219:5000/api/v1/Request/movie");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("ApiKey: " + API);


            //passes requestHeaders
            requestHeaders headers = new requestHeaders();
            headers.theMovieDbId = ID;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(headers);

                streamWriter.Write(json);
            }



            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string result = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            
            return result;
        }

    }

}
