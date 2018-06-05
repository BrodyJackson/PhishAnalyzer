using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhishAnalyzer.Models
{
    public class Message
    {
        public int ID { get; set; }
        public String sender { get; set; }
        public String subject { get; set; }
        public String body { get; set; }
        public DateTime recieved { get; set; }


    }
}
