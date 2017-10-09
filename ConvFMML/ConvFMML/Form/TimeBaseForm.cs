using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvFMML.Form
{
    public partial class TimeBaseForm : System.Windows.Forms.Form
    {
        public decimal Timebase
        {
            get
            {
                return numericUpDown1.Value;
            }
        }

        public TimeBaseForm(decimal timebase)
        {
            InitializeComponent();

            numericUpDown1.Value = timebase;
        }

        private void settingButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
