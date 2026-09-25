using static Fracaz.Declarations;

namespace Fracaz;

public partial class StatScreen : Form
{
    public StatScreen()
    {
        InitializeComponent();

        for (int i = 0; i < 3; i++)
        {
            var col = new DataGridViewTextBoxColumn();
            col.HeaderText = "";
            col.Width = 40;
            this.FGtotals.Columns.Add(col);

            var col0 = new DataGridViewTextBoxColumn();
            col0.HeaderText = "";
            col0.Width = 60;
            this.FGrankings.Columns.Add(col0);
        }

        for (int i = 0; i < 7; i++)
        {
            var col = new DataGridViewTextBoxColumn();
            col.HeaderText = "";
            col.Width = 40;
            this.FGattacked.Columns.Add(col);

            var col2 = new DataGridViewTextBoxColumn();
            col2.HeaderText = "";
            col2.Width = 40;
            this.FGovertaken.Columns.Add(col2);

            var col3 = new DataGridViewTextBoxColumn();
            col3.HeaderText = "";
            col3.Width = 40;
            this.FGkilled.Columns.Add(col3);

            var col4 = new DataGridViewTextBoxColumn();
            col4.HeaderText = "";
            col4.Width = 40;
            this.FGdefeated.Columns.Add(col4);
        }

        this.FGattacked.Rows.Add(7);
        this.FGattacked.RowHeadersVisible = false;
        this.FGovertaken.Rows.Add(7);
        this.FGovertaken.RowHeadersVisible = false;
        this.FGkilled.Rows.Add(7);
        this.FGkilled.RowHeadersVisible = false;

        this.FGdefeated.Rows.Add(7);
        this.FGdefeated.RowHeadersVisible = false;
        this.FGtotals.Rows.Add(7);
        this.FGtotals.RowHeadersVisible = false;
        this.FGrankings.Rows.Add(7);
        this.FGrankings.RowHeadersVisible = false;

    }

