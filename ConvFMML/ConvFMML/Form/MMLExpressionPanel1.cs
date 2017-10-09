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
    public partial class MMLExpressionPanel1 : BasePanel
    {
        private Settings.MMLExpression settings;
        public event EventHandler TimeBaseFormButtonClick;
        public event EventHandler MMLStyleChanged;

        public MMLExpressionPanel1(Settings.MMLExpression settings)
        {
            InitializeComponent();

            this.settings = settings;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            comboBox1.SelectedIndex = (int)settings.MMLStyle;
            UpdateComboBox2();

            textBox1.Text = settings.TimeBase.ToString();
            comboBox3.SelectedIndex = settings.NewBlockByBar;
            numericUpDown2.Value = settings.NewLineBarCount;
            comboBox4.SelectedIndex = settings.NewLineByTimeSignature;
        }

        public override void UpdateSelections(MMLStyle mmlStyle) { }

        public override void LoadMusicData(List<NotesStatus> statusList)
        {
            textBox1.Text = settings.TimeBase.ToString();
        }

        private void UpdateComboBox2()
        {
            comboBox2.Items.Clear();
            if (settings.MMLStyle == MMLStyle.PMD)
            {
                comboBox2.Items.AddRange(new string[] { "無効", "#Zenlenコマンドで出力", "Cコマンドで出力" });
                comboBox2.SelectedIndex = settings.PrintTimeBasePMD;
            }
            else
            {
                comboBox2.Items.AddRange(new string[] { "無効", "有効" });
                comboBox2.SelectedIndex = settings.PrintTimeBase;
            }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.MMLStyle = (MMLStyle)cb.SelectedIndex;
            UpdateComboBox2();
            MMLStyleChanged?.Invoke(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TimeBaseFormButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            if (settings.MMLStyle == MMLStyle.PMD)
            {
                settings.PrintTimeBasePMD = comboBox2.SelectedIndex;
            }
            else
            {
                settings.PrintTimeBase = comboBox2.SelectedIndex;
            }
        }

        private void comboBox3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.NewBlockByBar = cb.SelectedIndex;
        }

        private void numericUpDown2_Leave(object sender, EventArgs e)
        {
            var nud = (NumericUpDown)sender;
            settings.NewLineBarCount = nud.Value;
        }

        private void comboBox4_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.NewLineByTimeSignature = cb.SelectedIndex;
        }
    }
}
