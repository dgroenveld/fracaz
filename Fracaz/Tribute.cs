using static Fracaz.Declarations;
using static Fracaz.LoadAndSave;
using static Fracaz.NetworkPlay;
using static Fracaz.SoundFX;

namespace Fracaz;

public partial class Tribute : Form
{
    public Tribute()
    {
        InitializeComponent();
    }

    private void Form_Load(object sender, EventArgs e)
    {
        // Somebody won! Let's give them a salute.

        Application.DoEvents(); // Let the form come up clean...

        if (MyNetworkRole != NW_NONE)
        {
            // Turn off high scores if part of a network game.
            HiScoreButton.Enabled = false;
            // Also send out the final country configuration and stats if we're the server.
            if (MyNetworkRole == NW_SERVER)
            {
                SendNewCountryData();
                SendUpdatedStats();
            }
        }
        else
        {
            HiScoreButton.Enabled = true;
        }

        SNDApplause();

        // Let's get the exact turn of the person who won.
        int WinnerPl = 0;
        for (int i = 1; i <= 6; i++)
        {
            if (PlayerType[i] > PTYPE_INACTIVE && NumOccupied[i] > -1)
            {
                WinnerPl = i;
            }
        }

        Application.DoEvents();
        // Set up the big text.
        WinningPlayer.Text = PlayerName[WinnerPl] + " wins!";
        WinningPlayer.BackColor = Color.FromArgb(PlayerColorCodes[Player[WinnerPl]]);
        WinningPlayer.ForeColor = Color.FromArgb(PlayerTextColor[Player[WinnerPl]]);

        // Set up the sentencing.
        switch (Random.Shared.Next(1, 9))
        {
            case 1:
                WinningSentence.Text = "The people of " + PlayerName[WinnerPl] + " have chosen to spare the lives of " +
                                        "their enemies.  However, they will rot in prison for the next millenium!";
                break;
            case 2:
                WinningSentence.Text = PlayerName[WinnerPl] + " reigns supreme!  The rest of you upstarts had better " +
                                        "get back to your farmwork!";
                break;
            case 3:
                WinningSentence.Text = "As a gift on this glorious day of celebration, " + PlayerName[WinnerPl] + " has " +
                                        "decided not to behead the rest of you.";
                break;
            case 4:
                WinningSentence.Text = "The enemies of " + PlayerName[WinnerPl] + " have been found guilty of ... well ... " +
                                        "something ... and will be hung at sunup.";
                break;
            case 5:
                WinningSentence.Text = "The enemies of " + PlayerName[WinnerPl] + " will be made to pay for their crimes " +
                                        "against the new empire.  Long live " + PlayerName[WinnerPl] + "!";
                break;
            case 6:
                WinningSentence.Text = PlayerName[WinnerPl] + " has conquered the world!  " +
                                        "Well -- this little part of it, at least...";
                break;
            case 7:
                WinningSentence.Text = PlayerName[WinnerPl] + " has achieved demigod status by crushing the opposing " +
                                        "armies into submission.";
                break;
            case 8:
                WinningSentence.Text = PlayerName[WinnerPl] + " has implemented a dictatorship in the newly conquered lands.  " +
                                        "All bow before the new king!";
                break;
        }

        // Set up the humbling button.
        switch (Random.Shared.Next(1, 9))
        {
            case 1:
                WinningButton.Text = "Long live " + PlayerName[WinnerPl] + "!";
                break;
            case 2:
                WinningButton.Text = PlayerName[WinnerPl] + " reigns supreme!";
                break;
            case 3:
                WinningButton.Text = PlayerName[WinnerPl] + " is our master!";
                break;
            case 4:
                WinningButton.Text = "Until next time...";
                break;
            case 5:
                WinningButton.Text = "Click here to play again.";
                break;
            case 6:
                WinningButton.Text = "Three cheers for " + PlayerName[WinnerPl] + "!";
                break;
            case 7:
                WinningButton.Text = "Sleep with one eye open, " + PlayerName[WinnerPl] + "...";
                break;
            case 8:
                WinningButton.Text = "All hail " + PlayerName[WinnerPl] + "!";
                break;
        }

        // Now let's calculate the final scores and put this player in the
        // high score list, if they deserve it.
        StatScreen.CalculateScores();
        HiScores.AddPlayerToHighScoreList(WinnerPl);
    }

    private bool skipClosePrompt = false;

