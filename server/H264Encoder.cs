#define HAMED_LOG_FPS_1
//#define HAMED_LOG_FPS_2
#define HAMED_LOG_BITRATE

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using DirectShow;
using DirectShow.BaseClasses;
using Sonic;
using System.IO;
using System.Threading;
using System.Reflection;

namespace ExampleFilters
{
    #region H264 Encoder 

    public enum rate_control : int
	{
		VBR = 1,
		CBR = 2,
	}

	public enum mb_encoding : int
	{
		CAVLC = 0,
		CABAC = 1,
	}

	public enum profile_idc : int
	{
		PROFILE_AUTO	 = 0,
		PROFILE_BASELINE = 66,
		PROFILE_MAIN     = 77,
		PROFILE_HIGH     = 100,
	}

	public enum level_idc : int
	{
		LEVEL_AUTO		= 0,
		LEVEL_1			= 10,
		LEVEL_1_1		= 11,
		LEVEL_1_2		= 12,
		LEVEL_1_3		= 13,
		LEVEL_2			= 20,
		LEVEL_2_1		= 21,
		LEVEL_2_2		= 22,
		LEVEL_3			= 30,
		LEVEL_3_1		= 31,
		LEVEL_3_2		= 32,
		LEVEL_4			= 40,
		LEVEL_4_1		= 41,
		LEVEL_4_2		= 42,
		LEVEL_5			= 50,
		LEVEL_5_1		= 51
	}

