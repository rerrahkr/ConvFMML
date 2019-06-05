using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConvFMML.Data.Intermediate;

namespace ConvFMML.Form
{
    public partial class OutputPartPanel : BasePanel
    {
        private Settings.OutputPart settings;
        private MMLStyle mmlStyle;

        public OutputPartPanel(Settings.OutputPart settings, MMLStyle mmlStyle)
        {
            InitializeComponent();

            this.settings = settings;
            this.mmlStyle = mmlStyle;

            partDataGridView1.Settings = settings;
            partDataGridView1.MMLStyle = mmlStyle;
        }

        public override void UpdateSelections(MMLStyle mmlStyle)
        {
            this.mmlStyle = mmlStyle;

            comboBox1.Items.Clear();
            if (this.mmlStyle == MMLStyle.Custom)
            {
                comboBox1.Items.AddRange(new string[] { "出力しない", "それぞれ指定して出力" });
                comboBox1.SelectedIndex = settings.PrintStyleCustom;
            }
            else
            {
                comboBox1.Items.AddRange(new string[] { "出力しない", "それぞれ指定して出力", "自動で割り当てて出力" });
                comboBox1.SelectedIndex = settings.PrintStyle;
            }

            if (comboBox1.SelectedIndex == 2)
            {
                if (this.mmlStyle == MMLStyle.MXDRV || this.mmlStyle == MMLStyle.MUCOM88)
                {
                    label2.Enabled = false;
                    comboBox2.Enabled = false;
                }
                else
                {
                    label2.Enabled = true;
                    comboBox2.Enabled = true;
                }
            }
            else
            {
                label2.Enabled = false;
                comboBox2.Enabled = false;
            }

            comboBox2.Items.Clear();
            switch (this.mmlStyle)
            {
                case MMLStyle.FMP7:
                    comboBox2.Items.AddRange(new string[]
                    {
                        "ABC...XYZ",
                        "A0B0C0...X0Y0Z0A1B1C1...",
                        "A0A1A2...A7A8A9B0B1B2..."
                    });
                    comboBox2.SelectedIndex = settings.AutoNameFMP7;
                    break;
                case MMLStyle.FMP:
                    comboBox2.Items.AddRange(new string[]
                    {
                        "FM: A～C | SSG: D～F | FM3ch: X～Z",
                        "FM: A～C | SSG: D～F | FM: G～I | FM3ch: X～Z"
                    });
                    comboBox2.SelectedIndex = settings.AutoNameFMP;
                    break;
                case MMLStyle.PMD:
                    comboBox2.Items.AddRange(new string[]
                    {
                        "FM: A～C | SSG: G～I",
                        "FM: A～C | FM3ch: D～F | SSG: G～I",
                        "FM: A～F | SSG: G～I | FM3ch: X～Z",
                        "FM: A～F",
                        "FM: A～H",
                        "FM: A～I"
                    });
                    comboBox2.SelectedIndex = settings.AutoNamePMD;
                    break;
                case MMLStyle.NRTDRV:
                    comboBox2.Items.AddRange(new string[]
                    {
                        "SSG: 1～3",
                        "FM: A～P | SSG:  1～3",
                        "FM: A～H | SSG:  1～3 | FM: I～P"
                    });
                    comboBox2.SelectedIndex = settings.AutoNameNRTDRV;
                    break;
                default:
                    break;
            }

            checkBox1.Checked = settings.RemoveEmptyParts;

            ChangeOutputPartMMLSyle(mmlStyle);
        }

        public override void LoadMusicData(List<NotesStatus> notesStatusList)
        {
            partDataGridView1.DataSource = notesStatusList;
            partDataGridView1.ShowData();
        }

        public void ChangeOutputPartMMLSyle(MMLStyle style)
        {
            partDataGridView1.MMLStyle = style;
            partDataGridView1.ShowData();
        }

        public List<NotesStatus> GetOutputPartSettings()
        {
            return partDataGridView1.DataSource;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            if (mmlStyle == MMLStyle.Custom)
            {
                settings.PrintStyleCustom = cb.SelectedIndex;
            }
            else
            {
                settings.PrintStyle = cb.SelectedIndex;
            }

            if (cb.SelectedIndex == 0)
            {
                label2.Enabled = false;
                comboBox2.Enabled = false;
            }
            else if (cb.SelectedIndex == 1)
            {
                label2.Enabled = false;
                comboBox2.Enabled = false;
            }
            else
            {
                if (mmlStyle == MMLStyle.MXDRV || mmlStyle == MMLStyle.MUCOM88)
                {
                    label2.Enabled = false;
                    comboBox2.Enabled = false;
                }
                else
                {
                    label2.Enabled = true;
                    comboBox2.Enabled = true;
                }
            }

            partDataGridView1.ShowData();
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            switch (mmlStyle)
            {
                case MMLStyle.FMP7:
                    settings.AutoNameFMP7 = cb.SelectedIndex;
                    break;
                case MMLStyle.FMP:
                    settings.AutoNameFMP = cb.SelectedIndex;
                    break;
                case MMLStyle.PMD:
                    settings.AutoNamePMD = cb.SelectedIndex;
                    break;
                case MMLStyle.NRTDRV:
                    settings.AutoNameNRTDRV = cb.SelectedIndex;
                    break;
                default:
                    break;
            }

            partDataGridView1.ShowData();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox)sender;
            settings.RemoveEmptyParts = cb.Checked;
            partDataGridView1.ShowData();
        }
    }
}
