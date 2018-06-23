using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class MessageWithInteger
    {
        public int Value { get; set; }
        public string Guid { get; set; }
        public string ReplyTo { get; set; }

        public MessageWithInteger()
        {
        }

        public override string ToString()
        {
            return $"{Value} (guid: {Guid}) (ReplyTo: {ReplyTo}";
        }
    }
}
