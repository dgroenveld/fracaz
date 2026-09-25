using System.Runtime.InteropServices;

namespace Fracaz;

public static class Declarations
{
    // Since VB arrays are 1-based and C# arrays are 0-based, we add one to the size of all arrays.
    public const int ONE_ARRAY_FIX = 1;

    public const int SRCCOPY = 0xCC0020;        // (DWORD) dest = source
    public const int SRCINVERT = 0x660046;      // (DWORD) dest = source XOR dest
    public const int SRCPAINT = 0xEE0086;       // (DWORD) dest = source OR dest
    public const int SRCAND = 0x8800C6;         // (DWORD) dest = source AND dest

    public const string VersionStr = "Fracas# 1.0";
    public const string WebStr = "";
    public const string EmailStr = "";

    public static int GameMode;
    public const int GM_TITLE_SCREEN = 1;
    public const int GM_BUILDING_MAP = 2;
    public const int GM_BUILDING_TITLE = 3;
    public const int GM_DIALOG_OPEN = 4;
    public const int GM_TITLE_DIALOG_OPEN = 5;
    public const int GM_GAME_ACTIVE = 6;

    // MapMade tells us that at least one map has been made during this session.
    public static bool MapMade = false;

    // These parameters are used to parse menu settings.
    public static int MinLakeSize;
    public static int MaxCountrySize;
    public static int CFGCountrySize;
    public static int CFGLandPct;
    public static int CFGIslands;
    public static int CFGLakeSize;
    public static int CFGProportion;
    public static int CFGShape;
    public static int CFGBorders;
    public static int CFGInitTroopPl;
    public static int CFGInitTroopCt;
    public static int CFGBonusTroops;
    public static int CFGShips;
    public static int CFGPorts;
    public static int CFGConquer;
    public static int CFGAISpeed;
    public static int CFGEvents;
    public static int CFG1st;
    public static int CFGHQSelect;
    public static int CFGSound;
    public static int CFGExplosions;          // Toggle for ball engine.
    public static int CFGWaves;               // Toggle for water animations.
    public static int CFGUnoccupiedColor;     // Color code of empty countries.
    public static int CFGFlashing;            // Do we flash countries while moving?
    public static int CFGResolution;          // Horizontal resolution we're currently at.
    public static int CFGPrompt;              // Prompt after the game is over?
    public static int MaxAllowedResolution;   // Which resolution is the max.
    public static double LandPct;
    public static double PropPct;
    public static double ShapePct;
    public static double CoastPctKeep;
    public static double IslePctKeep;
    public static float ShipPct;
    public static bool Done;

    // These are the screen dimensions.
    public static int Wide;
    public static int Tall;

    public const int XDIM640x480 = 66;
    public const int YDIM640x480 = 45; // Add 3 for 1.5 compatibility.
    public const int XDIM800x600 = 86;
    public const int YDIM800x600 = 60; // Add 3 for 1.5 compatibility.
    public const int XDIM1024x768 = 114;
    public const int YDIM1024x768 = 80;

    // Make some map variables public;
    public static Map? MyMap;
    public static int LastRightClick;

    // Number of times to loop through menu routines.
    public const int MAX_MENU_ITEMS = 7;

    //Gameplay constants.
    public const int MAX_COUNTRY_CAPACITY = 999;


    // Constants used to get around in the map builder.
    public const int TILEVAL_COASTLINE = 999;
    public const int MIN_COUNTRY_SIZE = 30;  // Max number of blocks on the map per country.
    public const int MAX_COUNTRY_SIZE = 150;
    public const float MAX_PROP_PCT = 0.7f;
    public const int MAX_COUNTRY_TRIES = 600;
    public const int MAX_TRIES_TO_PLACE_COUNTRY = 100;
    public const int UNUSABLE_GRID = -37;
    public const int MAXIMUM_NEIGHBORS = 20;
    public const int UP = 1;
    public const int DOWN = 2;
    public const int LEFTY = 3;
    public const int RIGHTY = 4;

