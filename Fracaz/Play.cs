using static Fracaz.Declarations;
using static Fracaz.GraphicFX;
using static Fracaz.NetworkPlay;
using static Fracaz.RandomEvents;
using static Fracaz.SoundFX;

namespace Fracaz;

public class Play
{
    public static void NextTurn(int Turn, int Phase)
    {
        // This sub just bumps up the turn counter.

        Application.DoEvents();
        Land.Refs.Oopsie.Text = "";
        if (GameMode != GM_GAME_ACTIVE) return;

        Land.Refs.SetupIndicators();

        Turn += 1;
        Phase = 1;
        if (Turn > 6)
        {
            Turn = 1;
            TurnCounter += 1;
        }

        // Set this false so that we'll check for an event at the start of the next player's turn.
        CheckedForEvent = false;
        // Clear a few more flags for 2.0 BETA incidents...
        AlreadyAddedTroopsThisPhase = false;
        AlreadyPerformedActionThisPhase = false;
        AlreadyMovedTroopsThisPhase = false;
        AlreadyChoseTroopsThisPhase = false;
        AlreadyCheckedForMaxedOutCountries = false;

        // Only enable the save game option if this is the start of a human's turn,
        // and if the map has been saved already!  No saved games during network games.
        if (PlayerType[Turn] == PTYPE_HUMAN && NumOccupied[Turn] > 0 && MyNetworkRole == NW_NONE)
        {
            Land.Refs.MenuSaveGame.Enabled = true;
        }
        else
        {
            Land.Refs.MenuSaveGame.Enabled = false;
        }

        // Check for a random event at the start of this player's turn.
        if ((CFGEvents > 1) && !CheckedForEvent && (NumOccupied[Turn] > 0))
        {
            // Only check if we're configured to do so.
            RollForRandomEvent(Turn);
            CheckedForEvent = true;
        }

        Land.Refs.SetTurn(Turn);
        Land.Refs.SetPhase(Phase);

        Land.Refs.SetUpPlayerControls();

        Land.Refs.UpdateMessages();
    }

    public static void NextPhase(int Turn, int Phase)
    {
        Application.DoEvents();
        Land.Refs.Oopsie.Text = "";
        if (GameMode != GM_GAME_ACTIVE) return;

        Land.Refs.SetupIndicators();

        AlreadyPassedThisPhase = false;

        if (Phase == 1)
        {
            Phase = 2;
            Land.Refs.SetPhase(Phase);
            Land.Refs.SetUpPlayerControls();
            Land.Refs.UpdateMessages();
            Application.DoEvents();
            return;
        }

        if (Phase == 2)
        {
            // Let's check and see if we have more than one country.
            // If not, there is no troop movement phase.
            if (NumOccupied[Turn] <= 1)
            {
                NextTurn(Turn, Phase);
                Land.Refs.SetUpPlayerControls();
                return;
            }

            // Let's go to the troop movement phase.
            Land.Refs.TroopMoveNum.Text = "0";
            Phase = 3;
            Land.Refs.SetPhase(Phase);
            Land.Refs.SetUpPlayerControls();
            Land.Refs.UpdateMessages();
            Application.DoEvents();
            return;
        }

        if (Phase == 3)
        {
            // Set up the default number of troops in the text box.
            long TempNum = MyMap!.TroopCount(TroopMoveSrc);   // Default to all troops.
            if (TempNum == 0) TempNum = 1;
            Land.Refs.TroopMoveNum.Text = TempNum.ToString();
            Phase = 4;
            Land.Refs.SetPhase(Phase);
            Land.Refs.SetUpPlayerControls();
            return;
        }

        if (Phase == 4)
        {
            NextTurn(Turn, Phase);
            Land.Refs.SetUpPlayerControls();
            return;
        }

    }

