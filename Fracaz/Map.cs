using Fracaz.Helpers;
using static Fracaz.Declarations;
using static Fracaz.NameGen;
using static Fracaz.TitleScreen;

namespace Fracaz;

public class Map
{
    public int XDimension { get; private set; }
    public int YDimension { get; private set; }

    private int Direction;
    private int LastDirection;
    private int TryNumber;
    private int Country;

    private bool CountryDone;
    private bool FoundLand;
    private bool FilledIn;

    private int NumCountries;

    public int LakeCode;
    public int MaxCountrySize;
    public int MinLakeSize;

    private int CountrySize;
    private int TentNum;
    private int Block;
    private int BlockTry;
    private int FudgeCounter;

    private string MapFileName = string.Empty;
    private string MapTimeStamp = string.Empty; // Used to link maps and saved games.

    public int[,] _Map { get; private set; } = new int[XDIM1024x768 + ONE_ARRAY_FIX, YDIM1024x768 + ONE_ARRAY_FIX];
    // Map Legend:
    // 0   = Plain water during build process (empty square)
    // x   = Country number x
    // 999 = Coastline during build process
    // 1000 and up = Bodies of water (determined after land placement)

    private int[,] Tent = new int[3, ((int)(MAX_COUNTRY_SIZE * (1 + MAX_PROP_PCT))) + 1];
    private int[] TentOrder = new int[((int)(MAX_COUNTRY_SIZE * (1 + MAX_PROP_PCT))) + 1];
    // Tent(1,a) = x coordinate of point a
    // Tent(2,a) = y coordinate of point a
    // TentOrder = a random ordering of the elements in Tent.

    float[,] Seed = new float[XDIM1024x768 + ONE_ARRAY_FIX, YDIM1024x768 + ONE_ARRAY_FIX];
    // The seed matrix contains the details of the random parts of the map.
    // This is for display purposes only -- Just a tweak.
    private int[,] Neighbor { get; set; } = new int[1, 1];
    // The Neighbor array will be populated with data about which
    // countries border others:
    // For example:  z = Neighbor(x,y)
    // x  = Country in question
    // y  = Index number
    // z  = The country number of a neighboring country.  Unused indexes return 0.
    // There will be no more than MAXIMUM_NEIGHBORS possible borders detected,
    // but this should be changed if the resolution is increased.
    public int[,] DisplaySpot { get; private set; } = new int[1, 1];
    // The DisplaySpot array will hold the x and y coordinates of a place on
    // each country to store information.  This will be populated once the map
    // is drawn.
    // For example:  z = DisplaySpot(x,y)
    // x = Country in question
    // If y = 1, z = x coordinate of spot
    // If y = 2, z = y coordinate of spot
    // If y = 3, z = number of squares of surrounding country (used during build)

    public int[] Owners { get; private set; } = new int[1];
    // 'The Owners array holds the owner of each country.
    // 0 = Neutral territory/unoccupied
    // x = Controlled by player x
    public int[] CountryClr { get; private set; } = new int[1];
    // The CountryClr array holds the color of each country.

    public int[] Troops { get; private set; } = new int[1];
    // The Troops array holds the number that will be displayed on top of each country.

    public int[] Special { get; private set; } = new int[1];
    // The Special array tells whether or not this country has special status.
    // These values are binary and can be combined for multiple effects.
    // 0 = Nothing special about this country
    // 1 = This country is a PORT (can participate in naval warfare).
    // 2 = This country is an HQ (the player's starting country).
    // 4 = ...

    public string[] CName { get; private set; } = new string[1];
    // The CName array holds the name of each country, for display purposes.

    public string[] WName { get; private set; } = new string[1];
    // The WName array holds the name of a body of water.  Subtract 1000
    // from the water ID to get the index.  For example, the name of water
    // mass 1005 would be WName(5).

    public int[,] FlashArray { get; private set; } = new int[1, 1];
    // FlashArray holds data regarding how to highlight each country and is
    // updated by a timer mechanism in GraphicFX.bas.
    // FlashArray(x, 1) = 1 if country x is highlighted currently, 0 if not
    // FlashArray(x, 2) = decrementing counter for country x
    // FlashArray(x, 3) = timer value for each flash of country x
    // FlashArray(x, 4) = number of times left to flash country x

    public int Xsize { get { return XDimension; } }

    public int Ysize { get { return YDimension; } }

    public int NumberOfCountries { get { return NumCountries; } }

    public int MaxNeighbors { get { return MAXIMUM_NEIGHBORS; } }

    public int Grid(int x, int y)
    {
        // We put bounds on the inputs so as not to generate subscript out
        // of range errors.  This is because balls may fly one or two
        // squares off of the map before they're caught.  :)

        if (x > XDimension) x = XDimension;
        if (y > YDimension) y = YDimension;
        if (x < 1) x = 1;
        if (y < 1) y = 1;
        return _Map[x, y];
    }

    internal void Grid(int x, int y, int z)
    {
        // We put bounds on the inputs so as not to generate subscript out
        // of range errors.  This is because balls may fly one or two
        // squares off of the map before they're caught.  :)
        if (x > XDimension) x = XDimension;
        if (y > YDimension) y = YDimension;
        if (x < 1) x = 1;
        if (y < 1) y = 1;

        _Map[x, y] = z;
    }

    internal int Neighbors(int x, int y)
    {
        return Neighbor[x, y];
    }

    internal int DigitCoords(int x, int y)
    {
        return DisplaySpot[x, y];
    }

    internal int Owner(int x)
    {
        return Owners[x];
    }

    internal void Owner(int x, int y)
    {
        Owners[x] = y;
    }

    internal int CountryColor(int x)
    {
        return CountryClr[x];
    }

    internal void CountryColor(int x, int y)
    {
        CountryClr[x] = y;
    }

    internal int TroopCount(int x)
    {
        return Troops[x];
    }

    internal void TroopCount(int x, int y)
    {
        Troops[x] = y;
    }

    internal int CountryType(int x)
    {
        return Special[x];
    }

    internal void CountryType(int x, int y)
    {
        Special[x] = y;
    }

    internal string CountryName(int xxx)
    {
        return CName[xxx];
    }

    internal void CountryName(int xxx, string yyy)
    {
        CName[xxx] = yyy;
    }

    internal string WaterName(int xxx)
    {
        return WName[xxx];
    }

    internal void WaterName(int xxx, string yyy)
    {
        WName[xxx] = yyy;
    }

    public string MapName { get { return MapFileName; } set { MapFileName = value; } }
    public string MapStamp { get { return MapTimeStamp; } set { MapTimeStamp = value; } }

    public void Flash(int CountryId, int ParamID, int z)
    {
        // We put bounds on the inputs so as not to generate subscript out
        // of range errors.  This is because balls may fly one or two
        // squares off of the map before they're caught.  :)
        if (CountryId > NumCountries) CountryId = NumCountries;
        if (ParamID > 4) ParamID = 4;
        if (CountryId < 1) CountryId = 1;
        if (ParamID < 1) ParamID = 1;
        FlashArray[CountryId, ParamID] = z;
    }