    public const int GFX_NUM_LAND_PIECES = 19;
    public const int GFX_NUM_LAND_PIECE_SETS = 4;
    public const int GFX_NUM_PIER_PIECES = 14;
    public const int GFX_NUM_PIER_PIECE_SETS = 3;
    public const int GFX_GRID_HALF = 4;
    public const int GFX_GRID = 8;
    public const int GFX_GRID_X2 = 16;

    public const int GFX_PIER_X_BASE = 29;
    public const int GFX_DIGIT_X_BASE = 20;
    public const int GFX_EDIT_PIECE_X_BASE = 15;
    public const int GFX_BLACK_X_COORD = (4 * GFX_GRID);
    public const int GFX_WHITE_X_COORD = (17 * GFX_GRID);
    public const int GFX_WORK_X_COORD = (18 * GFX_GRID);
    public const int GFX_HQ_X_COORD = (19 * GFX_GRID);

    public const int GFX_MASK_Y_LINE = 12;
    public const int GFX_SUPP_Y_LINE = 14;
    public const int GFX_SUPP_MASK_Y_LINE = 15;

    public const int GFX_ICONS_X = 5;
    public const int GFX_ICONS_Y = 13;

    public const int GFX_LAND_MASK_Y_COORD = (GFX_MASK_Y_LINE * GFX_GRID);
    public const int GFX_HQ_Y_COORD = (GFX_SUPP_Y_LINE * GFX_GRID);
    public const int GFX_WHITE_Y_COORD = (GFX_SUPP_MASK_Y_LINE * GFX_GRID);

    // Some directions, used for borders...
    public const int DIR_UP = 0;
    public const int DIR_RIGHT = 1;

    // These are the offsets (in pixels) for the map within the form container.
    public const int XOFFSET = 10;
    public const int YOFFSET = 27;

    // The maximum number of balls that we can track.
    public const int BALLMAX = 500;

    // The maximum number of waves in the ocean at one time.
    public const int WAVEMAX = 100;
    public const int OCEAN_X_SIZE = 200;
    public const int OCEAN_Y_SIZE = 150;

    // The gravitational constant (in pixels/timer cycle^2)
    public const int GRAVITY = 1;

    // These constants contain properties for each type of explosion used.
    // Note that some of these are multipliers, not absolutes.
    // Little splashes in the water.
    public const int SPLASH_COUNT = 18;
    public const int SPLASH_INTENSITY = 15;
    public const int SPLASH_SPREAD = 20;
    public const int SPLASH_ELASTIC = 0;
    public const int SPLASH_SIZE = 2;
    public const int SPLASH_COLOR = 0;

    // A small explosion.
    public const float SMALL_COUNT = 0.2f;
    public const float SMALL_INTENSITY = 0.03f;
    public const int SMALL_SPREAD = 25;
    public const int SMALL_ELASTIC = 50;
    public const int SMALL_SIZE = 2;

    // A medium explosion.
    public const float MED_COUNT = 0.3f;
    public const float MED_INTENSITY = 0.05f;
    public const int MED_SPREAD = 30;
    public const int MED_ELASTIC = 55;
    public const int MED_SIZE = 3;

    // A big explosion.
    public const float BIG_COUNT = 0.4f;
    public const float BIG_INTENSITY = 0.07f;
    public const int BIG_SPREAD = 35;
    public const int BIG_ELASTIC = 60;
    public const int BIG_SIZE = 4;

    // Bonus twinkles for one country.
    public const int BONUS_COUNT = 120;
    public const int BONUS_INTENSITY = 80;
    public const int BONUS_SPREAD = 25;
    public const int BONUS_ELASTIC = 0;
    public const int BONUS_SIZE = 7;

    // Small bonus twinkles for many countries at once.
    public const int SMBONUS_COUNT = 20;
    public const int SMBONUS_INTENSITY = 85;
    public const int SMBONUS_SPREAD = 19;
    public const int SMBONUS_ELASTIC = 0;
    public const int SMBONUS_SIZE = 9;

