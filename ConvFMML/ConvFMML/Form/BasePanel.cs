using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvFMML.Form
{
    public partial class BasePanel : UserControl
    {
        public BasePanel()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var canvas = new Bitmap(titleLabel.Width, titleLabel.Height);
            using (Graphics g = Graphics.FromImage(canvas))
            using (LinearGradientBrush lgb = new LinearGradientBrush(g.VisibleClipBounds, Color.Orange, SystemColors.Control, LinearGradientMode.Horizontal))
            {
                g.FillRectangle(lgb, g.VisibleClipBounds);
            }
            titleLabel.Image = canvas;
        }

        public virtual void UpdateSelections(MMLStyle mmlStyle) { }

        public virtual void LoadMusicData(List<Data.Intermediate.NotesStatus> statusList) { }
    }
}
