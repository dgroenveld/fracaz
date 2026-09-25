namespace Fracaz;

partial class GfxOptions
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GfxOptions));
        ExplosionsCheckbox = new CheckBox();
        WavesCheckbox = new CheckBox();
        PromptCheckbox = new CheckBox();
        FlashingCheckbox = new CheckBox();
        UnOccColor = new ComboBox();
        Label4 = new Label();
        AniSpeed = new TrackBar();
        Label3 = new Label();
        Label1 = new Label();
        Label2 = new Label();
        CANCELbutton = new Button();
        OKbutton = new Button();
        ((System.ComponentModel.ISupportInitialize)AniSpeed).BeginInit();
        SuspendLayout();
        // 
        // ExplosionsCheckbox
        // 
        ExplosionsCheckbox.AutoSize = true;
        ExplosionsCheckbox.Checked = true;
        ExplosionsCheckbox.CheckState = CheckState.Checked;
        ExplosionsCheckbox.Location = new Point(19, 17);
        ExplosionsCheckbox.Name = "ExplosionsCheckbox";
        ExplosionsCheckbox.Size = new Size(112, 21);
        ExplosionsCheckbox.TabIndex = 0;
        ExplosionsCheckbox.Text = "Enable Explosions";
        ExplosionsCheckbox.UseCompatibleTextRendering = true;
        ExplosionsCheckbox.UseVisualStyleBackColor = true;
        // 
        // WavesCheckbox
        // 
        WavesCheckbox.AutoSize = true;
        WavesCheckbox.Checked = true;
        WavesCheckbox.CheckState = CheckState.Checked;
        WavesCheckbox.Location = new Point(149, 17);
        WavesCheckbox.Name = "WavesCheckbox";
        WavesCheckbox.Size = new Size(92, 21);
        WavesCheckbox.TabIndex = 1;
        WavesCheckbox.Text = "Enable Waves";
        WavesCheckbox.UseCompatibleTextRendering = true;
        WavesCheckbox.UseVisualStyleBackColor = true;
        // 
        // PromptCheckbox
        // 
        PromptCheckbox.AutoSize = true;
        PromptCheckbox.Checked = true;
        PromptCheckbox.CheckState = CheckState.Checked;
        PromptCheckbox.Location = new Point(149, 55);
        PromptCheckbox.Name = "PromptCheckbox";
        PromptCheckbox.Size = new Size(126, 21);
        PromptCheckbox.TabIndex = 2;
        PromptCheckbox.Text = "Prompt to Save Map";
        PromptCheckbox.UseCompatibleTextRendering = true;
        PromptCheckbox.UseVisualStyleBackColor = true;
        // 
        // FlashingCheckbox
        // 
        FlashingCheckbox.AutoSize = true;
        FlashingCheckbox.Checked = true;
        FlashingCheckbox.CheckState = CheckState.Checked;
        FlashingCheckbox.Location = new Point(19, 55);
        FlashingCheckbox.Name = "FlashingCheckbox";
        FlashingCheckbox.Size = new Size(99, 21);
        FlashingCheckbox.TabIndex = 3;
        FlashingCheckbox.Text = "Flash Countries";
        FlashingCheckbox.UseCompatibleTextRendering = true;
        FlashingCheckbox.UseVisualStyleBackColor = true;
        // 
        // UnOccColor
        // 
        UnOccColor.FormattingEnabled = true;
        UnOccColor.Location = new Point(163, 101);
        UnOccColor.Name = "UnOccColor";
        UnOccColor.Size = new Size(105, 21);
        UnOccColor.TabIndex = 4;
        UnOccColor.SelectedValueChanged += UnOccColor_Click;
        // 
        // Label4
        // 
        Label4.AutoSize = true;
        Label4.Font = new Font("Segoe UI", 8.25F);
        Label4.Location = new Point(18, 103);
        Label4.Name = "Label4";
        Label4.Size = new Size(139, 20);
        Label4.TabIndex = 5;
        Label4.Text = "Unoccupied Country Color:";
        Label4.UseCompatibleTextRendering = true;
        // 
        // AniSpeed
        // 
        AniSpeed.LargeChange = 2;
        AniSpeed.Location = new Point(12, 164);
        AniSpeed.Maximum = 200;
        AniSpeed.Minimum = 1;
        AniSpeed.Name = "AniSpeed";
        AniSpeed.Size = new Size(256, 45);
        AniSpeed.TabIndex = 6;
        AniSpeed.TickFrequency = 20;
        AniSpeed.Value = 20;
        AniSpeed.ValueChanged += AniSpeed_Change;
        // 
        // Label3
        // 
        Label3.AutoSize = true;
        Label3.Font = new Font("Segoe UI", 8.25F);
        Label3.Location = new Point(75, 141);
        Label3.Name = "Label3";
        Label3.Size = new Size(126, 20);
        Label3.TabIndex = 7;
        Label3.Text = "Overall Animaton Speed";
        Label3.UseCompatibleTextRendering = true;
        // 
        // Label1
        // 
        Label1.AutoSize = true;
        Label1.Font = new Font("Segoe UI", 8.25F);
        Label1.Location = new Point(18, 141);
        Label1.Name = "Label1";
        Label1.Size = new Size(24, 20);
        Label1.TabIndex = 8;
        Label1.Text = "Fast";
        Label1.UseCompatibleTextRendering = true;
        // 
        // Label2
        // 
        Label2.AutoSize = true;
        Label2.Font = new Font("Segoe UI", 8.25F);
        Label2.Location = new Point(244, 141);
        Label2.Name = "Label2";
        Label2.Size = new Size(28, 20);
        Label2.TabIndex = 9;
        Label2.Text = "Slow";
        Label2.UseCompatibleTextRendering = true;
        // 
        // CANCELbutton
        // 
        CANCELbutton.Location = new Point(193, 203);
        CANCELbutton.Name = "CANCELbutton";
        CANCELbutton.Size = new Size(75, 23);
        CANCELbutton.TabIndex = 10;
        CANCELbutton.Text = "Cancel";
        CANCELbutton.UseVisualStyleBackColor = true;
        CANCELbutton.Click += CANCELbutton_Click;
        // 
        // OKbutton
        // 
        OKbutton.Location = new Point(19, 203);
        OKbutton.Name = "OKbutton";
        OKbutton.Size = new Size(75, 23);
        OKbutton.TabIndex = 11;
        OKbutton.Text = "OK";
        OKbutton.UseVisualStyleBackColor = true;
        OKbutton.Click += OKbutton_Click;
        // 
        // GfxOptions
        // 
        AutoScaleDimensions = new SizeF(6F, 13F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(280, 238);
        Controls.Add(OKbutton);
        Controls.Add(CANCELbutton);
        Controls.Add(Label2);
        Controls.Add(Label1);
        Controls.Add(Label3);
        Controls.Add(AniSpeed);
        Controls.Add(Label4);
        Controls.Add(UnOccColor);
        Controls.Add(FlashingCheckbox);
        Controls.Add(PromptCheckbox);
        Controls.Add(WavesCheckbox);
        Controls.Add(ExplosionsCheckbox);
        Font = new Font("Segoe UI", 8.25F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "GfxOptions";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Graphics Optons";
        FormClosing += Form_QueryUnload;
        Load += Form_Load;
        ((System.ComponentModel.ISupportInitialize)AniSpeed).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private CheckBox ExplosionsCheckbox;
    private CheckBox WavesCheckbox;
    private CheckBox PromptCheckbox;
    private CheckBox FlashingCheckbox;
    private ComboBox UnOccColor;
    private Label Label4;
    private TrackBar AniSpeed;
    private Label Label3;
    private Label Label1;
    private Label Label2;
    private Button CANCELbutton;
    private Button OKbutton;
}