    // The PlayerColors array contains text versions of each player color
    // for display purposes. The PlayerColorCodes array contains the color itself.
    public static string[] PlayerColors = new string[13];
    public static int[] PlayerColorCodes = new int[13];
    public static int[] PlayerTextColor = new int[13];

    // The Player array contains the color code of each player,
    // used for identification purposes. The PlayerName is each player's name.
    public const int MAX_PLAYERS = 6;
    public static int[] Player = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
    public static string[] PlayerName = new string[MAX_PLAYERS + ONE_ARRAY_FIX];

    // The PlayerType array tells whether or not this player is a computer.
    // 3 = Networked Human, 2 = Computer Controlled, 1 = Local Human, 0 = Inactive.
    public static int[] PlayerType = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int[] TempPlayerType = new int[MAX_PLAYERS + ONE_ARRAY_FIX];   // Used by clients for saving and displaying icons.
    public const int PTYPE_INACTIVE = 0;
    public const int PTYPE_HUMAN = 1;
    public const int PTYPE_COMPUTER = 2;
    public const int PTYPE_NETWORK = 3;
    public const int PTYPE_NET_AVAIL = 4; // Only used when clients are choosing color.
    public const int PTYPE_SERVER = 5; // Used by clients. The server controls this player.

    // The Personality array tells which computer personality this computer
    // player is.
    public static int[] Personality = new int[MAX_PLAYERS + ONE_ARRAY_FIX];

    // Turn contains the player whose turn it is (1-6).
    // Phase contains the phase of the current turn:
    // 1 = receive/place troops
    // 2 = attack
    // 3,4 = move troops
    public static int TurnCounter;

    //These are used to ensure that troop additions, actions, and passes only happen once.
    public static bool AlreadyAddedTroopsThisPhase;
    public static bool AlreadyPerformedActionThisPhase;
    public static bool AlreadyChoseTroopsThisPhase;
    public static bool AlreadyMovedTroopsThisPhase;
    public static bool AlreadyPassedThisPhase;
    public static bool AlreadyCheckedForMaxedOutCountries;

    // WonTurn is used to capture the winner of the game.
    // Since the turns and phases are dynamic, we need to be able to
    // freeze the number.
    public static int WonTurn;
    public static bool WonGame;

    // NumOccupied contains the number of countries owned by each player.
    // NumTroops contains the total number of troops owned by each player.
    // Used for AI and for tiebreaking situations.
    public static int[] NumOccupied = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int[] NumTroops = new int[MAX_PLAYERS + ONE_ARRAY_FIX];

    // NumPlayers contains the number of people playing this game (2-6).
    // NumNetworkPlayers is the number of networked humans the host will talk to.
    public static int NumPlayers;

    // These variables are used to calculate country strengths.
    public static int AttackStrength;
    public static int DefendStrength;
    public static int WaterAttackStrength;
    public static int WaterDefendStrength;

    // The following four variables keep count of the number of countries
    // in each category, for display purposes when right-clicking.
    public static int AttLndNum;
    public static int AttWtrNum;
    public static int DefLndNum;
    public static int DefWtrNum;

    // The following variables control the random events that appear from
    // time to time.  CheckedForEvent is false until the probability of an
    // event has been calculated.  When EventInProgress is true, it means
    // that game flow is suspended until it goes false again.
    public static bool CheckedForEvent;
    public static bool EventInProgress;

    // TroopMoveSrc needs to be global so that we can keep track of which
    // country is currently selected during a troop movement.
    public static int TroopMoveSrc;

    // The path to the last map played and its time stamp.
    public static string LastMapPath = string.Empty;
    public static string LastMapStamp = string.Empty;

