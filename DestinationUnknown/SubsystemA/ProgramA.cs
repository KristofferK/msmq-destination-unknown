using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SubsystemA
{
    class ProgramA
    {
        static void Main(string[] args)
        {
            Console.Title = "Subsystem A";

            var messageFromSubsystemCChannel = MessageQueueGenerator.GenerateMessageQueue(MessageQueueGenerator.CToAChannel);
            ReceiveMessageFromSubsystemC(messageFromSubsystemCChannel);
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

            Console.WriteLine("Requesting the address of Subsystem B trhough Subsystem C");
            channel.Send(message);
        }
        private static void ReceiveMessageFromSubsystemC(MessageQueue channel)
        {
            channel.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            channel.ReceiveCompleted += (object source, ReceiveCompletedEventArgs asyncResult) =>
            {
                MessageQueue messageQueue = (MessageQueue)source;
                var message = messageQueue.EndReceive(asyncResult.AsyncResult);
                var SubsystemBAdderss = (string)message.Body;
                Console.WriteLine("Received address of Subsytem B via Subsystem C: " + SubsystemBAdderss);
                SendMessagesToSubsystemB(SubsystemBAdderss);
                messageQueue.BeginReceive();
            };
            channel.BeginReceive();
        }

        private static void SendMessagesToSubsystemB(string addresss)
        {
            Console.WriteLine("** Should send four messages to Subsystem B **");
        }
    }
}
