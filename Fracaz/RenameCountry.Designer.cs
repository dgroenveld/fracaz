namespace Fracaz;

partial class RenameCountry
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
        OKButt = new Button();
        CancelButt = new Button();
        NewName = new TextBox();
        OldName = new Label();
        label1 = new Label();
        label2 = new Label();
        SuspendLayout();
        // 
        // OKButt
        // 
        OKButt.Location = new Point(202, 73);
        OKButt.Name = "OKButt";
        OKButt.Size = new Size(75, 23);
        OKButt.TabIndex = 0;
        OKButt.Text = "OK";
        OKButt.UseCompatibleTextRendering = true;
        OKButt.UseVisualStyleBackColor = true;
        OKButt.Click += OKButt_Click;
        // 
        // CancelButt
        // 
        CancelButt.Location = new Point(12, 73);
        CancelButt.Name = "CancelButt";
        CancelButt.Size = new Size(75, 23);
        CancelButt.TabIndex = 1;
        CancelButt.Text = "Cancel";
        CancelButt.UseCompatibleTextRendering = true;
        CancelButt.UseVisualStyleBackColor = true;
        CancelButt.Click += CancelButt_Click;
        // 
        // NewName
        // 
        NewName.Location = new Point(116, 43);
        NewName.Name = "NewName";
        NewName.Size = new Size(139, 23);
        NewName.TabIndex = 2;
        NewName.Text = "New Name";
        // 
        // OldName
        // 
        OldName.Location = new Point(116, 18);
        OldName.Name = "OldName";
        OldName.Size = new Size(139, 23);
        OldName.TabIndex = 3;
        OldName.Text = "Old Name";
        OldName.UseCompatibleTextRendering = true;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(39, 45);
        label1.Name = "label1";
        label1.Size = new Size(68, 21);
        label1.TabIndex = 4;
        label1.Text = "New Name:";
        label1.UseCompatibleTextRendering = true;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(39, 18);
        label2.Name = "label2";
        label2.Size = new Size(63, 21);
        label2.TabIndex = 5;
        label2.Text = "Old Name:";
        label2.UseCompatibleTextRendering = true;
        // 
        // RenameCountry
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(289, 108);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(OldName);
        Controls.Add(NewName);
        Controls.Add(CancelButt);
        Controls.Add(OKButt);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "RenameCountry";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Rename";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private Button CancelButt;
    private Label label1;
    private Label label2;
    private Button OKButt;
    internal TextBox NewName;
    internal Label OldName;
}