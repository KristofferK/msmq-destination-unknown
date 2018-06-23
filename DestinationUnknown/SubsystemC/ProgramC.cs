using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SubsystemC
{
    public class ProgramC
    {
        static void Main(string[] args)
        {
            Console.Title = "Subsystem C";

            var messageFromSubsystemAChannel = MessageQueueGenerator.GenerateMessageQueue(MessageQueueGenerator.AToCChannel);
            ReceiveMessageFromSubsystemA(messageFromSubsystemAChannel);

            Console.ReadLine();
        }

        private static void ReceiveMessageFromSubsystemA(MessageQueue channel)
        {
            channel.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            channel.ReceiveCompleted += (object source, ReceiveCompletedEventArgs asyncResult) =>
            {
                MessageQueue messageQueue = (MessageQueue)source;
                var message = messageQueue.EndReceive(asyncResult.AsyncResult);
                var returnAddress = (string)message.Body;

                var addressB = MessageQueueGenerator.AToBChannel;
                Console.WriteLine("Received request from Subsytem A");
                Console.WriteLine("Sending response to: " + returnAddress);
                Console.WriteLine("Body of response: " + addressB);

                var returnChannel = MessageQueueGenerator.GenerateMessageQueue(returnAddress);
                returnChannel.Send(new Message()
                {
                    Body = addressB,
                    CorrelationId = message.Id
                });

                messageQueue.BeginReceive();
            };
            channel.BeginReceive();
        }
    }
}
