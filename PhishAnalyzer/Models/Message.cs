using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MsgReader.Outlook;

namespace PhishAnalyzer.Models
{
    public class Message
    {
        public int ID { get; set; }
        public string sender { get; set; }
        public DateTime? recieved { get; set; }
        public string recipient { get; set; }
        public string recipientCC { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        
        public Message()
        {

        }
        public Message(/*int id,*/ string from, DateTime? sentOn, string sentTo, string sentTocc, string subject, string htmlBody)
        {
            //ID = id;
            this.sender = from;
            this.recieved = sentOn;
            this.recipient = sentTo;
            this.recipientCC = sentTocc;
            this.subject = subject;
            this.body = htmlBody;
        }
    }
}