    [ComVisible(true)]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("089C95D9-7DAE-4916-BC52-54B45D3925D2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IH264Encoder
    {
        [PreserveSig]
        int get_Bitrate([Out] out int plValue);

		[PreserveSig]
        int put_Bitrate([In] int lValue);

		[PreserveSig]
        int get_RateControl([Out] out rate_control pValue);

		[PreserveSig]
        int put_RateControl([In] rate_control value);

		[PreserveSig]
        int get_MbEncoding([Out] out mb_encoding pValue);

		[PreserveSig]
        int put_MbEncoding([In] mb_encoding value);

		[PreserveSig]
        int get_Deblocking([Out,MarshalAs(UnmanagedType.Bool)] out bool pValue);

		[PreserveSig]
        int put_Deblocking([In,MarshalAs(UnmanagedType.Bool)] bool value);

		[PreserveSig]
        int get_GOP([Out,MarshalAs(UnmanagedType.Bool)] out bool pValue);

		[PreserveSig]
        int put_GOP([In,MarshalAs(UnmanagedType.Bool)] bool value);

		[PreserveSig]
        int get_AutoBitrate([Out,MarshalAs(UnmanagedType.Bool)] out bool pValue);

		[PreserveSig]
        int put_AutoBitrate([In,MarshalAs(UnmanagedType.Bool)] bool value);

		[PreserveSig]
        int get_Profile([Out] out profile_idc pValue);

		[PreserveSig]
        int put_Profile([In] profile_idc value);

		[PreserveSig]
        int get_Level([Out] out level_idc pValue);

		[PreserveSig]
        int put_Level([In] level_idc value);

		[PreserveSig]
        int get_SliceIntervals([Out] out int piIDR,[Out] out int piP);

		[PreserveSig]
        int put_SliceIntervals([In] ref int piIDR,[In] ref int piP);
    }

    [ComVisible(true)]
    [Guid("3FD83588-D403-40a2-9739-5F75E1590AB8")]
    [AMovieSetup(true)]
    [PropPageSetup(typeof(H264PropertiesForm),typeof(AboutForm))]
    public class H264EncoderFilter : TransformFilter, IH264Encoder
    {
        #region Cuda

        protected class CUDA
        {
            public const uint NV_CODEC_TYPE_H264 = 4;

            public const uint NVVE_PROFILE_BASELINE	= 0xff42;
	        public const uint NVVE_PROFILE_MAIN		= 0xff4d;
            public const uint NVVE_PROFILE_HIGH     = 0xff64;

            public const uint NVVE_PIC_TYPE_IFRAME  = 1;
            public const uint NVVE_PIC_TYPE_PFRAME  = 2;
            public const uint NVVE_PIC_TYPE_BFRAME = 3;

            public enum NVVE_SurfaceFormat
            {
                UYVY = 0,
                YUY2,
                YV12,
                NV12,
                IYUV
            };

            public enum NVVE_FIELD_MODE
            {
                MODE_FRAME = 0,
                MODE_FIELD_TOP_FIRST,
                MODE_FIELD_BOTTOM_FIRST,
                MODE_FIELD_PICAFF,
            };
            public enum NVVE_RateCtrlType
            {
                RC_CQP = 0,
                RC_VBR,
                RC_CBR,
                RC_VBR_MINQP
            };

            public enum NVVE_ASPECT_RATIO_TYPE
            {
                ASPECT_RATIO_DAR = 0,
                ASPECT_RATIO_SAR = 1
            }

            public enum NVVE_PicStruct
            {
                TOP_FIELD = 0x01,
                BOTTOM_FIELD = 0x02,
                FRAME_PICTURE = 0x03
            };

            public enum NVVE_DI_MODE
            {
                DI_OFF,
                DI_MEDIAN,
            }

            [StructLayout(LayoutKind.Sequential)]
            public class NVVE_EncodeFrameParams
            {
                public int Width;
                public int Height;
                public int Pitch;
                public NVVE_SurfaceFormat SurfFmt;
                public NVVE_PicStruct PictureStruc;
                public int topfieldfirst;
                public int repeatFirstField;
                public int progressiveFrame;
                public int bLast;
                public IntPtr picBuf;
            }

            [StructLayout(LayoutKind.Sequential)]
            public class NVVE_BeginFrameInfo
            {
                public int nFrameNumber;                   // Frame Number
                public int nPicType;                       // Picture Type
            }

            [StructLayout(LayoutKind.Sequential)]
            public class NVVE_EndFrameInfo
            {
                public int nFrameNumber;                   // Frame Number
                public int nPicType;                       // Picture Type
            }

            public enum NVVE_GPUOffloadLevel
            {
                NVVE_GPU_OFFLOAD_DEFAULT = -1,  // default setting for pel processing 
                NVVE_GPU_OFFLOAD_ESTIMATORS = 8,// pel processing on CPU, Motion Estimation on GPU
                NVVE_GPU_OFFLOAD_ALL = 16  // pel processing on GPU
            }

            public enum NVVE_EncodeParams: uint 
            {
                NVVE_OUT_SIZE = 1,
                NVVE_ASPECT_RATIO = 2,
                NVVE_FIELD_ENC_MODE = 3,
                NVVE_P_INTERVAL = 4,
                NVVE_IDR_PERIOD = 5,
                NVVE_DYNAMIC_GOP = 6,
                NVVE_RC_TYPE = 7,
                NVVE_AVG_BITRATE = 8,
                NVVE_PEAK_BITRATE = 9,
                NVVE_QP_LEVEL_INTRA = 10,
                NVVE_QP_LEVEL_INTER_P = 11,
                NVVE_QP_LEVEL_INTER_B = 12,
                NVVE_FRAME_RATE = 13,
                NVVE_DEBLOCK_MODE = 14,
                NVVE_PROFILE_LEVEL = 15,
                NVVE_FORCE_INTRA = 16, //DShow only
                NVVE_FORCE_IDR = 17, //DShow only
                NVVE_CLEAR_STAT = 18, //DShow only
                NVVE_SET_DEINTERLACE = 19,
                NVVE_PRESETS = 20,
                NVVE_IN_SIZE = 21,
                NVVE_STAT_NUM_CODED_FRAMES = 22, //DShow only
                NVVE_STAT_NUM_RECEIVED_FRAMES = 23, //DShow only
                NVVE_STAT_BITRATE = 24, //DShow only
                NVVE_STAT_NUM_BITS_GENERATED = 25, //DShow only
                NVVE_GET_PTS_DIFF_TIME = 26, //DShow only
                NVVE_GET_PTS_BASE_TIME = 27, //DShow only
                NVVE_GET_PTS_CODED_TIME = 28, //DShow only
                NVVE_GET_PTS_RECEIVED_TIME = 29, //DShow only
                NVVE_STAT_ELAPSED_TIME = 30, //DShow only
                NVVE_STAT_QBUF_FULLNESS = 31, //DShow only
                NVVE_STAT_PERF_FPS = 32, //DShow only
                NVVE_STAT_PERF_AVG_TIME = 33, //DShow only
                NVVE_DISABLE_CABAC = 34,
                NVVE_CONFIGURE_NALU_FRAMING_TYPE = 35,
                NVVE_DISABLE_SPS_PPS = 36,
                NVVE_SLICE_COUNT = 37,
                NVVE_GPU_OFFLOAD_LEVEL = 38,
                NVVE_GPU_OFFLOAD_LEVEL_MAX = 39,
                NVVE_MULTI_GPU = 40,
                NVVE_GET_GPU_COUNT = 41,
                NVVE_GET_GPU_ATTRIBUTES = 42,
                NVVE_FORCE_GPU_SELECTION = 43,
                NVVE_DEVICE_MEMORY_INPUT = 44,
                NVVE_DEVICE_CTX_LOCK = 45,
                NVVE_LOW_LATENCY = 46,
            };

            //GPU attributes
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public class NVVE_GPUAttributes
            {
                public int                     iGpuOrdinal;                // GPU device number
                [MarshalAs(UnmanagedType.ByValTStr,SizeConst=256)]
                public string                  cName;                      // string identifying GPU device
                public uint                    uiTotalGlobalMem;           // total global memory available on device in bytes
                public int                     iMajor;                     // GPU device compute capability major version number
                public int                     iMinor;                     // GPU device compute capability minor version number
                public int                     iClockRate;                 // GPU clock frequency in kilohertz
                public int                     iMultiProcessorCount;       // number of multiprocessors on the GPU device
                public NVVE_GPUOffloadLevel    MaxGpuOffloadLevel;         // max offload level supported for this GPU device
            }

            public delegate IntPtr PFNACQUIREBITSTREAM(ref int pBufferSize, IntPtr pUserdata);
            public delegate void PFNRELEASEBITSTREAM(int nBytesInBuffer, IntPtr cb, IntPtr pUserdata);
            public delegate void PFNONBEGINFRAME([MarshalAs(UnmanagedType.LPStruct)] NVVE_BeginFrameInfo pbfi, IntPtr pUserdata);
            public delegate void PFNONENDFRAME([MarshalAs(UnmanagedType.LPStruct)] NVVE_EndFrameInfo pefi, IntPtr pUserdata);

            [StructLayout(LayoutKind.Sequential)]
            public struct NVVE_CallbackParams
            {
                public IntPtr pfnacquirebitstream;
                public IntPtr pfnreleasebitstream;
                public IntPtr pfnonbeginframe;
                public IntPtr pfnonendframe;
            }

            [DllImport("nvcuvenc.dll")]
            public static extern int NVCreateEncoder(out IntPtr pNVEncoder);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVDestroyEncoder(IntPtr hNVEncoder);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVIsSupportedCodec(IntPtr hNVEncoder, uint dwCodecType);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVIsSupportedCodecProfile(IntPtr hNVEncoder, uint dwCodecType, uint dwProfileType);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVSetCodec(IntPtr hNVEncoder, uint dwCodecType);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVGetCodec(IntPtr hNVEncoder, out uint pdwCodecType);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVIsSupportedParam(IntPtr hNVEncoder, NVVE_EncodeParams dwParamType);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVSetParamValue(IntPtr hNVEncoder, NVVE_EncodeParams dwParamType, IntPtr pData);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVGetParamValue(IntPtr hNVEncoder, NVVE_EncodeParams dwParamType, IntPtr pData);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVSetDefaultParam(IntPtr hNVEncoder);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVCreateHWEncoder(IntPtr hNVEncoder);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVGetSPSPPS(IntPtr hNVEncoder, IntPtr pSPSPPSbfr, int nSizeSPSPPSbfr, out int pDatasize);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVEncodeFrame(IntPtr hNVEncoder, [MarshalAs(UnmanagedType.LPStruct)] NVVE_EncodeFrameParams pFrmIn, uint flag, IntPtr pData);

            [DllImport("nvcuvenc.dll")]
            public static extern int NVGetHWEncodeCaps();

            [DllImport("nvcuvenc.dll")]
            public static extern void NVRegisterCB(IntPtr hNVEncoder, NVVE_CallbackParams cb, IntPtr pUserdata);

        }

        protected struct Config
        {
            public uint Profile;
            public CUDA.NVVE_FIELD_MODE Fields;
            public CUDA.NVVE_RateCtrlType RateControl;
            public bool bCabac;
            public int IDRPeriod;
            public int PPeriod;
            public long Bitrate;
            public bool bGOP;
            public bool bAutoBitrate;
            public bool bDeblocking;
        }

        #endregion

        #region Constants

        private static readonly Guid MEDIASUBTYPE_H264 = new Guid("34363248-0000-0010-8000-00aa00389b71");
        private static readonly Guid MEDIASUBTYPE_AVC = new Guid("31435641-0000-0010-8000-00aa00389b71");

        #endregion

        #region Variables

        private IntPtr m_hEncoder = IntPtr.Zero;
        private IntPtr m_cuContext = IntPtr.Zero;
        private IntPtr m_cuDevice = IntPtr.Zero;
        private IntPtr m_cuCtxLock = IntPtr.Zero;
        private IntPtr m_dptrVideoFrame = IntPtr.Zero;
        private int m_nPitch = 0;
        private int m_nWidth = 0;
        private int m_nHeight = 0;
        private CUDA.NVVE_SurfaceFormat m_SurfaceFormat = CUDA.NVVE_SurfaceFormat.NV12;
        private IntPtr m_pActiveSample = IntPtr.Zero;
        private ManualResetEvent m_evReady = new ManualResetEvent(false);
        private AutoResetEvent m_evFlush = new AutoResetEvent(false);
        private long m_rtPosition = 0;
        private bool m_bSyncSample = false;
        private Config m_Config = new Config();
        private long m_rtFrameRate = 0;

        private CUDA.PFNACQUIREBITSTREAM m_fnAcquireBitstreamDelegate = new CUDA.PFNACQUIREBITSTREAM(AcquireBitstream);
        private CUDA.PFNRELEASEBITSTREAM m_fnReleaseBitstreamDelegate = new CUDA.PFNRELEASEBITSTREAM(ReleaseBitstream);
        private CUDA.PFNONBEGINFRAME m_fnOnBeginFrame = new CUDA.PFNONBEGINFRAME(OnBeginFrame);
        private CUDA.PFNONENDFRAME m_fnOnEndFrame = new CUDA.PFNONENDFRAME(OnEndFrame);

        #endregion

        #region Constructor

        public H264EncoderFilter()
            : base("CSharp H264 CUDA Encoder Filter")
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            m_Config.Profile = CUDA.NVVE_PROFILE_MAIN;
            m_Config.Fields = CUDA.NVVE_FIELD_MODE.MODE_FRAME;
            m_Config.RateControl = CUDA.NVVE_RateCtrlType.RC_VBR;
            m_Config.bCabac = false;
            m_Config.IDRPeriod = 5;
            m_Config.PPeriod = 1;            
            m_Config.bDeblocking = false;
            m_Config.bGOP = false;
            m_Config.Bitrate = 0;
            m_Config.bAutoBitrate = false;
        }

        ~H264EncoderFilter()
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif
            CloseEncoder();
        }

