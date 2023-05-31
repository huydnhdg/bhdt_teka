using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.Utils
{
    public class Robot
    {
        public static string Get_Code_Warranti(string stt)
        {
            string date = DateTime.Now.ToString("yyMMdd");
            string index = stt.PadLeft(6, '0');
            string code = date + index;
            return code;
        }

        public static string getnamestatus(int? status)
        {
            string str;
            switch (status)
            {
                case 9:
                    str = "Hủy bỏ";
                    break;
                case 0:
                    str = "Mới tiếp nhận";
                    break;
                case 1:
                    str = "Hoàn thành";
                    break;
                case 2:
                    str = "Chuyển trạm";
                    break;
                case 3:
                    str = "Trạm từ chối";
                    break;
                case 5:
                    str = "Đang xử lý";
                    break;
                case 6:
                    str = "Đem sản phẩm về trạm";
                    break;
                case 7:
                    str = "Đang đợi linh kiện";
                    break;
                case 8:
                    str = "Chờ khách phản hồi";
                    break;
                default:
                    str = "Mới tạo";
                    break;
            }
            return str;
        }
    }
}