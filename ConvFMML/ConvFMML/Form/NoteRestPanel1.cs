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
    public partial class NoteRestPanel1 : BasePanel
    {
        private Settings.NoteRest settings;

        public NoteRestPanel1(Settings.NoteRest settings)
        {
            InitializeComponent();

            this.settings = settings;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Exclude HeaderCheckBox from GroupBox
            if (checkBox1.Parent == groupBox1)
            {
                groupBox1.Parent.Controls.Add(checkBox1);
                checkBox1.Left += groupBox1.Left;
                checkBox1.Top += groupBox1.Top;
                checkBox1.BringToFront();
            }

            comboBox1.SelectedIndex = settings.OctaveInNewLine;
            textBox1.Text = settings.OctaveCommandCustom;
            comboBox2.SelectedIndex = settings.OctaveDirection;
            comboBox3.SelectedIndex = settings.LengthStyle;
            checkBox1.Checked = settings.DotEnable;
            numericUpDown1.Value = settings.DotLength;
        }

        public override void UpdateSelections(MMLStyle mmlstyle)
        {
            if (mmlstyle == MMLStyle.Custom)
            {
                label2.Enabled = true;
                textBox1.Enabled = true;
            }
            else
            {
                label2.Enabled = false;
                textBox1.Enabled = false;
            }

            switch (mmlstyle)
            {
                case MMLStyle.MXDRV:
                case MMLStyle.NRTDRV:
                case MMLStyle.PMD:
                case MMLStyle.Mml2vgm:
                case MMLStyle.Custom:
                    label3.Enabled = true;
                    comboBox2.Enabled = true;
                    break;
                default:
                    label3.Enabled = false;
                    comboBox2.Enabled = false;
                    break;
            }

            if (mmlstyle == MMLStyle.MUCOM88)
            {
                label6.Enabled = false;
                numericUpDown1.Enabled = false;
                label5.Enabled = false;
            }
            else
            {
                label6.Enabled = true;
                numericUpDown1.Enabled = true;
                label5.Enabled = true;
            }
            groupBox1.Enabled = checkBox1.Checked;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.OctaveInNewLine = cb.SelectedIndex;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            settings.OctaveCommandCustom = tb.Text;
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.OctaveDirection = cb.SelectedIndex;
        }

        private void comboBox3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.LengthStyle = cb.SelectedIndex;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            settings.DotEnable = cb.Checked;
            groupBox1.Enabled = cb.Checked;
        }

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {
            var nud = (NumericUpDown)sender;
            settings.DotLength = nud.Value;
        }
    }
}
