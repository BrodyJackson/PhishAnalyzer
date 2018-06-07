using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Text;

namespace Helpers
{
    #region enum OPCODE

    /// <summary>
    /// 
    /// </summary>
    public enum OPCODE
    {
        /// <summary>
        ///  a standard query.
        /// </summary>
        QUERY = 0,

        /// <summary>
        /// an inverse query.
        /// </summary>
        IQUERY = 1,

        /// <summary>
        /// a server status request.
        /// </summary>
        STATUS = 2,
    }

    #endregion

    #region enum QTYPE

    /// <summary>
    /// ´Query type.
    /// </summary>
    public enum QTYPE
    {
        /// <summary>
        /// IPv4 host address
        /// </summary>
        A = 1,

        /// <summary>
        /// an authoritative name server
        /// </summary>
        NS = 2,

        //	MD    = 3,  Obsolete
        //	MF    = 5,  Obsolete

        /// <summary>
        /// the canonical name for an alias
        /// </summary>
        CNAME = 5,

        /*	/// <summary>
			/// marks the start of a zone of authority
			/// </summary>
			SOA   = 6,  */

        //	MB    = 7,  EXPERIMENTAL
        //	MG    = 8,  EXPERIMENTAL
        //  MR    = 9,  EXPERIMENTAL
        //	NULL  = 10, EXPERIMENTAL

        /*	/// <summary>
			/// a well known service description
			/// </summary>
			WKS   = 11, */

        /// <summary>
        /// a domain name pointer
        /// </summary>
        PTR = 12,

        /*	/// <summary>
			/// host information
			/// </summary>
			HINFO = 13, 

			/// <summary>
			/// mailbox or mail list information
			/// </summary>
			MINFO = 14, */

        /// <summary>
        /// mail exchange
        /// </summary>
        MX = 15,

        /// <summary>
        /// text strings
        /// </summary>
        TXT = 16,

        /// <summary>
        /// IPv6 host address
        /// </summary>
        AAAA = 28,

        /*	/// <summary>
			/// UnKnown
			/// </summary>
			UnKnown = 9999, */
    }

    #endregion

    #region enum RCODE

    /// <summary>
    /// Dns server reply codes.
    /// </summary>
    public enum RCODE
    {
        /// <summary>
        /// No error condition.
        /// </summary>
        NO_ERROR = 0,

        /// <summary>
        /// Format error - The name server was unable to interpret the query.
        /// </summary>
        FORMAT_ERRROR = 1,

        /// <summary>
        /// Server failure - The name server was unable to process this query due to a problem with the name server.
        /// </summary>
        SERVER_FAILURE = 2,

        /// <summary>
        /// Name Error - Meaningful only for responses from an authoritative name server, this code signifies that the
        /// domain name referenced in the query does not exist.
        /// </summary>
        NAME_ERROR = 3,

        /// <summary>
        /// Not Implemented - The name server does not support the requested kind of query.
        /// </summary>
        NOT_IMPLEMENTED = 4,

        /// <summary>
        /// Refused - The name server refuses to perform the specified operation for policy reasons.
        /// </summary>
        REFUSED = 5,
    }

    #endregion

    /// <summary>
    /// Dns reply codes.
    /// </summary>
    public enum DnsReplyCode
    {
        // TODO: remove me

        /// <summary>
        /// Requested records retrieved sucessfully.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// No requested records found.
        /// </summary>
        NoEntries = 1,

        /// <summary>
        /// There was error retrieving records.
        /// </summary>
        TempError = 2,
    }

    /// <summary>
    /// Dns resolver.
    /// </summary>
    /// <example>
    /// <code>
    /// // Set dns servers
    /// DnsEx.DnsServers = new string[]{"194.126.115.18"};
    /// 
    /// DnsEx dns = DnsEx();
    /// 
    /// // Get MX records
    /// MX_Record[] mxRecords = dns.GetMXRecords("lumisoft.ee")
    /// 
    /// // Do your stuff
    /// </code>
    /// </example>
    public class DnsEx
    {
        private static string[] m_DnsServers = null;
        private static bool m_UseDnsCache = true;
        private static int m_ID = 100;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DnsEx()
        {
        }


        #region Query