    private void Form_Load(object sender, EventArgs e)
    {
        // Calculate everything fresh.
        CalculateScores();

        // Setup table data.
        FGattacked.Rows[0].Cells[0].Value = string.Empty;
        FGattacked.Columns[0].Width = 62;
        FGattacked.Rows[0].Cells[0].Style.BackColor = this.BackColor;
        FGovertaken.Rows[0].Cells[0].Value = string.Empty;
        FGovertaken.Columns[0].Width = 62;
        FGovertaken.Rows[0].Cells[0].Style.BackColor = this.BackColor;
        FGkilled.Rows[0].Cells[0].Value = string.Empty;
        FGkilled.Columns[0].Width = 62;
        FGkilled.Rows[0].Cells[0].Style.BackColor = this.BackColor;
        FGdefeated.Rows[0].Cells[0].Value = string.Empty;
        FGdefeated.Columns[0].Width = 64;
        FGdefeated.Rows[0].Cells[0].Style.BackColor = this.BackColor;
        FGtotals.Rows[0].Cells[0].Value = string.Empty;
        FGtotals.Columns[0].Width = 62;
        FGtotals.Rows[0].Cells[0].Style.BackColor = this.BackColor;
        FGrankings.Rows[0].Cells[0].Value = string.Empty;
        FGrankings.Columns[0].Width = 62;
        FGrankings.Rows[0].Cells[0].Style.BackColor = this.BackColor;
        // Set up player names, column widths, etc.
        for (int i = 1; i <= 6; i++)
        {
            FGattacked.Columns[i].Width = 43;
            FGovertaken.Columns[i].Width = 43;
            FGkilled.Columns[i].Width = 43;
            FGdefeated.Columns[i].Width = 20;

            if (i < 3)
            {
                FGtotals.Columns[i].Width = 43;
                FGrankings.Columns[i].Width = 43;
            }

            if (PlayerType[i] > PTYPE_INACTIVE)
            {

                FGattacked.Rows[i].Cells[0].Value = PlayerName[i];
                FGattacked.Rows[i].Cells[0].Style.BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);
                if (NumOccupied[i] < 0)
                {
                    for (int j = 0; j <= 6; j++)
                    {
                        FGattacked.Rows[i].Cells[j].Style.ForeColor = Color.Gray;
                    }
                }
                FGattacked.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);

                FGovertaken.Rows[i].Cells[0].Value = PlayerName[i];
                FGovertaken.Rows[i].Cells[0].Style.BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);
                if (NumOccupied[i] < 0)
                {
                    for (int j = 0; j <= 6; j++)
                    {
                        FGovertaken.Rows[i].Cells[j].Style.ForeColor = Color.Gray;
                    }
                }
                FGovertaken.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);

                FGkilled.Rows[i].Cells[0].Value = PlayerName[i];
                FGkilled.Rows[i].Cells[0].Style.BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);
                if (NumOccupied[i] < 0)
                {
                    for (int j = 0; j <= 6; j++)
                    {
                        FGkilled.Rows[i].Cells[j].Style.ForeColor = Color.Gray;
                    }
                }
                FGkilled.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);

                FGtotals.Rows[i].Cells[0].Value = PlayerName[i];
                FGtotals.Rows[i].Cells[0].Style.BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);
                if (NumOccupied[i] < 0)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        FGtotals.Rows[i].Cells[j].Style.ForeColor = Color.Gray;
                    }
                }

                FGrankings.Rows[i].Cells[0].Value = PlayerName[i];
                FGrankings.Rows[i].Cells[0].Style.BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);
                if (NumOccupied[i] < 0)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        FGrankings.Rows[i].Cells[j].Style.ForeColor = Color.Gray;
                    }
                }

                FGdefeated.Rows[i].Cells[0].Value = PlayerName[i];
                FGdefeated.Rows[i].Cells[0].Style.BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);
                if (NumOccupied[i] < 0)
                {
                    for (int j = 0; j <= 6; j++)
                    {
                        FGdefeated.Rows[i].Cells[j].Style.ForeColor = Color.Gray;
                    }
                }
                FGdefeated.Rows[0].Cells[i].Style.BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);

            }
            else
            {
                for (var j = 0; j <= 6; j++)
                {
                    FGattacked.Rows[j].Cells[i].Value = string.Empty;
                    FGattacked.Rows[j].Cells[i].Style.BackColor = this.BackColor;
                    FGattacked.Rows[i].Cells[j].Value = string.Empty;
                    FGattacked.Rows[i].Cells[j].Style.BackColor = this.BackColor;

                    FGovertaken.Rows[j].Cells[i].Value = string.Empty;
                    FGovertaken.Rows[j].Cells[i].Style.BackColor = this.BackColor;
                    FGovertaken.Rows[i].Cells[j].Value = string.Empty;
                    FGovertaken.Rows[i].Cells[j].Style.BackColor = this.BackColor;

                    FGkilled.Rows[j].Cells[i].Value = string.Empty;
                    FGkilled.Rows[j].Cells[i].Style.BackColor = this.BackColor;
                    FGkilled.Rows[i].Cells[j].Value = string.Empty;
                    FGkilled.Rows[i].Cells[j].Style.BackColor = this.BackColor;

                    FGdefeated.Rows[j].Cells[i].Value = string.Empty;
                    FGdefeated.Rows[j].Cells[i].Style.BackColor = this.BackColor;
                    FGdefeated.Rows[i].Cells[j].Value = string.Empty;
                    FGdefeated.Rows[i].Cells[j].Style.BackColor = this.BackColor;
                }

                FGtotals.Rows[i].Cells[0].Value = string.Empty;
                FGtotals.Rows[i].Cells[0].Style.BackColor = this.BackColor;
                FGtotals.Rows[i].Cells[1].Value = string.Empty;
                FGtotals.Rows[i].Cells[1].Style.BackColor = this.BackColor;
                FGtotals.Rows[i].Cells[2].Value = string.Empty;
                FGtotals.Rows[i].Cells[2].Style.BackColor = this.BackColor;
                if (i < 3)
                {
                    FGtotals.Rows[0].Cells[i].Style.BackColor = this.BackColor;
                }

                FGrankings.Rows[i].Cells[0].Value = string.Empty;
                FGrankings.Rows[i].Cells[0].Style.BackColor = this.BackColor;
                FGrankings.Rows[i].Cells[1].Value = string.Empty;
                FGrankings.Rows[i].Cells[1].Style.BackColor = this.BackColor;
                FGrankings.Rows[i].Cells[2].Value = string.Empty;
                FGrankings.Rows[i].Cells[2].Style.BackColor = this.BackColor;
                if (i < 3)
                {
                    FGrankings.Rows[0].Cells[i].Style.BackColor = this.BackColor;
                }

            }
            // Diagonals will have no data.
            FGattacked.Rows[i].Cells[i].Style.BackColor = this.BackColor;
            FGovertaken.Rows[i].Cells[i].Style.BackColor = this.BackColor;
            FGkilled.Rows[i].Cells[i].Style.BackColor = this.BackColor;
            FGdefeated.Rows[i].Cells[i].Style.BackColor = this.BackColor;
        }

        // Set up headings on some of the smaller tables.
        FGtotals.Rows[0].Cells[1].Value = "Countries";
        FGtotals.Rows[0].Cells[2].Value = "Troops";
        FGtotals.Columns[1].Width = 61;
        FGtotals.Columns[2].Width = 61;
        FGrankings.Rows[0].Cells[1].Value = "Score";
        FGrankings.Rows[0].Cells[2].Value = "Rank";
        FGrankings.Columns[1].Width = 61;
        FGrankings.Columns[2].Width = 61;

        // Assign the correct values from our statistics arrays.
        for (int i = 1; i <= 6; i++)
        {
            for (int j = 1; j <= 6; j++)
            {
                // Countries attacked.
                if (STATattacked[i, j] == 0)
                {
                    FGattacked.Rows[i].Cells[j].Value = string.Empty;
                }
                else
                {
                    FGattacked.Rows[i].Cells[j].Value = STATattacked[i, j].ToString();
                }

                // Countries overtaken.
                if (STATovertaken[i, j] == 0)
                {
                    FGovertaken.Rows[i].Cells[j].Value = string.Empty;
                }
                else
                {
                    FGovertaken.Rows[i].Cells[j].Value = STATovertaken[i, j].ToString();
                }

                // Troops killed.
                if (STATkilled[i, j] == 0)
                {
                    FGkilled.Rows[i].Cells[j].Value = string.Empty;
                }
                else
                {
                    FGkilled.Rows[i].Cells[j].Value = STATkilled[i, j].ToString();
                }

                // HQs defeated.
                if (i != j && PlayerType[i] > PTYPE_INACTIVE)
                {
                    switch (STATdefeated[i, j])
                    {
                        case -1:
                            FGdefeated.Rows[i].Cells[j].Value = "R";
                            break;
                        case 0:
                            FGdefeated.Rows[i].Cells[j].Value = string.Empty;
                            break;
                        case 1:
                            FGdefeated.Rows[i].Cells[j].Value = "X";
                            break;
                    }
                }
            }
            // Totals.
            if (STATcountries[i] <= 0)
            {
                FGtotals.Rows[i].Cells[1].Value = string.Empty;
            }
            else
            {
                FGtotals.Rows[i].Cells[1].Value = STATcountries[i].ToString();
            }
            if (STATtroops[i] <= 0)
            {
                FGtotals.Rows[i].Cells[2].Value = string.Empty;
            }
            else
            {
                FGtotals.Rows[i].Cells[2].Value = STATtroops[i].ToString();
            }
            // Scores
            if (STATtroops[i] <= 0)
            {
                // Not playing.
                FGrankings.Rows[i].Cells[1].Value = string.Empty;
                if (PlayerType[i] == PTYPE_INACTIVE)
                {
                    FGrankings.Rows[i].Cells[2].Value = string.Empty;
                }
                else
                {
                    FGrankings.Rows[i].Cells[2].Value = STATrank[i].ToString();
                }
            }
            else
            {
                FGrankings.Rows[i].Cells[1].Value = STATscore[i].ToString();
                if (STATrank[i] == 1)
                {
                    // This player is in first place!  Highlight.
                    FGrankings.Rows[i].Cells[2].Style.Font = new Font(FGrankings.Font, FontStyle.Bold);
                }
                else
                {
                    FGrankings.Rows[i].Cells[2].Style.Font = new Font(FGrankings.Font, FontStyle.Regular);
                }
                FGrankings.Rows[i].Cells[2].Value = STATrank[i].ToString();
                if (STATrank[i] == 1)
                {
                    // This player is in first place!  Highlight.
                    FGrankings.Rows[i].Cells[2].Style.Font = new Font(FGrankings.Font, FontStyle.Bold);
                }
                else
                {
                    FGrankings.Rows[i].Cells[2].Style.Font = new Font(FGrankings.Font, FontStyle.Regular);
                }
            }
        }
    }


    internal static void ResetAllStats()
    {
        // This sub completely zeroes out all statistical information.
        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            for (int j = 1; j <= MAX_PLAYERS; j++)
            {
                STATattacked[i, j] = 0;
                STATovertaken[i, j] = 0;
                STATkilled[i, j] = 0;
                STATdefeated[i, j] = 0;
            }
            STATscore[i] = 0;
        }
    }

    internal static void UpdateAttackedStats(int Attacker, int Attackee)
    {
        // This sub increments the Country Attacked stat for the passed players.
        STATattacked[Attacker, Attackee] = STATattacked[Attacker, Attackee] + 1;
    }

    internal static void UpdateOvertakenStats(int Attacker, int Attackee)
    {
        // This sub increments the Country Overtaken stat for the passed players.
        STATovertaken[Attacker, Attackee] = STATovertaken[Attacker, Attackee] + 1;
    }

    internal static void UpdateKilledStats(int Attacker, int Attackee, int Amount)
    {
        // This sub increments the Troops Killed stat for the passed players.
        STATkilled[Attacker, Attackee] += Amount;
    }

    internal static void UpdateDefeatedStats(int Attacker, int Attackee)
    {
        // This sub increments the HQ Overtaken stat for the passed players.
        if (Attacker != Attackee)
        {
            STATdefeated[Attacker, Attackee] = STATdefeated[Attacker, Attackee] + 1;
        }
    }

    internal static void UpdateResignedStats(int Resignee)
    {
        // This sub increments the HQ Overtaken stat for the passed players.

        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            if (i != Resignee)
            {
                STATdefeated[i, Resignee] = -1;
            }
        }
    }

    private void OKbutt_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    internal static void CalculateScores()
    {
        int[] AttackedSubTotal = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
        int[] OvertakenSubTotal = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
        int[] KilledSubTotal = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
        int[] DefeatedSubTotal = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
        int[] ResignedSubTotal = new int[MAX_PLAYERS + ONE_ARRAY_FIX];

        int[] TempPlayerNum = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
        int[] TempScore = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
        bool RankChanged;

        NumPlayers = Players.CalcNumPlayers();
        int AttackedTotal = 0;
        int OvertakenTotal = 0;
        int KilledTotal = 0;
        int CountryTotal = 0;
        int TroopTotal = 0;
        // First, calculate everyone's score.
        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            AttackedSubTotal[i] = 0;
            OvertakenSubTotal[i] = 0;
            KilledSubTotal[i] = 0;
            DefeatedSubTotal[i] = 0;
            ResignedSubTotal[i] = 0;
            STATcountries[i] = 0;
            STATtroops[i] = 0;
            // Calculate this player's score for warfare.
            for (int j = 1; j <= MAX_PLAYERS; j++)
            {
                AttackedSubTotal[i] += STATattacked[i, j];
                OvertakenSubTotal[i] += STATovertaken[i, j];
                KilledSubTotal[i] += STATkilled[i, j];
                if (STATdefeated[i, j] == 1)
                {
                    DefeatedSubTotal[i] += 1;
                }
                else if (STATdefeated[i, j] == -1)
                {
                    ResignedSubTotal[i] += 1;
                }
            }
            // We get the troop and country subtotals by looking at the MAP.
            for (int k = 1; k <= MyMap!.NumberOfCountries; k++)
            {
                if (MyMap.Owner(k) == i)
                {
                    // This player owns this country.  Count it for scoring.
                    // Note that unowned countries don't contribute in any way to score.
                    STATcountries[i] += 1;
                    CountryTotal += 1;
                    STATtroops[i] += MyMap.TroopCount(k);
                    TroopTotal += MyMap.TroopCount(k);
                }
            }
        }

        // Now get absolute totals for each value.
        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            AttackedTotal += AttackedSubTotal[i];
            OvertakenTotal += OvertakenSubTotal[i];
            KilledTotal += KilledSubTotal[i];
        }

        // Now divide to get a score for each between 0 and 100.  There are 100
        // points to distribute in each category!
        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            if (PlayerType[i] == PTYPE_INACTIVE)
            {
                // Not playing - no score.
                STATscore[i] = -1;
            }
            else
            {
                STATscore[i] = 0;
                if (AttackedTotal > 0)
                {
                    STATscore[i] += (100 * AttackedSubTotal[i] / AttackedTotal);
                }
                if (OvertakenTotal > 0)
                {
                    STATscore[i] += (100 * OvertakenSubTotal[i] / OvertakenTotal);
                }
                if (KilledTotal > 0)
                {
                    STATscore[i] += (100 * KilledSubTotal[i] / KilledTotal);
                }
                // Jason -- redo these, look at map variables.
                if (CountryTotal > 0)
                {
                    STATscore[i] += (100 * STATcountries[i] / CountryTotal);
                }
                if (TroopTotal > 0)
                {
                    STATscore[i] += (100 * STATtroops[i] / TroopTotal);
                }
                // Determine how much each HQ is worth.  There are 500 points unaccounted for
                // out of the 1000, so we need to do some division.
                // Thus, with 2 players, one of them will get the whole 500.
                // With 6 players, each of the other five is worth 100.
                float PointsPerHQ = 500f / (NumPlayers - 1);
                // Add in points per HQ they've killed...
                STATscore[i] = (int)(STATscore[i] + (PointsPerHQ * DefeatedSubTotal[i]));
                // Plus points for someone resigning...
                STATscore[i] = (int)(STATscore[i] + ((PointsPerHQ / (NumPlayers - 1)) * ResignedSubTotal[i]));
            }
        }

        // Now order the scores and assign ranks.
        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            TempPlayerNum[i] = i;
            TempScore[i] = STATscore[i];
        }

        // Bubble sort the six.
        do
        {
            RankChanged = false;
            for (int i = 1; i <= (MAX_PLAYERS - 1); i++)
            {
                if (TempScore[i] < TempScore[i + 1])
                {
                    // This score is less than the one below it, so swap the two.
                    int k = TempScore[i];
                    TempScore[i] = TempScore[i + 1];
                    TempScore[i + 1] = k;
                    // Swap the score *and* this player's number.
                    int j = TempPlayerNum[i];
                    TempPlayerNum[i] = TempPlayerNum[i + 1];
                    TempPlayerNum[i + 1] = j;
                    // Mark as changed so we do another iteration.
                    RankChanged = true;
                }
            }
        } while (RankChanged == true);

        // Now determine rank according to the player number order.
        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            STATrank[TempPlayerNum[i]] = i;
        }

        // If there is a tie, then two players are tied at the same rank.
        for (int i = 2; i <= MAX_PLAYERS; i++)
        {
            if (TempScore[i] == TempScore[i - 1])
            {
                // This player is tied with the person before them.
                STATrank[TempPlayerNum[i]] = STATrank[TempPlayerNum[i - 1]];
            }
        }
    }
}