    internal void CreateMap(int CountryCount, int MaximumCountrySize, int MinimumLakeSize, double LandPct, double PropPct, double ShapePct, double CoastPctKeep, double IslePctKeep)
    {
        int x = 0;
        int y = 0;
        bool StillPossible;
        bool NewOrder;

        NumCountries = CountryCount;

        // We want some passed variables to stay public.
        MaxCountrySize = MaximumCountrySize;
        MinLakeSize = MinimumLakeSize;

        // Redimension our arrays to save some memory.
        Neighbor = new int[NumCountries + ONE_ARRAY_FIX, MAXIMUM_NEIGHBORS + ONE_ARRAY_FIX];
        CountryClr = new int[NumCountries + ONE_ARRAY_FIX];
        Owners = new int[NumCountries + ONE_ARRAY_FIX];
        DisplaySpot = new int[NumCountries + ONE_ARRAY_FIX, 3 + ONE_ARRAY_FIX];
        Troops = new int[NumCountries + ONE_ARRAY_FIX];
        Special = new int[NumCountries + ONE_ARRAY_FIX];
        CName = new string[NumCountries + ONE_ARRAY_FIX];
        FlashArray = new int[NumCountries + ONE_ARRAY_FIX, 4 + ONE_ARRAY_FIX];

        do // Loop through everything until we pass spec.
        {
            //All of map is considered coastline for placing of first country.
            for (var i = 1; i <= XDimension; i++)
            {
                for (var j = 1; j <= YDimension; j++)
                {
                    _Map[i, j] = TILEVAL_COASTLINE;
                }
            }

            Country = 0;

            while (Country < NumCountries)
            {
                Country++;
                StillKicking(); //Update our commentary line.

                FudgeCounter = 0;

                CountryDone = false;

                while (!CountryDone)    // This Country Loop.
                {
                    //First, make sure that we *can* place a country on this map.
                    //if we have a coastline or 0 we can still do it.
                    StillPossible = false;
                    for (var i = 1; i <= XDimension; i++)
                    {
                        for (var j = 1; j <= YDimension; j++)
                        {
                            if (_Map[i, j] == 0 || _Map[i, j] == TILEVAL_COASTLINE)
                            {
                                StillPossible = true;
                                break;
                            }
                        }
                    }
                    StillKicking();
                    Application.DoEvents();
                    if (StillPossible)
                    {
                        //Find the starting position of the next country.
                        Done = false;
                        do
                        {
                            x = Random.Shared.Next(1, XDimension + 1);
                            y = Random.Shared.Next(1, YDimension + 1);

                            //Take care of the None or Lots of Islands options.
                            if ((CFGIslands == 1) && (_Map[x, y] == TILEVAL_COASTLINE))
                            {
                                Done = true;
                            }
                            if ((CFGIslands == 3) && ((_Map[x, y] == 0) || (_Map[x, y] == TILEVAL_COASTLINE)))
                            {
                                Done = true;
                            }
                            //The 'Some Islands' option is more complex.
                            //If we found coastline, there's a large chance of keeping it.
                            //If we found empty, there's a small chance of keeping it.
                            if (CFGIslands == 2)
                            {
                                if (((_Map[x, y] == TILEVAL_COASTLINE) && (Random.Shared.NextDouble() < CoastPctKeep)) || ((_Map[x, y] == 0) && (Random.Shared.NextDouble() < IslePctKeep)))
                                {
                                    Done = true;
                                }
                            }
                        } while (!Done);
                    }
                    else
                    {
                        //Couldn't find a place for it, so we're done.
                        EndItAll();
                    }

                    if (StillPossible)
                    {
                        // Clear out the last country by clearing out the tentative array.
                        for (var i = 1; i <= 2; i++)
                        {
                            for (var j = 1; j <= MAX_COUNTRY_SIZE * (1 + MAX_PROP_PCT); j++)
                            {
                                Tent[i, j] = 0;
                            }
                        }

                        // This is our first tentative block.
                        Tent[1, 1] = x;
                        Tent[2, 1] = y;
                        TentNum = 1;

                        // Get a random size for this country.
                        if (CFGCountrySize != 6)
                        {
                            if (Random.Shared.NextDouble() < 0.5)
                            {
                                // Subtract a random amount within the proportional limit.
                                CountrySize = MaxCountrySize - (int)(Random.Shared.NextDouble() * MaxCountrySize * PropPct);
                            }
                            else
                            {
                                // Add a random amount within the proportional limit.  This will keep
                                // all countries equal to MaxCountrySize on average.
                                // (So it's not really a MAX then, is it?)
                                CountrySize = MaxCountrySize + (int)(Random.Shared.NextDouble() * MaxCountrySize * PropPct);
                            }
                        }
                        else
                        {
                            // Hodge-Podge:  Pick anything between min and max.
                            var LocalMax = (int)(MAX_COUNTRY_SIZE * (1 + MAX_PROP_PCT));
                            var LocalMin = (int)(MIN_COUNTRY_SIZE * (1 - MAX_PROP_PCT));
                            CountrySize = (int)(Random.Shared.NextDouble() * (LocalMax - LocalMin)) + LocalMin;
                        }

                        NewOrder = true;
                        // Now we search for blocks contiguous to this one.
                        while (TentNum != CountrySize) // Block Search Loop.
                        {
                            // Each block must be tried only once. Order them randomly,
                            // and pick a first choice before we get into this DO.  Then jump
                            // to the next one in order for each iteration.  The first choice
                            // will be based on the irregularity of the country.

                            if (NewOrder)
                            {
                                // Set up our array of integers.  There are TentNum integers to mix up.
                                // This is the order that we check elements in the Tent array.
                                for (var i = 1; i <= TentNum; i++)
                                {
                                    TentOrder[i] = i;
                                }

                                // Now randomize them a bit.  Don't move the last block.
                                if (TentNum >= 4)
                                {
                                    for (var i = 1; i <= TentNum * 2; i++)
                                    {
                                        do
                                        {
                                            x = (int)(Random.Shared.NextDouble() * (TentNum - 1)) + 1;
                                            y = (int)(Random.Shared.NextDouble() * (TentNum - 1)) + 1;
                                        } while (x == y);
                                        Block = TentOrder[x];
                                        TentOrder[x] = TentOrder[y];
                                        TentOrder[y] = Block;
                                    }
                                }

                                // When BlockTry reaches TentNum, we've tried to bud off of every
                                // square in this country.
                                BlockTry = 0;

                                // Regular or Irregular?
                                // Block is which element in the Tent array we're trying to 'bud' off of.
                                if (Random.Shared.NextDouble() < ShapePct)
                                {
                                    // Start at the first random block.
                                    Block = 1;
                                }
                                else
                                {
                                    // Start at the last block we used.  Makes it more irregular.
                                    Block = TentNum;
                                }
                            } // NewOrder = true

                            // Get the coordinates of a block in this country.
                            x = Tent[1, TentOrder[Block]];
                            y = Tent[2, TentOrder[Block]];

                            // Pick a random direction for a contiguous block.
                            NewDirection();

                            Done = false;

                            while (TryNumber != 5 && !Done) // Each dir.
                            {
                                switch (Direction)
                                {
                                    case UP:
                                        var OneAway = y - 1;
                                        if (OneAway > 0)
                                        {
                                            // Check and see what's up.
                                            if (ClearDirection(x, OneAway))
                                            {
                                                // Map is clear in that direction.
                                                y = y - 1;
                                                WriteToTent(x, y);
                                                Done = true;
                                            }
                                            else
                                            {
                                                // Blocked.  Try new direction.
                                                NextDirection();
                                            }
                                        }
                                        else
                                        {
                                            // We went off the top.  Try new direction.
                                            NextDirection();
                                        }
                                        break;
                                    case DOWN:
                                        OneAway = y + 1;
                                        if (OneAway <= YDimension)
                                        {
                                            // Check and see what's down.
                                            if (ClearDirection(x, OneAway))
                                            {
                                                // Map is clear in that direction.
                                                y = y + 1;
                                                WriteToTent(x, y);
                                                Done = true;
                                            }
                                            else
                                            {
                                                // Blocked.  Try new direction.
                                                NextDirection();
                                            }
                                        }
                                        else
                                        {
                                            // We went off the bottom.  Try new direction.
                                            NextDirection();
                                        }
                                        break;
                                    case LEFTY:
                                        OneAway = x - 1;
                                        if (OneAway > 0)
                                        {
                                            // Check and see what's up.
                                            if (ClearDirection(OneAway, y))
                                            {
                                                // Map is clear in that direction.
                                                x = x - 1;
                                                WriteToTent(x, y);
                                                Done = true;
                                            }
                                            else
                                            {
                                                // Blocked.  Try new direction.
                                                NextDirection();
                                            }
                                        }
                                        else
                                        {
                                            // We went off the left.  Try new direction.
                                            NextDirection();
                                        }
                                        break;
                                    case RIGHTY:
                                        OneAway = x + 1;
                                        if (OneAway <= XDimension)
                                        {
                                            // Check and see what's up.
                                            if (ClearDirection(OneAway, y))
                                            {
                                                // Map is clear in that direction.
                                                x = x + 1;
                                                WriteToTent(x, y);
                                                Done = true;
                                            }
                                            else
                                            {
                                                // Blocked.  Try new direction.
                                                NextDirection();
                                            }
                                        }
                                        else
                                        {
                                            // We went off the right.  Try new direction.
                                            NextDirection();
                                        }
                                        break;
                                }
                            } // Each dir.

                            if (TryNumber == 5)
                            {
                                // This block is boxed in.  Try the next block in the country.
                                NewOrder = false;
                                Block++;
                                if (Block > TentNum)
                                {
                                    Block = 1;
                                }
                                // Have we tried all blocks?
                                BlockTry++;
                                if (BlockTry > TentNum)
                                {
                                    // This Country cannot fit.  Need new starting location.
                                    // Remove this small area from further consideration -- probably a lake.
                                    for (var i = 1; i <= TentNum; i++)
                                    {
                                        // This space won't get picked next time.
                                        _Map[Tent[1, i], Tent[2, i]] = UNUSABLE_GRID;
                                    }
                                    TentNum = CountrySize; // Fudging out of loop.
                                    FudgeCounter++;
                                    StillPossible = false;
                                    // Move on to next country if we can't place this one
                                    // within a reasonable number of tries.
                                    if (FudgeCounter > MAX_COUNTRY_TRIES)
                                    {
                                        // Screw it ... taking too long.  We're done.
                                        EndItAll();
                                    }
                                }
                            }
                            else
                            {
                                NewOrder = true;
                            }
                        } // Block Search Loop.

                        // Yay!  Our country is sitting in the Tent array.
                        // Let's copy it over to the Map.
                        // If we blew it last time, then we don't need to do anything here.
                        if (StillPossible)
                        {
                            for (var i = 1; i <= CountrySize; i++)
                            {
                                _Map[Tent[1, i], Tent[2, i]] = Country;
                            }
                            // Let's outline all countries on the map with coastline.
                            // Because if there are no islands, we must build on coastline next time.
                            OutlineCoastline();
                        } // StillPossible.
                    } // StillPossible..
                } // This Country Loop.
            } // Main country loop.

            // Give us our final coastline.
            OutlineCoastline();

            // Fill in lakes according to lakesize parameter.
            FillLakes();

            //Populate the Neighbors array.
        } while (!FindNeighbors());

        // Find the best display spot for each country.
        FindSpots();

        // Initialize the seed matrix.
        BuildSeedMatrix();

        // Clear out the troops, special, and flash arrays.  Name each country too.
        for (var i = 1; i <= NumCountries; i++)
        {
            Troops[i] = 0;
            Special[i] = 0;
            for (var j = 1; j <= 4; j++)
            {
                FlashArray[i, j] = 0;
            }
            if (Neighbor[i, 2] == 0 && Neighbor[i, 1] > TILEVAL_COASTLINE)
            {
                //We have an island!  Let's give it an island name.
                CName[i] = GenerateName(2, 6, false);
                var TempNum = (int)(Random.Shared.NextDouble() * 3) + 23;
                if (TempNum == 25)
                {
                    CName[i] = NameSpice[TempNum] + " " + CName[i];
                }
                else
                {
                    CName[i] = CName[i] + " " + NameSpice[TempNum];
                }
            }
            else
            {
                CName[i] = GenerateName(2, 6, true);
            }
        }

        // Now let's name each body of water.
        if (LakeCode == 1001)
        {
            //There were no bodies of water in this map.
            return;
        }

        // Let's set up the array.
        WName = new string[LakeCode - 1000 + 1];

        for (var i = LakeCode - 1000; i >= 1; i--)
        {
            // Search the borders of the map for this lakecode.
            // Check the top and bottom borders.
            Done = false;
            for (var j = 1; j <= XDimension; j++)
            {
                if (_Map[j, 1] == (i + 1000) || _Map[j, YDimension] == (i + 1000))
                {
                    //Yup, it's on the border.
                    Done = true;
                    break;
                }
            }
            //Check the left and right borders.
            for (var j = 1; j <= YDimension; j++)
            {
                if (_Map[1, j] == (i + 1000) || _Map[XDimension, j] == (i + 1000))
                {
                    //Yup, it's on the border.
                    Done = true;
                    break;
                }
            }
            if (Done == false)
            {
                //We haven't found it on the edge, so it must be a LAKE.
                switch ((int)(Random.Shared.NextDouble() * 11) + 1)
                {
                    case 1:
                    case 2:
                    case 3:
                        WName[i] = GenerateName(2, 6, false) + " Lake";
                        break;
                    case 4:
                    case 5:
                    case 6:
                        WName[i] = "Lake " + GenerateName(2, 6, false);
                        break;
                    case 7:
                        WName[i] = GenerateName(2, 6, false) + " Vista";
                        break;
                    case 8:
                    case 9:
                        WName[i] = GenerateName(2, 6, false) + " Sea";
                        break;
                    case 10:
                    case 11:
                        WName[i] = "The Sea of " + GenerateName(2, 6, false);
                        break;
                }
            }
            else
            {
                //We found it on the edge of the map, so this is an OCEAN.
                switch ((int)(Random.Shared.NextDouble() * 7) + 1)
                {
                    case 1:
                        WName[i] = GenerateName(2, 6, false) + " Ocean";
                        break;
                    case 2:
                    case 3:
                        WName[i] = GenerateName(2, 6, false) + " Harbor";
                        break;
                    case 4:
                    case 5:
                        WName[i] = GenerateName(2, 6, false) + " Bay";
                        break;
                    case 6:
                    case 7:
                        WName[i] = "The Bay of " + GenerateName(2, 6, false);
                        break;
                }
            }
        }

        // Finally, this is a fresh map so it hasn't been saved yet.
        MapFileName = string.Empty;
        MapTimeStamp = string.Empty;
    }