        #endregion

        #region Overridden Methods

        public override int Pause()
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            if (m_State == FilterState.Stopped)
            {
                if (m_hEncoder == IntPtr.Zero)
                {
                    HRESULT hr = OpenEncoder();
                    if (FAILED(hr)) return hr;
                }
                m_rtPosition = 0;
                m_evReady.Set();
                m_evFlush.Reset();
            }
            return base.Pause();
        }

        public override int Stop()
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            m_evFlush.Set();
            HRESULT hr = (HRESULT)base.Stop();
            if (m_pActiveSample != IntPtr.Zero)
            {
                Marshal.Release(m_pActiveSample);
                m_pActiveSample = IntPtr.Zero;
            }
            return hr;
        }

        public override int CheckInputType(AMMediaType pmt)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            string x1 = "X";
            if (pmt.majorType == MediaType.Video) x1 = "Video";

            string x2 = "X";
            if (pmt.formatType == FormatType.VideoInfo) x2 = "VideoInfo";
            if (pmt.formatType == FormatType.VideoInfo2) x2 = "VideoInfo2";

            string x3 = "X";
            if (pmt.subType == MediaSubType.YV12) x3 = "YV12";
            if (pmt.subType == MediaSubType.UYVY) x3 = "UYVY";
            if (pmt.subType == MediaSubType.NV12) x3 = "NV12";
            if (pmt.subType == MediaSubType.YUY2) x3 = "YUY2";
            if (pmt.subType == MediaSubType.YUYV) x3 = "YUYV";
            if (pmt.subType == MediaSubType.IYUV) x3 = "IYUV";
            if (pmt.subType == MediaSubType.RGB32) x3 = "RGB32";

            //Console.WriteLine("SUB TYPE " + pmt.subType);
            Console.WriteLine("CHECKED TYPE: " + x1 + " " + x2 + " " + x3);

            if (pmt.majorType != MediaType.Video)
            {
                Console.WriteLine("NOT ACCEPTED MAJOR TYPE");
                return VFW_E_TYPE_NOT_ACCEPTED;
            }
            if (pmt.formatType != FormatType.VideoInfo && pmt.formatType != FormatType.VideoInfo2)
            {
                Console.WriteLine("NOT ACCEPTED FORMAT TYPE");
                return VFW_E_TYPE_NOT_ACCEPTED;
            }
            if (pmt.formatPtr == IntPtr.Zero)
            {
                Console.WriteLine("NOT ACCEPTED NO POINTER");
                return VFW_E_TYPE_NOT_ACCEPTED;
            }
            if (
                   (pmt.subType != MediaSubType.YV12)
                && (pmt.subType != MediaSubType.UYVY)
                && (pmt.subType != MediaSubType.NV12)
                && (pmt.subType != MediaSubType.YUY2)
                && (pmt.subType != MediaSubType.YUYV)
                && (pmt.subType != MediaSubType.IYUV)
                )
            {
                Console.WriteLine("NOT ACCEPTED SUB TYPE");
                return VFW_E_TYPE_NOT_ACCEPTED;
            }

            if (
                    (pmt.subType == MediaSubType.YV12)
                ||  (pmt.subType == MediaSubType.NV12)
                ||  (pmt.subType == MediaSubType.IYUV)
                )
            {
                BitmapInfoHeader _bmi = pmt;
                Console.WriteLine("WIDTH " + _bmi.Width);

                if (ALIGN16(_bmi.Width) != _bmi.Width)
                {
                    Console.WriteLine("NOT ACCEPTED WIDTH 16x");
                    return VFW_E_TYPE_NOT_ACCEPTED;
                }
            }

            Console.WriteLine("ACCEPTED TYPE: " + x1 + " " + x2 + " " + x3);

            return NOERROR;
        }

        public override int CheckTransform(AMMediaType mtIn, AMMediaType mtOut)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            return NOERROR;
        }

        public override int DecideBufferSize(ref IMemAllocatorImpl pAlloc, ref AllocatorProperties prop)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            if (!Output.IsConnected) return VFW_E_NOT_CONNECTED;
            AllocatorProperties _actual = new AllocatorProperties();
            BitmapInfoHeader _bmi = (BitmapInfoHeader)Input.CurrentMediaType;
            if (_bmi == null) return VFW_E_INVALIDMEDIATYPE;
            prop.cbBuffer = _bmi.GetBitmapSize();
            prop.cbAlign = 1;
            if (prop.cbBuffer < Input.CurrentMediaType.sampleSize)
            {
                prop.cbBuffer = Input.CurrentMediaType.sampleSize;
            }
            if (prop.cbBuffer < _bmi.ImageSize)
            {
                prop.cbBuffer = _bmi.ImageSize;
            }
            int lSize = (_bmi.Width * Math.Abs(_bmi.Height) * (_bmi.BitCount + _bmi.BitCount % 8) / 8);
            if (prop.cbBuffer < lSize)
            {
                prop.cbBuffer = lSize;
            }
            prop.cBuffers = 10;
            int hr = pAlloc.SetProperties(prop, _actual);
            return hr;
        }

        public override int GetMediaType(int iPosition, ref AMMediaType pMediaType)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            if (pMediaType == null) return E_POINTER;
            if (iPosition < 0) return E_INVALIDARG;
            if (!Input.IsConnected) return VFW_E_NOT_CONNECTED;

            if (iPosition > 2) return VFW_S_NO_MORE_ITEMS;

            HRESULT hr = OpenEncoder();
            if (FAILED(hr)) return hr;

            pMediaType.majorType = MediaType.Video;
            pMediaType.subType = MEDIASUBTYPE_H264;

            BitmapInfoHeader _bmi = Input.CurrentMediaType;
            long _rate = 0;
            if (_rate == 0)
            {
                VideoInfoHeader _vih = Input.CurrentMediaType;
                if (_vih != null)
                {
                    _rate = _vih.AvgTimePerFrame;
                }
            }
            if (_rate == 0)
            {
                VideoInfoHeader2 _vih = Input.CurrentMediaType;
                if (_vih != null)
                {
                    _rate = _vih.AvgTimePerFrame;
                }
            }
            int _width = _bmi.Width;
            int _height = Math.Abs(_bmi.Height);

            if (iPosition == 0 || iPosition == 2)
            {
                int _extrasize = 0;
                IntPtr pExtraData = IntPtr.Zero;
                int _profile_idc = 0;
                int _level_idc = 0;
                VideoInfoHeader2 _vih = null;
                Mpeg2VideoInfo _mpegVI = null;
                if (iPosition == 0)
                {
                    _vih = new VideoInfoHeader2();
                    pMediaType.formatType = FormatType.VideoInfo2;
                }
                else
                {
                    pExtraData = Marshal.AllocCoTaskMem(100);
                    hr = (HRESULT)CUDA.NVGetSPSPPS(m_hEncoder, pExtraData, 100, out _extrasize);
                    if (FAILED(hr))
                    {
                        Marshal.FreeCoTaskMem(pExtraData);
                        return VFW_S_NO_MORE_ITEMS;
                    }
                    _profile_idc = Marshal.ReadByte(pExtraData,3);
                    _level_idc = Marshal.ReadByte(pExtraData,5);
                    pMediaType.subType = MEDIASUBTYPE_AVC;
                    pMediaType.formatType = FormatType.Mpeg2Video;
                    _mpegVI = new Mpeg2VideoInfo();
                    _vih = _mpegVI.hdr;
                }
                _vih.AvgTimePerFrame = _rate;

                _vih.BmiHeader.Size = Marshal.SizeOf(typeof(BitmapInfoHeader));
                _vih.BmiHeader.Width = _width;
                _vih.BmiHeader.Height = _height;
                _vih.BmiHeader.BitCount = 12;
                _vih.BmiHeader.ImageSize = _vih.BmiHeader.Width * Math.Abs(_vih.BmiHeader.Height) * (_vih.BmiHeader.BitCount > 0 ? _vih.BmiHeader.BitCount : 24) / 8;
                _vih.BmiHeader.Planes = 1;
                _vih.BmiHeader.Compression = MAKEFOURCC('H', '2', '6', '4');

                _vih.SrcRect.right = _width;
                _vih.SrcRect.bottom = _height;
                _vih.TargetRect.right = _width;
                _vih.TargetRect.bottom = _height;

                if (m_Config.Bitrate == 0)
                {
                    _vih.BitRate = _vih.BmiHeader.ImageSize;
                }
                else
                {
                    _vih.BitRate = (int)m_Config.Bitrate;
                }
                pMediaType.sampleSize = _vih.BmiHeader.ImageSize;
                int _aspectW = _width;
                int _aspectH = _height;
                if (_aspectW != 0 && _aspectH != 0)
                {
                    int a = _aspectW;
                    int b = _aspectH;
                    int c = a % b;
                    while (c != 0)
                    {
                        a = b;
                        b = c;
                        c = a % b;
                    }
                    _aspectW /= b;
                    _aspectH /= b;
                }

                _vih.PictAspectRatioX = _aspectW;
                _vih.PictAspectRatioY = _aspectH;

                _vih.InterlaceFlags = AMInterlace.None;

                if (iPosition == 0)
                {
                    pMediaType.SetFormat(_vih);
                }
                else
                {
                    _vih.BmiHeader.Compression = MAKEFOURCC('A', 'V', 'C', '1');
                    _mpegVI.dwProfile = (uint)_profile_idc;
                    _mpegVI.dwLevel = (uint)_level_idc;
                    _mpegVI.dwFlags = 4;
                    if (pExtraData != IntPtr.Zero)
                    {
                        _mpegVI.cbSequenceHeader = (uint)_extrasize;
                        _mpegVI.dwSequenceHeader = new byte[_extrasize];
                        Marshal.Copy(pExtraData, _mpegVI.dwSequenceHeader, 0, _extrasize);
                        Marshal.FreeCoTaskMem(pExtraData);
                    }
                    pMediaType.formatSize = Marshal.SizeOf(_mpegVI) + _extrasize;
                    pMediaType.formatPtr = Marshal.AllocCoTaskMem(pMediaType.formatSize);
                    Marshal.StructureToPtr(_mpegVI, pMediaType.formatPtr,false);
                }
            }
            if (iPosition == 1)
            {
                pMediaType.formatType = FormatType.VideoInfo;
                VideoInfoHeader _vih = new VideoInfoHeader();

                _vih.AvgTimePerFrame = _rate;

                _vih.BmiHeader.Size = Marshal.SizeOf(typeof(BitmapInfoHeader));
                _vih.BmiHeader.Width = _width;
                _vih.BmiHeader.Height = _height;
                _vih.BmiHeader.BitCount = 12;
                _vih.BmiHeader.ImageSize = _vih.BmiHeader.Width * Math.Abs(_vih.BmiHeader.Height) * (_vih.BmiHeader.BitCount > 0 ? _vih.BmiHeader.BitCount : 24) / 8;
                _vih.BmiHeader.Planes = 1;
                _vih.BmiHeader.Compression = MAKEFOURCC('H', '2', '6', '4');

                _vih.SrcRect.right = _width;
                _vih.SrcRect.bottom = _height;
                _vih.TargetRect.right = _width;
                _vih.TargetRect.bottom = _height;

                if (m_Config.Bitrate == 0)
                {
                    _vih.BitRate = _vih.BmiHeader.ImageSize;
                }
                else
                {
                    _vih.BitRate = (int)m_Config.Bitrate;
                }
                pMediaType.sampleSize = _vih.BmiHeader.ImageSize;

                pMediaType.SetFormat(_vih);
            }

            return NOERROR;
        }

        public override int SetMediaType(PinDirection _direction, AMMediaType mt)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            HRESULT hr = (HRESULT)base.SetMediaType(_direction, mt);
            if (hr.Failed) return hr;
            if (_direction == PinDirection.Input)
            {
                BitmapInfoHeader _bmi = mt;
                if (_bmi != null)
                {
                    m_nWidth = _bmi.Width;
                    m_nHeight = Math.Abs(_bmi.Height);
                }
                if (mt.subType == MediaSubType.YV12)
                {
                    m_SurfaceFormat = CUDA.NVVE_SurfaceFormat.YV12;
                }
                if (mt.subType == MediaSubType.YUYV || mt.subType == MediaSubType.YUY2)
                {
                    m_SurfaceFormat = CUDA.NVVE_SurfaceFormat.YUY2;
                }
                if (mt.subType == MediaSubType.NV12)
                {
                    m_SurfaceFormat = CUDA.NVVE_SurfaceFormat.NV12;
                }
                if (mt.subType == MediaSubType.UYVY)
                {
                    m_SurfaceFormat = CUDA.NVVE_SurfaceFormat.UYVY;
                }
                if (mt.subType == MediaSubType.IYUV)
                {
                    m_SurfaceFormat = CUDA.NVVE_SurfaceFormat.IYUV;
                }
                hr = OpenEncoder();
                if (FAILED(hr)) 
                    return hr;
            }
            if (_direction == PinDirection.Output)
            {
                hr = OpenEncoder();
                if (FAILED(hr)) return hr;
                int nFramingType = (mt.subType != MEDIASUBTYPE_AVC) ? 0 : 4;
                IntPtr _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(nFramingType));
                Marshal.WriteInt32(_ptr, nFramingType);
                hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_CONFIGURE_NALU_FRAMING_TYPE, _ptr);
                Marshal.FreeCoTaskMem(_ptr);
            }
            return hr;
        }

        public override int BreakConnect(PinDirection _direction)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            HRESULT hr = (HRESULT)base.BreakConnect(_direction);
            if (hr.Failed) return hr;
            if (_direction == PinDirection.Input)
            {
                CloseEncoder();
            }
            return hr;
        }

        public override int CompleteConnect(PinDirection _direction, ref IPinImpl pPin)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            HRESULT hr = (HRESULT)base.CompleteConnect(_direction, ref pPin);
	        if (hr.Failed) return hr;
            if (_direction == PinDirection.Input && Output.IsConnected)
            {
                hr = (HRESULT)Output.ReconnectPin();
            }
            return hr;
        }

        public override int BeginFlush()
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            HRESULT hr = (HRESULT)base.BeginFlush();
	        if (hr.Failed) return hr;
	        m_evFlush.Set();
	        return hr;
        }

        public override int EndFlush()
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            m_evFlush.Reset();
            return base.EndFlush();
        }

        public override int NewSegment(long tStart, long tStop, double dRate)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            m_rtPosition = 0;
            return base.NewSegment(tStart, tStop, dRate);
        }

        public override int OnReceive(ref IMediaSampleImpl _sample)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            {
		        AMMediaType pmt;
		        if (S_OK == _sample.GetMediaType(out pmt))
		        {
			        SetMediaType(PinDirection.Input,pmt);
			        Input.CurrentMediaType.Set(pmt);
			        pmt.Free();
		        }
	        }
            CUDA.NVVE_EncodeFrameParams _params = new CUDA.NVVE_EncodeFrameParams();
            _params.Height           = m_nHeight;
            _params.Width            = m_nWidth;
	        if (m_nPitch != 0)
	        {
		        _params.Pitch		 = m_nPitch;
	        }
	        else
	        {
		        _params.Pitch        = m_nWidth;
		        if (m_SurfaceFormat == CUDA.NVVE_SurfaceFormat.YUY2 || m_SurfaceFormat == CUDA.NVVE_SurfaceFormat.UYVY)
		        {
			        _params.Pitch *= 2;
		        }
	        }
            _params.PictureStruc     = CUDA.NVVE_PicStruct.FRAME_PICTURE; 
            _params.SurfFmt          = m_SurfaceFormat;
	        _params.progressiveFrame = 1;
            _params.repeatFirstField = 0;
	        _params.topfieldfirst    = 0;

	        _sample.GetPointer(out _params.picBuf);
	        _params.bLast = m_evFlush.WaitOne(0) ? 1 : 0;
	        HRESULT hr = (HRESULT)CUDA.NVEncodeFrame(m_hEncoder, _params, 0, m_dptrVideoFrame);

	        m_evReady.Reset();
	        return hr;
        }

        public override int Transform(ref IMediaSampleImpl pIn, ref IMediaSampleImpl pOut)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif
            return E_UNEXPECTED;
        }

        #endregion

        #region Methods

        protected HRESULT OpenEncoder()
        {

#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif
            if (m_hEncoder != IntPtr.Zero) return NOERROR;
            HRESULT hr = (HRESULT)CUDA.NVCreateEncoder(out m_hEncoder);
            if (FAILED(hr)) return hr;
            hr = (HRESULT)CUDA.NVSetCodec(m_hEncoder, CUDA.NV_CODEC_TYPE_H264);

            if (FAILED(hr))
            {
                CloseEncoder();
                return hr;
            }
            IntPtr _ptr;
            int nCount = 0;
            _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(nCount));
            hr = (HRESULT)CUDA.NVGetParamValue(m_hEncoder,CUDA.NVVE_EncodeParams.NVVE_GET_GPU_COUNT, _ptr);
            nCount = Marshal.ReadInt32(_ptr);
            Marshal.FreeCoTaskMem(_ptr);
            if (FAILED(hr) || nCount == 0)
            {
                CloseEncoder();
                if (hr.Succeeded) hr = E_UNEXPECTED;
                return hr;
            }
            int nBestOne = 0;
            if (nCount > 1)
            {
                int _perfomance = 0;
                CUDA.NVVE_GPUAttributes _attributes;
                for (int i = 0; i < nCount; i++)
                {
                    _attributes = new CUDA.NVVE_GPUAttributes();
                    _attributes.iGpuOrdinal = i;
                    _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(_attributes));
                    Marshal.StructureToPtr(_attributes, _ptr, true);
                    hr = (HRESULT)CUDA.NVGetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_GET_GPU_ATTRIBUTES, _ptr);
                    if (SUCCEEDED(hr))
                    {
                        _attributes = (CUDA.NVVE_GPUAttributes)Marshal.PtrToStructure(_ptr, typeof(CUDA.NVVE_GPUAttributes));
                        int _temp = _attributes.iClockRate * _attributes.iMultiProcessorCount;
                        if (_temp > _perfomance)
                        {
                            nBestOne = i;
                            _perfomance = _temp;
                        }
                    }
                    Marshal.FreeCoTaskMem(_ptr);
                }
            }
            _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(nBestOne));
            Marshal.WriteInt32(_ptr,nBestOne);
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_FORCE_GPU_SELECTION, _ptr);
            if (FAILED(hr))
            {
                Marshal.FreeCoTaskMem(_ptr);
                CloseEncoder();
                return hr;
            }

            CUDA.NVVE_GPUOffloadLevel eMaxOffloadLevel = CUDA.NVVE_GPUOffloadLevel.NVVE_GPU_OFFLOAD_DEFAULT;
            Marshal.WriteInt32(_ptr, (int)eMaxOffloadLevel);
            hr = (HRESULT)CUDA.NVGetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_GPU_OFFLOAD_LEVEL_MAX, _ptr);
            if (FAILED(hr))
            {
                Marshal.FreeCoTaskMem(_ptr);
                CloseEncoder();
                return hr;
            }
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_GPU_OFFLOAD_LEVEL, _ptr);
            if (FAILED(hr))
            {
                Marshal.FreeCoTaskMem(_ptr);
                CloseEncoder();
                return hr;
            }

            Marshal.FreeCoTaskMem(_ptr);
            hr = ConfigureEncoder();
            if (FAILED(hr))
            {
                CloseEncoder();
                return hr;
            }
            CUDA.NVVE_CallbackParams _callback = new CUDA.NVVE_CallbackParams();
            _callback.pfnacquirebitstream = Marshal.GetFunctionPointerForDelegate(m_fnAcquireBitstreamDelegate);
            _callback.pfnonbeginframe = Marshal.GetFunctionPointerForDelegate(m_fnOnBeginFrame);
            _callback.pfnonendframe = Marshal.GetFunctionPointerForDelegate(m_fnOnEndFrame);
            _callback.pfnreleasebitstream = Marshal.GetFunctionPointerForDelegate(m_fnReleaseBitstreamDelegate);

	        CUDA.NVRegisterCB(m_hEncoder, _callback, Marshal.GetIUnknownForObject(this));

	        hr = (HRESULT)CUDA.NVCreateHWEncoder(m_hEncoder);

	        if (FAILED(hr)) 
	        {
		        CloseEncoder();
		        return hr;
	        }

            _ptr = Marshal.AllocCoTaskMem(4);
            hr = (HRESULT)CUDA.NVGetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_CONFIGURE_NALU_FRAMING_TYPE, _ptr);
            if (FAILED(hr))
            {
                Marshal.FreeCoTaskMem(_ptr);
                CloseEncoder();
                return hr;
            }
            int sps = Marshal.ReadInt32(_ptr);

            return S_OK;
        }

        protected HRESULT CloseEncoder()
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            if (m_hEncoder != IntPtr.Zero)
            {
                CUDA.NVDestroyEncoder(m_hEncoder);
                m_hEncoder = IntPtr.Zero;
            }
            return NOERROR;
        }

        protected HRESULT ConfigureEncoder()
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif
            if (!Input.IsConnected) return VFW_E_NOT_CONNECTED;
	        HRESULT hr;
	        BitmapInfoHeader _bmi = Input.CurrentMediaType;
            IntPtr _ptr = IntPtr.Zero;
	        int[] aiSize = new int[2];
            aiSize[0] = _bmi.Width;
            aiSize[1] = Math.Abs(_bmi.Height);
            CUDA.NVVE_DI_MODE _interlaced = CUDA.NVVE_DI_MODE.DI_OFF;
            int nFramingType = 0;
            if (Output.IsConnected)
            {
                if (Output.CurrentMediaType.subType == MEDIASUBTYPE_AVC)
                {
                    nFramingType = 4;
                }
            }
            _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * aiSize.Length);
            Marshal.Copy(aiSize,0,_ptr,aiSize.Length);
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_OUT_SIZE, _ptr);
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_IN_SIZE, _ptr);
            Marshal.WriteInt32(_ptr,(int)m_Config.Fields);
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_FIELD_ENC_MODE, _ptr);
            Marshal.WriteInt32(_ptr, (int)m_Config.PPeriod);
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_P_INTERVAL, _ptr);
            Marshal.WriteInt32(_ptr, (int)m_Config.IDRPeriod);
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_IDR_PERIOD, _ptr);
            Marshal.WriteInt32(_ptr, (int)m_Config.Profile);
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_PROFILE_LEVEL, _ptr);
            Marshal.WriteInt32(_ptr, (int)_interlaced);
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_SET_DEINTERLACE, _ptr);
            Marshal.WriteInt32(_ptr, (int)nFramingType);
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_CONFIGURE_NALU_FRAMING_TYPE, _ptr);
            Marshal.WriteInt32(_ptr, (int)(m_Config.bGOP ? 1 : 0));
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_DYNAMIC_GOP, _ptr);
            Marshal.WriteInt32(_ptr, (int)m_Config.RateControl);
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_RC_TYPE, _ptr);
            Marshal.WriteInt32(_ptr, (int)(m_Config.bDeblocking ? 1 : 0));
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_DEBLOCK_MODE, _ptr);
            Marshal.FreeCoTaskMem(_ptr);
            long _rate = 0;
            if (_rate == 0)
            {
                VideoInfoHeader _vih = Input.CurrentMediaType;
                if (_vih != null)
                {
                    _rate = _vih.AvgTimePerFrame;
                }
            }
            if (_rate == 0)
            {
                VideoInfoHeader2 _vih = Input.CurrentMediaType;
                if (_vih != null)
                {
                    _rate = _vih.AvgTimePerFrame;
                }
            }
            int[] aiFrameRate = new int[2];
            if (_rate != 0)
            {
                long a = UNITS;
                long b = _rate;
                long c = a % b;
                while (c != 0)
                {
                    a = b;
                    b = c;
                    c = a % b;
                }
                aiFrameRate[0] = (int)(UNITS / b);
                aiFrameRate[1] = (int)(_rate / b);
                _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * aiFrameRate.Length);
                Marshal.Copy(aiFrameRate, 0, _ptr, aiFrameRate.Length);
                hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_FRAME_RATE, _ptr);
                Marshal.FreeCoTaskMem(_ptr);
            }
            m_rtFrameRate = _rate;
            if (m_nWidth != 0 && m_nHeight != 0)
	        {
		        int[] aiAspectRatio = new int[3];
		        int a = m_nWidth;
		        int b = m_nHeight;
		        int c = a % b;
		        while(c != 0) 
		        {
			        a = b;
			        b = c;
			        c = a % b;
		        }
		        aiAspectRatio[0] = (int)(m_nWidth / b);
		        aiAspectRatio[1] = (int)(m_nHeight / b);
                aiAspectRatio[2] = (int)CUDA.NVVE_ASPECT_RATIO_TYPE.ASPECT_RATIO_DAR;
                _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * aiAspectRatio.Length);
                Marshal.Copy(aiAspectRatio, 0, _ptr, aiAspectRatio.Length);
                hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_ASPECT_RATIO, _ptr);
                Marshal.FreeCoTaskMem(_ptr);
	        }
            _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));
            long lRecomended = ((long)m_nWidth * (long)m_nHeight) << 3;
            {
                long lBitrate = m_Config.Bitrate;
                if (lBitrate <= 0 || (lBitrate < lRecomended && m_Config.bAutoBitrate))
                {
                    lBitrate = lRecomended;
                }
                Marshal.WriteInt32(_ptr, (int)lBitrate);
                hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_AVG_BITRATE, _ptr);
                lBitrate *= 10;
                Marshal.WriteInt32(_ptr, (int)lBitrate);
                hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_PEAK_BITRATE, _ptr);
            }
            int nCabac = (m_Config.bCabac ? 0 : 1);
            Marshal.WriteInt32(_ptr, nCabac);
            hr = (HRESULT)CUDA.NVSetParamValue(m_hEncoder, CUDA.NVVE_EncodeParams.NVVE_DISABLE_CABAC, _ptr);
            Marshal.FreeCoTaskMem(_ptr);
            return S_OK;
        }

        #endregion

        #region Callbacks

        private static IntPtr AcquireBitstream(ref int pBufferSize, IntPtr pUserData)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(typeof(H264EncoderFilter).FullName + " - " + method.Name + " - " + method.ToString());
