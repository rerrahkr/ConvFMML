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
    public partial class VolumePanel : BasePanel
    {
        private Settings.ControlCommand.Volume settings;
        private MMLStyle mmlStyle;

        public VolumePanel(Settings.ControlCommand.Volume settings, MMLStyle mmlStyle)
        {
            InitializeComponent();

            this.settings = settings;
            this.mmlStyle = mmlStyle;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            checkBox1.Checked = settings.Enable;
            textBox1.Text = settings.CommandCustom;
            // Load PMD, MXDRV and NRTDRV Command in UpdateSelections
            numericUpDown1.Value = settings.RangeCustom;
            numericUpDown2.Value = settings.VStep;
        }

        public override void UpdateSelections(MMLStyle mmlStyle)
        {
            this.mmlStyle = mmlStyle;

            panel2.Enabled = checkBox1.Checked;

            if (this.mmlStyle == MMLStyle.Custom)
            {
                comboBox1.Visible = false;
                textBox1.Visible = true;
                panel3.Enabled = true;
                panel4.Enabled = false;
            }
            else
            {
                comboBox1.Visible = true;
                textBox1.Visible = false;
                comboBox1.Items.Clear();
                switch (this.mmlStyle)
                {
                    case MMLStyle.FMP7:
                    case MMLStyle.FMP:
                    case MMLStyle.MUCOM88:
                    case MMLStyle.Mml2vgm:
                        panel2.Enabled = false;
                        break;
                    case MMLStyle.PMD:
                        comboBox1.Items.AddRange(new string[] { "v", "V" });
                        comboBox1.SelectedIndex = settings.CommandPMD;
                        panel3.Enabled = false;
                        panel4.Enabled = false;
                        break;
                    case MMLStyle.MXDRV:
                        comboBox1.Items.AddRange(new string[] { "v", "@v" });
                        comboBox1.SelectedIndex = settings.CommandMXDRV;
                        panel3.Enabled = false;
                        panel4.Enabled = false;
                        break;
                    case MMLStyle.NRTDRV:
                        comboBox1.Items.AddRange(new string[] { "v", "V" });
                        comboBox1.SelectedIndex = settings.CommandNRTDRV;
                        panel3.Enabled = false;
                        panel4.Enabled = (comboBox1.SelectedIndex == 0);
                        break;
                    default:
                        break;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            settings.Enable = cb.Checked;

            switch (mmlStyle)
            {
                case MMLStyle.FMP7:
                case MMLStyle.FMP:
                case MMLStyle.MUCOM88:
                case MMLStyle.Mml2vgm:
                    break;
                case MMLStyle.PMD:
                case MMLStyle.MXDRV:
                    panel2.Enabled = cb.Checked;
                    if (panel2.Enabled)
                    {
                        panel3.Enabled = false;
                        panel4.Enabled = false;
                    }
                    break;
                case MMLStyle.NRTDRV:
                    panel2.Enabled = cb.Checked;
                    if (panel2.Enabled)
                    {
                        panel3.Enabled = false;
                        panel4.Enabled = (comboBox1.SelectedIndex == 0);
                    }
                    break;
                case MMLStyle.Custom:
                    panel2.Enabled = cb.Checked;
                    if (panel2.Enabled)
                    {
                        panel3.Enabled = true;
                        panel4.Enabled = false;
                    }
                    break;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            settings.CommandCustom = tb.Text;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            switch (mmlStyle)
            {
                case MMLStyle.PMD:
                    settings.CommandPMD = cb.SelectedIndex;
                    return;
                case MMLStyle.MXDRV:
                    settings.CommandMXDRV = cb.SelectedIndex;
                    return;
                case MMLStyle.NRTDRV:
                    settings.CommandNRTDRV = cb.SelectedIndex;
                    panel4.Enabled = (cb.SelectedIndex == 0);
                    return;
                default:
                    return;
            }
        }

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {
            var nud = (NumericUpDown)sender;
            settings.RangeCustom = nud.Value;
        }

        private void numericUpDown2_Leave(object sender, EventArgs e)
        {
            var nud = (NumericUpDown)sender;
            settings.VStep = nud.Value;
        }
    }
}
