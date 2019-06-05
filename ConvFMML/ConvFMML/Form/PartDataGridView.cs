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
    public partial class PartDataGridView : UserControl
    {
        public List<NotesStatus> DataSource { get; set; }
        public MMLStyle MMLStyle { get; set; }
        public Settings.OutputPart Settings { get; set; }

        public PartDataGridView()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            dgv.CurrentCellDirtyStateChanged += new EventHandler(dgv_CurrentCellDirtyStateChanged);
            dgv.CellValueChanged += new DataGridViewCellEventHandler(dgv_CellValueChanged);
        }

        public void ShowData()
        {
            dgv.Rows.Clear();

            if (DataSource == null) throw new NullReferenceException();

            if (MMLStyle == MMLStyle.Custom && Settings.PrintStyleCustom == 0)        // Don't print part name
            {
                PartNameColumn.ReadOnly = true;
                DataSource.AsEnumerable().Select(x => x.Name = String.Empty).ToList();
                SoundModuleColumn.ReadOnly = false;
            }
            else
            {
                switch (Settings.PrintStyle)
                {
                    case 0:     // Don't print
                        PartNameColumn.ReadOnly = true;
                        DataSource.AsEnumerable().Select(x => x.Name = String.Empty).ToList();
                        SoundModuleColumn.ReadOnly = false;
                        break;
                    case 1:     // Custom
                        PartNameColumn.ReadOnly = false;
                        SoundModuleColumn.ReadOnly = false;
                        break;
                    case 2:     // Auto
                        PartNameColumn.ReadOnly = true;
                        DataSource.AsEnumerable().Select(x => x.Name = String.Empty).ToList();
                        SoundModuleColumn.ReadOnly = (MMLStyle != MMLStyle.FMP7);
                        break;
                }
            }

            if (Settings.RemoveEmptyParts)
            {
                DataSource.AsEnumerable().Where(x => x.IsEmpty).Select(x => x.Printable = false).ToList();
            }

            AddRows();
        }

        private void AddRows()
        {
            var rows = new List<DataGridViewRow>();

            foreach (NotesStatus ns in DataSource)
            {
                if (!Settings.RemoveEmptyParts || !ns.IsEmpty)
                {
                    var row = new DataGridViewRow();
                    row.CreateCells(dgv);
                    row.Height = 18;
                    row.DefaultCellStyle.BackColor = (ns.Printable ? Color.White : SystemColors.ControlLight);
                    row.Tag = ns;

                    row.Cells[0].Value = ns.Printable;
                    row.Cells[1].Value = ns.Name;
                    row.Cells[2].Value = ns.TrackNumber;
                    row.Cells[3].Value = ns.TrackName;
                    row.Cells[4].Value = (ns.IsEmpty ? "None" : "Exisits");

                    var cbc = (DataGridViewComboBoxCell)row.Cells[5];
                    switch (MMLStyle)
                    {
                        case MMLStyle.FMP7:
                            cbc.Items.AddRange(new[] {
                            new { Display = "FM", Value = SoundModule.FM },
                            new { Display = "SSG", Value = SoundModule.SSG }
                        });
                            break;
                        case MMLStyle.FMP:
                            cbc.Items.AddRange(new[] {
                            new { Display = "FM", Value = SoundModule.FM },
                            new { Display = "SSG", Value = SoundModule.SSG },
                            new { Display = "FM3ch", Value = SoundModule.FM3ch }
                        });
                            break;
                        case MMLStyle.PMD:
                            cbc.Items.AddRange(new[] {
                            new { Display = "FM", Value = SoundModule.FM },
                            new { Display = "SSG", Value = SoundModule.SSG },
                            new { Display = "FM3ch", Value = SoundModule.FM3ch }
                        });
                            break;
                        case MMLStyle.MXDRV:
                            cbc.Items.AddRange(new[] {
                            new { Display = "FM", Value = SoundModule.FM },
                        });
                            break;
                        case MMLStyle.NRTDRV:
                            cbc.Items.AddRange(new[] {
                            new { Display = "FM", Value = SoundModule.FM },
                            new { Display = "SSG", Value = SoundModule.SSG }
                        });
                            break;
                        case MMLStyle.MUCOM88:
                            cbc.Items.AddRange(new[] {
                            new { Display = "FM", Value = SoundModule.FM },
                            new { Display = "SSG", Value = SoundModule.SSG }
                        });
                            break;
                        default:
                            cbc.Items.AddRange(new[] {
                            new { Display = "FM", Value = SoundModule.FM },
                            new { Display = "SSG", Value = SoundModule.SSG },
                            new { Display = "FM3ch", Value = SoundModule.FM3ch },
                            new { Display = "Others", Value = SoundModule.Others }
                        });
                            break;
                    }
                    cbc.DisplayMember = "Display";
                    cbc.ValueMember = "Value";
                    cbc.Value = ns.SoundModule;

                    rows.Add(row);
                }
            }

            dgv.Rows.AddRange(rows.ToArray());
            dgv.CurrentCell = null;

            if (MMLStyle != MMLStyle.Custom && Settings.PrintStyle == 2)      // Auto-set part name and sound module
            {
                AutoModifyPart();
            }
        }

        private void AutoModifyPart()
        {
            int n = 0;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                var status = (NotesStatus)row.Tag;

                if (!status.Printable)
                {
                    row.Cells[1].Value = String.Empty;
                    status.Name = String.Empty;
                    continue;
                }

                string name;
                SoundModule module;
                Color backColor;

                switch (MMLStyle)
                {
                    case MMLStyle.FMP7:
                        switch (Settings.AutoNameFMP7)
                        {
                            case 0:
                                if (n < 26)
                                {
                                    name = "\'" + Convert.ToChar(0x41 + n);
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            case 1:
                                if (n < 260)
                                {
                                    name = "\'" + Convert.ToChar(0x41 + n % 26) + (n / 26);
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            case 2:
                                if (n < 260)
                                {
                                    name = "\'" + Convert.ToChar(0x41 + n / 10) + (n % 10);
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            default:
                                name = String.Empty;
                                backColor = Color.LavenderBlush;
                                break;
                        }
                        module = SoundModule.FM;
                        break;

                    case MMLStyle.FMP:
                        switch (Settings.AutoNameFMP)
                        {
                            case 0:
                                if (n < 3)
                                {
                                    name = "\'" + Convert.ToChar(0x41 + n);
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else if (n < 6)
                                {
                                    name = "\'" + Convert.ToChar(0x41 + n);
                                    module = SoundModule.SSG;
                                    backColor = Color.White;
                                }
                                else if (n < 9)
                                {
                                    name = "\'" + Convert.ToChar(0x58 + n - 6);
                                    module = SoundModule.FM3ch;
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    module = SoundModule.FM;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            case 1:
                                if (n < 3)
                                {
                                    name = "\'" + Convert.ToChar(0x41 + n);
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else if (n < 6)
                                {
                                    name = "\'" + Convert.ToChar(0x41 + n);
                                    module = SoundModule.SSG;
                                    backColor = Color.White;
                                }
                                else if (n < 9)
                                {
                                    name = "\'" + Convert.ToChar(0x41 + n);
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else if (n < 12)
                                {
                                    name = "\'" + Convert.ToChar(0x58 + n - 9);
                                    module = SoundModule.FM3ch;
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    module = SoundModule.FM;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            default:
                                name = String.Empty;
                                module = SoundModule.FM;
                                backColor = Color.LavenderBlush;
                                break;
                        }
                        break;

                    case MMLStyle.PMD:
                        switch (Settings.AutoNamePMD)
                        {
                            case 0:
                                if (n < 3)
                                {
                                    name = Convert.ToChar(0x41 + n).ToString();
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else if (n < 6)
                                {
                                    name = Convert.ToChar(0x47 + n - 3).ToString();
                                    module = SoundModule.SSG;
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    module = SoundModule.FM;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            case 1:
                                if (n < 3)
                                {
                                    name = Convert.ToChar(0x41 + n).ToString();
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else if (n < 6)
                                {
                                    name = Convert.ToChar(0x41 + n).ToString();
                                    module = SoundModule.FM3ch;
                                    backColor = Color.White;
                                }
                                else if (n < 9)
                                {
                                    name = Convert.ToChar(0x41 + n).ToString();
                                    module = SoundModule.SSG;
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    module = SoundModule.FM;
                                    backColor = Color.LavenderBlush;
                                }
                                break;

                            case 2:
                                if (n < 6)
                                {
                                    name = Convert.ToChar(0x41 + n).ToString();
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else if (n < 9)
                                {
                                    name = Convert.ToChar(0x41 + n).ToString();
                                    module = SoundModule.SSG;
                                    backColor = Color.White;
                                }
                                else if (n < 12)
                                {
                                    name = Convert.ToChar(0x58 + n - 9).ToString();
                                    module = SoundModule.FM3ch;
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    module = SoundModule.FM;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            case 3:
                                if (n < 6)
                                {
                                    name = Convert.ToChar(0x41 + n).ToString();
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    module = SoundModule.FM;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            case 4:
                                if (n < 8)
                                {
                                    name = Convert.ToChar(0x41 + n).ToString();
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    module = SoundModule.FM;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            case 5:
                                if (n < 9)
                                {
                                    name = Convert.ToChar(0x41 + n).ToString();
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    module = SoundModule.FM;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            default:
                                name = String.Empty;
                                module = SoundModule.FM;
                                backColor = Color.LavenderBlush;
                                break;
                        }
                        break;

                    case MMLStyle.MXDRV:
                        if (n < 8)
                        {
                            name = Convert.ToChar(0x41 + n).ToString();
                            module = SoundModule.FM;
                            backColor = Color.White;
                        }
                        else
                        {
                            name = String.Empty;
                            module = SoundModule.FM;
                            backColor = Color.LavenderBlush;
                        }
                        break;

                    case MMLStyle.NRTDRV:
                        switch (Settings.AutoNameNRTDRV)
                        {
                            case 0:
                                if (n < 3)
                                {
                                    name = (n + 1).ToString();
                                    module = SoundModule.SSG;
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    module = SoundModule.FM;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            case 1:
                                if (n < 16)
                                {
                                    name = Convert.ToChar(0x41 + n).ToString();
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else if (n < 19)
                                {
                                    name = (n - 15).ToString();
                                    module = SoundModule.SSG;
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    module = SoundModule.FM;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            case 2:
                                if (n < 8)
                                {
                                    name = Convert.ToChar(0x41 + n).ToString();
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else if (n < 11)
                                {
                                    name = (n - 7).ToString();
                                    module = SoundModule.SSG;
                                    backColor = Color.White;
                                }
                                else if (n < 19)
                                {
                                    name = Convert.ToChar(0x49 + n - 11).ToString();
                                    module = SoundModule.FM;
                                    backColor = Color.White;
                                }
                                else
                                {
                                    name = String.Empty;
                                    module = SoundModule.FM;
                                    backColor = Color.LavenderBlush;
                                }
                                break;
                            default:
                                name = String.Empty;
                                module = SoundModule.FM;
                                backColor = Color.LavenderBlush;
                                break;
                        }
                        break;

                    case MMLStyle.MUCOM88:
                        if (n < 3)
                        {
                            name = Convert.ToChar(0x41 + n).ToString();
                            module = SoundModule.FM;
                            backColor = Color.White;
                        }
                        else if (n < 6)
                        {
                            name = Convert.ToChar(0x41 + n).ToString();
                            module = SoundModule.SSG;
                            backColor = Color.White;
                        }
                        else if (n < 9)
                        {
                            name = Convert.ToChar(0x42 + n).ToString();
                            module = SoundModule.FM;
                            backColor = Color.White;
                        }
                        else
                        {
                            name = String.Empty;
                            module = SoundModule.FM;
                            backColor = Color.LavenderBlush;
                        }
                        break;

                    default:
                        name = String.Empty;
                        module = SoundModule.FM;
                        backColor = Color.LavenderBlush;
                        break;
                }
                row.Cells[1].Value = name;
                status.Name = name;
                row.Cells[5].Value = module;
                status.SoundModule = module;
                row.DefaultCellStyle.BackColor = backColor;

                n++;
            }
        }

        private void dgv_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = (DataGridView)sender;
            DataGridViewColumn column = dgv.Columns[e.ColumnIndex];
            if (column is DataGridViewTextBoxColumn && column == PartNameColumn)
            {
                SendKeys.SendWait("{F2}");
            }
            else if (column is DataGridViewComboBoxColumn && column == SoundModuleColumn)
            {
                SendKeys.SendWait("{F4}");
            }
        }

        private void dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var dgv = (DataGridView)sender;
            if (dgv.IsCurrentCellDirty)
            {
                switch (dgv.CurrentCellAddress.X)
                {
                    case 0:     // Checkbox
                    case 1:     // PartName
                    case 5:     // SoundModule
                        dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
                        break;
                    default:
                        break;
                }
            }
        }

        private void dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = ((DataGridView)sender).Rows[e.RowIndex];
            var status = (NotesStatus)row.Tag;

            switch (e.ColumnIndex)
            {
                case 0:     // CheckBox
                    if (row.Cells[e.ColumnIndex].EditedFormattedValue.Equals(true))
                    {
                        status.Printable = true;
                        row.DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        status.Printable = false;
                        row.DefaultCellStyle.BackColor = SystemColors.ControlLight;
                    }
                    if (MMLStyle != MMLStyle.Custom && Settings.PrintStyle == 2)      // Auto-set part name and sound module
                    {
                        AutoModifyPart();
                    }
                    break;
                case 1:     // PartName
                    string name = row.Cells[e.ColumnIndex].Value?.ToString();
                    status.Name = (name ?? String.Empty);
                    break;
                case 5:     // Soundmodule
                    status.SoundModule = (SoundModule)row.Cells[e.ColumnIndex].Value;
                    break;
                default:
                    break;
            }
        }

        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var dgv = (DataGridView)sender;
            var status = (NotesStatus)dgv.Rows[e.RowIndex].Tag;

            if (e.ColumnIndex == 5)     // SoundModule
            {
                dgv[e.ColumnIndex, e.RowIndex].Value = SoundModule.FM;
                status.SoundModule = SoundModule.FM;
            }
        }
    }
}
