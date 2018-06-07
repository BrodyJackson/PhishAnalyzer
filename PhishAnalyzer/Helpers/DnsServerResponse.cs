using System;
using System.Collections;

namespace Helpers
{
    /// <summary>
    /// This class holds dns server response.
    /// </summary>
    public class DnsServerResponse
    {
        private bool m_Success = true;
        private RCODE m_pRCODE = RCODE.NO_ERROR;
        private ArrayList m_pAnswers = null;

        public DnsServerResponse(bool connectionOk, RCODE rcode, ArrayList answers)
        {
            m_Success = connectionOk;
            m_pRCODE = rcode;
            m_pAnswers = answers;
        }

        #region method GetARecords

        /// <summary>
        /// Gets IPv4 host addess records.
        /// </summary>
        /// <returns></returns>
        public A_Record[] GetARecords()
        {
            return (A_Record[])FilterRecords(m_pAnswers, typeof(A_Record)).ToArray(typeof(A_Record));
        }

        #endregion

        #region method GetNSRecords

        /// <summary>
        /// Gets name server records.
        /// </summary>
        /// <returns></returns>
        public NS_Record[] GetNSRecords()
        {
            return (NS_Record[])FilterRecords(m_pAnswers, typeof(NS_Record)).ToArray(typeof(NS_Record));
        }

        #endregion

        #region method GetCNAMERecords

        /// <summary>
        /// Gets CNAME records.
        /// </summary>
        /// <returns></returns>
        public CNAME_Record[] GetCNAMERecords()
        {
            return (CNAME_Record[])FilterRecords(m_pAnswers, typeof(CNAME_Record)).ToArray(typeof(CNAME_Record));
        }

        #endregion

        #region method GetPTRRecords

        /// <summary>
        /// Gets PTR records.
        /// </summary>
        /// <returns></returns>
        public PTR_Record[] GetPTRRecords()
        {
            return (PTR_Record[])FilterRecords(m_pAnswers, typeof(PTR_Record)).ToArray(typeof(PTR_Record));
        }

        #endregion

        #region method GetMXRecords

        /// <summary>
        /// Gets MX records.(MX records are sorted by preference, lower array element is prefered)
        /// </summary>
        /// <returns></returns>
        public MX_Record[] GetMXRecords()
        {
            MX_Record[] mxRecords = (MX_Record[])FilterRecords(m_pAnswers, typeof(MX_Record)).ToArray(typeof(MX_Record));
            SortedList mx = new SortedList();
            ArrayList duplicateList = new ArrayList();
            foreach (MX_Record mxRecord in mxRecords)
            {
                if (!mx.Contains(mxRecord.Preference))
                {
                    mx.Add(mxRecord.Preference, mxRecord);
                }
                else
                {
                    duplicateList.Add(mxRecord);
                }
            }

            mxRecords = new MX_Record[mx.Count + duplicateList.Count];
            mx.Values.CopyTo(mxRecords, 0);
            duplicateList.CopyTo(mxRecords, mx.Count);

            return mxRecords;
        }

        #endregion

        #region method GetTXTRecords

        /// <summary>
        /// Gets text records.
        /// </summary>
        /// <returns></returns>
        public TXT_Record[] GetTXTRecords()
        {
            return (TXT_Record[])FilterRecords(m_pAnswers, typeof(TXT_Record)).ToArray(typeof(TXT_Record));
        }

        #endregion

        #region method GetAAAARecords

        /// <summary>
        /// Gets IPv6 host addess records.
        /// </summary>
        /// <returns></returns>
        public A_Record[] GetAAAARecords()
        {
            return (A_Record[])FilterRecords(m_pAnswers, typeof(A_Record)).ToArray(typeof(A_Record));
        }

        #endregion

        #region method FilterRecords

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

        #region Properties Implementation

        /// <summary>
        /// Gets if connection dns server was successfull.
        /// </summary>
        public bool ConnectionOk
        {
            get { return m_Success; }
        }

        /// <summary>
        /// Gets dns server response code.
        /// </summary>
        public RCODE ResponseCode
        {
            get { return m_pRCODE; }
        }

        /// <summary>
        /// Gets dns server returned answers. NOTE: Before using this property ensure that ConnectionOk=true and ResponseCode=RCODE.NO_ERROR.
        /// </summary>
        public ArrayList Answers
        {
            get { return m_pAnswers; }
        }

        #endregion
    }
}
