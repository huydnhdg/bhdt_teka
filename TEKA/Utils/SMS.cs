using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.Utils
{
    public class SMS
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static int SentSMS(string phone, string message)
        {
            return SentMTMessage(message, phone, "TEKA", "TEKA", "0");
        }
        public static int SentMTMessage(string message, string phone, string commandCode, string cpNumber, string requestId)
        {

            string messageSent = EncodeTo64(message);
            string phoneSent = phone.StartsWith("+") ? phone.Replace("+", "") : phone;
            if (phoneSent.StartsWith("0"))
            {
                string test = phoneSent.Substring(1);
                phoneSent = string.Concat("84", test);
            }
            if (!phoneSent.StartsWith("84"))
            {

                phoneSent = string.Concat("84", phoneSent);
            }
            int sendMessageResult = -1;
            try
            {
                SendMsgReceiver smsSend = new SendMsgReceiver();
                smsSend.UserName = "tekasms";
                smsSend.Password = "tekasms123456";
                smsSend.PreAuthenticate = true;
                sendMessageResult = smsSend.sendMT(phoneSent, messageSent, cpNumber, commandCode, "1", requestId, "1", "1", "0", "0");
                return sendMessageResult;
            }
            catch (Exception ex)
            {
                logger.Info(String.Format("CustomerActive-Active-sendMT-{0}", ex.Message));
                //log.Error(String.Format("CustomerActive-Active-sendMT-{0}", ex.Message));
                return sendMessageResult;
            }
        }
        static private string EncodeTo64(string toEncode)
        {

            byte[] toEncodeAsBytes

                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

            string returnValue

                  = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }
    }
}