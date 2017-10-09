using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvFMML.Form
{
    public partial class VersionInfoForm : System.Windows.Forms.Form
    {
        public VersionInfoForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            descriptionLabel.Text = Common.AssemblyDescription;
            titleLabel.Text = Common.AssemblyTitle;
            versionLabel.Text = "Version " + Common.AssemblyFileVersion + "β";
            copyrightLabel.Text = Common.AssemblyCopyright;
            iconPictureBox.Image = Common.Icon.ToBitmap();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