#endif

            H264EncoderFilter pEncoder = (H264EncoderFilter)Marshal.GetObjectForIUnknown(pUserData);
	        pEncoder.m_pActiveSample = IntPtr.Zero;
	        pBufferSize = 0;
	        HRESULT hr = (HRESULT)pEncoder.Output.GetDeliveryBuffer(out pEncoder.m_pActiveSample,null,null, AMGBF.None);
	        IntPtr pBuffer = IntPtr.Zero;
	        if (SUCCEEDED(hr))
	        {
                IMediaSampleImpl pSample = new IMediaSampleImpl(pEncoder.m_pActiveSample);
		        {
			        AMMediaType pmt;
			        if (S_OK == pSample.GetMediaType(out pmt))
			        {
                        pEncoder.SetMediaType(PinDirection.Output, pmt);
				        pEncoder.Output.CurrentMediaType.Set(pmt);
				        pmt.Free();
			        }
		        }
		        pSample.SetSyncPoint(pEncoder.m_bSyncSample);
                pBufferSize = pSample.GetSize();
		        pSample.GetPointer(out pBuffer);
	        }
	        return pBuffer;
        }

        private static DateTime _FPSStart = DateTime.Now;
        private static int _FPSCount = 1;

        private static DateTime _FPS2Start = DateTime.Now;
        private static int _FPS2Count = 0;

        private static DateTime _BITRATEStart = DateTime.Now;
        private static int _BITRATECount = 0;

        private static int _sam = 0;
        private static int _mode = 0;

        private static void ReleaseBitstream(int nBytesInBuffer, IntPtr cb, IntPtr pUserData)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(typeof(H264EncoderFilter).FullName + " - " + method.Name + " - " + method.ToString());
