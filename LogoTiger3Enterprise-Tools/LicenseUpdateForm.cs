using System;
using System.Windows.Forms;

namespace UpdateApp
{
    public partial class LicenseUpdateForm : Form
    {
        public LicenseUpdateForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
    }
}