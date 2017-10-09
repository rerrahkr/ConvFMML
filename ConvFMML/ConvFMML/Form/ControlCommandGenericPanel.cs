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
    public partial class ControlCommandGenericPanel : BasePanel
    {
        private Settings.ControlCommand.Generic settings;

        public ControlCommandGenericPanel(Settings.ControlCommand.Generic settings)
        {
            InitializeComponent();

            this.settings = settings;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            comboBox1.SelectedIndex = settings.Invalid;
            comboBox2.SelectedIndex = settings.SamePosition;
            comboBox3.SelectedIndex = settings.Predeclared;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.Invalid = cb.SelectedIndex;
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.SamePosition = cb.SelectedIndex;
        }

        private void comboBox3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var cb = (ComboBox)sender;
            settings.Predeclared = cb.SelectedIndex;
        }
    }
}
