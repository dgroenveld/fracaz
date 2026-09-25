using Fracaz.Helpers;
using static Fracaz.Boom;
using static Fracaz.ComputerAI;
using static Fracaz.Declarations;
using static Fracaz.GraphicFX;
using static Fracaz.Helpers.ColorHelper;
using static Fracaz.LoadAndSave;
using static Fracaz.NameGen;
using static Fracaz.NetworkPlay;
using static Fracaz.Play;
using static Fracaz.RandomEvents;
using static Fracaz.SoundFX;

namespace Fracaz;

public partial class Land : Form
{
    private ToolStripMenuItem[] MenuBonus = new ToolStripMenuItem[6];
    private ToolStripMenuItem[] MenuInitTroops = new ToolStripMenuItem[5];
    private ToolStripMenuItem[] MenuConquer = new ToolStripMenuItem[5];
    private ToolStripMenuItem[] MenuShips = new ToolStripMenuItem[4];
    private ToolStripMenuItem[] MenuPorts = new ToolStripMenuItem[4];
    private ToolStripMenuItem[] MenuRandom = new ToolStripMenuItem[3];
    private ToolStripMenuItem[] MenuInitTroopCts = new ToolStripMenuItem[3];
    private ToolStripMenuItem[] MenuHQSelect = new ToolStripMenuItem[2];
    private ToolStripMenuItem[] Menu1st = new ToolStripMenuItem[6];

    private ToolStripMenuItem[] MenuSize = new ToolStripMenuItem[6];
    private ToolStripMenuItem[] MenuProp = new ToolStripMenuItem[3];
    private ToolStripMenuItem[] MenuShape = new ToolStripMenuItem[3];
    private ToolStripMenuItem[] MenuLakeSize = new ToolStripMenuItem[4];
    private ToolStripMenuItem[] MenuPct = new ToolStripMenuItem[5];
    private ToolStripMenuItem[] MenuIslands = new ToolStripMenuItem[3];
    private ToolStripMenuItem[] MenuResolution = new ToolStripMenuItem[3];
    private ToolStripMenuItem[] MenuBorders = new ToolStripMenuItem[3];
    private ToolStripMenuItem[] MenuSfx = new ToolStripMenuItem[2];
    private ToolStripMenuItem[] MenuAISpeed = new ToolStripMenuItem[4];

    private Bitmap Ocean = new Bitmap(1, 1);
    public static Bitmap MapBuffer = new Bitmap(1, 1);
    public static Bitmap PicBuffer = new Bitmap(1, 1);
    public static Bitmap LandMap = new Bitmap(Properties.Resources.Landmap);
    public static Bitmap WaterPic = new Bitmap(Properties.Resources.Water);
    public static Bitmap BallPic = new Bitmap(Properties.Resources.Ball);

    private static DateTime CadenceTime = DateTime.UtcNow;
    private int CfgNumCountries;

    private int Turn;
    private int Phase;

    private bool NetworkArbitrationInProgress;

    private Label[] PlyrNames = new Label[MAX_PLAYERS + 1];
    private Label[] PlyrTotals = new Label[MAX_PLAYERS + 1];

    private static Land? Me { get; set; }
    public static class Refs
    {
        public static System.Windows.Forms.Timer BallTimer => Me!.BallTimer;
        public static ToolStripMenuItem[] Menu1st => Me!.Menu1st;
        public static ToolStripMenuItem[] MenuResolution => Me!.MenuResolution;
        public static ToolStripMenuItem[] MenuProp => Me!.MenuProp;
        public static ToolStripMenuItem[] MenuShape => Me!.MenuShape;
        public static ToolStripMenuItem[] MenuIslands => Me!.MenuIslands;
        public static ToolStripMenuItem[] MenuLakeSize => Me!.MenuLakeSize;
        public static ToolStripMenuItem[] MenuPct => Me!.MenuPct;
        public static ToolStripMenuItem[] MenuSize => Me!.MenuSize;
        public static ToolStripMenuItem[] MenuBorders => Me!.MenuBorders;
        public static ToolStripMenuItem[] MenuSfx => Me!.MenuSfx;
        public static Action ChangeRes => Me!.ChangeRes;
        public static Action<bool> CollectMenuSettings => Me!.CollectMenuSettings;
        public static Label Commentary => Me!.Commentary;
        public static Label Oopsie => Me!.Oopsie;
        public static Action SetupIndicators => Me!.SetupIndicators;
        public static Action UpdateMessages => Me!.UpdateMessages;
        public static Action<int> SetPhase => Me!.SetPhase;
        public static Action<int> SetTurn => Me!.SetTurn;
        public static Action SetUpPlayerControls => Me!.SetUpPlayerControls;
        public static TextBox TroopMoveNum => Me!.TroopMoveNum;
        public static ToolStripMenuItem[] MenuAISpeed => Me!.MenuAISpeed;
        public static ToolStripMenuItem MenuSaveGame => Me!.MenuSaveGame;
        public static ToolStripMenuItem[] MenuBonus => Me!.MenuBonus;
        public static ToolStripMenuItem[] MenuInitTroops => Me!.MenuInitTroops;
        public static ToolStripMenuItem[] MenuConquer => Me!.MenuConquer;
        public static ToolStripMenuItem[] MenuShips => Me!.MenuShips;
        public static ToolStripMenuItem[] MenuInitTroopCts => Me!.MenuInitTroopCts;
        public static ToolStripMenuItem[] MenuPorts => Me!.MenuPorts;
        public static ToolStripMenuItem[] MenuRandom => Me!.MenuRandom;
        public static ToolStripMenuItem[] MenuHQSelect => Me!.MenuHQSelect;
        public static Action<int, int> Explode => Me!.Explode;
        public static Action RedrawScreen => Me!.RedrawScreen;
        public static Label CntryName => Me!.CntryName;
        public static Action CalculateTotals => Me!.CalculateTotals;
        public static Action SetupMenusForGameOver => Me!.SetupMenusForGameOver;
        public static ToolStripMenuItem MenuNew => Me!.MenuNew;
        public static ToolStripMenuItem MenuJoinGame => Me!.MenuJoinGame;
        public static ToolStripMenuItem MenuSame => Me!.MenuSame;
        public static ToolStripMenuItem MenuLoad => Me!.MenuLoad;
        public static ToolStripMenuItem MenuLoadSavedGame => Me!.MenuLoadSavedGame;
        public static ToolStripMenuItem MenuGetOptionsFromMapFile => Me!.MenuGetOptionsFromMapFile;
        public static Func<int> GetTurn => Me!.GetTurn;
        public static Func<int> GetPhase => Me!.GetPhase;
        public static Action<int> ResignProc => Me!.ResignProc;
    }