    // Statistical information storage.
    public static int[,] STATattacked = new int[MAX_PLAYERS + ONE_ARRAY_FIX, MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int[,] STATovertaken = new int[MAX_PLAYERS + ONE_ARRAY_FIX, MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int[,] STATkilled = new int[MAX_PLAYERS + ONE_ARRAY_FIX, MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int[,] STATdefeated = new int[MAX_PLAYERS + ONE_ARRAY_FIX, MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int[] STATscore = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int[] STATrank = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int[] STATcountries = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int[] STATtroops = new int[MAX_PLAYERS + ONE_ARRAY_FIX];

    // High score information storage.
    public const int NUM_HI_SCORES = 10;
    public static int[] HiScore = new int[NUM_HI_SCORES + ONE_ARRAY_FIX]; // The 10 high scores for this map.
    public static string[] HiScoreName = new string[NUM_HI_SCORES + ONE_ARRAY_FIX]; // And the people who got them!
    public static int[] HiScoreColor = new int[NUM_HI_SCORES + ONE_ARRAY_FIX]; // The color of each.

    // The following is a bunch of declares for sound.
    [DllImport("winmm.dll")]
    public static extern int sndPlaySound(string lpszSoundName, int uFlags);
    public const int SND_ALIAS = 0x10000;       // name is a WIN.INI [sounds] entry 
    public const int SND_ALIAS_ID = 0x110000;   // name is a WIN.INI [sounds] entry identifier
    public const int SND_ALIAS_START = 0;       // must be > 4096 to keep strings in same section of resource file
    public const int SND_APPLICATION = 0x80;    // look for application specific association
    public const int SND_ASYNC = 0x1;           // play asynchronously
    public const int SND_FILENAME = 0x20000;    // name is a file name
    public const int SND_LOOP = 0x8;            // loop the sound until next sndPlaySound
    public const int SND_MEMORY = 0x4;          // lpszSoundName points to a memory file
    public const int SND_NODEFAULT = 0x2;       // silence not default, if sound not found
    public const int SND_NOSTOP = 0x10;         // don't stop any currently playing sound
    public const int SND_NOWAIT = 0x2000;       // don't wait if the driver is busy
    public const int SND_PURGE = 0x40;          // purge non-static events for task
    public const int SND_RESERVED = -16777216;  // In particular these flags are reserved
    public const int SND_RESOURCE = 0x40004;    // name is a resource name or atom
    public const int SND_SYNC = 0x0;            // play synchronously (default)
    public const int SND_TYPE_MASK = 0x170007;
    public const int SND_VALID = 0x1F;          // valid flags          / ;Internal /
    public const int SND_VALIDFLAGS = 0x17201F; // Set of valid flag bits.Anything outside

    //public const int SND_GAME_FLAGS = SND_ASYNC | SND_NOSTOP | SND_NOWAIT;
    // The flags SND_ASYNC | SND_NOSTOP | SND_NOWAIT are in the original source code, but it made the sound not quite right.
    public const int SND_GAME_FLAGS = SND_ASYNC | SND_NOWAIT;

    // Paths.
    public static string INIpath = string.Empty;
    public static string HLPpath = string.Empty;
    public static string MAPpath = string.Empty;
    public static string SFXpath = string.Empty;

    // flag that the game is ending (ie, the user wants to exit)
    public static bool GameEnding = false;

    public static void SetupPaths()
    {
        INIpath = AppDomain.CurrentDomain.BaseDirectory + "\\Fracas.ini";
        HLPpath = AppDomain.CurrentDomain.BaseDirectory + "\\Fracas.hlp";
        MAPpath = AppDomain.CurrentDomain.BaseDirectory + "\\Maps\\";
        SFXpath = AppDomain.CurrentDomain.BaseDirectory + "\\Sfx\\";
    }

    public static int vbBlack = Color.Black.ToArgb();
    public static int vbWhite = Color.White.ToArgb();

    public static int NumNetworkPlayers()
    {
        int i;
        int NumNetworkPlayers = 0;

        for (i = 1; i <= 6; i++)
        {
            if (PlayerType[i] == PTYPE_NETWORK)
            {
                NumNetworkPlayers = NumNetworkPlayers + 1;
            }
        }

        return NumNetworkPlayers;
    }
}

