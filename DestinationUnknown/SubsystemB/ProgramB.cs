using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SubsystemB
{
    class ProgramB
    {
        private static object lockObject = new object();
        private static Dictionary<string, int> savedValues = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            Console.Title = "Subsystem B";

            var messageFromSubsystemAChannel = MessageQueueGenerator.GenerateMessageQueue(MessageQueueGenerator.AToBChannel);
            ReceiveMessageFromSubsystemA(messageFromSubsystemAChannel);

            Console.ReadLine();
        }

        private static void ReceiveMessageFromSubsystemA(MessageQueue channel)
        {
            channel.Formatter = new XmlMessageFormatter(new Type[] { typeof(MessageWithInteger) });
            channel.ReceiveCompleted += (object source, ReceiveCompletedEventArgs asyncResult) =>
            {
                MessageQueue messageQueue = (MessageQueue)source;
                var message = messageQueue.EndReceive(asyncResult.AsyncResult);
                var body = (MessageWithInteger)message.Body;

                lock (lockObject)
                {
                    Console.WriteLine("Received integer from Subsytem A " + body);
                    if (savedValues.ContainsKey(body.Guid))
                    {
                        Console.WriteLine("We've received this Guid previously. Calculating sum and sending reply to System A\n");

                        var sum = savedValues[body.Guid] + body.Value;
                        savedValues.Remove(body.Guid);

                        var returnChannel = MessageQueueGenerator.GenerateMessageQueue(body.ReplyTo);
                        returnChannel.Send(new Message()
                        {
                            Body = sum,
                            CorrelationId = message.Id
                        });
                    }
                    else
                    {
                        Console.WriteLine("This is the first message with this Guid. Saving value in state.");
                        savedValues[body.Guid] = body.Value;
                    }

                    messageQueue.BeginReceive();
                }

            };
            channel.BeginReceive();
        }
    }
}
