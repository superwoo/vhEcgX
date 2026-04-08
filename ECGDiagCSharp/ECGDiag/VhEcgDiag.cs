using System;
using VH.ECGDiag.Core;

namespace VH.ECGDiag
{
    public class CvhEcgDiag
    {
        public enum Vindex
        {
            V1 = 0, V2, V3, V4, V5, V6, V3R, V4R, V5R, V7, V8, V9
        }

        protected short[] m_nVindex = new short[12]; // V1-V6,V3R-V5R,V7,V9
        protected VH_ECGparm m_pEcgParm;
        protected VH_ECGlead[] m_pEcgLead;
        protected VH_ECGbeat[]? m_pEcgBeat;
        protected VH_ECGinfo m_pEcgInfo;
        protected char[] m_szLeadName = new char[64];

        // Global static references (converted to instance or singleton pattern)
        private ECGprop? g_pEcgProp = null;
        private CvhCode? g_vhCode = null;

        public CvhEcgDiag()
        {
            g_vhCode = new CvhCode();
            for (int i = 0; i < 12; i++)
                m_nVindex[i] = (short)(i + 6);

            m_pEcgParm = new VH_ECGparm();
            m_pEcgLead = new VH_ECGlead[EcgConstants.VH_EcgMaxLeads];
            m_pEcgInfo = new VH_ECGinfo();
            m_pEcgBeat = null;
        }

        ~CvhEcgDiag()
        {
            // Destructor - C# will handle garbage collection
        }

        public void SetVindex(Vindex index, short chn)
        {
            if (index < Vindex.V1 || index > Vindex.V9) return;
            if (chn < 6 || chn >= 18) chn = -1;
            m_nVindex[(int)index] = chn;
        }

        public bool CreateEcgDiag(short chNumber, short[][] dataIn, short seconds, short samplerate, double uVperbit)
        {
            if (g_pEcgProp != null)
            {
                g_pEcgProp = null; // Allow GC to clean up
            }

            g_pEcgProp = new ECGprop(chNumber, dataIn, seconds, samplerate, uVperbit);
            short[][]? data = g_pEcgProp.m_pDataIn;
            int length = seconds * samplerate;

            g_vhCode?.SetEcgDataInfo(samplerate, chNumber, uVperbit);
            g_vhCode?.SetEcgData(data, length);

            if (g_pEcgProp != null && g_pEcgProp.AutoProcess())
            {
                short templpos = g_pEcgProp.Temp.Pos;
                short templen = g_pEcgProp.Temp.Length;
                short[][]? templ = g_pEcgProp.Temp.Data;

                g_vhCode?.SetEcgTempl(templpos, templen, templ);
                SetParameters();

                return true;
            }
            else
            {
                g_pEcgProp = null;
                return false;
            }
        }

