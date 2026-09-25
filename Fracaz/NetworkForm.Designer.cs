namespace Fracaz;

partial class NetworkForm
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
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkForm));
        lblName = new Label();
        lblHost = new Label();
        HostName = new TextBox();
        MyPlayerName = new TextBox();
        JoinBut = new Button();
        CancelBut = new Button();
        StatusText = new Label();
        NetProg = new ProgressBar();
        ColorPool1 = new Label();
        ColorStatus1 = new Label();
        ColorStatus2 = new Label();
        ColorPool2 = new Label();
        ColorStatus3 = new Label();
        ColorPool3 = new Label();
        ColorStatus4 = new Label();
        ColorPool4 = new Label();
        ColorStatus5 = new Label();
        ColorPool5 = new Label();
        ColorStatus6 = new Label();
        ColorPool6 = new Label();
        HBtimer = new System.Windows.Forms.Timer(components);
        SuspendLayout();
        // 
        // lblName
        // 
        lblName.AutoSize = true;
        lblName.Location = new Point(17, 179);
        lblName.Name = "lblName";
        lblName.Size = new Size(77, 15);
        lblName.TabIndex = 0;
        lblName.Text = "Player Name:";
        // 
        // lblHost
        // 
        lblHost.AutoSize = true;
        lblHost.Location = new Point(17, 147);
        lblHost.Name = "lblHost";
        lblHost.Size = new Size(65, 15);
        lblHost.TabIndex = 1;
        lblHost.Text = "Hostname:";
        // 
        // HostName
        // 
        HostName.Location = new Point(100, 147);
        HostName.Name = "HostName";
        HostName.Size = new Size(100, 23);
        HostName.TabIndex = 2;
        HostName.Text = "BigSmozz";
        // 
        // MyPlayerName
        // 
        MyPlayerName.Location = new Point(100, 176);
        MyPlayerName.Name = "MyPlayerName";
        MyPlayerName.Size = new Size(100, 23);
        MyPlayerName.TabIndex = 3;
        // 
        // JoinBut
        // 
        JoinBut.Location = new Point(69, 205);
        JoinBut.Name = "JoinBut";
        JoinBut.Size = new Size(75, 23);
        JoinBut.TabIndex = 4;
        JoinBut.Text = "Join!";
        JoinBut.UseVisualStyleBackColor = true;
        JoinBut.Click += JoinBut_Click;
        // 
        // CancelBut
        // 
        CancelBut.Location = new Point(69, 234);
        CancelBut.Name = "CancelBut";
        CancelBut.Size = new Size(75, 23);
        CancelBut.TabIndex = 5;
        CancelBut.Text = "Cancel";
        CancelBut.UseVisualStyleBackColor = true;
        CancelBut.Click += CancelBut_Click;
        // 
        // StatusText
        // 
        StatusText.Location = new Point(12, 270);
        StatusText.Name = "StatusText";
        StatusText.Size = new Size(188, 44);
        StatusText.TabIndex = 6;
        StatusText.Text = "Label1";
        StatusText.TextAlign = ContentAlignment.TopCenter;
        // 
        // NetProg
        // 
        NetProg.Location = new Point(16, 312);
        NetProg.Name = "NetProg";
        NetProg.Size = new Size(183, 23);
        NetProg.TabIndex = 7;
        // 
        // ColorPool1
        // 
        ColorPool1.Location = new Point(16, 7);
        ColorPool1.Name = "ColorPool1";
        ColorPool1.Size = new Size(80, 20);
        ColorPool1.TabIndex = 8;
        ColorPool1.Text = "Color1";
        ColorPool1.TextAlign = ContentAlignment.MiddleCenter;
        ColorPool1.UseCompatibleTextRendering = true;
        ColorPool1.Click += ColorPool_Click;
        // 
        // ColorStatus1
        // 
        ColorStatus1.Location = new Point(120, 7);
        ColorStatus1.Name = "ColorStatus1";
        ColorStatus1.Size = new Size(80, 20);
        ColorStatus1.TabIndex = 9;
        ColorStatus1.Text = "Label1";
        ColorStatus1.TextAlign = ContentAlignment.MiddleCenter;
        ColorStatus1.UseCompatibleTextRendering = true;
        // 
        // ColorStatus2
        // 
        ColorStatus2.Location = new Point(120, 27);
        ColorStatus2.Name = "ColorStatus2";
        ColorStatus2.Size = new Size(80, 20);
        ColorStatus2.TabIndex = 11;
        ColorStatus2.Text = "Label1";
        ColorStatus2.TextAlign = ContentAlignment.MiddleCenter;
        ColorStatus2.UseCompatibleTextRendering = true;
        // 
        // ColorPool2
        // 
        ColorPool2.Location = new Point(16, 27);
        ColorPool2.Name = "ColorPool2";
        ColorPool2.Size = new Size(80, 20);
        ColorPool2.TabIndex = 10;
        ColorPool2.Text = "Color1";
        ColorPool2.TextAlign = ContentAlignment.MiddleCenter;
        ColorPool2.UseCompatibleTextRendering = true;
        ColorPool2.Click += ColorPool_Click;
        // 
        // ColorStatus3
        // 
        ColorStatus3.Location = new Point(120, 47);
        ColorStatus3.Name = "ColorStatus3";
        ColorStatus3.Size = new Size(80, 20);
        ColorStatus3.TabIndex = 13;
        ColorStatus3.Text = "Label1";
        ColorStatus3.TextAlign = ContentAlignment.MiddleCenter;
        ColorStatus3.UseCompatibleTextRendering = true;
        // 
        // ColorPool3
        // 
        ColorPool3.Location = new Point(16, 47);
        ColorPool3.Name = "ColorPool3";
        ColorPool3.Size = new Size(80, 20);
        ColorPool3.TabIndex = 12;
        ColorPool3.Text = "Color1";
        ColorPool3.TextAlign = ContentAlignment.MiddleCenter;
        ColorPool3.UseCompatibleTextRendering = true;
        ColorPool3.Click += ColorPool_Click;
        // 
        // ColorStatus4
        // 
        ColorStatus4.Location = new Point(120, 67);
        ColorStatus4.Name = "ColorStatus4";
        ColorStatus4.Size = new Size(80, 20);
        ColorStatus4.TabIndex = 15;
        ColorStatus4.Text = "Label1";
        ColorStatus4.TextAlign = ContentAlignment.MiddleCenter;
        ColorStatus4.UseCompatibleTextRendering = true;
        // 
        // ColorPool4
        // 
        ColorPool4.Location = new Point(16, 67);
        ColorPool4.Name = "ColorPool4";
        ColorPool4.Size = new Size(80, 20);
        ColorPool4.TabIndex = 14;
        ColorPool4.Text = "Color1";
        ColorPool4.TextAlign = ContentAlignment.MiddleCenter;
        ColorPool4.UseCompatibleTextRendering = true;
        ColorPool4.Click += ColorPool_Click;
        // 
        // ColorStatus5
        // 
        ColorStatus5.Location = new Point(120, 87);
        ColorStatus5.Name = "ColorStatus5";
        ColorStatus5.Size = new Size(80, 20);
        ColorStatus5.TabIndex = 17;
        ColorStatus5.Text = "Label1";
        ColorStatus5.TextAlign = ContentAlignment.MiddleCenter;
        ColorStatus5.UseCompatibleTextRendering = true;
        // 
        // ColorPool5
        // 
        ColorPool5.Location = new Point(16, 87);
        ColorPool5.Name = "ColorPool5";
        ColorPool5.Size = new Size(80, 20);
        ColorPool5.TabIndex = 16;
        ColorPool5.Text = "Color1";
        ColorPool5.TextAlign = ContentAlignment.MiddleCenter;
        ColorPool5.UseCompatibleTextRendering = true;
        ColorPool5.Click += ColorPool_Click;
        // 
        // ColorStatus6
        // 
        ColorStatus6.Location = new Point(120, 107);
        ColorStatus6.Name = "ColorStatus6";
        ColorStatus6.Size = new Size(80, 20);
        ColorStatus6.TabIndex = 19;
        ColorStatus6.Text = "Label1";
        ColorStatus6.TextAlign = ContentAlignment.MiddleCenter;
        ColorStatus6.UseCompatibleTextRendering = true;
        // 
        // ColorPool6
        // 
        ColorPool6.Location = new Point(16, 107);
        ColorPool6.Name = "ColorPool6";
        ColorPool6.Size = new Size(80, 20);
        ColorPool6.TabIndex = 18;
        ColorPool6.Text = "Color1";
        ColorPool6.TextAlign = ContentAlignment.MiddleCenter;
        ColorPool6.UseCompatibleTextRendering = true;
        ColorPool6.Click += ColorPool_Click;
        // 
        // HBtimer
        // 
        HBtimer.Enabled = true;
        HBtimer.Interval = 1;
        HBtimer.Tick += HBtimer_Timer;
        // 
        // NetworkForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(212, 347);
        Controls.Add(ColorStatus6);
        Controls.Add(ColorPool6);
        Controls.Add(ColorStatus5);
        Controls.Add(ColorPool5);
        Controls.Add(ColorStatus4);
        Controls.Add(ColorPool4);
        Controls.Add(ColorStatus3);
        Controls.Add(ColorPool3);
        Controls.Add(ColorStatus2);
        Controls.Add(ColorPool2);
        Controls.Add(ColorStatus1);
        Controls.Add(ColorPool1);
        Controls.Add(NetProg);
        Controls.Add(StatusText);
        Controls.Add(CancelBut);
        Controls.Add(JoinBut);
        Controls.Add(MyPlayerName);
        Controls.Add(HostName);
        Controls.Add(lblHost);
        Controls.Add(lblName);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MdiChildrenMinimizedAnchorBottom = false;
        MinimizeBox = false;
        Name = "NetworkForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Network Game";
        FormClosing += Form_QueryUnload;
        Load += Form_Load;
        Paint += Form_Paint;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblName;
    private Label lblHost;
    private TextBox HostName;
    private TextBox MyPlayerName;
    private Button JoinBut;
    private Button CancelBut;
    private Label StatusText;
    private ProgressBar NetProg;
    private Label ColorPool1;
    private Label ColorStatus1;
    private Label ColorStatus2;
    private Label ColorPool2;
    private Label ColorStatus3;
    private Label ColorPool3;
    private Label ColorStatus4;
    private Label ColorPool4;
    private Label ColorStatus5;
    private Label ColorPool5;
    private Label ColorStatus6;
    private Label ColorPool6;
    private System.Windows.Forms.Timer HBtimer;
}