#endif
            H264EncoderFilter pEncoder = (H264EncoderFilter)Marshal.GetObjectForIUnknown(pUserData);
            IMediaSampleImpl pSample = new IMediaSampleImpl(pEncoder.m_pActiveSample);
	        pEncoder.m_pActiveSample = IntPtr.Zero;
            if (pSample.IsValid)
            {
                long _start, _stop;
                _start = pEncoder.m_rtPosition;
                _stop = _start + pEncoder.m_rtFrameRate;
                pEncoder.m_rtPosition = _stop;
                pSample.SetTime(_start, _stop);
                pSample.SetActualDataLength(nBytesInBuffer);


                int s = nBytesInBuffer;
                //byte[] b = new byte[s];                
                //Marshal.Copy(cb, b, 0, b.Length);                

                //Console.WriteLine("s " + s);

                //File.WriteAllBytes("d:\\frames\\" + _sam + ".264", b);
                //if (pEncoder.m_bSyncSample)
                //{
                //    _sam++;

                //    if (_sam % 5 == 0)
                //    {
                //        _mode = 1;
                //    }
                //    else
                //    {
                //        _mode = 0;
                //    }
                //}

                //if (_mode == 0)

                pEncoder.Output.Deliver(ref pSample);
                //FileStream f = new FileStream("d:\\frames\\a.264", FileMode.Append, FileAccess.Write);
                //f.Write(b, 0, b.Length);
                //f.Flush();
                //f.Close();

                //if (_sam < 10)
                //    pEncoder.Output.Deliver(ref pSample);

                //bool flag = true;
                //if (_sam % 3 != 0 && !pEncoder.m_bSyncSample)
                //{                    
                //    flag = false;
                //}
                //else
                //{
                //    pEncoder.Output.Deliver(ref pSample);
                //}



                pSample._Release();

#if HAMED_LOG_FPS_2                        
            _FPS2Count++;
            if (_FPS2Count == 10)
            {
                DateTime FPS2End = DateTime.Now;
                double m2 = FPS2End.Subtract(_FPS2Start).TotalMilliseconds;
                int fps = (int)(_FPS2Count * 1000 / m2);                                
                Console.WriteLine("FPS: Avg " + fps + " [100 Frames in " + (int)m2 + "ms]");
                _FPS2Start = FPS2End;
                _FPS2Count = 0;
            }
#endif

#if HAMED_LOG_BITRATE
                {
                    DateTime End = DateTime.Now;
                    double m = End.Subtract(_BITRATEStart).TotalMilliseconds;
                    _BITRATECount += s;
                    if (m != 0)
                    {
                        double b = Math.Round(_BITRATECount * 1000.0 / m / 1024 / 1024, 2);
                        //Console.WriteLine(b + " MB");
                        _BITRATEStart = End;
                        _BITRATECount = 0;
                    }
                }
#endif

#if HAMED_LOG_FPS_1
                {
                    DateTime FPSEnd = DateTime.Now;
                    double m = FPSEnd.Subtract(_FPSStart).TotalMilliseconds;
                    if (m != 0)
                    {
                        int fps = (int)(1000 / m);
                        fps *= _FPSCount;
                        //Console.WriteLine((pEncoder.m_bSyncSample ? "I" : "P") + ": " + fps + " [" + _FPSCount + " Frames in " + (int)m + "ms] " + s + "b");
                        _FPSStart = FPSEnd;
                        _FPSCount = 1;
                    }
                    else
                    {
                        _FPSCount++;
                    }
                }
#endif
            }


        }

        private static void OnBeginFrame(CUDA.NVVE_BeginFrameInfo pbfi, IntPtr pUserData)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(typeof(H264EncoderFilter).FullName + " - " + method.Name + " - " + method.ToString());
