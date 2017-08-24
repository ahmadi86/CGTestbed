using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sonic;
using DirectShow;
using System.Threading;
using ExampleFilters;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private NullFilterGraph _graph;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _graph = new NullFilterGraph();
            _graph.Initialize();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _graph.Play();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _graph.Pause();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_graph != null)
            {
                _graph.Dispose();
                _graph = null;
            }
        }

        int c = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            c++;
            label1.Text = c.ToString();
        }
    }

   
}
