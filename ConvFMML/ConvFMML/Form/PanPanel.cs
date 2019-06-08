using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConvFMML.Properties;

namespace ConvFMML.Form
{
    public partial class PanPanel : BasePanel
    {
        private Settings.ControlCommand.Pan settings;
        private MMLStyle mmlStyle;

        public PanPanel(Settings.ControlCommand.Pan settings, MMLStyle mmlStyle)
        {
            InitializeComponent();

            this.settings = settings;
            this.mmlStyle = mmlStyle;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            checkBox1.Checked = settings.Enable;
            // Load FMP and Custom Command in UpdateSelections
            textBox1.Text = settings.MIDICommandCustom;
            textBox2.Text = settings.LeftCommandCustom;
            textBox3.Text = settings.CenterCommandCustom;
            textBox4.Text = settings.RightCommandCustom;
            checkBox2.Checked = settings.BorderUsingNegative;

            if (checkBox2.Checked)
            {
                numericUpDown1.Minimum = -64;
                numericUpDown1.Maximum = settings.BorderRight - 65;
                numericUpDown2.Minimum = settings.BorderLeft - 63;
                numericUpDown2.Maximum = 63;
                numericUpDown1.Value = settings.BorderLeft - 64;
                numericUpDown2.Value = settings.BorderRight - 64;
            }
            else
            {
                numericUpDown1.Minimum = 0;
                numericUpDown1.Maximum = settings.BorderRight - 1;
                numericUpDown2.Minimum = settings.BorderLeft + 1;
                numericUpDown2.Maximum = 127;
                numericUpDown1.Value = settings.BorderLeft;
                numericUpDown2.Value = settings.BorderRight;
            }

            checkBox2.CheckedChanged += new EventHandler(checkBox2_CheckedChanged);
            numericUpDown1.ValueChanged += new EventHandler(numericUpDown1_ValueChanged);
            numericUpDown2.ValueChanged += new EventHandler(numericUpDown2_ValueChanged);
        }

