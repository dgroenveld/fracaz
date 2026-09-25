namespace Fracaz;

public class NameGen
{
    public static string[] NameSpice = new string[26];
    private static int[] LetterPrp = new int[27];
    private static float[] LetterPct = new float[27];

    public static string GenerateName(int MinSize, int MaxSize, bool Specials)
    {
        string WorkingName;
        string LastChar;
        string TwoCharsAgo;
        bool GoodWord;
        int i;

        do
        {

            // The first character has an equal chance of being any letter.
            WorkingName = Convert.ToChar(Random.Shared.Next(26) + 65).ToString();
            LastChar = WorkingName;
            TwoCharsAgo = "";
            GoodWord = true; // For now.

            for (i = 1; i <= (Random.Shared.Next(MaxSize - MinSize + 1) + MinSize) - 1; i++)
            {

                // Let's see if we had a consonant or a vowel last.
                if (Consonant(LastChar))
                {
                    // We had a consonant last time.
                    if (Consonant(TwoCharsAgo))
                    {
                        // We have two consonants in a row.  This one must be a vowel,
                        // Unless the last one was a Y.
                        if (LastChar == "Y")
                        {
                            WorkingName += AddChar("BCDFGKLMNPRSTVXZ");
                        }
                        else
                        {
                            WorkingName += AddChar("AEIOU");
                        }
                    }
                    else
                    {
                        // We only have one consonant so far, so there's a chance that
                        // the next character could be one too.
                        if (Random.Shared.NextDouble() < 0.2)
                        {
                            // It's a vowel this time.
                            // Let's make sure that a U follows a Q...
                            if (LastChar == "Q")
                            {
                                WorkingName += AddChar("U");
                            }
                            else
                            {
                                WorkingName += AddChar("AEIOU");
                            }
                        }
                        else
                        {
                            // Let's add a sensible consonant after this one.
                            switch (LastChar)
                            {
                                case "B":
                                    WorkingName += AddChar("BLRSY");
                                    break;
                                case "C":
                                    WorkingName += AddChar("CHKLRSTY");
                                    break;
                                case "D":
                                    WorkingName += AddChar("DRSWY");
                                    break;
                                case "F":
                                    WorkingName += AddChar("FLRSTY");
                                    break;
                                case "G":
                                    WorkingName += AddChar("GHLRS");
                                    break;
                                case "H":
                                    WorkingName += AddChar("AEIOUY");
                                    break;
                                case "J":
                                    WorkingName += AddChar("AEIOU");
                                    break;
                                case "K":
                                    WorkingName += AddChar("HKLNRSTW");
                                    break;
                                case "L":
                                    WorkingName += AddChar("CDKLMNPSTY");
                                    break;
                                case "M":
                                    WorkingName += AddChar("MPSY");
                                    break;
                                case "N":
                                    WorkingName += AddChar("CDKNSTY");
                                    break;
                                case "P":
                                    WorkingName += AddChar("HLPRSTY");
                                    break;
                                case "Q":
                                    WorkingName += AddChar("U");
                                    break;
                                case "R":
                                    WorkingName += AddChar("CDKMNPRSTY");
                                    break;
                                case "S":
                                    WorkingName += AddChar("CHKLMNPSTWY");
                                    break;
                                case "T":
                                    WorkingName += AddChar("HRSTWY");
                                    break;
                                case "V":
                                    WorkingName += AddChar("LRS");
                                    break;
                                case "W":
                                    WorkingName += AddChar("HKST");
                                    break;
                                case "X":
                                    WorkingName += AddChar("CSYAEIOU");
                                    break;
                                case "Y":
                                    WorkingName += AddChar("LSTKMNPC");
                                    break;
                                case "Z":
                                    WorkingName += AddChar("HZZZ");
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    // We had a vowel last time.
                    if (Consonant(TwoCharsAgo))
                    {
                        // We only have one vowel so far, so there's a chance that
                        // the next character could be a vowel too.
                        if ((Random.Shared.NextDouble() < 0.9) && WorkingName.Substring(WorkingName.Length - 2) != "QU")
                        {
                            // It's a consonant this time.
                            WorkingName += AddChar("BCDFGHJKLMNPQRSTVWX");
                        }
                        else
                        {
                            // 'Let's add a sensible vowel after this one.
                            switch (LastChar)
                            {
                                case "A":
                                    WorkingName += AddChar("IU");
                                    break;
                                case "E":
                                    WorkingName += AddChar("AEIOU");
                                    break;
                                case "I":
                                    WorkingName += AddChar("AEO");
                                    break;
                                case "O":
                                    WorkingName += AddChar("AEIOU");
                                    break;
                                case "U":
                                    WorkingName += AddChar("AEI");
                                    break;
                            }
                        }
                    }
                    else
                    {
                        // We have two vowels in a row.  This one must be a consonant.
                        // Note that some consonants don't work well after two vowels.
                        WorkingName += AddChar("BCDFGKLMNPRST");
                    }
                }

                TwoCharsAgo = LastChar;
                LastChar = WorkingName.Substring(WorkingName.Length - 1);
            }

            // We need to double-check some illegal letter combinations at the start.
            TwoCharsAgo = WorkingName.Substring(0, 1);  // First char.
            LastChar = WorkingName.Substring(1, 1);     // Second char.

            // If there's a double-consonant at the beginning, that's bad.
            if (TwoCharsAgo == LastChar) GoodWord = false;

            // If there's a consonant followed by S or D, that's bad.
            if (LastChar == "S" || LastChar == "D")
            {
                if (Consonant(TwoCharsAgo)) GoodWord = false;
            }

            // If there's a consonant followed by T, K, M, N, P, or C, that's bad.
            // Unless an S starts the word, of course.
            if (LastChar == "T" || LastChar == "K" || LastChar == "M" || LastChar == "N" || LastChar == "P" || LastChar == "C")
            {
                if (Consonant(TwoCharsAgo) && TwoCharsAgo != "S") GoodWord = false;
            }
            // If this word starts with a Y, it had better not have a consonant after it.
            if (TwoCharsAgo == "Y" && Consonant(LastChar)) GoodWord = false;
            // No QU at the end.  Yuck.
            if (WorkingName.Substring(WorkingName.Length - 2) == "QU") GoodWord = false;
            // IY, YI and IW sure look stupid.
            if (WorkingName.Contains("IY") || WorkingName.Contains("IW") || WorkingName.Contains("YI")) GoodWord = false;
            // So do UY and UW.
            if (WorkingName.Contains("UY") || WorkingName.Contains("UW")) GoodWord = false;
            // And while we're at it, so do IH and UH.
            if (WorkingName.Contains("IH") || WorkingName.Contains("UH")) GoodWord = false;
            // Words that start with X or IL generally suck, I find.
            if (TwoCharsAgo == "X" || WorkingName.Substring(0, 2) == "IL") GoodWord = false;

        } while (GoodWord == false);

        LastChar = WorkingName.Substring(WorkingName.Length - 1);
        TwoCharsAgo = WorkingName.Substring(WorkingName.Length - 2, 1); // 2nd to last char.

        // Now we will spice up the name with some embellishments.
        if ((Consonant(LastChar) && Consonant(TwoCharsAgo)) || WorkingName.Length < 4 || Random.Shared.NextDouble() < 0.15)
        {
            // Let's add something cool to the end of the name!
            if (Consonant(LastChar) && LastChar.ToUpper() != "Y")
            {
                WorkingName += NameSpice[Random.Shared.Next(6) + 17];
            }
            else
            {
                WorkingName += NameSpice[Random.Shared.Next(6) + 11];
            }
        }

        // Let's put the proper case on this word.
        WorkingName = WorkingName.Substring(0, 1).ToUpper() + WorkingName.Substring(1).ToLower();

        if (Specials)
        {
            if (Random.Shared.NextDouble() < 0.13)
            {
                // Let's give this country a formal title!
                WorkingName = NameSpice[Random.Shared.Next(10) + 1] + " " + WorkingName;
            }
        }

        // That's it!  We've got one cool name for you.
        return WorkingName;
    }

    private static bool Consonant(string CharIn)
    {
        if (CharIn == "A" || CharIn == "E" || CharIn == "I" || CharIn == "O" || CharIn == "U" || CharIn.Length != 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private static string AddChar(string Letters)
    {
        int j;
        float LetterTotal;
        float LetterThresh;
        float RandLetter;

        // Let's figure out our proportions.
        LetterTotal = 0;
        for (j = 1; j <= Letters.Length; j++)
        {
            LetterTotal += LetterPrp[Convert.ToInt32(Letters[j - 1]) - 64];
        }

        // Let's set up our threshholds for each character in the Letters string.
        LetterThresh = 0;
        for (j = 1; j <= Letters.Length; j++)
        {
            LetterThresh += LetterPrp[Convert.ToInt32(Letters[j - 1]) - 64] / LetterTotal;
            LetterPct[j] = LetterThresh;
        }

        // Now grab one of them.
        RandLetter = Random.Shared.NextSingle();
        for (j = 1; j <= Letters.Length; j++)
        {
            if (RandLetter <= LetterPct[j])
            {
                // It's this one!
                return Letters[j - 1].ToString();
            }
        }

        return "";
    }

    public static void InitNameStuff()
    {

        // We will use the Scrabble(TM) letter proportions!
        LetterPrp[1] = 9;
        LetterPrp[2] = 2;
        LetterPrp[3] = 2;
        LetterPrp[4] = 4;
        LetterPrp[5] = 12;
        LetterPrp[6] = 2;
        LetterPrp[7] = 3;
        LetterPrp[8] = 2;
        LetterPrp[9] = 9;
        LetterPrp[10] = 1;
        LetterPrp[11] = 1;
        LetterPrp[12] = 4;
        LetterPrp[13] = 2;
        LetterPrp[14] = 6;
        LetterPrp[15] = 8;
        LetterPrp[16] = 2;
        LetterPrp[17] = 1;
        LetterPrp[18] = 6;
        LetterPrp[19] = 4;
        LetterPrp[20] = 6;
        LetterPrp[21] = 4;
        LetterPrp[22] = 2;
        LetterPrp[23] = 2;
        LetterPrp[24] = 1;
        LetterPrp[25] = 2;
        LetterPrp[26] = 1;

        NameSpice[1] = "Upper";
        NameSpice[2] = "Lower";
        NameSpice[3] = "Old";
        NameSpice[4] = "New";
        NameSpice[5] = "San";
        NameSpice[6] = "Costa";
        NameSpice[7] = "North";
        NameSpice[8] = "South";
        NameSpice[9] = "East";
        NameSpice[10] = "West";
        NameSpice[11] = "tia";
        NameSpice[12] = "lia";
        NameSpice[13] = "way";
        NameSpice[14] = "land";
        NameSpice[15] = "ton";
        NameSpice[16] = "burg";
        NameSpice[17] = "ary";
        NameSpice[18] = "age";
        NameSpice[19] = "ia";
        NameSpice[20] = "any";
        NameSpice[21] = "ica";
        NameSpice[22] = "ania";
        NameSpice[23] = "Isle";
        NameSpice[24] = "Island";
        NameSpice[25] = "The Isle of";
    }
}