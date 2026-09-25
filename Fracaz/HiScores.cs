using static Fracaz.Declarations;

namespace Fracaz;

public partial class HiScores : Form
{
    public HiScores()
    {
        InitializeComponent();
    }

    internal static void ClearHiScores()
    {
        // This sub simply sets up the 'default' hi score list.
        string[] DefaultName = new string[10 + ONE_ARRAY_FIX];
        string[] Order = new string[10 + ONE_ARRAY_FIX];

        DefaultName[1] = "Bowie";
        DefaultName[2] = "BooBoo";
        DefaultName[3] = "Pinky";
        DefaultName[4] = "Zack";
        DefaultName[5] = "Smudge";
        DefaultName[6] = "Ozzie";
        DefaultName[7] = "Mac";
        DefaultName[8] = "Penny";
        DefaultName[9] = "Lady";
        DefaultName[10] = "Queenie";

        // Set up the order array..
        for (int i = 1; i <= 10; i++)
        {
            Order[i] = i.ToString();
        }

        // And randomize it so 1-10 appear in a random order.
        for (int i = 1; i <= 20; i++)
        {
            int j, l;
            string k;
            do
            {
                j = Random.Shared.Next(1, 11);
                l = Random.Shared.Next(1, 11);
            } while (j == l);
            k = Order[j];
            Order[j] = Order[l];
            Order[l] = k;
        }

        // Now set up the hi scores!
        for (int i = 1; i <= 10; i++)
        {
            HiScoreName[i] = DefaultName[int.Parse(Order[i])];
            HiScore[i] = ((10 - i) * 10) + (Random.Shared.Next(1, 10));
            HiScoreColor[i] = 0;
        }
    }

    internal static void AddPlayerToHighScoreList(int LuckyGuy)
    {
        // This sub adds the passed player's name and score to the high score list.
        // If they don't belong there, this sub does nothing.
        for (var i = 1; i <= 10; i++)
        {
            if (STATscore[LuckyGuy] > HiScore[i])
            {
                // The player belongs at position i.
                // First, move everyone from here down a notch.
                if (i < 10)
                {
                    // Don't bother if it's the last one.
                    for (var j = 9; j >= i; j--)
                    {
                        HiScore[j + 1] = HiScore[j];
                        HiScoreName[j + 1] = HiScoreName[j];
                        HiScoreColor[j + 1] = HiScoreColor[j];
                    }
                }
                // Now put us in its place.
                HiScore[i] = STATscore[LuckyGuy];
                HiScoreName[i] = PlayerName[LuckyGuy];
                HiScoreColor[i] = Player[LuckyGuy];
                break;
            }
        }
    }

    private void Form_Load(object sender, EventArgs e)
    {
        Label?[] HiNamez = { null, HiNamez1, HiNamez2, HiNamez3, HiNamez4, HiNamez5, HiNamez6, HiNamez7, HiNamez8, HiNamez9, HiNamez10 };

        Label?[] HiScorez = { null, HiScorez1, HiScorez2, HiScorez3, HiScorez4, HiScorez5, HiScorez6, HiScorez7, HiScorez8, HiScorez9, HiScorez10 };

        // Someone wants to view the high score list!
        // We only get here during a game, so our scores *must* exist.

        for (int i = 1; i <= NUM_HI_SCORES; i++)
        {
            HiNamez[i]!.Text = HiScoreName[i];
            HiScorez[i]!.Text = HiScore[i].ToString();

            if (HiScoreColor[i] == 0)
            {
                HiNamez[i]!.BackColor = this.BackColor;
            }
            else
            {
                HiNamez[i]!.BackColor =
Color.FromArgb((int)PlayerColorCodes[HiScoreColor[i]]);
            }
        }
    }

    private void OkBootie_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
