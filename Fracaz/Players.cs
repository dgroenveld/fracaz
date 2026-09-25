using static Fracaz.Boom;
using static Fracaz.ComputerAI;
using static Fracaz.Declarations;

namespace Fracaz;

public partial class Players : Form
{
    private ComboBox[] Colors = new ComboBox[MAX_PLAYERS + 1];
    private RadioButton[] None = new RadioButton[MAX_PLAYERS + 1];
    private RadioButton[] Human = new RadioButton[MAX_PLAYERS + 1];
    private RadioButton[] Computer = new RadioButton[MAX_PLAYERS + 1];
    private RadioButton[] Network = new RadioButton[MAX_PLAYERS + 1];
    private TextBox[] PName = new TextBox[MAX_PLAYERS + 1];
    private ComboBox[] Personal = new ComboBox[MAX_PLAYERS + 1];

    public Players()
    {
        InitializeComponent();
    }

    private void Color_Click(object sender, EventArgs e)
    {
        UpdateColorDropdowns();

        PlayerOK.Focus();
    }

    private void UpdateColorDropdowns()
    {
        // This sub just writes the chosen colors to the BackColor of the
        // color drop-downs.

        for (var i = 1; i <= 6; i++)
        {
            for (var j = 1; j <= 11; j++)
            {
                if (Colors[i].Text == PlayerColors[j])
                {
                    Colors[i].BackColor = Color.FromArgb(PlayerColorCodes[j]);
                    Colors[i].ForeColor = Color.FromArgb(PlayerTextColor[j]);
                }
            }
        }
    }

    private void Computer_Click(object sender, EventArgs e)
    {
        var index = Array.IndexOf(Computer, sender);
        Personal[index].Enabled = Computer[index].Checked;
    }

    private void Form_Load(object sender, EventArgs e)
    {
        Colors = new[] { null!, Color1, Color2, Color3, Color4, Color5, Color6 };
        None = new[] { null!, None1, None2, None3, None4, None5, None6 };
        Human = new[] { null!, Human1, Human2, Human3, Human4, Human5, Human6 };
        Computer = new[] { null!, Computer1, Computer2, Computer3, Computer4, Computer5, Computer6 };
        Network = new[] { null!, Network1, Network2, Network3, Network4, Network5, Network6 };
        PName = new[] { null!, PName1, PName2, PName3, PName4, PName5, PName6 };
        Personal = new[] { null!, Personal1, Personal2, Personal3, Personal4, Personal5, Personal6 };


        for (var i = 1; i <= 6; i++)
        {
            // Set up the color control.
            Colors[i].Items.Clear();

            for (var j = 1; j <= 11; j++) //Not 12 since white is reserved for flashing.
            {
                //Don't add the unoccupied color to the dropdowns.

                if (j != CFGUnoccupiedColor)
                {
                    Colors[i].Items.Add(PlayerColors[j]);
                }
            }

            Colors[i].Text = PlayerColors[Player[i]];

            // Set up the player types.
            switch (PlayerType[i])
            {
                case PTYPE_INACTIVE:
                    None[i].Checked = true;
                    break;
                case PTYPE_HUMAN:
                    Human[i].Checked = true;
                    break;
                case PTYPE_COMPUTER:
                    Computer[i].Checked = true;
                    break;
                case PTYPE_NETWORK:
                    Network[i].Checked = true;
                    break;
            }

            // Set up the player names.
            PName[i].Text = PlayerName[i];

            // Set up the computer personalities.
            for (var j = 1; j <= NUMPERSONALITIES; j++)
            {
                Personal[i].Items.Add(PersonalityName[j]);
            }
            Personal[i].Text = PersonalityName[Personality[i]];
            Personal[i].Enabled = false;
            if (PlayerType[i] == PTYPE_COMPUTER)
            {
                Personal[i].Enabled = true;
            }
        }

        UpdateColorDropdowns();
    }

