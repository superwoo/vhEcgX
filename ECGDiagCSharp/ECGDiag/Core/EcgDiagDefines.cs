using System;
using System.Runtime.InteropServices;

namespace VH.ECGDiag.Core
{
    public static class EcgConstants
    {
        public const int VH_EcgMaxLeads = 18;
    }

    public enum EcgLeadIndex
    {
        I = 0, II, III, aVR, aVL, aVF, V1, V2, V3, V4, V5, V6, V3R, V4R, V5R, V7, V8, V9
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VH_EcgLeadInfo
    {
        public short Lead;
        public short Chn;  // only this value will be changed
        public uint Mask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string Name;
        public short Quality; // -2:no channel,-1:unknown,0:normal,1:VF,2:Vf,3:no signal
    }

    [StructLayout(LayoutKind.Explicit, Size = 128)]
    public struct VH_ECGparm
    {
        // Common members
        [FieldOffset(0)] public short RR;
        [FieldOffset(2)] public short HR;
        [FieldOffset(4)] public short Pd;
        [FieldOffset(6)] public short PR;
        [FieldOffset(8)] public short QRS;
        [FieldOffset(10)] public short QT;
        [FieldOffset(12)] public short QTC;
        [FieldOffset(14)] public byte WPW;  // return: A/a B/b W/w  A型, B型, W型, 小写: 可疑, 其它: 无WPW
        [FieldOffset(15)] public byte Empty;
        [FieldOffset(16)] public short QTdis;
        [FieldOffset(18)] public short QTmax;
        [FieldOffset(20)] public short QTmin;
        [FieldOffset(22)] public short QTmaxLead;
        [FieldOffset(24)] public short QTminLead;
        [FieldOffset(26)] public short AxisP;
        [FieldOffset(28)] public short AxisQRS;
        [FieldOffset(30)] public short AxisT;  // return Degree,  for ChNumber==12 only
        [FieldOffset(32)] public short UvRV5;   // return unit: uV,         for ChNumber==12 only
        [FieldOffset(34)] public short UvRV6;   // return unit: uV,         for ChNumber==12 only
        [FieldOffset(36)] public short UvSV1;   // return unit: uV,         for ChNumber==12 only
        [FieldOffset(38)] public short UvSV2;   // return unit: uV,         for ChNumber==12 only

        // OnOff array (6 elements)
        [FieldOffset(40)] public short OnOff0;
        [FieldOffset(42)] public short OnOff1;
        [FieldOffset(44)] public short OnOff2;
        [FieldOffset(46)] public short OnOff3;
        [FieldOffset(48)] public short OnOff4;
        [FieldOffset(50)] public short OnOff5;

        [FieldOffset(52)] public short UvRV1;
        [FieldOffset(54)] public short UvSV5;

        public short[] GetOnOff()
        {
            return new short[] { OnOff0, OnOff1, OnOff2, OnOff3, OnOff4, OnOff5 };
        }

        public void SetOnOff(short[] values)
        {
            if (values.Length >= 6)
            {
                OnOff0 = values[0];
                OnOff1 = values[1];
                OnOff2 = values[2];
                OnOff3 = values[3];
                OnOff4 = values[4];
                OnOff5 = values[5];
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 128)]
    public struct VH_ECGlead
    {
        // OnOff array (6 elements) - Pb,Pe,QRSb,QRSe,Tb,Te
        [FieldOffset(0)] public short OnOff0;
        [FieldOffset(2)] public short OnOff1;
        [FieldOffset(4)] public short OnOff2;
        [FieldOffset(6)] public short OnOff3;
        [FieldOffset(8)] public short OnOff4;
        [FieldOffset(10)] public short OnOff5;

        [FieldOffset(12)] public short Pstatus;   // 0: none, 1: +, 2: -; 3: +-; 4: -+
        [FieldOffset(14)] public short Tstatus;
        [FieldOffset(16)] public short Pd;
        [FieldOffset(18)] public short Qd;
        [FieldOffset(20)] public short Rd1;
        [FieldOffset(22)] public short Rd2;
        [FieldOffset(24)] public short Sd1;
        [FieldOffset(26)] public short Sd2;
        [FieldOffset(28)] public short Td;
        [FieldOffset(30)] public short PR;
        [FieldOffset(32)] public short QT;
        [FieldOffset(34)] public short QRS;
        [FieldOffset(36)] public short Pa1;
        [FieldOffset(38)] public short Pa2;
        [FieldOffset(40)] public short Qa;
        [FieldOffset(42)] public short Ra1;
        [FieldOffset(44)] public short Ra2;
        [FieldOffset(46)] public short Sa1;
        [FieldOffset(48)] public short Sa2;
        [FieldOffset(50)] public short Ta1;
        [FieldOffset(52)] public short Ta2;
        [FieldOffset(54)] public short Rnotch;  // 0,1,2,3: none,上升边，下降边，两边

        // ST array (8 elements) - STj,ST1,ST2,ST3,ST20,ST40,ST60,ST80
        [FieldOffset(56)] public short ST0;
        [FieldOffset(58)] public short ST1;
        [FieldOffset(60)] public short ST2;
        [FieldOffset(62)] public short ST3;
        [FieldOffset(64)] public short ST4;
        [FieldOffset(66)] public short ST5;
        [FieldOffset(68)] public short ST6;
        [FieldOffset(70)] public short ST7;

