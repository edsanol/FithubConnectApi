using Application.Interfaces;
using Domain.Entities;
using FirebaseAdmin.Messaging;
using Infrastructure.Persistences.Contexts;

namespace Application.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly DbFithubContext _context;

        public PushNotificationService(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task SendPushNotificationAsync(List<string> deviceTokens, string title, string body)
        {
            if (deviceTokens == null || deviceTokens.Count == 0)
                return;

            var message = new MulticastMessage
            {
                Tokens = deviceTokens,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                // Opcional: enviar datos extras
                // Data = new Dictionary<string, string>
                // {
                //     { "key1", "value1" },
                //     { "key2", "value2" }
                // }
            };

            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

            //for (int i = 0; i < response.Responses.Count; i++)
            //{
            //    var sendResponse = response.Responses[i];

            //    if (!sendResponse.IsSuccess)
            //    {
            //        var failedToken = deviceTokens[i];

            //        var exception = sendResponse.Exception;
            //        if (exception is FirebaseMessagingException fcmEx)
            //        {
                        
            //        }
            //    }
            //}
        }
    }
}