        protected void SetParameters()
        {
            if (g_pEcgProp == null) return;

            short templchn = g_pEcgProp.GetTemplChN();
            for (short i = 0; i < templchn; i++)
            {
                var lead = g_pEcgProp.Lead[i];
                m_pEcgLead[i].SetOnOff(lead.GetOnOff());
                m_pEcgLead[i].Pstatus = lead.Pstatus;
                m_pEcgLead[i].Tstatus = lead.Tstatus;
                m_pEcgLead[i].Pd = lead.Pd;
                m_pEcgLead[i].Qd = lead.Qd;
                m_pEcgLead[i].Rd1 = lead.Rd1;
                m_pEcgLead[i].Rd2 = lead.Rd2;
                m_pEcgLead[i].Sd1 = lead.Sd1;
                m_pEcgLead[i].Sd2 = lead.Sd2;
                m_pEcgLead[i].Td = lead.Td;
                m_pEcgLead[i].PR = lead.PR;
                m_pEcgLead[i].QT = lead.QT;
                m_pEcgLead[i].QRS = lead.QRS;
                m_pEcgLead[i].Pa1 = lead.Pa1;
                m_pEcgLead[i].Pa2 = lead.Pa2;
                m_pEcgLead[i].Qa = lead.Qa;
                m_pEcgLead[i].Ra1 = lead.Ra1;
                m_pEcgLead[i].Ra2 = lead.Ra2;
                m_pEcgLead[i].Sa1 = lead.Sa1;
                m_pEcgLead[i].Sa2 = lead.Sa2;
                m_pEcgLead[i].Ta1 = lead.Ta1;
                m_pEcgLead[i].Ta2 = lead.Ta2;
                m_pEcgLead[i].Rnotch = lead.Rnotch;
                m_pEcgLead[i].SetST(lead.GetST());
                m_pEcgLead[i].SetSTslope(lead.GetSTslope());
                m_pEcgLead[i].Morpho = lead.Morpho;
            }

            g_vhCode?.SetEcglead(m_pEcgLead);

            m_pEcgParm.RR = g_pEcgProp.RR();
            m_pEcgParm.HR = g_pEcgProp.HR();
            m_pEcgParm.Pd = g_pEcgProp.Pd_ms();
            m_pEcgParm.PR = g_pEcgProp.PR();
            m_pEcgParm.QRS = g_pEcgProp.QRS();
            m_pEcgParm.QT = g_pEcgProp.QT();
            m_pEcgParm.QTC = g_pEcgProp.QTc();
            m_pEcgParm.WPW = (byte)g_pEcgProp.WPW();
            m_pEcgParm.Empty = (byte)' ';
            m_pEcgParm.QTdis = g_pEcgProp.QTdis();
            m_pEcgParm.QTmax = g_pEcgProp.QTmax();
            m_pEcgParm.QTmin = g_pEcgProp.QTmin();
            m_pEcgParm.QTmaxLead = g_pEcgProp.QTmaxLead();
            m_pEcgParm.QTminLead = g_pEcgProp.QTminLead();
            m_pEcgParm.AxisP = g_pEcgProp.Paxis();
            m_pEcgParm.AxisQRS = g_pEcgProp.QRSaxis();
            m_pEcgParm.AxisT = g_pEcgProp.Taxis();
            m_pEcgParm.UvRV1 = RV1();
            m_pEcgParm.UvRV5 = RV5();
            m_pEcgParm.UvRV6 = RV6();
            m_pEcgParm.UvSV1 = SV1();
            m_pEcgParm.UvSV2 = SV2();
            m_pEcgParm.UvSV5 = SV5();

            g_vhCode?.SetEcgParm(ref m_pEcgParm);
            m_pEcgParm.SetOnOff(g_pEcgProp.Parm.GetOnOff());

            // Handle beat information
            if (g_pEcgProp.m_pOutPut != null)
            {
                int beatsNum = g_pEcgProp.m_pOutPut.BeatsNum;
                if (beatsNum > 1)
                {
                    m_pEcgBeat = new VH_ECGbeat[beatsNum];
                    m_pEcgInfo.Status = g_pEcgProp.m_pOutPut.Status == (short)PROC_STATUS.PROC_OK;
                    m_pEcgInfo.AflutAfib = g_pEcgProp.m_pOutPut.AflutAfib;
                    m_pEcgInfo.LeadNo = g_pEcgProp.m_pOutPut.LeadNo;
                    m_pEcgInfo.SubLeadNo = g_pEcgProp.m_pOutPut.SubLeadNo;
                    m_pEcgInfo.Vrate = g_pEcgProp.m_pOutPut.Vrate;
                    m_pEcgInfo.Arate = g_pEcgProp.m_pOutPut.Arate;
                    m_pEcgInfo.BeatsNum = (short)beatsNum;

                    for (short i = 0; i < beatsNum; i++)
                    {
                        var beat = g_pEcgProp.m_pOutPut.Beats[i];
                        m_pEcgBeat[i].Status = beat.Status == BEAT_STATUS_TYPE.OK;
                        m_pEcgBeat[i].QRSonset = beat.QRSonset;
                        m_pEcgBeat[i].Pos = beat.Pos;
                        m_pEcgBeat[i].QRSw = beat.QRSw;
                        m_pEcgBeat[i].PR = beat.PR;
                        m_pEcgBeat[i].QT = beat.QT;
                        m_pEcgBeat[i].Pdir = beat.Pdir;
                        m_pEcgBeat[i].QRSdir = beat.QRSdir;
                        m_pEcgBeat[i].Tdir = beat.Tdir;
                        m_pEcgBeat[i].Udir = beat.Udir;
                        m_pEcgBeat[i].Pnum = beat.Pnum;
                        m_pEcgBeat[i].SubQRSw = beat.SubQRSw;
                        m_pEcgBeat[i].SubQRSdir = beat.SubQRSdir;
                    }

                    m_pEcgInfo.Beats = m_pEcgBeat;
                    m_pEcgInfo.PaceMaker = g_pEcgProp.m_pOutPut.PaceMaker;
                    m_pEcgInfo.SpikesN = g_pEcgProp.m_pOutPut.SpikesN;
                    m_pEcgInfo.SpikesPos = g_pEcgProp.m_pOutPut.SpikesPos;

                    char[]? pBeatsType = g_pEcgProp.Beats;
                    g_vhCode?.SetEcgInfo(m_pEcgInfo, pBeatsType);
                }
            }
        }

