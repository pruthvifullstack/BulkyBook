using System;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunction
{
     public class AzureFunction
    {
       [FunctionName("ProcessRegistrationFunction")]
        public void Run(
            [ServiceBusTrigger("register", Connection = "ServiceBusConnectionString")] string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            string emailAddress = ExtractEmailAddress(myQueueItem);

            if (!string.IsNullOrEmpty(emailAddress))
            {
                log.LogInformation($"Extracted email address: {emailAddress}");
            }
            else
            {
                log.LogWarning("No email address found in the message body.");
            }
        }
        private string ExtractEmailAddress(string input)
        {
            string pattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";
            Regex regex = new Regex(pattern);

            Match match = regex.Match(input);

            if (match.Success)
            {
                return match.Value;
            }

            return null;
        }
    }
}
