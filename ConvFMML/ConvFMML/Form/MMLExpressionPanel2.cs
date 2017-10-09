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
    public partial class MMLExpressionPanel2 : BasePanel
    {
        private Settings.MMLExpression settings;

        public MMLExpressionPanel2(Settings.MMLExpression settings)
        {
            InitializeComponent();

            this.settings = settings;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            comboBox1.SelectedIndex = settings.TitleEnable;
            UpdatePanel2(settings.MMLStyle);
        }

        public override void UpdateSelections(MMLStyle mmlStyle)
        {
            UpdatePanel2(mmlStyle);
        }

        private void UpdatePanel2(MMLStyle mmlStyle)
        {
            switch (mmlStyle)
            {
                case MMLStyle.Custom:
                case MMLStyle.FMP:
                    panel2.Enabled = false;
                    break;
                default:
                    panel2.Enabled = true;
                    break;
            }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.TitleEnable = cb.SelectedIndex;
        }
    }
}
