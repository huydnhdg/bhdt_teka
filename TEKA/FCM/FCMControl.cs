using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace TEKA.FCM
{
    public class FCMControl
    {
        public string SendNotification(object data)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var json = serializer.Serialize(data);
            Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);

            return SendNotification(byteArray);
        }
        public string SendNotification(Byte[] byteArray)
        {
            try
            {

                string SERVER_API_KEY = "AAAAnAt6nOw:APA91bEa4Y--dy5UKq7Nn5Nj90xD0lVANV6XXHFtUuq7ze9bDzOVDUW_2Vrz5AWGlTdSqp_849EoLFIzHsjDZAhNhchUeZa5NwQPYCnr48CxC32XbuqOvO2rt2MfSMMhEfl6nx7aU3uZ";
                string SENDER_ID = "670207483116";

                WebRequest tRequest;
                tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();

                dataStream = tResponse.GetResponseStream();

                StreamReader tReader = new StreamReader(dataStream);

                String sResponseFromServer = tReader.ReadToEnd();

                tReader.Close();
                dataStream.Close();
                tResponse.Close();
                return sResponseFromServer;
            }
            catch (Exception)
            {                
                throw;
            }
        }
        
    }
}