using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace TEKA.Utils
{
    public class ConvertTo
    {
        public static DateTime? TryParse(string stringDate)
        {
            DateTime date;
            return DateTime.TryParse(stringDate, out date) ? date : (DateTime?)null;
        }
        public static String FormatPhoneStartWith84(string PhoneNumber)
        {
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                return PhoneNumber;
            }
            return Utils.FormatString.formatUserId(PhoneNumber, 2);
        }
    }
}