    public Land()
    {
        InitializeComponent();

        Me = this;

        this.DoubleBuffered = true;

        MenuSize = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuCountrySize);
        MenuProp = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuCountrySizeProp);
        MenuShape = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuCountryShapes);
        MenuLakeSize = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuMinLakeSize);
        MenuPct = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuGlobal);
        MenuIslands = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuIsle);
        MenuResolution = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuRes);

        MenuAISpeed = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuComputerSpeed,

            item => CFGAISpeed = Array.IndexOf(MenuAISpeed, item)
            );

        MenuBorders = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuBorderChoice,

            item =>
            {
                CFGBorders = Array.IndexOf(MenuBorders, item);

                // Changed a draw setting, let's redraw!
                RedrawScreen();
            }
            );

        MenuSfx = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuSfxChoice,

            item =>
            {
                CFGSound = Array.IndexOf(MenuSfx, item);

                // Make this change immediately!
                CollectDrawSettings();
            }
            );

        MenuBonus = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuBonusChoice,
            item =>
            {
                CFGBonusTroops = Array.IndexOf(MenuBonus, item);

                // Disable no starting troops if we have no bonus troops.
                if (CFGBonusTroops == 0)
                {
                    MenuInitTroops[1].Enabled = false;
                }
                else
                {
                    MenuInitTroops[1].Enabled = true;
                }
            },
            indexModifier: 0);
        MenuInitTroops = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuInitTroopsChoice,
            item =>
            {
                CFGInitTroopPl = Array.IndexOf(MenuInitTroops, item);

                // Disable no bonus troops if we start with none.
                if (CFGInitTroopPl == 1)
                {
                    MenuBonus[0].Enabled = false;
                }
                else
                {
                    MenuBonus[0].Enabled = true;
                }
            }
            );

        MenuConquer = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuConquerChoice);
        MenuShips = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuShipsChoice);
        MenuPorts = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuPortsChoice);
        MenuRandom = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuRandomChoice);
        MenuInitTroopCts = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuInitTroopCtsChoice);
        MenuHQSelect = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuHQSelectChoice);
        Menu1st = ToolStripMenuItems.CreateToolStripMenuItemArray(this.MenuFirst);

        PlyrNames = new[]{
            null!,
            this.PlyrNames1,
            this.PlyrNames2,
            this.PlyrNames3,
            this.PlyrNames4,
            this.PlyrNames5,
            this.PlyrNames6
        };

        PlyrTotals = new[]{
            null!,
            this.PlyrTotals1,
            this.PlyrTotals2,
            this.PlyrTotals3,
            this.PlyrTotals4,
            this.PlyrTotals5,
            this.PlyrTotals6
        };
    }

    private int GetTurn()
    {
        return this.Turn;
    }

    private void SetTurn(int Val)
    {
        this.Turn = Val;
    }

    private int GetPhase()
    {
        return this.Phase;
    }

    private void SetPhase(int Val)
    {
        this.Phase = Val;
    }

    private void Form_Load(object sender, EventArgs e)
    {
        MapMade = false;
        GameMode = GM_BUILDING_TITLE;
        MyNetworkRole = NW_NONE;

        SetupPaths();
        DetermineMaxRes();

        // No balls currently onscreen.
        // Clean out the balls and waves arrays.
        ClearAllBalls();
        ClearAllWaves();

        // Set up water colors array.
        WaterColors[1] = 0xEF1010;
        WaterColors[2] = 0xFF2020;
        WaterColors[3] = 0xFF4040;
        WaterColors[4] = 0xFF0000;
        WaterColors[5] = 0xE00000;
        WaterColors[6] = 0xD00000;

        // Initialize a bunch of stuff.
        InitPlayerData();
        InitNameStuff();
        InitPersonalities();

        // Collect all menu settings.
        CollectMapSettings(false);
        CollectMenuSettings(false);
        CollectDrawSettings();

        // New in 2.0 BETA: an .ini file which contains all of our config parameters.
        // Check it now, and if it's there, overwrite settings with what's in there.
        ReadINI(INIpath);
        FixResolution();
        SetupMenusForGameOver();

        // Create a new ocean pic.
        CreateOcean();
    }

    private void Form_QueryUnload(object sender, FormClosingEventArgs e)
    {
        // No matter who kills us or how we die, we need to try to gracefully
        // kill our network connection.
        if (MyNetworkRole == NW_CLIENT || MyNetworkRole == NW_SERVER)
        {
            NetworkForm.CancelBut_Click();
        }
    }

    private void MenuAbortGame_Click(object sender, EventArgs e)
    {
        if (GameMode != GM_GAME_ACTIVE) return;

        string AbortStr = "Are you sure you want to abort the game?";

        if (MyNetworkRole == NW_SERVER)
        {
            AbortStr += "\n\nWARNING! You are the host of a networked game.\nAborting will end the game for all networked players.";
        }
        else if (MyNetworkRole == NW_CLIENT)
        {
            AbortStr += "\n\nWARNING! You are currently part of a networked game.\nYou will not be able to enter this game again.";
        }

        var resp = MessageBox.Show(AbortStr, "Abort Game", MessageBoxButtons.YesNo);

        if (resp == DialogResult.No)
        {
            return;
        }

        if (MyNetworkRole == NW_CLIENT || MyNetworkRole == NW_SERVER)
        {
            NetworkForm.CancelBut_Click();
        }

        LastMapPath = MyMap!.MapName;
        LastMapStamp = MyMap.MapStamp;
        GameMode = GM_BUILDING_TITLE;
        MyNetworkRole = NW_NONE;
        SetupMenusForGameOver();
        Commentary.Text = "Game aborted.";
        Oopsie.Text = "";
        this.Refresh();
        DrawMap();
    }

    private int SetupNetwork()
    {

        // This function is called when this workstation starts the game, thus
        // making it the server.  We only continue when the network is squared away.

        NetworkState = NS_WAITING_TO_START; // Something other than NS_IDLE.
        if (NumNetworkPlayers() > 0)
        {
            // We do in fact have network players.  Connect with them.
            MyNetworkRole = NW_SERVER;
            GameMode = GM_DIALOG_OPEN;
            NetworkForm.ShowMe(this);
            GameMode = GM_TITLE_SCREEN;
        }

        return NetworkState;
    }

    private void MenuJoinGame_Click(object sender, EventArgs e)
    {
        MyNetworkRole = NW_CLIENT;
        NetworkState = NS_IDLE;
        GameMode = GM_DIALOG_OPEN;
        NetworkForm.ShowMe(this);
        GameMode = GM_TITLE_SCREEN;
        if (NetworkState == NS_IDLE)
        {
            return;
        }

        // Other initialization goes here.
        Commentary.BackColor = this.BackColor;
        Commentary.ForeColor = Color.Black;
        SetupMenusForInGame();

        for (int i = 1; i <= 6; i++)
        {
            //Stop players from making moves!
            NumOccupied[i] = -1;
        }

        TurnCounter = 0;

        //Clean out the balls and waves arrays.
        ClearAllBalls();
        ClearAllWaves();

        //Reset stats.  No high scores for net game.
        StatScreen.ResetAllStats();

        InitGameStuff();
    }

    private void StartNetworkGame()
    {
        // At this point, our map is totally created, HQs have been picked if
        // necessary, and the starting player has been determined.  Now we need
        // to send *all* of this information to the other players if we're a server.
        if (MyNetworkRole == NW_SERVER)
        {
            SendStartupDataToAll();
            GameMode = GM_DIALOG_OPEN;
            NetworkForm.ShowMe(this);
            GameMode = GM_GAME_ACTIVE;
        }

    }

    private void MenuNew_Click(object sender, EventArgs e)
    {

        if (SetupNetwork() == NS_IDLE)
        {
            return;
        }

        Commentary.BackColor = this.BackColor;
        Commentary.ForeColor = Color.Black;
        GameMode = GM_BUILDING_MAP;
        SetupMenusForInGame();

        for (int i = 1; i <= 6; i++)
        {
            // Stop players from making moves!
            NumOccupied[i] = -1;
        }

        TurnCounter = 0;
        MyMap = new Map();
        SetupForm();

        // Collect all menu settings.
        CollectMapSettings(true);
        CollectMenuSettings(true);
        CollectDrawSettings();

        // Clean out the balls and waves arrays.
        ClearAllBalls();
        ClearAllWaves();

        // Reset stats and such.
        HiScores.ClearHiScores();
        StatScreen.ResetAllStats();

        // This is the syntax for building a map.
        MyMap.CreateMap(CfgNumCountries, MaxCountrySize, MinLakeSize, LandPct, PropPct, ShapePct, CoastPctKeep, IslePctKeep);

        InitGameStuff();

        StartNetworkGame();
    }

    private void MenuSame_Click(object sender, EventArgs e)
    {
        if (LastMapPath == "") return;   //No last map.

        for (int i = 1; i <= 6; i++)
        {
            //Stop players from making moves!
            NumOccupied[i] = -1;
        }

        string Oldstr = Commentary.Text;
        Commentary.Text = "Loading map...";
        Application.DoEvents();

        //Load the last map, no dialog, no nothin'.
        int Doit = ReadMapDataKitchenSink(LastMapPath, false);

        if (Doit == 0)
        {
            //We had an error.  Leave.
            Commentary.Text = Oldstr;
            return;
        }

        if (SetupNetwork() == NS_IDLE) return;

        CfgNumCountries = MyMap!.NumberOfCountries;

        TurnCounter = 0;

        Commentary.BackColor = Land.DefaultBackColor;
        Commentary.ForeColor = Color.FromArgb((int)vbBlack);
        GameMode = GM_BUILDING_MAP;
        SetupMenusForInGame();

        //Collect menu settings.
        CollectMenuSettings(true);
        CollectDrawSettings();

        //Clean out the balls and waves arrays.
        ClearAllBalls();
        ClearAllWaves();

        //Reset stats and such.
        StatScreen.ResetAllStats();

        InitGameStuff();

        StartNetworkGame();
    }

    private void MenuGetOptionsFromMapFile_Click(object sender, EventArgs e)
    {
        LoadMap(true);
    }

    private void MenuChat_Click(object sender, EventArgs e)
    {
        var chatForm = new ChatForm();
        chatForm.Show(this);
    }

    private void MenuLoad_Click(object sender, EventArgs e)
    {
        for (int i = 1; i <= 6; i++)
        {
            // Stop players from making moves!
            NumOccupied[i] = -1;
        }

        string Oldstr = Commentary.Text;
        Commentary.Text = "Loading map...";

        Application.DoEvents();

        // This is where we actually load the map.
        int Doit = LoadMap(false);

        if (Doit == 0)
        {
            // We canceled the load dialog or had an error. Leave.
            Commentary.Text = Oldstr;
            return;
        }

        if (SetupNetwork() == NS_IDLE) return;

        CfgNumCountries = MyMap!.NumberOfCountries;

        TurnCounter = 0;

        Commentary.BackColor = Land.DefaultBackColor;
        Commentary.ForeColor = Color.FromArgb((int)vbBlack);
        GameMode = GM_BUILDING_MAP;
        SetupMenusForInGame();

        // Collect menu settings.
        CollectMenuSettings(true);
        CollectDrawSettings();

        // Clean out the balls and waves arrays.
        ClearAllBalls();
        ClearAllWaves();

        // Reset stats and such.
        StatScreen.ResetAllStats();

        InitGameStuff();

        StartNetworkGame();
    }

    private void MenuLoadSavedGame_Click(object sender, EventArgs e)
    {
        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            // Stop players from making moves!
            NumOccupied[i] = -1;
        }

        string Oldstr = Commentary.Text;
        Commentary.Text = "Loading game...";
        Commentary.Refresh();

        // This is where we actually load the map.
        int Doit = LoadGame();

        if (Doit == 0)
        {
            // We canceled the load dialog or had an error.  Leave.
            Commentary.Text = Oldstr;
            return;
        }

        CfgNumCountries = MyMap!.NumberOfCountries;
        Commentary.BackColor = this.BackColor;
        Commentary.ForeColor = Color.FromArgb((int)vbBlack);
        GameMode = GM_BUILDING_MAP;
        SetupMenusForInGame();

        // Collect menu settings.
        CollectMenuSettings(true);
        CollectDrawSettings();

        // Clean out the balls and waves arrays.
        ClearAllBalls();
        ClearAllWaves();

        // Clear the previous display.
        DrawBkg();

        // Reset our anti-double-clickin' flags.
        AlreadyAddedTroopsThisPhase = false;
        AlreadyPerformedActionThisPhase = false;
        AlreadyMovedTroopsThisPhase = false;
        AlreadyChoseTroopsThisPhase = false;
        AlreadyCheckedForMaxedOutCountries = false;
        AlreadyPassedThisPhase = false;

        // Set up the onscreen indicators.
        SetupIndicators();

        MapMade = true;
        CheckedForEvent = false;
        EventInProgress = false;

        // Draw the map.
        DrawMap();

        // Set up first message.
        UpdateMessages();
        this.Refresh();

        GameMode = GM_GAME_ACTIVE;
    }

    private void InitGameStuff()
    {
        // Called when a new game is about to begin.
        NetworkArbitrationInProgress = true;

        // Clear the previous display.
        DrawBkg();

        // Reset all game data.
        ResetGameData();

        // Set up initial troop positions based on menu settings.
        if (MyNetworkRole == NW_CLIENT)
        {
            CalculateTotals();
        }
        else
        {
            PlaceTroops();
        }

        // Set up the onscreen indicators.
        SetupIndicators();
        CntryName.Visible = false;
        AttackTxt.Visible = false;
        AttackTot.Visible = false;
        AttackLnd.Visible = false;
        AttackWtr.Visible = false;
        DefendTxt.Visible = false;
        DefendTot.Visible = false;
        DefendLnd.Visible = false;
        DefendWtr.Visible = false;

        MapMade = true;
        CheckedForEvent = false;
        EventInProgress = false;
        // Kick it off.
        GameMode = GM_GAME_ACTIVE;

        // Draw the map.
        DrawMap();

        // Set up initial HQs if we need to.
        if (MyNetworkRole != NW_CLIENT)
        {
            AutoHQSelection();
        }

        // Set up the initial messages.
        UpdateMessages();
        this.Refresh();

        NetworkArbitrationInProgress = false;
    }

    private void MenuSave_Click(object sender, EventArgs e)
    {
        // Save the current map.
        SaveMap();
    }

    private void MenuSaveGame_Click(object sender, EventArgs e)
    {
        // Save the current game.
        if (MyMap!.MapName != "")
        {
            SaveGame(Turn, Phase);
        }
        else
        {
            MessageBox.Show("You must save this map before you can save the game.", "Map Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

    }

    private void MenuHiScores_Click(object sender, EventArgs e)
    {
        // Display the Hi Scores dialog, if we're in a game...
        // ...or the ones for the last game.

        var TempMode = GameMode;

        GameMode = GM_DIALOG_OPEN;
        var hiScores = new HiScores();
        hiScores.ShowDialog(this);
        GameMode = TempMode; // Restore the old mode.
    }

    private void MenuStats_Click(object sender, EventArgs e)
    {
        // Display the Statistics dialog, if we're in a game.
        if (GameMode != GM_GAME_ACTIVE) return;

        var TempMode = GameMode;

        GameMode = GM_DIALOG_OPEN;
        var statScreen = new StatScreen();
        statScreen.ShowDialog(this);
        GameMode = TempMode; // Restore the old mode.
    }

    private void Form_KeyDown(object sender, KeyEventArgs e)
    {
        // We want to get out at any time by pressing ESC.
        if (e.KeyCode == Keys.Escape)
        {
            MenuExit_Click(sender, e);
        }
    }

    private static bool BallTimerRunning;

    private void BallTimer_Timer(object sender, EventArgs e)
    {
        if (BallTimerRunning) return;

        BallTimerRunning = true;
        try
        {
            BallTimer_Timer_Tick(sender, e);
        }
        finally
        {
            BallTimerRunning = false;
        }
    }

    private void BallTimer_Timer_Tick(object sender, EventArgs e)
    {
        // This sub animates the balls.
        if (GameMode != GM_GAME_ACTIVE && GameMode != GM_TITLE_SCREEN && GameMode != GM_DIALOG_OPEN)
        {
            return;
        }

        BallTimerSub(); // The BallTimerSub is in the Boom.bas module.

        this.InvokePaint(this, new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
    }

    private void AutoHQSelection()
    {
        int CurrentPick;
        int RoundAbout;

        // See if we need to quickly pick HQs for everyone (automatic HQ select).

        if (CFGHQSelect == 2)
        {
            UpdateMessages();
            this.Refresh();

            // It's the first round and we need to grab an HQ fast.
            if (CFG1st == 7)
            {
                CurrentPick = Random.Shared.Next(1, 7);
            }
            else
            {
                CurrentPick = CFG1st;
            }

            RoundAbout = CurrentPick;

            bool Done = false;

            do
            {
                if (PlayerType[CurrentPick] > PTYPE_INACTIVE)
                {
                    AIChooseHQ(true, CurrentPick, 1);
                }

                CurrentPick++;

                if (CurrentPick == 7) CurrentPick = 0;
                if (CurrentPick == RoundAbout) Done = true;
            } while (!Done);

            DrawMap();
            SetupIndicators();
        }
    }

    private void ResetGameData()
    {
        bool TurnOK;
        int i;
        int j;

        // This function basically resets everything so that a new game is started.

        Players.CalcNumPlayers();

        for (i = 1; i <= MAX_PLAYERS; i++)
        {
            NumOccupied[i] = 0;
            NumTroops[i] = 0;
            for (j = 1; j <= MAX_PLAYERS; j++)
            {
                // We start off liking everyone.
                Hate[i, j] = 1;
            }
        }

        Phase = 1;

        if (MyNetworkRole != NW_CLIENT)
        {
            // Reset Country data.
            for (i = 1; i <= MyMap!.NumberOfCountries; i++)
            {
                // Mark country as unoccupied.
                MyMap.Owner(i, 0);
                MyMap.CountryType(i, 0);
                // Assign it an 'unoccupied' color.
                MyMap.CountryColor(i, CFGUnoccupiedColor);
            }

            // If we want a random 1st player, make sure we pick one that isn't inactive.
            if (CFG1st == 7)
            {
                TurnOK = false;
                do
                {
                    Turn = Random.Shared.Next(1, 7);
                    if (PlayerType[Turn] > PTYPE_INACTIVE)
                    {
                        TurnOK = true;
                    }
                } while (TurnOK == false);
            }
            else
            {
                // The menu specifies who's first.
                Turn = CFG1st;
            }
        }

        // Reset our anti-double-clickin' flags.
        AlreadyAddedTroopsThisPhase = false;
        AlreadyPerformedActionThisPhase = false;
        AlreadyMovedTroopsThisPhase = false;
        AlreadyChoseTroopsThisPhase = false;
        AlreadyCheckedForMaxedOutCountries = false;
        AlreadyPassedThisPhase = false;
    }

    private void MenuAbout_Click(object sender, EventArgs e)
    {
        var about = new About();
        about.ShowDialog();
    }

    private void MenuExit_Click(object sender, EventArgs e)
    {

        // Store away all of our menu settings for the next session.
        MakeINI(INIpath);

        Application.Exit();
    }

    private void UnloadAll()
    {
        GameEnding = true;

        // Assume we have only one form and no form unloading is needed.
    }

    private void MenuResolution_Click(object sender, EventArgs e)
    {

        // Only allowable on the title screen.
        if (GameMode != GM_TITLE_SCREEN)
        {
            return;
        }

        int index = Array.IndexOf(MenuResolution, sender);

        for (int i = 1; i <= 3; i++)
        {
            MenuResolution[i].Checked = false;
        }

        MenuResolution[index].Checked = true;
        CFGResolution = index;

        // Update the screen!
        ChangeRes();
    }

    private void MenuRefresh_Click(object sender, EventArgs e)
    {

        // Let's redraw!
        if (GameMode == GM_GAME_ACTIVE || GameMode == GM_TITLE_SCREEN)
        {
            RedrawScreen();
            UpdateMessages();
        }
    }

    private void MenuPlayers_Click(object sender, EventArgs e)
    {

        var TempMode = GameMode;

        GameMode = GM_DIALOG_OPEN;

        var players = new Players();
        players.ShowDialog(this);

        GameMode = TempMode; // Restore the old mode.
    }

    private void RedrawScreen()
    {
        // Grab the display settings
        CollectDrawSettings();

        if (GameMode == GM_GAME_ACTIVE)
        {
            // Set up the CountryColor array
            for (var i = 1; i <= MyMap!.NumberOfCountries; i++)
            {
                if (MyMap.Owner(i) == 0)
                {
                    MyMap.CountryColor(i, CFGUnoccupiedColor);
                }
            }

        }

        // Clear the previous display.
        DrawBkg();

        // Draw the map!
        DrawMap();
    }

    private void CollectDrawSettings()
    {
        // This subroutine grabs the display info from the menus.

        for (int i = 1; i <= MAX_MENU_ITEMS; i++)
        {
            if (i <= 6)
            {
                if (MenuBorders[i].Checked == true)
                {
                    CFGBorders = i;
                }
            }
            if (i <= 4)
            {
                if (MenuAISpeed[i].Checked == true)
                {
                    CFGAISpeed = i;
                }
            }
            if (i <= 2)
            {
                if (MenuSfx[i].Checked == true)
                {
                    CFGSound = i;
                }
            }
        }
    }

    private void CollectMenuSettings(bool Parse)
    {

        // This subroutine grabs the game info from the menus.

        for (var i = 0; i <= MAX_MENU_ITEMS; i++)
        {
            if (i <= 7 && i > 0)
            {
                if (Menu1st[i].Checked) CFG1st = i;
            }

            if (i <= 5)
            {
                if (MenuBonus[i].Checked) CFGBonusTroops = i;
            }

            if (i <= 5 && i > 0)
            {
                if (MenuInitTroops[i].Checked) CFGInitTroopPl = i;
                if (MenuConquer[i].Checked) CFGConquer = i;
            }

            if (i <= 4 && i > 0)
            {
                if (MenuShips[i].Checked) CFGShips = i;
                if (MenuPorts[i].Checked) CFGPorts = i;
            }

            if (i <= 3 && i > 0)
            {
                if (MenuRandom[i].Checked) CFGEvents = i;
                if (MenuInitTroopCts[i].Checked) CFGInitTroopCt = i;
            }

            if (i <= 2 && i > 0)
            {
                if (MenuHQSelect[i].Checked) CFGHQSelect = i;
            }
        }

        // Set the number of players here, since this function is called
        // every time we start a new game.
        NumPlayers = Players.CalcNumPlayers();

        //Now parse the menu settings.
        if (Parse == false) return;

        // Set up the overwater offensive/defensive percentage.
        switch (CFGShips)
        {
            case 1:
                ShipPct = 0.1F;
                break;
            case 2:
                ShipPct = 0.25F;
                break;
            case 3:
                ShipPct = 0.5F;
                break;
            case 4:
                ShipPct = 1F;
                break;
        }

    }

    private void CollectMapSettings(bool Parse)
    {
        // This subroutine grabs the map config info from the menus.
        int i;
        int SizeDiv;

        for (i = 0; i < MAX_MENU_ITEMS; i++)
        {
            if (i <= 6 && i > 0)
            {
                if (MenuSize[i].Checked == true)
                {
                    CFGCountrySize = i;
                }
            }

            if (i <= 5 && i > 0)
            {
                if (MenuPct[i].Checked == true)
                {
                    CFGLandPct = i;
                }
            }
            if (i <= 4 && i > 0)
            {
                if (MenuLakeSize[i].Checked == true)
                {
                    CFGLakeSize = i;
                }
            }
            if (i <= 3 && i > 0)
            {
                if (MenuIslands[i].Checked == true)
                {
                    CFGIslands = i;
                }
                if (MenuResolution[i].Checked == true)
                {
                    CFGResolution = i;
                }
                if (MenuShape[i].Checked == true)
                {
                    CFGShape = i;
                }
                if (MenuProp[i].Checked == true)
                {
                    CFGProportion = i;
                }
            }
        }

        //Now parse the menu settings.
        if (Parse == false)
        {
            return;
        }

        //Get the number of countries that will fit in the
        //selected area based on country size.
        if (CFGCountrySize < 6)
        {
            //Stair-step what our size should be.
            SizeDiv = (MAX_COUNTRY_SIZE - MIN_COUNTRY_SIZE) / 4;
            MaxCountrySize = (SizeDiv * CFGCountrySize) + (MIN_COUNTRY_SIZE - SizeDiv);
        }
        else
        {
            //Hodge-Podge.  Average the value for now.
            MaxCountrySize = (MAX_COUNTRY_SIZE + MIN_COUNTRY_SIZE) / 2;
        }

        switch (CFGLandPct)
        {
            case 1:
                LandPct = 0.1;
                break;
            case 2:
                LandPct = 0.25;
                break;
            case 3:
                LandPct = 0.5;
                break;
            case 4:
                LandPct = 0.75;
                break;
            case 5:
                LandPct = 0.9;
                break;
        }
        CfgNumCountries = (int)((MyMap!.Xsize * MyMap.Ysize * LandPct) / MaxCountrySize);
        if (CfgNumCountries == 0)
        {
            CfgNumCountries = 1;
        }
        if (CfgNumCountries >= 999)
        {
            CfgNumCountries = 998;
        }
        //Make sure the number of countries is evenly divisible by the number of players.
        if (NumPlayers == 0)
        {
            return;
        }    //Prevent /0.
        if ((int)(CfgNumCountries / NumPlayers) != CfgNumCountries / NumPlayers)
        {
            CfgNumCountries = CfgNumCountries + (NumPlayers - (CfgNumCountries % NumPlayers));
        }

        //Get the proportional size variance.
        switch (CFGProportion)
        {
            case 1:
                PropPct = MAX_PROP_PCT / 4;
                break;
            case 2:
                PropPct = MAX_PROP_PCT / 2;
                break;
            case 3:
                PropPct = MAX_PROP_PCT;
                break;
        }

        //Get the minimum allowable lake size.
        switch (CFGLakeSize)
        {
            case 1:
                MinLakeSize = 0;
                break;
            case 2:
                MinLakeSize = 5;
                break;
            case 3:
                MinLakeSize = 10;
                break;
            case 4:
                MinLakeSize = 20;
                break;
        }

        //Get the percentage irregularity.
        switch (CFGShape)
        {
            case 1:
                ShapePct = 1;
                break;
            case 2:
                ShapePct = 0.5;
                break;
            case 3:
                ShapePct = 0.25;
                break;
        }

        //Get the island parameters.
        if (CFGIslands == 2)
        {
            CoastPctKeep = 0.99;
            IslePctKeep = 0.01;
        }
    }

    public void DrawBkg()
    {

        // BitBlt MapBuffer.hdc, 0, 0, Wide, Tall, Ocean.hdc, 0, 0, SRCCOPY

        using (var gdiOperations = new GDIOperations(MapBuffer, Ocean))
        {
            // Tile the ocean many times in each direction.
            for (int i = 0; i <= Wide; i += OCEAN_X_SIZE)
            {
                for (int j = 0; j <= Tall; j += OCEAN_Y_SIZE)
                {
                    gdiOperations.BitBlt(i, j, OCEAN_X_SIZE, OCEAN_Y_SIZE, 0, 0, SRCCOPY);
                }
            }
        }
    }

    private void CreateOcean()
    {
        Ocean = new Bitmap(OCEAN_X_SIZE, OCEAN_Y_SIZE);

        using (var gdiOperations = new GDIOperations(Ocean))
        {
            for (int i = 0; i < OCEAN_X_SIZE; i++)
            {
                for (int j = 0; j < OCEAN_Y_SIZE; j++)
                {
                    var colorValue = WaterColors[GetWaterColor()];
                    gdiOperations.SetPixel(i, j, colorValue);
                }
            }
        }
    }

    private int GetWaterColor()
    {
        var r = Random.Shared.NextSingle();

        if (r < 0.1)
        {
            return 1;
        }
        else if (r < 0.2)
        {
            return 2;
        }
        else if (r < 0.3)
        {
            return 3;
        }
        else if (r < 0.4)
        {
            return 4;
        }
        else if (r < 0.9)
        {
            return 5;
        }
        else
        {
            return 6;
        }
    }

    private void Form_MouseDown(object sender, MouseEventArgs e)
    {
        if (GameMode != GM_GAME_ACTIVE) return;
        if (EventInProgress == true) return;

        // Calculate the map coordinates where the mouse is.
        int MapX = (e.X - XOFFSET) / 8 + 1;
        int MapY = (e.Y - YOFFSET) / 8 + 1;

        int CurrentMouse;

        // Identify the country we're over.
        if (MapX > 0 && MapX <= MyMap!.Xsize && MapY > 0 && MapY <= MyMap.Ysize)
        {
            CurrentMouse = MyMap.Grid(MapX, MapY);
        }
        else
        {
            // We shouldn't be clicking outside the map *anyway*.
            return;
        }

        if (MapX > 0 && MapX <= MyMap.Xsize && MapY > 0 && MapY <= MyMap.Ysize)
        {
            if (e.Button == MouseButtons.Left)
            {
                // The user clicked the left mouse button.  If it is the user's turn,
                // we go to the turn sub.  If it is a computer player's turn, exi

                // Start a small splash if we're clicking in water.
                if (CurrentMouse > TILEVAL_COASTLINE)
                {
                    BuildBalls(SPLASH_COUNT,
                        e.X - XOFFSET,
                        e.Y - YOFFSET,
                        SPLASH_INTENSITY,
                        SPLASH_SPREAD,
                        SPLASH_ELASTIC,
                        SPLASH_SIZE,
                        SPLASH_COLOR);

                    SNDSplishSplash();
                    return;
                }

                if (PlayerType[Turn] != PTYPE_HUMAN) return;

                PlayerClick(CurrentMouse, Turn, Phase); // In the Play.bas module.
            }
            else if (e.Button == MouseButtons.Right)
            {
                // what happens when we click the right mouse button.
                if (CurrentMouse >= TILEVAL_COASTLINE)
                {
                    // Make a small splash, but different than the left water-click.
                    BuildBalls(SPLASH_COUNT + 10,
                        e.X - XOFFSET,
                        e.Y - YOFFSET,
                        SPLASH_INTENSITY + 3,
                        SPLASH_SPREAD,
                        SPLASH_ELASTIC,
                        SPLASH_SIZE,
                        SPLASH_COLOR);

                    PopulateCountryFrame(Turn, CurrentMouse);

                    SNDSplishSplash();

                    LastRightClick = CurrentMouse;

                    return;
                }

                // If we right-clicked on land, we need to populate the items on the right side.
                // First, see how many human players there are.

                int TempNumb1 = 0;
                int TempNumb2 = 0;

                for (var i = 1; i <= 6; i++)
                {
                    if (PlayerType[i] == PTYPE_HUMAN)
                    {
                        TempNumb1++;
                        TempNumb2 = i;
                    }
                }

                if (TempNumb1 == 1)
                {
                    // Only one human playing, so populate the table with their stats.
                    CalculateStrengths(TempNumb2, CurrentMouse);
                    PopulateCountryFrame(TempNumb2, CurrentMouse);
                }
                else
                {
                    // More than one human.  If it's a human's turn, do like normal.
                    if (PlayerType[Turn] == PTYPE_HUMAN)
                    {
                        CalculateStrengths(Turn, CurrentMouse);
                        PopulateCountryFrame(Turn, CurrentMouse);
                    }
                    else
                    {
                        // We're right-clicking during a computer's turn!  Put up JUST
                        // defense stats
                        CalculateStrengths(Turn, CurrentMouse);
                        PopulateCountryFrame(Turn, CurrentMouse);

                        AttackTxt.Visible = false;
                        AttackTot.Visible = false;
                        AttackLnd.Visible = false;
                        AttackWtr.Visible = false;
                    }
                }

                // Now flash either the right-clicked country or everything it borders.
                // Basically, the second right-click on a country will show its neighbors.
                if (LastRightClick == CurrentMouse)
                {
                    // Flash our neighbors.  If we have a port, do the overseas ones too.
                    for (var j = 1; j <= MyMap.NumberOfCountries; j++)
                    {
                        TempNumb1 = AIxCanReachy(CurrentMouse, j);

                        if (TempNumb1 == 1 || ((TempNumb1 == 2) && (MyMap.CountryType(CurrentMouse) & 1) == 1))
                        {
                            LongerFlash(j);
                        }
                    }

                    // Also erase the flash on the country itself (it looks cooler).
                    for (var k = 1; k <= 4; k++)
                    {
                        MyMap.Flash(CurrentMouse, k, 0);
                    }

                    // Update everything and get it flashing the first time.
                    DrawMap();
                    LastRightClick = -LastRightClick; // Flag it as clicked twice so renames still work.
                }
                else
                {
                    // Flash just us.
                    LongFlash(CurrentMouse);
                    DrawMap();
                    LastRightClick = CurrentMouse;
                }
            }
        }
    }

    private void Explode(int CountryID, int ExpColor)
    {
        // Start a big ass explosion based on country size and troop count.
        if (CountryID < TILEVAL_COASTLINE && CountryID > 0)
        {
            if (MyMap!.TroopCount(CountryID) > 0)
            {
                switch (CFGCountrySize)
                {
                    case 1:
                        BuildBalls(12 + (int)(MyMap.TroopCount(CountryID) * SMALL_COUNT),
                        (MyMap.DigitCoords(CountryID, 1) - 1) * 8,
                        (MyMap.DigitCoords(CountryID, 2) - 1) * 8,
                        (int)(15 + SMALL_INTENSITY * MyMap.TroopCount(CountryID)),
                        SMALL_SPREAD,
                        SMALL_ELASTIC,
                        SMALL_SIZE,
                        ExpColor);
                        break;
                    case 2:
                        BuildBalls(15 + (int)(MyMap.TroopCount(CountryID) * SMALL_COUNT),
                                      (MyMap.DigitCoords(CountryID, 1) - 1) * 8,
                                      (MyMap.DigitCoords(CountryID, 2) - 1) * 8,
                                      (int)(16 + SMALL_INTENSITY * MyMap.TroopCount(CountryID)),
                                      SMALL_SPREAD + 2,
                                      SMALL_ELASTIC,
                                      SMALL_SIZE + 1,
                                      ExpColor);
                        break;
                    case 3:
                        BuildBalls(18 + (int)(MyMap.TroopCount(CountryID) * MED_COUNT),
                                      (MyMap.DigitCoords(CountryID, 1) - 1) * 8,
                                      (MyMap.DigitCoords(CountryID, 2) - 1) * 8,

                                      (int)(17 + MED_INTENSITY * MyMap.TroopCount(CountryID)),
                                      MED_SPREAD,
                                      MED_ELASTIC,
                                      MED_SIZE,
                                      ExpColor);
                        break;
                    case 4:
                        BuildBalls(21 + (int)(MyMap.TroopCount(CountryID) * MED_COUNT),
                                      (MyMap.DigitCoords(CountryID, 1) - 1) * 8,
                                      (MyMap.DigitCoords(CountryID, 2) - 1) * 8,

                                      (int)(18 + MED_INTENSITY * MyMap.TroopCount(CountryID)),
                                      MED_SPREAD + 2,
                                      MED_ELASTIC,
                                      MED_SIZE + 1,
                                      ExpColor);
                        break;
                    case 5:
                    case 6:
                        BuildBalls(24 + (int)(MyMap.TroopCount(CountryID) * BIG_COUNT),
                                      (MyMap.DigitCoords(CountryID, 1) - 1) * 8,
                                      (MyMap.DigitCoords(CountryID, 2) - 1) * 8,
                                      (int)(20 + BIG_INTENSITY * MyMap.TroopCount(CountryID)),
                                      BIG_SPREAD,
                                      BIG_ELASTIC,
                                      BIG_SIZE,
                                      ExpColor);
                        break;

                }

            }
            else
            {
                // This country just got taken over.  Let's make a big balls explosion for it.
                BuildBalls(1 + (int)(250 * BIG_COUNT),
                                    (MyMap.DigitCoords(CountryID, 1) - 1) * 8,
                                    (MyMap.DigitCoords(CountryID, 2) - 1) * 8,
                                    (int)(35 + (BIG_INTENSITY * 200)),
                                    BIG_SPREAD + 5,
                                    BIG_ELASTIC,
                                    BIG_SIZE,
                                    ExpColor);
            }
        }
    }

    private static bool GameTimerRunning;

    private void GameTimer_Timer(object sender, EventArgs e)
    {
        if (GameTimerRunning) return;

        GameTimerRunning = true;
        try
        {
            GameTimer_Timer_Tick(sender, e);
        }
        finally
        {
            GameTimerRunning = false;
        }
    }

    private void GameTimer_Timer_Tick(object sender, EventArgs e)
    {
        int WinPl = 0;
        int TitX;
        int TitY;

        // This timer controls the game sequencing.  If it is a human's turn,
        // then nothing happens here.  But if it is a computer's turn, we run
        // through the AI subroutines at the appropriate turn phases.

        // Draw the title map if it's not there already.
        if (GameMode == GM_BUILDING_TITLE)
        {
            if (MapMade)
            {
                CfgNumCountries = 4;
            }
            else
            {
                // This is the first time the map is being created.
                MyMap = new Map();
            }

            // If the main form is minimized, get it back up!
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }

            SetupForm();

            MyMap!.SetupTitleScreen();

            DrawBkg();

            DrawMap();

            GameMode = GM_TITLE_SCREEN; // Title is now built.

        }

        // Is there currently no game in progress?
        // If so, we need to do titlescreen activities.
        if (GameMode == GM_TITLE_SCREEN)
        {
            // Then we do the cadence once every 8 seconds.
            if (DateTime.UtcNow >= CadenceTime.AddSeconds(8))
            {
                CadenceTime = DateTime.UtcNow;
                SNDPlayWarCadence();

                // Put a random explosion on the title screen (land only) every once in a while.
                do
                {
                    TitX = Random.Shared.Next(1, MyMap!.Xsize + 1);
                    TitY = Random.Shared.Next(1, MyMap.Ysize + 1);
                } while (MyMap.Grid(TitX, TitY) < TILEVAL_COASTLINE);


                BuildBalls(
                    70 + Random.Shared.Next(1, 101),
                    (TitX - 1) * 8,
                    (TitY - 1) * 8,
                    30 + Random.Shared.Next(1, 41),
                    MED_SPREAD + Random.Shared.Next(1, 31),
                    MED_ELASTIC,
                    BIG_SIZE,
                    Random.Shared.Next(1, 12)
                );
            }
        }

        // Only worry about this junk if a game is underway.
        if (GameMode != GM_GAME_ACTIVE || NetworkArbitrationInProgress == true)
        {
            return;
        }

        // First, if this player is inactive or already dead, move up to the next player.
        if (PlayerType[Turn] == PTYPE_INACTIVE || NumOccupied[Turn] == -1)
        {
            NextTurn(Turn, Phase);
            return;
        }

        // Do any outstanding events...
        if (EventInProgress == true)
        {
            RandomEventDoer();
            return;
        }

        // Let's check to see if there is only one player left.
        // If so, then this is the winner!
        // TODO:  Update this to include servers and clients.
        WonTurn = 0;
        for (var i = 1; i <= 6; i++)
        {
            if (PlayerType[i] > PTYPE_INACTIVE && NumOccupied[i] > -1)
            {
                WonTurn = WonTurn + 1;
                WinPl = i;
            }
        }

        if (WonTurn == 1)
        {
            // Only one person left alive!
            // Get rid of the last flash.
            ResetFlashing();
            DrawMap();

            // Stop the game and all timers.
            WonGame = false;
            do
            {
                WonGame = NoBalls(); // This function is in Boom.bas.
                Application.DoEvents(); // We need this or the program will get stuck in an infinite loop.
            } while (!WonGame);

            Application.DoEvents();
            Commentary.Text = PlayerName[WinPl] + " has won the game!";
            Commentary.BackColor = this.BackColor;
            Commentary.ForeColor = Color.Black;
            GameMode = GM_DIALOG_OPEN;
            Tribute tribute = new Tribute();
            tribute.ShowDialog(this);
            return;
        }

        // Get out if all players haven't picked HQ yet during auto HQ selection.
        if (PickedHQYet() == false) return;

        // If we're in the reinforcement phase, check some things.
        if (Phase == 1 && NumOccupied[Turn] > 0)
        {
            // Check if we have no reinforcements.  If not, skip this phase.
            if (CFGBonusTroops == 0)
            {
                NextPhase(Turn, Phase);
                return;
            }
            else
            {
                // Check if we have nowhere to put troops!  If everything we have is
                // maxed out, skip this phase.  Bugfix for version 2.0 BETA
                if (AlreadyCheckedForMaxedOutCountries == false)
                {
                    for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
                    {
                        // If this country belongs to the current player and is not maxed out...
                        if (MyMap.Owner(i) == Turn && MyMap.TroopCount(i) < MAX_COUNTRY_CAPACITY)
                        {
                            // ...then stop looking, we *can* place troops this turn.
                            AlreadyCheckedForMaxedOutCountries = true;
                            break;
                        }
                    }
                }

                // If we found just one country that's not maxed, keep going.  Otherwise leave.
                if (!AlreadyCheckedForMaxedOutCountries)
                {
                    AlreadyCheckedForMaxedOutCountries = true;
                    NextPhase(Turn, Phase);
                    return;
                }
            }
        }

        // If this is a human, we exit and basically wait for them to finish.
        // Or if this is a networked player, we wait on their input.
        if (PlayerType[Turn] == PTYPE_HUMAN || PlayerType[Turn] == PTYPE_NETWORK || PlayerType[Turn] == PTYPE_SERVER) return;

        // If we got here, then this is a computer!

        // Now see what we'll do.

        if (NumOccupied[Turn] == 0)
        {
            // Timing fix: If we got here, and auto HQ selection is enabled, leave!
            if (CFGHQSelect == 2) return; // We shouldn't be picking our own HQ.
            // This is their first turn.
            AIdelay();
            AIChooseHQ(false, Turn, Phase);
            NextTurn(Turn, Phase);
            return;
        }

        switch (Phase)
        {
            case 1:
                // Add reinforcements.
                AIdelay();
                AIReinforce(Turn, Phase);
                NextPhase(Turn, Phase);
                return;
            case 2:
                // Action!
                AIdelay();
                AIaction(Turn, Phase);
                NextPhase(Turn, Phase);
                return;
            case 3:
                // Troop movement.
                AIdelay();
                AITroopMove(Turn, Phase);
                NextTurn(Turn, Phase);
                MsgXferAmount = 0; // Reset the total in the troop move message.
                return;
        }
    }

    private static bool FlashTimerRunning;
    private void FlashTimer_Timer(object sender, EventArgs e)
    {
        if (FlashTimerRunning) return;
        FlashTimerRunning = true;

        try
        {
            FlashTimer_Timer_Tick(sender, e);
        }
        finally
        {
            FlashTimerRunning = false;
        }

    }

    private void FlashTimer_Timer_Tick(object sender, EventArgs e)
    {
        // Let's do a cycle of the Flashing engine real quick.
        if (GameMode == GM_GAME_ACTIVE) FlashTimerSub(sender, e);
    }

    private void UpdateMessages()
    {
        // This sub puts the appropriate message in the commentary line.

        // Leave if we don't need messaging.
        if (GameMode != GM_GAME_ACTIVE) return;

        // Get out of here if we don't need to print anything.
        if (NumOccupied[Turn] == -1 || PlayerType[Turn] == PTYPE_INACTIVE) return;

        // Fix the Resign button's visibility.
        if (PlayerType[Turn] == PTYPE_HUMAN && NumOccupied[Turn] > 0 && Phase == 1)
        {
            // If we're the last person left, *don't* put up the resign button.
            int j = 0;
            for (var i = 1; i <= MAX_PLAYERS; i++)
            {
                if (NumOccupied[i] > -1)
                {
                    j = j + 1;
                }
            }
            if (j != 1)
            {
                ResignBut.Visible = true;
                ResignBut.Focus();
            }
            else
            {
                ResignBut.Visible = false;
            }
        }
        else
        {
            ResignBut.Visible = false;
        }

        // Punch in the current player's name so we all know whose turn it is.
        for (var i = 1; i <= MAX_PLAYERS; i++)
        {
            if (Turn == i)
            {
                PlyrNames[i].BorderStyle = BorderStyle.Fixed3D;
            }
            else
            {
                PlyrNames[i].BorderStyle = BorderStyle.None;
            }
        }

        // If we're auto-picking HQs, put up a message to that effect...
        if (PickedHQYet() == false)
        {
            Commentary.BackColor = this.BackColor;
            Commentary.ForeColor = Color.Black;
            Commentary.Text = "Selecting HQs...";
        }

        // Put the right color on the message text.
        Commentary.BackColor = Color.FromArgb(PlayerColorCodes[Player[Turn]]);
        Commentary.ForeColor = Color.FromArgb(PlayerTextColor[Player[Turn]]);

        switch (PlayerType[Turn])
        {
            case PTYPE_HUMAN:
                // This is a human's turn.
                // See if they have any countries at all.

                if (NumOccupied[Turn] == 0 && CFGHQSelect == 1)
                {
                    // Nope, then this is their FIRST turn.
                    Commentary.Text = " " + PlayerName[Turn] + ": Choose your first country. This country will be your HQ for the rest of the game.";
                    return;
                }

                switch (Phase)
                {
                    case 1:
                        // Reinforcement phase.
                        if (CFGBonusTroops * NumOccupied[Turn] == 1)
                        {
                            Commentary.Text = " " + PlayerName[Turn] + ": Place " + (CFGBonusTroops * NumOccupied[Turn]) + " little troop in one of your countries.";
                        }
                        else
                        {
                            Commentary.Text = " " + PlayerName[Turn] + ": Place " + (CFGBonusTroops * NumOccupied[Turn]) + " troops in one of your countries.";
                        }
                        return;
                    case 2:
                        // Action phase.
                        Commentary.Text = " " + PlayerName[Turn] + ": Choose an enemy country to attack, a neutral country to annex, or your country to build a port.";
                        return;
                    case 3:
                        // Troop movement source select phase.
                        Commentary.Text = " " + PlayerName[Turn] + ": Choose a country to move troops from, or Pass for no troop movement.";
                        return;
                    case 4:
                        // Troop movement destination select phase.
                        Commentary.Text = " " + PlayerName[Turn] + ": Enter the number of troops to move and choose a destination, or Pass for no troop movement.";
                        return;
                }

                break;

            case PTYPE_COMPUTER:
                // This is a computer's turn.
                // See if they have any countries at all.
                if (NumOccupied[Turn] == 0 && CFGHQSelect == 1)
                {
                    // Nope, then this is their FIRST turn.
                    Commentary.Text = "" + PlayerName[Turn] + " is setting up headquarters...";
                    return;
                }

                switch (Phase)
                {
                    case 1:
                        // Reinforcement phase.
                        switch (CFGBonusTroops * NumOccupied[Turn])
                        {
                            case 0:
                                Commentary.Text = "";
                                break;
                            case 1:
                                Commentary.Text = " " + PlayerName[Turn] + " is placing one little troop...";
                                break;
                            default:
                                Commentary.Text = " " + PlayerName[Turn] + " is placing " + (CFGBonusTroops * NumOccupied[Turn]) + " troops...";
                                break;
                        }
                        return;
                    case 2:
                        // Action phase.
                        Commentary.Text = " " + PlayerName[Turn] + " is thinking...";
                        return;
                    case 3:
                        // Troop movement source select phase.
                        switch (MsgXferAmount)
                        {
                            case 0:
                                Commentary.Text = " " + PlayerName[Turn] + " is thinking about a troop movement...";
                                break;
                            case 1:
                                Commentary.Text = " " + PlayerName[Turn] + " is moving one little troop...";
                                break;
                            default:
                                Commentary.Text = " " + PlayerName[Turn] + " is moving " + MsgXferAmount + " troops...";
                                break;
                        }
                        return;
                    case 4:
                        // Troop movement destination select phase.  We should never, ever get here.  :)
                        Commentary.Text = " " + PlayerName[Turn] + " has found a BUG!  How did you get here?";
                        return;
                }

                break;

            case PTYPE_NETWORK:
            case PTYPE_SERVER:
                // Either this is a server waiting on a client or this is a client waiting for the server.
                if (NumOccupied[Turn] == 0 && CFGHQSelect == 1)
                {
                    // Nope, then this is their FIRST turn.
                    Commentary.Text = " Waiting for " + PlayerName[Turn] + " to set up headquarters...";
                    return;
                }

                switch (Phase)
                {
                    case 1:
                        // Reinforcement phase.
                        switch (CFGBonusTroops * NumOccupied[Turn])
                        {
                            case 0:
                                Commentary.Text = "";
                                break;
                            case 1:
                                Commentary.Text = " Waiting for " + PlayerName[Turn] + " to place one little troop...";
                                break;
                            default:
                                Commentary.Text = " Waiting for " + PlayerName[Turn] + " to place " + (CFGBonusTroops * NumOccupied[Turn]) + " troops...";
                                break;
                        }
                        return;
                    case 2:
                        // Action phase.
                        Commentary.Text = " Waiting for " + PlayerName[Turn] + " to complete an action...";
                        return;
                    case 3:
                        // Troop movement source select phase.
                        Commentary.Text = " Waiting for " + PlayerName[Turn] + " to move troops...";
                        return;
                    case 4:
                        // Troop movement destination select phase.  We should never, ever get here.  :)
                        Commentary.Text = " " + PlayerName[Turn] + " has found a BUG!  How did you get here?";
                        return;
                }

                break;
        }

        Commentary.Text = "";
    }

    private void Form_Unload(object sender, FormClosedEventArgs e)
    {
        // Store away all of our menu settings for the next session.
        MakeINI(INIpath);

        UnloadAll();
    }

    private void Form_Paint(object sender, PaintEventArgs e)
    {
        using (var gdiOperations = new GDIOperations(e.Graphics, PicBuffer))
        {
            // Copy the map to the screen each time the form is painted.
            // This will automatically refresh the map if something eclipses it
            // (like the tribute dialog).
            gdiOperations.BitBlt(XOFFSET, YOFFSET, Wide, Tall, 0, 0, SRCCOPY);
        }

        if (GameMode != GM_GAME_ACTIVE)
        {
            return;
        }


        double YBase = this.menuStrip1.Height + 38.5;
        double Dist = 470 / 15;
        int XBase = (this.Width) - 112;

        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            int IconType = 0;

            if (MyNetworkRole == NW_SERVER || MyNetworkRole == NW_NONE)
            {
                switch (PlayerType[i])
                {
                    case PTYPE_HUMAN:
                        IconType = 1;
                        break;
                    case PTYPE_COMPUTER:
                        IconType = 2;
                        break;
                    case PTYPE_NETWORK:
                        IconType = 4;
                        break;
                    default:
                        IconType = 0;
                        break;
                }
            }
            else if (MyNetworkRole == NW_CLIENT)
            {
                switch (TempPlayerType[i])
                {
                    case PTYPE_HUMAN:
                        IconType = 3; // Host machine human.
                        break;
                    case PTYPE_COMPUTER:
                        IconType = 2;
                        break;
                    case PTYPE_NETWORK:
                        if (i == MyClientIndex)
                        {
                            IconType = 1; // Human on this network client.
                        }
                        else
                        {
                            IconType = 4; // Human on another network client.
                        }
                        break;
                    default:
                        IconType = 0;
                        break;
                }
            }

            if (IconType > 0)
            {
                using (var gdiOperations = new GDIOperations(e.Graphics, LandMap))
                {
                    gdiOperations.BitBlt(XBase, (int)(YBase + ((i - 1) * Dist)), GFX_GRID, GFX_GRID, (GFX_ICONS_X + IconType) * GFX_GRID, (GFX_ICONS_Y + 1) * GFX_GRID, SRCAND);
                    gdiOperations.BitBlt(XBase, (int)(YBase + ((i - 1) * Dist)), GFX_GRID, GFX_GRID, (GFX_ICONS_X + IconType) * GFX_GRID, GFX_ICONS_Y * GFX_GRID, SRCINVERT);
                }
            }
        }
    }

    private void PlaceTroops()
    {
        int TMax = 0;
        int TMin = 0;
        int TRange = 0;
        int Cntry = 0;
        bool Bool1;
        bool Bool2;
        int k = 0;
        int l = 0;
        float r = 0;

        // Clean out all countries first.
        for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            MyMap.TroopCount(i, 0);
        }

        // Leave if there are no initial troops.
        if (CFGInitTroopPl == 1)
        {
            return;
        }

        switch (CFGInitTroopCt)
        {
            case 1:
                TMax = 10;
                TMin = 1;
                TRange = 10;
                break;
            case 2:
                TMax = 50;
                TMin = 10;
                TRange = 40;
                break;
            case 3:
                TMax = 100;
                TMin = 40;
                TRange = 60;
                break;
        }

        // First, we will place a number of large quantities of troops around the map.
        // One bunch per player, and no two next to each other.
        for (int i = 1; i <= NumPlayers; i++)
        {
            Bool1 = false;
            k = 0;
            do
            {
                k = k + 1;
                int j = (int)(Random.Shared.NextDouble() * MyMap.NumberOfCountries);
                // Let's check all neighbors of this country for existing troops.
                if (MyMap.TroopCount(j) > 0)
                {
                    // This one is already stacked.
                    Bool1 = false;
                }
                else
                {
                    Bool2 = false;
                    for (l = 1; l <= MyMap.MaxNeighbors; l++)
                    {
                        Cntry = MyMap.Neighbors(j, l);
                        if (Cntry == 0 || Cntry >= TILEVAL_COASTLINE)
                        {
                            break;
                        }
                        if (MyMap.TroopCount(Cntry) > 0)
                        {
                            // We're next to one already.
                            Bool2 = true;
                            break;
                        }
                    }
                    if (Bool2 == false)
                    {
                        // We found a good one!
                        Bool1 = true;
                        MyMap.TroopCount(j, TMax - (int)(Random.Shared.NextDouble() * (TRange * 0.1)));
                    }
                }
            } while (Bool1 == false && k != 100);
            if (k == 100)
            {
                // We must be playing on a map with not many countries.
                // Put it in the first one we find.
                for (l = 1; l <= MyMap.NumberOfCountries; l++)
                {
                    if (MyMap.TroopCount(l) == 0)
                    {
                        MyMap.TroopCount(l, TMax - (int)(Random.Shared.NextDouble() * (TRange * 0.1)));
                    }
                }
            }
        }

        // Now we will populate the rest of the map with lesser quantities,
        // based on the configuration settings.
        for (int i = 1; i <= MyMap.NumberOfCountries; i++)
        {
            r = (float)Random.Shared.NextDouble();
            if (r < (CFGInitTroopPl - 1) * 0.25)
            {
                if (MyMap.TroopCount(i) == 0)
                {
                    // This is an empty country.
                    MyMap.TroopCount(i, (int)(Random.Shared.NextDouble() * TRange) + TMin);
                }
            }
        }
    }

    private void PassTurnBut_Click(object sender, EventArgs e)
    {
        // If this button got clicked, then it means the human player has
        // chosen to do nothing this phase.  We will simply bounce to the
        // next phase.

        if (AlreadyPassedThisPhase == true) return;

        AlreadyPassedThisPhase = true;

        SNDWhistle();
        SendPassToNetwork(Turn, Phase, 0);

        if (Phase < 3)
        {
            NextPhase(Turn, Phase);
        }
        else
        {
            // If we're somewhere in troop movement, just skip it all.
            NextTurn(Turn, Phase);
        }
    }

    private void SetupIndicators()
    {

        // In between turns or phases, we update the player frame.
        for (int i = 1; i <= 6; i++)
        {
            PlyrNames[i].Text = PlayerName[i];
            PlyrNames[i].BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);
            PlyrNames[i].ForeColor = Color.FromArgb(PlayerTextColor[Player[i]]);
            PlyrTotals[i].Text = NumOccupied[i].ToString() + " : " + NumTroops[i].ToString();

            // If a player is inactive, don't display anything.
            if (PlayerType[i] == PTYPE_INACTIVE)
            {
                PlyrNames[i].Visible = false;
                PlyrTotals[i].Visible = false;
            }
            else
            {
                PlyrNames[i].Visible = true;
                PlyrTotals[i].Visible = true;
            }

            // If a player got killed, then gray out their name.
            if (NumOccupied[i] < 0)
            {
                PlyrTotals[i].Visible = false;
                PlyrNames[i].Enabled = false;
            }
            else if (PlayerType[i] != PTYPE_INACTIVE)
            {
                PlyrTotals[i].Visible = true;
                PlyrNames[i].Enabled = true;
            }
        }


        // In between turns or phases, we clear out the contents of the country frame.
        // Only a right-click can populate this frame!

        // If the last turn was a computer, let's leave the side dialog up since a human
        // was probably making some observations.  If a human just played, let's kill
        // the country frame so that the next player doesn't see too much!
        if (PlayerType[Turn] == PTYPE_HUMAN)
        {
            CntryName.Visible = false;
            AttackTxt.Visible = false;
            AttackTot.Visible = false;
            AttackLnd.Visible = false;
            AttackWtr.Visible = false;
            DefendTxt.Visible = false;
            DefendTot.Visible = false;
            DefendLnd.Visible = false;
            DefendWtr.Visible = false;
        }

        //Show both frames
        PlyrFrame.Visible = true;
        CntryFrame.Visible = true;
    }

    private void PopulateCountryFrame(int RightClicker, int CurrentMouse)
    {
        // This function gets called when a country is right-clicked.  It basically just
        // populates the right-hand information area.

        if (CurrentMouse >= TILEVAL_COASTLINE)
        {
            // We right-clicked in water.  Let's put up the name of the water mass. 
            CntryName.Text = MyMap!.WaterName(CurrentMouse - (1000 + ONE_ARRAY_FIX));
            CntryName.BackColor = BackColor;
            CntryName.ForeColor = Color.FromArgb((int)vbBlack);
            CntryName.Visible = true;
            AttackTxt.Visible = false;
            AttackTot.Visible = false;
            AttackLnd.Visible = false;
            AttackWtr.Visible = false;
            DefendTxt.Visible = false;
            DefendTot.Visible = false;
            DefendLnd.Visible = false;
            DefendWtr.Visible = false;

            return;
        }

        CntryName.Text = MyMap!.CountryName(CurrentMouse);

        if (MyMap.Owner(CurrentMouse) > 0)
        {
            CntryName.BackColor = Color.FromArgb(PlayerColorCodes[Player[MyMap.Owner(CurrentMouse)]]);
            CntryName.ForeColor = Color.FromArgb(PlayerTextColor[Player[MyMap.Owner(CurrentMouse)]]);
        }
        else
        {
            CntryName.BackColor = this.BackColor;
            CntryName.ForeColor = Color.FromArgb(vbBlack);
        }

        AttackTot.Text = AttackStrength.ToString();
        DefendTot.Text = DefendStrength.ToString();
        AttackLnd.Text = "Land: " + AttLndNum.ToString();
        AttackWtr.Text = "Water: " + AttWtrNum.ToString();
        DefendLnd.Text = "Land: " + DefLndNum.ToString();
        DefendWtr.Text = "Water: " + DefWtrNum.ToString();
        CntryName.Visible = true;
        AttackTxt.Visible = true;
        AttackTot.Visible = true;
        AttackLnd.Visible = true;
        AttackWtr.Visible = true;
        DefendTxt.Visible = true;
        DefendTot.Visible = true;
        DefendLnd.Visible = true;
        DefendWtr.Visible = true;

        // Now let's tweak what needs to be on and off, etc.

        if (MyMap.Owner(CurrentMouse) == RightClicker)
        {
            // The player right-clicked their own country.
            // Don't show the attack strength.
            AttackTxt.Visible = false;
            AttackTot.Visible = false;
            AttackLnd.Visible = false;
            AttackWtr.Visible = false;
            // Defend color should be theirs.
            DefendTxt.BackColor = Color.FromArgb(PlayerColorCodes[Player[RightClicker]]);
            DefendTxt.ForeColor = Color.FromArgb(PlayerTextColor[Player[RightClicker]]);
        }
        else if (MyMap.Owner(CurrentMouse) == 0)
        {
            // The player right-clicked unclaimed land.
            // Don't show attack or defense, but mention that it's empty.
            AttackTxt.Visible = false;
            AttackTot.Visible = false;
            AttackLnd.Visible = false;
            AttackWtr.Visible = false;
            DefendTxt.Visible = false;
            DefendTot.Visible = false;
            DefendLnd.Text = "Unclaimed";
            DefendWtr.Visible = false;
        }
        else
        {
            // The player right-clicked enemy land.
            // All things are visible, but change the attack and defend text
            // so that the colors are right.
            AttackTxt.BackColor = Color.FromArgb(PlayerColorCodes[Player[RightClicker]]);
            AttackTxt.ForeColor = Color.FromArgb(PlayerTextColor[Player[RightClicker]]);
            DefendTxt.BackColor = Color.FromArgb(PlayerColorCodes[Player[MyMap.Owner(CurrentMouse)]]);
            DefendTxt.ForeColor = Color.FromArgb(PlayerTextColor[Player[MyMap.Owner(CurrentMouse)]]);

            // If there is no attack strength here, let's get rid of the extra numbers.
            if (AttackStrength == 0)
            {
                AttackTot.Text = "----";
                AttackLnd.Visible = false;
                AttackWtr.Visible = false;
            }
        }
    }


    private void PlyrNames_Click(object sender, EventArgs e)
    {
        // Just flash this player's countries!

        var Index = Array.IndexOf(PlyrNames, sender);

        if (PlayerType[Index] > PTYPE_INACTIVE && NumOccupied[Index] > 0)
        {
            for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
            {
                if (MyMap.CountryColor(i) == Player[Index])
                {
                    // This country belongs to the player whose name we clicked.  Flash it.
                    LongFlash(i);
                }
            }
            DrawMap();
        }
    }

    private void CntryName_Click(object sender, EventArgs e)
    {
        // Compensate for two right-clicks in a row...
        LastRightClick = Math.Abs(LastRightClick);

        // Allow the player to change the name of this country.
        // Leave if not our country.
        if (PlayerType[Turn] != PTYPE_HUMAN)
        {
            Oopsie.Text = "You can only rename your countries during your turn.";
            SNDBooBoo();
            return;
        }

        // Leave if not a country or water at all.
        if (LastRightClick < 1)
        {
            Oopsie.Text = "Only land and water can be renamed.";
            SNDBooBoo();
            return;
        }

        // If this is water, all of its coastline must belong to the current player.
        bool FoundIt = false;
        if (LastRightClick > TILEVAL_COASTLINE)
        {
            // See if the current player owns all coastline near this water mass.
            for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
            {
                if (FoundIt == true) break;
                for (int j = 1; j <= MyMap.MaxNeighbors; j++)
                {
                    int Cntry = MyMap.Neighbors(i, j);
                    if (Cntry == 0) break;  //Done with this country.
                    if (Cntry == LastRightClick)
                    {   //It's water.
                        if (MyMap.Owner(i) != Turn)
                        {
                            //This is a country we don't own which borders the right-clicked one.
                            FoundIt = true;
                            break;
                        }
                    }
                }
            }
            //Now see if we found a country that borders us that we don't own yet.
            if (FoundIt == true)
            {
                Oopsie.Text = "To rename a body of water, you must own all of its coastline.";
                SNDBooBoo();
                return;
            }
            else
            {
                //We can rename this one!  Populate the rename dialog....
                var renameCountry = new RenameCountry();
                renameCountry.OldName.Text = MyMap.WaterName(LastRightClick - 1000);
                renameCountry.OldName.BackColor = renameCountry.BackColor;
                renameCountry.NewName.Text = "";
                //....and show it.
                renameCountry.ShowDialog();
                return;
            }
        }

        // At this point, we are trying to rename a country.  Leave if not our country.
        if (MyMap!.Owner(LastRightClick) != Turn)
        {
            Oopsie.Text = "You can only rename your own countries.";
            SNDBooBoo();
            return;
        }
        else
        {
            //Populate the rename dialog....
            var renameCountry = new RenameCountry();
            renameCountry.RefOldName!.Text = MyMap.CountryName(LastRightClick);
            renameCountry.RefOldName.BackColor = Color.FromArgb(PlayerColorCodes[MyMap.CountryColor(LastRightClick)]);
            renameCountry.RefNewName!.Text = "";
            //....and show it.
            renameCountry.ShowDialog();
        }

    }

    private void ResignBut_Click(object sender, EventArgs e)
    {
        // Leave if this isn't a human's first turn phase.
        if (Phase != 1 || PlayerType[Turn] != PTYPE_HUMAN) return;

        // See if the *last* player is trying to resign.  There's a window where
        // this could happen if you're just clickin' stuff.
        int a = 0;
        for (int i = 1; i <= 6; i++)
        {
            if (NumOccupied[i] > 0) a = a + 1;
        }
        if (a == 1) return;  // We're the only one left!  Wait until game over hits.

        // The player has resigned!  Let's check to make sure they meant it...
        var resp = MessageBox.Show(PlayerName[Turn] + ": Are you sure you want to resign?", "Resign", MessageBoxButtons.YesNo);

        if (resp == DialogResult.No) return;   // I didn't think so.

        // Perform the resign.
        ResignProc(Turn);
        // Send this signal to others if necessary.
        SendResignToNetwork(Turn, Phase, 0);

        ResignBut.Visible = false;

        // The GameTimer will detect that the resign has occured and pass the turn.
    }

    private void ResignProc(int Turn)
    {
        // Now we need to resign the player.  Their countries either stay put
        // or go away based on the config settings.

        for (int a = 1; a <= MyMap!.NumberOfCountries; a++)
        {
            if (MyMap.Owner(a) == Turn)
            {
                // We need to do something with this country.
                switch (CFGConquer)
                {
                    case 2:
                    case 4:
                        // All this player's countries turn neutral again.
                        // Kill ports, leave troops.
                        MyMap.Owner(a, 0);
                        MyMap.CountryType(a, 0);
                        // Assign it an 'unoccupied' color.
                        MyMap.CountryColor(a, CFGUnoccupiedColor);
                        // If 'Enemy is completely eradicated', then remove troops.
                        if (CFGConquer == 4)
                        {
                            MyMap.TroopCount(a, 0);
                        }
                        break;
                    case 1:
                    case 3:
                        // Countries stay owned by the defunct player.
                        // Basically, we do nothing here!  The remaining players
                        // will need to attack and conquer the countries to claim them.
                        // Since there is no 'victor' here, we'll leave them alone.
                        // We do need to kill the HQ, though...
                        if ((MyMap.CountryType(a) & 2) == 2)
                        {
                            MyMap.CountryType(a, MyMap.CountryType(a) & 253);
                        }
                        break;
                    case 5:
                        // Chaos erupts!
                        if (MyNetworkRole != NW_CLIENT)
                        {
                            // First, kill all ports and HQs.
                            MyMap.CountryType(a, 0);
                            // There is a 50% chance of going neutral, and a
                            // 50% chance of staying loyal.
                            if (Random.Shared.NextDouble() < 0.5)
                            {
                                // Country goes neutral.
                                MyMap.Owner(a, 0);
                                MyMap.CountryColor(a, CFGUnoccupiedColor);
                                // Else it stays loyal, do nothing.
                            }
                            // Now see if we kill troops in it.  40% chance.
                            if (Random.Shared.NextDouble() < 0.4)
                            {
                                // Kill 'em all!
                                MyMap.TroopCount(a, 0);
                            }
                        }
                        break;
                }
            }
        }

        NumOccupied[Turn] = -1;  // This effectively disables the player.

        // Update our statistics...
        StatScreen.UpdateResignedStats(Turn);

        // Now send all map data to the clients if we are the server.
        if (MyNetworkRole == NW_SERVER && CFGConquer == 5)
        {
            SendNewCountryData();
        }

        // Refresh, since lots of countries may have changed...
        RedrawScreen();
    }

    private void TroopMoveDn_Click(object sender, EventArgs e)
    {
        // Decrement the troop entry prompt by one.
        if (Convert.ToInt32(TroopMoveNum.Text) > 1)
        {
            TroopMoveNum.Text = (Convert.ToInt32(TroopMoveNum.Text) - 1).ToString();
        }
    }

    private void TroopMoveNum_Change(object sender, EventArgs e)
    {
        // This sub is called whenever the number in the troop entry prompt changes.
        // We will verify that it is between 1 and the max troops for the selected
        // country and adjust it if it isn't.  Basically, the number in the box
        // will always be valid.  No error checking is necessary elsewhere!

        if (TroopMoveNum.Visible == false) return;

        // Check for non-numeric characters.
        for (int q = 0; q < TroopMoveNum.Text.Length; q++)
        {
            if (!char.IsDigit(TroopMoveNum.Text[q]))
            {
                TroopMoveNum.Text = "1";
                return;
            }
        }

        // Check for too low.
        if (TroopMoveNum.Text == "" || Convert.ToInt32(TroopMoveNum.Text) == 0)
        {
            TroopMoveNum.Text = "1";
            return;
        }

        // Check for too high.
        if (Convert.ToInt32(TroopMoveNum.Text) > MyMap!.TroopCount(TroopMoveSrc))
        {
            TroopMoveNum.Text = MyMap.TroopCount(TroopMoveSrc).ToString();
            return;
        }
    }

    private void TroopMoveQtr_Click(object sender, EventArgs e)
    {
        // Put 25% of troops in the troop entry prompt.

        int TempNum = (int)Math.Floor(MyMap!.TroopCount(TroopMoveSrc) * 0.25);
        if (TempNum == 0) TempNum = 1;

        TroopMoveNum.Text = TempNum.ToString();
    }

    private void TroopMoveHlf_Click(object sender, EventArgs e)
    {
        // Put 50% of troops in the troop entry prompt.

        int TempNum = (int)Math.Floor(MyMap!.TroopCount(TroopMoveSrc) * 0.5);
        if (TempNum == 0) TempNum = 1;

        TroopMoveNum.Text = TempNum.ToString();
    }

    private void TroopMove3Qt_Click(object sender, EventArgs e)
    {
        // Put 75% of troops in the troop entry prompt.

        int TempNum = (int)Math.Floor(MyMap!.TroopCount(TroopMoveSrc) * 0.75);
        if (TempNum == 0) TempNum = 1;

        TroopMoveNum.Text = TempNum.ToString();
    }

    private void TroopMoveAll_Click(object sender, EventArgs e)
    {
        // Put 100% of troops in the troop entry prompt.

        TroopMoveNum.Text = MyMap!.TroopCount(TroopMoveSrc).ToString();
    }

    private void TroopMoveUp_Click(object sender, EventArgs e)
    {
        // Increment the troop entry prompt by one.
        if (Convert.ToInt32(TroopMoveNum.Text) < MyMap!.TroopCount(TroopMoveSrc))
        {
            TroopMoveNum.Text = (Convert.ToInt32(TroopMoveNum.Text) + 1).ToString();
        }
    }

    private void CancelBut_Click(object sender, EventArgs e)
    {
        // Pressing this button will allow the player to choose a new source country
        // during the troop movement phase.
        if (Phase == 4) Phase = 3;

        // Also clear our double-click prevention flags.
        AlreadyMovedTroopsThisPhase = false;
        AlreadyChoseTroopsThisPhase = false;

        // And get rid of the troop movement controls.
        SetUpPlayerControls();
    }

    private void MenuAniSpeed_Click(object sender, EventArgs e)
    {

        var TempMode = GameMode;

        if (GameMode == GM_TITLE_SCREEN)
        {
            GameMode = GM_TITLE_DIALOG_OPEN;
        }
        else
        {
            GameMode = GM_DIALOG_OPEN;
        }

        var gfxOptions = new GfxOptions();
        gfxOptions.ShowDialog(this);

        GameMode = TempMode; // Restore the old mode.

        // Draw the map.
        DrawMap();
    }

    private void InitPlayerData()
    {

        // Initialize the player settings.
        Player[1] = 1;
        Player[2] = 3;
        Player[3] = 5;
        Player[4] = 9;
        Player[5] = 7;
        Player[6] = 4;

        PlayerType[1] = PTYPE_HUMAN;
        PlayerType[2] = PTYPE_COMPUTER;
        PlayerType[3] = PTYPE_COMPUTER;
        PlayerType[4] = PTYPE_COMPUTER;
        PlayerType[5] = PTYPE_COMPUTER;
        PlayerType[6] = PTYPE_COMPUTER;

        PlayerName[1] = "Player 1";
        PlayerName[2] = "Player 2";
        PlayerName[3] = "Player 3";
        PlayerName[4] = "Player 4";
        PlayerName[5] = "Player 5";
        PlayerName[6] = "Player 6";

        PlayerColors[1] = "Purple";
        PlayerColors[2] = "Blue";
        PlayerColors[3] = "Yellow";
        PlayerColors[4] = "Orange";
        PlayerColors[5] = "Red";
        PlayerColors[6] = "Gray";
        PlayerColors[7] = "Light Blue";
        PlayerColors[8] = "Pink";
        PlayerColors[9] = "Green";
        PlayerColors[10] = "Light Green";
        PlayerColors[11] = "Brown";
        PlayerColors[12] = "White";

        PlayerColorCodes[1] = RGB(204, 51, 255);
        PlayerColorCodes[2] = RGB(0, 153, 204);
        PlayerColorCodes[3] = RGB(255, 255, 51);
        PlayerColorCodes[4] = RGB(255, 153, 0);
        PlayerColorCodes[5] = RGB(204, 51, 0);
        PlayerColorCodes[6] = RGB(102, 102, 102);
        PlayerColorCodes[7] = RGB(0, 255, 255);
        PlayerColorCodes[8] = RGB(255, 102, 153);
        PlayerColorCodes[9] = RGB(0, 153, 0);
        PlayerColorCodes[10] = RGB(102, 255, 51);
        PlayerColorCodes[11] = RGB(153, 102, 0);
        PlayerColorCodes[12] = RGB(255, 255, 255);

        PlayerTextColor[1] = vbBlack;
        PlayerTextColor[2] = vbBlack;
        PlayerTextColor[3] = vbBlack;
        PlayerTextColor[4] = vbBlack;
        PlayerTextColor[5] = vbWhite;
        PlayerTextColor[6] = vbWhite;
        PlayerTextColor[7] = vbBlack;
        PlayerTextColor[8] = vbBlack;
        PlayerTextColor[9] = vbWhite;
        PlayerTextColor[10] = vbBlack;
        PlayerTextColor[11] = vbWhite;
        PlayerTextColor[12] = vbBlack;

        // Also initialize anything else that shouldn't be a 0 or FALSE on start.
        CFGExplosions = 1;
        CFGWaves = 1;
        CFGUnoccupiedColor = 6;
        CFGFlashing = 1;
        CFGPrompt = 1;
    }

    private bool PickedHQYet()
    {
        // Return False if all players haven't picked HQ yet during auto HQ selection.

        if (CFGHQSelect == 2)
        {
            for (var i = 1; i <= 6; i++)
            {
                if (NumOccupied[i] == 0 && PlayerType[i] > PTYPE_INACTIVE)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void SetupForm()
    {
        int WinWidth = 0;
        int WinHeight = 0;

        // This function just sets up our form's parameters, dimensions, and the like.
        // We return TRUE if we had to change the dimensions, and FALSE if
        // we were already at this resolution.

        // First, set up our form's dimensions based on the menu setting.
        switch (CFGResolution)
        {
            case 1:
                WinWidth = 640 + XOFFSET;
                WinHeight = 450 + YOFFSET;
                MyMap!.SetDimensions(XDIM640x480, YDIM640x480);
                break;
            case 2:
                WinWidth = 800 + XOFFSET;
                WinHeight = 570 + YOFFSET;
                MyMap!.SetDimensions(XDIM800x600, YDIM800x600);
                break;
            case 3:
                WinWidth = 1024 + XOFFSET;
                WinHeight = 738 + YOFFSET;
                MyMap!.SetDimensions(XDIM1024x768, YDIM1024x768);
                break;
        }

        // See if we need to resize this form later.
        var LetsResize = false;
        if (WinWidth != this.Width)
        {
            LetsResize = true;
        }

        Wide = MyMap!.Xsize * 8;
        Tall = MyMap.Ysize * 8;

        MapBuffer = new Bitmap(Wide - 4, Tall - 4);
        PicBuffer = new Bitmap(Wide - 4, Tall - 4);

        // Set up window position and dimensions.
        this.Width = WinWidth;
        this.Height = WinHeight;

        // Add other controls.
        PassTurnBut.Left = (WinWidth - 100);
        PassTurnBut.Top = menuStrip1.Height + 1;
        PassTurnBut.Visible = false;

        PlyrFrame.Left = (WinWidth - 100);
        PlyrFrame.Top = menuStrip1.Height + 24;
        PlyrFrame.Visible = false;

        CntryFrame.Left = (WinWidth - 100);
        CntryFrame.Top = menuStrip1.Height + 225;
        CntryFrame.Visible = false;

        TroopMoveLbl.Left = (WinWidth - 100);
        TroopMoveLbl.Top = (WinHeight - 157);
        TroopMoveLbl.Visible = false;

        TroopMoveNum.Left = (WinWidth - 100);
        TroopMoveNum.Top = (WinHeight - 140);
        TroopMoveNum.Text = "1";
        TroopMoveNum.Visible = false;

        TroopMoveUp.Left = (WinWidth - 44);
        TroopMoveUp.Top = (WinHeight - 148);
        TroopMoveUp.Visible = false;

        TroopMoveDn.Left = (WinWidth - 44);
        TroopMoveDn.Top = (WinHeight - 131);
        TroopMoveDn.Visible = false;

        TroopMoveQtr.Left = (WinWidth - 100);
        TroopMoveQtr.Top = (WinHeight - 112);
        TroopMoveQtr.Visible = false;

        TroopMoveHlf.Left = (WinWidth - 81);
        TroopMoveHlf.Top = (WinHeight - 112);
        TroopMoveHlf.Visible = false;

        TroopMove3Qt.Left = (WinWidth - 62);
        TroopMove3Qt.Top = (WinHeight - 112);
        TroopMove3Qt.Visible = false;

        TroopMoveAll.Left = (WinWidth - 44);
        TroopMoveAll.Top = (WinHeight - 112);
        TroopMoveAll.Visible = false;

        CancelBut.Left = (WinWidth - 100);
        CancelBut.Top = (WinHeight - 87);
        CancelBut.Visible = false;

        ResignBut.Left = (WinWidth - 100);
        ResignBut.Top = menuStrip1.Height + 1;
        ResignBut.Visible = false;

        Commentary.Left = XOFFSET;
        Commentary.Top = this.Height - 75;
        Commentary.Text = "Choose game parameters from the Options menu, and landmass parameters from the Terraform menu.";
        Commentary.Width = (Wide - 4);
        Commentary.BackColor = this.BackColor;
        Commentary.ForeColor = Color.Black;
        Commentary.Visible = true;

        Oopsie.Left = XOFFSET;
        Oopsie.Top = this.Height - 90;
        Oopsie.Text = "Welcome to Fracaz.";
        Oopsie.Width = (Wide - 4) * 3;
        Oopsie.BackColor = this.BackColor;

        if (LetsResize)
        {
            CenterOurForm();
        }
    }

    private void CenterOurForm()
    {
        var screen = Screen.FromControl(this);
        this.Left = (screen.Bounds.Width / 2) - (this.Width / 2);

        if (CFGResolution == MaxAllowedResolution && (screen.Bounds.Height / screen.Bounds.Height) < 800)
        {
            this.Top = 0;
        }
        else
        {
            this.Top = (screen.Bounds.Height / 2) - (this.Height / 2);
        }
    }

    private void ChangeRes()
    {
        // Things to do when the map resolution changes.
        MyMap = new Map();

        // 'If the main form is minimized, get it back up!
        if (this.WindowState == FormWindowState.Minimized)
        {
            this.WindowState = FormWindowState.Normal;
            this.Focus();
        }

        SetupForm();

        MyMap.SetupTitleScreen();
        DrawBkg();
        DrawMap();
        this.Refresh();
    }

    private void DetermineMaxRes()
    {
        // Only enable resolutions less than or equal to what we have currently.
        Screen screen = Screen.FromControl(this);
        var bounds = screen.Bounds;

        if (bounds.Width < 800)
        {
            MaxAllowedResolution = 1;
        }
        else if (bounds.Width < 1024)
        {
            MaxAllowedResolution = 2;
        }
        else
        {
            MaxAllowedResolution = 3;
        }
    }

    private void FixResolution()
    {
        // This sub verifies that our resolution is good.
        if (CFGResolution > MaxAllowedResolution)
        {
            CFGResolution = MaxAllowedResolution;
            for (int i = 1; i <= 3; i++)
            {
                MenuResolution[i].Checked = false;
            }
            MenuResolution[CFGResolution].Checked = true;
        }
    }

    private void SetupMenusForGameOver()
    {

        // Get rid of the troop movement entry tool and other player controls.
        Commentary.BackColor = Land.DefaultBackColor;
        Commentary.ForeColor = Color.Black;
        PassTurnBut.Visible = false;
        TroopMoveNum.Visible = false;
        TroopMoveUp.Visible = false;
        TroopMoveDn.Visible = false;
        TroopMoveLbl.Visible = false;
        TroopMoveQtr.Visible = false;
        TroopMoveHlf.Visible = false;
        TroopMove3Qt.Visible = false;
        TroopMoveAll.Visible = false;
        CancelBut.Visible = false;
        ResignBut.Visible = false;
        PlyrFrame.Visible = false;
        CntryFrame.Visible = false;

        // Set up menu items for when a game is not being played.
        MenuNew.Enabled = true;
        MenuLoad.Enabled = true;
        MenuGetOptionsFromMapFile.Enabled = true;
        MenuSame.Enabled = false;
        MenuHiScores.Enabled = false;

        if (!string.IsNullOrEmpty(LastMapPath))
        {
            MenuSame.Enabled = true;
            MenuHiScores.Enabled = true;
        }

        MenuLoadSavedGame.Enabled = true;
        MenuJoinGame.Enabled = true;
        MenuChat.Enabled = false;
        MenuSave.Enabled = false;
        MenuSaveGame.Enabled = false;
        MenuPlayers.Enabled = true;
        MenuAbortGame.Enabled = false;
        MenuStats.Enabled = false;

        for (var i = 0; i <= MAX_MENU_ITEMS; i++)
        {
            if (i <= 7 && i > 0)
            {
                Menu1st[i].Enabled = true;
            }
            if (i <= 6 && i > 0)
            {
                MenuSize[i].Enabled = true;
            }

            if (i <= 5)
            {
                MenuBonus[i].Enabled = true;
            }

            if (i <= 5 && i > 0)
            {
                MenuInitTroops[i].Enabled = true;
                MenuPct[i].Enabled = true;
                MenuConquer[i].Enabled = true;
            }

            if (i <= 4 && i > 0)
            {
                MenuPorts[i].Enabled = true;
                MenuShips[i].Enabled = true;
                MenuLakeSize[i].Enabled = true;
            }
            if (i <= 3 && i > 0)
            {
                MenuRandom[i].Enabled = true;
                MenuInitTroopCts[i].Enabled = true;
                MenuIslands[i].Enabled = true;
                MenuShape[i].Enabled = true;
                MenuProp[i].Enabled = true;
                MenuResolution[i].Enabled = false; // Turn on the ones we want below.
            }
            if (i <= 2 && i > 0)
            {
                MenuHQSelect[i].Enabled = true;
            }
        }

        // Disable no starting troops if we have no bonus troops.
        if (MenuBonus[0].Checked)
        {
            MenuInitTroops[1].Enabled = false;
        }
        else
        {
            MenuInitTroops[1].Enabled = true;
        }

        // Disable no bonus troops if we start with none.
        if (MenuInitTroops[1].Checked)
        {
            MenuBonus[0].Enabled = false;
        }
        else
        {
            MenuBonus[0].Enabled = true;
        }

        // Only enable resolutions less than or equal to what we have currently.
        for (int i = 1; i <= MaxAllowedResolution; i++)
        {
            MenuResolution[i].Enabled = true;
        }

        this.Invalidate();
    }

    private void SetupMenusForInGame()
    {

        // Set up menu items for in-game use.
        MenuNew.Enabled = false;
        MenuSame.Enabled = false;
        MenuLoad.Enabled = false;
        MenuGetOptionsFromMapFile.Enabled = false;
        MenuLoadSavedGame.Enabled = false;
        MenuSave.Enabled = true;
        MenuJoinGame.Enabled = false;
        MenuPlayers.Enabled = false;
        MenuAbortGame.Enabled = true;
        MenuHiScores.Enabled = true;
        MenuChat.Enabled = false;
        if (MyNetworkRole != NW_NONE)
        {
            MenuHiScores.Enabled = false;
            MenuChat.Enabled = true;
        }
        MenuStats.Enabled = true;
        for (int i = 0; i <= MAX_MENU_ITEMS; i++)
        {
            if (i <= 7 && i > 0)
            {
                Menu1st[i].Enabled = false;
            }
            if (i <= 5)
            {
                MenuBonus[i].Enabled = false;
            }
            if (i <= 6 && i > 0)
            {
                MenuSize[i].Enabled = false;
            }
            if (i <= 5 && i > 0)
            {
                MenuInitTroops[i].Enabled = false;
                MenuPct[i].Enabled = false;
                MenuConquer[i].Enabled = false;
            }
            if (i <= 4 && i > 0)
            {
                MenuPorts[i].Enabled = false;
                MenuShips[i].Enabled = false;
                MenuLakeSize[i].Enabled = false;
            }
            if (i <= 3 && i > 0)
            {
                MenuRandom[i].Enabled = false;
                MenuInitTroopCts[i].Enabled = false;
                MenuIslands[i].Enabled = false;
                MenuResolution[i].Enabled = false;
                MenuShape[i].Enabled = false;
                MenuProp[i].Enabled = false;
            }
            if (i <= 2 && i > 0)
            {
                MenuHQSelect[i].Enabled = false;
            }
        }
    }

    private void SetUpPlayerControls()
    {
        switch (PlayerType[Turn])
        {
            case PTYPE_HUMAN:
                // This is a human's turn.
                // See if they have any countries at all.
                if (NumOccupied[Turn] == 0 && CFGHQSelect == 1)
                {
                    // Nope, then this is their FIRST turn.
                    PassTurnBut.Visible = false;
                    CntryFrame.Visible = true;
                    return;
                }

                switch (Phase)
                {
                    case 1:
                        // Reinforcement phase.
                        PassTurnBut.Visible = false;
                        TroopMoveNum.Visible = false;
                        TroopMoveUp.Visible = false;
                        TroopMoveDn.Visible = false;
                        TroopMoveLbl.Visible = false;
                        TroopMoveQtr.Visible = false;
                        TroopMoveHlf.Visible = false;
                        TroopMove3Qt.Visible = false;
                        TroopMoveAll.Visible = false;
                        CancelBut.Visible = false;
                        CntryFrame.Visible = true;
                        return;

                    case 2:
                        // Action phase.
                        PassTurnBut.Visible = true;
                        TroopMoveNum.Visible = false;
                        TroopMoveUp.Visible = false;
                        TroopMoveDn.Visible = false;
                        TroopMoveLbl.Visible = false;
                        TroopMoveQtr.Visible = false;
                        TroopMoveHlf.Visible = false;
                        TroopMove3Qt.Visible = false;
                        TroopMoveAll.Visible = false;
                        CancelBut.Visible = false;
                        ResignBut.Visible = false;
                        CntryFrame.Visible = true;

                        PassTurnBut.Focus();
                        return;

                    case 3:
                        // Troop movement source select phase.
                        PassTurnBut.Visible = true;
                        TroopMoveNum.Visible = false;
                        TroopMoveUp.Visible = false;
                        TroopMoveDn.Visible = false;
                        TroopMoveLbl.Visible = false;
                        TroopMoveQtr.Visible = false;
                        TroopMoveHlf.Visible = false;
                        TroopMove3Qt.Visible = false;
                        TroopMoveAll.Visible = false;
                        CancelBut.Visible = false;
                        ResignBut.Visible = false;
                        CntryFrame.Visible = true;

                        PassTurnBut.Focus();
                        return;

                    case 4:
                        // Troop movement destination select phase.
                        PassTurnBut.Visible = true;
                        TroopMoveNum.Visible = true;

                        // Set the focus there and highlight the text so we can type immediately.
                        TroopMoveNum.Focus();
                        TroopMoveNum.SelectAll();
                        TroopMoveUp.Visible = true;
                        TroopMoveDn.Visible = true;
                        TroopMoveLbl.Visible = true;
                        TroopMoveQtr.Visible = true;
                        TroopMoveHlf.Visible = true;
                        TroopMove3Qt.Visible = true;
                        TroopMoveAll.Visible = true;
                        CancelBut.Visible = true;
                        ResignBut.Visible = false;
                        if (CFGResolution == 1)
                        {
                            CntryFrame.Visible = false;
                        }
                        else
                        {
                            CntryFrame.Visible = true;
                        }
                        return;
                }
                break;
            case PTYPE_COMPUTER:
            case PTYPE_NETWORK:
            case PTYPE_SERVER:
                // This is a computer's turn!
                // Get rid of all the onscreen controls...
                PassTurnBut.Visible = false;
                TroopMoveNum.Visible = false;
                TroopMoveUp.Visible = false;
                TroopMoveDn.Visible = false;
                TroopMoveLbl.Visible = false;
                TroopMoveQtr.Visible = false;
                TroopMoveHlf.Visible = false;
                TroopMove3Qt.Visible = false;
                TroopMoveAll.Visible = false;
                CancelBut.Visible = false;
                ResignBut.Visible = false;
                break;
        }
    }

    private void CalculateTotals()
    {
        // This sub recalculates the values that should be in the box to the right.
        // Used when setting up a client so we don't have to send these over the wire.
        // Also used after resending all country data after a random event.
        for (int j = 1; j <= MAX_PLAYERS; j++)
        {
            if (NumOccupied[j] != -1)
            {
                NumOccupied[j] = 0;
                NumTroops[j] = 0;
            }
        }

        for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            int j = MyMap.Owner(i);
            if (j > 0)
            {
                if (NumOccupied[j] != -1)
                {
                    NumOccupied[j] = NumOccupied[j] + 1;
                    NumTroops[j] = NumTroops[j] + MyMap.TroopCount(i);
                }
            }
        }
    }
}