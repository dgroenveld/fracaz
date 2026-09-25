using static Fracaz.Declarations;

namespace Fracaz;

internal static class TitleScreen
{
    public const int BIGTITLEHEIGHT = 38 + ONE_ARRAY_FIX;
    public const int BIGTITLEWIDTH = 80 + ONE_ARRAY_FIX;
    public const int SMALLTITLEHEIGHT = 36 + ONE_ARRAY_FIX;
    public const int SMALLTITLEWIDTH = 64 + ONE_ARRAY_FIX;
    public static int CurrentTitleHeight;
    public static int CurrentTitleWidth;
    public static string[] TitleLand = new string[BIGTITLEHEIGHT];

    internal static void SetupBigTitleScreenLand()
    {
        TitleLand[1] = "                                                          BBB                   ";
        TitleLand[2] = "                                         AAAAAA         BBBBB                   ";
        TitleLand[3] = "                                          AAAAAAAAA    BBBBBB                   ";
        TitleLand[4] = "                                          AAAAAAAAA  BBBBBBBBBB                 ";
        TitleLand[5] = "                                           AAAAAAAAAABBBBBBBBBB                 ";
        TitleLand[6] = "                                          AAAAAAAAAAABBBBBBBBBB                 ";
        TitleLand[7] = "  XXXX     XXXXXXXXXXX   X               AAAAAAAAAAABBBBBBBBBBBB                ";
        TitleLand[8] = " XXXXXX   XXXXXXXXXXXXXX X               AAAAAAAAAAABBBBBBBBBBBB                ";
        TitleLand[9] = "XXX   XXXXXXX         XXXX                AAAAAAAAACCCBBBBBBBBBB                ";
        TitleLand[10] = "XXX    XXXX            XXX                 AAAAAACCCCCCBBBBBBBB                 ";
        TitleLand[11] = " XXX   XXX              XX                   AAACCCCCCCCCBBB                    ";
        TitleLand[12] = "  XXX  XXX               X                    CCCCCCCCCCCC                      ";
        TitleLand[13] = "       XXX               X                    CCCCCCCCCCCC                      ";
        TitleLand[14] = "       XXX                                     CCCCCCCCCC                       ";
        TitleLand[15] = "       XXX     X                                CCCCCCCC                        ";
        TitleLand[16] = "       XXX     X                                CCCCCCCCC                       ";
        TitleLand[17] = "       XXXX   XX                                   CCCCCC                       ";
        TitleLand[18] = "       XXXXXXXXX                                     CCC                X       ";
        TitleLand[19] = "       XXXX   XX                                                        XX      ";
        TitleLand[20] = "       XXX     X       XXXX            XXX                       XXX    XXX     ";
        TitleLand[21] = "       XXX     X XXX  XXXXXX  XXXXX   XX     XXXX X     XXXXX   XX      XXXX    ";
        TitleLand[22] = "       XXX      XXXX XX    XXXX   XX XX    XXX  XXX   XXX   XX XX      XXXXXX   ";
        TitleLand[23] = "       XXX     XX XXXX     XXX     XXX    XXX    XX  XXX     XXX       XXX XXX  ";
        TitleLand[24] = "       XXX   XXX  XXX     XXX      XXX   XXX      X XXX      XXX      XXX   XXX ";
        TitleLand[25] = "       XXX XXX    XXX     XXX      XXX   XXX        XXX      XXX      XXX   XXX ";
        TitleLand[26] = "       XXX        XXX     XXX      XXX   XXX        XXX      XXX     XXX     XXX";
        TitleLand[27] = "       XXX        XXX     XXX      XXX   XXX        XXX      XXX     XXX     XXX";
        TitleLand[28] = "       XXX        XXX     XXX      XXX   XXX        XXX      XXX    XXX      XXX";
        TitleLand[29] = " XXX   XXX        XXX     XXX      XXX   XXX        XXX      XXX    XXX      XXX";
        TitleLand[30] = "XXX   XXXX  XXX   XXX      XXX     XXX    XXX      XXXXX     XXX   XXX XX   XXX ";
        TitleLand[31] = "XXX  XXXX  XXX   XXX        XXX   XX XX  XXXXX   XXX  XXX   XX XX  XXX  XX  XXX ";
        TitleLand[32] = " XXXXXX     XXXXXXX           XXXXX   XXXX   XXXXX      XXXXX   XXXXX    XXXX   ";
        TitleLand[33] = "                                                                                ";
        TitleLand[34] = "                                                                                ";
        TitleLand[35] = "                                                                                ";
        TitleLand[36] = "                                                                                ";
        TitleLand[37] = "                                                                                ";
        TitleLand[38] = "                                                                                ";
        //                1234567890123456789012345678901234567890123456789012345678901234567890123456789e1*34567890123456789
    }