        public override void UpdateSelections(MMLStyle mmlStyle)
        {
            this.mmlStyle = mmlStyle;

            panel2.Enabled = checkBox1.Checked;

            groupBox1.Enabled = false;
            comboBox1.Items.Clear();
            switch (mmlStyle)
            {
                case MMLStyle.FMP7:
                    comboBox1.Items.AddRange(new string[] { Resources.PanP, Resources.PanPLPCPR });
                    comboBox1.SelectedIndex = settings.CommandFMP7;
                    groupBox1.Enabled = true;
                    label2.Enabled = false;
                    textBox1.Enabled = false;
                    label3.Enabled = false;
                    textBox2.Enabled = false;
                    label4.Enabled = false;
                    textBox3.Enabled = false;
                    label5.Enabled = false;
                    textBox4.Enabled = false;
                    groupBox2.Enabled = false;
                    break;
                case MMLStyle.Custom:
                    comboBox1.Items.AddRange(new string[] { Resources.PanMIDI, Resources.PanLCR });
                    comboBox1.SelectedIndex = settings.CommandCustom;
                    groupBox1.Enabled = true;
                    if (comboBox1.SelectedIndex == 0)
                    {
                        label2.Enabled = true;
                        textBox1.Enabled = true;
                        label3.Enabled = false;
                        textBox2.Enabled = false;
                        label4.Enabled = false;
                        textBox3.Enabled = false;
                        label5.Enabled = false;
                        textBox4.Enabled = false;
                        groupBox2.Enabled = false;
                    }
                    else if (comboBox1.SelectedIndex == 1)
                    {
                        label2.Enabled = false;
                        textBox1.Enabled = false;
                        label3.Enabled = true;
                        textBox2.Enabled = true;
                        label4.Enabled = true;
                        textBox3.Enabled = true;
                        label5.Enabled = true;
                        textBox4.Enabled = true;
                        groupBox2.Enabled = true;
                    }
                    break;
                default:
                    groupBox2.Enabled = true;
                    break;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            settings.Enable = cb.Checked;

            switch (mmlStyle)
            {
                case MMLStyle.FMP7:
                    panel2.Enabled = cb.Checked;
                    if (panel2.Enabled)
                    {
                        groupBox1.Enabled = true;
                        label2.Enabled = false;
                        textBox1.Enabled = false;
                        label3.Enabled = false;
                        textBox2.Enabled = false;
                        label4.Enabled = false;
                        textBox3.Enabled = false;
                        label5.Enabled = false;
                        textBox4.Enabled = false;
                        groupBox2.Enabled = false;
                    }
                    break;
                case MMLStyle.Custom:
                    panel2.Enabled = cb.Checked;
                    if (panel2.Enabled)
                    {
                        groupBox1.Enabled = true;
                        if (comboBox1.SelectedIndex == 0)
                        {
                            label2.Enabled = true;
                            textBox1.Enabled = true;
                            label3.Enabled = false;
                            textBox2.Enabled = false;
                            label4.Enabled = false;
                            textBox3.Enabled = false;
                            label5.Enabled = false;
                            textBox4.Enabled = false;
                            groupBox2.Enabled = false;
                        }
                        else if (comboBox1.SelectedIndex == 1)
                        {
                            label2.Enabled = false;
                            textBox1.Enabled = false;
                            label3.Enabled = true;
                            textBox2.Enabled = true;
                            label4.Enabled = true;
                            textBox3.Enabled = true;
                            label5.Enabled = true;
                            textBox4.Enabled = true;
                            groupBox2.Enabled = true;
                        }
                    }
                    break;
                default:
                    panel2.Enabled = checkBox1.Checked;
                    groupBox1.Enabled = false;
                    break;
            }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            switch (mmlStyle)
            {
                case MMLStyle.FMP7:
                    settings.CommandFMP7 = cb.SelectedIndex;
                    break;
                case MMLStyle.Custom:
                    settings.CommandCustom = cb.SelectedIndex;
                    if (cb.SelectedIndex == 0)
                    {
                        label2.Enabled = true;
                        textBox1.Enabled = true;
                        label3.Enabled = false;
                        textBox2.Enabled = false;
                        label4.Enabled = false;
                        textBox3.Enabled = false;
                        label5.Enabled = false;
                        textBox4.Enabled = false;
                        groupBox2.Enabled = false;
                    }
                    else if (cb.SelectedIndex == 1)
                    {
                        label2.Enabled = false;
                        textBox1.Enabled = false;
                        label3.Enabled = true;
                        textBox2.Enabled = true;
                        label4.Enabled = true;
                        textBox3.Enabled = true;
                        label5.Enabled = true;
                        textBox4.Enabled = true;
                        groupBox2.Enabled = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            settings.MIDICommandCustom = tb.Text;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            settings.LeftCommandCustom = tb.Text;
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            settings.CenterCommandCustom = tb.Text;
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            settings.RightCommandCustom = tb.Text;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var nud = (NumericUpDown)sender;
            if (checkBox2.Checked)
            {
                settings.BorderLeft = nud.Value + 64;
            }
            else
            {
                settings.BorderLeft = nud.Value;
            }
            numericUpDown2.Minimum = nud.Value + 1;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            var nud = (NumericUpDown)sender;
            if (checkBox2.Checked)
            {
                settings.BorderRight = nud.Value + 64;
            }
            else
            {
                settings.BorderRight = nud.Value;
            }
            numericUpDown1.Maximum = nud.Value - 1;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            settings.BorderUsingNegative = cb.Checked;

            if (cb.Checked)
            {
                numericUpDown1.Minimum -= 64;
                numericUpDown1.Value -= 64;
                numericUpDown1.Maximum -= 64;
                numericUpDown2.Minimum -= 64;
                numericUpDown2.Value -= 64;
                numericUpDown2.Maximum -= 64;
            }
            else
            {
                numericUpDown2.Maximum += 64;
                numericUpDown2.Value += 64;
                numericUpDown2.Minimum += 64;
                numericUpDown1.Maximum += 64;
                numericUpDown1.Value += 64;
                numericUpDown1.Minimum += 64;
            }
        }
    }
}
