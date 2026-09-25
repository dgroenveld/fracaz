using static Fracaz.Boom;
using static Fracaz.Declarations;
using static Fracaz.GraphicFX;
using static Fracaz.NetworkPlay;
using static Fracaz.Play;
using static Fracaz.SoundFX;

namespace Fracaz;

public static class ComputerAI
{
    public static float[] Score { get; set; } = new float[MAX_PLAYERS + ONE_ARRAY_FIX];
    public static float BestScore { get; set; }
    public static int BestCountry { get; set; }
    public static float NumReinforce { get; set; }
    public static float AttackDiff { get; set; }
    public static int AttackedCountryThisTurn { get; set; }
    public static int ToWhere { get; set; }
    public static float XferAmount { get; set; }
    public static int MsgXferAmount { get; set; } // Public so we can see it for messages.

    public static int Cntry { get; set; }
    public static int Cntry2 { get; set; }

    // These variables are used to calculate the number of troops to move.
    public static float AttProp { get; set; }
    public static float AttSum1 { get; set; }
    public static float AttSum2 { get; set; }
    public static float A1 { get; set; }
    public static float A2 { get; set; }
    public static float A3 { get; set; }
    public static float A4 { get; set; }
    public static float A5 { get; set; }
    public static float A6 { get; set; }
    public static float B1 { get; set; }
    public static float B2 { get; set; }
    public static float B3 { get; set; }
    public static float B4 { get; set; }
    public static float B5 { get; set; }
    public static float B6 { get; set; }
    public static float D1 { get; set; }
    public static float D2 { get; set; }

    // Temporary registers used for a variety of purposes.
    public static float TempNum1 { get; set; }
    public static float TempNum2 { get; set; }
    public static float HQTemp { get; set; }

    // The Hate array is used to keep track of, well, how much each computer
    // hates each other player.
    // For example:  a = Hate(x, y)
    // a = the hate value, from 1 to 100.
    // x = the player doing the hating.
    // y = the player being hated.
    public static int[,] Hate = new int[6 + ONE_ARRAY_FIX, 6 + ONE_ARRAY_FIX];

    // The following variables define the number of available computer
    // personalities to pick from.
    public const int NUMPERSONALITIES = 12;
    public static string[] PersonalityName = new string[NUMPERSONALITIES + ONE_ARRAY_FIX];

    public const int NOTPOSSIBLE = -999999; // Just a very low number.
    public const int UNLIKELY = 1;    // Higher than NOTPOSSIBLE, but any other move is better.

    // These variables define the personality of the current AI.
    public static float SAMESCOREKEEPER { get; private set; } // Chance of keeping a country with the same bestscore.
    public static int HATEFACTOR { get; private set; } // How badly this personality holds a grudge.
    public static int HQTROOPSMULTIPLIER { get; private set; } // Number of times that the troops in a country count.
    public static int HQBODYOFWATER { get; private set; } // Number of points a body of water is worth to HQ.
    public static int HQBORDERINGCOUNTRY { get; private set; } // Number of points a bordering country is worth to HQ.
    public static float HQONEAWAYMULT { get; private set; } // Fraction of troops counted for countries 1 away from HQ.
    public static float HQTWOAWAYMULT { get; private set; } // Fraction of troops counted for countries 2 away from HQ.
    public static float HQNEXTTOENEMYHQ { get; private set; } // Score for putting HQ near enemy HQs.
    public static float HQNEARCENTERMULT { get; private set; } // Factor for HQ being closer to the center of the map.
    public static int HQDEFENDPROPENSITY { get; private set; } // Points per strength difference to defend HQ.
    public static int REDEFLOSINGPROPOSITION { get; private set; } // Factor for reinforcing defensive troops in a losing battle.
    public static int REDEFWINNINGPROPOSITION { get; private set; } // Factor for reinforcing defensive troops in a winning battle.
    public static int REATTLOSINGPROPOSITION { get; private set; } // Factor for reinforcing offensive troops in a losing battle.
    public static int REATTWINNINGPROPOSITION { get; private set; } // Factor for reinforcing offensive troops in a winning battle.
    public static int REPUTPRESSUREONHQBASE { get; private set; } // Score for reinforcing near an enemy HQ.
    public static int REPUTPRESSUREONHQMULT { get; private set; } // Pro-rated Score for reinforcing near an enemy HQ.
    public static int REBETONASURETHING { get; private set; } // Score for reinforcing a battle that can already be won.
    public static float ACANNEXTROOPS { get; private set; } // Factor for annexing a country with troops in it.
    public static float ACANNEXBASE { get; private set; } // Score for just annexing any piece of land. (* CFGBonusTroops)
    public static float ACANNEXNEARENEMYMULT { get; private set; } // Multiplier for taking free troops near enemy.
    public static float ACANNEXNEXTTOHQ { get; private set; } // Score for annexing unclaimed land next to an enemy HQ.
    public static float ACANNEXTWOAWAYMULT { get; private set; } // Fraction that free troops two countries away are worth.
    public static int ACPORTBASE { get; private set; } // Score for just building a port. (* CFGShips)
    public static int ACPORTTROOPS { get; private set; } // Factor for building a port on a country with troops.
    public static int ACATTACKBASE { get; private set; } // Score for simply attacking another defending player.
    public static int ACATTACKFACTOR { get; private set; } // Pro-rated score for attacking another defending player.
    public static int ACBULLYBASE { get; private set; } // Score for beating up a player in an easy battle.
    public static int ACBULLYFACTOR { get; private set; } // Pro-rated score for attacking in an easy battle.
    public static int ACCLEANUPEMPTYS { get; private set; } // Score for attacking an empty enemy country.
    public static int ACATTACKENEMYHQ { get; private set; } // Score for being able to attack enemy HQ.
    public static int ACHATEATTACKED { get; private set; } // How much more I hate you for attacking me.
    public static int ACHATEFORGIVEN { get; private set; } // How much more I hate you now that I've attacked you.
    public static int ACHATEENEMYOFMYENEMY { get; private set; } // How much more I hate you now that you've attacked someone else.
    public static float TMPUTPRESSUREONHQBASE { get; private set; } // Score for moving troops near an enemy HQ.
    public static float TMPUTPRESSUREONHQMULT { get; private set; } // Pro-rated score for moving troops near an enemy HQ.
    public static float TMSWOOPINSCORE { get; private set; } // Score for moving troops into a just-taken country.
    public static float TMDEFENDPROPENSITY { get; private set; } // Pro-rated score for defending a country with a troop movement.
    public static float TMATTACKPROPENSITY { get; private set; } // Pro-rated score for attacking a country with a troop movement.
    public static float TMBETONASURETHING { get; private set; } // Score for moving troops to attack a country that can already be taken.
    public static float TMMOVEFROMHQ { get; private set; } // Score for HQ being the only country to get troops from.
    public static float TMMAXPCTFROMHQ { get; private set; } // The maximum percentage of troops that can be moved from HQ.
    public static float TMTWOAWAYDEFENSEFACTOR { get; private set; } // The percent of a normal score for defending a country two moves away.

    public static void AIChooseHQ(bool AutoPick, int Turn, int Phase)
    {

        if (GameMode != GM_GAME_ACTIVE) return;

        if (PlayerType[Turn] == PTYPE_COMPUTER)
        {
            GetPersonalityData(Turn);
        }
        else
        {
            GetPersonalityData(7); // Default to Stonewall if we're autopicking HQ.
        }

        // This sub picks the computer's first country.
        AIsetupscore();

        // First, we check to see which country has the highest troop total in and
        // around it.  If initial troops are at none, then we skip this step.
        // The troops in the country itself count more than once.
        // Unclaimed bordering troops count once.

        Application.DoEvents();
        for (var i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            if (GameMode != GM_GAME_ACTIVE) return;

            // If this country is already owned, skip it for now.
            if (MyMap.Owner(i) == 0)
            {
                // First add the troop score for the country itself.
                Score[i] = Score[i] + (MyMap.TroopCount(i) * HQTROOPSMULTIPLIER);

                // Now let's look at this country's neighbors.
                for (var j = 1; j <= MyMap.MaxNeighbors; j++)
                {
                    Cntry = MyMap.Neighbors(i, j);

                    // If we hit a zero, then we're done.
                    if (Cntry == 0) break;

                    if (Cntry < TILEVAL_COASTLINE) // It's land.
                    {
                        // If this country is not owned, it is worth some points.
                        if (MyMap.Owner(Cntry) == 0)
                        {
                            Score[i] = Score[i] + (MyMap.TroopCount(Cntry) * HQONEAWAYMULT); // Score for troops in it.
                            Score[i] = Score[i] + HQBORDERINGCOUNTRY; // Score for bordering a country.

                            // Now we check all of this neighbor's neighbors for more stuff.
                            for (var k = 1; k <= MyMap.MaxNeighbors; k++)
                            {
                                Cntry2 = MyMap.Neighbors(Cntry, k);

                                if (Cntry2 == 0) break;

                                if (Cntry2 < TILEVAL_COASTLINE) // It's land.
                                {

                                    // Now see if this country already touches the first one!
                                    if (AIxCanReachy(Cntry2, i) == 0)
                                    {

                                        // Add the two-away score for troops.
                                        if (MyMap.Owner(Cntry2) == 0)
                                        {
                                            // Unowned country two steps away.

                                            Score[i] = Score[i] + (MyMap.TroopCount(Cntry2) * HQTWOAWAYMULT);
                                        }
                                        else
                                        {
                                            // It's an enemy HQ.
                                            Score[i] = Score[i] + ((HQNEXTTOENEMYHQ * (4 ^ CFGInitTroopCt)) / 2);
                                        }

                                    }
                                }
                                else // It's water.
                                {
                                    // Right now, do nothing.
                                }
                            }
                        }
                        else
                        {
                            // This neighbor is owned by someone else.  Scoring...
                            Score[i] = Score[i] + (HQNEXTTOENEMYHQ * (4 ^ CFGInitTroopCt));

                        }
                    }
                    else // It's water.
                    {
                        Score[i] = Score[i] + HQBODYOFWATER;
                    }
                }

                // Now, let's add a factor for how close to the edge of the map it is.
                // We're using the DisplaySpot to calculate this. Not perfect, but
                // very elegant!  The closer to the edge, the better.
                // First, check the horizontal direction.
                if (MyMap.DigitCoords(i, 1) > (MyMap.Xsize / 2))
                {
                    TempNum1 = MyMap.Xsize - MyMap.DigitCoords(i, 1);
                }
                else
                {
                    TempNum1 = MyMap.DigitCoords(i, 1);
                }

                // Now the vertical direction.
                if (MyMap.DigitCoords(i, 2) > (MyMap.Ysize / 2))
                {
                    TempNum2 = MyMap.Ysize - MyMap.DigitCoords(i, 2);
                }
                else
                {
                    TempNum2 = MyMap.DigitCoords(i, 2);
                }

                // Now add in scores for each.
                Score[i] = Score[i] + ((TempNum1 + TempNum2) * HQNEARCENTERMULT);


            }
            else
            {
                // This country is already owned. We *can't* choose it as our country.
                Score[i] = NOTPOSSIBLE;
            }
        }

        AIFindbestscore(Turn);

        if (AutoPick == false)
        {
            if (MyNetworkRole == NW_SERVER) SendClickToNetwork(Turn, Phase, BestCountry, 0);

            ChooseHQProc(BestCountry, Turn);
        }
        else
        {
            // We're picking this HQ quickly for automatic HQ selection.  No delays.
            ClaimHQ(BestCountry, Turn);
        }

    }

