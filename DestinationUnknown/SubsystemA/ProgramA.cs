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
        private static Random random = new Random();
        static void Main(string[] args)
        {
            Console.Title = "Subsystem A";

            var messageFromSubsystemCChannel = MessageQueueGenerator.GenerateMessageQueue(MessageQueueGenerator.CToAChannel);
            var messageFromSubsystemBChannel = MessageQueueGenerator.GenerateMessageQueue(MessageQueueGenerator.BToAChannel);
            ReceiveMessageFromSubsystemC(messageFromSubsystemCChannel);
            ReceiveMessageFromSubsystemB(messageFromSubsystemBChannel);
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

            Console.WriteLine("Requesting the address of Subsystem B through Subsystem C");
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
                Console.WriteLine($"Received address of Subsytem B via Subsystem C: {SubsystemBAdderss}\n");
                SendMessagesToSubsystemB(SubsystemBAdderss);
                messageQueue.BeginReceive();
            };
            channel.BeginReceive();
        }

        private static void SendMessagesToSubsystemB(string addresss)
        {
            var channel = MessageQueueGenerator.GenerateMessageQueue(addresss);
            var guids = new string[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };
            for (var i = 0; i < 4; i++)
            {
                var message = new Message()
                {
                    Body = new MessageWithInteger()
                    {
                        Value = random.Next(1, 50),
                        Guid = guids[i / 2],
                        ReplyTo = MessageQueueGenerator.BToAChannel
                    }
                };
                Console.WriteLine($"Sending message {i + 1}: {(MessageWithInteger)message.Body}");
                channel.Send(message);
                if (i % 2 == 1)
                {
                    Console.WriteLine($"Id of sent message is {message.Id}\n");
                }
            }
        }

        private static void ReceiveMessageFromSubsystemB(MessageQueue channel)
        {
            channel.Formatter = new XmlMessageFormatter(new Type[] { typeof(Int32) });
            channel.MessageReadPropertyFilter.SetAll();
            channel.ReceiveCompleted += (object source, ReceiveCompletedEventArgs asyncResult) =>
            {
                MessageQueue messageQueue = (MessageQueue)source;
                var message = messageQueue.EndReceive(asyncResult.AsyncResult);
                var sum = (Int32)message.Body;
                Console.WriteLine($"Received from B: Sum of message {message.CorrelationId} is {sum}");
                messageQueue.BeginReceive();
            };
            channel.BeginReceive();
        }
    }
}