#endif

            H264EncoderFilter pEncoder = (H264EncoderFilter)Marshal.GetObjectForIUnknown(pUserData);
            pEncoder.m_bSyncSample = (pbfi != null && pbfi.nPicType == CUDA.NVVE_PIC_TYPE_IFRAME);

            //if (pbfi.nPicType == CUDA.NVVE_PIC_TYPE_IFRAME)
            //    Console.WriteLine("I");
            //else if (pbfi.nPicType == CUDA.NVVE_PIC_TYPE_PFRAME)
            //    Console.WriteLine("P");
        }

        private static void OnEndFrame(CUDA.NVVE_EndFrameInfo pefi, IntPtr pUserData)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(typeof(H264EncoderFilter).FullName + " - " + method.Name + " - " + method.ToString());
#endif

        }
        
        #endregion

        #region IH264Encoder Members

        public int get_Bitrate(out int plValue)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            plValue = 0;
            if (m_Config.Bitrate > 0)
            {
                plValue = (int)m_Config.Bitrate;
            }
            else
            {
                plValue = (m_nWidth * m_nHeight) << 3;
            }
            return NOERROR;
        }

        public int put_Bitrate(int lValue)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            if (lValue <= 0) return E_INVALIDARG;
            if (m_Config.Bitrate != lValue)
            {
                m_Config.Bitrate = lValue;
                if (m_hEncoder != IntPtr.Zero)
                {
                    if (m_State != FilterState.Stopped) return VFW_E_NOT_STOPPED;
                    CloseEncoder();
                    if (Input.IsConnected && Output.IsConnected && Output.CurrentMediaType.subType == MEDIASUBTYPE_AVC)
                    {
                        Output.ReconnectPin();
                    }
                }
            }
            return NOERROR;
        }

        public int get_RateControl(out rate_control pValue)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            pValue = (rate_control)m_Config.RateControl;
            return NOERROR;
        }

        public int put_RateControl(rate_control value)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            if (m_Config.RateControl != (CUDA.NVVE_RateCtrlType)value)
            {
                m_Config.RateControl = (CUDA.NVVE_RateCtrlType)value;
                if (m_hEncoder != IntPtr.Zero)
                {
                    if (m_State != FilterState.Stopped) return VFW_E_NOT_STOPPED;
                    CloseEncoder();
                    if (Input.IsConnected && Output.IsConnected && Output.CurrentMediaType.subType == MEDIASUBTYPE_AVC)
                    {
                        Output.ReconnectPin();
                    }
                }
            }
            return NOERROR;
        }

        public int get_MbEncoding(out mb_encoding pValue)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            pValue = (mb_encoding)(m_Config.bCabac ? mb_encoding.CABAC : mb_encoding.CAVLC);
            return NOERROR;
        }

        public int put_MbEncoding(mb_encoding value)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            if (m_Config.bCabac != (value == mb_encoding.CABAC))
            {
                m_Config.bCabac = (value == mb_encoding.CABAC ? true : false);
                if (m_hEncoder != IntPtr.Zero)
                {
                    if (m_State != FilterState.Stopped) return VFW_E_NOT_STOPPED;
                    CloseEncoder();
                    if (Input.IsConnected && Output.IsConnected && Output.CurrentMediaType.subType == MEDIASUBTYPE_AVC)
                    {
                        Output.ReconnectPin();
                    }
                }
            }
            return NOERROR;
        }

        public int get_Deblocking(out bool pValue)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            pValue = m_Config.bDeblocking;
            return NOERROR;
        }

        public int put_Deblocking(bool value)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            if (m_Config.bDeblocking != value)
            {
                m_Config.bDeblocking = value;
                if (m_hEncoder != IntPtr.Zero)
                {
                    if (m_State != FilterState.Stopped) return VFW_E_NOT_STOPPED;
                    CloseEncoder();
                    if (Input.IsConnected && Output.IsConnected && Output.CurrentMediaType.subType == MEDIASUBTYPE_AVC)
                    {
                        Output.ReconnectPin();
                    }
                }
            }
            return NOERROR;
        }

        public int get_GOP(out bool pValue)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            pValue = m_Config.bGOP;
            return NOERROR;
        }

        public int put_GOP(bool value)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            if (m_Config.bGOP != value)
            {
                m_Config.bGOP = value;
                if (m_hEncoder != IntPtr.Zero)
                {
                    if (m_State != FilterState.Stopped) return VFW_E_NOT_STOPPED;
                    CloseEncoder();
                    if (Input.IsConnected && Output.IsConnected && Output.CurrentMediaType.subType == MEDIASUBTYPE_AVC)
                    {
                        Output.ReconnectPin();
                    }
                }
            }
            return NOERROR;
        }

        public int get_AutoBitrate(out bool pValue)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            pValue = m_Config.bAutoBitrate;
            return NOERROR;
        }

        public int put_AutoBitrate(bool value)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            if (m_Config.bAutoBitrate != value)
            {
                m_Config.bAutoBitrate = value;
                if (m_hEncoder != IntPtr.Zero)
                {
                    if (m_State != FilterState.Stopped) return VFW_E_NOT_STOPPED;
                    CloseEncoder();
                    if (Input.IsConnected && Output.IsConnected && Output.CurrentMediaType.subType == MEDIASUBTYPE_AVC)
                    {
                        Output.ReconnectPin();
                    }
                }
            }
            return NOERROR;
        }

        public int get_Profile(out profile_idc pValue)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            pValue = (profile_idc)(m_Config.Profile & 0xff);
            return NOERROR;
        }

        public int put_Profile(profile_idc value)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            uint _profile = ((int)value == 0 ? (uint)profile_idc.PROFILE_MAIN : (uint)value);
            if ((m_Config.Profile & 0xff) != _profile)
            {
                m_Config.Profile = ((m_Config.Profile & 0xff00) | _profile);
                if (m_hEncoder != IntPtr.Zero)
                {
                    if (m_State != FilterState.Stopped) return VFW_E_NOT_STOPPED;
                    CloseEncoder();
                    if (Input.IsConnected && Output.IsConnected && Output.CurrentMediaType.subType == MEDIASUBTYPE_AVC)
                    {
                        Output.ReconnectPin();
                    }
                }
            }
            return NOERROR;
        }

        public int get_Level(out level_idc pValue)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            pValue = (level_idc)((m_Config.Profile >> 8) & 0xff);
            return NOERROR;
        }

        public int put_Level(level_idc value)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            uint _level = ((int)value == 0 ? 0xff : (uint)value);
            if (((m_Config.Profile >> 8) & 0xff) != _level)
            {
                m_Config.Profile = ((m_Config.Profile & 0x00ff) | (_level << 8));
                if (m_hEncoder != IntPtr.Zero)
                {
                    if (m_State != FilterState.Stopped) return VFW_E_NOT_STOPPED;
                    CloseEncoder();
                    if (Input.IsConnected && Output.IsConnected && Output.CurrentMediaType.subType == MEDIASUBTYPE_AVC)
                    {
                        Output.ReconnectPin();
                    }
                }
            }
            return NOERROR;
        }

        public int get_SliceIntervals(out int piIDR, out int piP)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            piIDR = m_Config.IDRPeriod;
            piP = m_Config.PPeriod;
            return NOERROR;
        }

        public int put_SliceIntervals(ref int piIDR, ref int piP)
        {
#if HAMED_LOG_H26_METHOD_INFO || HAMED_LOG_METHOD_INFO
            MethodBase method = new StackTrace().GetFrame(0).GetMethod();
            Console.WriteLine(this.GetType().FullName + " - " + method.Name + " - " + method.ToString());
#endif

            if ((object)piIDR == null && (object)piP == null) return E_POINTER;
            bool bModified = false;
            if ((object)piIDR != null)
            {
                if (piIDR < 1) return E_INVALIDARG;
                if (m_Config.IDRPeriod != piIDR)
                {
                    m_Config.IDRPeriod = piIDR;
                    bModified = TRUE;
                }
            }
            if ((object)piP != null)
            {
                if (piP < 1 || piP > 17) return E_INVALIDARG;
                if (m_Config.PPeriod != piP)
                {
                    m_Config.PPeriod = piP;
                    bModified = TRUE;
                }
            }
            if (m_hEncoder != IntPtr.Zero && bModified)
            {
                if (m_State != FilterState.Stopped) return VFW_E_NOT_STOPPED;
                CloseEncoder();
                if (Input.IsConnected && Output.IsConnected && Output.CurrentMediaType.subType == MEDIASUBTYPE_AVC)
                {
                    Output.ReconnectPin();
                }
            }
            return NOERROR;
        }

        #endregion
    }

    #endregion

}
