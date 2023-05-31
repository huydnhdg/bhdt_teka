using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;

namespace TEKA.Utils
{
    public static class PACallAPI
    {
        static string api_key = WebConfigurationManager.AppSettings["TOKEN_PA"];
        static string v2 = "ok";
        
        public static string GetCDRReport(PA pa)
        {
            string result = "";
            try
            {
                var url = "https://crm.pavietnam.vn/api/getCDRReport.php?api_key=" + api_key + "&v2=" + v2
                + "&from_date=" + pa.from_date + "&to_date=" + pa.to_date + "&source=" + pa.source
                + "&destination=" + pa.destination + "&status=" + pa.status
                + "&limit=" + pa.limit;
                   // + "&page=" + pa.page;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";

                httpRequest.ContentType = "application/json";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                Console.WriteLine(httpResponse.StatusCode);
            }
            catch (Exception ex)
            {

            }

            return result;

        }
        public static string CallNow(string key, string phone)
        {
            string result = "";
            try
            {
                var url = "https://crm.pavietnam.vn/api/callNow.php?api_key=" + api_key + "&extension=" + key + "&phone=" + phone;

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";

                httpRequest.ContentType = "application/json";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                Console.WriteLine(httpResponse.StatusCode);
            }
            catch (Exception ex)
            {

            }

            return result;

        }
        public class PAModel
        {
            public string action { get; set; }
            public string source { get; set; }
            public string destination { get; set; }
            public string uniqueid { get; set; }
            public string cusname { get; set; }
        }
        public class PA
        {
            public string api_key { get; set; }
            public string to_date { get; set; }
            public string from_date { get; set; }
            public int duration_max { get; set; }
            public int duration_min { get; set; }
            public int limit { get; set; }
            public string status { get; set; }
            public int? source { get; set; }
            public int? destination { get; set; }
            public int page { get; set; }
            public string v2 { get; set; }

        }
        public class PA_Response
        {
            public int code { get; set; }
            public string message { get; set; }
            public string data { get; set; }
        }
        public class DataModel
        {
            public DateTime date_create { get; set; }
            public string from { get; set; }
            public string to { get; set; }
            public int duration { get; set; }
            public int bill { get; set; }
            public string recording { get; set; }
            public string outbound { get; set; }
        }
    }
}