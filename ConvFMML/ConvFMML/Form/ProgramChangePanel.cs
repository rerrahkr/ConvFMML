using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvFMML.Form
{
    public partial class ProgramChangePanel : BasePanel
    {
        private Settings.ControlCommand.ProgramChange settings;

        public ProgramChangePanel(Settings.ControlCommand.ProgramChange settings)
        {
            InitializeComponent();

            this.settings = settings;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            checkBox1.Checked = settings.Enable;
            textBox1.Text = settings.CommandCustom;
        }

        public override void UpdateSelections(MMLStyle mmlStyle)
        {
            panel2.Enabled = checkBox1.Checked;
            panel3.Enabled = (mmlStyle == MMLStyle.Custom);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            settings.Enable = cb.Checked;
            panel2.Enabled = cb.Checked;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            settings.CommandCustom = tb.Text;
        }
    }
}
