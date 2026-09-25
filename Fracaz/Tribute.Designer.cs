namespace Fracaz;

partial class Tribute
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tribute));
        HiScoreButton = new Button();
        StatsButton = new Button();
        WinningButton = new Button();
        WinningPlayer = new Label();
        WinningSentence = new Label();
        SuspendLayout();
        // 
        // HiScoreButton
        // 
        HiScoreButton.Location = new Point(155, 157);
        HiScoreButton.Name = "HiScoreButton";
        HiScoreButton.Size = new Size(130, 23);
        HiScoreButton.TabIndex = 0;
        HiScoreButton.Text = "View Hi Scores";
        HiScoreButton.UseVisualStyleBackColor = true;
        HiScoreButton.Click += HiScoreButton_Click;
        // 
        // StatsButton
        // 
        StatsButton.Location = new Point(12, 157);
        StatsButton.Name = "StatsButton";
        StatsButton.Size = new Size(130, 23);
        StatsButton.TabIndex = 1;
        StatsButton.Text = "View Statistics";
        StatsButton.UseVisualStyleBackColor = true;
        StatsButton.Click += StatsButton_Click;
        // 
        // WinningButton
        // 
        WinningButton.Location = new Point(12, 128);
        WinningButton.Name = "WinningButton";
        WinningButton.Size = new Size(273, 23);
        WinningButton.TabIndex = 2;
        WinningButton.Text = "Command1";
        WinningButton.UseVisualStyleBackColor = true;
        WinningButton.Click += WinningButton_Click;
        // 
        // WinningPlayer
        // 
        WinningPlayer.Font = new Font("Segoe UI", 18F);
        WinningPlayer.Location = new Point(12, 9);
        WinningPlayer.Name = "WinningPlayer";
        WinningPlayer.Size = new Size(273, 38);
        WinningPlayer.TabIndex = 3;
        WinningPlayer.Text = "Winning Player!";
        WinningPlayer.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // WinningSentence
        // 
        WinningSentence.Location = new Point(12, 62);
        WinningSentence.Name = "WinningSentence";
        WinningSentence.Size = new Size(273, 63);
        WinningSentence.TabIndex = 4;
        WinningSentence.Text = "Sentencing";
        WinningSentence.TextAlign = ContentAlignment.TopCenter;
        // 
        // Tribute
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(297, 192);
        Controls.Add(WinningSentence);
        Controls.Add(WinningPlayer);
        Controls.Add(WinningButton);
        Controls.Add(StatsButton);
        Controls.Add(HiScoreButton);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "Tribute";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Game Over";
        FormClosing += Form_QueryUnload;
        Load += Form_Load;
        ResumeLayout(false);
    }

    #endregion

    private Button HiScoreButton;
    private Button StatsButton;
    private Button WinningButton;
    private Label WinningPlayer;
    private Label WinningSentence;
}