        /// <summary>
        /// Queries server with specified query.
        /// </summary>
        /// <param name="queryText">Query text. It depends on queryType.</param>
        /// <param name="queryType">Query type.</param>
        /// <returns></returns>
        public DnsServerResponse Query(string queryText, QTYPE queryType)
        {
            if (queryType == QTYPE.PTR)
            {
                string ip = queryText;

                // See if IP is ok.
                IPAddress ipA = IPAddress.Parse(ip);
                queryText = "";

                // IPv6
                if (ipA.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    // 4321:0:1:2:3:4:567:89ab
                    // would be
                    // b.a.9.8.7.6.5.0.4.0.0.0.3.0.0.0.2.0.0.0.1.0.0.0.0.0.0.0.1.2.3.4.IP6.ARPA

                    char[] ipChars = ip.Replace(":", "").ToCharArray();
                    for (int i = ipChars.Length - 1; i > -1; i--)
                    {
                        queryText += ipChars[i] + ".";
                    }
                    queryText += "IP6.ARPA";
                }
                // IPv4
                else
                {
                    // 213.35.221.186
                    // would be
                    // 186.221.35.213.in-addr.arpa

                    string[] ipParts = ip.Split('.');
                    //--- Reverse IP ----------
                    for (int i = 3; i > -1; i--)
                    {
                        queryText += ipParts[i] + ".";
                    }
                    queryText += "in-addr.arpa";
                }
            }

            return QueryServer(queryText, queryType, 1);
        }

        #endregion

        #region GetARecords

        /// <summary>
        /// Gets IPv4 host addess records.
        /// </summary>
        /// <param name="domain">Domain name which A records to get.</param>
        /// <returns></returns>
        [Obsolete("Use DnsEx.Query instead")]
        public A_Record[] GetARecords(string domain)
        {
            ArrayList answers = QueryServer(domain, QTYPE.A, 1).Answers;

            return (A_Record[])FilterRecords(answers, typeof(A_Record)).ToArray(typeof(A_Record));
        }

        #endregion

        #region GetNSRecords

        /// <summary>
        /// Gets name server records.
        /// </summary>
        /// <param name="domain">Domain name which A records to get.</param>
        /// <returns></returns>
        [Obsolete("Use DnsEx.Query instead")]
        public NS_Record[] GetNSRecords(string domain)
        {
            ArrayList answers = QueryServer(domain, QTYPE.NS, 1).Answers;

            return (NS_Record[])FilterRecords(answers, typeof(NS_Record)).ToArray(typeof(NS_Record));
        }

        #endregion

        #region GetCNAMERecords

        /// <summary>
        /// Gets CNAME records.
        /// </summary>
        /// <param name="domain">Domain name which CNAME records to get.</param>
        /// <returns></returns>
        [Obsolete("Use DnsEx.Query instead")]
        public CNAME_Record[] GetCNAMERecords(string domain)
        {
            ArrayList answers = QueryServer(domain, QTYPE.CNAME, 1).Answers;

            return (CNAME_Record[])FilterRecords(answers, typeof(CNAME_Record)).ToArray(typeof(CNAME_Record));
        }

        #endregion

        #region GetPTRRecords

        /// <summary>
        /// Gets PTR records.
        /// </summary>
        /// <param name="ip">IP address which domain names to get.</param>
        /// <returns></returns>
        [Obsolete("Use DnsEx.Query instead")]
        public PTR_Record[] GetPTRRecords(string ip)
        {
            // See if IP is ok.
            IPAddress ipA = IPAddress.Parse(ip);
            string query = "";

            // IPv6
            if (ipA.AddressFamily == AddressFamily.InterNetworkV6)
            {
                // 4321:0:1:2:3:4:567:89ab
                // would be
                // b.a.9.8.7.6.5.0.4.0.0.0.3.0.0.0.2.0.0.0.1.0.0.0.0.0.0.0.1.2.3.4.IP6.ARPA

                char[] ipChars = ip.Replace(":", "").ToCharArray();
                for (int i = ipChars.Length - 1; i > -1; i--)
                {
                    query += ipChars[i] + ".";
                }
                query += "IP6.ARPA";
            }
            // IPv4
            else
            {
                // 213.35.221.186
                // would be
                // 186.221.35.213.in-addr.arpa

                string[] ipParts = ip.Split('.');
                //--- Reverse IP ----------
                for (int i = 3; i > -1; i--)
                {
                    query += ipParts[i] + ".";
                }
                query += "in-addr.arpa";
            }

            ArrayList answers = QueryServer(query, QTYPE.PTR, 1).Answers;

            return (PTR_Record[])FilterRecords(answers, typeof(PTR_Record)).ToArray(typeof(PTR_Record));
        }