    private void WriteToTent(int x, int y)
    {
        // Found a good contiguous block, lets record its X and Y.
        TentNum = TentNum + 1;
        Tent[1, TentNum] = x;
        Tent[2, TentNum] = y;

        // Was this the last tentative block?  If so, we need to signal we're done.
        if (TentNum == CountrySize)
        {
            CountryDone = true;
        }
    }

    private bool ClearDirection(int Xcheck, int Ycheck)
    {
        // This function checks the Map and Tent arrays to see if
        // the suggested block is already used.
        var clearDirection = true;

        // Is the suggested block part of another country?
        if (_Map[Xcheck, Ycheck] > 0 && _Map[Xcheck, Ycheck] < TILEVAL_COASTLINE)
        {
            clearDirection = false;
        }

        // Is the suggested block part of the current country (or lake)?
        for (var i = 1; i <= TentNum; i++)
        {
            if (Tent[1, i] == Xcheck && Tent[2, i] == Ycheck)
            {
                clearDirection = false;
            }
        }

        return clearDirection;
    }

    private void OutlineCoastline()
    {
        for (var i = 1; i <= XDimension; i++)
        {
            for (var j = 1; j <= YDimension; j++)
            {
                // If this was the first country, we erase all coastline.
                // Also erase coastline if we find our temporary block-out code.
                if ((Country == 1 && _Map[i, j] == TILEVAL_COASTLINE) || (_Map[i, j] == UNUSABLE_GRID))
                {
                    _Map[i, j] = 0;
                }
                if (_Map[i, j] == 0 || _Map[i, j] > TILEVAL_COASTLINE)
                {
                    FoundLand = false;
                    // Land up?
                    int x = i;
                    int y = j - 1;
                    if (y > 0)
                    {
                        if (_Map[x, y] > 0 && _Map[x, y] < TILEVAL_COASTLINE) FoundLand = true;
                    }
                    // Land down?
                    x = i;
                    y = j + 1;
                    if (y <= YDimension)
                    {
                        if (_Map[x, y] > 0 && _Map[x, y] < TILEVAL_COASTLINE) FoundLand = true;
                    }
                    // Land left?
                    x = i - 1;
                    y = j;
                    if (x > 0)
                    {
                        if (_Map[x, y] > 0 && _Map[x, y] < TILEVAL_COASTLINE) FoundLand = true;
                    }
                    // Land right?
                    x = i + 1;
                    y = j;
                    if (x <= XDimension)
                    {
                        if (_Map[x, y] > 0 && _Map[x, y] < TILEVAL_COASTLINE) FoundLand = true;
                    }
                    // Place coastline.
                    if (FoundLand == true)
                    {
                        _Map[i, j] = TILEVAL_COASTLINE;
                    }
                    else
                    {
                        _Map[i, j] = 0;
                    }
                }
            }
        }
    }

    private void EndItAll()
    {

        // We call this when we can go no further with placing countries.

        CountryDone = true;
        NumCountries = Country - 1;

        // Redimension our arrays to save some memory.
        Neighbor = new int[NumCountries + ONE_ARRAY_FIX, MAXIMUM_NEIGHBORS + ONE_ARRAY_FIX];
        CountryClr = new int[NumCountries + ONE_ARRAY_FIX];
        Owners = new int[NumCountries + ONE_ARRAY_FIX];
        DisplaySpot = new int[NumCountries + 1, 3 + ONE_ARRAY_FIX];
        Troops = new int[NumCountries + ONE_ARRAY_FIX];
        Special = new int[NumCountries + ONE_ARRAY_FIX];
        CName = new string[NumCountries + ONE_ARRAY_FIX];
        FlashArray = new int[NumCountries + 1, 4 + ONE_ARRAY_FIX];

    }

    private void GetAdjacentColor(int Xcheck, int Ycheck)
    {
        if (_Map[Xcheck, Ycheck] != 0 && _Map[Xcheck, Ycheck] != TILEVAL_COASTLINE && Country == 0)
        {
            Country = _Map[Xcheck, Ycheck];
        }
    }

