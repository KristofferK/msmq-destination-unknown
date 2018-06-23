using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public static class MessageQueueGenerator
    {
        public static string AToBChannel { get; private set; } = @".\Private$\Destination_Unknown_A_To_B_Channel";
        public static string BToAChannel { get; private set; } = @".\Private$\Destination_Unknown_B_To_A_Channel";
        public static string AToCChannel { get; private set; } = @".\Private$\Destination_Unknown_A_To_C_Channel";
        public static string CToAChannel { get; private set; } = @".\Private$\Destination_Unknown_C_To_A_Channel";

        public static MessageQueue GenerateMessageQueue(string messageQueueName)
        {
            if (!MessageQueue.Exists(messageQueueName))
            {
                MessageQueue.Create(messageQueueName);
            }
            return new MessageQueue(messageQueueName)
            {
                Label = "I'm located at " + messageQueueName
            };
        }
    }
}