        #endregion

        #region GetMXRecords

        /// <summary>
        /// Gets MX records.(MX records are sorted by preference, lower array element is prefered)
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        [Obsolete("Use DnsEx.Query instead")]
        public MX_Record[] GetMXRecords(string domain)
        {
            MX_Record[] retVal = null;
            GetMXRecords(domain, out retVal);

            return retVal;
        }

        /// <summary>
        /// Gets MX records.(MX records are sorted by preference, lower array element is prefered)
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="mxRecords"></param>
        /// <returns></returns>
        [Obsolete("Use DnsEx.Query instead")]
        public DnsReplyCode GetMXRecords(string domain, out MX_Record[] mxRecords)
        {
            mxRecords = null;

            ArrayList answers = QueryServer(domain, QTYPE.MX, 1).Answers;
            if (answers != null)
            {
                SortedList mx = new SortedList();
                ArrayList duplicateList = new ArrayList();
                foreach (object answer in answers)
                {
                    if (answer is MX_Record)
                    {
                        MX_Record mxRec = (MX_Record)answer;

                        if (!mx.Contains(mxRec.Preference))
                        {
                            mx.Add(mxRec.Preference, mxRec);
                        }
                        else
                        {
                            duplicateList.Add(mxRec);
                        }
                    }
                }

                mxRecords = new MX_Record[mx.Count + duplicateList.Count];
                mx.Values.CopyTo(mxRecords, 0);
                duplicateList.CopyTo(mxRecords, mx.Count);
            }

            if (mxRecords == null || mxRecords.Length == 0)
            {
                return DnsReplyCode.NoEntries;
            }

            return DnsReplyCode.Ok;
        }

        #endregion

        #region GetTXTRecords

        /// <summary>
        /// Gets text records.
        /// </summary>
        /// <param name="domain">Domain name which A records to get.</param>
        /// <returns></returns>
        [Obsolete("Use DnsEx.Query instead")]
        public TXT_Record[] GetTXTRecords(string domain)
        {
            ArrayList answers = QueryServer(domain, QTYPE.TXT, 1).Answers;

            return (TXT_Record[])FilterRecords(answers, typeof(TXT_Record)).ToArray(typeof(TXT_Record));
        }

        #endregion

        #region GetAAAARecords

        /// <summary>
        /// Gets IPv6 host addess records.
        /// </summary>
        /// <param name="domain">Domain name which AAAA records to get.</param>
        /// <returns></returns>
        [Obsolete("Use DnsEx.Query instead")]
        public A_Record[] GetAAAARecords(string domain)
        {
            ArrayList answers = QueryServer(domain, QTYPE.AAAA, 1).Answers;

            return (A_Record[])FilterRecords(answers, typeof(A_Record)).ToArray(typeof(A_Record));
        }

        #endregion

        #region FilterRecords

        /// <summary>
        /// Filters out specified type of records from answer.
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private ArrayList FilterRecords(ArrayList answers, Type type)
        {
            ArrayList aRecords = new ArrayList();
            foreach (object answer in answers)
            {
                if (answer.GetType() == type)
                {
                    aRecords.Add(answer);
                }
            }

            return aRecords;
        }

        #endregion

        #region ParseARecord

        private A_Record ParseARecord(byte[] reply, ref int offset, int rdLength)
        {
            // IPv4 = byte byte byte byte

            byte[] ip = new byte[rdLength];
            Array.Copy(reply, offset, ip, 0, rdLength);

            return new A_Record(ip[0] + "." + ip[1] + "." + ip[2] + "." + ip[3]);
        }

        #endregion

        #region ParseNSRecord

        private NS_Record ParseNSRecord(byte[] reply, ref int offset, int rdLength)
        {
            // Name server name

            string name = "";
            if (GetQName(reply, ref offset, ref name))
            {
                return new NS_Record(name);
            }

            return null;
        }

        #endregion

        #region ParseCNAMERecord

        private CNAME_Record ParseCNAMERecord(byte[] reply, ref int offset, int rdLength)
        {
            // Alias

            string name = "";
            if (GetQName(reply, ref offset, ref name))
            {
                return new CNAME_Record(name);
            }

            return null;
        }

        #endregion