    private bool IsOnCoastLine(int i, int j)
    {
        // This sub returns true if the passed coordinate is next to coastline.
        if (
            LookRight(i, j) >= TILEVAL_COASTLINE ||
            LookLeft(i, j) >= TILEVAL_COASTLINE ||
            LookUp(i, j) >= TILEVAL_COASTLINE ||
            LookDown(i, j) >= TILEVAL_COASTLINE ||
            LookRight(i, j) == 0 ||
            LookLeft(i, j) == 0 ||
            LookUp(i, j) == 0 ||
            LookDown(i, j) == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int LookRight(int ii, int jj)
    {
        var lookRight = -1;
        var xx = ii + 1;
        var yy = jj;
        if (xx <= XDimension)
        {
            lookRight = _Map[xx, yy];
        }

        return lookRight;
    }

    private int LookLeft(int ii, int jj)
    {
        var lookLeft = -1;
        var xx = ii - 1;
        var yy = jj;
        if (xx > 0)
        {
            lookLeft = _Map[xx, yy];
        }

        return lookLeft;
    }

    private int LookDown(int ii, int jj)
    {
        var lookDown = -1;
        var xx = ii;
        var yy = jj + 1;
        if (yy <= YDimension)
        {
            lookDown = _Map[xx, yy];
        }

        return lookDown;
    }

    private int LookUp(int ii, int jj)
    {
        var lookUp = -1;
        var xx = ii;
        var yy = jj - 1;
        if (yy > 0)
        {
            lookUp = _Map[xx, yy];
        }

        return lookUp;
    }

    private int LookUpRight(int ii, int jj)
    {
        var lookUpRight = -1;
        var xx = ii + 1;
        var yy = jj - 1;
        if (yy > 0 && xx <= XDimension)
        {
            lookUpRight = _Map[xx, yy];
        }

        return lookUpRight;
    }

    private int LookUpLeft(int ii, int jj)
    {
        var lookUpLeft = -1;
        var xx = ii - 1;
        var yy = jj - 1;
        if (yy > 0 && xx > 0)
        {
            lookUpLeft = _Map[xx, yy];
        }

        return lookUpLeft;
    }

    private int LookDownRight(int ii, int jj)
    {
        var lookDownRight = -1;
        var xx = ii + 1;
        var yy = jj + 1;
        if (yy <= YDimension && xx <= XDimension)
        {
            lookDownRight = _Map[xx, yy];
        }

        return lookDownRight;
    }
    private void NextDirection()
    {
        // The last direction didn't work, so we try a the next until we've done them all.
        Direction = Direction + 1;
        if (Direction == 5) Direction = 1;
        TryNumber = TryNumber + 1;
    }

    private void NewDirection()
    {
        // Record which way we went last time.
        LastDirection = Direction;
        if (LastDirection < 1 || LastDirection > 4)
        {
            LastDirection = RandomDir();
        }

        Direction = RandomDir();
        // This will be our first try for a new way to go.
        TryNumber = 1;
    }

    internal void DisplayMap(Bitmap Source, Bitmap dest)
    {
        int PieceNum = 0;

        int Xadj = 0;

        int Yadj = 0;

        int UpPiece = 0;
        int DownPiece = 0;
        int LeftPiece = 0;
        int RightPiece = 0;
        int UpRightPiece = 0;
        int UpLeftPiece = 0;
        int DownRightPiece = 0;

        int ii = 0;
        int jj = 0;
        int ThisPiece = 0;

        int PieceXcoord = 0;
        int PieceXbase = 0;
        int LineLength = 0;
        int MySeedVal = 0;

        //Let's update the map for everyone to see.
        using (var gdiOperations = new GDIOperations(dest, Source))
        {

            for (ii = 1; ii <= XDimension; ii++)
            {
                for (jj = 1; jj <= YDimension; jj++)
                {
                    ThisPiece = _Map[ii, jj];
                    if (ThisPiece > 0 && ThisPiece < TILEVAL_COASTLINE)
                    {

                        //This routine will determine which 'piece' gets used for this block.
                        //Initialize our counters.
                        PieceNum = 0;
                        UpPiece = LookUp(ii, jj);
                        DownPiece = LookDown(ii, jj);
                        RightPiece = LookRight(ii, jj);
                        LeftPiece = LookLeft(ii, jj);

                        //See if there's water or coastline to the right of us.
                        if (RightPiece == 0 || RightPiece >= TILEVAL_COASTLINE) PieceNum = PieceNum + 1;   //Add a binary 1

                        //See if there's water or coastline to the left of us.
                        if (LeftPiece == 0 || LeftPiece >= TILEVAL_COASTLINE) PieceNum = PieceNum + 2;   //Add a binary 2

                        //See if there's water or coastline below us.
                        if (DownPiece == 0 || DownPiece >= TILEVAL_COASTLINE) PieceNum = PieceNum + 4;   //Add a binary 4

                        //See if there's water or coastline above us.
                        if (UpPiece == 0 || UpPiece >= TILEVAL_COASTLINE) PieceNum = PieceNum + 8;  //Add a binary 8

                        //At this point, we know what shape this piece should be.

                        //Calculate the x coordinates in LandMap, since we'll use it a lot.
                        PieceXbase = GFX_NUM_LAND_PIECES * (int)(Seed[ii, jj] * GFX_NUM_LAND_PIECE_SETS);
                        PieceXcoord = PieceNum + PieceXbase;

                        //Draw the land piece in.
                        DrawBlock(gdiOperations, ii, jj, CorrectColor(ThisPiece), PieceXcoord);

                        //Draw the correct base border if this piece is different than ones around it.
                        //Only do up and right, since that's where the stripes are drawn.
                        //Check up.
                        if (CFGBorders == 5)
                        {
                            if ((UpPiece != ThisPiece) && (UpPiece > 0) && (UpPiece < TILEVAL_COASTLINE))
                            {
                                DrawLine(gdiOperations, ii, jj, DIR_UP);
                            }
                            //Check right.
                            if ((RightPiece != ThisPiece) && (RightPiece > 0) && (RightPiece < TILEVAL_COASTLINE))
                            {
                                DrawLine(gdiOperations, ii, jj, DIR_RIGHT);
                            }
                        }


                        //If this country is an HQ, we need to draw the HQ symbol on it...
                        if ((Special[ThisPiece] & 2) == 2)
                        {
                            //First, copy the HQ logo and its mask to the working area.
                            gdiOperations.BitBltFromSourceToSource(GFX_WORK_X_COORD, GFX_HQ_Y_COORD, GFX_GRID, GFX_GRID_X2, GFX_HQ_X_COORD, GFX_HQ_Y_COORD, SRCCOPY);
                            //Now, add the mask for the appropriate land piece in on top of the HQ mask.
                            gdiOperations.BitBltFromSourceToSource(GFX_WORK_X_COORD, GFX_WHITE_Y_COORD, GFX_GRID, GFX_GRID, (PieceXcoord * GFX_GRID), GFX_LAND_MASK_Y_COORD, SRCPAINT);
                            //Invert the land piece mask by XORing it into whiteness.
                            //There is a permanent white square in landmap at 17,14.
                            //First copy the white square.
                            gdiOperations.BitBltFromSourceToSource(GFX_WHITE_X_COORD, GFX_HQ_Y_COORD, GFX_GRID, GFX_GRID, GFX_WHITE_X_COORD, GFX_WHITE_Y_COORD, SRCCOPY);
                            //Then XOR the mask in.
                            gdiOperations.BitBltFromSourceToSource(GFX_WHITE_X_COORD, GFX_HQ_Y_COORD, GFX_GRID, GFX_GRID, (PieceXcoord * GFX_GRID), GFX_LAND_MASK_Y_COORD, SRCINVERT);
                            //Add the inverted mask to the HQ logo to blank out parts we don't want to draw.
                            gdiOperations.BitBltFromSourceToSource(GFX_WORK_X_COORD, GFX_HQ_Y_COORD, GFX_GRID, GFX_GRID, GFX_WHITE_X_COORD, GFX_HQ_Y_COORD, SRCAND);
                            //Now draw it!
                            DrawBlock(gdiOperations, ii, jj, GFX_SUPP_Y_LINE, GFX_NUM_LAND_PIECES - 1);
                        }

                        // Now we check for the edit pieces between countries to smooth out borders.
                        // Note that we *only* write on the current square.  We don't butt into
                        // someone else's country!

                        // All this does is smooth out a corner of our square if a country is diagonal
                        // to us.

                        // Remember that borders are drawn on the top edge and right edge of each tile.

                        // Check upper left.
                        if ((UpPiece == LeftPiece) && (UpPiece != ThisPiece) && (UpPiece > 0) && (UpPiece < TILEVAL_COASTLINE))
                        {
                            //Now fill in our corner...
                            DrawCorner(gdiOperations, ii, jj, CorrectColor(UpPiece), GFX_EDIT_PIECE_X_BASE + PieceXbase);
                        }
                        //Check upper right.
                        if ((RightPiece == UpPiece) && (RightPiece != ThisPiece) && (RightPiece > 0) && (RightPiece < TILEVAL_COASTLINE))
                        {
                            DrawCorner(gdiOperations, ii, jj, CorrectColor(RightPiece), GFX_EDIT_PIECE_X_BASE + 1 + PieceXbase);
                        }

                        //Check lower left.
                        if ((LeftPiece == DownPiece) && (LeftPiece != ThisPiece) && (LeftPiece > 0) && (LeftPiece < TILEVAL_COASTLINE))
                        {
                            DrawCorner(gdiOperations, ii, jj, CorrectColor(LeftPiece), GFX_EDIT_PIECE_X_BASE + 2 + PieceXbase);
                        }

                        //Check lower right.
                        if ((DownPiece == RightPiece) && (DownPiece != ThisPiece) && (DownPiece > 0) && (DownPiece < TILEVAL_COASTLINE))
                        {
                            DrawCorner(gdiOperations, ii, jj, CorrectColor(DownPiece), GFX_EDIT_PIECE_X_BASE + 3 + PieceXbase);
                        }

                        if (CFGBorders == 5)
                        {
                            // Finally, clean up borders to the top and right if we need to.
                            UpRightPiece = LookUpRight(ii, jj);
                            UpLeftPiece = LookUpLeft(ii, jj);
                            DownRightPiece = LookDownRight(ii, jj);
                            MySeedVal = (int)(Seed[ii, jj] * GFX_NUM_LAND_PIECE_SETS) + 3;

                            if ((ThisPiece != UpPiece) && (UpPiece < TILEVAL_COASTLINE) &&
                                                                           (ThisPiece == RightPiece) && (ThisPiece == UpRightPiece))
                            {
                                LineLength = 5 - (int)(Seed[ii, jj - 1] * GFX_NUM_LAND_PIECE_SETS);
                                if (MySeedVal < LineLength) LineLength = MySeedVal;
                                DrawFixLine(gdiOperations, ii, jj, CorrectColor(ThisPiece), 3, LineLength);
                            }
                            if ((ThisPiece != UpPiece) && (UpPiece < TILEVAL_COASTLINE) &&
                                                                           (ThisPiece == LeftPiece) && (ThisPiece == UpLeftPiece))
                            {
                                LineLength = 5 - (int)(Seed[ii, jj - 1] * GFX_NUM_LAND_PIECE_SETS);
                                if (MySeedVal < LineLength) LineLength = MySeedVal;
                                DrawFixLine(gdiOperations, ii, jj, CorrectColor(ThisPiece), 1, LineLength);
                            }
                            if ((ThisPiece != RightPiece) && (RightPiece < TILEVAL_COASTLINE) &&
                                                                           (ThisPiece == UpPiece) && (ThisPiece == UpRightPiece))
                            {
                                LineLength = 5 - (int)(Seed[ii + 1, jj] * GFX_NUM_LAND_PIECE_SETS);
                                if (MySeedVal < LineLength) LineLength = MySeedVal;
                                DrawFixLine(gdiOperations, ii, jj, CorrectColor(ThisPiece), 4, LineLength);
                            }
                            if ((ThisPiece != RightPiece) && (RightPiece < TILEVAL_COASTLINE) &&
                                                                           (ThisPiece == DownPiece) && (ThisPiece == DownRightPiece))
                            {
                                LineLength = 5 - (int)(Seed[ii + 1, jj] * GFX_NUM_LAND_PIECE_SETS);
                                if (MySeedVal < LineLength) LineLength = MySeedVal;
                                DrawFixLine(gdiOperations, ii, jj, CorrectColor(ThisPiece), 2, LineLength);
                            }
                        }

                        // Now we will add the cute little piers and docks if this country
                        // has a port.
                        if ((Special[ThisPiece] & 1) == 1)
                        {
                            // This country has a port.  Let's draw in the port piece if it borders
                            // water.  In other words, PieceNum is between 1 and 14.
                            if (PieceNum >= 1 && PieceNum <= 14)
                            {
                                // Yup, this one needs a pier.  Let's calculate the x and y adjustments...
                                Xadj = 0;
                                Yadj = 0;
                                if ((PieceNum == 1)) Xadj = 3;
                                if ((PieceNum == 2)) Xadj = -3;
                                if ((PieceNum == 4)) Yadj = 3;
                                if ((PieceNum == 8)) Yadj = -3;
                                if ((PieceNum == 5))
                                {
                                    Xadj = 2;
                                    Yadj = 2;
                                }
                                if ((PieceNum == 6))
                                {
                                    Xadj = -2;
                                    Yadj = 2;
                                }
                                if ((PieceNum == 9))
                                {
                                    Xadj = 2;
                                    Yadj = -2;
                                }
                                if ((PieceNum == 10))
                                {
                                    Xadj = -2;
                                    Yadj = -2;
                                }

                                DrawBlock(gdiOperations, ii, jj, GFX_SUPP_Y_LINE,
                                                                           ((GFX_NUM_PIER_PIECES * (int)(Seed[ii, jj] * GFX_NUM_PIER_PIECE_SETS)) + PieceNum + GFX_PIER_X_BASE), Xadj, Yadj);
                            }
                        }

                        // We will also add the selected border if two adjacent squares are
                        // different countries.  If no border selected, then move on.

                        if (CFGBorders < 5)
                        {
                            // Check up.
                            if ((UpPiece != ThisPiece) && (UpPiece > 0) && (UpPiece < TILEVAL_COASTLINE))
                            {
                                DrawBlock(gdiOperations, ii, jj, GFX_SUPP_Y_LINE, 0 + (CFGBorders - 1) * 4);
                            }
                            // Check right.
                            if ((RightPiece != ThisPiece) && (RightPiece > 0) && (RightPiece < TILEVAL_COASTLINE))
                            {
                                DrawBlock(gdiOperations, ii, jj, GFX_SUPP_Y_LINE, 2 + (CFGBorders - 1) * 4);
                            }
                            // Check left.
                            if ((LeftPiece != ThisPiece) && (LeftPiece > 0) && (LeftPiece < TILEVAL_COASTLINE))
                            {
                                DrawBlock(gdiOperations, ii, jj, GFX_SUPP_Y_LINE, 3 + (CFGBorders - 1) * 4);
                            }
                            // Check down.
                            if ((DownPiece != ThisPiece) && (DownPiece > 0) && (DownPiece < TILEVAL_COASTLINE))
                            {
                                DrawBlock(gdiOperations, ii, jj, GFX_SUPP_Y_LINE, 1 + (CFGBorders - 1) * 4);
                            }
                        }

                    }
                }
            }

            // Now we will superimpose each country's troop count on top of each country in
            // the predetermined display spot.
            if (GameMode == GM_GAME_ACTIVE || GameMode == GM_DIALOG_OPEN)
            {
                for (var i = 1; i <= NumCountries; i++)
                {
                    DrawNumber(gdiOperations, DisplaySpot[i, 1], DisplaySpot[i, 2], Troops[i]);
                }
            }
        }
    }

    private void DrawBlock(GDIOperations gdiOperations, int ii, int jj, int Color, int PieceNum, int Xadjust = 0, int Yadjust = 0)
    {
        int PieceX = 0;
        int ColorY = 0;
        int MaskColor = 0;

        //Set up the mask and coords.

        if (Color > 12)
        {
            //We're drawing borders or numbers or something else.
            MaskColor = (Color + 1) * GFX_GRID;
            PieceX = GFX_GRID * PieceNum;
            ColorY = GFX_GRID * Color;
        }
        else
        {
            //We're drawing land.
            MaskColor = GFX_MASK_Y_LINE * GFX_GRID;
            PieceX = GFX_GRID * PieceNum;
            ColorY = GFX_GRID * (Color - 1);
        }

        gdiOperations.BitBlt(((ii - 1) * GFX_GRID) + Xadjust, ((jj - 1) * GFX_GRID) + Yadjust, GFX_GRID, GFX_GRID, PieceX, MaskColor, SRCAND);
        gdiOperations.BitBlt(((ii - 1) * GFX_GRID) + Xadjust, ((jj - 1) * GFX_GRID) + Yadjust, GFX_GRID, GFX_GRID, PieceX, ColorY, SRCINVERT);
    }

    private void DrawCorner(GDIOperations gdiOperations, int x, int y, int Color, int PieceNum)
    {
        int PieceX = 0;
        int ColorY = 0;
        int MaskColor = 0;

        //Set up the mask and coords.
        PieceX = GFX_GRID * PieceNum;
        ColorY = GFX_GRID * (Color - 1);
        if (CFGBorders == 5)
        {
            //We're drawing borders or numbers or something else.
            MaskColor = (GFX_MASK_Y_LINE + 1) * GFX_GRID;
        }
        else
        {
            //We're drawing normal land.
            MaskColor = GFX_MASK_Y_LINE * GFX_GRID;
        }

        gdiOperations.BitBlt(((x - 1) * GFX_GRID), ((y - 1) * GFX_GRID), GFX_GRID, GFX_GRID, PieceX, MaskColor, SRCAND);
        gdiOperations.BitBlt(((x - 1) * GFX_GRID), ((y - 1) * GFX_GRID), GFX_GRID, GFX_GRID, PieceX, ColorY, SRCINVERT);
    }

    private void DrawLine(GDIOperations gdiOperations, int x, int y, int Direction)
    {
        int PieceX = 0;
        int ColorY = 0;
        int MaskColor = 0;

        //Set up the mask and coords.
        PieceX = GFX_GRID * (Direction + 1);
        ColorY = GFX_GRID * (GFX_MASK_Y_LINE + 1);
        MaskColor = ColorY;

        gdiOperations.BitBlt(((x - 1) * GFX_GRID), ((y - 1) * GFX_GRID), GFX_GRID, GFX_GRID, PieceX, MaskColor, SRCAND);
        gdiOperations.BitBlt(((x - 1) * GFX_GRID), ((y - 1) * GFX_GRID), GFX_GRID, GFX_GRID, 0, ColorY, SRCINVERT);
    }

    private void DrawFixLine(GDIOperations gdiOperations, int x, int y, int Color, int Shape, int Length)
    {
        // Set up the mask and coords.
        int ColorY = GFX_GRID * (Color - 1);

        switch (Shape)
        {
            case 1:
                gdiOperations.BitBlt(((x - 1) * GFX_GRID), ((y - 1) * GFX_GRID), Length, 1, 0, ColorY, SRCCOPY);
                break;
            case 2:
                gdiOperations.BitBlt(((x - 1) * GFX_GRID) + 7, ((y - 1) * GFX_GRID) + (8 - Length), 1, Length, 0, ColorY, SRCCOPY);
                break;
            case 3:
                gdiOperations.BitBlt(((x - 1) * GFX_GRID) + (8 - Length), ((y - 1) * GFX_GRID), Length, 1, 0, ColorY, SRCCOPY);
                break;
            case 4:
                gdiOperations.BitBlt(((x - 1) * GFX_GRID) + 7, ((y - 1) * GFX_GRID), 1, Length, 0, ColorY, SRCCOPY);
                break;
        }
    }

    private int RandomDir()
    {
        // This function generates a random integer, 1-4.
        return Random.Shared.Next(1, 5);
    }

    private void FillLakes()
    {
        // This function checks for lakes on the entire map.
        // If a lake is under the minimum size, then we fill it in
        // with the color of a random adjacent country.
        // If a lake is >= the minimum size, we will fill it in with
        // the code for the next body of water (1001 and up).
        // This will speed up the fill-in procedure so that each body
        // of water is only tested once.  Also, we will be able to
        // distinguish between lakes when we are done!

        LakeCode = 1001;

        if (MinLakeSize == 0)
        {
            //If we chose No Lake Correction, we need to plant seeds so that we
            //can still identify bodies of water.
            for (var j = 1; j <= XDimension; j++)
            {
                for (var k = 1; k <= YDimension; k++)
                {
                    if (_Map[j, k] == TILEVAL_COASTLINE)
                    {
                        _Map[j, k] = LakeCode;
                        LabelBodyOfWater(LakeCode);
                        LakeCode = LakeCode + 1;
                    }
                }
            }
            return;
        }

        for (var j = 1; j <= XDimension; j++)
        {
            for (var k = 1; k <= YDimension; k++)
            {
                //We always start our lake search on coastline.
                FilledIn = false;
                if (_Map[j, k] == TILEVAL_COASTLINE)
                {

                    // Clear out the last lake by clearing out the tentative array.
                    for (var i = 1; i <= MinLakeSize; i++)
                    {
                        Tent[1, i] = 0;
                        Tent[2, i] = 0;
                    }

                    Tent[1, 1] = j;
                    Tent[2, 1] = k;
                    TentNum = 1;
                    Country = 0;
                    Block = 1;

                    // Now we search for blocks contiguous to this one.
                    while (TentNum != MinLakeSize)
                    {
                        //Get the coordinates of a block in this country.
                        var x = Tent[1, Block];
                        var y = Tent[2, Block];
                        //Pick a direction to look for a contiguous block.
                        Direction = 1;
                        Done = false;

                        while (Direction != 5 && Done == false)
                        {
                            switch (Direction)
                            {
                                case UP:
                                    var OneAway = y - 1;
                                    if (OneAway > 0)
                                    {
                                        //Check and see what's up.
                                        if (ClearDirection(x, OneAway))
                                        {
                                            //Map is clear in that direction.
                                            y = y - 1;
                                            WriteToTent(x, y);
                                            Done = true;
                                        }
                                        else
                                        {
                                            //Blocked.  Try new direction.
                                            GetAdjacentColor(x, OneAway);
                                            Direction = Direction + 1;
                                        }
                                    }
                                    else
                                    {
                                        //We went off the top.  Try new direction.
                                        Direction = Direction + 1;
                                    }
                                    break;
                                case DOWN:
                                    OneAway = y + 1;
                                    if (OneAway <= YDimension)
                                    {
                                        //Check and see what's down.
                                        if (ClearDirection(x, OneAway))
                                        {
                                            //Map is clear in that direction.
                                            y = y + 1;
                                            WriteToTent(x, y);
                                            Done = true;
                                        }
                                        else
                                        {
                                            //Blocked.  Try new direction.
                                            GetAdjacentColor(x, OneAway);
                                            Direction = Direction + 1;
                                        }
                                    }
                                    else
                                    {
                                        //We went off the bottom.  Try new direction.
                                        Direction = Direction + 1;
                                    }
                                    break;
                                case LEFTY:
                                    OneAway = x - 1;
                                    if (OneAway > 0)
                                    {
                                        //Check and see what's up.
                                        if (ClearDirection(OneAway, y))
                                        {
                                            //Map is clear in that direction.
                                            x = x - 1;
                                            WriteToTent(x, y);
                                            Done = true;
                                        }
                                        else
                                        {
                                            //Blocked.  Try new direction.
                                            GetAdjacentColor(OneAway, y);
                                            Direction = Direction + 1;
                                        }
                                    }
                                    else
                                    {
                                        //We went off the left.  Try new direction.
                                        Direction = Direction + 1;
                                    }
                                    break;
                                case RIGHTY:
                                    OneAway = x + 1;
                                    if (OneAway <= XDimension)
                                    {
                                        //Check and see what's up.
                                        if (ClearDirection(OneAway, y))
                                        {
                                            //Map is clear in that direction.
                                            x = x + 1;
                                            WriteToTent(x, y);
                                            Done = true;
                                        }
                                        else
                                        {
                                            //Blocked.  Try new direction.
                                            GetAdjacentColor(OneAway, y);
                                            Direction = Direction + 1;
                                        }
                                    }
                                    else
                                    {
                                        //We went off the right.  Try new direction.
                                        Direction = Direction + 1;
                                    }
                                    break;
                            }
                        }

                        if (Direction == 5)
                        {
                            //This block is boxed in.  Try the next block in the lake.
                            Block = Block + 1;
                            if (Block > TentNum)
                            {
                                //Minimum lake can't fit here -- need to fill it in.
                                for (var i = 1; i <= TentNum; i++)
                                {
                                    _Map[Tent[1, i], Tent[2, i]] = Country;
                                }
                                TentNum = MinLakeSize;   //Fudging out of loop.
                                FilledIn = true;
                            }
                        }
                        if (Done == true)
                        {
                            Block = 1;
                        }
                    }

                    if (FilledIn == false)
                    {
                        //We've reached the minimum lake size.  Now we need to identify this
                        //entire body of water with the next lake code.  'Note that we overwrite
                        //our coastline here -- we don't need it anymore.
                        //Part one of this procedure is to fill in the lake part we've
                        //found already.
                        for (var i = 1; i <= TentNum; i++)
                        {
                            _Map[Tent[1, i], Tent[2, i]] = LakeCode;
                        }
                        //Part two of this procedure is to keep filling in the lake until
                        //there is no more to fill in.  We do this with quick multiple passes.
                        LabelBodyOfWater(LakeCode);
                        LakeCode = LakeCode + 1;
                    }
                }
            }
        }
    }

    private void LabelBodyOfWater(int LakeCode)
    {
        StillKicking();    //Put a period in the commentary line for each lake.

        Done = false;
        while (Done == false)
        {
            Done = true;  //If nothing gets written this pass, this will stay.
            for (var ii = 1; ii <= XDimension; ii++)
            {
                for (var jj = 1; jj <= YDimension; jj++)
                {
                    if (_Map[ii, jj] == LakeCode)
                    {
                        //Check for all adjacent squares and fill them in if they are
                        //water or coastline.  Note that we do diagonals here!
                        var xx = ii;
                        var yy = jj;
                        //Check up.
                        if (yy - 1 > 0)
                        {
                            if (_Map[xx, yy - 1] == 0 || _Map[xx, yy - 1] == TILEVAL_COASTLINE)
                            {
                                _Map[xx, yy - 1] = LakeCode;
                                Done = false;
                            }
                            //Check upper left.
                            if (xx - 1 > 0)
                            {
                                if (_Map[xx - 1, yy - 1] == 0 || _Map[xx - 1, yy - 1] == TILEVAL_COASTLINE)
                                {
                                    _Map[xx - 1, yy - 1] = LakeCode;
                                    Done = false;
                                }
                            }
                        }
                        //Check down.
                        if (yy + 1 <= YDimension)
                        {
                            if (_Map[xx, yy + 1] == 0 || _Map[xx, yy + 1] == TILEVAL_COASTLINE)
                            {
                                _Map[xx, yy + 1] = LakeCode;
                                Done = false;
                            }
                            //Check lower right.
                            if (xx + 1 <= XDimension)
                            {
                                if (_Map[xx + 1, yy + 1] == 0 || _Map[xx + 1, yy + 1] == TILEVAL_COASTLINE)
                                {
                                    _Map[xx + 1, yy + 1] = LakeCode;
                                    Done = false;
                                }
                            }
                        }
                        //Check left.
                        if (xx - 1 > 0)
                        {
                            if (_Map[xx - 1, yy] == 0 || _Map[xx - 1, yy] == TILEVAL_COASTLINE)
                            {
                                _Map[xx - 1, yy] = LakeCode;
                                Done = false;
                            }
                            //Check lower left.
                            if (yy + 1 <= YDimension)
                            {
                                if (_Map[xx - 1, yy + 1] == 0 || _Map[xx - 1, yy + 1] == TILEVAL_COASTLINE)
                                {
                                    _Map[xx - 1, yy + 1] = LakeCode;
                                    Done = false;
                                }
                            }
                        }
                        //Check right.
                        if (xx + 1 <= XDimension)
                        {
                            if (_Map[xx + 1, yy] == 0 || _Map[xx + 1, yy] == TILEVAL_COASTLINE)
                            {
                                _Map[xx + 1, yy] = LakeCode;
                                Done = false;
                            }
                            //Check upper right.
                            if (yy - 1 > 0)
                            {
                                if (_Map[xx + 1, yy - 1] == 0 || _Map[xx + 1, yy - 1] == TILEVAL_COASTLINE)
                                {
                                    _Map[xx + 1, yy - 1] = LakeCode;
                                    Done = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private bool FindNeighbors()
    {
        bool findNeighbors = true;

        // This routine will populate the Neighbors array.
        for (var i = 1; i <= NumCountries; i++)
        {
            for (var j = 1; j <= MAXIMUM_NEIGHBORS; j++)
            {
                Neighbor[i, j] = 0;
            }
        }

        for (var i = 1; i <= XDimension; i++)
        {
            StillKicking(); // Update text.
            for (var j = 1; j <= YDimension; j++)
            {
                var x = i;
                var y = j;

                //See what's up.
                if (y - 1 > 0) findNeighbors = AddNeighbor(_Map[x, y], _Map[x, y - 1]);

                //See what's down.
                if (y + 1 <= YDimension) findNeighbors = AddNeighbor(_Map[x, y], _Map[x, y + 1]);

                // See what's left.
                if (x - 1 > 0) findNeighbors = AddNeighbor(_Map[x, y], _Map[x - 1, y]);

                // See what's right.
                if (x + 1 <= XDimension) findNeighbors = AddNeighbor(_Map[x, y], _Map[x + 1, y]);

                if (findNeighbors == false)
                {
                    //We failed our spec -- too many neighbors. Let's flag it up the chain.
                    return findNeighbors;
                }
            }
        }

        // Now we sort the Neighbor array so that all water is after land.
        // This is important when we calculate attack and defense strengths.
        // This is just a simple bubble sort.  Not very efficient.    :)
        for (var i = 1; i <= NumCountries; i++)
        {
            Done = false;

            while (!Done)
            {
                Done = true;
                for (var j = MAXIMUM_NEIGHBORS - 1; j >= 1; j--)
                {
                    // See if we need to swap two consecutive neighbors, keeping zeros at the end!
                    if (Neighbor[i, j + 1] < Neighbor[i, j] && Neighbor[i, j + 1] != 0)
                    {
                        // We swap these two.
                        var k = Neighbor[i, j + 1];

                        Neighbor[i, j + 1] = Neighbor[i, j];
                        Neighbor[i, j] = k;

                        // Mark that we need to go another round.
                        Done = false;
                    }

                }
            }
        }

        findNeighbors = true; // Success bulding the Neighbors array.

        return findNeighbors;
    }


    private bool AddNeighbor(int ToCountry, int NewNeighbor)
    {
        // This sub works with FindNeighbors to populate the Neighbors array.

        var addNeighbor = true; // Success adding this neighbor.

        if (ToCountry >= TILEVAL_COASTLINE) return addNeighbor; // This is water!
        if (ToCountry == NewNeighbor) return addNeighbor; // Can't be our own neighbor!

        int LastOne = -1;

        for (var k = 1; k <= MAXIMUM_NEIGHBORS; k++)
        {
            if (Neighbor[ToCountry, k] == NewNeighbor) return addNeighbor; // Already there.

            if (LastOne == -1 && Neighbor[ToCountry, k] == 0) LastOne = k;
        }

        if (LastOne == -1)
        {
            // We have overstepped our limit on the number of allowable neighbors
            // per country.  Let's flag this up the chain so we can rebuild our map.
            addNeighbor = false;

            return addNeighbor;
        }

        Neighbor[ToCountry, LastOne] = NewNeighbor;

        return addNeighbor;
    }

    private void FindSpots()
    {
        // This sub will populate the DisplaySpot array.

        // First, we zero it all out.
        for (var i = 1; i <= NumCountries; i++)
        {
            DisplaySpot[i, 1] = 0;
            DisplaySpot[i, 2] = 0;
            DisplaySpot[i, 3] = 0;
        }

        // We make one pass across the map.  We find the grid location for each
        // country that has the most land around it belonging to the same country.
        // we check up to 2 squares max in each direction.  Therefore, this number
        // can range from 2 to 24.
        for (var i = 1; i <= Xsize; i++)
        {
            StillKicking(); //Put a period in the commentary line.

            for (var j = 1; j <= Ysize; j++)
            {
                int CurrentCountry = _Map[i, j];
                if (CurrentCountry > 0 && CurrentCountry < TILEVAL_COASTLINE)
                {
                    //We really do have a country and not water.
                    //Lets check 2 squares in each direction for like land.
                    int LandCount = 0;

                    for (var k = -2; k <= 2; k++)
                    {
                        for (var l = -2; l <= 2; l++)
                        {
                            int x = i + k;
                            int y = j + l;

                            if (x > 0 && x <= Xsize && y > 0 && y <= Ysize)
                            {
                                //We aren't off the edge of the map.

                                if (_Map[x, y] == CurrentCountry)
                                {

                                    // We found a matching piece.  Let's add the correct score
                                    // according to the following diagram:
                                    // 1 1 1 1 1
                                    // 1 3 3 3 1
                                    // 1 9 X 9 1
                                    // 1 3 3 3 1
                                    // 1 1 1 1 1
                                    // This way, a square with both left *and* right free has
                                    // a much better chance of making it.  With both of these free,
                                    // the number is *much* more readable and there won't be
                                    // as many conflicts with the borderlines.
                                    // Note that having one of the horizontally adjacent
                                    // squares is just as important as having all three to the
                                    // top or bottom!
                                    if (l == 0 && (k == -1 || k == 1))
                                    {
                                        //The big score.
                                        LandCount = LandCount + 9;
                                    }
                                    else if ((l == -1 || l == 1) && (k > -2 && k < 2))
                                    {
                                        //The middle score.
                                        LandCount = LandCount + 3;
                                    }
                                    else
                                    {
                                        //Just one point;
                                        LandCount = LandCount + 1;
                                    }
                                }
                            }

                        }
                    }

                    //Now we see if we beat our last high score for this country.
                    if (LandCount > DisplaySpot[CurrentCountry, 3])
                    {
                        // We do have more than our previous best.  Record this one.
                        DisplaySpot[CurrentCountry, 1] = i;
                        DisplaySpot[CurrentCountry, 2] = j;
                        DisplaySpot[CurrentCountry, 3] = LandCount;
                    }
                    else if (LandCount == DisplaySpot[CurrentCountry, 3])
                    {
                        // We tied our last high score.  Let's add some randomness
                        // so that we don't always have the best spot to the upper
                        // left of the country.
                        if (Random.Shared.NextDouble() < 0.05)
                        {
                            // 5% chance of taking the new one.
                            DisplaySpot[CurrentCountry, 1] = i;
                            DisplaySpot[CurrentCountry, 2] = j;
                            DisplaySpot[CurrentCountry, 3] = LandCount;

                        }

                    }
                }
            }

        }
    }

    private void BuildSeedMatrix()
    {
        bool[] marker = new bool[NumCountries + 1];

        // Initializing seed matrix.
        for (var i = 1; i <= NumCountries; i++)
        {
            // Using the troops array to flag which countries we've hit.
            marker[i] = false;
        }

        for (var i = 1; i <= XDimension; i++)
        {
            for (var j = 1; j <= YDimension; j++)
            {
                if (_Map[i, j] > 0 && _Map[i, j] < TILEVAL_COASTLINE)
                {
                    // if this square is on coastline, and it's the FIRST piece of coastline
                    // then we need to force a wooden pier on it.  If not, then go normal.
                    if (marker[_Map[i, j]] == false && IsOnCoastLine(i, j) == true)
                    {
                        Seed[i, j] = 0;
                        marker[_Map[i, j]] = true;
                    }
                    else
                    {
                        Seed[i, j] = (float)Random.Shared.NextDouble();
                    }
                }
            }
        }

    }

    private void DrawNumber(GDIOperations gdiOperations, int Xcoord, int Ycoord, int ThreeDigitNumber)
    {
        // Get the color of this text.
        int TempNum = MyMap!.Grid(Xcoord, Ycoord); // Get the country ID.

        if (TempNum > TILEVAL_COASTLINE) return; // Shouldn't get this!

        int Digit = MyMap.Owner(TempNum); // Get the owner.

        bool TextColor = true; // Default to white text.

        // Now see if we should have black instead.
        if (Digit > 0)
        {
            if (PlayerTextColor[Player[Digit]] == vbBlack)
                TextColor = false;
        }
        else
        {
            if (PlayerTextColor[CFGUnoccupiedColor] == vbBlack)
                TextColor = false;
        }

        // Always black if flashing!
        if (FlashArray[TempNum, 1] == 1) TextColor = false;

        TempNum = ThreeDigitNumber;

        // Get the three digits.
        int HDigit = TempNum / 100;
        TempNum = TempNum - (HDigit * 100);
        int TDigit = TempNum / 10;
        TempNum = TempNum - (TDigit * 10);
        Digit = TempNum;

        int Yadj = (Ycoord - 1) * 8;

        int Xadj;

        // Hundreds digit.  Offset 5 pixels left if it exists.
        if (HDigit > 0)
        {
            Xadj = ((Xcoord - 1) * GFX_GRID) - 5;

            PlaceDigit(gdiOperations, HDigit, Xadj, Yadj, TextColor);
        }

        // Tens digit.
        if (HDigit > 0 || TDigit > 0)
        {
            if (HDigit == 0)
            {
                // If less than 100, we offset by 3 pixels left;
                Xadj = ((Xcoord - 1) * GFX_GRID) - 3;
            }
            else
            {
                // No offset.
                Xadj = ((Xcoord - 1) * GFX_GRID);
            }

            PlaceDigit(gdiOperations, TDigit, Xadj, Yadj, TextColor);
        }

        // Ones digit.
        if (HDigit > 0 || TDigit > 0 || Digit > 0)
        {
            if (HDigit == 0 && TDigit == 0)
            {
                // If less than 10, we center it.
                Xadj = ((Xcoord - 1) * GFX_GRID);
            }
            else if (HDigit == 0 && TDigit > 0)
            {
                // If less than 100, we offset by 2 pixels right.
                Xadj = ((Xcoord - 1) * GFX_GRID) + 2;
            }
            else
            {
                // Offset 5 pixels right.
                Xadj = ((Xcoord - 1) * GFX_GRID) + 5;
            }

            PlaceDigit(gdiOperations, Digit, Xadj, Yadj, TextColor);
        }
    }

    private void PlaceDigit(GDIOperations gdiOperations, int Digit, int Xadj, int Yadj, bool BlackAndWhite)
    {

        // Draw a single number on the screen.
        gdiOperations.BitBlt(Xadj, Yadj, GFX_GRID_HALF, GFX_GRID, (GFX_DIGIT_X_BASE + Digit) * GFX_GRID, GFX_WHITE_Y_COORD, SRCAND);

        if (BlackAndWhite == false)
        {
            // Black.
            gdiOperations.BitBlt(Xadj, Yadj, GFX_GRID_HALF, GFX_GRID, GFX_BLACK_X_COORD, GFX_HQ_Y_COORD, SRCINVERT);
        }
        else
        {
            // White.
            gdiOperations.BitBlt(Xadj, Yadj, GFX_GRID_HALF, GFX_GRID, (GFX_DIGIT_X_BASE + Digit) * GFX_GRID, GFX_HQ_Y_COORD, SRCINVERT);
        }
    }

    internal void SetupTitleScreen()
    {

        // Everything is water...
        for (int i = 1; i <= XDimension; i++)
        {
            for (int j = 1; j <= YDimension; j++)
            {
                _Map[i, j] = 1337;
                Seed[i, j] = Random.Shared.NextSingle();
            }
        }

        // Get which title we're supposed to be showing.
        SetupTitleScreenLand();

        // Except where there's an "X" in our template.
        for (var j = 1; j <= CurrentTitleHeight; j++)
        {

            for (var i = 1; i <= CurrentTitleWidth; i++)
            {
                var titleLand = TitleLand[j];
                var mapItem = 1337;
                switch (titleLand[i - 1])
                {
                    case 'A':
                        // This square of the map has land on it!  Let's paint it in.
                        mapItem = 2;
                        break;
                    case 'B':
                        // This square of the map has land on it!  Let's paint it in.
                        mapItem = 3;
                        break;
                    case 'C':
                        // This square of the map has land on it!  Let's paint it in.
                        mapItem = 4;
                        break;
                    case 'X':
                        // This square of the map has land on it!  Let's paint it in.
                        mapItem = 1;
                        break;
                    default:
                        // Make it water by default.
                        mapItem = 1337;
                        break;
                }

                // Find the map coordinate where this should go.  Center it and bounds check. 
                var Xcoo = (XDimension / 2) - (CurrentTitleWidth / 2) + i;
                var Ycoo = (YDimension / 2) - (CurrentTitleHeight / 2) + j;
                if (Xcoo < 1) Xcoo = 1;
                if (Xcoo > XDimension) Xcoo = XDimension;
                if (Ycoo < 1) Ycoo = 1;
                if (Ycoo > YDimension) Ycoo = YDimension;

                // Finally, add this square to our map grid.
                _Map[Xcoo, Ycoo] = mapItem;
            }
        }

        Special = new int[5];
        CountryClr = new int[5];
        FlashArray = new int[5, 5];

        CountryClr[1] = CFGUnoccupiedColor;
        CountryClr[2] = 1;
        CountryClr[3] = 3;
        CountryClr[4] = 5;

        Special[1] = ((Random.Shared.NextDouble() < 0.2) ? 1 : 0) + ((Random.Shared.NextDouble() < 0.5) ? 2 : 0);
        Special[2] = ((Random.Shared.NextDouble() < 0.5) ? 1 : 0) + ((Random.Shared.NextDouble() < 0.2) ? 2 : 0);
        Special[3] = ((Random.Shared.NextDouble() < 0.5) ? 1 : 0) + ((Random.Shared.NextDouble() < 0.2) ? 2 : 0);
        Special[4] = ((Random.Shared.NextDouble() < 0.5) ? 1 : 0) + ((Random.Shared.NextDouble() < 0.2) ? 2 : 0);
    }

    internal void RedimensionStuff(int xxx, int yyy)
    {
        // Redimension our arrays to save some memory.
        NumCountries = xxx;
        LakeCode = yyy;

        Neighbor = new int[NumCountries + ONE_ARRAY_FIX, MAXIMUM_NEIGHBORS + ONE_ARRAY_FIX];
        CountryClr = new int[NumCountries + ONE_ARRAY_FIX];
        Owners = new int[NumCountries + ONE_ARRAY_FIX];
        DisplaySpot = new int[NumCountries + ONE_ARRAY_FIX, 3 + ONE_ARRAY_FIX];
        Troops = new int[NumCountries + ONE_ARRAY_FIX];
        Special = new int[NumCountries + ONE_ARRAY_FIX];
        CName = new string[NumCountries + ONE_ARRAY_FIX];
        WName = new string[LakeCode - 1000 + ONE_ARRAY_FIX];
        FlashArray = new int[NumCountries + ONE_ARRAY_FIX, 4 + ONE_ARRAY_FIX];
    }

    internal void FinishUpLoad()
    {
        // FindNeighbors *has* to be true, or we never would have built the map
        // in the first place.
        bool x = FindNeighbors();

        FindSpots();
        BuildSeedMatrix();

        // We should have loaded the stamp from file.  For a net game, this doesn't matter.
        MyMap!.MapStamp = LastMapStamp;
    }

    public bool UpdateFlashing(int Cntry)
    {
        // Update the flash array for the passed country.  If we triggered
        // a flash, flag the caller to redraw the screen.

        bool updateFlashing = false;
        if (FlashArray[Cntry, 2] > 0)
        {
            // This country is flashing now.  Decrement the counter.
            FlashArray[Cntry, 2] = FlashArray[Cntry, 2] - 1;
            if (FlashArray[Cntry, 2] == 0)
            {
                // Reverse our status...
                FlashArray[Cntry, 1] = 1 - FlashArray[Cntry, 1];
                // Set the flag...
                updateFlashing = true;
                // One flash down...
                FlashArray[Cntry, 4] = FlashArray[Cntry, 4] - 1;
                if (FlashArray[Cntry, 4] > 0)
                {
                    // Still more to go.  Reset for next.
                    FlashArray[Cntry, 2] = FlashArray[Cntry, 3];
                }
            }
        }

        return updateFlashing;
    }

    private int CorrectColor(int Cntry)
    {
        // This sub either returns the color of the specified country,
        // or white if it's flashing!
        if (FlashArray[Cntry, 1] == 0)
        {
            // We'll use our normal color -- we're not flashing.
            return CountryClr[Cntry];
        }
        else
        {
            // This country is flashing!  It's white by default.
            return 12;
        }
    }

    private void StillKicking()
    {
        //This sub puts a period on the end of the commentary to show that the
        //map maker is still working..........

        if (Land.Refs.Commentary.Text.Length > 80)
        {
            Land.Refs.Commentary.Text = "Creating map...";
            Land.Refs.Oopsie.Text = "";
        }
        else
        {
            Land.Refs.Commentary.Text = Land.Refs.Commentary.Text + ".";
        }
    }

    internal void SetDimensions(int x, int y)
    {
        XDimension = x;
        YDimension = y;
    }
}
