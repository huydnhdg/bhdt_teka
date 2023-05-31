using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TEKA.Models;

namespace TEKA.Utils
{
    public class SendCodeQuayThuong
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static string SendCode(QuayThuong model, long? Id)
        {
            TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
            //var code = db.QuayThuongs.Where(a => a.ProductSerial == null).OrderBy(a => Guid.NewGuid()).Take(1).FirstOrDefault();//lay random ma du thuong chua duoc su dung
            var code = db.QuayThuongs.Find(Id);
            code.ProductSerial = model.ProductSerial;
            code.ProductName = model.ProductName;
            code.CustomerPhone = model.CustomerPhone;
            code.CustomerAddress = model.CustomerAddress;
            code.CreateDate = DateTime.Now;
            db.Entry(code).State = System.Data.Entity.EntityState.Modified;
            //gửi brandname ma du thuong cho khach hang
            int sendresult = Utils.SMS.SentSMS(code.CustomerPhone, String.Format("Chuc mung QK da tham gia CT RUOC BO BEP SANG CHAO DON XUAN VANG cua TEKA. Lich Quay Trung Thuong 06/01/2020. Ma du thuong CT {0}. Chi tiet LH 18007227 hoac tekaluckydraw.vn", code.Code));
            if (sendresult != -1)
            {
                logger.Info(String.Format("Send Code QuayThuong {0} {1} {2} {3} {4} {5}", "ok", code.ProductSerial, code.ProductName, code.CustomerPhone, code.CustomerAddress, code.Code));
                
                db.SaveChanges();//save thong tin khach hang vao ma du thuong
            }
            else
            {
                logger.Info(String.Format("Send Code QuayThuong {0} {1} {2} {3} {4} {5}", "error", code.ProductSerial, code.ProductName, code.CustomerPhone, code.CustomerAddress, code.Code));
                
            }
            return code.Code;
        }
        public static string SendCode(QuayThuong model)
        {
            TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
            var code = db.QuayThuongs.Where(a => a.ProductSerial == null).OrderBy(a => Guid.NewGuid()).Take(1).FirstOrDefault();//lay random ma du thuong chua duoc su dung
            code.ProductSerial = model.ProductSerial;
            code.ProductName = model.ProductName;
            code.CustomerPhone = model.CustomerPhone;
            code.CustomerAddress = model.CustomerAddress;
            code.CreateDate = DateTime.Now;
            db.Entry(code).State = System.Data.Entity.EntityState.Modified;
            //gửi brandname ma du thuong cho khach hang
            int sendresult = Utils.SMS.SentSMS(code.CustomerPhone, String.Format("Chuc mung QK da tham gia CT RUOC BO BEP SANG CHAO DON XUAN VANG cua TEKA. Lich Quay Trung Thuong 06/01/2020. Ma du thuong CT {0}. Chi tiet LH 18007227 hoac tekaluckydraw.vn", code.Code));
            if (sendresult != -1)
            {
                logger.Info(String.Format("Send Code QuayThuong {0} {1} {2} {3} {4} {5}", "ok", code.ProductSerial, code.ProductName, code.CustomerPhone, code.CustomerAddress, code.Code));
                db.SaveChanges();//save thong tin khach hang vao ma du thuong
            }
            else
            {
                logger.Info(String.Format("Send Code QuayThuong {0} {1} {2} {3} {4} {5}", "error", code.ProductSerial, code.ProductName, code.CustomerPhone, code.CustomerAddress, code.Code));
            }
            return code.Code;
        }
        public static string SendCode(QuayThuong model, String SMS)
        {
            TEKA.Models.TEKAEntities db = new Models.TEKAEntities();
            var code = db.QuayThuongs.Where(a => a.ProductSerial == null).OrderBy(a => Guid.NewGuid()).Take(1).FirstOrDefault();//lay random ma du thuong chua duoc su dung
            code.ProductSerial = model.ProductSerial;
            code.ProductName = model.ProductName;
            code.CustomerPhone = model.CustomerPhone;
            code.CustomerAddress = model.CustomerAddress;
            code.CreateDate = DateTime.Now;
            db.Entry(code).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            logger.Info(String.Format("Send Code QuayThuong {0} {1} {2} {3} {4} {5}", "ok", code.ProductSerial, code.ProductName, code.CustomerPhone, code.CustomerAddress, code.Code));
            //gửi brandname ma du thuong cho khach hang
            //int sendresult = Utils.SMS.SentSMS(code.CustomerPhone, String.Format("Chuc mung QK da tham gia CT RUOC BO BEP SANG CHAO DON XUAN VANG cua TEKA. Lich Quay Trung Thuong 06/01/2020. Ma du thuong CT {0}. Chi tiet LH 18007227 hoac tekaluckydraw.vn", code.Code));
            //if (sendresult != -1)
            //{
            //    logger.Info(String.Format("Send Code QuayThuong {0} {1} {2} {3} {4} {5}", "ok", code.ProductSerial, code.ProductName, code.CustomerPhone, code.CustomerAddress, code.Code));
            //    db.SaveChanges();//save thong tin khach hang vao ma du thuong
            //}
            //else
            //{
            //    logger.Info(String.Format("Send Code QuayThuong {0} {1} {2} {3} {4} {5}", "error", code.ProductSerial, code.ProductName, code.CustomerPhone, code.CustomerAddress, code.Code));
            //}
            return code.Code;
        }
    }
}