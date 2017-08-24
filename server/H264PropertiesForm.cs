using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DirectShow.BaseClasses;
using System.Runtime.InteropServices;
using Sonic;
using DirectShow;

namespace ExampleFilters
{
    [ComVisible(true)]
    [Guid("5B9F540D-7F60-4fc2-8948-32E537A79DF3")]
    public partial class H264PropertiesForm : BasePropertyPage
    {
        #region Variables

        private IH264Encoder m_pH264Encoder = null;
        private IBaseFilter m_pFilter = null;

        #endregion

        #region Constructor

        public H264PropertiesForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Form Handlers

        private void H264PropertiesForm_Load(object sender, EventArgs e)
        {
            cmboProfile.Items.Add("Baseline");
            cmboProfile.Items.Add("Main");
            cmboProfile.Items.Add("High");

            cmboLevel.Items.Add("Auto");
            cmboLevel.Items.Add("1.0");
            cmboLevel.Items.Add("1.1");
            cmboLevel.Items.Add("1.2");
            cmboLevel.Items.Add("1.3");
            cmboLevel.Items.Add("2.0");
            cmboLevel.Items.Add("2.1");
            cmboLevel.Items.Add("2.2");
            cmboLevel.Items.Add("3.0");
            cmboLevel.Items.Add("3.1");
            cmboLevel.Items.Add("3.2");
            cmboLevel.Items.Add("4.0");
            cmboLevel.Items.Add("4.1");
            cmboLevel.Items.Add("4.2");
            cmboLevel.Items.Add("5.0");
            cmboLevel.Items.Add("5.1");

            cmboRateControl.Items.Add("VBR");
            cmboRateControl.Items.Add("CBR");

            cmboMBEncoding.Items.Add("CAVLC");
            cmboMBEncoding.Items.Add("CABAC");

            if (m_pH264Encoder != null)
            {
                bool bValue;

                m_pH264Encoder.get_AutoBitrate(out bValue);
                cbAutoBitrate.Checked = bValue;

                m_pH264Encoder.get_Deblocking(out bValue);
                cbDeblocking.Checked = bValue;

                m_pH264Encoder.get_AutoBitrate(out bValue);
                cbAutoBitrate.Checked = bValue;

                int nIDR;
                int nP;
                m_pH264Encoder.get_SliceIntervals(out nIDR, out nP);
                tbPPeriod.Text = nP.ToString();
                tbIDRPeriod.Text = nIDR.ToString();

                int nBitrate;
                m_pH264Encoder.get_Bitrate(out nBitrate);
                nBitrate = nBitrate / 1000;
                if (nBitrate > 0)
                {
                    tbBitrate.Text = nBitrate.ToString();
                }

                rate_control _rc;
                m_pH264Encoder.get_RateControl(out _rc);
                cmboRateControl.SelectedIndex = ((int)_rc) - 1;

                mb_encoding _mb;
                m_pH264Encoder.get_MbEncoding(out _mb);
                cmboMBEncoding.SelectedIndex = (int)_mb;

                int nIndex = 0;
                profile_idc _profile;
                m_pH264Encoder.get_Profile(out _profile);

                switch (_profile)
                {
                    case profile_idc.PROFILE_BASELINE:
                        nIndex = 0;
                        break;
                    case profile_idc.PROFILE_MAIN:
                        nIndex = 1;
                        break;
                    case profile_idc.PROFILE_HIGH:
                        nIndex = 2;
                        break;
                }
                cmboProfile.SelectedIndex = nIndex;

                nIndex = 0;
                level_idc _level;
                m_pH264Encoder.get_Level(out _level);
                switch (_level)
                {
                    case level_idc.LEVEL_AUTO:
                        nIndex = 0;
                        break;
                    case level_idc.LEVEL_1:
                        nIndex = 1;
                        break;
                    case level_idc.LEVEL_1_1:
                        nIndex = 2;
                        break;
                    case level_idc.LEVEL_1_2:
                        nIndex = 3;
                        break;
                    case level_idc.LEVEL_1_3:
                        nIndex = 4;
                        break;
                    case level_idc.LEVEL_2:
                        nIndex = 5;
                        break;
                    case level_idc.LEVEL_2_1:
                        nIndex = 6;
                        break;
                    case level_idc.LEVEL_2_2:
                        nIndex = 7;
                        break;
                    case level_idc.LEVEL_3:
                        nIndex = 8;
                        break;
                    case level_idc.LEVEL_3_1:
                        nIndex = 9;
                        break;
                    case level_idc.LEVEL_3_2:
                        nIndex = 10;
                        break;
                    case level_idc.LEVEL_4:
                        nIndex = 11;
                        break;
                    case level_idc.LEVEL_4_1:
                        nIndex = 12;
                        break;
                    case level_idc.LEVEL_4_2:
                        nIndex = 13;
                        break;
                    case level_idc.LEVEL_5:
                        nIndex = 14;
                        break;
                    case level_idc.LEVEL_5_1:
                        nIndex = 15;
                        break;
                }
                cmboLevel.SelectedIndex = nIndex;
            }
            timerCheck_Tick(sender, e);

            Dirty = false;
            timerCheck.Enabled = true;
        }

        private void HandleChanges(object sender, EventArgs e)
        {
            Dirty = true;
        }