    public static void PlayerClick(int CurrentMouse, int Turn, int Phase)
    {
        // In this sub we perform whatever action the current player
        // is trying to do.  We only get here on a human's turn.

        // This sub acts on a left mouse click.  First we determine
        // which phase of the player's turn it is, then act accordingly.
        // Note that CurrentMouse contains the ID of the entity that
        // is currently selected.

        // If the user clicked on water, then leave this sub.
        if (CurrentMouse >= TILEVAL_COASTLINE) return;

        // If the user owns no countries, then Phase isn't important --
        // this is their first turn.
        if (NumOccupied[Turn] == 0)
        {
            // Leave if the computer should be picking HQ for us (automatic HQ selection).
            if (CFGHQSelect == 2) return;

            // Let's see if the country they chose is already occupied.
            if (MyMap!.Owner(CurrentMouse) == 0)
            {
                // This country is not owned by anyone -- let's give it to this player.
                // This will be this players HQ from now on, so we'll mark it as such.
                SendClickToNetwork(Turn, Phase, CurrentMouse, 0);
                ClaimHQ(CurrentMouse, Turn);
                DrawMap();
                SNDPlayFanfare(Player[Turn]);
                NextTurn(Turn, Phase);
                return;
            }
            else
            {
                // This country is already owned.
                Land.Refs.Oopsie.Text = "That country is already taken.";
                SNDBooBoo();
                return;
            }
        }

        switch (Phase)
        {
            case 1:
                // This is phase 1 -- Place troops.
                // Bugfix for 2.0: If we already added troops this phase, leave.
                // In 1.1 you could add troops forever by clicking repeatedly.
                if (AlreadyAddedTroopsThisPhase) return;
                // If there are no troops to add, then leave!
                if (CFGBonusTroops == 0) return;
                if (MyMap!.Owner(CurrentMouse) == Turn)
                {
                    // The current player owns this country.  Good!
                    if (MyMap.TroopCount(CurrentMouse) >= MAX_COUNTRY_CAPACITY)
                    {
                        Land.Refs.Oopsie.Text = "This country is already maxed out.  The maximum allowable population of any country is 999.";
                        SNDBooBoo();
                        return;
                    }
                    AlreadyAddedTroopsThisPhase = true;
                    SendClickToNetwork(Turn, Phase, CurrentMouse, 0);
                    Reinforce(CurrentMouse, Turn);
                    SNDTroopsIn(Player[Turn]);
                    ShortFlash(CurrentMouse);
                    DrawMap();
                    NextPhase(Turn, Phase);
                    return;
                }
                else
                {
                    // This country is not owned or is owned by someone else.
                    Land.Refs.Oopsie.Text = "You must put troops in your own country.";
                    SNDBooBoo();
                    return;
                }

            case 2:
                // This is phase 2 -- attack, annex, or build port.
                // Bugfix for 2.0: If we already performed an action this phase, leave. 
                // In 1.1 you could annex multiple countries by clicking repeatedly.
                if (AlreadyPerformedActionThisPhase) return;
                // CurrentMouse is the country we want to attack, annex, or build a port on.
                if (MyMap!.Owner(CurrentMouse) == 0)
                {
                    // This country is unowned, so we will annex it -- But
                    // only if it is adjacent to a country we own!
                    for (int i = 1; i <= MyMap.MaxNeighbors; i++)
                    {
                        // Let's look at each neighbor of the chosen country.
                        if (MyMap.Neighbors(CurrentMouse, i) < TILEVAL_COASTLINE && MyMap.Neighbors(CurrentMouse, i) > 0)
                        {
                            // This neighbor is land.
                            if (MyMap.Owner(MyMap.Neighbors(CurrentMouse, i)) == Turn)
                            {
                                // Yup, there's an adjacent country owned by this player.
                                AlreadyPerformedActionThisPhase = true;
                                SendClickToNetwork(Turn, Phase, CurrentMouse, 0);
                                AnnexCountry(CurrentMouse, Turn);
                                SNDPlayFanfare(Player[Turn]);
                                ShortFlash(CurrentMouse);
                                DrawMap();
                                NextPhase(Turn, Phase);
                                return;
                            }
                        }
                        else if (MyMap.Neighbors(CurrentMouse, i) >= TILEVAL_COASTLINE)
                        {
                            // Let's see if the chosen country is accessible by water.
                            for (int a = 1; a <= MyMap.NumberOfCountries; a++)
                            {
                                // The country we're looking at has to belong to this player,
                                // and it has to have a port!
                                if ((MyMap.Owner(a) == Turn) && ((MyMap.CountryType(a) & 1) == 1))
                                {
                                    for (int j = 1; j <= MyMap.MaxNeighbors; j++)
                                    {
                                        // If they share the same water mass as a neighbor...
                                        if (MyMap.Neighbors(a, j) == MyMap.Neighbors(CurrentMouse, i))
                                        {
                                            // Then they can sail a boat there!  Claim it.
                                            AlreadyPerformedActionThisPhase = true;
                                            SendClickToNetwork(Turn, Phase, CurrentMouse, 0);
                                            AnnexCountry(CurrentMouse, Turn);
                                            SNDPlayFanfare(Player[Turn]);
                                            ShortFlash(CurrentMouse);
                                            DrawMap();
                                            NextPhase(Turn, Phase);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // No adjacent countries are owned by this player.  Ignore the click.
                    Land.Refs.Oopsie.Text = "You cannot reach that country to annex it.";
                    SNDBooBoo();
                    return;
                }
                else if (MyMap.Owner(CurrentMouse) != Turn)
                {
                    // We are attacking someone!
                    // Let's calculate our attack strength.  This is the sum of all of this
                    // player's troop counts in countries that border this one.
                    // We also calculate the defense strength of this country.  This is the
                    // sum of all of this country's owner's troop counts in this country and
                    // adjacent ones.

                    CalculateStrengths(Turn, CurrentMouse);

                    if (AttackStrength == 0)
                    {
                        // They can't attack this country -- either too far away or no manpower.
                        // Leave quietly so they can choose again.
                        Land.Refs.Oopsie.Text = "You cannot reach that country to attack it.";
                        SNDBooBoo();
                        return;
                    }

                    // Now we kill stuff!
                    // When attacking another country, you only need to have more
                    // attack strength than your opponent to do damage.  The opponent
                    // loses the difference in strengths.  If they drop to zero, this country
                    // is ours!  That's all there is to it.
                    if (AttackStrength > DefendStrength)
                    {
                        AlreadyPerformedActionThisPhase = true;
                        SendClickToNetwork(Turn, Phase, CurrentMouse, 0);
                        AttackCountry(CurrentMouse, Turn);
                        ShortFlash(CurrentMouse);
                        DrawMap();
                        NextPhase(Turn, Phase);
                        return;
                    }
                    else
                    {
                        // Can't attack -- not enough strength!
                        Land.Refs.Oopsie.Text = "You do not yet have enough strength to attack that country.";
                        SNDBooBoo();
                        return;
                    }
                }
                else
                {
                    // This country is owned by this player.  Let's build a port on it!
                    // First we see if this country borders water.
                    if (Coastal(CurrentMouse) == false)
                    {
                        // This country doesn't border water!  No point in putting a port on it.
                        Land.Refs.Oopsie.Text = "To build a port, a country must border a body of water.";
                        SNDBooBoo();
                        return;
                    }

                    // Does this country already have a port?  Check bit 1.
                    if ((MyMap.CountryType(CurrentMouse) & 1) == 1)
                    {
                        // We already have one here!  Leave quietly.
                        Land.Refs.Oopsie.Text = "That country already has a port.";
                        SNDBooBoo();
                        return;
                    }

                    // Let's put a port here!  Turn on bit 1.
                    AlreadyPerformedActionThisPhase = true;
                    SendClickToNetwork(Turn, Phase, CurrentMouse, 0);
                    MakePort(CurrentMouse);
                    SNDBuildAPort();
                    ShortFlash(CurrentMouse);
                    DrawMap();
                    NextPhase(Turn, Phase);
                    return;
                }
            case 3:
                // This is phase 3 -- Troop movement source select.
                if (AlreadyChoseTroopsThisPhase) return;

                TroopMoveSrc = 0;  //To prevent a second click before this is done!

                if (MyMap!.Owner(CurrentMouse) != Turn)
                {
                    //We didn't click on one of our countries.
                    Land.Refs.Oopsie.Text = "You must move troops from your own country.";
                    SNDBooBoo();
                    return;
                }
                else if (MyMap.TroopCount(CurrentMouse) == 0)
                {
                    //This country is ours, but no troops are in it!
                    Land.Refs.Oopsie.Text = "There are no troops in that country.";
                    SNDBooBoo();
                }
                else
                {
                    //We picked a country of our own that has troops in it.
                    AlreadyChoseTroopsThisPhase = true;
                    SNDTroopsOut(Player[Turn]);
                    ShortFlash(CurrentMouse);
                    DrawMap();
                    TroopMoveSrc = CurrentMouse;
                    NextPhase(Turn, Phase);
                }

                break;

            case 4:
                // This is phase 4 -- Troop movement destination select.
                // Prevention for 2.0: If we already moved troops this phase, leave.
                // If we aren't yet ready for troop movements, then leave.  (Timing problem)
                if (AlreadyMovedTroopsThisPhase) return;
                if (TroopMoveSrc == 0) return;
                if (int.Parse(Land.Refs.TroopMoveNum.Text) == 0) return;
                if (MyMap!.Owner(CurrentMouse) != Turn)
                {
                    // This isn't our country.
                    Land.Refs.Oopsie.Text = "You must move troops to your own country.";
                    SNDBooBoo();
                    return;
                }

                if (CurrentMouse == TroopMoveSrc)
                {
                    // We chose to move troops to the same country.  Goofy!
                    Land.Refs.Oopsie.Text = "You must move troops to a different country.";
                    SNDBooBoo();
                    return;
                }

                if (MyMap.TroopCount(CurrentMouse) >= MAX_COUNTRY_CAPACITY)
                {
                    // This country is maxed out.
                    Land.Refs.Oopsie.Text = "This country is already maxed out.  The maximum allowable population of any country is 999.";
                    SNDBooBoo();
                    return;
                }

                // We need to see if this country is adjacent to the last one.
                bool Done = false;
                for (int i = 1; i <= MyMap.MaxNeighbors; i++)
                {
                    if (MyMap.Neighbors(CurrentMouse, i) == TroopMoveSrc)
                    {
                        // We are a direct neighbor.
                        Done = true;
                        break;
                    }
                    else if ((MyMap.Neighbors(CurrentMouse, i) >= TILEVAL_COASTLINE) && ((MyMap.CountryType(TroopMoveSrc) & 1) == 1))
                    {
                        // We need to check over water.
                        for (int j = 1; j <= MyMap.MaxNeighbors; j++)
                        {
                            if (MyMap.Neighbors(CurrentMouse, i) == MyMap.Neighbors(TroopMoveSrc, j))
                            {
                                // We can reach it over water.
                                Done = true;
                                break;
                            }
                        }
                    }
                    if (Done == true) break;
                }

                if (Done == false)
                {
                    // We can't reach the country in question.
                    Land.Refs.Oopsie.Text = "Your troops cannot reach that country.";
                    SNDBooBoo();
                    return;
                }

                if (MyMap.TroopCount(CurrentMouse) + int.Parse(Land.Refs.TroopMoveNum.Text) > MAX_COUNTRY_CAPACITY)
                {
                    // We can't move all the troops in.  Only move as many as it takes to max it.
                    Land.Refs.TroopMoveNum.Text = (MAX_COUNTRY_CAPACITY - MyMap.TroopCount(CurrentMouse)).ToString();
                }

                // Now, we move troops from TroopMoveSrc to CurrentMouse!
                AlreadyMovedTroopsThisPhase = true;
                // Encode the source country, dest country, and movement amount.
                SendClickToNetwork(Turn, Phase, (1000000 * int.Parse(Land.Refs.TroopMoveNum.Text))
                                     + (1000 * TroopMoveSrc) + CurrentMouse, 0);
                AddTroops(CurrentMouse, int.Parse(Land.Refs.TroopMoveNum.Text));
                KillTroops(TroopMoveSrc, int.Parse(Land.Refs.TroopMoveNum.Text));
                SNDTroopsIn(Player[Turn]);
                ShortFlash(CurrentMouse);
                DrawMap();
                NextTurn(Turn, Phase);

                break;
        }
    }

    public static void AnnexCountry(int ThisCountry, int PlayerNumber)
    {
        // This sub claims a country for a player, makes it their color,
        // and updates the player's totals.

        bool KillIt = false;
        MyMap!.Owner(ThisCountry, PlayerNumber);
        MyMap.CountryColor(ThisCountry, Player[PlayerNumber]);
        NumOccupied[PlayerNumber] += 1;
        NumTroops[PlayerNumber] += MyMap.TroopCount(ThisCountry);
        // Let's make sure this is not an HQ by removing bit 2.
        MyMap.CountryType(ThisCountry, MyMap.CountryType(ThisCountry) & 253);

        // Let's destroy its port if configured to do so.
        switch (CFGPorts)
        {
            case 1:
                // Captured.  Do nothing.
                KillIt = false;
                break;
            case 2:
                // Destroyed.  Kill the port.
                KillIt = true;
                break;
            case 3:
                // Depends on how bad we beat up the other guy.
                if (DefendStrength == 0) // Don't want division by 0 below.
                    KillIt = true;
                else if ((AttackStrength / DefendStrength) > 4)
                    KillIt = true;
                break;
            case 4:
                // Random.  If we are a network client, don't do this, wait for new data from the server.
                if (MyNetworkRole != NW_CLIENT)
                {
                    if (Random.Shared.NextDouble() < 0.5)
                        KillIt = true;
                }
                break;
        }

        // Kill the port if we're supposed to.
        if (KillIt)
        {
            MyMap.CountryType(ThisCountry, MyMap.CountryType(ThisCountry) & 254);
        }
    }

    private static void SecedeCountry(int ThisCountry)
    {
        // This sub updates a player's totals when a country is lost.
        NumOccupied[MyMap!.Owner(ThisCountry)] -= 1;
        NumTroops[MyMap.Owner(ThisCountry)] -= MyMap.TroopCount(ThisCountry);
    }

    public static void AddTroops(int ToCountry, int Amount)
    {
        // This sub just adds troops to a country.
        MyMap!.TroopCount(ToCountry, MyMap.TroopCount(ToCountry) + Amount);

        // We need to make sure we didn't overshoot the limit!
        if (MyMap.TroopCount(ToCountry) > MAX_COUNTRY_CAPACITY) MyMap.TroopCount(ToCountry, MAX_COUNTRY_CAPACITY);
    }

    public static void KillTroops(int FromCountry, int Amount)
    {
        // This sub kills troops from a country.
        MyMap!.TroopCount(FromCountry, MyMap.TroopCount(FromCountry) - Amount);

        // We need to make sure we didn't drop under zero!
        if (MyMap.TroopCount(FromCountry) < 0) MyMap.TroopCount(FromCountry, 0);
    }

    public static void CalculateStrengths(int AttackerTurn, int DefenderCountry)
    {
        AttLndNum = 0;
        AttWtrNum = 0;
        DefLndNum = 0;
        DefWtrNum = 0;
        AttackStrength = 0;
        DefendStrength = MyMap!.TroopCount(DefenderCountry);
        WaterAttackStrength = 0;
        WaterDefendStrength = 0;

        for (int a = 1; a <= MyMap.NumberOfCountries; a++)
        {
            if ((MyMap.Owner(a) == AttackerTurn || MyMap.Owner(a) == MyMap.Owner(DefenderCountry)) && a != DefenderCountry)
            {
                //Let's only look at countries owned by one of the two combatants.
                for (int i = 1; i <= MyMap.MaxNeighbors; i++)
                {
                    //See if this country borders the contested country.
                    if (MyMap.Neighbors(a, i) <= 0)
                    {
                        //We've checked all neighbors of this country.
                        break;
                    }
                    else if ((MyMap.Neighbors(a, i) == DefenderCountry) && (MyMap.TroopCount(a) > 0))
                    {
                        // This country directly borders the contested country and
                        // has troops in it which can attack or defend.
                        if (MyMap.Owner(a) == AttackerTurn && MyMap.Owner(DefenderCountry) != AttackerTurn)
                        {
                            // This neighboring country belongs to the attacking player.
                            AttackStrength = AttackStrength + MyMap.TroopCount(a);
                            AttLndNum = AttLndNum + 1;
                        }
                        else if (MyMap.Owner(a) == AttackerTurn && MyMap.Owner(DefenderCountry) == AttackerTurn)
                        {
                            // This neighboring country belongs to the same person as the
                            // country clicked on.  Probably a right-click.
                            DefendStrength = DefendStrength + MyMap.TroopCount(a);
                            DefLndNum = DefLndNum + 1;
                        }
                        else if (MyMap.Owner(a) == MyMap.Owner(DefenderCountry))
                        {
                            // This neighboring country belongs to the defending player.
                            DefendStrength = DefendStrength + MyMap.TroopCount(a);
                            DefLndNum = DefLndNum + 1;
                        }
                        // Note that we exit the for loop here so that we don't count
                        // bordering countries that share a water mass twice!
                        break;
                    }
                    else if (MyMap.Neighbors(a, i) >= TILEVAL_COASTLINE)
                    {
                        //Let's see if this country can reach the contested country by water.
                        for (int j = 1; j <= MyMap.MaxNeighbors; j++)
                        {
                            if (MyMap.Neighbors(DefenderCountry, j) <= 0)
                            {
                                //We've checked all the bodies of water we can get here by.
                                break;
                            }
                            // Let's see if we can make it there.  A port is necessary!
                            else if ((MyMap.Neighbors(DefenderCountry, j) == MyMap.Neighbors(a, i)) &&
                                     ((MyMap.CountryType(a) & 1) == 1) && (MyMap.TroopCount(a) > 0))
                            {
                                // Country 'a' can reach the contested country by water!
                                if (MyMap.Owner(a) == AttackerTurn && MyMap.Owner(DefenderCountry) != AttackerTurn)
                                {
                                    //The attacking player attacks over water.
                                    WaterAttackStrength = WaterAttackStrength + MyMap.TroopCount(a);
                                    AttWtrNum = AttWtrNum + 1;
                                }
                                else if (MyMap.Owner(a) == AttackerTurn && MyMap.Owner(DefenderCountry) == AttackerTurn)
                                {
                                    //We probably right-clicked.  Add to defense total for current player.
                                    WaterDefendStrength = WaterDefendStrength + MyMap.TroopCount(a);
                                    DefWtrNum = DefWtrNum + 1;
                                }
                                else if (MyMap.Owner(a) == MyMap.Owner(DefenderCountry))
                                {
                                    //The defending player defends with boats.
                                    WaterDefendStrength = WaterDefendStrength + MyMap.TroopCount(a);
                                    DefWtrNum = DefWtrNum + 1;
                                }
                                //Exit the loops so we don't count two bodies of water.   :)
                                i = MyMap.MaxNeighbors;   //Fudging.
                                break;
                            }
                        }
                    }
                }
            }
        }

        // New for 2.0 BETA:  Add up all overseas countries and THEN multiply by ship modifier.
        AttackStrength = AttackStrength + (int)(WaterAttackStrength * ShipPct);
        DefendStrength = DefendStrength + (int)(WaterDefendStrength * ShipPct);

        // A quick fudge to fix a defunct player with countries still out there.
        // Although the player won't be able to right-click anymore, this will
        // still affect computer AI.  If there's not a threat, don't react to it!
        if (NumOccupied[AttackerTurn] == -1)
        {
            AttackStrength = 0;
        }

    }

    public static void ClaimHQ(int ThatCountry, int PlayerNumber)
    {
        // This sub marks a country as the HQ for the current player.
        AnnexCountry(ThatCountry, PlayerNumber);

        MyMap!.CountryType(ThatCountry, MyMap.CountryType(ThatCountry) | 2);
    }

    public static void Reinforce(int ThatCountry, int Turn)
    {
        int TempStore = MyMap!.TroopCount(ThatCountry);
        AddTroops(ThatCountry, CFGBonusTroops * NumOccupied[Turn]);
        NumTroops[MyMap.Owner(ThatCountry)] = NumTroops[MyMap.Owner(ThatCountry)] +
                            (MyMap.TroopCount(ThatCountry) - TempStore);
        DrawMap();
    }

    public static void MakePort(int ThatCountry)
    {
        MyMap!.CountryType(ThatCountry, MyMap.CountryType(ThatCountry) | 1);
        DrawMap();
    }

    public static bool Coastal(int ThatCountry)
    {
        for (int i = 1; i <= MyMap!.MaxNeighbors; i++)
        {
            if (MyMap.Neighbors(ThatCountry, i) >= TILEVAL_COASTLINE)
            {
                return true;
            }
        }

        return false;
    }

    public static void AttackCountry(int ThatCountry, int Turn)
    {
        // This sub assumes that the AttackStrength and DefendStrength have already
        // been calculated and that AttackStrength > DefendStrength.  The current
        // player is doing the attacking.

        // Update stats to show this country was attacked.
        StatScreen.UpdateAttackedStats(Turn, MyMap!.Owner(ThatCountry));

        // Let's kill some defenders!
        int TempNumber = MyMap.TroopCount(ThatCountry);
        KillTroops(ThatCountry, AttackStrength - DefendStrength);
        // Let's update the player totals.
        if (MyMap.TroopCount(ThatCountry) == 0)
        {
            NumTroops[MyMap.Owner(ThatCountry)] = NumTroops[MyMap.Owner(ThatCountry)] - TempNumber;
            StatScreen.UpdateKilledStats(Turn, MyMap.Owner(ThatCountry), TempNumber);  //Update stats.
        }
        else
        {
            NumTroops[MyMap.Owner(ThatCountry)] = NumTroops[MyMap.Owner(ThatCountry)] - (AttackStrength - DefendStrength);
            StatScreen.UpdateKilledStats(Turn, MyMap.Owner(ThatCountry), (AttackStrength - DefendStrength));  //Update stats.
        }

        Land.Refs.Explode(ThatCountry, MyMap.CountryColor(ThatCountry));

        if (MyMap.TroopCount(ThatCountry) == 0 && ((MyMap.CountryType(ThatCountry) & 2) == 2))
        {
            // We conquered an enemy HQ!
            // Update stats.
            int TempOwnerb = MyMap.Owner(ThatCountry);
            StatScreen.UpdateDefeatedStats(Turn, TempOwnerb);
            StatScreen.UpdateOvertakenStats(Turn, TempOwnerb);
            NumOccupied[TempOwnerb] = -1;
            NumTroops[TempOwnerb] = 0;
            // This country gets claimed like normal.
            AnnexCountry(ThatCountry, Turn);
            // But we divvy up the remaining countries according to the config setting.
            for (int a = 1; a <= MyMap.NumberOfCountries; a++)
            {
                if (MyMap.Owner(a) == TempOwnerb)
                {
                    // We need to do something with this country.
                    switch (CFGConquer)
                    {
                        case 1:
                            // The victor wins all taken countries.
                            AnnexCountry(a, Turn);
                            // Count this country as overtaken.
                            StatScreen.UpdateOvertakenStats(Turn, TempOwnerb);
                            break;
                        case 2:
                            // All this player's countries turn neutral again.
                            // Kill ports, leave troops.
                            MyMap.Owner(a, 0);
                            MyMap.CountryType(a, 0);
                            // Assign it an 'unoccupied' color.
                            MyMap.CountryColor(a, CFGUnoccupiedColor);
                            break;
                        case 3:
                            // Countries stay owned by the defunct player.
                            // Basically, we do nothing here!  The remaining players
                            // will need to attack and conquer the countries to claim them.
                            break;
                        case 4:
                            // All countries turn neutral and troops are destroyed.
                            // Same as case 2 with troops reset.
                            MyMap.Owner(a, 0);
                            MyMap.CountryType(a, 0);
                            MyMap.TroopCount(a, 0);
                            // Assign it an 'unoccupied' color.
                            MyMap.CountryColor(a, CFGUnoccupiedColor);
                            break;
                        case 5:
                            // Chaos erupts!
                            // This is determined randomly.  Therefore, in a network game the server
                            // will perform the random rolls and then all map data will be sent
                            // to each client.  This is the best way to do this, since it's possible
                            // for MANY countries to be affected.  We'll also have to update
                            // overtaken statistics on the clients as well.
                            if (MyNetworkRole != NW_CLIENT)
                            {
                                // First, kill all ports and HQs.
                                MyMap.CountryType(a, 0);
                                // Now see if we kill troops in it.  40% chance.
                                if (Random.Shared.NextDouble() < 0.4)
                                {
                                    // Kill 'em all!
                                    MyMap.TroopCount(a, 0);
                                }
                                // There is a 50% chance of going neutral, a
                                // 25% chance of victor claiming, and a
                                // 25% chance of staying loyal.
                                if (Random.Shared.NextDouble() < 0.5)
                                {
                                    // Country goes neutral.
                                    MyMap.Owner(a, 0);
                                    MyMap.CountryColor(a, CFGUnoccupiedColor);
                                }
                                else if (Random.Shared.NextDouble() < 0.5)
                                {
                                    // Victor claims it.
                                    AnnexCountry(a, Turn);
                                    // Count this country as overtaken.
                                    StatScreen.UpdateOvertakenStats(Turn, TempOwnerb);
                                }
                                // Else it stays loyal, do nothing.
                            }
                            break;
                    }
                }
            }

            // Now send all map data to the clients if we are the server and a random
            // game parameter is in effect.
            if (MyNetworkRole == NW_SERVER && CFGConquer == 5)
            {
                SendNewCountryData();
                SendUpdatedStats();
                Land.Refs.SetupIndicators();
                Land.Refs.RedrawScreen();
            }
            SNDLargeExplosion();
        }
        else if (MyMap.TroopCount(ThatCountry) == 0 && NumOccupied[MyMap.Owner(ThatCountry)] > 0)
        {
            // We just conquered a player's country, but it wasn't the last one.
            StatScreen.UpdateOvertakenStats(Turn, MyMap.Owner(ThatCountry));
            SecedeCountry(ThatCountry);
            AnnexCountry(ThatCountry, Turn);
            if (MyNetworkRole == NW_SERVER && CFGPorts == 4)
            {
                SendPortStatus(ThatCountry);
                SendUpdatedStats();
                Land.Refs.SetupIndicators();
                DrawMap();
            }
            SNDMediumExplosion();
        }
        else if (MyMap.TroopCount(ThatCountry) == 0 && NumOccupied[MyMap.Owner(ThatCountry)] == -1)
        {
            // We're cleaning up countries owned by a now-defunct player.
            StatScreen.UpdateOvertakenStats(Turn, MyMap.Owner(ThatCountry));
            AnnexCountry(ThatCountry, Turn);
            if (MyNetworkRole == NW_SERVER && CFGPorts == 4)
            {
                SendPortStatus(ThatCountry);
                SendUpdatedStats();
                Land.Refs.SetupIndicators();
                DrawMap();
            }
            SNDMediumExplosion();
        }
        else
        {
            SNDSmallExplosion();
        }

        // If we conquered countries, we may need to remove old port graphics.
        Land.Refs.RedrawScreen();
    }
}
