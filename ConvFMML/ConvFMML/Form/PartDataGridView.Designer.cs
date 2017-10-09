namespace ConvFMML.Form
{
    partial class PartDataGridView
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.PrintableCheckColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.PartNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MIDITrackNumberColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MIDITrackNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsEnptyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoundModuleColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.SpaceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.ColumnHeadersHeight = 20;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PrintableCheckColumn,
            this.PartNameColumn,
            this.MIDITrackNumberColumn,
            this.MIDITrackNameColumn,
            this.IsEnptyColumn,
            this.SoundModuleColumn,
            this.SpaceColumn});
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.GridColor = System.Drawing.SystemColors.Control;
            this.dgv.Location = new System.Drawing.Point(0, 0);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowTemplate.Height = 18;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.ShowCellToolTips = false;
            this.dgv.Size = new System.Drawing.Size(494, 112);
            this.dgv.TabIndex = 16;
            this.dgv.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellEnter);
            this.dgv.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgv_DataError);
            // 
            // PrintableCheckColumn
            // 
            this.PrintableCheckColumn.HeaderText = "";
            this.PrintableCheckColumn.Name = "PrintableCheckColumn";
            this.PrintableCheckColumn.Width = 20;
            // 
            // PartNameColumn
            // 
            this.PartNameColumn.HeaderText = "パート名";
            this.PartNameColumn.Name = "PartNameColumn";
            this.PartNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PartNameColumn.Width = 58;
            // 
            // MIDITrackNumberColumn
            // 
            this.MIDITrackNumberColumn.HeaderText = "MIDIトラックNo.";
            this.MIDITrackNumberColumn.Name = "MIDITrackNumberColumn";
            this.MIDITrackNumberColumn.ReadOnly = true;
            this.MIDITrackNumberColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MIDITrackNumberColumn.Width = 90;
            // 
            // MIDITrackNameColumn
            // 
            this.MIDITrackNameColumn.HeaderText = "MIDIトラック名";
            this.MIDITrackNameColumn.Name = "MIDITrackNameColumn";
            this.MIDITrackNameColumn.ReadOnly = true;
            this.MIDITrackNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MIDITrackNameColumn.Width = 160;
            // 
            // IsEnptyColumn
            // 
            this.IsEnptyColumn.HeaderText = "音符の有無";
            this.IsEnptyColumn.Name = "IsEnptyColumn";
            this.IsEnptyColumn.ReadOnly = true;
            this.IsEnptyColumn.Width = 72;
            // 
            // SoundModuleColumn
            // 
            this.SoundModuleColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.SoundModuleColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SoundModuleColumn.HeaderText = "音源";
            this.SoundModuleColumn.Name = "SoundModuleColumn";
            this.SoundModuleColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SoundModuleColumn.Width = 64;
            // 
            // SpaceColumn
            // 
            this.SpaceColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.SpaceColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.SpaceColumn.HeaderText = "";
            this.SpaceColumn.Name = "SpaceColumn";
            this.SpaceColumn.ReadOnly = true;
            this.SpaceColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // PartDataGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgv);
            this.Name = "PartDataGridView";
            this.Size = new System.Drawing.Size(494, 112);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.DataGridViewCheckBoxColumn PrintableCheckColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PartNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MIDITrackNumberColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MIDITrackNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsEnptyColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn SoundModuleColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SpaceColumn;
    }
}