        public bool EcgCode(char bySex, short age, short ageYmd = 0)
        {
            g_vhCode?.SetPatientInfo(bySex, age, ageYmd);
            g_vhCode?.Code();
            return true;
        }

        public void SetPrematurePpercent(short percentPAC, short percentPVC)
        {
            g_pEcgProp?.SetPrematurePpercent(percentPAC, percentPVC);
        }

        // Diagnostic code retrieval methods
        public short GetFirstMcode(string? szLeadName = null)
        {
            return g_vhCode?.McCodeGetFirst(szLeadName) ?? 0;
        }

        public short GetNextMcode(string? szLeadName = null)
        {
            return g_vhCode?.McCodeGetNext(szLeadName) ?? 0;
        }

        public short GetFirstRcode(string? szLeadName = null)
        {
            return g_vhCode?.VhCodeGetFirst(szLeadName) ?? 0;
        }

        public short GetNextRcode(string? szLeadName = null)
        {
            return g_vhCode?.VhCodeGetNext(szLeadName) ?? 0;
        }

        public short GetMcCodeCount()
        {
            return g_vhCode?.McCodeCount() ?? 0;
        }

        public short GetVhCodeCount()
        {
            return g_vhCode?.VhCodeCount() ?? 0;
        }

        public static string? McCode(short code)
        {
            return CvhCode.McCodeString((ushort)code);
        }

        public static string? VhCode(short code)
        {
            return CvhCode.VhCodeString((ushort)code);
        }

        public short GetCriticalValue()
        {
            return g_vhCode?.GetCriticalValue() ?? 0;
        }

        // Template data access
        public short GetTemplChNumber()
        {
            return (g_pEcgProp != null) ? g_pEcgProp.GetTemplChN() : (short)0;
        }

        public VH_ECGinfo GetEcgInfo() => m_pEcgInfo;
        public VH_ECGbeat[]? GetEcgBeats() => m_pEcgBeat;
        public short GetTemplLength() => (g_pEcgProp != null) ? g_pEcgProp.Temp.Length : (short)0;
        public short[][]? GetTemplData() => (g_pEcgProp != null) ? g_pEcgProp.Temp.Data : null;
        public short GetTemplPos() => (g_pEcgProp != null) ? g_pEcgProp.Temp.Pos : (short)0;

        public int GetBeatNum() => (g_pEcgProp != null) ? g_pEcgProp.m_nBeats : 0;

        public char[]? GetBeats(out int beatNum)
        {
            beatNum = g_pEcgProp?.m_nBeats ?? 0;
            return g_pEcgProp?.Beats;
        }

        public byte[]? GetBeatAdd(out int beatNum)
        {
            beatNum = g_pEcgProp?.m_nBeats ?? 0;
            return g_pEcgProp?.BeatAdd;
        }

        public VH_ECGparm GetECGparm() => m_pEcgParm;
        public VH_ECGlead[] GetECGlead() => m_pEcgLead;

