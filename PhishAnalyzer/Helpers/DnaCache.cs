using System;
using System.Collections;

namespace Helpers
{
    #region struct CacheEntry

    public struct CacheEntry
    {
        object m_RecordObj;
        int m_Time;

        public CacheEntry(object recordObj, int addTime)
        {
            m_RecordObj = recordObj;
            m_Time = addTime;
        }

        public object RecordObj
        {
            get { return m_RecordObj; }
        }

        public int Time
        {
            get { return m_Time; }
        }
    }

    #endregion

    /// <summary>
    /// This class implement dns query cache.
    /// </summary>
    public class DnsCache
    {
        private static Hashtable m_ChacheTbl = null;
        private static int m_HoldInCacheTime = 1000000;

        static DnsCache()
        {
            m_ChacheTbl = new Hashtable();
        }


        #region function GetFromCache

        /// <summary>
        /// Tries to get dns records from cache, if any.
        /// </summary>
        /// <param name="qname"></param>
        /// <param name="qtype"></param>
        /// <returns>Returns null if not in cache.</returns>
        public static DnsServerResponse GetFromCache(string qname, int qtype)
        {
            try
            {
                if (m_ChacheTbl.Contains(qname + qtype))
                {
                    CacheEntry entry = (CacheEntry)m_ChacheTbl[qname + qtype];

                    // If cache object isn't expired
                    if (entry.Time + m_HoldInCacheTime > Environment.TickCount)
                    {
                        //		Console.WriteLine("domain:" + qname + ":" + qtype + " from cahce.");
                        return (DnsServerResponse)entry.RecordObj;
                    }
                }
            }
            catch//(Exception x)
            {
                //		Console.WriteLine(x.Message);
            }

            return null;
        }

        #endregion

        #region function AddToCache

        /// <summary>
        /// Adds dns records to cache.
        /// </summary>
        /// <param name="qname"></param>
        /// <param name="qtype"></param>
        /// <param name="answers"></param>
        public static void AddToCache(string qname, int qtype, DnsServerResponse answers)
        {
            if (answers == null)
            {
                return;
            }

            try
            {
                lock (m_ChacheTbl)
                {
                    // Remove old cache entry, if any.
                    if (m_ChacheTbl.Contains(qname + qtype))
                    {
                        m_ChacheTbl.Remove(qname + qtype);
                    }
                    m_ChacheTbl.Add(qname + qtype, new CacheEntry(answers, Environment.TickCount));
                    //	Console.WriteLine("domain:" + qname + ":" + qtype + " added cahce.");
                }
            }
            catch//(Exception x)
            {
                //		Console.WriteLine(x.Message);
            }
        }

        #endregion


        #region Properties Implementation

        public static bool CacheInited
        {
            get { return (m_ChacheTbl != null); }
        }

        #endregion

    }
}
