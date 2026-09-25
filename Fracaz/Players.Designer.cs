namespace Fracaz;

partial class Players
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Players));
        groupBox1 = new GroupBox();
        Color1 = new ComboBox();
        Personal1 = new ComboBox();
        PName1 = new TextBox();
        None1 = new RadioButton();
        Computer1 = new RadioButton();
        Network1 = new RadioButton();
        Human1 = new RadioButton();
        groupBox2 = new GroupBox();
        Color2 = new ComboBox();
        Personal2 = new ComboBox();
        PName2 = new TextBox();
        None2 = new RadioButton();
        Computer2 = new RadioButton();
        Network2 = new RadioButton();
        Human2 = new RadioButton();
        PlayerOK = new Button();
        PlayerCancel = new Button();
        groupBox3 = new GroupBox();
        Color3 = new ComboBox();
        Personal3 = new ComboBox();
        PName3 = new TextBox();
        None3 = new RadioButton();
        Computer3 = new RadioButton();
        Network3 = new RadioButton();
        Human3 = new RadioButton();
        groupBox4 = new GroupBox();
        Color4 = new ComboBox();
        Personal4 = new ComboBox();
        PName4 = new TextBox();
        None4 = new RadioButton();
        Computer4 = new RadioButton();
        Network4 = new RadioButton();
        Human4 = new RadioButton();
        groupBox5 = new GroupBox();
        Color5 = new ComboBox();
        Personal5 = new ComboBox();
        PName5 = new TextBox();
        None5 = new RadioButton();
        Computer5 = new RadioButton();
        Network5 = new RadioButton();
        Human5 = new RadioButton();
        groupBox6 = new GroupBox();
        Color6 = new ComboBox();
        Personal6 = new ComboBox();
        PName6 = new TextBox();
        None6 = new RadioButton();
        Computer6 = new RadioButton();
        Network6 = new RadioButton();
        Human6 = new RadioButton();
        groupBox1.SuspendLayout();
        groupBox2.SuspendLayout();
        groupBox3.SuspendLayout();
        groupBox4.SuspendLayout();
        groupBox5.SuspendLayout();
        groupBox6.SuspendLayout();
        SuspendLayout();
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(Color1);
        groupBox1.Controls.Add(Personal1);
        groupBox1.Controls.Add(PName1);
        groupBox1.Controls.Add(None1);
        groupBox1.Controls.Add(Computer1);
        groupBox1.Controls.Add(Network1);
        groupBox1.Controls.Add(Human1);
        groupBox1.Font = new Font("Segoe UI", 8.25F);
        groupBox1.Location = new Point(12, 12);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(97, 212);
        groupBox1.TabIndex = 0;
        groupBox1.TabStop = false;
        groupBox1.Text = "Player 1";
        groupBox1.UseCompatibleTextRendering = true;
        // 
        // Color1
        // 
        Color1.Font = new Font("Segoe UI", 8.25F);
        Color1.FormattingEnabled = true;
        Color1.Location = new Point(6, 177);
        Color1.Name = "Color1";
        Color1.Size = new Size(85, 21);
        Color1.TabIndex = 6;
        Color1.SelectedIndexChanged += Color_Click;
        Color1.TextUpdate += Color_TextUpdate;
        Color1.KeyDown += Color_KeyDown;
        Color1.KeyPress += Color_KeyDown;
        // 
        // Personal1
        // 
        Personal1.Font = new Font("Segoe UI", 8.25F);
        Personal1.FormattingEnabled = true;
        Personal1.Location = new Point(6, 126);
        Personal1.Name = "Personal1";
        Personal1.Size = new Size(85, 21);
        Personal1.TabIndex = 5;
        Personal1.TextUpdate += Color_TextUpdate;
        Personal1.KeyDown += Color_KeyDown;
        Personal1.KeyPress += Color_KeyDown;
        // 
        // PName1
        // 
        PName1.Font = new Font("Segoe UI", 8.25F);
        PName1.Location = new Point(6, 22);
        PName1.Name = "PName1";
        PName1.Size = new Size(85, 22);
        PName1.TabIndex = 4;
        // 
        // None1
        // 
        None1.AutoSize = true;
        None1.Font = new Font("Segoe UI", 8.25F);
        None1.Location = new Point(5, 152);
        None1.Name = "None1";
        None1.Size = new Size(58, 21);
        None1.TabIndex = 3;
        None1.TabStop = true;
        None1.Text = "Inactve";
        None1.UseCompatibleTextRendering = true;
        None1.UseVisualStyleBackColor = true;
        None1.CheckedChanged += None_Click;
        // 
        // Computer1
        // 
        Computer1.AutoSize = true;
        Computer1.Font = new Font("Segoe UI", 8.25F);
        Computer1.Location = new Point(5, 101);
        Computer1.Name = "Computer1";
        Computer1.Size = new Size(72, 21);
        Computer1.TabIndex = 2;
        Computer1.TabStop = true;
        Computer1.Text = "Computer";
        Computer1.UseCompatibleTextRendering = true;
        Computer1.UseVisualStyleBackColor = true;
        Computer1.CheckedChanged += Computer_Click;
        // 
        // Network1
        // 
        Network1.AutoSize = true;
        Network1.Font = new Font("Segoe UI", 8.25F);
        Network1.Location = new Point(5, 76);
        Network1.Name = "Network1";
        Network1.Size = new Size(65, 21);
        Network1.TabIndex = 1;
        Network1.TabStop = true;
        Network1.Text = "Network";
        Network1.UseCompatibleTextRendering = true;
        Network1.UseVisualStyleBackColor = true;
        Network1.CheckedChanged += Network_Click;
        // 
        // Human1
        // 
        Human1.AutoSize = true;
        Human1.Font = new Font("Segoe UI", 8.25F);
        Human1.Location = new Point(6, 51);
        Human1.Name = "Human1";
        Human1.Size = new Size(59, 21);
        Human1.TabIndex = 0;
        Human1.TabStop = true;
        Human1.Text = "Human";
        Human1.UseCompatibleTextRendering = true;
        Human1.UseVisualStyleBackColor = true;
        Human1.CheckedChanged += Human_Click;
        // 
        // groupBox2
        // 
        groupBox2.Controls.Add(Color2);
        groupBox2.Controls.Add(Personal2);
        groupBox2.Controls.Add(PName2);
        groupBox2.Controls.Add(None2);
        groupBox2.Controls.Add(Computer2);
        groupBox2.Controls.Add(Network2);
        groupBox2.Controls.Add(Human2);
        groupBox2.Font = new Font("Segoe UI", 8.25F);
        groupBox2.Location = new Point(115, 12);
        groupBox2.Name = "groupBox2";
        groupBox2.Size = new Size(97, 212);
        groupBox2.TabIndex = 7;
        groupBox2.TabStop = false;
        groupBox2.Text = "Player 2";
        groupBox2.UseCompatibleTextRendering = true;
        // 
        // Color2
        // 
        Color2.Font = new Font("Segoe UI", 8.25F);
        Color2.FormattingEnabled = true;
        Color2.Location = new Point(6, 177);
        Color2.Name = "Color2";
        Color2.Size = new Size(85, 21);
        Color2.TabIndex = 6;
        Color2.SelectedIndexChanged += Color_Click;
        Color2.TextUpdate += Color_TextUpdate;
        Color2.KeyDown += Color_KeyDown;
        Color2.KeyPress += Color_KeyDown;
        // 
        // Personal2
        // 
        Personal2.Font = new Font("Segoe UI", 8.25F);
        Personal2.FormattingEnabled = true;
        Personal2.Location = new Point(6, 126);
        Personal2.Name = "Personal2";
        Personal2.Size = new Size(85, 21);
        Personal2.TabIndex = 5;
        Personal2.TextUpdate += Color_TextUpdate;
        Personal2.KeyDown += Color_KeyDown;
        Personal2.KeyPress += Color_KeyDown;
        // 
        // PName2
        // 
        PName2.Font = new Font("Segoe UI", 8.25F);
        PName2.Location = new Point(6, 22);
        PName2.Name = "PName2";
        PName2.Size = new Size(85, 22);
        PName2.TabIndex = 4;
        // 
        // None2
        // 
        None2.AutoSize = true;
        None2.Font = new Font("Segoe UI", 8.25F);
        None2.Location = new Point(5, 152);
        None2.Name = "None2";
        None2.Size = new Size(58, 21);
        None2.TabIndex = 3;
        None2.TabStop = true;
        None2.Text = "Inactve";
        None2.UseCompatibleTextRendering = true;
        None2.UseVisualStyleBackColor = true;
        None2.CheckedChanged += None_Click;
        // 
        // Computer2
        // 
        Computer2.AutoSize = true;
        Computer2.Font = new Font("Segoe UI", 8.25F);
        Computer2.Location = new Point(5, 101);
        Computer2.Name = "Computer2";
        Computer2.Size = new Size(72, 21);
        Computer2.TabIndex = 2;
        Computer2.TabStop = true;
        Computer2.Text = "Computer";
        Computer2.UseCompatibleTextRendering = true;
        Computer2.UseVisualStyleBackColor = true;
        Computer2.CheckedChanged += Computer_Click;
        // 
        // Network2
        // 
        Network2.AutoSize = true;
        Network2.Font = new Font("Segoe UI", 8.25F);
        Network2.Location = new Point(5, 76);
        Network2.Name = "Network2";
        Network2.Size = new Size(65, 21);
        Network2.TabIndex = 1;
        Network2.TabStop = true;
        Network2.Text = "Network";
        Network2.UseCompatibleTextRendering = true;
        Network2.UseVisualStyleBackColor = true;
        Network2.CheckedChanged += Network_Click;
        // 
        // Human2
        // 
        Human2.AutoSize = true;
        Human2.Font = new Font("Segoe UI", 8.25F);
        Human2.Location = new Point(6, 51);
        Human2.Name = "Human2";
        Human2.Size = new Size(59, 21);
        Human2.TabIndex = 0;
        Human2.TabStop = true;
        Human2.Text = "Human";
        Human2.UseCompatibleTextRendering = true;
        Human2.UseVisualStyleBackColor = true;
        Human2.CheckedChanged += Human_Click;
        // 
        // PlayerOK
        // 
        PlayerOK.Location = new Point(59, 448);
        PlayerOK.Name = "PlayerOK";
        PlayerOK.Size = new Size(75, 23);
        PlayerOK.TabIndex = 8;
        PlayerOK.Text = "OK";
        PlayerOK.UseVisualStyleBackColor = true;
        PlayerOK.Click += PlayerOK_Click;
        // 
        // PlayerCancel
        // 
        PlayerCancel.Location = new Point(196, 448);
        PlayerCancel.Name = "PlayerCancel";
        PlayerCancel.Size = new Size(75, 23);
        PlayerCancel.TabIndex = 9;
        PlayerCancel.Text = "Cancel";
        PlayerCancel.UseVisualStyleBackColor = true;
        PlayerCancel.Click += PlayerCancel_Click;
        // 
        // groupBox3
        // 
        groupBox3.Controls.Add(Color3);
        groupBox3.Controls.Add(Personal3);
        groupBox3.Controls.Add(PName3);
        groupBox3.Controls.Add(None3);
        groupBox3.Controls.Add(Computer3);
        groupBox3.Controls.Add(Network3);
        groupBox3.Controls.Add(Human3);
        groupBox3.Font = new Font("Segoe UI", 8.25F);
        groupBox3.Location = new Point(218, 12);
        groupBox3.Name = "groupBox3";
        groupBox3.Size = new Size(97, 212);
        groupBox3.TabIndex = 8;
        groupBox3.TabStop = false;
        groupBox3.Text = "Player 3";
        groupBox3.UseCompatibleTextRendering = true;
        // 
        // Color3
        // 
        Color3.Font = new Font("Segoe UI", 8.25F);
        Color3.FormattingEnabled = true;
        Color3.Location = new Point(6, 177);
        Color3.Name = "Color3";
        Color3.Size = new Size(85, 21);
        Color3.TabIndex = 6;
        Color3.SelectedIndexChanged += Color_Click;
        Color3.TextUpdate += Color_TextUpdate;
        Color3.KeyDown += Color_KeyDown;
        Color3.KeyPress += Color_KeyDown;
        // 
        // Personal3
        // 
        Personal3.Font = new Font("Segoe UI", 8.25F);
        Personal3.FormattingEnabled = true;
        Personal3.Location = new Point(6, 126);
        Personal3.Name = "Personal3";
        Personal3.Size = new Size(85, 21);
        Personal3.TabIndex = 5;
        Personal3.TextUpdate += Color_TextUpdate;
        Personal3.KeyDown += Color_KeyDown;
        Personal3.KeyPress += Color_KeyDown;
        // 
        // PName3
        // 
        PName3.Font = new Font("Segoe UI", 8.25F);
        PName3.Location = new Point(6, 22);
        PName3.Name = "PName3";
        PName3.Size = new Size(85, 22);
        PName3.TabIndex = 4;
        // 
        // None3
        // 
        None3.AutoSize = true;
        None3.Font = new Font("Segoe UI", 8.25F);
        None3.Location = new Point(5, 152);
        None3.Name = "None3";
        None3.Size = new Size(58, 21);
        None3.TabIndex = 3;
        None3.TabStop = true;
        None3.Text = "Inactve";
        None3.UseCompatibleTextRendering = true;
        None3.UseVisualStyleBackColor = true;
        None3.CheckedChanged += None_Click;
        // 
        // Computer3
        // 
        Computer3.AutoSize = true;
        Computer3.Font = new Font("Segoe UI", 8.25F);
        Computer3.Location = new Point(5, 101);
        Computer3.Name = "Computer3";
        Computer3.Size = new Size(72, 21);
        Computer3.TabIndex = 2;
        Computer3.TabStop = true;
        Computer3.Text = "Computer";
        Computer3.UseCompatibleTextRendering = true;
        Computer3.UseVisualStyleBackColor = true;
        Computer3.CheckedChanged += Computer_Click;
        // 
        // Network3
        // 
        Network3.AutoSize = true;
        Network3.Font = new Font("Segoe UI", 8.25F);
        Network3.Location = new Point(5, 76);
        Network3.Name = "Network3";
        Network3.Size = new Size(65, 21);
        Network3.TabIndex = 1;
        Network3.TabStop = true;
        Network3.Text = "Network";
        Network3.UseCompatibleTextRendering = true;
        Network3.UseVisualStyleBackColor = true;
        Network3.CheckedChanged += Network_Click;
        // 
        // Human3
        // 
        Human3.AutoSize = true;
        Human3.Font = new Font("Segoe UI", 8.25F);
        Human3.Location = new Point(6, 51);
        Human3.Name = "Human3";
        Human3.Size = new Size(59, 21);
        Human3.TabIndex = 0;
        Human3.TabStop = true;
        Human3.Text = "Human";
        Human3.UseCompatibleTextRendering = true;
        Human3.UseVisualStyleBackColor = true;
        Human3.CheckedChanged += Human_Click;
        // 
        // groupBox4
        // 
        groupBox4.Controls.Add(Color4);
        groupBox4.Controls.Add(Personal4);
        groupBox4.Controls.Add(PName4);
        groupBox4.Controls.Add(None4);
        groupBox4.Controls.Add(Computer4);
        groupBox4.Controls.Add(Network4);
        groupBox4.Controls.Add(Human4);
        groupBox4.Font = new Font("Segoe UI", 8.25F);
        groupBox4.Location = new Point(12, 230);
        groupBox4.Name = "groupBox4";
        groupBox4.Size = new Size(97, 212);
        groupBox4.TabIndex = 10;
        groupBox4.TabStop = false;
        groupBox4.Text = "Player 4";
        groupBox4.UseCompatibleTextRendering = true;
        // 
        // Color4
        // 
        Color4.Font = new Font("Segoe UI", 8.25F);
        Color4.FormattingEnabled = true;
        Color4.Location = new Point(6, 177);
        Color4.Name = "Color4";
        Color4.Size = new Size(85, 21);
        Color4.TabIndex = 6;
        Color4.SelectedIndexChanged += Color_Click;
        Color4.TextUpdate += Color_TextUpdate;
        Color4.KeyDown += Color_KeyDown;
        Color4.KeyPress += Color_KeyDown;
        // 
        // Personal4
        // 
        Personal4.Font = new Font("Segoe UI", 8.25F);
        Personal4.FormattingEnabled = true;
        Personal4.Location = new Point(6, 126);
        Personal4.Name = "Personal4";
        Personal4.Size = new Size(85, 21);
        Personal4.TabIndex = 5;
        Personal4.TextUpdate += Color_TextUpdate;
        Personal4.KeyDown += Color_KeyDown;
        Personal4.KeyPress += Color_KeyDown;
        // 
        // PName4
        // 
        PName4.Font = new Font("Segoe UI", 8.25F);
        PName4.Location = new Point(6, 22);
        PName4.Name = "PName4";
        PName4.Size = new Size(85, 22);
        PName4.TabIndex = 4;
        // 
        // None4
        // 
        None4.AutoSize = true;
        None4.Font = new Font("Segoe UI", 8.25F);
        None4.Location = new Point(5, 152);
        None4.Name = "None4";
        None4.Size = new Size(58, 21);
        None4.TabIndex = 3;
        None4.TabStop = true;
        None4.Text = "Inactve";
        None4.UseCompatibleTextRendering = true;
        None4.UseVisualStyleBackColor = true;
        None4.CheckedChanged += None_Click;
        // 
        // Computer4
        // 
        Computer4.AutoSize = true;
        Computer4.Font = new Font("Segoe UI", 8.25F);
        Computer4.Location = new Point(5, 101);
        Computer4.Name = "Computer4";
        Computer4.Size = new Size(72, 21);
        Computer4.TabIndex = 2;
        Computer4.TabStop = true;
        Computer4.Text = "Computer";
        Computer4.UseCompatibleTextRendering = true;
        Computer4.UseVisualStyleBackColor = true;
        Computer4.CheckedChanged += Computer_Click;
        // 
        // Network4
        // 
        Network4.AutoSize = true;
        Network4.Font = new Font("Segoe UI", 8.25F);
        Network4.Location = new Point(5, 76);
        Network4.Name = "Network4";
        Network4.Size = new Size(65, 21);
        Network4.TabIndex = 1;
        Network4.TabStop = true;
        Network4.Text = "Network";
        Network4.UseCompatibleTextRendering = true;
        Network4.UseVisualStyleBackColor = true;
        Network4.CheckedChanged += Network_Click;
        // 
        // Human4
        // 
        Human4.AutoSize = true;
        Human4.Font = new Font("Segoe UI", 8.25F);
        Human4.Location = new Point(6, 51);
        Human4.Name = "Human4";
        Human4.Size = new Size(59, 21);
        Human4.TabIndex = 0;
        Human4.TabStop = true;
        Human4.Text = "Human";
        Human4.UseCompatibleTextRendering = true;
        Human4.UseVisualStyleBackColor = true;
        Human4.CheckedChanged += Human_Click;
        // 
        // groupBox5
        // 
        groupBox5.Controls.Add(Color5);
        groupBox5.Controls.Add(Personal5);
        groupBox5.Controls.Add(PName5);
        groupBox5.Controls.Add(None5);
        groupBox5.Controls.Add(Computer5);
        groupBox5.Controls.Add(Network5);
        groupBox5.Controls.Add(Human5);
        groupBox5.Font = new Font("Segoe UI", 8.25F);
        groupBox5.Location = new Point(115, 230);
        groupBox5.Name = "groupBox5";
        groupBox5.Size = new Size(97, 212);
        groupBox5.TabIndex = 11;
        groupBox5.TabStop = false;
        groupBox5.Text = "Player 5";
        groupBox5.UseCompatibleTextRendering = true;
        // 
        // Color5
        // 
        Color5.Font = new Font("Segoe UI", 8.25F);
        Color5.FormattingEnabled = true;
        Color5.Location = new Point(6, 177);
        Color5.Name = "Color5";
        Color5.Size = new Size(85, 21);
        Color5.TabIndex = 6;
        Color5.SelectedIndexChanged += Color_Click;
        Color5.TextUpdate += Color_TextUpdate;
        Color5.KeyDown += Color_KeyDown;
        Color5.KeyPress += Color_KeyDown;
        // 
        // Personal5
        // 
        Personal5.Font = new Font("Segoe UI", 8.25F);
        Personal5.FormattingEnabled = true;
        Personal5.Location = new Point(6, 126);
        Personal5.Name = "Personal5";
        Personal5.Size = new Size(85, 21);
        Personal5.TabIndex = 5;
        Personal5.TextUpdate += Color_TextUpdate;
        Personal5.KeyDown += Color_KeyDown;
        Personal5.KeyPress += Color_KeyDown;
        // 
        // PName5
        // 
        PName5.Font = new Font("Segoe UI", 8.25F);
        PName5.Location = new Point(6, 22);
        PName5.Name = "PName5";
        PName5.Size = new Size(85, 22);
        PName5.TabIndex = 4;
        // 
        // None5
        // 
        None5.AutoSize = true;
        None5.Font = new Font("Segoe UI", 8.25F);
        None5.Location = new Point(5, 152);
        None5.Name = "None5";
        None5.Size = new Size(58, 21);
        None5.TabIndex = 3;
        None5.TabStop = true;
        None5.Text = "Inactve";
        None5.UseCompatibleTextRendering = true;
        None5.UseVisualStyleBackColor = true;
        None5.CheckedChanged += None_Click;
        // 
        // Computer5
        // 
        Computer5.AutoSize = true;
        Computer5.Font = new Font("Segoe UI", 8.25F);
        Computer5.Location = new Point(5, 101);
        Computer5.Name = "Computer5";
        Computer5.Size = new Size(72, 21);
        Computer5.TabIndex = 2;
        Computer5.TabStop = true;
        Computer5.Text = "Computer";
        Computer5.UseCompatibleTextRendering = true;
        Computer5.UseVisualStyleBackColor = true;
        Computer5.CheckedChanged += Computer_Click;
        // 
        // Network5
        // 
        Network5.AutoSize = true;
        Network5.Font = new Font("Segoe UI", 8.25F);
        Network5.Location = new Point(5, 76);
        Network5.Name = "Network5";
        Network5.Size = new Size(65, 21);
        Network5.TabIndex = 1;
        Network5.TabStop = true;
        Network5.Text = "Network";
        Network5.UseCompatibleTextRendering = true;
        Network5.UseVisualStyleBackColor = true;
        Network5.CheckedChanged += Network_Click;
        // 
        // Human5
        // 
        Human5.AutoSize = true;
        Human5.Font = new Font("Segoe UI", 8.25F);
        Human5.Location = new Point(6, 51);
        Human5.Name = "Human5";
        Human5.Size = new Size(59, 21);
        Human5.TabIndex = 0;
        Human5.TabStop = true;
        Human5.Text = "Human";
        Human5.UseCompatibleTextRendering = true;
        Human5.UseVisualStyleBackColor = true;
        Human5.CheckedChanged += Human_Click;
        // 
        // groupBox6
        // 
        groupBox6.Controls.Add(Color6);
        groupBox6.Controls.Add(Personal6);
        groupBox6.Controls.Add(PName6);
        groupBox6.Controls.Add(None6);
        groupBox6.Controls.Add(Computer6);
        groupBox6.Controls.Add(Network6);
        groupBox6.Controls.Add(Human6);
        groupBox6.Font = new Font("Segoe UI", 8.25F);
        groupBox6.Location = new Point(218, 230);
        groupBox6.Name = "groupBox6";
        groupBox6.Size = new Size(97, 212);
        groupBox6.TabIndex = 8;
        groupBox6.TabStop = false;
        groupBox6.Text = "Player 6";
        groupBox6.UseCompatibleTextRendering = true;
        // 
        // Color6
        // 
        Color6.Font = new Font("Segoe UI", 8.25F);
        Color6.FormattingEnabled = true;
        Color6.Location = new Point(6, 177);
        Color6.Name = "Color6";
        Color6.Size = new Size(85, 21);
        Color6.TabIndex = 6;
        Color6.SelectedIndexChanged += Color_Click;
        Color6.TextUpdate += Color_TextUpdate;
        Color6.KeyDown += Color_KeyDown;
        Color6.KeyPress += Color_KeyDown;
        // 
        // Personal6
        // 
        Personal6.Font = new Font("Segoe UI", 8.25F);
        Personal6.FormattingEnabled = true;
        Personal6.Location = new Point(6, 126);
        Personal6.Name = "Personal6";
        Personal6.Size = new Size(85, 21);
        Personal6.TabIndex = 5;
        Personal6.TextUpdate += Color_TextUpdate;
        Personal6.KeyDown += Color_KeyDown;
        Personal6.KeyPress += Color_KeyDown;
        // 
        // PName6
        // 
        PName6.Font = new Font("Segoe UI", 8.25F);
        PName6.Location = new Point(6, 22);
        PName6.Name = "PName6";
        PName6.Size = new Size(85, 22);
        PName6.TabIndex = 4;
        // 
        // None6
        // 
        None6.AutoSize = true;
        None6.Font = new Font("Segoe UI", 8.25F);
        None6.Location = new Point(5, 152);
        None6.Name = "None6";
        None6.Size = new Size(58, 21);
        None6.TabIndex = 3;
        None6.TabStop = true;
        None6.Text = "Inactve";
        None6.UseCompatibleTextRendering = true;
        None6.UseVisualStyleBackColor = true;
        None6.CheckedChanged += None_Click;
        // 
        // Computer6
        // 
        Computer6.AutoSize = true;
        Computer6.Font = new Font("Segoe UI", 8.25F);
        Computer6.Location = new Point(5, 101);
        Computer6.Name = "Computer6";
        Computer6.Size = new Size(72, 21);
        Computer6.TabIndex = 2;
        Computer6.TabStop = true;
        Computer6.Text = "Computer";
        Computer6.UseCompatibleTextRendering = true;
        Computer6.UseVisualStyleBackColor = true;
        Computer6.CheckedChanged += Computer_Click;
        // 
        // Network6
        // 
        Network6.AutoSize = true;
        Network6.Font = new Font("Segoe UI", 8.25F);
        Network6.Location = new Point(5, 76);
        Network6.Name = "Network6";
        Network6.Size = new Size(65, 21);
        Network6.TabIndex = 1;
        Network6.TabStop = true;
        Network6.Text = "Network";
        Network6.UseCompatibleTextRendering = true;
        Network6.UseVisualStyleBackColor = true;
        Network6.CheckedChanged += Network_Click;
        // 
        // Human6
        // 
        Human6.AutoSize = true;
        Human6.Font = new Font("Segoe UI", 8.25F);
        Human6.Location = new Point(6, 51);
        Human6.Name = "Human6";
        Human6.Size = new Size(59, 21);
        Human6.TabIndex = 0;
        Human6.TabStop = true;
        Human6.Text = "Human";
        Human6.UseCompatibleTextRendering = true;
        Human6.UseVisualStyleBackColor = true;
        Human6.CheckedChanged += Human_Click;
        // 
        // Players
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(341, 480);
        Controls.Add(groupBox6);
        Controls.Add(groupBox5);
        Controls.Add(groupBox4);
        Controls.Add(groupBox3);
        Controls.Add(PlayerCancel);
        Controls.Add(PlayerOK);
        Controls.Add(groupBox2);
        Controls.Add(groupBox1);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "Players";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Player Selection";
        FormClosing += Form_QueryUnload;
        Load += Form_Load;
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        groupBox2.ResumeLayout(false);
        groupBox2.PerformLayout();
        groupBox3.ResumeLayout(false);
        groupBox3.PerformLayout();
        groupBox4.ResumeLayout(false);
        groupBox4.PerformLayout();
        groupBox5.ResumeLayout(false);
        groupBox5.PerformLayout();
        groupBox6.ResumeLayout(false);
        groupBox6.PerformLayout();
        ResumeLayout(false);
    }




    #endregion

    private GroupBox groupBox1;
    private RadioButton Network1;
    private RadioButton Human1;
    private RadioButton Computer1;
    private ComboBox Color1;
    private ComboBox Personal1;
    private TextBox PName1;
    private RadioButton None1;
    private GroupBox groupBox2;
    private ComboBox Color2;
    private ComboBox Personal2;
    private TextBox PName2;
    private RadioButton None2;
    private RadioButton Computer2;
    private RadioButton Network2;
    private RadioButton Human2;
    private Button PlayerOK;
    private Button PlayerCancel;
    private GroupBox groupBox3;
    private ComboBox Color3;
    private ComboBox Personal3;
    private TextBox PName3;
    private RadioButton None3;
    private RadioButton Computer3;
    private RadioButton Network3;
    private RadioButton Human3;
    private GroupBox groupBox4;
    private ComboBox Color4;
    private ComboBox Personal4;
    private TextBox PName4;
    private RadioButton None4;
    private RadioButton Computer4;
    private RadioButton Network4;
    private RadioButton Human4;
    private GroupBox groupBox5;
    private ComboBox Color5;
    private ComboBox Personal5;
    private TextBox PName5;
    private RadioButton None5;
    private RadioButton Computer5;
    private RadioButton Network5;
    private RadioButton Human5;
    private GroupBox groupBox6;
    private ComboBox Color6;
    private ComboBox Personal6;
    private TextBox PName6;
    private RadioButton None6;
    private RadioButton Computer6;
    private RadioButton Network6;
    private RadioButton Human6;
}