        #region ParsePTRRecord

        private PTR_Record ParsePTRRecord(byte[] reply, ref int offset, int rdLength)
        {
            string name = "";
            GetQName(reply, ref offset, ref name);

            return new PTR_Record(name);
        }

        #endregion

        #region ParseMxRecord

        /// <summary>
        /// Parses MX record.
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="offset"></param>
        /// <returns>Returns null, if failed.</returns>
        private MX_Record ParseMxRecord(byte[] reply, ref int offset)
        {
            /* RFC 1035	3.3.9. MX RDATA format

			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                  PREFERENCE                   |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			/                   EXCHANGE                    /
			/                                               /
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

			where:

			PREFERENCE      
				A 16 bit integer which specifies the preference given to
				this RR among others at the same owner.  Lower values
                are preferred.

			EXCHANGE 
			    A <domain-name> which specifies a host willing to act as
                a mail exchange for the owner name. 
			*/

            try
            {
                int pref = reply[offset++] << 8 | reply[offset++];

                string name = "";
                if (GetQName(reply, ref offset, ref name))
                {
                    return new MX_Record(pref, name);
                }
            }
            catch
            {
            }

            return null;
        }

        #endregion

        #region ParseTXTRecord

        private TXT_Record ParseTXTRecord(byte[] reply, ref int offset, int rdLength)
        {
            // just text

            byte[] ip = new byte[rdLength];
            Array.Copy(reply, offset, ip, 0, rdLength);

            return new TXT_Record(System.Text.Encoding.Default.GetString(ip));
        }

        #endregion

        #region ParseAAAARecord

        private A_Record ParseAAAARecord(byte[] reply, ref int offset, int rdLength)
        {
            // IPv6 = 16xbyte

            byte[] ip = new byte[rdLength];
            Array.Copy(reply, offset, ip, 0, rdLength);

            // Covert byte array to IPv6
            // ip = 8 x 2byte blocks in HEX
            string ipStr = "";
            if (rdLength == 16)
            {
                for (int i = 1; i < 16; i += 2)
                {
                    long a = (ip[i - 1] << 8) | ip[i];
                    ipStr += a.ToString("x");

                    if (i < 15)
                    {
                        ipStr += ":";
                    }
                }
            }

            return new A_Record(ipStr);
        }

        #endregion

        #region QueryServer

        /// <summary>
        /// Sends query to server.
        /// </summary>
        /// <param name="qname">Query text.</param>
        /// <param name="qtype">Query type.</param>
        /// <param name="qclass">Query class.</param>
        /// <returns></returns>
        private DnsServerResponse QueryServer(string qname, QTYPE qtype, int qclass)
        {
            if (m_DnsServers == null || m_DnsServers.Length == 0)
            {
                throw new Exception("Dns server isn't specified !!!");
            }

            // See if query is in cache
            if (m_UseDnsCache)
            {
                DnsServerResponse resopnse = DnsCache.GetFromCache(qname, (int)qtype);
                if (resopnse != null)
                {
                    return resopnse;
                }
            }

            int queryID = DnsEx.ID;
            byte[] query = CreateQuery(queryID, qname, qtype, qclass);

            int helper = 0;
            for (int i = 0; i < (m_DnsServers.Length * 2); i++)
            {
                if (helper >= m_DnsServers.Length)
                {
                    helper = 0;
                }

                try
                {
                    Socket udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    udpClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
                    udpClient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);

                    udpClient.Connect(new IPEndPoint(IPAddress.Parse(m_DnsServers[helper]), 53));

                    //send query
                    udpClient.Send(query);

                    byte[] retVal = new byte[512];
                    int countRecieved = udpClient.Receive(retVal);

                    udpClient.Close();

                    // If reply is ok, return it
                    DnsServerResponse serverResponse = ParseAnswers(retVal, queryID);

                    // Cache query
                    if (m_UseDnsCache && serverResponse.ResponseCode == RCODE.NO_ERROR)
                    {
                        DnsCache.AddToCache(qname, (int)qtype, serverResponse);
                    }

                    return serverResponse;
                }
                catch
                {
                }

                helper++;
            }

            // If we reach so far, we probably won't get connection to dsn server
            return new DnsServerResponse(false, RCODE.SERVER_FAILURE, null);
        }

        #endregion

        #region CreateQuery

