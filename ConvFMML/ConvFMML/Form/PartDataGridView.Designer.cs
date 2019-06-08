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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PartDataGridView));
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
            resources.ApplyResources(this.dgv, "dgv");
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PrintableCheckColumn,
            this.PartNameColumn,
            this.MIDITrackNumberColumn,
            this.MIDITrackNameColumn,
            this.IsEnptyColumn,
            this.SoundModuleColumn,
            this.SpaceColumn});
            this.dgv.GridColor = System.Drawing.SystemColors.Control;
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowTemplate.Height = 18;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.ShowCellToolTips = false;
            this.dgv.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellEnter);
            this.dgv.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgv_DataError);
            // 
            // PrintableCheckColumn
            // 
            resources.ApplyResources(this.PrintableCheckColumn, "PrintableCheckColumn");
            this.PrintableCheckColumn.Name = "PrintableCheckColumn";
            // 
            // PartNameColumn
            // 
            resources.ApplyResources(this.PartNameColumn, "PartNameColumn");
            this.PartNameColumn.Name = "PartNameColumn";
            this.PartNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MIDITrackNumberColumn
            // 
            resources.ApplyResources(this.MIDITrackNumberColumn, "MIDITrackNumberColumn");
            this.MIDITrackNumberColumn.Name = "MIDITrackNumberColumn";
            this.MIDITrackNumberColumn.ReadOnly = true;
            this.MIDITrackNumberColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MIDITrackNameColumn
            // 
            resources.ApplyResources(this.MIDITrackNameColumn, "MIDITrackNameColumn");
            this.MIDITrackNameColumn.Name = "MIDITrackNameColumn";
            this.MIDITrackNameColumn.ReadOnly = true;
            this.MIDITrackNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // IsEnptyColumn
            // 
            resources.ApplyResources(this.IsEnptyColumn, "IsEnptyColumn");
            this.IsEnptyColumn.Name = "IsEnptyColumn";
            this.IsEnptyColumn.ReadOnly = true;
            // 
            // SoundModuleColumn
            // 
            this.SoundModuleColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.SoundModuleColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.SoundModuleColumn, "SoundModuleColumn");
            this.SoundModuleColumn.Name = "SoundModuleColumn";
            this.SoundModuleColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // SpaceColumn
            // 
            this.SpaceColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.SpaceColumn.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.SpaceColumn, "SpaceColumn");
            this.SpaceColumn.Name = "SpaceColumn";
            this.SpaceColumn.ReadOnly = true;
            this.SpaceColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // PartDataGridView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgv);
            this.Name = "PartDataGridView";
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