    internal static void SetupSmallTitleScreenLand()
    {
        TitleLand[1] = "                                                        BBB     ";
        TitleLand[2] = "                                       AAAAAA         BBBBB     ";
        TitleLand[3] = "                                        AAAAAAAAA    BBBBBB     ";
        TitleLand[4] = "                                        AAAAAAAAA  BBBBBBBBBB   ";
        TitleLand[5] = "                                         AAAAAAAAAABBBBBBBBBB   ";
        TitleLand[6] = "                                        AAAAAAAAAAABBBBBBBBBB   ";
        TitleLand[7] = "  XXX    XXXXXXXXXXX   X               AAAAAAAAAAABBBBBBBBBBBB  ";
        TitleLand[8] = " XX XX  XXXX      XXX  X               AAAAAAAAAAABBBBBBBBBBBB  ";
        TitleLand[9] = "XX   XXXXX          XXXX                AAAAAAAAACCCBBBBBBBBBB  ";
        TitleLand[10] = "XX    XXX            XXX                 AAAAAACCCCCCBBBBBBBB   ";
        TitleLand[11] = " XX   XX              XX                   AAACCCCCCCCCBBB      ";
        TitleLand[12] = "  XX  XX               X                    CCCCCCCCCCCC        ";
        TitleLand[13] = "      XX               X                    CCCCCCCCCCCC        ";
        TitleLand[14] = "      XX                                     CCCCCCCCCC         ";
        TitleLand[15] = "      XX     X                                CCCCCCCC          ";
        TitleLand[16] = "      XX     X                                CCCCCCCCC         ";
        TitleLand[17] = "      XXX   XX                                   CCCCCC         ";
        TitleLand[18] = "      XXXXXXXX                                     CCC          ";
        TitleLand[19] = "      XXX   XX                                            X     ";
        TitleLand[20] = "      XX     X       XXX         XX                   XX  XX    ";
        TitleLand[21] = "      XX     X XX  XXX XX XXXX  XX   XXXX X    XXXX  XX   XXX   ";
        TitleLand[22] = "      XX      XXX XX    XXX  XXXX   XX  XXX   XX  XXXX    XXXX  ";
        TitleLand[23] = "      XX     XX XXX     XX    XX   XX    XX  XX    XX     XX XX ";
        TitleLand[24] = "      XX   XXX  XX     XX     XX  XX      X XX     XX    XX  XX ";
        TitleLand[25] = "      XX XXX    XX     XX     XX  XX        XX     XX    XX   XX";
        TitleLand[26] = "      XX        XX     XX     XX  XX        XX     XX   XX    XX";
        TitleLand[27] = " XX   XX        XX     XX     XX  XX        XX     XX   XX    XX";
        TitleLand[28] = "XX   XX    XX   XX      XX    XX   XX      XXXX    XX  XX XX  XX";
        TitleLand[29] = "XX  XX    XX   XX        XX  XXXX XXXX   XXX  XX  XXXX XX  XXXX ";
        TitleLand[30] = " XXXX      XXXXX          XXXX  XXX  XXXXX     XXXX  XXX    XX  ";
        TitleLand[31] = "                                                                ";
        TitleLand[32] = "                                                                ";
        TitleLand[33] = "                                                                ";
        TitleLand[34] = "                                                                ";
        TitleLand[35] = "                                                                ";
        TitleLand[36] = "                                                                ";
        //                1234567890123456789012345678901234567890123456789012345678901234567890123456789e1*34567890123456789
    }

    internal static void SetupTitleScreenLand()
    {
        if (CFGResolution == 1)
        {
            CurrentTitleHeight = SMALLTITLEHEIGHT - 1;
            CurrentTitleWidth = SMALLTITLEWIDTH - 1;
            SetupSmallTitleScreenLand();
        }
        else
        {
            CurrentTitleHeight = BIGTITLEHEIGHT - 1;
            CurrentTitleWidth = BIGTITLEWIDTH - 1;
            SetupBigTitleScreenLand();
        }
    }
}