        /// <summary>
        /// Creates new query.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="qname"></param>
        /// <param name="qtype"></param>
        /// <param name="qclass"></param>
        /// <returns></returns>
        private byte[] CreateQuery(int ID, string qname, QTYPE qtype, int qclass)
        {
            byte[] query = new byte[512];

            //---- Create header --------------------------------------------//
            // Header is first 12 bytes of query

            /* 4.1.1. Header section format
										  1  1  1  1  1  1
			0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                      ID                       |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                    QDCOUNT                    |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                    ANCOUNT                    |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                    NSCOUNT                    |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                    ARCOUNT                    |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			
			QR  A one bit field that specifies whether this message is a
                query (0), or a response (1).
				
			OPCODE          A four bit field that specifies kind of query in this
                message.  This value is set by the originator of a query
                and copied into the response.  The values are:

                0               a standard query (QUERY)

                1               an inverse query (IQUERY)

                2               a server status request (STATUS)
				
			*/

            //--------- Header part -----------------------------------//
            query[0] = (byte)(ID >> 8); query[1] = (byte)(ID & 0xFF);
            query[2] = (byte)1; query[3] = (byte)0;
            query[4] = (byte)0; query[5] = (byte)1;
            query[6] = (byte)0; query[7] = (byte)0;
            query[8] = (byte)0; query[9] = (byte)0;
            query[10] = (byte)0; query[11] = (byte)0;
            //---------------------------------------------------------//

            //---- End of header --------------------------------------------//


            //----Create query ------------------------------------//

            /* 	Rfc 1035 4.1.2. Question section format
											  1  1  1  1  1  1
			0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                                               |
			/                     QNAME                     /
			/                                               /
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                     QTYPE                     |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                     QCLASS                    |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			
			QNAME
				a domain name represented as a sequence of labels, where
				each label consists of a length octet followed by that
				number of octets.  The domain name terminates with the
				zero length octet for the null label of the root.  Note
				that this field may be an odd number of octets; no
				padding is used.
			*/
            string[] labels = qname.Split(new char[] { '.' });
            int position = 12;

            // Copy all domain parts(labels) to query
            // eg. lumisoft.ee = 2 labels, lumisoft and ee.
            // format = label.length + label(bytes)
            foreach (string label in labels)
            {
                // add label lenght to query
                query[position++] = (byte)(label.Length);

                // convert label string to byte array
                byte[] b = Encoding.ASCII.GetBytes(label);
                b.CopyTo(query, position);

                // Move position by label length
                position += b.Length;
            }

            // Terminate domain (see note above)
            query[position++] = (byte)0;

            // Set QTYPE 
            query[position++] = (byte)0;
            query[position++] = (byte)qtype;

            // Set QCLASS
            query[position++] = (byte)0;
            query[position++] = (byte)qclass;
            //-------------------------------------------------------//

            return query;
        }

        #endregion

        #region GetQName

