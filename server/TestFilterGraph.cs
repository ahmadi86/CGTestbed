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
    public class TestFilterGraph
    {
        private IVideoWindow m_VideoWindow;
        private IMediaControl m_MediaControl;
        private DSFilter _filterCamera;
        private DSFilter _filterRenderer;
        private IGraphBuilder _graphBuilder;

        public void Initialize()
        {
            _graphBuilder = (IGraphBuilder)new FilterGraph();

            _filterCamera = new DSFilter(new VirtualCamFilter());
            _graphBuilder.AddFilter(_filterCamera.Value, "Virtual Camera");

            _filterRenderer = new DSFilter(new Guid("51B4ABF3-748F-4E3B-A276-C828330E926A"));
            _graphBuilder.AddFilter(_filterRenderer.Value, "Renderer");

            _filterCamera.OutputPin.ConnectDirect(_filterRenderer.InputPin);

            m_MediaControl = (IMediaControl)_graphBuilder;
            m_VideoWindow = (IVideoWindow)_graphBuilder;

            m_VideoWindow.put_Visible(1);
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
                    _graphBuilder.RemoveFilter(_filterRenderer.Value);

                    _filterCamera.Dispose();
                    _filterRenderer.Dispose();

                    _filterRenderer = null;
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
