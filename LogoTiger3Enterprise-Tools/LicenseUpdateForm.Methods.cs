using System;
using System.Windows.Forms;

namespace UpdateApp
{
    partial class LicenseUpdateForm
    {
        public void UpdateStatus(string message)
        {
            if (txtStatus.IsDisposed || txtStatus == null)
                return;

            txtStatus.AppendText(message + Environment.NewLine);
            txtStatus.ScrollToCaret();
            Application.DoEvents();
        }

        public void UpdateProgress(int percentage)
        {
            if (progressBar.IsDisposed || progressBar == null)
                return;

            if (percentage < 0) percentage = 0;
            if (percentage > 100) percentage = 100;

            progressBar.Value = percentage;
            Application.DoEvents();
        }
    }
}