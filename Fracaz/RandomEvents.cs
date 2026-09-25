using static Fracaz.Boom;
using static Fracaz.Declarations;
using static Fracaz.GraphicFX;
using static Fracaz.NetworkPlay;
using static Fracaz.Play;
using static Fracaz.SoundFX;

namespace Fracaz;

internal class RandomEvents
{
    internal static void RollForRandomEvent(int Turn)
    {
        // First, we calculate rankings.
        // * The player in first place is not eligible for a random event.
        // * The player in last place has a 2% chance of getting an event.
        // Thus, the better off you are, the less likely you are to have
        // something good fall out of the sky!

        // We don't roll for a random event if we're a client.  We get that information
        // from the server.
        if (MyNetworkRole == NW_CLIENT) return;

        // No random events until after turn 5.  This allows the game to balance
        // a bit before we skew it, hehehe.
        if (TurnCounter < 6) return;

        // Get the current totals.
        StatScreen.CalculateScores();

        // Calculate the Percentages for this player.
        float MyPct = (CFGEvents - 1) / 100f;
        if (CFGEvents == 3) MyPct = MyPct * 3;  // Frequent means thrice as much!

        // See if we're in first place!
        int EventNumber = 0;
        EventInProgress = false;
        if (STATrank[Turn] == 1)
        {
            // No events for you!  You're leading!
            EventInProgress = false;
        }
        else if (STATrank[Turn] == NumPlayers)
        {
            // Twice the chance of random event.  You're in dead last!
            if (Random.Shared.NextSingle() < (MyPct * 2))
            {
                EventInProgress = true;
                EventNumber = Random.Shared.Next(1, 6);    // 1 thru 5.
            }
        }
        else
        {
            // Normal chance of random event.  You're in the middle somewhere.
            if (Random.Shared.NextSingle() < MyPct)
            {
                EventInProgress = true;
                EventNumber = Random.Shared.Next(1, 5);    // 1 thru 4.
            }
        }

        // DEBUG stuff
        //EventInProgress = True
        //EventNumber = Int(Rnd(1) * 5) + 1

        if (EventInProgress == false) return;   // Leave if we don't have one this turn.

        int RandomCountryNumber = 0;

        // Now, see which event we're creating!
        switch (EventNumber)
        {
            case 0:
                return;   // No event for you!
            case 1:  // Gift of land.
                // We pick a totally random unoccupied plot of land and give it to this player.
                // This case just finds the country and stores its ID.
                {
                    int i;
                    for (i = 0; i <= 150; i++)
                    {
                        RandomCountryNumber = Random.Shared.Next(1, MyMap!.NumberOfCountries + 1);
                        if (MyMap.Owner(RandomCountryNumber) == 0) break;
                    }
                    if (i > 150)
                    {
                        // We couldn't find empty land -- so no special bonus.
                        EventInProgress = false;
                        return;
                    }
                }
                break;
            case 2: // Coast Guard arrives.
                // We go through each country that this player owns and add a port if we can.
                // If we can add just one port, then we can do this event.
                {
                    bool Done = false;
                    for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
                    {
                        if (Done == true) break;
                        if (MyMap.Owner(i) == Turn)
                        {
                            for (int j = 1; j <= MyMap.MaxNeighbors; j++)
                            {
                                if (MyMap.Neighbors(i, j) >= TILEVAL_COASTLINE)
                                {
                                    // We found a country that borders water.
                                    if ((MyMap.CountryType(i) & 1) != 1)
                                    {
                                        // And it doesn't yet have a port!  We can do this.
                                        Done = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (Done == false)
                    {
                        // Not bordering any water, or already have ports in the right spots.
                        EventInProgress = false;
                        return;
                    }
                }
                break;
            case 3:  // 1st contact.
                // Pick a random country owned by this player and add CFGInitTroopCt times 25 to it.
                // Make sure we can find a country of theirs.
                {
                    int i;
                    for (i = 0; i <= 150; i++)
                    {
                        RandomCountryNumber = Random.Shared.Next(1, MyMap!.NumberOfCountries + 1);
                        if ((MyMap.Owner(RandomCountryNumber) == Turn) &&
                            (MyMap.TroopCount(RandomCountryNumber) + (25 * CFGInitTroopCt) < MAX_COUNTRY_CAPACITY))
                        {
                            break;
                        }
                    }
                    if (i > 150)
                    {
                        // We couldn't find a country where troops could fit -- so no special bonus.
                        EventInProgress = false;
                        return;
                    }
                }
                break;

            case 4:  // HQ bolstered.
                // Add a port plus (25 times CFGInitTroopCt) troops to this player's HQ.
                {
                    int i;
                    bool Done = false;
                    for (i = 1; i <= MyMap!.NumberOfCountries; i++)
                    {
                        if ((MyMap.Owner(i) == Turn) && ((MyMap.CountryType(i) & 2) == 2))
                        {
                            // We found it.  First, add a port if not there.
                            // Store the HQ in RandomCountryNumber so we don't have to find it again later.
                            RandomCountryNumber = i;
                            Done = false;
                            // If there's no port or room for troops, then we'll do it.  Otherwise, no.
                            if ((MyMap.CountryType(i) & 1) != 1) Done = true;
                            if (MyMap.TroopCount(i) + (25 * CFGInitTroopCt) < MAX_COUNTRY_CAPACITY) Done = true;
                            if (Done == false)
                            {
                                // Not enough space in the HQ and already had a port!
                                EventInProgress = false;
                                return;
                            }
                            break;
                        }
                    }
                }
                break;

            case 5:  // Population explosion!
                     // We add 25% troops to all of this player's countries.
                {
                    bool Done = false;
                    for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
                    {
                        if ((MyMap.Owner(i) == Turn) && (MyMap.TroopCount(i) >= 4))
                        {
                            if (MyMap.TroopCount(i) + (int)(0.25 * MyMap.TroopCount(i)) <= MAX_COUNTRY_CAPACITY)
                            {
                                // We found a country of theirs that can be added to.  We can do this event.
                                Done = true;
                                break;
                            }
                        }
                    }
                    if (Done == false)
                    {
                        // Already all maxed out!
                        EventInProgress = false;
                        return;
                    }
                }
                break;
        }
        int MessageNumber = Random.Shared.Next(1, 4);
        // At this point, we have the event number, which country it can occur on (if applicable),
        // and which random message to throw at the player.
        // So now send this event and event information to all clients!
        if (MyNetworkRole == NW_SERVER) SendRandomEvent(Turn, EventNumber, RandomCountryNumber, MessageNumber);

        // Finally, perform the event ourself.
        ActivateRandomEvent(Turn, EventNumber, RandomCountryNumber, MessageNumber);
        DrawMap();
        Land.Refs.UpdateMessages();
        Land.Refs.SetupIndicators();
        SNDBonusTwinkles();
    }

    public static void ActivateRandomEvent(int Turn, int EventNumber, int RandomCountryNumber, int MessageNumber)
    {
        // This sub actually performs the passed random event.  This could come from the function above
        // during a normal game, or from the SERVER in a networked game.

        // Now, see which event we're creating!
        switch (EventNumber)
        {
            case 0:
                return;   //No event for you!
            case 1:  //Gift of land.
                //Give the passed random country to this player.
                AnnexCountry(RandomCountryNumber, Turn);
                BonusTwinkles(RandomCountryNumber, Player[Turn]);
                //Random message.
                switch (MessageNumber)
                {
                    case 1:
                        Land.Refs.Oopsie.Text = " " + PlayerName[Turn] + " has inherited a plot of land from a rich uncle!";
                        break;
                    case 2:
                        Land.Refs.Oopsie.Text = " The natives of " + MyMap!.CountryName(RandomCountryNumber) + " have sworn allegiance to " + PlayerName[Turn] + "!";
                        break;
                    case 3:
                        Land.Refs.Oopsie.Text = " " + PlayerName[Turn] + " is granted a bonus country this turn!";
                        break;
                }
                break;
            case 2:  //Coast Guard arrives.
                //We go through each country that this player owns and add a port.
                for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
                {
                    if (MyMap.Owner(i) == Turn)
                    {
                        for (int j = 1; j <= MyMap.MaxNeighbors; j++)
                        {
                            if (MyMap.Neighbors(i, j) >= TILEVAL_COASTLINE)
                            {
                                //We found a country that borders water.
                                if ((MyMap.CountryType(i) & 1) != 1)
                                {
                                    //And it doesn't yet have a port!
                                    MyMap.CountryType(i, MyMap.CountryType(i) | 1);
                                    SmallBonusTwinkles(i, Player[Turn]);
                                }
                                break;
                            }
                        }
                    }
                }
                switch (MessageNumber)
                {
                    case 1:
                        Land.Refs.Oopsie.Text = " " + PlayerName[Turn] + " has instated a national coast guard!  All coastal countries gain ports.";
                        break;
                    case 2:
                        Land.Refs.Oopsie.Text = "Ports have been built on all available countries to protect the lands of " + PlayerName[Turn] + "!";
                        break;
                    case 3:
                        Land.Refs.Oopsie.Text = " " + PlayerName[Turn] + " has been granted naval power!  All coastal countries gain ports.";
                        break;
                }
                break;

            case 3:  //1st contact.
                //Add CFGInitTroopCt times 25 to the passed random country.
                AddTroops(RandomCountryNumber, (25 * CFGInitTroopCt));
                NumTroops[Turn] = NumTroops[Turn] + (25 * CFGInitTroopCt);
                BonusTwinkles(RandomCountryNumber, Player[Turn]);
                switch (MessageNumber)
                {
                    case 1:
                        Land.Refs.Oopsie.Text = " " + PlayerName[Turn] + " has made contact with an extraterrestrial race! " +
                                              (25 * CFGInitTroopCt).ToString() + " colonists are staying behind.";
                        break;
                    case 2:
                        Land.Refs.Oopsie.Text = " " + (25 * CFGInitTroopCt).ToString() + " alien refugees have been stranded here from another planet.  " +
                                              PlayerName[Turn] + " has put them to work!";
                        break;
                    case 3:
                        Land.Refs.Oopsie.Text = " " + PlayerName[Turn] + " has been 'visited' by an extraterrestrial race!";
                        break;
                }
                break;

            case 4:  //HQ bolstered.
                //Add a port plus (25 times CFGInitTroopCt) troops to this player's HQ.
                //We stored the HQ's ID in RandomCountryNumber before we came here, remember?
                MyMap!.CountryType(RandomCountryNumber, MyMap.CountryType(RandomCountryNumber) | 1);
                AddTroops(RandomCountryNumber, 25 * CFGInitTroopCt);
                NumTroops[Turn] = NumTroops[Turn] + (25 * CFGInitTroopCt);
                BonusTwinkles(RandomCountryNumber, Player[Turn]);
                switch (MessageNumber)
                {
                    case 1:
                        Land.Refs.Oopsie.Text = " " + PlayerName[Turn] + " has recruited peasants to bolster HQ defense.";
                        break;
                    case 2:
                        Land.Refs.Oopsie.Text = " A rich lord sympathetic to " + PlayerName[Turn] + " has offered to help defend HQ!";
                        break;
                    case 3:
                        Land.Refs.Oopsie.Text = " " + PlayerName[Turn] + " has been granted a boost to HQ!";
                        break;
                }
                break;

            case 5: // Population explosion!
                //We add 25% troops to all of this player's countries.
                Done = false;
                for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
                {
                    if ((MyMap.Owner(i) == Turn) && (MyMap.TroopCount(i) >= 4))
                    {
                        if (MyMap.TroopCount(i) + (int)(0.25 * MyMap.TroopCount(i)) <= MAX_COUNTRY_CAPACITY)
                        {
                            MyMap.TroopCount(i, MyMap.TroopCount(i) + (int)(0.25 * MyMap.TroopCount(i)));
                            SmallBonusTwinkles(i, Player[Turn]);
                        }
                    }
                }
                Land.Refs.CalculateTotals();
                switch (MessageNumber)
                {
                    case 1:
                        Land.Refs.Oopsie.Text = " " + PlayerName[Turn] + " has distributed fertility pills to the unsuspecting populace!";
                        break;
                    case 2:
                        Land.Refs.Oopsie.Text = " During other players' turns, the people of " + PlayerName[Turn] + " have been making babies!";
                        break;
                    case 3:
                        Land.Refs.Oopsie.Text = " " + PlayerName[Turn] + " has experienced an unusual growth in population.";
                        break;
                }
                break;
        }
    }

    internal static void RandomEventDoer()
    {
        // Basically, we just wait for all the little gems to bounce away.

        if (EventInProgress == false) return;

        if (NoBalls() == false) return;

        EventInProgress = false;
    }

    public static void BonusTwinkles(int CountryID, int ExpColor)
    {
        //Big bonus twinkle explosion for one country.
        BuildBalls(
            BONUS_COUNT,
            (MyMap!.DigitCoords(CountryID, 1) - 1) * 8,
            (MyMap.DigitCoords(CountryID, 2) - 1) * 8,
            BONUS_INTENSITY,
            BONUS_SPREAD,
            BONUS_ELASTIC,
            BONUS_SIZE,
            ExpColor);
    }

    public static void SmallBonusTwinkles(int CountryID, int ExpColor)
    {
        //Big bonus twinkle explosion for one country.
        BuildBalls(
            SMBONUS_COUNT,
            (MyMap!.DigitCoords(CountryID, 1) - 1) * 8,
            (MyMap.DigitCoords(CountryID, 2) - 1) * 8,
            SMBONUS_INTENSITY,
            SMBONUS_SPREAD,
            SMBONUS_ELASTIC,
            SMBONUS_SIZE,
            ExpColor);
    }

}
