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
        private static MessageQueue messageFromSubsystemCChannel;
        static void Main(string[] args)
        {
            Console.Title = "Subsystem A";

            messageFromSubsystemCChannel = MessageQueueGenerator.GenerateMessageQueue(MessageQueueGenerator.CToAChannel);

            RequestAdressFromSubsystemC();
            ReceiveMessageFromSubsystemC();

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
        private static void ReceiveMessageFromSubsystemC()
        {
            messageFromSubsystemCChannel.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            messageFromSubsystemCChannel.ReceiveCompleted += (object source, ReceiveCompletedEventArgs asyncResult) =>
            {
                MessageQueue messageQueue = (MessageQueue)source;
                var message = messageQueue.EndReceive(asyncResult.AsyncResult);
                var SubsystemBAdderss = (string)message.Body;
                Console.WriteLine("Received address of Subsytem B via Subsystem C: " + SubsystemBAdderss);
                SendMessagesToSubsystemB(SubsystemBAdderss);
                messageQueue.BeginReceive();
            };
            messageFromSubsystemCChannel.BeginReceive();
        }

        private static void SendMessagesToSubsystemB(string addresss)
        {
            Console.WriteLine("Sending four messages to Subsystem B");
        }
    }
}