    public static void AIReinforce(int Turn, int Phase)
    {
        GetPersonalityData(Turn);

        if (GameMode != GM_GAME_ACTIVE) return;

        HQTemp = AIassessHQ(Turn);

        NumReinforce = CFGBonusTroops * NumOccupied[Turn];

        // This sub finds the best place to dump reinforcement troops.
        // The spot is based on which country needs the most defense right now,
        // and which country could use the force to attack.

        AIsetupscore();

        // First, find which country would benefit most from defensive troops.
        Application.DoEvents();
        for (var i = 1; i <= MyMap!.NumberOfCountries; i++)
        {

            if (GameMode != GM_GAME_ACTIVE) return;

            // -------------------------------------------------------------------------------------------

            if (MyMap.Owner(i) == Turn)
            {
                // We do own this country, so it is a contender...
                if ((MyMap.CountryType(i) & 2) == 2)
                {
                    // We found our HQ!  Let's move our HQ assessment over.
                    Score[i] = Score[i] + HQTemp;
                }

                for (var j = 1; j <= 6; j++)
                {
                    // Let's find out what would happen to this country if each other player
                    // were to attack it.
                    if (j != Turn)
                    {
                        CalculateStrengths(j, i);
                        if (AttackStrength > DefendStrength)
                        {
                            // We could lose this country!
                            if ((AttackStrength - DefendStrength) > NumReinforce)
                            {
                                // Even reinforcing it with these troops won't guarantee safety.
                                Score[i] = Score[i] + (((NumReinforce + DefendStrength) /
                                          (NumReinforce + DefendStrength + AttackStrength)) *
                                          REDEFLOSINGPROPOSITION);
                            }
                            else
                            {
                                // We could prevent an invasion with these troops.
                                Score[i] = Score[i] + (((NumReinforce + DefendStrength) /
                                          (NumReinforce + DefendStrength + AttackStrength)) *
                                          REDEFWINNINGPROPOSITION);

                            }
                        }
                        else
                        {
                            // This country won't be lost just yet, so we don't need troops here.
                        }
                    }
                }
            }
            else
            {
                // We don't own this country, so we can't exactly put any troops here
                Score[i] = NOTPOSSIBLE;
            }

            // Now, find which country would benefit most from offensive troops.
            if (MyMap.Owner(i) == Turn)
            {
                for (var j = 1; j <= MyMap.NumberOfCountries; j++)
                {
                    if ((MyMap.Owner(j) != Turn) &&
                         ((AIxCanReachy(i, j) == 1) || ((AIxCanReachy(i, j) == 2) &&
                         ((MyMap.CountryType(i) & 1) == 1))))
                    {
                        CalculateStrengths(Turn, j);
                        AttackDiff = AttackStrength - DefendStrength;

                        // Only look at countries where we have a military influence.
                        if (AttackStrength != 0)
                        {
                            if (AttackStrength > DefendStrength)
                            {
                                // We already can take this country, so we probably don't need troops here.
                                Score[i] = Score[i] + REBETONASURETHING;
                            }
                            else
                            {
                                // We can't take this country yet..
                                if ((AttackStrength + NumReinforce) > DefendStrength)
                                {
                                    // We could take this country if we put our troops here!
                                    Score[i] = Score[i] + (((NumReinforce + AttackStrength) /
                                          (NumReinforce + AttackStrength + DefendStrength)) *
                                          REATTWINNINGPROPOSITION);
                                }
                                else
                                {
                                    // We really won't be able to take this country.
                                    Score[i] = Score[i] + (((NumReinforce + AttackStrength) /
                                          (NumReinforce + AttackStrength + DefendStrength)) *
                                          REATTLOSINGPROPOSITION);
                                    // But, if it is an enemy HQ, we should put pressure on it!
                                    if (((MyMap.CountryType(j) & 2) == 2) && (MyMap.Owner(j) != Turn))
                                    {
                                        if ((AIxCanReachy(i, j)) == 1)
                                        {
                                            // We can get to the HQ by land this way.  Full points!
                                            Score[i] = Score[i] + REPUTPRESSUREONHQBASE + (((NumReinforce + AttackStrength) /
                                                  (NumReinforce + AttackStrength + DefendStrength)) *
                                                  REPUTPRESSUREONHQMULT);
                                        }
                                        else if ((AIxCanReachy(i, j) == 2))
                                        {
                                            // We can get to the HQ by water this way.  Fractional points.
                                            Score[i] = Score[i] + (ShipPct * (REPUTPRESSUREONHQBASE + (((NumReinforce + AttackStrength) /
                                                  (NumReinforce + AttackStrength + DefendStrength)) *
                                                  REPUTPRESSUREONHQMULT)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (GameMode != GM_GAME_ACTIVE) return;

            // Check out port countries.  If there are no enemies on the same shores,
            // then don't bother!
            if (MyMap.Owner(i) == Turn && ((MyMap.CountryType(i) & 1) == 1))
            {
                // This is one of our port countries.
                TempNum2 = 0;
                for (var j = 1; j <= MyMap.NumberOfCountries; j++)
                {
                    if ((AIxCanReachy(i, j) == 2) && ((MyMap.Owner(j) != Turn) && (MyMap.Owner(j) > 0)))
                    {
                        // Here's a country that we could put pressure on by reinforcing this port.
                        TempNum2 = TempNum2 + 1;
                    }
                }

                if (TempNum2 == 0)
                {
                    // Reinforcing this port will do no good.
                    Score[i] = UNLIKELY;
                }
            }

            // Now, we need to knock out the countries that are maxed out.  Otherwise, the
            // computer will keep dumping troops into the same country over and over!
            if (MyMap.Owner(i) == Turn && MyMap.TroopCount(i) >= MAX_COUNTRY_CAPACITY)
            {
                // We're going to lose our reinforcements.  Let's make this impossible.
                Score[i] = NOTPOSSIBLE;
            }
        }

        AIFindbestscore(Turn);

        WaitForNoBalls();

        if (BestScore < 0)
        {
            // We're passing.
            SendPassToNetwork(Turn, Phase, 0);
            SNDWhistle();
            return;
        }

        if (MyNetworkRole == NW_SERVER) SendClickToNetwork(Turn, Phase, BestCountry, 0);

        ReinforceProc(BestCountry, Turn);
    }
    public static void AIaction(int Turn, int Phase)
    {
        GetPersonalityData(Turn);

        if (GameMode != GM_GAME_ACTIVE) return;

        // This sub calculates the best course of action this turn by calculating
        // scores based on annexing, adding ports, and attacking.

        AttackedCountryThisTurn = 0;
        AIsetupscore();

        Application.DoEvents();
        for (var i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            if (GameMode != GM_GAME_ACTIVE) return;

            TempNum1 = AIreachable(i, Turn);
            if (TempNum1 == 0)
            {
                //If we can't even touch that country, then screw it!
                Score[i] = NOTPOSSIBLE;
            }
            else if (TempNum1 == -1 && (MyMap.CountryType(i) & 1) != 1 && Coastal(i) == true)
            {
                //Let's consider a port here on our land since it doesn't have one.
                //Need to include the Ships setting in our equation.  The more damage a
                //port can do, the better it is to have one!
                Score[i] = Score[i] + (ACPORTBASE * ShipPct) + ((MyMap.TroopCount(i) * ACPORTTROOPS) / (5 - CFGShips));
                //But, a port here may not be necessary.  Let's see if there are
                //any enemy countries at all overseas.
                TempNum2 = 0;
                for (var j = 1; j <= MyMap.NumberOfCountries; j++)
                {
                    if (AIxCanReachy(i, j) == 2 && (((MyMap.Owner(j) != Turn) && (MyMap.Owner(j) > 0)) ||
                             ((MyMap.Owner(j) == 0) && (MyMap.TroopCount(j) > 0))))
                    {
                        //Here's a country that we could put pressure on with a port.
                        TempNum2 = TempNum2 + 1;
                    }
                }
                if (TempNum2 == 0)
                {
                    //A port here will do no good.
                    Score[i] = UNLIKELY;
                }
            }
            else if (MyMap.Owner(i) == 0)
            {
                //Ah, this country can be annexed...but we don't want to take just anything.
                //The number of troops we'll get for it next turn is important, as is the
                //number of troops in it that we'll be inheriting.
                Score[i] = Score[i] + (ACANNEXBASE * (CFGBonusTroops + 0.1f)) + (MyMap.TroopCount(i) * ACANNEXTROOPS);
                //The total defense that we can give the new country will be the current
                //attack total for it, plus the troops in it already.
                CalculateStrengths(Turn, i);
                TempNum2 = AttackStrength + MyMap.TroopCount(i);
                //Now we will check the attack strength for each other player on this country.
                for (var j = 1; j <= 6; j++)
                {
                    if (j != Turn)
                    {
                        CalculateStrengths(j, i);
                        if (AttackStrength + (NumOccupied[j] * CFGBonusTroops) > TempNum2)
                        {
                            //If we take this country, we will need to move troops here this turn
                            //or we will lose it.  For now, we'll steer clear.  We add the troopcount
                            //here because if there's nothing better to do, at least we'll grab
                            //the one with the most troops!
                            Score[i] = UNLIKELY + (MyMap.TroopCount(i) * ACANNEXNEARENEMYMULT);
                            break;
                        }
                    }
                }
                // Now, let's look at all the neighbors of this country and see if there
                // are more goodies two countries beyond us.
                for (var j = 1; j <= MyMap.MaxNeighbors; j++)
                {
                    Cntry = MyMap.Neighbors(i, j);
                    if (Cntry == 0) break;
                    if (Cntry < TILEVAL_COASTLINE) //It's land.
                    {
                        //Now see if this country already borders the first.
                        if (AIxCanReachy(Cntry, i) == 0)
                        {
                            //It can't get to the original, so let's add the score.
                            if (MyMap.Owner(Cntry) == 0 && MyMap.TroopCount(Cntry) > 0)
                            {
                                Score[i] = Score[i] + (MyMap.TroopCount(Cntry) * ACANNEXTWOAWAYMULT);
                            }
                            else if ((MyMap.CountryType(Cntry) & 2) == 2 &&
                                                          MyMap.Owner(Cntry) != Turn)
                            {
                                //We're two away from an enemy HQ.  Let's tussle.
                                Score[i] = Score[i] + ACANNEXNEXTTOHQ;
                            }
                        }
                    }
                    else
                    {
                        //It's water.
                        //Right now, do nothing.
                    }
                }
            }
            else if (MyMap.Owner(i) != Turn && TempNum1 > 0)
            {
                //Maybe we can attack this country!
                CalculateStrengths(Turn, i);
                if (AttackStrength > DefendStrength)
                {
                    //Yes, we can attack it and do damage.
                    //There are several factors at work here.  First, the bully factor.
                    //The more we can damage it, the more we are interested in attacking it!
                    Score[i] = Score[i] + ACBULLYBASE + ((AttackStrength / (AttackStrength + DefendStrength)) * ACBULLYFACTOR);
                    //Also, if a country has *no* troops in it, we add in the cleanup factor.
                    if (MyMap.TroopCount(i) == 0)
                    {
                        Score[i] = UNLIKELY + ACCLEANUPEMPTYS;
                    }
                    //Second, the cautious factor.  Kill the larger enemy first.
                    //The more the opponent can defend, the more we are interested in it.
                    Score[i] = Score[i] + ACATTACKBASE + ((DefendStrength / (AttackStrength + DefendStrength)) * ACATTACKFACTOR);
                    //Check if we're lookin' at an HQ.
                    if ((MyMap.CountryType(i) & 2) == 2)
                    {
                        //Whoa -- this is another player's HQ!  We should put extra effort
                        //Into killing it.
                        Score[i] = Score[i] + ACATTACKENEMYHQ;
                    }
                    //Third, the hate factor.  If we hate this person, we are more inclined
                    //to attack them over other players.
                    Score[i] = Score[i] + (Hate[Turn, MyMap.Owner(i)] * HATEFACTOR);
                }
                else
                {
                    //We can't attack this country at all.
                    Score[i] = NOTPOSSIBLE;
                }

            }
        }

        AIFindbestscore(Turn);

        WaitForNoBalls();

        if (BestScore <= 0)
        {
            //We're passing.
            SendPassToNetwork(Turn, Phase, 0);
            SNDWhistle();
            return;
        }

        if (MyNetworkRole == NW_SERVER) SendClickToNetwork(Turn, Phase, BestCountry, 0);
        ActionProc(BestCountry, Turn);
    }
    public static void AITroopMove(int Turn, int Phase)
    {
        GetPersonalityData(Turn);

        if (GameMode != GM_GAME_ACTIVE) return;

        // This sub calculates if a troop movement will be beneficial, and how many
        // troops to move.  There are three things that might necessitate a troop
        // movement:
        // 1 - to defend a friendly country, especially HQ.
        // 2 - to attack an enemy country, especially HQ.
        // 3 - to 'swoop in' after an enemy country has been beaten, and generally
        //    make the computer look like it knows what it's doing.   :)

        AIsetupscore();
        XferAmount = 0;
        MsgXferAmount = 0;

        // Let's see what our HQ assessment is.
        HQTemp = AIassessHQ(Turn);

        // Do this now in case we pass our turn -- the SNDWhistle would be too fast.
        WaitForNoBalls();

        // Situation 1:  Defend a country of our own.
        Application.DoEvents();
        for (var i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            if (GameMode != GM_GAME_ACTIVE) return;
            if (MyMap.Owner(i) == Turn && MyMap.TroopCount(i) < MAX_COUNTRY_CAPACITY)
            {
                // This is a valid candidate for a troop recipient.
                if ((MyMap.CountryType(i) & 2) == 2)
                {
                    // We found our HQ!  Let's move our HQ assessment over and add it in.
                    Score[i] = Score[i] + HQTemp;
                }
                for (var j = 1; j <= 6; j++)
                {
                    // Let's find out what would happen to this country if each other player
                    // were to attack it.
                    if (j != Turn)
                    {
                        CalculateStrengths(j, i);
                        if (AttackStrength > DefendStrength) // Also prevents /0.
                        {
                            // We could lose this country!
                            Score[i] = Score[i] + ((DefendStrength / (DefendStrength + AttackStrength)) * TMDEFENDPROPENSITY);
                        }
                        else
                        {
                            // We're holding this country pretty well right now, so there's no rush to
                            // move troops here.
                            Score[i] = UNLIKELY;
                        }
                    }
                }
                // Now let's look at each of this country's neighbors and see if one of
                // them needs some defending.
                for (var j = 1; j <= MyMap.NumberOfCountries; j++)
                {
                    if ((AIxCanReachy(i, j) == 1) || (AIxCanReachy(i, j) == 2 && (MyMap.CountryType(i) & 1) == 1))
                    {
                        // We found a country that would benefit from us moving troops here.
                        if ((MyMap.CountryType(j) & 2) == 2)
                        {
                            // Our HQ is next to this one.  Moving here will defend it.
                            Score[i] = Score[i] + (HQTemp * TMTWOAWAYDEFENSEFACTOR);
                        }
                        for (var k = 1; k <= 6; k++)
                        {
                            // Let's find out what would happen to this two-away country if each
                            // other player were to attack it.
                            if (k != Turn)
                            {
                                CalculateStrengths(k, j);
                                if (AttackStrength > DefendStrength) // Also prevents /0.
                                {
                                    // That country is in danger!
                                    Score[i] = Score[i] + ((DefendStrength / (DefendStrength + AttackStrength)) * (TMDEFENDPROPENSITY * TMTWOAWAYDEFENSEFACTOR));
                                }
                                else
                                {
                                    // We're holding this country pretty well right now, so there's no rush to
                                    // move troops here.
                                    Score[i] = Score[i] + UNLIKELY;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Situation 2:  Attack an enemy country.
        for (var i = 1; i <= MyMap.NumberOfCountries; i++)
        {
            if (GameMode != GM_GAME_ACTIVE) return;
            if (MyMap.Owner(i) == Turn && MyMap.TroopCount(i) < MAX_COUNTRY_CAPACITY)
            {
                // This is a valid candidate for a troop recipient.
                for (var j = 1; j <= MyMap.NumberOfCountries; j++)
                {
                    if (MyMap.Owner(j) != Turn &&
                        (AIxCanReachy(i, j) == 1 || (AIxCanReachy(i, j) == 2 && (MyMap.CountryType(i) & 1) == 1)))
                    {
                        CalculateStrengths(Turn, j);
                        // Only look at countries where we have a military influence.
                        // If AttackStrength != 0 Then
                        if (AttackStrength > DefendStrength)
                        {
                            // We already can take this country, so we probably don't need troops here.
                            Score[i] = Score[i] + TMBETONASURETHING;
                        }
                        else
                        {
                            // We can't take this country yet..
                            if ((AttackStrength + DefendStrength) > 0) // Also prevents /0.
                            {
                                // We maybe could take this country if we moved troops here!
                                Score[i] = Score[i] + ((AttackStrength / (AttackStrength + DefendStrength)) * TMATTACKPROPENSITY);
                            }
                        }
                        // If this is an enemy HQ...
                        if ((MyMap.CountryType(j) & 2) == 2)
                        {
                            if (AIxCanReachy(i, j) == 1 && (AttackStrength + DefendStrength) > 0)
                            {
                                // We can get to the HQ by land this way.  Full points!
                                Score[i] = Score[i] + TMPUTPRESSUREONHQBASE + ((AttackStrength / (AttackStrength + DefendStrength)) * TMPUTPRESSUREONHQMULT);
                            }
                            else if (AIxCanReachy(i, j) == 2)
                            {
                                // We can get to the HQ by water this way.  Fractional points.
                                Score[i] = Score[i] + TMPUTPRESSUREONHQBASE + ((AttackStrength / (AttackStrength + DefendStrength)) * TMPUTPRESSUREONHQMULT * ShipPct);
                            }
                        }
                    }
                }
            }
        }

        // Situation 3:  Swoop in after taking a country this turn.
        // First, we check to see if we attacked a country this turn, and that we
        // did in fact take it over.
        if (AttackedCountryThisTurn > 0)
        {
            // Yup, we attacked one.
            if (MyMap.Owner(AttackedCountryThisTurn) == Turn)
            {
                // Yup, we own it now.  Let's augment the previously calculated scores.
                Score[AttackedCountryThisTurn] = Score[AttackedCountryThisTurn] + TMSWOOPINSCORE;
            }
        }

        // Now we knock out the impossibles.
        // Let's check if our HQ is the only country we can take troops from.
        // Also see if there is *any* adjacent country that can donate.
        for (var i = 1; i <= MyMap.NumberOfCountries; i++)
        {
            //  DoEvents
            if (GameMode != GM_GAME_ACTIVE) return;
            if (MyMap.Owner(i) == Turn && MyMap.TroopCount(i) < MAX_COUNTRY_CAPACITY)
            {
                // Let's find all countries of ours that can get here.
                TempNum1 = 0;   // Number of adjacent countries that can donate.
                TempNum2 = 0;   // 1 if our HQ can donate.
                for (var j = 1; j <= MyMap.NumberOfCountries; j++)
                {
                    if (MyMap.Owner(j) == Turn && MyMap.TroopCount(j) > 0 &&
                        (AIxCanReachy(j, i) == 1 || (AIxCanReachy(j, i) == 2 && (MyMap.CountryType(j) & 1) == 1)))
                    {
                        // Yup, it can get here.
                        TempNum1 = TempNum1 + 1;
                        // Check if it's our HQ...
                        if ((MyMap.CountryType(j) & 2) == 2)
                        {
                            TempNum2 = TempNum2 + 1;
                        }
                    }
                }
                if (TempNum1 == 0)
                {
                    // No one to give.  Not good.
                    Score[i] = NOTPOSSIBLE;
                }
                if (TempNum1 == 1 && TempNum2 == 1)
                {
                    // The only one who can give to this country is HQ.  Need to pro-rate this.
                    Score[i] = Score[i] + TMMOVEFROMHQ;
                }
            }
            else
            {
                // We don't own this country, or we're toked on troops.
                // So we can't exactly put any troops here.
                Score[i] = NOTPOSSIBLE;
            }
        }

        AIFindbestscore(Turn);

        if (BestScore <= 0)
        {
            // Nobody really needs troops.
            SendPassToNetwork(Turn, Phase, 0);
            SNDWhistle();
            return;
        }

        // Now we know who needs troops the most, but how many, and from where?
        // We're assuming that when we get here, we have at least one friendly
        // neighbor with troops in it.

        // Reset the score array!  Our 'TO' country is BestCountry.
        ToWhere = BestCountry;
        AIsetupscore();

        // Consider the defense of the country we're moving from.
        for (var i = 1; i <= MyMap.NumberOfCountries; i++)
        {
            //  DoEvents
            if (GameMode != GM_GAME_ACTIVE) return;
            if (MyMap.Owner(i) == Turn && MyMap.TroopCount(i) > 0 &&
                (AIxCanReachy(i, ToWhere) == 1 || (AIxCanReachy(i, ToWhere) == 2 && (MyMap.CountryType(i) & 1) == 1)))
            {
                // We found a friendly country with troops in it who can reach ToWhere.
                for (var j = 1; j <= 6; j++)
                {
                    // Let's find out what would happen to this country if each other player
                    // were to attack it.
                    if (j != Turn)
                    {
                        CalculateStrengths(j, i);
                        // Let's add the defense proportion to this country.
                        // Basically, if a country is well defended, it gets a high score
                        // meaning troops can leave here pretty easily.
                        // If a country isn't well defended, it gets a low score and troops
                        // are likely to stay (and be reinforced next turn, probably!)
                        Score[i] = Score[i] + ((DefendStrength / (DefendStrength + AttackStrength)) * TMDEFENDPROPENSITY);
                    }
                }
            }
        }

        // Now that we've gone through all that, we need to knock out countries
        // that we don't want to take troops from, like HQ countries.

        for (var i = 1; i <= MyMap.NumberOfCountries; i++)
        {
            //  DoEvents
            if (GameMode != GM_GAME_ACTIVE) return;
            if (MyMap.Owner(i) == Turn && MyMap.TroopCount(i) > 0 &&
                (AIxCanReachy(i, ToWhere) == 1 || (AIxCanReachy(i, ToWhere) == 2 && (MyMap.CountryType(i) & 1) == 1)) &&
                (MyMap.CountryType(i) & 2) == 2)
            {
                // It's an HQ.  We need to add the personality constant...
                Score[i] = Score[i] + TMMOVEFROMHQ;
            }
        }

        AIFindbestscore(Turn);

        if (BestScore <= 0)
        {
            // Nowhere to get troops from, really.
            SendPassToNetwork(Turn, Phase, 0);
            SNDWhistle();
            return;
        }

        // Now BestCountry is the FROM country, and ToWhere is the TO country.
        // We just need to know how much!

        // First we get the attack strengths of each player on the TO country.
        CalculateStrengths(1, ToWhere);
        A1 = AttackStrength;
        CalculateStrengths(2, ToWhere);
        A2 = AttackStrength;
        CalculateStrengths(3, ToWhere);
        A3 = AttackStrength;
        CalculateStrengths(4, ToWhere);
        A4 = AttackStrength;
        CalculateStrengths(5, ToWhere);
        A5 = AttackStrength;
        CalculateStrengths(6, ToWhere);
        A6 = AttackStrength;
        // Grab the defense of the TO country.
        D1 = DefendStrength;

        // Then we get the attack strengths of each player on the FROM country.
        CalculateStrengths(1, BestCountry);
        B1 = AttackStrength;
        CalculateStrengths(2, BestCountry);
        B2 = AttackStrength;
        CalculateStrengths(3, BestCountry);
        B3 = AttackStrength;
        CalculateStrengths(4, BestCountry);
        B4 = AttackStrength;
        CalculateStrengths(5, BestCountry);
        B5 = AttackStrength;
        CalculateStrengths(6, BestCountry);
        B6 = AttackStrength;
        // Grab the defense of the FROM country.
        D2 = DefendStrength;

        // Weigh the overall danger that each country is in.
        if ((A1 + D1) == 0 || (A2 + D1) == 0 || (A3 + D1) == 0 || (A4 + D1) == 0 || (A5 + D1) == 0 || (A6 + D1) == 0)
        {
            AttSum1 = 6;
        }
        else
        {
            AttSum1 = (A1 / (A1 + D1)) + (A2 / (A2 + D1)) + (A3 / (A3 + D1)) + (A4 / (A4 + D1)) + (A5 / (A5 + D1)) + (A6 / (A6 + D1));
        }

        if ((B1 + D2) == 0 || (B2 + D2) == 0 || (B3 + D2) == 0 || (B4 + D2) == 0 || (B5 + D2) == 0 || (B6 + D2) == 0)
        {
            AttSum2 = 6;
        }
        else
        {
            AttSum2 = (B1 / (B1 + D2)) + (B2 / (B2 + D2)) + (B3 / (B3 + D2)) + (B4 / (B4 + D2)) + (B5 / (B5 + D2)) + (B6 / (B6 + D2));
        }

        // If the quantity below is less than a half, then the FROM country is in more danger.
        // Else, the TO country is in more danger, and we should move troops to it!
        if ((AttSum1 + AttSum2) == 0)
        {
            AttProp = 1;    // No one attacking either, so why not combine them all?
        }
        else
        {
            AttProp = AttSum1 / (AttSum1 + AttSum2);
        }

        if (AttProp < 0.5)
        {
            // No troops move between these two countries.  Too risky.
            XferAmount = 0;
        }
        else
        {
            // We can afford to move a few troops!
            XferAmount = (AttProp - 0.5f) * 2 * MyMap.TroopCount(BestCountry);
        }

        // Fudge:  if moving troops across water, and both countries have ports,
        // and we're not moving from our HQ, why not move them all?
        //if (AIxCanReachy(BestCountry, ToWhere) == 2 &&
        //    (MyMap.CountryType(BestCountry) & 1) == 1 &&
        //    (MyMap.CountryType(ToWhere) & 1) == 1 &&
        //    (MyMap.CountryType(BestCountry) & 2) != 2)
        //{
        //    XferAmount = MyMap.TroopCount(BestCountry);
        //}

        // Make sure we're not merging two big numbers.
        if (XferAmount + MyMap.TroopCount(ToWhere) > MAX_COUNTRY_CAPACITY)
        {
            // Yup, we're being stupid.
            XferAmount = MAX_COUNTRY_CAPACITY - MyMap.TroopCount(ToWhere);
        }

        // If we're moving from our HQ, we need to figure in the personality percent.
        if ((MyMap.CountryType(BestCountry) & 2) == 2)
        {
            XferAmount = XferAmount * TMMAXPCTFROMHQ;
        }

        // Now, let's move 'em!
        if (XferAmount < 1 || BestCountry == ToWhere || BestScore <= 0)
        {
            // We're passing our turn.
            SendPassToNetwork(Turn, Phase, 0);
            SNDWhistle();
            return;
        }

        // Moose doesn't make troop movements!  We put this check down here for timing reasons.
        if (Personality[Turn] == 3 && PlayerType[Turn] == PTYPE_COMPUTER)
        {
            SendPassToNetwork(Turn, 4, 0);
            return;
        }

        if (MyNetworkRole == NW_SERVER)
        {
            SendClickToNetwork(Turn, 4,
                (1000000 * (int)XferAmount) + (1000 * BestCountry) + ToWhere, 0);
        }

        TroopMoveProc(BestCountry, ToWhere, (int)XferAmount, Turn);
    }

    public static void AIdelay()
    {
        if (GameMode != GM_GAME_ACTIVE) return;
        // This sub just waits a few seconds to give the player time to see the computer's moves.

        if (CFGAISpeed == 1) return;

        // Otherwise, put a small delay here.
        Application.DoEvents(); // Update the computer's internal time.

        DateTime currentTime = DateTime.UtcNow;
        DateTime targetTime = currentTime.AddSeconds(((CFGAISpeed - 2) * 2) + 1);
        while (currentTime < targetTime)
        {
            Application.DoEvents();
            currentTime = DateTime.UtcNow;

            if (GameMode != GM_GAME_ACTIVE) return;
        }
    }

    private static void AIsetupscore()
    {
        // Set up the scores array.
        Score = new float[MyMap!.NumberOfCountries + 1];

        for (var i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            if (GameMode != GM_GAME_ACTIVE) return;

            Score[i] = 0;
        }
    }

    private static void AIFindbestscore(int turn)
    {
        // This sub finds the country with the best score and puts its number in BestCountry.
        BestScore = NOTPOSSIBLE;

        for (var i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            if (GameMode != GM_GAME_ACTIVE) return;

            // If we beat it or tied it, then we have a new best score.
            if ((Score[i] > BestScore) || ((Score[i] == BestScore) && (Random.Shared.NextDouble() < SAMESCOREKEEPER)))
            {
                BestScore = Score[i];
                BestCountry = i;
            }
        }

        // If this is Clyde, he has a large tendency to take the second-best choice.
        if (Personality[turn] == 6 && Random.Shared.NextDouble() < 0.5)
        {
            // Let's find the second-best score.
            float SecondBestScore = NOTPOSSIBLE;

            for (var i = 1; i <= MyMap.NumberOfCountries; i++)
            {
                if (Score[i] != BestScore)
                {
                    if (Score[i] > SecondBestScore)
                    {
                        SecondBestScore = Score[i];
                        BestCountry = i;
                    }
                }
            }

            BestScore = SecondBestScore;
        }
    }

    public static int AIxCanReachy(int FromCountry, int ToCountry)
    {
        // This sub checkt to see if the FromCountry can reach the ToCountry.
        // // This sub returns 0 if can't reach, 1 if by land, 2 if by sea, -1 if they are the same.
        // Note that we're not considering ports here.

        // Are they the same country?
        if (FromCountry == ToCountry)
        {
            return -1;
        }

        for (int kk = 1; kk <= MyMap!.MaxNeighbors; kk++)
        {
            if (GameMode != GM_GAME_ACTIVE) return 0;

            var neighbor = MyMap.Neighbors(ToCountry, kk);

            if (neighbor >= TILEVAL_COASTLINE)
            {
                for (int ll = 1; ll <= MyMap.MaxNeighbors; ll++)
                {
                    if (MyMap.Neighbors(ToCountry, kk) == MyMap.Neighbors(FromCountry, ll))
                    {
                        // By water.
                        return 2;
                    }
                }
            }

            if (neighbor == 0) return 0; // Didn't find it.

            if (neighbor == FromCountry)
            {
                // By Land.
                return 1;
            }
        }

        return 0;
    }

    private static int AIreachable(int ThisCountry, int Turn)
    {
        // This sub checks to see if the passed country is reachable at all by the
        // current player.  It returns 0 if can't reach, 1 if by land, 2 if by sea,
        // -1 if already owned by this player.

        if (MyMap!.Owner(ThisCountry) == Turn)
        {
            //Already owned.
            return -1;
        }

        if (GameMode != GM_GAME_ACTIVE) return 0;

        for (int k = 1; k <= MyMap.MaxNeighbors; k++)
        {
            if (MyMap.Neighbors(ThisCountry, k) >= TILEVAL_COASTLINE)
            {
                for (int l = 1; l <= MyMap.NumberOfCountries; l++)
                {
                    for (int m = 1; m <= MyMap.MaxNeighbors; m++)
                    {
                        if (MyMap.Neighbors(ThisCountry, k) == MyMap.Neighbors(l, m) &&
                             l != ThisCountry && MyMap.Owner(l) == Turn &&
                             (MyMap.CountryType(l) & 1) == 1)
                        {
                            //By water.
                            return 2;
                        }
                    }
                }
            }
            else if (MyMap.Neighbors(ThisCountry, k) == 0)
            {
                //Didn't find it.
                return 0;
            }
            else if (MyMap.Owner(MyMap.Neighbors(ThisCountry, k)) == Turn)
            {
                //By Land.
                return 1;
            }
        }

        return 0;
    }

    private static void SelectionBall(int ThisCountry, int Size, int Turn)
    {
        // This sub bounces a single ball on the country that the computer just chose.
        // Just so the player can see what's happening.

        BuildBalls(1,
            (MyMap!.DigitCoords(ThisCountry, 1) - 1) * 8,
            (MyMap!.DigitCoords(ThisCountry, 2) - 1) * 8,
            -1,
            0,
            0,
            Size,
            Player[Turn]);
    }

    private static int AIassessHQ(int Turn)
    {
        int i;
        int j;
        TempNum2 = 0;

        if (GameMode != GM_GAME_ACTIVE) return 0;

        for (i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            if (MyMap!.Owner(i) == Turn && (MyMap!.CountryType(i) & 2) == 2)
            {
                // Found this player's HQ.
                // Now let's see what each other attacker can do.
                for (j = 1; j <= 6; j++)
                {
                    if (j != Turn)
                    {
                        CalculateStrengths(j, i);
                        if (AttackStrength == 0 || AttackStrength + DefendStrength == 0)
                        {
                            // Avoid /0.
                            // This player can't get to our HQ.  Don't add anything.
                        }
                        else
                        {
                            // Someone has attack points on our HQ, so let's add a fraction
                            // of the response factor.
                            TempNum2 = TempNum2 + (HQDEFENDPROPENSITY * (AttackStrength / (DefendStrength + AttackStrength)));
                        }
                    }
                }
            }
        }

        return (int)TempNum2;
    }

    private static void UpdateHate(int Attacker, int Attackee)
    {

        // First, the one who is attacked hates the attacker even more.
        Hate[Attackee, Attacker] = Hate[Attackee, Attacker] + ACHATEATTACKED;

        // Now the attacker relieves some of the pent-up emotion.
        Hate[Attacker, Attackee] = Hate[Attacker, Attackee] + ACHATEFORGIVEN;

        // Each other player likes the attacker that much more.
        for (var i = 1; i <= 6; i++)
        {
            if (i != Attacker && i != Attackee)
            {
                Hate[i, Attacker] = Hate[i, Attacker] + ACHATEENEMYOFMYENEMY;
            }
        }

        // Make sure we're within our hate limits.
        for (var i = 1; i <= 6; i++)
        {
            for (var j = 1; j <= 6; j++)
            {
                if (Hate[i, j] < 1) Hate[i, j] = 1;
                if (Hate[i, j] > 100) Hate[i, j] = 100;
            }
        }
    }

    private static void WaitForNoBalls()
    {
        bool NoBallsLeft = false;
        do
        {
            NoBallsLeft = NoBalls(); // This function is in Boom.bas.
            Application.DoEvents();
            if (GameMode != GM_GAME_ACTIVE && GameMode != GM_DIALOG_OPEN) return;
        } while (!NoBallsLeft && !GameEnding);
    }

    public static void InitPersonalities()
    {
        for (int i = 1; i <= 6; i++)
        {
            // Each computer player defaults to Stonewall.
            Personality[i] = 1;
        }

        PersonalityName[1] = "Stonewall";
        PersonalityName[2] = "Bully";
        PersonalityName[3] = "Moose";
        PersonalityName[4] = "Ahab";
        PersonalityName[5] = "Paranoid";
        PersonalityName[6] = "Clyde";
        PersonalityName[7] = "Custom 1";
        PersonalityName[8] = "Custom 2";
        PersonalityName[9] = "Custom 3";
        PersonalityName[10] = "Custom 4";
        PersonalityName[11] = "Custom 5";
        PersonalityName[12] = "Custom 6";
    }

    private static void GetPersonalityData(int Turn)
    {
        int MyPerson;

        if (Turn == 7)
        {
            MyPerson = 7;
        }
        else
        {
            MyPerson = Personality[Turn];
        }

        switch (MyPerson)
        {
            case 1:
            case 7: // Stonewall.
                // Probably the best all-around computer player.
                Stonewall();

                break;

            case 2: // Bully
                // Blindly charges into battles with all it's got.  Picks on smaller countries.
                SAMESCOREKEEPER = 0.5f;       //Chance of keeping a country with the same bestscore.
                HATEFACTOR = 40;               //How badly this personality holds a grudge.
                HQTROOPSMULTIPLIER = 4;        //Number of times that the troops in a country count.
                HQBODYOFWATER = -2;       //Number of points a body of water is worth to HQ.
                HQBORDERINGCOUNTRY = 5;       //Number of points a bordering country is worth to HQ.
                HQONEAWAYMULT = 1.5f;          //Fraction of troops counted for countries 1 away from HQ.
                HQTWOAWAYMULT = 0.5f;           //Fraction of troops counted for countries 2 away from HQ.
                HQNEXTTOENEMYHQ = -2;          //Score for putting HQ near enemy HQs.
                HQNEARCENTERMULT = -0.1f;       //Factor for HQ being closer to the center of the map.
                HQDEFENDPROPENSITY = 100;      //Points per strength difference to defend HQ.
                REDEFLOSINGPROPOSITION = 30;   //Factor for reinforcing defensive troops in a losing battle.
                REDEFWINNINGPROPOSITION = 50;  //Factor for reinforcing defensive troops in a winning battle.
                REATTLOSINGPROPOSITION = 550;  //Factor for reinforcing offensive troops in a losing battle.
                REATTWINNINGPROPOSITION = 800; //Factor for reinforcing offensive troops in a winning battle.
                REPUTPRESSUREONHQBASE = 4100;  //Score for reinforcing near an enemy HQ.
                REPUTPRESSUREONHQMULT = 3500;  //Pro-rated Score for reinforcing near an enemy HQ.
                REBETONASURETHING = 500;       //Score for reinforcing a battle that can already be won.
                ACANNEXTROOPS = 2;             //Factor for annexing a country with troops in it.
                ACANNEXBASE = 10;     //Score for just annexing any piece of land. (* CFGBonusTroops)
                ACANNEXNEARENEMYMULT = 3;   //Multiplier for taking free troops near enemy.
                ACANNEXNEXTTOHQ = 330;   //Score for annexing unclaimed land next to an enemy HQ.
                ACANNEXTWOAWAYMULT = 0.8f;   //Fraction that free troops two countries away are worth.
                ACPORTBASE = 8;       //Score for just building a port.            (* CFGShips)
                ACPORTTROOPS = 1;             //Factor for building a port on a country with troops.
                ACATTACKBASE = 600;        //Score for simply attacking another defending player.
                ACATTACKFACTOR = 2000;       //Pro-rated score for attacking another defending player.
                ACBULLYBASE = 1000;            //Score for beating up a player in an easy battle.
                ACBULLYFACTOR = 1000;          //Pro-rated score for attacking in an easy battle.
                ACCLEANUPEMPTYS = 1000;    //Score for attacking an empty enemy country.
                ACATTACKENEMYHQ = 2900;     //Score for being able to attack enemy HQ.
                ACHATEATTACKED = 50;    //How much more I hate you for attacking me.
                ACHATEFORGIVEN = -2;    //How much more I hate you now that I//ve attacked you.
                ACHATEENEMYOFMYENEMY = -5;    //How much more I hate you now that you//ve attacked someone else.
                TMPUTPRESSUREONHQBASE = 15000; //Score for moving troops near an enemy HQ.
                TMPUTPRESSUREONHQMULT = 7000;  //Pro-rated score for moving troops near an enemy HQ.
                TMSWOOPINSCORE = 3700;         //Score for moving troops into a just-taken country.
                TMDEFENDPROPENSITY = 300;      //Pro-rated score for defending a country with a troop movement.
                TMATTACKPROPENSITY = 7800;     //Pro-rated score for attacking a country with a troop movement.
                TMBETONASURETHING = 3500;      //Score for moving troops to attack a country that can already be taken.
                TMMOVEFROMHQ = 0;              //Score for HQ being the only country to get troops from.
                TMMAXPCTFROMHQ = 1;            //The maximum percentage of troops that can be moved from HQ.
                TMTWOAWAYDEFENSEFACTOR = 0.3f;  //The percent of a normal score for defending a country two moves away.
                break;
            case 3: // Moose
                    //Hates troop movements.  Digs in and doesn't budge.
                SAMESCOREKEEPER = 0.1f;         //Chance of keeping a country with the same bestscore.
                HATEFACTOR = 10;               //How badly this personality holds a grudge.
                HQTROOPSMULTIPLIER = 3;        //Number of times that the troops in a country count.
                HQBODYOFWATER = -10;           //Number of points a body of water is worth to HQ.
                HQBORDERINGCOUNTRY = -3;       //Number of points a bordering country is worth to HQ.
                HQONEAWAYMULT = 1;             //Fraction of troops counted for countries 1 away from HQ.
                HQTWOAWAYMULT = 0.1f;           //Fraction of troops counted for countries 2 away from HQ.
                HQNEXTTOENEMYHQ = -11;         //Score for putting HQ near enemy HQs.
                HQNEARCENTERMULT = -0.2f;       //Factor for HQ being closer to the center of the map.
                HQDEFENDPROPENSITY = 700;      //Points per strength difference to defend HQ.
                REDEFLOSINGPROPOSITION = 80;   //Factor for reinforcing defensive troops in a losing battle.
                REDEFWINNINGPROPOSITION = 150; //Factor for reinforcing defensive troops in a winning battle.
                REATTLOSINGPROPOSITION = 50;   //Factor for reinforcing offensive troops in a losing battle.
                REATTWINNINGPROPOSITION = 150; //Factor for reinforcing offensive troops in a winning battle.
                REPUTPRESSUREONHQBASE = 1100;  //Score for reinforcing near an enemy HQ.
                REPUTPRESSUREONHQMULT = 800;   //Pro-rated Score for reinforcing near an enemy HQ.
                REBETONASURETHING = 50;        //Score for reinforcing a battle that can already be won.
                ACANNEXTROOPS = 3;             //Factor for annexing a country with troops in it.
                ACANNEXBASE = 12;              //Score for just annexing any piece of land. (* CFGBonusTroops)
                ACANNEXNEARENEMYMULT = 1;      //Multiplier for taking free troops near enemy.
                ACANNEXNEXTTOHQ = 21;          //Score for annexing unclaimed land next to an enemy HQ.
                ACANNEXTWOAWAYMULT = 0.9f;      //Fraction that free troops two countries away are worth.
                ACPORTBASE = 10;               //Score for just building a port.            (* CFGShips)
                ACPORTTROOPS = 1;              //Factor for building a port on a country with troops.
                ACATTACKBASE = 200;            //Score for simply attacking another defending player.
                ACATTACKFACTOR = 800;          //Pro-rated score for attacking another defending player.
                ACBULLYBASE = 10;              //Score for beating up a player in an easy battle.
                ACBULLYFACTOR = 1;             //Pro-rated score for attacking in an easy battle.
                ACCLEANUPEMPTYS = 13;          //Score for attacking an empty enemy country.
                ACATTACKENEMYHQ = 1100;        //Score for being able to attack enemy HQ.
                ACHATEATTACKED = 9;            //How much more I hate you for attacking me.
                ACHATEFORGIVEN = -7;           //How much more I hate you now that I//ve attacked you.
                ACHATEENEMYOFMYENEMY = -9;     //How much more I hate you now that you//ve attacked someone else.
                TMPUTPRESSUREONHQBASE = 0;     //Score for moving troops near an enemy HQ.
                TMPUTPRESSUREONHQMULT = 0;     //Pro-rated score for moving troops near an enemy HQ.
                TMSWOOPINSCORE = 0;            //Score for moving troops into a just-taken country.
                TMDEFENDPROPENSITY = 0;       //Pro-rated score for defending a country with a troop movement.
                TMATTACKPROPENSITY = 0;        //Pro-rated score for attacking a country with a troop movement.
                TMBETONASURETHING = 0;         //Score for moving troops to attack a country that can already be taken.
                TMMOVEFROMHQ = 0;              //Score for HQ being the only country to get troops from.
                TMMAXPCTFROMHQ = 0;            //The maximum percentage of troops that can be moved from HQ.
                TMTWOAWAYDEFENSEFACTOR = 0;    //The percent of a normal score for defending a country two moves away.
                break;
            case 4:  //Ahab.
                     //Loves water.  Always builds toward coastlines.
                SAMESCOREKEEPER = 0.7f;         //Chance of keeping a country with the same bestscore.
                HATEFACTOR = 21;               //How badly this personality holds a grudge.
                HQTROOPSMULTIPLIER = 3;        //Number of times that the troops in a country count.
                HQBODYOFWATER = 15;            //Number of points a body of water is worth to HQ.
                HQBORDERINGCOUNTRY = -2;       //Number of points a bordering country is worth to HQ.
                HQONEAWAYMULT = 0.5f;           //Fraction of troops counted for countries 1 away from HQ.
                HQTWOAWAYMULT = 0.1f;           //Fraction of troops counted for countries 2 away from HQ.
                HQNEXTTOENEMYHQ = -10;         //Score for putting HQ near enemy HQs.
                HQNEARCENTERMULT = -0.3f;       //Factor for HQ being closer to the center of the map.
                HQDEFENDPROPENSITY = 900;      //Points per strength difference to defend HQ.
                REDEFLOSINGPROPOSITION = 50;   //Factor for reinforcing defensive troops in a losing battle.
                REDEFWINNINGPROPOSITION = 200; //Factor for reinforcing defensive troops in a winning battle.
                REATTLOSINGPROPOSITION = 300;  //Factor for re;inforcing offensive troops in a losing battle.
                REATTWINNINGPROPOSITION = 350; //Factor for reinforcing offensive troops in a winning battle.
                REPUTPRESSUREONHQBASE = 1800;  //Score for reinforcing near an enemy HQ.
                REPUTPRESSUREONHQMULT = 1200;  //Pro-rated Score for reinforcing near an enemy HQ.
                REBETONASURETHING = 300;       //Score for reinforcing a battle that can already be won.
                ACANNEXTROOPS = 2;             //Factor for annexing a country with troops in it.
                ACANNEXBASE = 10;              //Score for just annexing any piece of land. (* CFGBonusTroops)
                ACANNEXNEARENEMYMULT = 2;      //Multiplier for taking free troops near enemy.
                ACANNEXNEXTTOHQ = 63;          //Score for annexing unclaimed land next to an enemy HQ.
                ACANNEXTWOAWAYMULT = 1;        //Fraction that free troops two countries away are worth.
                ACPORTBASE = 79;               //Score for just building a port.            (* CFGShips)
                ACPORTTROOPS = 4;              //Factor for building a port on a country with troops.
                ACATTACKBASE = 200;            //Score for simply attacking another defending player.
                ACATTACKFACTOR = 1000;         //Pro-rated score for attacking another defending player.
                ACBULLYBASE = 10;              //Score for beating up a player in an easy battle.
                ACBULLYFACTOR = 1;             //Pro-rated score for attacking in an easy battle.
                ACCLEANUPEMPTYS = 30;          //Score for attacking an empty enemy country.
                ACATTACKENEMYHQ = 800;         //Score for being able to attack enemy HQ.
                ACHATEATTACKED = 21;           //How much more I hate you for attacking me.
                ACHATEFORGIVEN = -8;           //How much more I hate you now that I//ve attacked you.
                ACHATEENEMYOFMYENEMY = -1;     //How much more I hate you now that you//ve attacked someone else.
                TMPUTPRESSUREONHQBASE = 8000;  //Score for moving troops near an enemy HQ.
                TMPUTPRESSUREONHQMULT = 3000;  //Pro-rated score for moving troops near an enemy HQ.
                TMSWOOPINSCORE = 0;            //Score for moving troops into a just-taken country.
                TMDEFENDPROPENSITY = 2500;     //Pro-rated score for defending a country with a troop movement.
                TMATTACKPROPENSITY = 2500;     //Pro-rated score for attacking a country with a troop movement.
                TMBETONASURETHING = 500;       //Score for moving troops to attack a country that can already be taken.
                TMMOVEFROMHQ = -700;           //Score for HQ being the only country to get troops from.
                TMMAXPCTFROMHQ = 0.5f;          //The maximum percentage of troops that can be moved from HQ.
                TMTWOAWAYDEFENSEFACTOR = 0.6f;  //The percent of a normal score for defending a country two moves away.
                break;
            case 5:  //Paranoid.
                     //Defensive to the extreme!
                SAMESCOREKEEPER = 0.001f;       //Chance of keeping a country with the same bestscore.
                HATEFACTOR = 1;                //How badly this personality holds a grudge.
                HQTROOPSMULTIPLIER = 2;        //Number of times that the troops in a country count.
                HQBODYOFWATER = -15;           //Number of points a body of water is worth to HQ.
                HQBORDERINGCOUNTRY = -2;       //Number of points a bordering country is worth to HQ.
                HQONEAWAYMULT = 0.6f;           //Fraction of troops counted for countries 1 away from HQ.
                HQTWOAWAYMULT = 0.2f;           //Fraction of troops counted for countries 2 away from HQ.
                HQNEXTTOENEMYHQ = -40;         //Score for putting HQ near enemy HQs.
                HQNEARCENTERMULT = -0.8f;       //Factor for HQ being closer to the center of the map.
                HQDEFENDPROPENSITY = 1600;     //Points per strength difference to defend HQ.
                REDEFLOSINGPROPOSITION = 300;  //Factor for reinforcing defensive troops in a losing battle.
                REDEFWINNINGPROPOSITION = 350; //Factor for reinforcing defensive troops in a winning battle.
                REATTLOSINGPROPOSITION = 50;   //Factor for reinforcing offensive troops in a losing battle.
                REATTWINNINGPROPOSITION = 100; //Factor for reinforcing offensive troops in a winning battle.
                REPUTPRESSUREONHQBASE = 1500;  //Score for reinforcing near an enemy HQ.
                REPUTPRESSUREONHQMULT = 1100;  //Pro-rated Score for reinforcing near an enemy HQ.
                REBETONASURETHING = 600;       //Score for reinforcing a battle that can already be won.
                ACANNEXTROOPS = 2;             //Factor for annexing a country with troops in it.
                ACANNEXBASE = 10;              //Score for just annexing any piece of land. (* CFGBonusTroops)
                ACANNEXNEARENEMYMULT = 2;      //Multiplier for taking free troops near enemy.
                ACANNEXNEXTTOHQ = 10;          //Score for annexing unclaimed land next to an enemy HQ.
                ACANNEXTWOAWAYMULT = 1;        //Fraction that free troops two countries away are worth.
                ACPORTBASE = 5;                //Score for just building a port.            (* CFGShips)
                ACPORTTROOPS = 2;              //Factor for building a port on a country with troops.
                ACATTACKBASE = 200;            //Score for simply attacking another defending player.
                ACATTACKFACTOR = 800;          //Pro-rated score for attacking another defending player.
                ACBULLYBASE = 10;              //Score for beating up a player in an easy battle.
                ACBULLYFACTOR = 1;             //Pro-rated score for attacking in an easy battle.
                ACCLEANUPEMPTYS = 30;          //Score for attacking an empty enemy country.
                ACATTACKENEMYHQ = 500;         //Score for being able to attack enemy HQ.
                ACHATEATTACKED = 3;            //How much more I hate you for attacking me.
                ACHATEFORGIVEN = -3;           //How much more I hate you now that I//ve attacked you.
                ACHATEENEMYOFMYENEMY = -3;    //How much more I hate you now that you//ve attacked someone else.
                TMPUTPRESSUREONHQBASE = 1000;  //Score for moving troops near an enemy HQ.
                TMPUTPRESSUREONHQMULT = 500;   //Pro-rated score for moving troops near an enemy HQ.
                TMSWOOPINSCORE = 0;            //Score for moving troops into a just-taken country.
                TMDEFENDPROPENSITY = 6000;     //Pro-rated score for defending a country with a troop movement.
                TMATTACKPROPENSITY = 1500;     //Pro-rated score for attacking a country with a troop movement.
                TMBETONASURETHING = 500;       //Score for moving troops to attack a country that can already be taken.
                TMMOVEFROMHQ = -900;           //Score for HQ being the only country to get troops from.
                TMMAXPCTFROMHQ = 0;            //The maximum percentage of troops that can be moved from HQ.
                TMTWOAWAYDEFENSEFACTOR = 1;    //The percent of a normal score for defending a country two moves away.
                break;
            case 6:  //Clyde.
                     //The 'easy' computer opponent.  Does the occasional dumb move.
                     //This is handled in code -- the stats are the same as Stonewall.
                SAMESCOREKEEPER = 0.2f;         //Chance of keeping a country with the same bestscore.
                HATEFACTOR = 15;               //How badly this personality holds a grudge.
                HQTROOPSMULTIPLIER = 3;        //Number of times that the troops in a country count.
                HQBODYOFWATER = -5;            //Number of points a body of water is worth to HQ.
                HQBORDERINGCOUNTRY = 1;        //Number of points a bordering country is worth to HQ.
                HQONEAWAYMULT = 1;             //Fraction of troops counted for countries 1 away from HQ.
                HQTWOAWAYMULT = 0.3f;           //Fraction of troops counted for countries 2 away from HQ.
                HQNEXTTOENEMYHQ = -10;         //Score for putting HQ near enemy HQs.
                HQNEARCENTERMULT = -0.1f;       //Factor for HQ being closer to the center of the map.
                HQDEFENDPROPENSITY = 500;      //Points per strength difference to defend HQ.
                REDEFLOSINGPROPOSITION = 50;   //Factor for reinforcing defensive troops in a losing battle.
                REDEFWINNINGPROPOSITION = 200; //Factor for reinforcing defensive troops in a winning battle.
                REATTLOSINGPROPOSITION = 250; //Factor for reinforcing offensive troops in a losing battle.
                REATTWINNINGPROPOSITION = 300; //Factor for reinforcing offensive troops in a winning battle.
                REPUTPRESSUREONHQBASE = 2100;  //Score for reinforcing near an enemy HQ.
                REPUTPRESSUREONHQMULT = 1500;  //Pro-rated Score for reinforcing near an enemy HQ.
                REBETONASURETHING = 50;        //Score for reinforcing a battle that can already be won.
                ACANNEXTROOPS = 1;             //Factor for annexing a country with troops in it.
                ACANNEXBASE = 10;              //Score for just annexing any piece of land. (* CFGBonusTroops)
                ACANNEXNEARENEMYMULT = 2;      //Multiplier for taking free troops near enemy.
                ACANNEXNEXTTOHQ = 47;          //Score for annexing unclaimed land next to an enemy HQ.
                ACANNEXTWOAWAYMULT = 0.8f;      //Fraction that free troops two countries away are worth.
                ACPORTBASE = 10;               //Score for just building a port.            (* CFGShips)
                ACPORTTROOPS = 1;              //Factor for building a port on a country with troops.
                ACATTACKBASE = 200;            //Score for simply attacking another defending player.
                ACATTACKFACTOR = 1000;         //Pro-rated score for attacking another defending player.
                ACBULLYBASE = 10;              //Score for beating up a player in an easy battle.
                ACBULLYFACTOR = 1;             //Pro-rated score for attacking in an easy battle.
                ACCLEANUPEMPTYS = 9;           //Score for attacking an empty enemy country.
                ACATTACKENEMYHQ = 900;         //Score for being able to attack enemy HQ.
                ACHATEATTACKED = 11;           //How much more I hate you for attacking me.
                ACHATEFORGIVEN = -3;           //How much more I hate you now that I//ve attacked you.
                ACHATEENEMYOFMYENEMY = -7;     //How much more I hate you now that you//ve attacked someone else.
                TMPUTPRESSUREONHQBASE = 10000; //Score for moving troops near an enemy HQ.
                TMPUTPRESSUREONHQMULT = 4000;  //Pro-rated score for moving troops near an enemy HQ.
                TMSWOOPINSCORE = 1500;         //Score for moving troops into a just-taken country.
                TMDEFENDPROPENSITY = 1100;    //Pro-rated score for defending a country with a troop movement.
                TMATTACKPROPENSITY = 3800;     //Pro-rated score for attacking a country with a troop movement.
                TMBETONASURETHING = 500;       //Score for moving troops to attack a country that can already be taken.
                TMMOVEFROMHQ = -100;           //Score for HQ being the only country to get troops from.
                TMMAXPCTFROMHQ = 1;            //The maximum percentage of troops that can be moved from HQ.
                TMTWOAWAYDEFENSEFACTOR = 0.75f; //The percent of a normal score for defending a country two moves away.
                break;

            default:
                // We are using a custom personality.  Let's grab the appropriate file and
                // read the personality parameters.
                var AIFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Custom" + (Personality[Turn] - 6).ToString().Trim() + ".AI");

                // If the file doesn't exist, we exit.
                if (!File.Exists(AIFile))
                {
                    // Default to Stonewall.
                    Stonewall();
                    return;
                }

                using (var sr = new StreamReader(AIFile))
                {
                    // Comment Line.
                    sr.ReadLine();
                    SAMESCOREKEEPER = CutNumberOut(sr.ReadLine());
                    HATEFACTOR = (int)CutNumberOut(sr.ReadLine());
                    HQTROOPSMULTIPLIER = (int)CutNumberOut(sr.ReadLine());
                    HQBODYOFWATER = (int)CutNumberOut(sr.ReadLine());
                    HQBORDERINGCOUNTRY = (int)CutNumberOut(sr.ReadLine());
                    HQONEAWAYMULT = CutNumberOut(sr.ReadLine());
                    HQTWOAWAYMULT = CutNumberOut(sr.ReadLine());
                    HQNEXTTOENEMYHQ = CutNumberOut(sr.ReadLine());
                    HQNEARCENTERMULT = CutNumberOut(sr.ReadLine());
                    HQDEFENDPROPENSITY = (int)CutNumberOut(sr.ReadLine());
                    REDEFLOSINGPROPOSITION = (int)CutNumberOut(sr.ReadLine());
                    REDEFWINNINGPROPOSITION = (int)CutNumberOut(sr.ReadLine());
                    REATTLOSINGPROPOSITION = (int)CutNumberOut(sr.ReadLine());
                    REATTWINNINGPROPOSITION = (int)CutNumberOut(sr.ReadLine());
                    REPUTPRESSUREONHQBASE = (int)CutNumberOut(sr.ReadLine());
                    REPUTPRESSUREONHQMULT = (int)CutNumberOut(sr.ReadLine());
                    REBETONASURETHING = (int)CutNumberOut(sr.ReadLine());
                    ACANNEXTROOPS = CutNumberOut(sr.ReadLine());
                    ACANNEXBASE = CutNumberOut(sr.ReadLine());
                    ACANNEXNEARENEMYMULT = CutNumberOut(sr.ReadLine());
                    ACANNEXNEXTTOHQ = CutNumberOut(sr.ReadLine());
                    ACANNEXTWOAWAYMULT = CutNumberOut(sr.ReadLine());
                    ACPORTBASE = (int)CutNumberOut(sr.ReadLine());
                    ACPORTTROOPS = (int)CutNumberOut(sr.ReadLine());
                    ACATTACKBASE = (int)CutNumberOut(sr.ReadLine());
                    ACATTACKFACTOR = (int)CutNumberOut(sr.ReadLine());
                    ACBULLYBASE = (int)CutNumberOut(sr.ReadLine());
                    ACBULLYFACTOR = (int)CutNumberOut(sr.ReadLine());
                    ACCLEANUPEMPTYS = (int)CutNumberOut(sr.ReadLine());
                    ACATTACKENEMYHQ = (int)CutNumberOut(sr.ReadLine());
                    ACHATEATTACKED = (int)CutNumberOut(sr.ReadLine());
                    ACHATEFORGIVEN = (int)CutNumberOut(sr.ReadLine());
                    ACHATEENEMYOFMYENEMY = (int)CutNumberOut(sr.ReadLine());
                    TMPUTPRESSUREONHQBASE = CutNumberOut(sr.ReadLine());
                    TMPUTPRESSUREONHQMULT = CutNumberOut(sr.ReadLine());
                    TMSWOOPINSCORE = CutNumberOut(sr.ReadLine());
                    TMDEFENDPROPENSITY = CutNumberOut(sr.ReadLine());
                    TMATTACKPROPENSITY = CutNumberOut(sr.ReadLine());
                    TMBETONASURETHING = CutNumberOut(sr.ReadLine());
                    TMMOVEFROMHQ = CutNumberOut(sr.ReadLine());
                    TMMAXPCTFROMHQ = CutNumberOut(sr.ReadLine());
                    TMTWOAWAYDEFENSEFACTOR = CutNumberOut(sr.ReadLine());

                    break;

                }
        }
    }

    private static float CutNumberOut(string? CurrentLine)
    {
        // Each line has the form
        // VARNAME = XX      'Comment
        // The comment may not exist.  We need to grab everything after the
        // equals sign, but before an apostrophe (or the end of the line).
        var Pos = CurrentLine!.IndexOf('=');
        if (Pos > 0)
        {
            // Found the equals sign.
            CurrentLine = CurrentLine[(Pos + 1)..]; // Right(CurrentLine, Len(CurrentLine) - Pos)
            // Step 2:  Now find the first apostrophe.
            Pos = CurrentLine.IndexOf('\'');
            if (Pos > 0)
            {
                // Found the apostrophe.  Now save everything before it.
                CurrentLine = CurrentLine[..Pos]; // Left(CurrentLine, Pos - 1)
                return (float)Convert.ToDouble(CurrentLine.Trim()); // Val(Trim(CurrentLine))
            }
            // Couldn't find an apostrophe!  We'll assume there is no comment.
            return (float)Convert.ToDouble(CurrentLine.Trim()); // Val(Trim(CurrentLine))
        }
        else
        {
            // Couldn't find an equals sign on the line!  Error!
            return 0; // CutNumberOut = 0
        }
    }

    private static void Stonewall()
    {
        SAMESCOREKEEPER = 0.2f;        //Chance of keeping a country with the same bestscore. 

        HATEFACTOR = 15;               //How badly this personality holds a grudge.
        HQTROOPSMULTIPLIER = 3;        //Number of times that the troops in a country count.
        HQBODYOFWATER = -5;            //Number of points a body of water is worth to HQ.
        HQBORDERINGCOUNTRY = 1;        //Number of points a bordering country is worth to HQ.
        HQONEAWAYMULT = 1;             //Fraction of troops counted for countries 1 away from HQ.
        HQTWOAWAYMULT = 0.3f;           //Fraction of troops counted for countries 2 away from HQ.
        HQNEXTTOENEMYHQ = -10;         //Score for putting HQ near enemy HQs.
        HQNEARCENTERMULT = -0.1f;       //Factor for HQ being closer to the center of the map.
        HQDEFENDPROPENSITY = 500;      //Points per strength difference to defend HQ.
        REDEFLOSINGPROPOSITION = 50;   //Factor for reinforcing defensive troops in a losing battle.
        REDEFWINNINGPROPOSITION = 200; //Factor for reinforcing defensive troops in a winning battle.
        REATTLOSINGPROPOSITION = 250;  //Factor for reinforcing offensive troops in a losing battle.
        REATTWINNINGPROPOSITION = 300; //Factor for reinforcing offensive troops in a winning battle.
        REPUTPRESSUREONHQBASE = 2100;  //Score for reinforcing near an enemy HQ.
        REPUTPRESSUREONHQMULT = 1500;  //Pro-rated Score for reinforcing near an enemy HQ.
        REBETONASURETHING = 50;        //Score for reinforcing a battle that can already be won.
        ACANNEXTROOPS = 1;             //Factor for annexing a country with troops in it.
        ACANNEXBASE = 10;              //Score for just annexing any piece of land. (* CFGBonusTroops)
        ACANNEXNEARENEMYMULT = 2;      //Multiplier for taking free troops near enemy.
        ACANNEXNEXTTOHQ = 47;          //Score for annexing unclaimed land next to an enemy HQ.
        ACANNEXTWOAWAYMULT = 0.8f;      //Fraction that free troops two countries away are worth.
        ACPORTBASE = 10;               //Score for just building a port.            (* CFGShips)
        ACPORTTROOPS = 1;              //Factor for building a port on a country with troops.
        ACATTACKBASE = 200;            //Score for simply attacking another defending player.
        ACATTACKFACTOR = 1000;         //Pro-rated score for attacking another defending player.
        ACBULLYBASE = 10;              //Score for beating up a player in an easy battle.
        ACBULLYFACTOR = 1;             //Pro-rated score for attacking in an easy battle.
        ACCLEANUPEMPTYS = 9;           //Score for attacking an empty enemy country.
        ACATTACKENEMYHQ = 900;         //Score for being able to attack enemy HQ.
        ACHATEATTACKED = 11;           //How much more I hate you for attacking me.
        ACHATEFORGIVEN = -3;           //How much more I hate you now that I//ve attacked you.
        ACHATEENEMYOFMYENEMY = -7;  //How much more I hate you now that you//ve attacked someone else.
        TMPUTPRESSUREONHQBASE = 10000; //Score for moving troops near an enemy HQ.
        TMPUTPRESSUREONHQMULT = 4000;  //Pro-rated score for moving troops near an enemy HQ.
        TMSWOOPINSCORE = 1500;     //Score for moving troops into a just-taken country.
        TMDEFENDPROPENSITY = 1100;    //Pro-rated score for defending a country with a troop movement.
        TMATTACKPROPENSITY = 3800;     //Pro-rated score for attacking a country with a troop movement.
        TMBETONASURETHING = 500;       //Score for moving troops to attack a country that can already be taken.
        TMMOVEFROMHQ = -100;           //Score for HQ being the only country to get troops from.
        TMMAXPCTFROMHQ = 0.2f;          //The maximum percentage of troops that can be moved from HQ.
        TMTWOAWAYDEFENSEFACTOR = 0.75f; //The percent of a normal score for defending a country two moves away.
    }

    public static void ChooseHQProc(int BestCountry, int Turn)
    {
        SelectionBall(BestCountry, 5, Turn);
        WaitForNoBalls();
        ClaimHQ(BestCountry, Turn);
        DrawMap();
        SNDPlayFanfare(Player[Turn]);
    }

    public static void ReinforceProc(int BestCountry, int Turn)
    {
        if (WonTurn == 1 && WonGame) return;
        WaitForNoBalls();
        SelectionBall(BestCountry, 5, Turn);
        WaitForNoBalls();
        Reinforce(BestCountry, Turn);
        ShortFlash(BestCountry);
        DrawMap();
        SNDTroopsIn(Player[Turn]);
    }

    public static void ActionProc(int BestCountry, int Turn)
    {
        if (WonTurn == 1 && WonGame) return;

        WaitForNoBalls();
        if (MyMap!.Owner(BestCountry) == 0)
        {
            //We're annexing.
            SelectionBall(BestCountry, 5, Turn);
            WaitForNoBalls();
            AnnexCountry(BestCountry, Turn);
            ShortFlash(BestCountry);
            DrawMap();
            SNDPlayFanfare(Player[Turn]);
        }
        else if (MyMap.Owner(BestCountry) == Turn)
        {
            //We're making a port.
            SelectionBall(BestCountry, 5, Turn);
            WaitForNoBalls();
            MakePort(BestCountry);
            ShortFlash(BestCountry);
            DrawMap();
            SNDBuildAPort();
        }
        else if (MyMap.Owner(BestCountry) != Turn)
        {
            //We're attacking.
            UpdateHate(Turn, MyMap.Owner(BestCountry));
            CalculateStrengths(Turn, BestCountry);   //Gotta get the right numbers again!
            AttackCountry(BestCountry, Turn);
            ShortFlash(BestCountry);
            DrawMap();
            AttackedCountryThisTurn = BestCountry;        //Used during troop movement.
        }
    }

    public static void TroopMoveProc(int BestCountry, int ToWhere, int XferAmount, int turn)
    {
        if (WonTurn == 1 && WonGame) return;

        MsgXferAmount = XferAmount;  // Puts up the right message.
        Land.Refs.UpdateMessages();
        Application.DoEvents();

        WaitForNoBalls();
        KillTroops(BestCountry, XferAmount);
        ShortFlash(BestCountry);
        DrawMap();
        SelectionBall(ToWhere, 5, turn);
        SelectionBall(BestCountry, 6, turn);
        SNDTroopsOut(Player[turn]);
        WaitForNoBalls();
        AddTroops(ToWhere, XferAmount);
        ShortFlash(ToWhere);
        DrawMap();
        SNDTroopsIn(Player[turn]);
    }
}
