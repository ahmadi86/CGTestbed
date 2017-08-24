using System;
using System.Collections.Generic;
using System.Text;
using DirectShow;
using Sonic;
using ExampleFilters;
using System.Threading;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public class NullFilterGraph
    {
        private IGraphBuilder _graphBuilder;
        private IVideoWindow m_VideoWindow;
        private IMediaControl m_MediaControl;

        private DSFilter _filterCamera;        
        private DSFilter _filterYUV;
        private DSFilter _filterH264Encoder;
        private DSFilter _filterVMR9Renderer;
        private DSFilter _filterNullRenderer;
        private DSFilter _filterRTSPServer;

        public void Initialize()
        {
            _graphBuilder = (IGraphBuilder)new FilterGraph();

            _filterCamera = new DSFilter(new VirtualCamFilter());
            _graphBuilder.AddFilter(_filterCamera.Value, "Virtual Camera");

            //_filterCamera.Pins[0].Direction = PinDirection.

            //_filterVMR9Renderer = new DSFilter(new Guid("51B4ABF3-748F-4E3B-A276-C828330E926A"));
            //_graphBuilder.AddFilter(_filterVMR9Renderer.Value, "Renderer");

            _filterYUV = new DSFilter(new Guid("B179A682-641B-11D2-A4D9-0060080BA634"));
            _graphBuilder.AddFilter(_filterYUV.Value, "YUV");

            //_filterH264Encoder = new DSFilter(new Guid("3FD83588-D403-40A2-9739-5F75E1590AB8"));
            _filterH264Encoder= new DSFilter(new H264EncoderFilter());
            _graphBuilder.AddFilter(_filterH264Encoder.Value, "H264 En");

            //_filterNullRenderer = new DSFilter(new Guid("C1F400A4-3F08-11D3-9F0B-006008039E37"));
            //_graphBuilder.AddFilter(_filterNullRenderer.Value, "Null");

            _filterRTSPServer = new DSFilter(new Guid("EABF2C99-F9AD-43CD-8108-109D4E9FADC5"));
            _graphBuilder.AddFilter(_filterRTSPServer.Value, "RTSP Server");
            
            // Camera -> RGB2YUV -> H264 -> RTSP
            _filterCamera.OutputPin.ConnectDirect(_filterYUV.InputPin);            
            _filterYUV.OutputPin.ConnectDirect(_filterH264Encoder.InputPin);
            _filterH264Encoder.OutputPin.ConnectDirect(_filterRTSPServer.InputPin);            

            m_MediaControl = (IMediaControl)_graphBuilder;
            m_VideoWindow = (IVideoWindow)_graphBuilder;

            m_VideoWindow.put_Visible(0);
        }

        public bool IsStopped
        {
            get
            {
                if (m_MediaControl != null)
                {
                    FilterState _state;
                    int hr = 0;
                    do
                    {
                        hr = m_MediaControl.GetState(200, out _state);
                    }
                    while (hr == 0x00040237 || hr == 0x00040268);
                    if (hr == 0)
                    {
                        return _state == FilterState.Stopped;
                    }
                }
                return true;
            }
        }

        public bool IsRunning
        {
            get
            {
                if (m_MediaControl != null)
                {
                    FilterState _state;
                    int hr = 0;
                    do
                    {
                        hr = m_MediaControl.GetState(200, out _state);
                    }
                    while (hr == 0x00040237 || hr == 0x00040268);
                    if (hr == 0)
                    {
                        return _state == FilterState.Running;
                    }
                }
                return false;
            }
        }

        public bool IsPaused
        {
            get
            {
                if (m_MediaControl != null)
                {
                    FilterState _state;
                    int hr = 0;
                    do
                    {
                        hr = m_MediaControl.GetState(200, out _state);
                    }
                    while (hr == 0x00040237 || hr == 0x00040268);
                    if (hr == 0)
                    {
                        return _state == FilterState.Paused;
                    }
                }
                return false;
            }
        }

        public HRESULT Play()
        {
            int hr = 1;
            while (hr == 1)
            {
                hr = m_MediaControl.Run();
                if (hr == 1) Thread.Sleep(50);
            }

            return (HRESULT)hr;
        }

        public HRESULT Pause()
        {
            int hr = 0;

            if (!IsPaused)
            {
                hr = m_MediaControl.Pause();
            }

            return (HRESULT)hr;
        }

        public HRESULT Dispose()
        {
            try
            {
                m_MediaControl.Stop();
                m_VideoWindow.put_Visible(0);

                m_MediaControl = null;
                m_VideoWindow = null;

                if (_graphBuilder != null)
                {
                    _graphBuilder.RemoveFilter(_filterCamera.Value);
                    _graphBuilder.RemoveFilter(_filterVMR9Renderer.Value);

                    _filterCamera.Dispose();
                    _filterVMR9Renderer.Dispose();

                    _filterVMR9Renderer = null;
                    _filterCamera = null;

                    Marshal.ReleaseComObject(_graphBuilder);
                    _graphBuilder = null;
                }
                GC.Collect();

                return COMHelper.NOERROR;
            }
            catch
            {
                return COMHelper.E_FAIL;
            }
        }
    }
}
