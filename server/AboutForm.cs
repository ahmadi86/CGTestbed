using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DirectShow.BaseClasses;

namespace ExampleFilters
{
    [ComVisible(true)]
    [Guid("3EFB481C-F35F-434c-A085-C3DFEFF65D94")]
    public partial class AboutForm : BasePropertyPage
    {
        public AboutForm()
        {
            InitializeComponent();
        }
    }
}