    private void Form_QueryUnload(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.None)
        {
            PlayerCancel_Click(sender, EventArgs.Empty);
        }
    }

    private void Human_Click(object sender, EventArgs e)
    {
        var index = Array.IndexOf(Human, sender);
        Personal[index].Enabled = false;
    }

    private void Network_Click(object sender, EventArgs e)
    {
        var index = Array.IndexOf(Network, sender);
        Personal[index].Enabled = false;
    }

    private void None_Click(object sender, EventArgs e)
    {
        var index = Array.IndexOf(None, sender);
        Personal[index].Enabled = false;
    }

    private void PlayerCancel_Click(object sender, EventArgs e)
    {
        // Clean out the balls and waves arrays.
        ClearAllBalls();
        ClearAllWaves();

        // Make this settings form disappear.
        this.Close();
    }

    private void PlayerOK_Click(object sender, EventArgs e)
    {
        // Check to make sure we have at least two players.
        int Numpl = 0;
        for (var i = 1; i <= 6; i++)
        {
            if (!None[i].Checked)
            {
                Numpl = Numpl + 1;
            }
        }
        if (Numpl < 2)
        {
            MessageBox.Show("There must be at least two players.");
            return;
        }

        // Check to make sure we have all different colors.  Inactive players
        // won't count during this check.
        for (var i = 1; i <= 6; i++)
        {
            for (var j = 1; j <= 6; j++)
            {
                if ((Colors[i].Text == Colors[j].Text) && (i != j) && (!None[i].Checked) && (!None[j].Checked))
                {
                    MessageBox.Show("Each active player must have a unique color.");
                    return;
                }
            }
        }

        // Check to make sure we have valid player names.
        for (var i = 1; i <= 6; i++)
        {
            // Names can't be null.
            if ((PName[i].Text == "") && (!None[i].Checked))
            {
                MessageBox.Show("Each active player must have a name.");
                return;
            }
            // Names can't be long.
            if ((PName[i].Text.Length > 10) && (!None[i].Checked))
            {
                MessageBox.Show("Player names are limited to 10 characters.");
                return;
            }
            // Names can't have commas or quotes because it screws up the saved games.
            if ((PName[i].Text.Contains(",")) || (PName[i].Text.Contains("\"")))
            {
                MessageBox.Show("One or more player names contain invalid characters.");
                return;
            }
            // Names can't be the same.
            for (var j = 1; j <= 6; j++)
            {
                if ((i != j) && (PName[i].Text == PName[j].Text) && (!None[i].Checked) && (!None[j].Checked))
                {
                    MessageBox.Show("Player names must be unique.");
                    return;
                }
            }
        }

        // Copy the form settings into our permanent variables.
        for (var i = 1; i <= 6; i++)
        {
            // Put the proper color into the Player array.
            for (var j = 1; j <= 11; j++)
            {
                if (Colors[i].Text == PlayerColors[j])
                {
                    Player[i] = j;
                }
            }
            // Put the correct type into the type array.
            if (None[i].Checked) PlayerType[i] = PTYPE_INACTIVE;
            if (Human[i].Checked) PlayerType[i] = PTYPE_HUMAN;
            if (Computer[i].Checked) PlayerType[i] = PTYPE_COMPUTER;
            if (Network[i].Checked) PlayerType[i] = PTYPE_NETWORK;
            // Record the player name.
            PlayerName[i] = PName[i].Text;
            Land.Refs.Menu1st[i].Text = PName[i].Text;
            // Put the proper personality into the Personality array.
            for (var j = 1; j <= NUMPERSONALITIES; j++)
            {
                if (Personal[i].Text == PersonalityName[j])
                {
                    Personality[i] = j;
                }
            }
        }

        // Get the number of players.
        NumPlayers = Numpl;
        // Clean out the balls and waves arrays.
        ClearAllBalls();
        ClearAllWaves();

        // Make this settings form disappear.
        this.Close();
    }

    public static int CalcNumPlayers()
    {
        int i;
        int CalcNumPlayers = 0;
        for (i = 1; i <= MAX_PLAYERS; i++)
        {
            if (PlayerType[i] > PTYPE_INACTIVE)
            {
                CalcNumPlayers = CalcNumPlayers + 1;
            }
        }
        return CalcNumPlayers;
    }

    private void Color_KeyDown(object sender, KeyEventArgs e)
    {
        // Ignore key press
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    private void Color_KeyDown(object sender, KeyPressEventArgs e)
    {
        // Ignore key press
        e.Handled = true;
    }

    private void Color_TextUpdate(object sender, EventArgs e)
    {
        var combo = sender as ComboBox;
        if (combo != null && combo.SelectedItem != null)
        {
            combo.Text = combo.SelectedItem.ToString();
        }
        else if (combo != null && combo.Items.Count > 0)
        {
            combo.Text = combo.Items[0]!.ToString();
        }
    }


}
