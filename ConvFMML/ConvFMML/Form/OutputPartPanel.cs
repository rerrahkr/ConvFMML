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
using ConvFMML.Properties;

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
                comboBox1.Items.AddRange(new string[] { Resources.PartDisabled, Resources.PartCustom });
                comboBox1.SelectedIndex = settings.PrintStyleCustom;
            }
            else
            {
                comboBox1.Items.AddRange(new string[] { Resources.PartDisabled, Resources.PartCustom, Resources.PartAuto });
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
                case MMLStyle.Mml2vgm:
                    comboBox2.Items.AddRange(new string[]
                    {
                        "FM: F1～F6   | SSG: S1～S4",
                        "FM: F1～F6   | FM3ch: F7～F9   | SSG: S1～S4",
                        "FM: E01～E06 | SSG: S1～S4",
                        "FM: E01～E06 | FM3ch: E07～E09 | SSG: S1～S4",
                        "SSG: S1～S4",
                        "FM: T01～T06 | SSG: T10～T12",
                        "FM: T01～T06 | FM3ch: T07～T09 | SSG: T10～T12",
                        "SSG: T10～T12",
                        "FM: P01～P06 | SSG: P10～P12",
                        "FM: P01～T06 | FM3ch: P07～P09 | SSG: P10～P12",
                        "SSG: P10～P12",
                        "FM: N1～N3   | SSG: N7～N9",
                        "FM: N1～N3   | FM3ch: N4～N6   | SSG: N7～N9",
                        "SSG: N7～N9",
                        "FM: X1～X8",
                        "SSG: A1～A3",
                        "FM: L01～L09"
                    });
                    comboBox2.SelectedIndex = settings.AutoNameMml2vgm;
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
                case MMLStyle.Mml2vgm:
                    settings.AutoNameMml2vgm = cb.SelectedIndex;
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
