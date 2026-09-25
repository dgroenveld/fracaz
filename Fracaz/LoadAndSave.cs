using static Fracaz.Declarations;
using static Fracaz.NetworkPlay;

namespace Fracaz;

public static class LoadAndSave
{

    public static int LoadMap(bool OptionsOnly)
    {
        int loadMap = 0;
        var CD1 = new OpenFileDialog();

        //  Set up the common dialog control.
        CD1.Title = "Load Map";
        CD1.FileName = "";
        CD1.Filter = "Map Files (.map)|*.map|All Files|*.*";
        CD1.FilterIndex = 1;
        CD1.InitialDirectory = MAPpath;

        // 'This displays it.
        var dialogResult = CD1.ShowDialog();

        if (dialogResult == DialogResult.Cancel) return loadMap;

        // If we got here, then the user didn't cancel the dialog.
        string MapFile = CD1.FileName;

        // If the file the user chose doesn't exist, we exit.
        if (string.IsNullOrEmpty(MapFile))
        {
            MessageBox.Show("The specified map could not be found.  Please check the filename and try again.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return loadMap;
        }

        loadMap = ReadMapDataKitchenSink(MapFile, OptionsOnly);

        return loadMap;
    }

    public static void SaveMap()
    {
        // Set up the common dialog control.
        var CD1 = new SaveFileDialog();
        CD1.Title = "Save Current Map";
        if (!string.IsNullOrEmpty(MyMap!.MapName))
        {
            CD1.FileName = RawMapName(MyMap.MapName) + ".map";
        }
        else
        {
            CD1.FileName = "Fracas.map";
        }
        CD1.Filter = "Map Files (.map)|*.map|All Files|*.*";
        CD1.FilterIndex = 1;
        CD1.InitialDirectory = MAPpath;
        CD1.OverwritePrompt = true;
        CD1.RestoreDirectory = true;

        // This displays it.
        var dialogResult = CD1.ShowDialog();
        if (dialogResult == DialogResult.Cancel) return;

        // If we got here, then the user didn't cancel the dialog.
        string MapFile = CD1.FileName;

        // First, put all of our INI settings in the save file.
        MakeINI(MapFile);

        // Assign our file name to the map.  In case the file is moved, this will
        // be updated on the next save for whatever reason.
        MyMap.MapName = MapFile;

        // Now write physical map data to the file.
        WriteMapData(MapFile);
    }

    public static int LoadGame()
    {
        int LoadGame = 0;

        // Set up the common dialog control.
        var CD1 = new OpenFileDialog();
        CD1.Title = "Load Saved Game";
        CD1.FileName = "";
        CD1.Filter = "Save Files (.sav)|*.sav|All Files|*.*";
        CD1.FilterIndex = 1;
        CD1.InitialDirectory = MAPpath;
        // This displays it.
        var dialogResult = CD1.ShowDialog();
        if (dialogResult == DialogResult.Cancel) return LoadGame;

        // If we got here, then the user didn't cancel the dialog.
        string GameFile = CD1.FileName;

        // If the file the user chose doesn't exist, we exit.
        if (!File.Exists(GameFile))
        {
            MessageBox.Show("The specified game could not be found.  Please check the filename and try again.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return LoadGame;
        }

        // Grab the map path and timestamp from the saved game.
        string MyMapPath = "";
        string MyMapStamp = "";
        using (var reader = new StreamReader(GameFile))
        {
            string? CurrentLine;
            // Get to the map info.
            while ((CurrentLine = reader.ReadLine()) != null)
            {
                if (CurrentLine == "{Map Path}")
                {
                    // Grab the path to the map we apply to.
                    MyMapPath = reader.ReadLine() ?? "";
                    CurrentLine = reader.ReadLine() ?? "";
                    MyMapStamp = StringAfterEqual(CurrentLine);
                    break;
                }
            }
        }

        // We found a path, now verify that it is valid.
        if (MyMapPath != "")
        {
            if (File.Exists(MyMapPath))
            {
                // We have a file there, so let's grab all information in it.
                LoadGame = ReadMapDataKitchenSink(MyMapPath, false);
                // If the stamp in the map doesn't match the stamp in the saved game, return 0.
                if (MyMapStamp != MyMap!.MapStamp)
                {
                    LoadGame = 0;
                    MessageBox.Show("This saved game does not apply to this map.", "Timestamp Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return LoadGame;
                }
                // Now that our map structure is in place, populate all game data from file.
                if (LoadGame > 0) LoadGame = ReadGameData(GameFile);
            }
            else
            {
                MessageBox.Show("Could not locate the map to which this saved game applies.", "Map Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return LoadGame;
            }
        }
        else
        {
            System.Windows.Forms.MessageBox.Show("Could not locate the map to which this saved game applies.", "Map Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return LoadGame;
        }

        return LoadGame;
    }


    public static int ReadMapDataKitchenSink(string MapFile, bool OptionsOnly)
    {
        // Called by load map and load game routines to grab everything from the map file.
        // At this point we've verified that MapFile is good.
        int result = 0;

        // Get all the menu settings in the map...
        ReadINI(MapFile);

        // Update our menus to reflect the saved config settings.
        ClearMenuChecks();
        UpdateMenus();

        // Now that we have a new resolution, see if it's valid.
        if (CFGResolution > MaxAllowedResolution)
        {
            MessageBox.Show("Saved map size is larger than the desktop and cannot be used.", "Map Too Large", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return result;
        }
        else
        {
            // Resolution is valid, set up map.
            Land.Refs.ChangeRes();
        }

        if (OptionsOnly == false)
        {
            // Get the physical map information from the same file.
            ReadMapData(MapFile);

            // Some map parameters can be calculated instead of saved, so do these now.
            MyMap!.FinishUpLoad();

            // Recalculate ShipPct.  Fudge.
            Land.Refs.CollectMenuSettings(true);
        }

        result = 1;

        return result;
    }

    public static void SaveGame(int Turn, int Phase)
    {
        // This sub saves the current game.  Note that we keep track of which map this
        // saved game applies to.

        var CD1 = new SaveFileDialog();
        CD1.Title = "Save Current Game";
        CD1.FileName = RawMapName(MyMap!.MapName) + ".sav";
        CD1.Filter = "Save Files (.sav)|*.sav|All Files|*.*";
        CD1.FilterIndex = 1;
        CD1.InitialDirectory = MAPpath;
        CD1.OverwritePrompt = true;
        CD1.RestoreDirectory = true;

        // This displays it.
        var dialogResult = CD1.ShowDialog();
        if (dialogResult == DialogResult.Cancel) return;

        // If we got here, then the user didn't cancel the dialog.
        string GameFile = CD1.FileName;

        // We will update the map file as well to take into account things
        // that may have changed like country names, etc.
        QuickMapUpdate();

        // Now write game data to the save file.
        WriteGameData(GameFile, Turn, Phase);
    }

    public static void QuickMapUpdate()
    {
        // Quickly rewrite our map file.
        string MapFile = MyMap!.MapName;
        MakeINI(MapFile);
        WriteMapData(MapFile);
    }

    private static void ReadMapData(string MapFile)
    {
        // This sub reads MAP data from the passed file path.  MAP data is the physical
        // structure of the map -- its dimensions, land/water squares, and names.

        try
        {

            // Menu settings are done, now grab map info.
            var lines = File.ReadAllLines(MapFile);

            var mapDataSegmentIndex = Array.FindIndex(lines, line => line == "{Menu and Map Data}");

            var TempNum1 = int.Parse(lines[mapDataSegmentIndex + 2]);
            var TempNum2 = int.Parse(lines[mapDataSegmentIndex + 3]);

            // Redim arrays.
            MyMap!.RedimensionStuff(TempNum1, TempNum2);

            // Input map contents.
            for (var sj = 1; sj <= MyMap.Ysize; sj++)
            {
                var line = lines[mapDataSegmentIndex + 4 + sj];

                // Grab each coordinate separately...
                var countryIds = line.Split(".");
                for (var j = 1; j <= MyMap.Xsize; j++)
                {
                    var countryId = countryIds[j - 1];
                    MyMap.Grid(j, sj, int.Parse(countryId));
                }
            }

            // Input country names.
            var countryNamesSegmentIndex = Array.FindIndex(lines, line => line == "{Country Names}");

            for (var sk = 1; sk <= TempNum1; sk++)
            {
                var countryName = lines[countryNamesSegmentIndex + sk];
                MyMap.CountryName(sk, countryName);
            }

            // Input water mass names.
            if (TempNum2 > 1001)
            {

                var waterNamesSegmentIndex = Array.FindIndex(lines, line => line == "{Water Names}");

                for (var n = 1; n <= TempNum2 - 1000; n++)
                {
                    var waterName = lines[waterNamesSegmentIndex + n];
                    MyMap.WaterName(n, waterName);
                }
            }

            var hiScoresSegmentIndex = Array.FindIndex(lines, line => line == "{Hi Scores}");

            for (var sk = 1; sk < NUM_HI_SCORES; sk++)
            {
                var hiScoreLine = lines[hiScoresSegmentIndex + sk];
                var parts = hiScoreLine.Split("=")[1].Split(",");
                HiScoreName[sk] = parts[0];
                HiScore[sk] = int.Parse(parts[1]);
                HiScoreColor[sk] = int.Parse(parts[2]);
            }

            MyMap.MapName = MapFile;
        }
        catch (IOException ex)
        {
            MessageBox.Show("A file error occurred. Code:\nReadMapData - " + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception)
        {
            throw;
        }
    }


    private static void WriteMapData(string MapFile)
    {

        // This sub writes all physical map data to the passed file.
        // Physical data includes country dimensions and placement, names, and high scores.

        try
        {

            using (var writer = new StreamWriter(MapFile, true))
            {
                writer.WriteLine();

                // Write governing numbers.
                writer.WriteLine("{Menu and Map Data}");
                writer.WriteLine(MyMap!.MapName);
                writer.WriteLine(MyMap.NumberOfCountries);
                writer.WriteLine(MyMap.LakeCode);

                // Write the entire map contents.
                writer.WriteLine("{Map}");
                for (var sj = 1; sj <= MyMap.Ysize; sj++)
                {
                    var currentLine = "";
                    for (var si = 1; si <= MyMap.Xsize; si++)
                    {
                        currentLine += MyMap.Grid(si, sj).ToString().Trim() + ".";
                    }
                    writer.WriteLine(currentLine);
                }

                // Write the country names.
                writer.WriteLine("{Country Names}");
                for (var sk = 1; sk <= MyMap.NumberOfCountries; sk++)
                {
                    writer.WriteLine(MyMap.CountryName(sk));
                }

                // Write the water mass names.
                writer.WriteLine("{Water Names}");
                if (MyMap.LakeCode > 1001)
                {
                    for (var sk = 1; sk <= MyMap.LakeCode - 1000; sk++)
                    {
                        writer.WriteLine(MyMap.WaterName(sk));
                    }
                }

                // Write the high scores.
                writer.WriteLine("{Hi Scores}");
                for (var sk = 1; sk <= NUM_HI_SCORES; sk++)
                {
                    writer.WriteLine("HI" + sk.ToString().Trim() + "=" + HiScoreName[sk].Trim() + "," + HiScore[sk].ToString().Trim() + "," + HiScoreColor[sk].ToString().Trim());
                }
            }
        }
        catch (IOException ex)
        {
            MessageBox.Show("A file error occurred. Code:\nWriteMapData - " + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static int ReadGameData(string GameFile)
    {
        int ReadGameData = 0;
        try
        {
            // Grab map info.
            using (var reader = new StreamReader(GameFile))
            {
                string? CurrentLine;
                // At this point, we've read our map data.  Now finish with the saved game.
                while ((CurrentLine = reader.ReadLine()) != null)
                {
                    if (!CurrentLine.StartsWith(";")) // Ignore comments.
                    {
                        string TempStr = StringBeforeEqual(CurrentLine);
                        if (!string.IsNullOrEmpty(TempStr)) // Ignore lines without an assignment.
                        {
                            int TempInt = int.TryParse(StringAfterEqual(CurrentLine), out TempInt) ? TempInt : 0;
                            switch (TempStr)
                            {
                                case "Turn":
                                    Land.Refs.SetTurn(TempInt);
                                    break;
                                case "Phase":
                                    Land.Refs.SetPhase(TempInt);
                                    break;
                                case "TurnCounter":
                                    TurnCounter = TempInt;
                                    break;
                                // Player data.
                                case "Player1":
                                case "Player2":
                                case "Player3":
                                case "Player4":
                                case "Player5":
                                case "Player6":
                                    TempInt = Convert.ToInt32(TempStr[^1].ToString()); // Take the number off the end...
                                    NumOccupied[TempInt] = Convert.ToInt32(StringAfterEqual(CurrentLine).Split(',')[0]);
                                    NumTroops[TempInt] = Convert.ToInt32(StringAfterEqual(CurrentLine).Split(',')[1]);
                                    break;
                            }
                            // Country data.
                            if (TempStr.StartsWith("C"))
                            {
                                int TempLong = Convert.ToInt32(TempStr[1..]); // Strip off the C.
                                MyMap!.Owner(TempLong, Convert.ToInt32(StringAfterEqual(CurrentLine).Split(',')[0]));
                                MyMap.TroopCount(TempLong, Convert.ToInt32(StringAfterEqual(CurrentLine).Split(',')[1]));
                                MyMap.CountryType(TempLong, Convert.ToInt32(StringAfterEqual(CurrentLine).Split(',')[2]));
                                MyMap.CountryColor(TempLong, Convert.ToInt32(StringAfterEqual(CurrentLine).Split(',')[3]));
                            }
                            // Stats.
                            if (TempStr.StartsWith("STAT"))
                            {
                                int i = Convert.ToInt32(TempStr[^1].ToString()); // Get the player number...
                                for (int j = 1; j <= MAX_PLAYERS; j++)
                                {
                                    var statsValues = StringAfterEqual(CurrentLine).Split(',');
                                    STATattacked[i, j] = Convert.ToInt32(statsValues[(j - 1) * 4 + 0]);
                                    STATovertaken[i, j] = Convert.ToInt32(statsValues[(j - 1) * 4 + 1]);
                                    STATkilled[i, j] = Convert.ToInt32(statsValues[(j - 1) * 4 + 2]);
                                    STATdefeated[i, j] = Convert.ToInt32(statsValues[(j - 1) * 4 + 3]);
                                }
                            }
                        }
                    }
                }
            }
            ReadGameData = 1;
        }
        catch (IOException ex)
        {
            MessageBox.Show("A file error occurred. Code:\nReadGameData - " + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception)
        {
            throw;
        }

        return ReadGameData;
    }

    private static void WriteGameData(string GameFile, int Turn, int Phase)
    {
        try
        {

            // This sub writes relevant game data to the passed file.
            if (System.IO.File.Exists(GameFile))
            {
                System.IO.File.Delete(GameFile);
            }

            using (var writer = new StreamWriter(GameFile, false))
            {
                writer.WriteLine(";" + VersionStr); // Version info.
                writer.WriteLine(";" + WebStr);     // Shameless self-promotion.
                writer.WriteLine(";" + EmailStr);   // More shameless self-promotion.

                // Write the path of this map first.  This is the map we'll apply these
                // game parameters to when we load.
                writer.WriteLine();
                writer.WriteLine("{Map Path}");
                writer.WriteLine(MyMap!.MapName);

                writer.WriteLine("MatchToStamp=" + MyMap.MapStamp);
                writer.WriteLine();

                // Write Game parameters.
                writer.WriteLine("{Game Data}");
                writer.WriteLine("Turn=" + Turn.ToString().Trim());
                writer.WriteLine("Phase=" + Phase.ToString().Trim());
                writer.WriteLine("TurnCounter=" + TurnCounter.ToString().Trim());

                // Write Country data.
                writer.WriteLine();
                writer.WriteLine("{Country Data}");
                for (var i = 1; i <= MyMap.NumberOfCountries; i++)
                {
                    writer.WriteLine("C" + i.ToString().Trim() + "=" +
                                     MyMap.Owner(i).ToString().Trim() + "," +
                                     MyMap.TroopCount(i).ToString().Trim() + "," +
                                     MyMap.CountryType(i).ToString().Trim() + "," +
                                     MyMap.CountryColor(i).ToString().Trim());
                }

                // Write Player data.
                writer.WriteLine();
                writer.WriteLine("{Player Data}");
                for (var i = 1; i <= 6; i++)
                {
                    writer.WriteLine("Player" + i.ToString().Trim() + "=" +
                                     NumOccupied[i].ToString().Trim() + "," +
                                     NumTroops[i].ToString().Trim());
                }

                // Write statistical info.
                writer.WriteLine();
                writer.WriteLine("{Statistics}");
                for (var si = 1; si <= MAX_PLAYERS; si++)
                {
                    var TempStr = "";
                    for (var sj = 1; sj <= MAX_PLAYERS; sj++)
                    {
                        TempStr += STATattacked[si, sj].ToString().Trim() + "," +
                                   STATovertaken[si, sj].ToString().Trim() + "," +
                                   STATkilled[si, sj].ToString().Trim() + "," +
                                     STATdefeated[si, sj].ToString().Trim() + ",";
                    }
                    writer.WriteLine("STAT" + si.ToString().Trim() + "=" + TempStr[..^1]);
                }
            }
        }
        catch (IOException ex)
        {
            MessageBox.Show("A file error occurred. Code:\nWriteGameData - " + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static void ReadINI(string INIFile)
    {
        // This routine will see if the passed file exists in the root folder.  If it does,
        // then we read in all of the config parameters.  If not, we create a default.
        // We call this the INI file, even though we also use this routine to read
        // menu settings from saved maps and saved games.

        string TempStr = string.Empty;
        string CurrentLine = string.Empty;
        int PlNum = 0;

        // If the file doesn't exist, we exit.
        if (!System.IO.File.Exists(INIFile))
        {
            // No INI file exists, so create the default.
            if (INIFile.EndsWith(".ini"))
            {
                MakeINI(INIFile);
            }
        }

        // If we got here, then the .ini file should exist, whether we created it or not.
        // Now parse it all out.
        foreach (var line in File.ReadAllLines(INIFile))
        {
            // Ignore comments.
            if (line.StartsWith(";")) continue;

            TempStr = StringBeforeEqual(line);

            // Ignore lines without an assignment.
            if (string.IsNullOrEmpty(TempStr)) continue;

            string value = StringAfterEqual(line);

            switch (TempStr)
            {
                case "CountrySize":
                    CFGCountrySize = Convert.ToInt32(value);
                    break;
                case "CountryProportions":
                    CFGProportion = Convert.ToInt32(value);
                    break;
                case "CountryShapes":
                    CFGShape = Convert.ToInt32(value);
                    break;
                case "MinLakeSize":
                    CFGLakeSize = Convert.ToInt32(value);
                    break;
                case "LandPct":
                    CFGLandPct = Convert.ToInt32(value);
                    break;
                case "Islands":
                    CFGIslands = Convert.ToInt32(value);
                    break;
                case "InitTroopPl":
                    CFGInitTroopPl = Convert.ToInt32(value);
                    break;
                case "InitTroopCt":
                    CFGInitTroopCt = Convert.ToInt32(value);
                    break;
                case "BonusTroops":
                    CFGBonusTroops = Convert.ToInt32(value);
                    break;
                case "Ships":
                    CFGShips = Convert.ToInt32(value);
                    break;
                case "Ports":
                    CFGPorts = Convert.ToInt32(value);
                    break;
                case "Conquer":
                    CFGConquer = Convert.ToInt32(value);
                    break;
                case "Events":
                    CFGEvents = Convert.ToInt32(value);
                    break;
                case "1stTurn":
                    CFG1st = Convert.ToInt32(value);
                    break;
                case "HQSelect":
                    CFGHQSelect = Convert.ToInt32(value);
                    break;
                case "Borders":
                    CFGBorders = Convert.ToInt32(value);
                    break;
                case "Sound":
                    CFGSound = Convert.ToInt32(value);
                    break;
                case "AISpeed":
                    CFGAISpeed = Convert.ToInt32(value);
                    break;
                case "AnimSpeed":
                    Land.Refs.BallTimer.Interval = Convert.ToInt32(value);
                    break;
                case "Explosions":
                    CFGExplosions = Convert.ToInt32(value);
                    break;
                case "Waves":
                    CFGWaves = Convert.ToInt32(value);
                    break;
                case "UnoccupiedColor":
                    CFGUnoccupiedColor = Convert.ToInt32(value);
                    break;
                case "Flashing":
                    CFGFlashing = Convert.ToInt32(value);
                    break;
                case "Resolution":
                    CFGResolution = Convert.ToInt32(value);
                    break;
                case "Prompt":
                    CFGPrompt = Convert.ToInt32(value);
                    break;

                // Player data.
                case "Player1":
                case "Player2":
                case "Player3":
                case "Player4":
                case "Player5":
                case "Player6":
                    PlNum = int.Parse(TempStr[^1..]); // Take the number off the end...
                    PlayerName[PlNum] = value.Split(",")[0];
                    Player[PlNum] = int.Parse(value.Split(",")[1]);
                    PlayerType[PlNum] = int.Parse(value.Split(",")[2]);
                    Personality[PlNum] = int.Parse(value.Split(",")[3]);
                    Land.Refs.Menu1st[PlNum].Text = PlayerName[PlNum];
                    break;
                // Time stamp.
                case "Created":
                    LastMapStamp = value;
                    break;
            }
        }

        UpdateMenus();
    }

    public static void MakeINI(string INIFile)
    {

        // This sub creates the passed file from scratch and puts menu settings in it.
        // It will always use whatever our current config settings are.
        // THIS IS ALWAYS THE FIRST THING IN A FRACAS FILE, whether it's a saved map,
        // saved game, or the .ini file.

        try
        {

            // If we've never saved this map before, time stamp it.
            if (!INIFile.EndsWith(".ini"))
            {
                if (string.IsNullOrEmpty(MyMap!.MapStamp))
                {
                    MyMap.MapStamp = DateTime.Now.ToString();
                }
            }

            if (System.IO.File.Exists(INIFile))
            {
                System.IO.File.Delete(INIFile);
            }

            using (var writer = new StreamWriter(INIFile, false))
            {
                writer.WriteLine(";" + VersionStr); // Version info.
                writer.WriteLine(";" + WebStr);     // Shameless self-promotion.
                writer.WriteLine(";" + EmailStr);   // More shameless self-promotion.
                if (!INIFile.EndsWith(".ini"))
                {
                    // Don't do this if we're writing the Fracas.ini file.
                    writer.WriteLine("Created=" + MyMap!.MapStamp);
                }
                // Now write the contents of the menus.
                writer.WriteLine();
                writer.WriteLine(";Terraform");
                writer.WriteLine("CountrySize=" + CFGCountrySize.ToString());
                writer.WriteLine("CountryProportions=" + CFGProportion.ToString());
                writer.WriteLine("CountryShapes=" + CFGShape.ToString());
                writer.WriteLine("MinLakeSize=" + CFGLakeSize.ToString());
                writer.WriteLine("LandPct=" + CFGLandPct.ToString());
                writer.WriteLine("Islands=" + CFGIslands.ToString());
                writer.WriteLine();
                writer.WriteLine(";Options");
                writer.WriteLine("InitTroopPl=" + CFGInitTroopPl.ToString());
                writer.WriteLine("InitTroopCt=" + CFGInitTroopCt.ToString());
                writer.WriteLine("BonusTroops=" + CFGBonusTroops.ToString());
                writer.WriteLine("Ships=" + CFGShips.ToString());
                writer.WriteLine("Ports=" + CFGPorts.ToString());
                writer.WriteLine("Conquer=" + CFGConquer.ToString());
                writer.WriteLine("Events=" + CFGEvents.ToString());
                writer.WriteLine("1stTurn=" + CFG1st.ToString());
                writer.WriteLine("HQSelect=" + CFGHQSelect.ToString());
                writer.WriteLine();
                writer.WriteLine(";Preferences");
                writer.WriteLine("Borders=" + CFGBorders.ToString());
                writer.WriteLine("Sound=" + CFGSound.ToString());
                writer.WriteLine("AISpeed=" + CFGAISpeed.ToString());
                writer.WriteLine("AnimSpeed=" + Land.Refs.BallTimer.Interval.ToString());
                writer.WriteLine("Explosions=" + CFGExplosions.ToString());
                writer.WriteLine("Waves=" + CFGWaves.ToString());
                writer.WriteLine("UnoccupiedColor=" + CFGUnoccupiedColor.ToString());
                writer.WriteLine("Flashing=" + CFGFlashing.ToString());
                writer.WriteLine("Resolution=" + CFGResolution.ToString());
                writer.WriteLine("Prompt=" + CFGPrompt.ToString());

                // Write Player info.
                writer.WriteLine();
                writer.WriteLine(";Players");
                for (int i = 1; i <= 6; i++)
                {
                    // Clients will use the TempPlayerType array instead of PlayerType because PlayerType
                    // has to be changed during a network game.
                    int TempType;
                    if (MyNetworkRole == NW_CLIENT)
                    {
                        TempType = TempPlayerType[i];
                    }
                    else
                    {
                        TempType = PlayerType[i];
                    }

                    // Write the line.
                    writer.WriteLine("Player" + i + "=" + PlayerName[i] + "," + Player[i] + "," + TempType + "," + Personality[i]);
                }
            }
        }
        catch (IOException ex)
        {
            MessageBox.Show("A file error occurred. Code:\nMakeINI - " + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception)
        {
            throw;

        }
    }

    private static string StringBeforeEqual(string MyStr)
    {
        // This function returns the last part of a string,
        // everything after the equals sign.
        int Pos;

        Pos = MyStr.IndexOf('=');

        if (Pos < 0) return "";

        return MyStr.Substring(0, Pos);
    }


    private static string StringAfterEqual(string MyStr)
    {
        // This function returns the last part of a string,
        // everything after the equals sign.
        int Pos;

        Pos = MyStr.IndexOf('=');

        if (Pos < 0) return "";

        return MyStr.Substring(Pos + 1, MyStr.Length - (Pos + 1));
    }

    public static string Arg(string MyStr, int ArgNum)
    {
        // This function returns the specified argument of the passed string.
        // Arguments are separated by commas.
        string TempStr = MyStr;
        for (int i = 1; i <= (ArgNum - 1); i++)
        {
            int Pos = TempStr.IndexOf(",");
            if (Pos == -1)
            {
                // Either this is the last argument or there was only one.
                // Either way, there's nothing more to do, so return what we have.
                return TempStr;
            }
            else
            {
                // We found a comma.  Cut off the first argument up to it.
                TempStr = TempStr.Substring(Pos + 1);
            }
        }

        // Now the string we want is the first argument in the string.
        int finalPos = TempStr.IndexOf(",");
        if (finalPos == -1)
        {
            // We're at the correct last argument.  Return it.
            return TempStr;
        }
        else
        {
            // Only return the string before the comma.
            return TempStr.Substring(0, finalPos);
        }
    }

    public static string ArgAt(string MyStr, int ArgNum)
    {
        // This function returns the specified argument of the passed string.
        // Arguments are separated by at signs (@).
        string TempStr = MyStr;
        for (int i = 1; i <= (ArgNum - 1); i++)
        {
            int Pos = TempStr.IndexOf("@");
            if (Pos == -1)
            {
                // Either this is the last argument or there was only one.
                // Either way, there's nothing more to do, so return what we have.
                return TempStr;
            }
            else
            {
                // We found a comma.  Cut off the first argument up to it.
                TempStr = TempStr.Substring(Pos + 1);
            }
        }
        // Now the string we want is the first argument in the string.
        int finalPos = TempStr.IndexOf("@");
        if (finalPos == -1)
        {
            // We're at the correct last argument.  Return it.
            return TempStr;
        }
        else
        {
            // Only return the string before the comma.
            return TempStr.Substring(0, finalPos);
        }
    }

    private static string RawMapName(string mapName)
    {
        // This sub extracts JUST the map name from the passed path.  In other words,
        // we return whatever is between the last \ and the last . (if it has a .)
        int pos1 = mapName.LastIndexOf("\\"); // String position of the last backslash.
        int pos2 = mapName.LastIndexOf("."); // String position of the last period.

        if (pos1 != -1 && pos2 != -1 && pos1 < pos2)
        {
            // There *is* a backslash, and a period, and the period is after the last backslash.
            return mapName.Substring(pos1 + 1, pos2 - pos1 - 1);
        }
        else if (pos1 != -1 && pos2 == -1)
        {
            // Backslash, but probably no period.  Return everything after the backslash.
            return mapName.Substring(pos1 + 1);
        }
        else
        {
            // Punt.  Return the whole thing!  I don't know what to do with it.
            return mapName;
        }

    }

    public static void UpdateMenus()
    {
        // This subroutine updates the menus with the cfg settings we have.

        // First, clear out all menu checks.
        ClearMenuChecks();

        // Now, populate each one with the appropriate check.
        Land.Refs.MenuBorders[CFGBorders].Checked = true;
        Land.Refs.MenuSfx[CFGSound].Checked = true;
        Land.Refs.MenuBonus[CFGBonusTroops].Checked = true;
        Land.Refs.MenuInitTroops[CFGInitTroopPl].Checked = true;
        Land.Refs.MenuConquer[CFGConquer].Checked = true;
        Land.Refs.MenuShips[CFGShips].Checked = true;
        Land.Refs.MenuInitTroopCts[CFGInitTroopCt].Checked = true;
        Land.Refs.MenuPorts[CFGPorts].Checked = true;
        Land.Refs.MenuRandom[CFGEvents].Checked = true;
        Land.Refs.Menu1st[CFG1st].Checked = true;
        Land.Refs.MenuHQSelect[CFGHQSelect].Checked = true;
        Land.Refs.MenuSize[CFGCountrySize].Checked = true;
        Land.Refs.MenuPct[CFGLandPct].Checked = true;
        Land.Refs.MenuLakeSize[CFGLakeSize].Checked = true;
        Land.Refs.MenuIslands[CFGIslands].Checked = true;
        Land.Refs.MenuShape[CFGShape].Checked = true;
        Land.Refs.MenuProp[CFGProportion].Checked = true;
        Land.Refs.MenuAISpeed[CFGAISpeed].Checked = true;
        Land.Refs.MenuResolution[CFGResolution].Checked = true;
    }

    public static void ClearMenuChecks()
    {
        // This sub erases the checkmarks on all menu items.

        for (int i = 0; i <= 6; i++)
        {
            if (i <= 2 && i > 0)
            {
                Land.Refs.MenuHQSelect[i].Checked = false;
                Land.Refs.MenuSfx[i].Checked = false;
            }
            if (i <= 3 && i > 0)
            {
                Land.Refs.MenuRandom[i].Checked = false;
                Land.Refs.MenuInitTroopCts[i].Checked = false;
                Land.Refs.MenuIslands[i].Checked = false;
                Land.Refs.MenuShape[i].Checked = false;
                Land.Refs.MenuProp[i].Checked = false;
                Land.Refs.MenuResolution[i].Checked = false;
            }
            if (i <= 4 && i > 0)
            {
                Land.Refs.MenuPorts[i].Checked = false;
                Land.Refs.MenuShips[i].Checked = false;
                Land.Refs.MenuLakeSize[i].Checked = false;
                Land.Refs.MenuAISpeed[i].Checked = false;
            }
            if (i <= 5 && i > 0)
            {
                Land.Refs.MenuInitTroops[i].Checked = false;
                Land.Refs.MenuConquer[i].Checked = false;
                Land.Refs.MenuPct[i].Checked = false;
            }
            if (i <= 5)
            {
                Land.Refs.MenuBonus[i].Checked = false;
            }
            if (i <= 6 && i > 0)
            {
                Land.Refs.MenuSize[i].Checked = false;
                Land.Refs.MenuBorders[i].Checked = false;
            }
            if (i <= 7 && i > 0)
            {
                Land.Refs.Menu1st[i].Checked = false;
            }
        }
    }
}