    private void Form_QueryUnload(object sender, FormClosingEventArgs e)
    {

        if (e.CloseReason == CloseReason.UserClosing && !skipClosePrompt)
        {
            // We're trying to close with the close gadget.
            DialogResult resp;
            // See if we've saved this map yet.  We want to save hi scores with the map.
            if (MyMap!.MapName != "")
            {
                // We've already saved the map, so update it.
                QuickMapUpdate();
                LastMapPath = MyMap.MapName;
                LastMapStamp = MyMap.MapStamp;
                Land.Refs.SetupMenusForGameOver();
                if (MyNetworkRole != NW_NONE)
                    NetworkForm.CancelBut_Click();
                GameMode = GM_BUILDING_TITLE;
                MyNetworkRole = NW_NONE;
            }
            else
            {
                // If we're configured to do so, prompt the user to save.
                if ((CFGPrompt == 1) && (MyNetworkRole == NW_NONE))
                {
                    // We haven't saved this map yet, ask the user if they want to.
                    resp = MessageBox.Show("This map has not been saved yet.  You must save the map\n" +
                                           "to keep a hi score list.  Would you like to save this map?",
                                           "Map Not Saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (resp == DialogResult.No)
                    {
                        LastMapPath = MyMap.MapName;
                        LastMapStamp = MyMap.MapStamp;
                        Land.Refs.SetupMenusForGameOver();
                        if (MyNetworkRole != NW_NONE)
                            NetworkForm.CancelBut_Click();
                        GameMode = GM_BUILDING_TITLE;
                        MyNetworkRole = NW_NONE;
                    }
                    else if (resp == DialogResult.Yes)
                    {
                        e.Cancel = true;
                        SaveMap();
                    }
                    else if (resp == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    // No prompt, so just leave.
                    LastMapPath = MyMap.MapName;
                    LastMapStamp = MyMap.MapStamp;
                    Land.Refs.SetupMenusForGameOver();
                    if (MyNetworkRole != NW_NONE)
                        NetworkForm.CancelBut_Click();
                    GameMode = GM_BUILDING_TITLE;
                    MyNetworkRole = NW_NONE;
                }
            }

        }
    }

    private void HiScoreButton_Click(object sender, EventArgs e)
    {
        var hiScores = new HiScores();
        hiScores.ShowDialog();
    }

    private void StatsButton_Click(object sender, EventArgs e)
    {
        var statScreen = new StatScreen();
        statScreen.ShowDialog();
    }

    private void WinningButton_Click(object sender, EventArgs e)
    {
        // See if we've saved this map yet.  We want to save hi scores with the map.
        if (MyMap!.MapName != "")
        {
            // We've already saved the map, so update it.
            QuickMapUpdate();
            LastMapPath = MyMap.MapName;
            LastMapStamp = MyMap.MapStamp;
            Land.Refs.SetupMenusForGameOver();
            if (MyNetworkRole != NW_NONE)
                NetworkForm.CancelBut_Click();
            GameMode = GM_BUILDING_TITLE;
            MyNetworkRole = NW_NONE;
            skipClosePrompt = true;
            this.Close();
        }
        else
        {
            // If we're configured to do so, prompt the user to save.
            if ((CFGPrompt == 1) && (MyNetworkRole == NW_NONE))
            {
                // We haven't saved this map yet, ask the user if they want to.
                var resp = MessageBox.Show("This map has not been saved yet.  You must save the map\n" +
                                           "to keep a hi score list.  Would you like to save this map?",
                                           "Map Not Saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (resp == DialogResult.No)
                {
                    LastMapPath = MyMap.MapName;
                    LastMapStamp = MyMap.MapStamp;
                    Land.Refs.SetupMenusForGameOver();
                    if (MyNetworkRole != NW_NONE)
                        NetworkForm.CancelBut_Click();
                    GameMode = GM_BUILDING_TITLE;
                    MyNetworkRole = NW_NONE;
                    skipClosePrompt = true;
                    this.Close();
                }
                else if (resp == DialogResult.Yes)
                {
                    SaveMap();
                }
            }
            else
            {
                // No prompt, so just leave.
                LastMapPath = MyMap.MapName;
                LastMapStamp = MyMap.MapStamp;
                Land.Refs.SetupMenusForGameOver();
                if (MyNetworkRole != NW_NONE)
                    NetworkForm.CancelBut_Click();
                GameMode = GM_BUILDING_TITLE;
                MyNetworkRole = NW_NONE;
                skipClosePrompt = true;
                this.Close();
            }
        }
    }
}
