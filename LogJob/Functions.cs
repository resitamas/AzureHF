using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace LogJob
{
    public class Functions
    {
        
        public static void ProcessQueueMessage([ServiceBusTrigger("authlog")] string message, TextWriter logger)
        {
            logger.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
