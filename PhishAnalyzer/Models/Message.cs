using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using MsgReader.Mime.Header;
using MsgReader.Outlook;
using Org.Mentalis.Utilities; 

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
        public string headerSender { get; set; }
        public string senderDomain { get; set; }
        //risk not yet added to db model
        public string riskRating { get; set; }
        //mx not yet added to db model
        public string mxRecords { get; set; }
        public string senderDomainRegDate { get; set; }
        public bool headerSenderValid { get; set; }
        public Message()
        {

        }
        public Message(/*int id,*/ string from, DateTime? sentOn, string sentTo, string sentTocc, string subject, string htmlBody, string headerSender, string senderDomain, bool headerSenderValid)
        {
            //ID = id;
            this.sender = from;
            this.recieved = sentOn;
            this.recipient = sentTo;
            this.recipientCC = sentTocc;
            this.subject = subject;
            this.body = htmlBody;
            this.headerSender = headerSender;
            this.senderDomain = senderDomain;
            this.mxRecords = "Mx Records will go here when there is network access"; //Find_MX_records(senderDomain); //add this in after the rest works
            this.senderDomainRegDate = "registration date will go here when there is network access"; //Find_domaion_creation_date(senderDomain);
            this.headerSenderValid = headerSenderValid; 
            this.riskRating = "XXXXX";  //getRiskRating(); will be uncommented when network access is provided hardcoded for now 
        }

        public string getRiskRating()
        {
            string rating = "";
            //no subject means that is is suspicious
            if (this.subject == null) rating += "X";

            //check if the sender is spoofed
            //skipped for now

            //check if there are no MX records
            if (this.mxRecords == "No MX records.") rating += "X";

            DateTime timeRecieved = Convert.ToDateTime(this.recieved);


            DateTime domainRegister;// = Convert.ToDateTime(correoAnalysis.AVisibleSenderDomainRegistrationDate);

            DateTime date = DateTime.MinValue;
            String strDate = this.senderDomainRegDate;
            if (DateTime.TryParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                //your string was successfully converted to dateTime (date variable holds it)
                domainRegister = date;
            }
            else
            {
                domainRegister = Convert.ToDateTime(this.senderDomainRegDate);

            }

            TimeSpan difference = timeRecieved - domainRegister;

            //Check on days between the message recieved and the creation of the spoofed Sender Domain Name 
            if (difference.Days < 8) rating += "X";

            //potentail forged IP sender
            //if (this.senderIP.Contains("(may be forged)")) rating += "X";

            //Check for spelling errors within th body of email

            return rating;

        }



        private String Find_MX_records(String pDomainName)
        {
            String dnsServer = "8.8.8.8"; //let's use Mr. Google. 

            DnsEx.DnsServers = new string[] { dnsServer };
            DnsEx dns = new DnsEx();
            DnsServerResponse response = null;

            String hostname = pDomainName;

            try
            {
                response = dns.Query(hostname, QTYPE.MX);
            }
            catch (Exception ex)
            {
                //throw new Exception("Network: Failed due to network connectivity of DNS to 8.8.8.8" + ex.ToString());
                return "DNS queries to 8.8.8.8 are not allowed";
            }

            StringBuilder sb = new StringBuilder();

            foreach (object rec in response.Answers)
            {

                if (rec is NS_Record)
                {
                    sb.Append(((NS_Record)rec).NameServer + "\n");
                }
                else if (rec is CNAME_Record)
                {
                    sb.Append(((CNAME_Record)rec).Alias + "\n");
                }

                else if (rec is MX_Record)
                {
                    sb.Append(((MX_Record)rec).Host + " ");
                    sb.Append("(Preference: " + ((MX_Record)rec).Preference.ToString() + ") \n");
                }
            }

            if (sb.ToString() == "")
                return "No MX records.";
            else
                return sb.ToString();
        }

        private String Find_domaion_creation_date(String pSenderSmtpAddressDomain)
        {
            String creationDate = "";
            String fechaDomain = "";

            try
            {
                fechaDomain = WhoisResolver.Whois2(pSenderSmtpAddressDomain);
            }
            catch (Exception ex)
            {
                throw new Exception(("Network: Whoisresolved failed" + ex.ToString()));
            }


            String[] fechaDomainKeys = fechaDomain.Split('\n');

            Int32 indice = 0;

            foreach (string key in fechaDomainKeys)
            {
                indice = indice + 1;
                if (key.ToUpper().Contains("CREATION"))
                {
                    indice = indice - 1;
                    break;
                }
            }

            if (indice != fechaDomainKeys.Length)
            {
                creationDate = fechaDomainKeys[indice].Split(':')[1];
                creationDate = creationDate.Replace(" ", "");
                creationDate = creationDate.Substring(0, 10);
            }
            else
            {
                creationDate = DateTime.Today.ToShortDateString();
            }

            return creationDate;
        }
    }
}
