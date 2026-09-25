using static Fracaz.Declarations;

namespace Fracaz;

internal class GraphicFX
{
    public static void DrawMap()
    {
        // These commands draw the map. MapBuffer is where the map is drawn.
        MyMap!.DisplayMap(Land.LandMap, Land.MapBuffer);
    }

    private static bool FlashTimerRunning;

    public static void FlashTimerSub(object sender, EventArgs e)
    {

        if (FlashTimerRunning) return;

        FlashTimerRunning = true;

        try
        {
            FlashTimerSub_Tick(sender, e);
        }
        finally
        {
            FlashTimerRunning = false;
        }
    }

    private static void FlashTimerSub_Tick(object sender, EventArgs e)
    {
        // This sub cycles through the flash array and updates everything.
        if (GameMode != GM_GAME_ACTIVE | CFGFlashing == 0) return;

        bool FlagUpdate = false;

        for (var i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            if (MyMap.UpdateFlashing(i) == true)
            {
                FlagUpdate = true;
            }
        }

        if (FlagUpdate) DrawMap();
    }

    public static void ResetFlashing()
    {
        // This sub clears out the flash array.

        for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            for (int j = 1; j <= 4; j++)
            {
                MyMap.Flash(i, j, 0);
            }
        }

    }

    public static void ShortFlash(int Cntry)
    {
        // This sub starts off a short single flash for the passed country.
        if (CFGFlashing == 0) return;

        MyMap!.Flash(Cntry, 1, 1);
        MyMap.Flash(Cntry, 2, 6);
        MyMap.Flash(Cntry, 3, 6);
        MyMap.Flash(Cntry, 4, 1);
    }

    public static void LongFlash(int Cntry)
    {
        // This sub starts off a long single flash for the passed country.
        if (CFGFlashing == 0) return;

        MyMap!.Flash(Cntry, 1, 1);
        MyMap.Flash(Cntry, 2, 12);
        MyMap.Flash(Cntry, 3, 12);
        MyMap.Flash(Cntry, 4, 1);
    }

    public static void LongerFlash(int Cntry)
    {
        // This sub starts off a long single flash for the passed country.
        if (CFGFlashing == 0) return;

        MyMap!.Flash(Cntry, 1, 1);
        MyMap.Flash(Cntry, 2, 30);
        MyMap.Flash(Cntry, 3, 30);
        MyMap.Flash(Cntry, 4, 1);
    }


}
