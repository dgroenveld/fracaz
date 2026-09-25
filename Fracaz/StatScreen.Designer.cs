namespace Fracaz;

partial class StatScreen
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatScreen));
        OKbutt = new Button();
        Frame1 = new GroupBox();
        FGattacked = new DataGridView();
        FGovertaken = new DataGridView();
        Frame2 = new GroupBox();
        FGkilled = new DataGridView();
        Frame3 = new GroupBox();
        Frame5 = new GroupBox();
        FGdefeated = new DataGridView();
        Frame4 = new GroupBox();
        FGtotals = new DataGridView();
        Frame6 = new GroupBox();
        FGrankings = new DataGridView();
        Frame1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)FGattacked).BeginInit();
        ((System.ComponentModel.ISupportInitialize)FGovertaken).BeginInit();
        Frame2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)FGkilled).BeginInit();
        Frame3.SuspendLayout();
        Frame5.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)FGdefeated).BeginInit();
        Frame4.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)FGtotals).BeginInit();
        Frame6.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)FGrankings).BeginInit();
        SuspendLayout();
        // 
        // OKbutt
        // 
        OKbutt.Location = new Point(556, 408);
        OKbutt.Name = "OKbutt";
        OKbutt.Size = new Size(55, 23);
        OKbutt.TabIndex = 0;
        OKbutt.Text = "OK";
        OKbutt.UseVisualStyleBackColor = true;
        OKbutt.Click += OKbutt_Click;
        // 
        // Frame1
        // 
        Frame1.Controls.Add(FGattacked);
        Frame1.Location = new Point(2, 2);
        Frame1.Name = "Frame1";
        Frame1.Size = new Size(340, 143);
        Frame1.TabIndex = 1;
        Frame1.TabStop = false;
        Frame1.Text = "Countries Attacked";
        // 
        // FGattacked
        // 
        FGattacked.AllowUserToAddRows = false;
        FGattacked.AllowUserToDeleteRows = false;
        FGattacked.AllowUserToResizeColumns = false;
        FGattacked.AllowUserToResizeRows = false;
        FGattacked.ColumnHeadersHeight = 12;
        FGattacked.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        FGattacked.ColumnHeadersVisible = false;
        dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.TopCenter;
        dataGridViewCellStyle1.BackColor = SystemColors.Window;
        dataGridViewCellStyle1.Font = new Font("Segoe UI", 8F);
        dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
        FGattacked.DefaultCellStyle = dataGridViewCellStyle1;
        FGattacked.Location = new Point(10, 19);
        FGattacked.Name = "FGattacked";
        FGattacked.ReadOnly = true;
        FGattacked.RowHeadersVisible = false;
        FGattacked.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        FGattacked.RowTemplate.Height = 16;
        FGattacked.ScrollBars = ScrollBars.None;
        FGattacked.SelectionMode = DataGridViewSelectionMode.CellSelect;
        FGattacked.Size = new Size(323, 115);
        FGattacked.TabIndex = 0;
        // 
        // FGovertaken
        // 
        FGovertaken.AllowUserToAddRows = false;
        FGovertaken.AllowUserToDeleteRows = false;
        FGovertaken.AllowUserToResizeColumns = false;
        FGovertaken.AllowUserToResizeRows = false;
        FGovertaken.ColumnHeadersHeight = 12;
        FGovertaken.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        FGovertaken.ColumnHeadersVisible = false;
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.TopCenter;
        dataGridViewCellStyle2.BackColor = SystemColors.Window;
        dataGridViewCellStyle2.Font = new Font("Segoe UI", 8F);
        dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
        FGovertaken.DefaultCellStyle = dataGridViewCellStyle2;
        FGovertaken.Location = new Point(10, 19);
        FGovertaken.Name = "FGovertaken";
        FGovertaken.ReadOnly = true;
        FGovertaken.RowHeadersVisible = false;
        FGovertaken.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        FGovertaken.RowTemplate.Height = 16;
        FGovertaken.ScrollBars = ScrollBars.None;
        FGovertaken.SelectionMode = DataGridViewSelectionMode.CellSelect;
        FGovertaken.Size = new Size(323, 115);
        FGovertaken.TabIndex = 0;
        // 
        // Frame2
        // 
        Frame2.Controls.Add(FGovertaken);
        Frame2.Location = new Point(2, 145);
        Frame2.Name = "Frame2";
        Frame2.Size = new Size(340, 143);
        Frame2.TabIndex = 2;
        Frame2.TabStop = false;
        Frame2.Text = "Countries Overtaken";
        // 
        // FGkilled
        // 
        FGkilled.AllowUserToAddRows = false;
        FGkilled.AllowUserToDeleteRows = false;
        FGkilled.AllowUserToResizeColumns = false;
        FGkilled.AllowUserToResizeRows = false;
        FGkilled.ColumnHeadersHeight = 12;
        FGkilled.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        FGkilled.ColumnHeadersVisible = false;
        dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.TopCenter;
        dataGridViewCellStyle3.BackColor = SystemColors.Window;
        dataGridViewCellStyle3.Font = new Font("Segoe UI", 8F);
        dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
        FGkilled.DefaultCellStyle = dataGridViewCellStyle3;
        FGkilled.Location = new Point(10, 19);
        FGkilled.Name = "FGkilled";
        FGkilled.ReadOnly = true;
        FGkilled.RowHeadersVisible = false;
        FGkilled.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        FGkilled.RowTemplate.Height = 16;
        FGkilled.ScrollBars = ScrollBars.None;
        FGkilled.SelectionMode = DataGridViewSelectionMode.CellSelect;
        FGkilled.Size = new Size(323, 115);
        FGkilled.TabIndex = 0;
        // 
        // Frame3
        // 
        Frame3.Controls.Add(FGkilled);
        Frame3.Location = new Point(2, 288);
        Frame3.Name = "Frame3";
        Frame3.Size = new Size(340, 143);
        Frame3.TabIndex = 3;
        Frame3.TabStop = false;
        Frame3.Text = "Troops Killed";
        // 
        // Frame5
        // 
        Frame5.Controls.Add(FGdefeated);
        Frame5.Location = new Point(348, 2);
        Frame5.Name = "Frame5";
        Frame5.Size = new Size(202, 143);
        Frame5.TabIndex = 2;
        Frame5.TabStop = false;
        Frame5.Text = "HQs Defeated";
        // 
        // FGdefeated
        // 
        FGdefeated.AllowUserToAddRows = false;
        FGdefeated.AllowUserToDeleteRows = false;
        FGdefeated.AllowUserToResizeColumns = false;
        FGdefeated.AllowUserToResizeRows = false;
        FGdefeated.ColumnHeadersHeight = 12;
        FGdefeated.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        FGdefeated.ColumnHeadersVisible = false;
        dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.TopCenter;
        dataGridViewCellStyle4.BackColor = SystemColors.Window;
        dataGridViewCellStyle4.Font = new Font("Segoe UI", 8F);
        dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
        FGdefeated.DefaultCellStyle = dataGridViewCellStyle4;
        FGdefeated.Location = new Point(10, 19);
        FGdefeated.Name = "FGdefeated";
        FGdefeated.ReadOnly = true;
        FGdefeated.RowHeadersVisible = false;
        FGdefeated.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        FGdefeated.RowTemplate.Height = 16;
        FGdefeated.ScrollBars = ScrollBars.None;
        FGdefeated.SelectionMode = DataGridViewSelectionMode.CellSelect;
        FGdefeated.Size = new Size(186, 115);
        FGdefeated.TabIndex = 0;
        // 
        // Frame4
        // 
        Frame4.Controls.Add(FGtotals);
        Frame4.Location = new Point(348, 145);
        Frame4.Name = "Frame4";
        Frame4.Size = new Size(202, 143);
        Frame4.TabIndex = 3;
        Frame4.TabStop = false;
        Frame4.Text = "Totals";
        // 
        // FGtotals
        // 
        FGtotals.AllowUserToAddRows = false;
        FGtotals.AllowUserToDeleteRows = false;
        FGtotals.AllowUserToResizeColumns = false;
        FGtotals.AllowUserToResizeRows = false;
        FGtotals.ColumnHeadersHeight = 12;
        FGtotals.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        FGtotals.ColumnHeadersVisible = false;
        dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.TopCenter;
        dataGridViewCellStyle5.BackColor = SystemColors.Window;
        dataGridViewCellStyle5.Font = new Font("Segoe UI", 8F);
        dataGridViewCellStyle5.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
        FGtotals.DefaultCellStyle = dataGridViewCellStyle5;
        FGtotals.Location = new Point(10, 19);
        FGtotals.Name = "FGtotals";
        FGtotals.ReadOnly = true;
        FGtotals.RowHeadersVisible = false;
        FGtotals.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        FGtotals.RowTemplate.Height = 16;
        FGtotals.ScrollBars = ScrollBars.None;
        FGtotals.SelectionMode = DataGridViewSelectionMode.CellSelect;
        FGtotals.Size = new Size(186, 115);
        FGtotals.TabIndex = 0;
        // 
        // Frame6
        // 
        Frame6.Controls.Add(FGrankings);
        Frame6.Location = new Point(348, 288);
        Frame6.Name = "Frame6";
        Frame6.Size = new Size(202, 143);
        Frame6.TabIndex = 4;
        Frame6.TabStop = false;
        Frame6.Text = "Rankings";
        // 
        // FGrankings
        // 
        FGrankings.AllowUserToAddRows = false;
        FGrankings.AllowUserToDeleteRows = false;
        FGrankings.AllowUserToResizeColumns = false;
        FGrankings.AllowUserToResizeRows = false;
        FGrankings.ColumnHeadersHeight = 12;
        FGrankings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        FGrankings.ColumnHeadersVisible = false;
        dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.TopCenter;
        dataGridViewCellStyle6.BackColor = SystemColors.Window;
        dataGridViewCellStyle6.Font = new Font("Segoe UI", 8F);
        dataGridViewCellStyle6.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
        FGrankings.DefaultCellStyle = dataGridViewCellStyle6;
        FGrankings.Location = new Point(10, 19);
        FGrankings.Name = "FGrankings";
        FGrankings.ReadOnly = true;
        FGrankings.RowHeadersVisible = false;
        FGrankings.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        FGrankings.RowTemplate.Height = 16;
        FGrankings.ScrollBars = ScrollBars.None;
        FGrankings.SelectionMode = DataGridViewSelectionMode.CellSelect;
        FGrankings.Size = new Size(186, 115);
        FGrankings.TabIndex = 0;
        // 
        // StatScreen
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(619, 434);
        Controls.Add(Frame6);
        Controls.Add(Frame4);
        Controls.Add(Frame5);
        Controls.Add(Frame3);
        Controls.Add(Frame2);
        Controls.Add(Frame1);
        Controls.Add(OKbutt);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "StatScreen";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Game Statistics";
        Load += Form_Load;
        Frame1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)FGattacked).EndInit();
        ((System.ComponentModel.ISupportInitialize)FGovertaken).EndInit();
        Frame2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)FGkilled).EndInit();
        Frame3.ResumeLayout(false);
        Frame5.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)FGdefeated).EndInit();
        Frame4.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)FGtotals).EndInit();
        Frame6.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)FGrankings).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Button OKbutt;
    private GroupBox Frame1;
    private DataGridView FGattacked;
    private DataGridView FGovertaken;
    private GroupBox Frame2;
    private DataGridView FGkilled;
    private GroupBox Frame3;
    private GroupBox Frame5;
    private DataGridView FGdefeated;
    private GroupBox Frame4;
    private DataGridView FGtotals;
    private GroupBox Frame6;
    private DataGridView FGrankings;
}