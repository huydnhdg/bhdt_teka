using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TEKA.Hubs
{
    [HubName("call")]
    public class ClientCall : Hub
    {
        public void Connect()
        {
            //trả về cho client phương thức  connect
            Clients.Caller.connect();
        }
        public void Message(string source, string destination,string name)
        {
            //trả về cho client pt message vs 2 tham số
            Clients.All.message(source, destination,name);
        }
    }
}