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
    public partial class NoteRestPanel2 : BasePanel
    {
        private Settings.NoteRest settings;
        private MMLStyle mmlStyle;

        public NoteRestPanel2(Settings.NoteRest settings, MMLStyle mmlStyle)
        {
            InitializeComponent();

            this.settings = settings;
            this.mmlStyle = mmlStyle;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Load CutByMeasure in UpdateSelections
            checkBox2.Checked = settings.NewBlockInCutted;
            // Load TieStyle in UpdateSelections
            checkBox3.Checked = settings.UnuseTiedRest;
        }

        public override void UpdateSelections(MMLStyle mmlStyle)
        {
            this.mmlStyle = mmlStyle;

            comboBox2.Items.Clear();
            switch (this.mmlStyle)
            {
                case MMLStyle.MXDRV:
                case MMLStyle.NRTDRV:
                case MMLStyle.MUCOM88:
                    comboBox2.Items.AddRange(new string[] { Resources.TieLengthStyleC4AndC16, Resources.TieLengthStyleC4Hat16 });
                    break;
                default:
                    comboBox2.Items.AddRange(new string[] { Resources.TieLengthStyleC4AndC16, Resources.TieLengthStyleC4And16 });
                    break;
            }
            comboBox2.SelectedIndex = settings.TieStyle;

            if (mmlStyle == MMLStyle.Custom)
            {
                label4.Enabled = true;
                textBox1.Enabled = true;
            }
            else
            {
                label4.Enabled = false;
                textBox1.Enabled = false;
            }

            switch (mmlStyle)
            {
                case MMLStyle.FMP:
                case MMLStyle.FMP7:
                case MMLStyle.Mml2vgm:
                    checkBox3.Enabled = false;
                    break;
                case MMLStyle.MUCOM88:
                    checkBox3.Enabled = (comboBox2.SelectedIndex != 0);
                    break;
                default:
                    checkBox3.Enabled = true;
                    break;
            }

            UpdateComboBox1(comboBox2);
            UpdateCheckBox2();
        }

        private void UpdateCheckBox2()
        {
            checkBox2.Enabled = (comboBox1.SelectedIndex == 1 && comboBox2.SelectedIndex == 0);
        }

        private void UpdateComboBox1(ComboBox comboBox2)
        {
            comboBox1.Items.Clear();

            string[] items;
            if (comboBox2.SelectedIndex == 0)
            {
                items = new string[] { Resources.OverMeasureC4, Resources.OverMeasureC8AndC8 };
            }
            else
            {
                switch (mmlStyle)
                {
                    case MMLStyle.MXDRV:
                    case MMLStyle.NRTDRV:
                        items = new string[] { Resources.OverMeasureC4, Resources.OverMeasureC8Hat8 };
                        break;
                    default:
                        items = new string[] { Resources.OverMeasureC4, Resources.OverMeasureC8And8 };
                        break;
                }
            }
            comboBox1.Items.AddRange(items);

            comboBox1.SelectedIndex = settings.CutByBar;
            UpdateCheckBox2();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.CutByBar = cb.SelectedIndex;
            UpdateCheckBox2();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            settings.NewBlockInCutted = cb.Checked;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            settings.TieCommandCustom = tb.Text;
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.TieStyle = cb.SelectedIndex;
            UpdateComboBox1(cb);

            if (mmlStyle == MMLStyle.MUCOM88)
            {
                checkBox3.Enabled = (cb.SelectedIndex != 0);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            settings.UnuseTiedRest = cb.Checked;
        }
    }
}