        private void timerCheck_Tick(object sender, EventArgs e)
        {
            bool bActive = false;
            if (m_pFilter != null)
            {
                FilterState _state;
                if (HRESULT.S_OK == m_pFilter.GetState(100, out _state))
                {
                    bActive = (_state == FilterState.Stopped);
                }
            }
            cmboProfile.Enabled = bActive;
            cmboLevel.Enabled = bActive;
            cmboRateControl.Enabled = bActive;
            cmboMBEncoding.Enabled = bActive;
            cbGOP.Enabled = bActive;
            cbDeblocking.Enabled = bActive;
            cbAutoBitrate.Enabled = bActive;
            tbBitrate.Enabled = bActive;
            tbIDRPeriod.Enabled = bActive;
            tbPPeriod.Enabled = bActive;
        }

        #endregion

        #region Overridden Methods
        
        public override HRESULT OnConnect(IntPtr pUnknown)
        {
            m_pH264Encoder = (IH264Encoder)Marshal.GetObjectForIUnknown(pUnknown);
            m_pFilter = (IBaseFilter)Marshal.GetObjectForIUnknown(pUnknown);
            return HRESULT.NOERROR;
        }

        public override HRESULT OnDisconnect()
        {
            m_pH264Encoder = null;
            m_pFilter = null;
            return HRESULT.NOERROR;
        }

        public override HRESULT OnApplyChanges()
        {
            bool bActive = false;
            if (m_pFilter != null)
            {
                FilterState _state;
                if (HRESULT.S_OK == m_pFilter.GetState(100, out _state))
                {
                    bActive = (_state == FilterState.Stopped);
                }
            }
            if (bActive && m_pH264Encoder != null)
            {
                int nIndex = cmboProfile.SelectedIndex;
                profile_idc _profile = (profile_idc)0;
                switch (nIndex)
                {
                    case 0:
                        _profile = profile_idc.PROFILE_BASELINE;
                        break;
                    case 1:
                        _profile = profile_idc.PROFILE_MAIN;
                        break;
                    case 2:
                        _profile = profile_idc.PROFILE_HIGH;
                        break;
                }
                m_pH264Encoder.put_Profile(_profile);

                nIndex = cmboLevel.SelectedIndex;
                level_idc _level = (level_idc)0;
                switch (nIndex)
                {
                    case 0:
                        _level = level_idc.LEVEL_AUTO;
                        break;
                    case 1:
                        _level = level_idc.LEVEL_1;
                        break;
                    case 2:
                        _level = level_idc.LEVEL_1_1;
                        break;
                    case 3:
                        _level = level_idc.LEVEL_1_2;
                        break;
                    case 4:
                        _level = level_idc.LEVEL_1_3;
                        break;
                    case 5:
                        _level = level_idc.LEVEL_2;
                        break;
                    case 6:
                        _level = level_idc.LEVEL_2_1;
                        break;
                    case 7:
                        _level = level_idc.LEVEL_2_2;
                        break;
                    case 8:
                        _level = level_idc.LEVEL_3;
                        break;
                    case 9:
                        _level = level_idc.LEVEL_3_1;
                        break;
                    case 10:
                        _level = level_idc.LEVEL_3_2;
                        break;
                    case 11:
                        _level = level_idc.LEVEL_4;
                        break;
                    case 12:
                        _level = level_idc.LEVEL_4_1;
                        break;
                    case 13:
                        _level = level_idc.LEVEL_4_2;
                        break;
                    case 14:
                        _level = level_idc.LEVEL_5;
                        break;
                    case 15:
                        _level = level_idc.LEVEL_5_1;
                        break;
                }
                m_pH264Encoder.put_Level(_level);

                rate_control _rate = (rate_control)(cmboRateControl.SelectedIndex + 1);
                m_pH264Encoder.put_RateControl(_rate);

                m_pH264Encoder.put_MbEncoding((mb_encoding)cmboMBEncoding.SelectedIndex);
                m_pH264Encoder.put_Deblocking(cbDeblocking.Checked);
                m_pH264Encoder.put_GOP(cbGOP.Checked);
                m_pH264Encoder.put_AutoBitrate(cbAutoBitrate.Checked);

                int nIDR;
                int nP;

                int.TryParse(tbIDRPeriod.Text, out nIDR);
                int.TryParse(tbPPeriod.Text, out nP);
                m_pH264Encoder.put_SliceIntervals(ref nIDR, ref nP);

                m_pH264Encoder.get_SliceIntervals(out nIDR, out nP);
                tbPPeriod.Text = nP.ToString();
                tbIDRPeriod.Text = nIDR.ToString();

                int nBitrate = 0;
                int.TryParse(tbBitrate.Text, out nBitrate);
                nBitrate *= 1000;
                m_pH264Encoder.put_Bitrate(nBitrate);
                m_pH264Encoder.get_Bitrate(out nBitrate);
                nBitrate = nBitrate / 1000;
                if (nBitrate > 0)
                {
                    tbBitrate.Text = nBitrate.ToString();
                }
            }
            return HRESULT.NOERROR;
        }

        public override HRESULT OnDeactivate()
        {
            timerCheck.Enabled = true;
            return HRESULT.NOERROR;
        }

        #endregion
    }
}
