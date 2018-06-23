using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SubsystemA
{
    class Program
    {
        static void Main(string[] args)
        {
            RequestAdressFromSubsystemC();

            Console.ReadLine();
        }

        private static void RequestAdressFromSubsystemC()
        {
            var channel = MessageQueueGenerator.GenerateMessageQueue(MessageQueueGenerator.AToCChannel);
            // The message queue is solely for retrieving the Address of channel B. So no actual content is needed for the message.
            // Instead we will just set the body to a string containing the Return Address.
            var message = new Message()
            {
                Body = MessageQueueGenerator.CToAChannel
            };

            channel.Send(message);
        }
    }
}
