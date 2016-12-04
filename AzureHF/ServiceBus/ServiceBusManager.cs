using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureHF.ServiceBus
{
    public class ServiceBusManager
    {


        public static void Log(string msg)
        {
            QueueClient client = QueueClient.CreateFromConnectionString("Endpoint=sb://azurehf.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8C4K/YMT6YPmsyJRp4e1IvRtDhIzcr0EMgi53ATSXf8=", "authlog");
            using (var bm = new BrokeredMessage(msg))
            {
                client.Send(bm);
            }
        }

    }
}