        private bool GetQName(byte[] reply, ref int offset, ref string name)
        {
            try
            {
                // Do while not terminator
                while (reply[offset] != 0)
                {

                    // Check if it's pointer(In pointer first two bits always 1)
                    bool isPointer = ((reply[offset] & 0xC0) == 0xC0);

                    // If pointer
                    if (isPointer)
                    {
                        int pStart = ((reply[offset] & 0x3F) << 8) | (reply[++offset]);
                        offset++;
                        return GetQName(reply, ref pStart, ref name);
                    }
                    else
                    {
                        // label length (length = 8Bit and first 2 bits always 0)
                        // 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7
                        // empty | lablel length in bytes 
                        int labelLength = (reply[offset] & 0x3F);
                        offset++;

                        // Copy label into name 
                        name += Encoding.ASCII.GetString(reply, offset, labelLength);
                        offset += labelLength;
                    }

                    // If the next char isn't terminator,
                    // label continues - add dot between two labels
                    if (reply[offset] != 0)
                    {
                        name += ".";
                    }
                }

                // Move offset by terminator lenght
                offset++;

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region ParseAnswers

        /// <summary>
        /// Parses answer.
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="queryID"></param>
        /// <returns>Returns Dns_Answer[] collection if answer parsed successfully or throws exception if failed.</returns>
        private DnsServerResponse ParseAnswers(byte[] reply, int queryID)
        {
            //--- Parse headers ------------------------------------//

            /*
											1  1  1  1  1  1
			  0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |                      ID                       |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |                    QDCOUNT                    |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |                    ANCOUNT                    |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |                    NSCOUNT                    |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 |                    ARCOUNT                    |
			 +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			 
			QDCOUNT
				an unsigned 16 bit integer specifying the number of
				entries in the question section.

			ANCOUNT
				an unsigned 16 bit integer specifying the number of
				resource records in the answer section.
			*/

            // Get reply code
            int id = (reply[0] << 8 | reply[1]);
            OPCODE opcode = (OPCODE)((reply[2] >> 3) & 15);
            RCODE replyCode = (RCODE)(reply[3] & 15);
            int queryCount = (reply[4] << 8 | reply[5]);
            int answerCount = (reply[6] << 8 | reply[7]);
            int nsCount = (reply[8] << 8 | reply[9]);
            int arCount = (reply[10] << 8 | reply[11]);
            //---- End of headers ---------------------------------//

            // Check that it's query what we want
            if (queryID != id)
            {
                throw new Exception("This isn't query with ID what we expected");
            }

            int pos = 12;

            //----- Parse question part ------------//
            for (int q = 0; q < queryCount; q++)
            {
                string dummy = "";
                GetQName(reply, ref pos, ref dummy);
                //qtype + qclass
                pos += 4;
            }
            //--------------------------------------//


            /*
										   1  1  1  1  1  1
			 0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                                               |
			/                                               /
			/                      NAME                     /
			|                                               |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                      TYPE                     |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                     CLASS                     |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                      TTL                      |
			|                                               |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			|                   RDLENGTH                    |
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
			/                     RDATA                     /
			/                                               /
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			*/

            ArrayList answers = new ArrayList();
            //---- Start parsing aswers ------------------------------------------------------------------//
            for (int i = 0; i < answerCount; i++)
            {
                string name = "";
                if (!GetQName(reply, ref pos, ref name))
                {
                    throw new Exception("Error parisng anser");
                }

                int type = reply[pos++] << 8 | reply[pos++];
                int rdClass = reply[pos++] << 8 | reply[pos++];
                int ttl = reply[pos++] << 24 | reply[pos++] << 16 | reply[pos++] << 8 | reply[pos++];
                int rdLength = reply[pos++] << 8 | reply[pos++];

                object answerObj = null;
                switch ((QTYPE)type)
                {
                    case QTYPE.A:
                        answerObj = ParseARecord(reply, ref pos, rdLength);
                        pos += rdLength;
                        break;

                    case QTYPE.NS:
                        answerObj = ParseNSRecord(reply, ref pos, rdLength);
                        //	pos += rdLength;		
                        break;

                    case QTYPE.CNAME:
                        answerObj = ParseCNAMERecord(reply, ref pos, rdLength);
                        //	pos += rdLength;		
                        break;

                    case QTYPE.PTR:
                        answerObj = ParsePTRRecord(reply, ref pos, rdLength);
                        //	pos += rdLength;		
                        break;

                    case QTYPE.MX:
                        answerObj = ParseMxRecord(reply, ref pos);
                        break;

                    case QTYPE.TXT:
                        answerObj = ParseTXTRecord(reply, ref pos, rdLength);
                        break;

                    case QTYPE.AAAA:
                        answerObj = ParseAAAARecord(reply, ref pos, rdLength);
                        pos += rdLength;
                        break;

                    default:
                        answerObj = "dummy"; // Dummy place holder for now
                        pos += rdLength;
                        break;
                }

                // Add answer to answer list
                if (answerObj != null)
                {
                    answers.Add(answerObj);
                }
            }
            //-------------------------------------------------------------------------------------------//

            return new DnsServerResponse(true, replyCode, answers);
        }

        #endregion

        #region Properties Implementation

        /// <summary>
        /// Gets or sets dns servers.
        /// </summary>
        public static string[] DnsServers
        {
            get { return m_DnsServers; }

            set { m_DnsServers = value; }
        }

        /// <summary>
        /// Gets or sets if to use dns caching.
        /// </summary>
        public static bool UseDnsCache
        {
            get { return m_UseDnsCache; }

            set { m_UseDnsCache = value; }
        }

        /// <summary>
        /// Get next query ID.
        /// </summary>
        public static int ID
        {
            get
            {
                if (m_ID >= 65535)
                {
                    m_ID = 100;
                }
                return m_ID++;
            }
        }

        #endregion

    }
}