        // STslope array (4 elements)
        [FieldOffset(72)] public float STslope0;
        [FieldOffset(76)] public float STslope1;
        [FieldOffset(80)] public float STslope2;
        [FieldOffset(84)] public float STslope3;

        // morpho string (8 bytes)
        [FieldOffset(88)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string Morpho;

        public short[] GetOnOff()
        {
            return new short[] { OnOff0, OnOff1, OnOff2, OnOff3, OnOff4, OnOff5 };
        }

        public void SetOnOff(short[] values)
        {
            if (values.Length >= 6)
            {
                OnOff0 = values[0];
                OnOff1 = values[1];
                OnOff2 = values[2];
                OnOff3 = values[3];
                OnOff4 = values[4];
                OnOff5 = values[5];
            }
        }

        public short[] GetST()
        {
            return new short[] { ST0, ST1, ST2, ST3, ST4, ST5, ST6, ST7 };
        }

        public void SetST(short[] values)
        {
            if (values.Length >= 8)
            {
                ST0 = values[0];
                ST1 = values[1];
                ST2 = values[2];
                ST3 = values[3];
                ST4 = values[4];
                ST5 = values[5];
                ST6 = values[6];
                ST7 = values[7];
            }
        }

        public float[] GetSTslope()
        {
            return new float[] { STslope0, STslope1, STslope2, STslope3 };
        }

        public void SetSTslope(float[] values)
        {
            if (values.Length >= 4)
            {
                STslope0 = values[0];
                STslope1 = values[1];
                STslope2 = values[2];
                STslope3 = values[3];
            }
        }
    }

    public struct VH_ECGbeat
    {
        public bool Status;  // 是否叠加
        public int QRSonset;
        public int Pos;
        public short QRSw;
        public short PR;
        public short QT;    // mS
        public short Pdir;
        public short QRSdir;
        public short Tdir;
        public short Udir;  // 0: none, 1: +, -1: -, 2: +- (+>-), -2: +- (->+)
        public short Pnum;    // 0, 1, 2 (P0,P1)
        public short SubQRSw;
        public short SubQRSdir;
    }

    public class VH_ECGinfo
    {
        public bool Status;  // 模板是否有效
        public short AflutAfib;  // 0:none, 1:Aflut(房扑), 2:Afib(房颤)
        public short LeadNo;
        public short SubLeadNo;
        public short Vrate;
        public short Arate;
        public short BeatsNum;  // Number of Beats
        public VH_ECGbeat[]? Beats;
        public char PaceMaker; // 'N': none, 'A': A-Type, 'V': V-type, 'B': Both
        public short SpikesN;
        public int[]? SpikesPos;
    }

    public class VH_Template
    {
        public short ChN;
        public short SampleRate; // 通道数，采样频率
        public double Uvperbit;  // 每位微伏数
        public short Length;  // length of template data
        public short Pos;     // 叠加位置
        public short[][]? Data;  // template data (8通道)
        public short Left;
        public short Right;  // analysis range is from Left to Right
        public short SpikeA;
        public short SpikeV; // 房室起搏钉位置
    }

    public static class InitialEcgLeadInfo
    {
        public static readonly VH_EcgLeadInfo[] Data = new VH_EcgLeadInfo[]
        {
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.I,   Chn = 0,  Mask = 0x000001, Name = "I",   Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.II,  Chn = 1,  Mask = 0x000002, Name = "II",  Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.III, Chn = 2,  Mask = 0x000004, Name = "III", Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.aVR, Chn = 3,  Mask = 0x000008, Name = "aVR", Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.aVL, Chn = 4,  Mask = 0x000010, Name = "aVL", Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.aVF, Chn = 5,  Mask = 0x000020, Name = "aVF", Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V1,  Chn = 6,  Mask = 0x000040, Name = "V1",  Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V2,  Chn = 7,  Mask = 0x000080, Name = "V2",  Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V3,  Chn = 8,  Mask = 0x000100, Name = "V3",  Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V4,  Chn = 9,  Mask = 0x000200, Name = "V4",  Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V5,  Chn = 10, Mask = 0x000400, Name = "V5",  Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V6,  Chn = 11, Mask = 0x000800, Name = "V6",  Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V3R, Chn = 12, Mask = 0x001000, Name = "V3R", Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V4R, Chn = 13, Mask = 0x002000, Name = "V4R", Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V5R, Chn = 14, Mask = 0x004000, Name = "V5R", Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V7,  Chn = 15, Mask = 0x008000, Name = "V7",  Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V8,  Chn = 16, Mask = 0x010000, Name = "V8",  Quality = -1 },
            new VH_EcgLeadInfo { Lead = (short)EcgLeadIndex.V9,  Chn = 17, Mask = 0x020000, Name = "V9",  Quality = -1 }
        };

        public static readonly uint MI_L = Data[(int)EcgLeadIndex.aVL].Mask | Data[(int)EcgLeadIndex.I].Mask | Data[(int)EcgLeadIndex.aVR].Mask;
        public static readonly uint MI_I = Data[(int)EcgLeadIndex.II].Mask | Data[(int)EcgLeadIndex.aVF].Mask | Data[(int)EcgLeadIndex.III].Mask;
        public static readonly uint MI_S = Data[(int)EcgLeadIndex.V1].Mask | Data[(int)EcgLeadIndex.V2].Mask;
        public static readonly uint MI_A = Data[(int)EcgLeadIndex.V3].Mask | Data[(int)EcgLeadIndex.V3].Mask | Data[(int)EcgLeadIndex.V5].Mask | Data[(int)EcgLeadIndex.V6].Mask;
    }
}
