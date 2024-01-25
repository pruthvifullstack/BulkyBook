using Microsoft.Azure.ServiceBus;
using System.Text;

namespace BulkyBookWeb.AzureService
{
    public class ServiceBus : IDisposable
    {
        private readonly IQueueClient _queueClient;

        public ServiceBus(IQueueClient queueClient)
        {
            _queueClient = queueClient ?? throw new ArgumentNullException(nameof(queueClient));
        }

        public async Task SendMessageAsync(string message)
        {
            var messageBody = new Message(Encoding.UTF8.GetBytes(message));
            await _queueClient.SendAsync(messageBody);
        }

        public async Task CloseQueueAsync()
        {
            await _queueClient.CloseAsync();
        }

        //to ensure proper cleanup
        public void Dispose()
        {
            _queueClient?.CloseAsync().GetAwaiter().GetResult();
        }
    }
}