        // Wave amplitude methods (uV)
        public short uvPa1(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvPa1(lead) : (short)0;
        public short uvPa2(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvPa2(lead) : (short)0;
        public short uvQa(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvQa(lead) : (short)0;
        public short uvRa1(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvRa1(lead) : (short)0;
        public short uvRa2(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvRa2(lead) : (short)0;
        public short uvSa1(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvSa1(lead) : (short)0;
        public short uvSa2(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvSa2(lead) : (short)0;
        public short uvTa1(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvTa1(lead) : (short)0;
        public short uvTa2(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvTa2(lead) : (short)0;

        public short uvQRSa(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvQRSa(lead) : (short)0;
        public short uvRa(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvRa(lead) : (short)0;
        public short uvSa(short lead) => (g_pEcgProp != null) ? g_pEcgProp.uvSa(lead) : (short)0;

        // Duration methods (ms)
        public short msPd(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msPd(lead) : (short)0;
        public short msQd(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msQd(lead) : (short)0;
        public short msRd1(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msRd1(lead) : (short)0;
        public short msRd2(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msRd2(lead) : (short)0;
        public short msSd1(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msSd1(lead) : (short)0;
        public short msSd2(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msSd2(lead) : (short)0;
        public short msTd(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msTd(lead) : (short)0;

        public short msPR(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msPR(lead) : (short)0;
        public short msQT(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msQT(lead) : (short)0;
        public short msQRS(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msQRS(lead) : (short)0;

        public short msSd(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msSd(lead) : (short)0;
        public short msRd(short lead) => (g_pEcgProp != null) ? g_pEcgProp.msRd(lead) : (short)0;

        // ST segment methods (uV)
        public short STj(short lead) => (g_pEcgProp != null) ? g_pEcgProp.STj(lead) : (short)0;
        public short ST1(short lead) => (g_pEcgProp != null) ? g_pEcgProp.ST1(lead) : (short)0;
        public short ST2(short lead) => (g_pEcgProp != null) ? g_pEcgProp.ST2(lead) : (short)0;
        public short ST3(short lead) => (g_pEcgProp != null) ? g_pEcgProp.ST3(lead) : (short)0;
        public short ST20(short lead) => (g_pEcgProp != null) ? g_pEcgProp.ST20(lead) : (short)0;
        public short ST40(short lead) => (g_pEcgProp != null) ? g_pEcgProp.ST40(lead) : (short)0;
        public short ST60(short lead) => (g_pEcgProp != null) ? g_pEcgProp.ST60(lead) : (short)0;
        public short ST80(short lead) => (g_pEcgProp != null) ? g_pEcgProp.ST80(lead) : (short)0;
        public short Rnotch(short lead) => (g_pEcgProp != null) ? g_pEcgProp.Rnotch(lead) : (short)0;

        // Common parameters
        public short HR() => (g_pEcgProp != null) ? g_pEcgProp.HR() : (short)0;
        public short RR() => (g_pEcgProp != null) ? g_pEcgProp.RR() : (short)0;
        public short Pd() => (g_pEcgProp != null) ? g_pEcgProp.Pd_ms() : (short)0;
        public short PR() => (g_pEcgProp != null) ? g_pEcgProp.PR() : (short)0;
        public short QRS() => (g_pEcgProp != null) ? g_pEcgProp.QRS() : (short)0;
        public short QT() => (g_pEcgProp != null) ? g_pEcgProp.QT() : (short)0;
        public short QTc() => (g_pEcgProp != null) ? g_pEcgProp.QTc() : (short)0;
        public short QTdis() => (g_pEcgProp != null) ? g_pEcgProp.QTdis() : (short)0;
        public short QTmax() => (g_pEcgProp != null) ? g_pEcgProp.QTmax() : (short)0;
        public short QTmin() => (g_pEcgProp != null) ? g_pEcgProp.QTmin() : (short)0;
        public short QTmaxLead() => (g_pEcgProp != null) ? g_pEcgProp.QTmaxLead() : (short)0;
        public short QTminLead() => (g_pEcgProp != null) ? g_pEcgProp.QTminLead() : (short)0;

        // V lead amplitudes
        public short RV5()
        {
            if (g_pEcgProp == null || m_nVindex[(int)Vindex.V5] < 0 || m_nVindex[(int)Vindex.V5] >= g_pEcgProp.GetTemplChN())
                return 0;
            var lead = g_pEcgProp.Lead;
            return Math.Max(lead[m_nVindex[(int)Vindex.V5]].Ra1, lead[m_nVindex[(int)Vindex.V5]].Ra2);
        }

        public short RV6()
        {
            if (g_pEcgProp == null || m_nVindex[(int)Vindex.V6] < 0 || m_nVindex[(int)Vindex.V6] >= g_pEcgProp.GetTemplChN())
                return 0;
            var lead = g_pEcgProp.Lead;
            return Math.Max(lead[m_nVindex[(int)Vindex.V6]].Ra1, lead[m_nVindex[(int)Vindex.V6]].Ra2);
        }

        public short SV1()
        {
            short uvSV = 0;
            short L = m_nVindex[(int)Vindex.V1];
            if (g_pEcgProp == null || L < 0 || L >= g_pEcgProp.GetTemplChN())
                return 0;
            var lead = g_pEcgProp.Lead;
            uvSV = Math.Min(lead[L].Sa1, lead[L].Sa2);
            if (isQS(L))
                uvSV = Math.Min(uvSV, lead[L].Qa);
            return uvSV;
        }

        public short SV2()
        {
            short uvSV = 0;
            short L = m_nVindex[(int)Vindex.V2];
            if (g_pEcgProp == null || L < 0 || L >= g_pEcgProp.GetTemplChN())
                return 0;
            var lead = g_pEcgProp.Lead;
            uvSV = Math.Min(lead[L].Sa1, lead[L].Sa2);
            if (isQS(L))
                uvSV = Math.Min(uvSV, lead[L].Qa);
            return uvSV;
        }

        public short SV5()
        {
            short uvSV = 0;
            short L = m_nVindex[(int)Vindex.V5];
            if (g_pEcgProp == null || L < 0 || L >= g_pEcgProp.GetTemplChN())
                return 0;
            var lead = g_pEcgProp.Lead;
            uvSV = Math.Min(lead[L].Sa1, lead[L].Sa2);
            if (isQS(L))
                uvSV = Math.Min(uvSV, lead[L].Qa);
            return uvSV;
        }

        public short SV6()
        {
            short uvSV = 0;
            short L = m_nVindex[(int)Vindex.V6];
            if (g_pEcgProp == null || L < 0 || L >= g_pEcgProp.GetTemplChN())
                return 0;
            var lead = g_pEcgProp.Lead;
            uvSV = Math.Min(lead[L].Sa1, lead[L].Sa2);
            if (isQS(L))
                uvSV = Math.Min(uvSV, lead[L].Qa);
            return uvSV;
        }

        public short RV1()
        {
            if (g_pEcgProp == null || m_nVindex[(int)Vindex.V1] < 0 || m_nVindex[(int)Vindex.V1] >= g_pEcgProp.GetTemplChN())
                return 0;
            var lead = g_pEcgProp.Lead;
            return Math.Max(lead[m_nVindex[(int)Vindex.V1]].Ra1, lead[m_nVindex[(int)Vindex.V1]].Ra2);
        }

        public short RV2()
        {
            if (g_pEcgProp == null || m_nVindex[(int)Vindex.V2] < 0 || m_nVindex[(int)Vindex.V2] >= g_pEcgProp.GetTemplChN())
                return 0;
            var lead = g_pEcgProp.Lead;
            return Math.Max(lead[m_nVindex[(int)Vindex.V2]].Ra1, lead[m_nVindex[(int)Vindex.V2]].Ra2);
        }

        // Axis methods (degrees)
        public short Paxis() => (g_pEcgProp != null) ? g_pEcgProp.Paxis() : (short)0;
        public short QRSaxis() => (g_pEcgProp != null) ? g_pEcgProp.QRSaxis() : (short)0;
        public short Taxis() => (g_pEcgProp != null) ? g_pEcgProp.Taxis() : (short)0;
        public char WPW() => (g_pEcgProp != null) ? g_pEcgProp.WPW() : ' ';

        // Beat parameters
        public short RR(int index) => (g_pEcgProp != null) ? g_pEcgProp.RR(index) : (short)0;
        public short beatPnum(int index) => (g_pEcgProp != null) ? g_pEcgProp.beatPnum(index) : (short)0;

        // Derived parameters
        public short positivePa(short lead) => (g_pEcgProp != null) ? g_pEcgProp.positivePa(lead) : (short)0;
        public short negativePa(short lead) => (g_pEcgProp != null) ? g_pEcgProp.negativePa(lead) : (short)0;
        public short positiveTa(short lead) => (g_pEcgProp != null) ? g_pEcgProp.positiveTa(lead) : (short)0;
        public short negativeTa(short lead) => (g_pEcgProp != null) ? g_pEcgProp.negativeTa(lead) : (short)0;
        public short positivePd(short lead) => (g_pEcgProp != null) ? g_pEcgProp.positivePd(lead) : (short)0;
        public short negativePd(short lead) => (g_pEcgProp != null) ? g_pEcgProp.negativePd(lead) : (short)0;
        public short positiveTd(short lead) => (g_pEcgProp != null) ? g_pEcgProp.positiveTd(lead) : (short)0;
        public short negativeTd(short lead) => (g_pEcgProp != null) ? g_pEcgProp.negativeTd(lead) : (short)0;

        // Wave type checking methods
        public bool isPositiveQRS(short lead) => (g_pEcgProp != null) && g_pEcgProp.isPositiveQRS(lead);
        public bool isNegativeQRS(short lead) => (g_pEcgProp != null) && g_pEcgProp.isNegativeQRS(lead);
        public bool isPositiveP(short lead) => (g_pEcgProp != null) && g_pEcgProp.isPositiveP(lead);
        public bool isNegativeP(short lead) => (g_pEcgProp != null) && g_pEcgProp.isNegativeP(lead);
        public bool isDualP(short lead) => (g_pEcgProp != null) && g_pEcgProp.isDualP(lead);
        public bool isPositiveT(short lead) => (g_pEcgProp != null) && g_pEcgProp.isPositiveT(lead);
        public bool isNegativeT(short lead) => (g_pEcgProp != null) && g_pEcgProp.isNegativeT(lead);
        public bool isDualT(short lead) => (g_pEcgProp != null) && g_pEcgProp.isDualT(lead);
        public bool isFlatT(short lead) => (g_pEcgProp != null) && g_pEcgProp.isFlatT(lead);
        public bool isQS(short lead) => (g_pEcgProp != null) && g_pEcgProp.isQS(lead);
        public bool isQr(short lead) => (g_pEcgProp != null) && g_pEcgProp.isQr(lead);
        public bool isrsR(short lead) => (g_pEcgProp != null) && g_pEcgProp.isrsR(lead);
        public bool isrsr(short lead) => (g_pEcgProp != null) && g_pEcgProp.isrsr(lead);
        public bool isRSrs(short lead) => (g_pEcgProp != null) && g_pEcgProp.isRSrs(lead);

        // ST segment analysis
        public int uvSTvalue(short lead, int MSor123) => (g_pEcgProp != null) ? g_pEcgProp.uvSTvalue(lead, MSor123) : 0;
        public float STslope(short lead, int msStep) => (g_pEcgProp != null) ? g_pEcgProp.STslope(lead, msStep) : 0;

        // Time conversion methods
        public int Samples2ms(int samples) => (g_pEcgProp != null) ? samples * 1000 / g_pEcgProp.GetSampleRate() : 0;
        public int ms2Samples(int ms) => (g_pEcgProp != null) ? ms * g_pEcgProp.GetSampleRate() / 1000 : 0;

        // Individual parameters
        public string? QRSmorpho(short lead) => (g_pEcgProp != null) ? g_pEcgProp.QRSmorpho(lead) : null;
        public char GetPaceMaker() => (g_pEcgProp != null) ? g_pEcgProp.GetPaceMaker() : ' ';
        public short GetAflutAfib() => (g_pEcgProp != null) ? g_pEcgProp.GetAflutAfib() : (short)0;

        // Status methods
        public bool IsPacedECG() => (g_pEcgProp != null) && g_pEcgProp.IsPacedECG();
        public bool TemplIsOk() => (g_pEcgProp != null) && g_pEcgProp.TemplIsOk();

        // Static utility methods
        public static short HR(short msRR) => ECGprop.HR(msRR);
        public static short QTc(short msQT, short msRR) => ECGprop.QTc(msQT, msRR);
    }

    // Placeholder enums (to be properly converted from ECGbeats.h)
    public enum PROC_STATUS
    {
        MUSH_NOISE = -3,
        MUCH_QRS = -2,
        NO_TEMPL = -1,
        FEW_QRS = 0,
        PROC_OK = 1
    }

    public enum BEAT_STATUS_TYPE
    {
        OK, BORDER, RR_Int, QRS_W, SubQRS_W, QRS_Dir, SubQRS_Dir, T_Dir, P_Dir, fOK, PR_Int, QRS_Range
    }

    // Placeholder classes (to be fully implemented)
    public class ECGprop
    {
        public short[][] m_pDataIn;
        public VH_Template Temp;
        public ECGlead[] Lead;
        public ECGparm Parm;
        public ECG_Parameters? m_pOutPut;
        public int m_nBeats;
        public char[]? Beats;
        public byte[]? BeatAdd;

        public ECGprop(short chNumber, short[][] dataIn, short seconds, short samplerate, double uVperbit)
        {
            m_pDataIn = dataIn;
            Temp = new VH_Template();
            Lead = new ECGlead[chNumber];
            Parm = new ECGparm();
            // ... initialization code
        }

        public bool AutoProcess() => false; // To be implemented
        public short GetTemplChN() => 0; // To be implemented
        public void SetPrematurePpercent(short percentPAC, short percentPVC) { }
        public short GetSampleRate() => 0; // To be implemented

        // Common parameter methods
        public short RR() => 0;
        public short HR() => 0;
        public short Pd_ms() => 0;
        public short PR() => 0;
        public short QRS() => 0;
        public short QT() => 0;
        public short QTc() => 0;
        public short QTdis() => 0;
        public short QTmax() => 0;
        public short QTmin() => 0;
        public short QTmaxLead() => 0;
        public short QTminLead() => 0;
        public short Paxis() => 0;
        public short QRSaxis() => 0;
        public short Taxis() => 0;
        public char WPW() => ' ';

        // Lead methods (to be implemented)
        public short uvPa1(short lead) => 0;
        public short uvPa2(short lead) => 0;
        public short uvQa(short lead) => 0;
        public short uvRa1(short lead) => 0;
        public short uvRa2(short lead) => 0;
        public short uvSa1(short lead) => 0;
        public short uvSa2(short lead) => 0;
        public short uvTa1(short lead) => 0;
        public short uvTa2(short lead) => 0;
        public short uvQRSa(short lead) => 0;
        public short uvRa(short lead) => 0;
        public short uvSa(short lead) => 0;
        public short msPd(short lead) => 0;
        public short msQd(short lead) => 0;
        public short msRd1(short lead) => 0;
        public short msRd2(short lead) => 0;
        public short msSd1(short lead) => 0;
        public short msSd2(short lead) => 0;
        public short msTd(short lead) => 0;
        public short msPR(short lead) => 0;
        public short msQT(short lead) => 0;
        public short msQRS(short lead) => 0;
        public short msSd(short lead) => 0;
        public short msRd(short lead) => 0;
        public short STj(short lead) => 0;
        public short ST1(short lead) => 0;
        public short ST2(short lead) => 0;
        public short ST3(short lead) => 0;
        public short ST20(short lead) => 0;
        public short ST40(short lead) => 0;
        public short ST60(short lead) => 0;
        public short ST80(short lead) => 0;
        public short Rnotch(short lead) => 0;
        public short positivePa(short lead) => 0;
        public short negativePa(short lead) => 0;
        public short positiveTa(short lead) => 0;
        public short negativeTa(short lead) => 0;
        public short positivePd(short lead) => 0;
        public short negativePd(short lead) => 0;
        public short positiveTd(short lead) => 0;
        public short negativeTd(short lead) => 0;

        // Wave type checking
        public bool isPositiveQRS(short lead) => false;
        public bool isNegativeQRS(short lead) => false;
        public bool isPositiveP(short lead) => false;
        public bool isNegativeP(short lead) => false;
        public bool isDualP(short lead) => false;
        public bool isPositiveT(short lead) => false;
        public bool isNegativeT(short lead) => false;
        public bool isDualT(short lead) => false;
        public bool isFlatT(short lead) => false;
        public bool isQS(short lead) => false;
        public bool isQr(short lead) => false;
        public bool isrsR(short lead) => false;
        public bool isrsr(short lead) => false;
        public bool isRSrs(short lead) => false;

        // Other methods
        public int uvSTvalue(short lead, int MSor123) => 0;
        public float STslope(short lead, int msStep) => 0;
        public string? QRSmorpho(short lead) => null;
        public char GetPaceMaker() => ' ';
        public short GetAflutAfib() => 0;
        public bool IsPacedECG() => false;
        public bool TemplIsOk() => false;
        public short RR(int index) => 0;
        public short beatPnum(int index) => 0;

        public static short HR(short msRR) => (short)(60 * 1000 / msRR);
        public static short QTc(short msQT, short msRR) => (msRR > 0) ? (short)(msQT * Math.Sqrt(1000.0) / Math.Sqrt(msRR)) : (short)0;
    }

    // Placeholder structures
    public struct ECGlead
    {
        public short Pstatus, Tstatus, Pd, Qd, Rd1, Rd2, Sd1, Sd2, Td, PR, QT, QRS;
        public short Pa1, Pa2, Qa, Ra1, Ra2, Sa1, Sa2, Ta1, Ta2, Rnotch;
        public short[] ST;
        public float[] STslope;
        public string Morpho;

        public short[] GetOnOff() => new short[6];
        public void SetOnOff(short[] values) { }
        public short[] GetST() => ST ?? new short[8];
        public void SetST(short[] values) => ST = values;
        public float[] GetSTslope() => STslope ?? new float[4];
        public void SetSTslope(float[] values) => STslope = values;
    }

    public struct ECGparm
    {
        public short RR, HR, Pd, PR, QRS, QT, QTC;
        public char WPW;
        public short QTdis, QTmax, QTmin, QTmaxLead, QTminLead;
        public short AxisP, AxisQRS, AxisT;
        public short UvRV5, UvRV6, UvSV1, UvSV2, UvRV1, UvSV5;

        public short[] GetOnOff() => new short[6];
        public void SetOnOff(short[] values) { }
    }

    public struct BeatParameters
    {
        public BEAT_STATUS_TYPE Status;
        public int QRSonset, Pos;
        public short QRSw, PR, QT;
        public short Pdir, QRSdir, Tdir, Udir, Pnum;
        public short SubQRSw, SubQRSdir;
    }

    public class ECG_Parameters
    {
        public short Status, AflutAfib, LeadNo, SubLeadNo, Vrate, Arate, BeatsNum;
        public BeatParameters[]? Beats;
        public char PaceMaker;
        public short SpikesN;
        public int[]? SpikesPos;
    }

    // Placeholder class for CvhCode (to be fully implemented)
    public class CvhCode
    {
        public void SetEcgDataInfo(short samplerate, short chNumber, double uVperbit) { }
        public void SetEcgData(short[][]? data, int length) { }
        public void SetEcgTempl(short templpos, short templen, short[][]? templ) { }
        public void SetEcglead(VH_ECGlead[] lead) { }
        public void SetEcgParm(ref VH_ECGparm parm) { }
        public void SetEcgInfo(VH_ECGinfo info, char[]? beatsType) { }
        public void SetPatientInfo(char sex, short age, short ageYmd) { }
        public void Code() { }
        public short McCodeGetFirst(string? szLeadName) => 0;
        public short McCodeGetNext(string? szLeadName) => 0;
        public short VhCodeGetFirst(string? szLeadName) => 0;
        public short VhCodeGetNext(string? szLeadName) => 0;
        public short McCodeCount() => 0;
        public short VhCodeCount() => 0;
        public short GetCriticalValue() => 0;

        public static string? McCodeString(ushort code) => null;
        public static string? VhCodeString(ushort code) => null;
    }
}
