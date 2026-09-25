using System.Net;
using System.Net.Sockets;
using System.Text;
using static Fracaz.ComputerAI;
using static Fracaz.Declarations;
using static Fracaz.GraphicFX;
using static Fracaz.LoadAndSave;
using static Fracaz.Play;
using static Fracaz.RandomEvents;
using static Fracaz.SoundFX;

namespace Fracaz;

internal static class NetworkPlay
{
    public static int MyNetworkRole;
    public const int NW_NONE = 0;
    public const int NW_SERVER = 1;
    public const int NW_CLIENT = 2;

    public static int NetworkState;
    public const int NS_IDLE = 0;
    public const int NS_WAITING_FOR_CONNECTIONS = 1;  //Server
    public const int NS_WAITING_FOR_JOIN_ACK = 2;     //Client
    public const int NS_WAITING_FOR_USER_COLOR = 3;   //Client
    public const int NS_WAITING_TO_START = 4;         //Client
    public const int NS_SENDING_SETUP_DATA = 5;       //Server
    public const int NS_RECEIVING_SETUP_DATA = 6;     //Client
    public const int NS_ACTIVE = 7;                   //Server & Client

    public const string NM_HEARTBEAT = "BuhBump";
    public const string NM_ACK = "Roger";
    public const string NM_IDENTIFY_YOURSELF = "WhoAreYou";
    public const string NM_MY_IDENTITY = "HelloIAm";
    public const string NM_PLAYER_LIST = "PlayerList";
    public const string NM_I_CHOOSE_COLOR = "IWillBe";
    public const string NM_I_AM_BUILDING_THE_MAP = "ReticulatingSplines";
    public const string NM_YOU_ARE_GOOD_TO_GO = "HangOn";
    public const string NM_I_AM_QUITTING = "SeeYa";
    public const string NM_SETUP_DATA_1 = "OptionsAndStuff";
    public const string NM_SETUP_DATA_1_RCV = "GotFirstOne";
    public const string NM_SETUP_DATA_2 = "CountryData";
    public const string NM_SETUP_DATA_2_RCV = "GotCountryData";
    public const string NM_SETUP_DATA_3 = "LandNameData";
    public const string NM_SETUP_DATA_3_RCV = "GotLandNameData";
    public const string NM_SETUP_DATA_4 = "WaterNameData";
    public const string NM_SETUP_DATA_4_RCV = "GotWaterNameData";
    public const string NM_SETUP_DATA_MAP = "MapLine";
    public const string NM_SETUP_DATA_MAP_RCV = "GotMapLine";
    public const string NM_SETUP_COMPLETE = "StickAForkInMe";
    public const string NM_PLAYER_MOVE = "WeAreMoving";
    public const string NM_PLAYER_PASS = "NoThankYa";
    public const string NM_PLAYER_RESIGN = "Uncle";
    public const string NM_RENAME = "LetsCallThis";
    public const string NM_NEW_COUNTRY_DATA = "LotsOfStuffChanged";
    public const string NM_NEW_STATS = "StatUpdate";
    public const string NM_PORT_STATS = "NewPort";
    public const string NM_RANDOM_EVENT = "HangOnToYourHats";
    public const string NM_RETRY = "GimmeAnotherChance"; // Never sent to a partner.
    public const string NM_CHAT = "Pssst";

    // NetArray contains the player number of each networked player.
    // The index is the control index for that player.
    // Used by servers only.
    public static int[] NetArray = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int[] DataArray = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
    public const int NA_PICKING_COLOR = 99;
    public static bool NetworkArbitrationInProgress;   //Used to block any sort of message sending activity.
    public static int MyClientIndex;  //Clients.
    public const int MAX_SEQ_NUM = 99999;
    public const int MSG_Q_SIZE = 15;  //Max messages that can be queued.
    public const int MSG_TIMEOUT = 250;  //Number of iterations of HeartBeat to wait for an ACK.
    public static bool TurnInProgress;  //Used to prevent multiple turns over LAN.
    public static string[,] MsgQ = new string[MAX_PLAYERS + ONE_ARRAY_FIX, MSG_Q_SIZE + ONE_ARRAY_FIX];
    public static string[] StringQ = new string[MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int[] Qtimer = new int[MAX_PLAYERS + ONE_ARRAY_FIX];
    public static int SeqNumber;   //Used to give each message a unique ID.

    public static void ListenForClients()
    {
        // We're the server.
        // The server is ready to start accepting connection requests.

        if (NetworkForm.FracasSockListener != null)
        {
            NetworkForm.FracasSockListener.Stop();
            InitNetwork();
        }

        NetworkForm.FracasSockListener = new TcpListener(IPAddress.Any, 3737);
        NetworkForm.FracasSockListener.Start();

        NetworkForm.FracasSock.Add(null!); // Server's own control is always index 0 and is never used.
        NetworkForm.FracasSockListener.BeginAcceptTcpClient(new AsyncCallback(FracasSock_ConnectionRequest), NetworkForm.FracasSockListener);
    }

    private static void FracasSock_ConnectionRequest(IAsyncResult ar)
    {
        var listener = (TcpListener)ar.AsyncState!;

        // Gets client and starts processing received request.
        //using (TcpClient client = listener.EndAcceptTcpClient(ar))
        //{
        TcpClient client;
        try
        {
            client = listener.EndAcceptTcpClient(ar);
        }
        catch (SocketException)
        {
            return;
            // Handle socket exception if needed
        }
        catch (ObjectDisposedException)
        {
            // Listener was stopped.
            return;
        }

        var c = client.Connected.ToString();
        // Just pass the request through to the network module.
        ConnectRequest(client);

        var index = NetworkForm.FracasSock.Count - 1;
        // Start the async data arrival loop
        _ = FracasSock_DataArrivalAsync(index);

        //}

        listener.BeginAcceptTcpClient(new AsyncCallback(FracasSock_ConnectionRequest), listener);
    }

    public static bool ConnectToHost(string HostName)
    {
        var connectToHost = false;
        // We're a client.
        // Use control 0 to talk to the server and establish a connection.
        if (NetworkForm.FracasSock.Count == 0)
        {
            NetworkForm.FracasSock.Add(null!);
        }

        if (NetworkForm.FracasSock[0] != null && NetworkForm.FracasSock[0].Connected)
        {
            InitNetwork();
            NetworkForm.FracasSock[0].Close();
            Application.DoEvents();
        }

        NetworkForm.FracasSock[0] = new TcpClient();

        IAsyncResult result = NetworkForm.FracasSock[0].BeginConnect(HostName, 3737, null, null);

        while (true)
        {
            Application.DoEvents();

            if (result.AsyncWaitHandle.WaitOne(100))
            {
                try
                {
                    NetworkForm.FracasSock[0].EndConnect(result);
                    _ = FracasSock_DataArrivalAsync(0);

                    connectToHost = true;
                }
                catch
                {
                    connectToHost = false;
                }
                break;
            }
        }

        return connectToHost;
    }

    public static void ConnectRequest(TcpClient client)
    {

        // The server got a connection request from a client.  If we haven't
        // yet heard from everyone, then we're still at the 'pick colors'
        // dialog.  If we have heard from everyone already, then someone's
        // trying to connect to a game in progress!  Reject it.

        int NewConnectionNum = 0;
        if (NetworkState == NS_WAITING_FOR_CONNECTIONS)
        {
            // Server got a new connection.
            // Find the first available slot in our netarray.
            for (int i = 1; i <= MAX_PLAYERS; i++)
            {
                if (NetArray[i] == 0)
                {
                    NewConnectionNum = i;
                    break;
                }
            }
            if (NewConnectionNum > 0)
            {
                // We found a slot for this player to join.  Mark them as
                // Configuring their color.
                NetArray[NewConnectionNum] = NA_PICKING_COLOR;
                NetworkForm.FracasSock.Add(client);

                // Send this client the standard greeting. This is:
                // * Which Player Number we want him to be.
                // * The current list of players as it stands.
                // We will get back a response from this player indicating
                // that they agree.  At that point, we are bound to the client.
                SendMsg(NewConnectionNum, NM_IDENTIFY_YOURSELF + "," + NewConnectionNum);
                SendMsg(NewConnectionNum, NM_PLAYER_LIST + "," + BuildPlayerList());
            }
        }
    }

    public static async Task FracasSock_DataArrivalAsync(int index)
    {
        var client = NetworkForm.FracasSock[index];
        if (client == null || !client.Connected)
            return;

        var stream = client.GetStream();
        var buffer = new byte[4096];

        while (client.Connected)
        {
            int bytesRead = 0;
            try
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            }
            catch
            {
                // Handle disconnects or errors
                break;
            }

            if (bytesRead == 0)
                break; // Client disconnected

            string netString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            if (!string.IsNullOrEmpty(netString))
            {
                int pnum = (MyNetworkRole == NW_CLIENT) ? 1 : index;
                ConcatenateStr(pnum, netString);
            }
        }
    }

    private static string QuitRequest(int NetIndex)
    {

        var result = string.Empty; // Do nothing unless we decide otherwise.

        if (MyNetworkRole == NW_SERVER)
        {
            // A client just WUSSED out on us.  If we're still picking colors,
            // Set this player back to available.  If we're in the middle
            // of a game, let a computer take over.  If it's the end of a game,
            // It doesn't matter what we do here.
            if (NetArray[NetIndex] > 0)
            {
                // We got the message from someone with an active connection.
                // Close out their control.
                KillWinsockControl(NetIndex);
                // See if they're in a game.
                if (GameMode == GM_GAME_ACTIVE)
                {
                    // We're in the middle of a game.  This guy's a quitter!
                    PlayerType[NetArray[NetIndex]] = PTYPE_COMPUTER;
                    Personality[NetArray[NetIndex]] = 1;  // Stonewall for now.
                    // Now clear their index.
                    NetArray[NetIndex] = 0;
                }
                else
                {
                    // We're picking colors.  Update our screen.
                    // Clear their index first.
                    NetArray[NetIndex] = 0;
                    NetworkForm.SetUpColorList();
                    // Tell everyone else that this player is a WUSS.
                    BuildAndSendPlayerList();
                }
                DataArray[NetIndex] = 0;
                for (int i = 1; i <= MSG_Q_SIZE; i++)
                {
                    MsgQ[NetIndex, i] = string.Empty;
                }
            }
        }
        else
        {
            // The server is leaving us.  We have no choice but to go away.
            if ((GameMode == GM_GAME_ACTIVE) ||
               (NetworkState == NS_WAITING_FOR_USER_COLOR) || (NetworkState == NS_WAITING_TO_START))
            {
                MessageBox.Show("The server has left the network.  Fracas cannot continue as a client.", "No Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = NM_I_AM_QUITTING;
            }
            else
            {
                // The server is probably leaving during the tribute dialog.  The game is already over.
            }
        }

        return result;
    }

    private static string ParseIncomingMessage(int NetIndex, string NetString)
    {
        int TempNum1;
        int TempInt2;
        int TempTurn;
        int TempPhase;
        int TempInt;

        // This sub takes the passed message from the passed player and acts on it.
        var result = string.Empty;

        // If the CRC check fails, we do NOTHING.
        int myCrc = Convert.ToInt32(ArgAt(NetString, 1));
        NetString = StripOffFirstAt(NetString);
        if (myCrc != CalcCRC(NetString))
            return result;  // Bad CRC.

        // Now, see if this is an ACK.
        if (Arg(NetString, 1) == NM_ACK)
        {
            // Yup, so let's get this msg off of our queue.
            int mySeqNum = Convert.ToInt32(Arg(NetString, 2));
            AckMsg(NetIndex, mySeqNum);
        }
        else if (Arg(NetString, 1) == NM_I_AM_QUITTING)
        {
            // We just got a quit request.
            result = QuitRequest(NetIndex);
        }
        else if (ArgAt(NetString, 1) == NM_CHAT)
        {
            // It's a chat message.  Decode it and move on quick.
            // We just got a line of chat.
            int tempInt = Convert.ToInt32(ArgAt(NetString, 2));
            string tempStr = ArgAt(NetString, 3);
            // Propagate it if we're the server!
            if (MyNetworkRole == NW_SERVER)
                SendChatToNetwork(tempInt, tempStr, NetIndex);
            ChatForm.ChatTextReceived(tempInt, tempStr);
        }
        else
        {
            // This isn't an ACK message, it's a real one!
            int mySeqNum = Convert.ToInt32(ArgAt(NetString, 1));
            NetString = StripOffFirstAt(NetString);
            // --------------------------------------SERVER----------------------------------------
            if (MyNetworkRole == NW_SERVER)
            {
                // We're the server and we just got a message from a client.
                switch (Arg(NetString, 1))
                {
                    case NM_HEARTBEAT:
                        // Just a heartbeat.  Ack it.
                        Acknowledge(NetIndex, mySeqNum);
                        break;
                    case NM_I_CHOOSE_COLOR:
                        // A player has chosen a color.  Check that it's valid, and
                        // then tell them to wait.
                        TempInt = Convert.ToInt32(Arg(NetString, 2));
                        if (NetArray[NetIndex] == NA_PICKING_COLOR)
                        {
                            // This player is now this color.
                            NetArray[NetIndex] = TempInt;
                            PlayerName[TempInt] = Arg(NetString, 3);
                            // Tell this player that we accept.
                            Acknowledge(NetIndex, mySeqNum);
                            SendMsg(NetIndex, NM_YOU_ARE_GOOD_TO_GO + "," + TempInt);
                            // Update everyone else of this fact.
                            BuildAndSendPlayerList();
                            // Update our own screen.
                            NetworkForm.SetUpColorList();
                            // Check to see if we have everyone.  If so, start the game!

                            TempInt = 0;
                            for (int i = 1; i <= MAX_PLAYERS; i++)
                            {
                                if (PlayerType[i] == PTYPE_NETWORK &&
                                    ((MyNetIndex(i) == 0) || (MyNetIndex(i) == NA_PICKING_COLOR)))
                                {
                                    // Found a network player that hasn't joined yet or is in
                                    // the process of picking their color.
                                    TempInt = 1;
                                    break;
                                }
                            }

                            if (TempInt == 0)
                            {
                                // We have everyone.
                                NetworkForm.Refs.StatusText.Text = "Sending game data.  Please wait.";
                                // We don't want people to cancel while sending data.  That's a misuse
                                // case that could get really, really messy.
                                NetworkForm.Refs.CancelBut.Visible = false;
                                // Send all players a heads-up that we're building the map now.
                                for (int i = 1; i <= MAX_PLAYERS; i++)
                                {
                                    if (NetArray[i] > 0)
                                    {
                                        SendMsg(i, NM_I_AM_BUILDING_THE_MAP);
                                    }
                                }
                                result = NM_YOU_ARE_GOOD_TO_GO;   // Return code to start the game.
                            }

                        }
                        else
                        {
                            // Tell the client that this isn't acceptable.  On the other
                            // hand, we might just do nothing here, since the client's list
                            // will be updated soon.  Basically, two players probably
                            // tried to pick the same color at the same time.
                            Acknowledge(NetIndex, mySeqNum);
                        }

                        break;
                    case NM_SETUP_DATA_1_RCV:
                        // This client got the first round of setup data.
                        Acknowledge(NetIndex, mySeqNum);
                        SendMsg(NetIndex, NM_SETUP_DATA_2 + "," + BuildCountryData());
                        break;
                    case NM_SETUP_DATA_2_RCV:
                        // This client got the second round of setup data.
                        Acknowledge(NetIndex, mySeqNum);
                        SendMsg(NetIndex, NM_SETUP_DATA_3 + "," + BuildLandNameData());
                        break;
                    case NM_SETUP_DATA_3_RCV:
                        // This client got the third round of setup data.
                        Acknowledge(NetIndex, mySeqNum);
                        SendMsg(NetIndex, NM_SETUP_DATA_4 + "," + BuildWaterNameData());
                        break;
                    case NM_SETUP_DATA_4_RCV:
                        // This client got their final data packet.  Now send them the first line
                        // of map info.  We will only send a line of map data when the previous
                        // line was confirmed.  If we don't confirm a line, we'll rely on our
                        // heartbeat timer to detect loss of comms and resend.
                        Acknowledge(NetIndex, mySeqNum);
                        SendMsg(NetIndex, BuildMapLine(1));
                        break;
                    case NM_SETUP_DATA_MAP_RCV:
                        // This client just told us they received the last line of map we sent.
                        Acknowledge(NetIndex, mySeqNum);
                        TempInt = Convert.ToInt32(Arg(NetString, 2));
                        // Increment our sent counter.  When everyone's sent counter is equal
                        // to the Y size of the map, we can start!
                        DataArray[NetIndex] = TempInt;
                        // Update our progress bar.
                        NetworkForm.UpdateProgressBar(0);
                        // Go to the next line.
                        TempInt = TempInt + 1;
                        if (TempInt > MyMap!.Ysize)
                        {
                            // We're done with this guy!  See if anyone else is still waiting on data.
                            SendMsg(NetIndex, NM_SETUP_COMPLETE);
                            TempNum1 = 0;
                            for (int i = 1; i <= MAX_PLAYERS; i++)
                            {
                                if ((NetArray[i] > 0) && (DataArray[i] < MyMap.Ysize))
                                {
                                    TempNum1 = 1;
                                    break;
                                }
                            }
                            if (TempNum1 == 0)
                            {
                                // Everyone has all of their map data!  Time to kick off the game.
                                NetworkState = NS_ACTIVE;
                                result = NM_SETUP_COMPLETE;   // Return code to start the game.
                            }
                        }
                        else
                        {
                            // Send out the next line.
                            SendMsg(NetIndex, BuildMapLine(TempInt));
                        }
                        break;
                    case NM_SETUP_COMPLETE:
                        // This client has confirmed that we gave them everything they need.
                        Acknowledge(NetIndex, mySeqNum);
                        SendMsg(NetIndex, NM_HEARTBEAT);
                        break;
                    case NM_PLAYER_MOVE:
                        // We just got a move request from a client.
                        result = NM_RETRY; // We retry by default unless we succeed.
                        if (TurnInProgress) return result;


                        TempTurn = Convert.ToInt32(Arg(NetString, 2));
                        TempPhase = Convert.ToInt32(Arg(NetString, 3));
                        TempNum1 = Convert.ToInt32(Arg(NetString, 4)); // Country num.
                        // Leave if it isn't this player's turn (probably a retried msg)
                        if (Land.Refs.GetTurn() != TempTurn) return result;
                        // Now just pretend that this click really happened.
                        if (NumOccupied[TempTurn] == 0)
                        {
                            // This player is choosing their country.
                            Acknowledge(NetIndex, mySeqNum);
                            SendClickToNetwork(TempTurn, TempPhase, TempNum1, NetIndex);
                            ChooseHQProc(TempNum1, TempTurn);
                            NextTurn(TempTurn, TempPhase);
                            result = string.Empty;
                            return result;
                        }

                        switch (TempPhase)
                        {
                            case 1:
                                // We got a reinforce request.  Make sure we're in the reinforce phase.
                                if (Land.Refs.GetPhase() != TempPhase)
                                    return result;

                                TurnInProgress = true;

                                Acknowledge(NetIndex, mySeqNum);

                                // Now do the reinforce.
                                SendClickToNetwork(TempTurn, TempPhase, TempNum1, NetIndex);
                                ReinforceProc(TempNum1, TempTurn);
                                NextPhase(TempTurn, TempPhase);
                                break;
                            case 2:
                                // This is an action request.  Make sure we're in the action phase.
                                if (Land.Refs.GetPhase() != TempPhase)
                                    return result;
                                TurnInProgress = true;
                                Acknowledge(NetIndex, mySeqNum);
                                // Now do the action.
                                SendClickToNetwork(TempTurn, TempPhase, TempNum1, NetIndex);
                                ActionProc(TempNum1, TempTurn);
                                NextPhase(TempTurn, TempPhase);
                                break;
                            case 3:
                                // We should *never* get here.  No netmsgs sent during phase 3.
                                MessageBox.Show("An error occurred during troop movement source select." + Environment.NewLine +
                                       "Please report this bug!");
                                break;
                            case 4:

                                // This is a troop movement request.  Make sure we're in the right phase.
                                if (Land.Refs.GetPhase() != 3)
                                    return result;
                                TurnInProgress = true;
                                Acknowledge(NetIndex, mySeqNum);
                                // Now do the troop movement.
                                int TempNum3 = Convert.ToInt32(TempNum1 / 1000000);
                                int TempNum4 = TempNum1 - (TempNum3 * 1000000);
                                int TempNum2 = Convert.ToInt32(TempNum4 / 1000);
                                TempNum4 = TempNum4 - (TempNum2 * 1000);
                                SendClickToNetwork(TempTurn, TempPhase, TempNum1, NetIndex);
                                TroopMoveProc(TempNum2, TempNum4, TempNum3, TempTurn);
                                NextTurn(TempTurn, TempPhase);
                                break;
                        }
                        result = string.Empty;
                        TurnInProgress = false;
                        break;
                    case NM_PLAYER_PASS:
                        // We just got a pass request from a client.
                        result = NM_RETRY; // We retry by default unless we succeed.
                        if (TurnInProgress) return result;
                        TempTurn = Convert.ToInt32(Arg(NetString, 2));
                        TempPhase = Convert.ToInt32(Arg(NetString, 3));
                        // Leave if it isn't this player's turn (probably a retried msg)
                        if (Land.Refs.GetTurn() != TempTurn) return result;
                        // Now do the pass.
                        if (TempPhase < 3)
                        {
                            if (Land.Refs.GetPhase() != TempPhase)
                                return result;
                            TurnInProgress = true;
                            Acknowledge(NetIndex, mySeqNum);
                            SendPassToNetwork(TempTurn, TempPhase, NetIndex);
                            SNDWhistle();
                            NextPhase(TempTurn, TempPhase);
                        }
                        else
                        {
                            // If we're somewhere in troop movement, just skip it all.
                            if (Land.Refs.GetPhase() != 3)
                                return result;
                            TurnInProgress = true;
                            Acknowledge(NetIndex, mySeqNum);
                            SendPassToNetwork(TempTurn, TempPhase, NetIndex);
                            SNDWhistle();
                            NextTurn(TempTurn, TempPhase);
                        }

                        result = string.Empty;
                        TurnInProgress = false;
                        break;
                    case NM_PLAYER_RESIGN:
                        // We just got a resign request from a client.  That weenie!
                        if (TurnInProgress) return result;
                        TempTurn = Convert.ToInt32(Arg(NetString, 2));
                        TempPhase = Convert.ToInt32(Arg(NetString, 3));
                        // Leave if it isn't this player's turn (probably a retried msg)
                        if (Land.Refs.GetTurn() != TempTurn) return result;
                        if (Land.Refs.GetPhase() != TempPhase) return result;
                        // Now do the resign.
                        TurnInProgress = true;
                        Acknowledge(NetIndex, mySeqNum);
                        SendResignToNetwork(TempTurn, TempPhase, NetIndex);
                        Land.Refs.ResignProc(TempTurn);
                        NextTurn(TempTurn, TempPhase);
                        TurnInProgress = false;
                        break;
                    case NM_RENAME:
                        // We just got a request to rename an entity, land or water.
                        // This one is easy, just grab the ID and name and make it so.
                        Acknowledge(NetIndex, mySeqNum);
                        TempNum1 = Convert.ToInt32(Arg(NetString, 2));   // Entity ID.
                        string TempStr = Arg(NetString, 3);    // New name for this entity.
                        if (TempNum1 < TILEVAL_COASTLINE)
                        {
                            // We're renaming land.
                            MyMap!.CountryName(TempNum1, TempStr);
                        }
                        else
                        {
                            // We're renaming water.
                            MyMap!.WaterName(TempNum1 - 1000, TempStr);
                        }
                        SendRenameToNetwork(TempNum1, TempStr, NetIndex);
                        break;
                    default:
                        break;
                }
            }
            else if (MyNetworkRole == NW_CLIENT)
            {
                // We're a client and we just got a message from the server.
                switch (Arg(NetString, 1))
                {
                    case NM_HEARTBEAT:
                        // Just a heartbeat.  Ack it.
                        Acknowledge(NetIndex, mySeqNum);
                        break;
                    case NM_IDENTIFY_YOURSELF:
                        // Identify myself. This will just put is into the
                        // proper state for picking colors.
                        NetworkState = NS_WAITING_FOR_USER_COLOR;
                        Acknowledge(NetIndex, mySeqNum);
                        break;
                    case NM_PLAYER_LIST:
                        // Update the player list on our dialog.
                        for (int i = 1; i <= 6; i++)
                        {
                            PlayerName[i] = Arg(NetString, ((i - 1) * 3) + 2);
                            Player[i] = Convert.ToInt32(Arg(NetString, ((i - 1) * 3) + 3));
                            PlayerType[i] = Convert.ToInt32(Arg(NetString, ((i - 1) * 3) + 4));
                        }
                        NetworkForm.SetUpColorList();
                        Acknowledge(NetIndex, mySeqNum);
                        break;
                    case NM_YOU_ARE_GOOD_TO_GO:
                        // The server has accepted our request to be this player.
                        // When we get the go signal, MyIndex will become our
                        // player number.
                        Acknowledge(NetIndex, mySeqNum);
                        MyClientIndex = Convert.ToInt32(Arg(NetString, 2));
                        SNDPlayFanfare(Player[MyClientIndex]);
                        NetworkState = NS_WAITING_TO_START;
                        NetworkForm.Refs.lblName.Visible = false;
                        NetworkForm.Refs.MyPlayerName.Visible = false;
                        NetworkForm.Refs.StatusText.Text = "Please wait for others to join.";
                        break;
                    case NM_I_AM_BUILDING_THE_MAP:
                        // The server has everyone joined and is now building the map.
                        // We'll just put up a message to that effect and disallow cancelling.
                        Acknowledge(NetIndex, mySeqNum);
                        NetworkForm.Refs.CancelBut.Visible = false;
                        NetworkForm.Refs.StatusText.Text = "The server is building the map. \nPlease wait...";
                        break;
                    case NM_SETUP_DATA_1:
                        // We just got our first setup packet.
                        if (NetworkState == NS_WAITING_TO_START)
                        {
                            NetworkForm.Refs.StatusText.Text = "Receiving data.  Please wait.";
                            NetworkState = NS_RECEIVING_SETUP_DATA;
                            CFGCountrySize = Convert.ToInt32(Arg(NetString, 2));
                            CFGLandPct = Convert.ToInt32(Arg(NetString, 3));
                            CFGIslands = Convert.ToInt32(Arg(NetString, 4));
                            CFGLakeSize = Convert.ToInt32(Arg(NetString, 5));
                            CFGProportion = Convert.ToInt32(Arg(NetString, 6));
                            CFGShape = Convert.ToInt32(Arg(NetString, 7));
                            CFGBorders = Convert.ToInt32(Arg(NetString, 8));
                            CFGInitTroopPl = Convert.ToInt32(Arg(NetString, 9));
                            CFGInitTroopCt = Convert.ToInt32(Arg(NetString, 10));
                            CFGBonusTroops = Convert.ToInt32(Arg(NetString, 11));
                            CFGShips = Convert.ToInt32(Arg(NetString, 12));
                            CFGPorts = Convert.ToInt32(Arg(NetString, 13));
                            CFGConquer = Convert.ToInt32(Arg(NetString, 14));
                            CFGAISpeed = Convert.ToInt32(Arg(NetString, 15));
                            CFG1st = Convert.ToInt32(Arg(NetString, 16));
                            CFGHQSelect = Convert.ToInt32(Arg(NetString, 17));
                            CFGUnoccupiedColor = Convert.ToInt32(Arg(NetString, 18));
                            CFGResolution = Convert.ToInt32(Arg(NetString, 19));
                            CFGConquer = Convert.ToInt32(Arg(NetString, 20));
                            CFGCountrySize = Convert.ToInt32(Arg(NetString, 21));
                            TempNum1 = Convert.ToInt32(Arg(NetString, 22));   // Number of countries.
                            int TempNum2 = Convert.ToInt32(Arg(NetString, 23));   // LakeCode (num of water masses).
                            Land.Refs.SetTurn(Convert.ToInt32(Arg(NetString, 24)));
                            // Put our progress bar up...
                            NetworkForm.Refs.NetProg.Value = 0;
                            NetworkForm.Refs.NetProg.Visible = true;
                            // Update our menus to reflect the saved config settings.
                            ClearMenuChecks();
                            UpdateMenus();
                            // Now that we have a new resolution, see if it's valid.
                            if (CFGResolution > MaxAllowedResolution)
                            {
                                // Can't display a map at that resolution.  Exit gracefully.
                                MessageBox.Show("The game you are joining is using a map too big for your desktop.", "Map Too Large", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                // Send back a code that says we can't go on.  Also signal
                                // the server that we're leaving.
                                SendMsg(0, NM_I_AM_QUITTING);
                                result = NM_I_AM_QUITTING;
                                return result;
                            }
                            else
                            {
                                // Resolution is valid, set up map.
                                Land.Refs.ChangeRes();
                                MyMap!.RedimensionStuff(TempNum1, TempNum2);
                            }
                            // Signal the server that we got this much...
                            Acknowledge(NetIndex, mySeqNum);
                            SendMsg(0, NM_SETUP_DATA_1_RCV);
                        }
                        break;
                    case NM_SETUP_DATA_2:
                        if (NetworkState == NS_RECEIVING_SETUP_DATA)
                        {
                            Acknowledge(NetIndex, mySeqNum);
                            // Parse country data.
                            for (int j = 1; j <= MyMap!.NumberOfCountries; j++)
                            {
                                string tempStr = Arg(NetString, j + 1);
                                MyMap.CountryColor(j, Convert.ToInt32(ArgAt(tempStr, 1)));
                                MyMap.CountryType(j, Convert.ToInt32(ArgAt(tempStr, 2)));
                                MyMap.Owner(j, Convert.ToInt32(ArgAt(tempStr, 3)));
                                MyMap.TroopCount(j, Convert.ToInt32(ArgAt(tempStr, 4)));
                            }
                            SendMsg(0, NM_SETUP_DATA_2_RCV);
                        }
                        break;
                    case NM_SETUP_DATA_3:
                        if (NetworkState == NS_RECEIVING_SETUP_DATA)
                        {
                            Acknowledge(NetIndex, mySeqNum);
                            // Parse land names.
                            for (int j = 1; j <= MyMap!.NumberOfCountries; j++)
                            {
                                MyMap.CountryName(j, Arg(NetString, j + 1));
                            }
                            SendMsg(0, NM_SETUP_DATA_3_RCV);
                        }
                        break;
                    case NM_SETUP_DATA_4:
                        if (NetworkState == NS_RECEIVING_SETUP_DATA)
                        {
                            Acknowledge(NetIndex, mySeqNum);
                            // Parse water names.
                            for (int j = 1001; j <= MyMap!.LakeCode; j++)
                            {
                                MyMap.WaterName(j - 1000, Arg(NetString, (j - 1000) + 1));
                            }
                            SendMsg(0, NM_SETUP_DATA_4_RCV);
                        }
                        break;
                    case NM_SETUP_DATA_MAP:
                        if (NetworkState == NS_RECEIVING_SETUP_DATA)
                        {
                            Acknowledge(NetIndex, mySeqNum);
                            // The server just sent us a line of the map.  Put it in our array and
                            // ask for the next.
                            TempInt = Convert.ToInt32(Arg(NetString, 2));   // This is the line number.
                            for (int i = 1; i <= MyMap!.Xsize; i++)
                            {
                                MyMap.Grid(i, TempInt, Convert.ToInt32(Arg(NetString, i + 2)));
                            }
                            // Update our progress bar.
                            NetworkForm.UpdateProgressBar(TempInt);
                            // Now tell the server we got this line.
                            SendMsg(0, NM_SETUP_DATA_MAP_RCV + "," + TempInt);
                        }
                        break;
                    case NM_SETUP_COMPLETE:
                        if (NetworkState == NS_RECEIVING_SETUP_DATA)
                        {
                            Acknowledge(NetIndex, mySeqNum);
                            // The server says we're finished!
                            SendMsg(0, NM_SETUP_COMPLETE);
                            // Some map parameters need to be calculated.
                            MyMap!.FinishUpLoad();
                            // Recalculate ShipPct.  Fudge.
                            Land.Refs.CollectMenuSettings(true);
                            result = NM_SETUP_COMPLETE;   // Return code to start the game.
                            NetworkState = NS_ACTIVE;
                        }
                        break;
                    case NM_PLAYER_MOVE:
                        // We just got a move request from the server.
                        result = NM_RETRY;  // We retry by default unless we succeed.
                        if (TurnInProgress) return result;
                        TempTurn = Convert.ToInt32(Arg(NetString, 2));
                        TempPhase = Convert.ToInt32(Arg(NetString, 3));
                        TempNum1 = Convert.ToInt32(Arg(NetString, 4));  // Country num.
                        // Special for clients:  Ack this message if it's the server
                        // telling US where WE should move.  Obviously came from the
                        // SendClickToNetwork function...                      
                        if (TempTurn == MyClientIndex)
                        {
                            Acknowledge(NetIndex, mySeqNum);
                            result = string.Empty;
                            return result;
                        }
                        // Leave if it isn't this player's turn (probably a retried msg)
                        if (Land.Refs.GetTurn() != TempTurn) return result;
                        // Now just pretend that this click really happened.
                        if (NumOccupied[TempTurn] == 0)
                        {
                            // This player is choosing their country.
                            Acknowledge(NetIndex, mySeqNum);
                            ChooseHQProc(TempNum1, TempTurn);
                            NextTurn(TempTurn, TempPhase);
                            result = string.Empty;
                            return result;
                        }
                        switch (TempPhase)
                        {
                            case 1:
                                // We got a reinforce request.  Make sure we're in the reinforce phase.
                                if (Land.Refs.GetPhase() != TempPhase) return result;
                                TurnInProgress = true;
                                Acknowledge(NetIndex, mySeqNum);
                                // Now do the reinforce.
                                ReinforceProc(TempNum1, TempTurn);
                                NextPhase(TempTurn, TempPhase);
                                break;
                            case 2:
                                // This is an action request.  Make sure we're in the action phase.
                                if (Land.Refs.GetPhase() != TempPhase) return result;
                                TurnInProgress = true;
                                Acknowledge(NetIndex, mySeqNum);
                                // Now do the action.
                                ActionProc(TempNum1, TempTurn);
                                NextPhase(TempTurn, TempPhase);
                                break;
                            case 3:
                                // We should *never* get here.  No netmsgs set during phase 3.
                                MessageBox.Show("An error occurred during troop movement source select." + Environment.NewLine +
                                       "Please report this bug!");
                                break;
                            case 4:
                                // This is a troop movement request.  Make sure we're in the right phase.
                                if (Land.Refs.GetPhase() != 3) return result;
                                TurnInProgress = true;
                                Acknowledge(NetIndex, mySeqNum);
                                // Now do the troop movement.
                                int TempNum3 = Convert.ToInt32(TempNum1 / 1000000);
                                int TempNum4 = TempNum1 - (TempNum3 * 1000000);
                                int TempNum2 = Convert.ToInt32(TempNum4 / 1000);
                                TempNum4 = TempNum4 - (TempNum2 * 1000);
                                TroopMoveProc(TempNum2, TempNum4, TempNum3, TempTurn);
                                NextTurn(TempTurn, TempPhase);
                                break;
                        }
                        result = string.Empty;
                        TurnInProgress = false;
                        break;
                    case NM_PLAYER_PASS:
                        // We just got a pass request from the server.
                        result = NM_RETRY;  // We retry by default unless we succeed.
                        if (TurnInProgress) return result;
                        TempTurn = Convert.ToInt32(Arg(NetString, 2));
                        TempPhase = Convert.ToInt32(Arg(NetString, 3));
                        // Special for clients:  Ack this message if it's the server
                        // telling US that WE should pass.  Obviously came from the
                        // SendPassToNetwork function...
                        if (TempTurn == MyClientIndex)
                        {
                            Acknowledge(NetIndex, mySeqNum);
                            result = string.Empty;
                            return result;
                        }
                        // Leave if it isn't this player's turn (probably a retried msg)
                        if (Land.Refs.GetTurn() != TempTurn) return result;
                        // Now do the pass.
                        if (TempPhase < 3)
                        {
                            if (Land.Refs.GetPhase() != TempPhase) return result;
                            TurnInProgress = true;
                            Acknowledge(NetIndex, mySeqNum);
                            SNDWhistle();
                            NextPhase(TempTurn, TempPhase);
                        }
                        else
                        {
                            // If we're somewhere in troop movement, just skip it all.
                            if (Land.Refs.GetPhase() != 3) return result;
                            TurnInProgress = true;
                            Acknowledge(NetIndex, mySeqNum);
                            SNDWhistle();
                            NextTurn(TempTurn, TempPhase);
                        }
                        result = string.Empty;
                        TurnInProgress = false;
                        break;
                    case NM_PLAYER_RESIGN:
                        // We just got a resign request from the server.
                        if (TurnInProgress) return result;
                        TempTurn = Convert.ToInt32(Arg(NetString, 2));
                        TempPhase = Convert.ToInt32(Arg(NetString, 3));
                        // Special for clients:  Ack this message if it's the server
                        // telling US that WE should resign.  Obviously came from the
                        // SendPassToNetwork function...
                        if (TempTurn == MyClientIndex)
                        {
                            Acknowledge(NetIndex, mySeqNum);
                            return result;
                        }
                        // Leave if it isn't this player's turn (probably a retried msg)
                        if (Land.Refs.GetTurn() != TempTurn) return result;
                        if (Land.Refs.GetPhase() != TempPhase) return result;
                        // Now do the resign.
                        TurnInProgress = true;
                        Acknowledge(NetIndex, mySeqNum);
                        Land.Refs.ResignProc(TempTurn);
                        NextTurn(TempTurn, TempPhase);
                        TurnInProgress = false;
                        break;
                    case NM_RENAME:
                        // We just got a request to rename an entity, land or water.
                        // This one is easy, just grab the ID and name and make it so.
                        Acknowledge(NetIndex, mySeqNum);
                        TempNum1 = Convert.ToInt32(Arg(NetString, 2));   // Entity ID.
                        string TempStr = Arg(NetString, 3);    // New name for this entity.
                        if (TempNum1 < TILEVAL_COASTLINE)
                        {
                            // We're renaming land.
                            MyMap!.CountryName(TempNum1, TempStr);
                        }
                        else
                        {
                            // We're renaming water.
                            MyMap!.WaterName(TempNum1 - 1000, TempStr);
                        }
                        break;
                    case NM_NEW_COUNTRY_DATA:
                        // Parse country data.  Similar to setup data 2.
                        Acknowledge(NetIndex, mySeqNum);
                        for (int j = 1; j <= MyMap!.NumberOfCountries; j++)
                        {
                            string tempStr = Arg(NetString, j + 1);
                            MyMap.CountryColor(j, Convert.ToInt32(ArgAt(tempStr, 1)));
                            MyMap.CountryType(j, Convert.ToInt32(ArgAt(tempStr, 2)));
                            MyMap.Owner(j, Convert.ToInt32(ArgAt(tempStr, 3)));
                            MyMap.TroopCount(j, Convert.ToInt32(ArgAt(tempStr, 4)));
                        }
                        Land.Refs.CalculateTotals();
                        Land.Refs.SetupIndicators();
                        Land.Refs.RedrawScreen();
                        break;
                    case NM_NEW_STATS:
                        // Just parse out the statistics.
                        Acknowledge(NetIndex, mySeqNum);
                        for (int i = 1; i <= MAX_PLAYERS; i++)
                        {
                            string tempStr = Arg(NetString, (i * 3) - 1);
                            for (int tempInt = 1; tempInt <= MAX_PLAYERS; tempInt++)
                            {
                                STATattacked[i, tempInt] = Convert.ToInt32(ArgAt(tempStr, tempInt));
                            }
                            tempStr = Arg(NetString, (i * 3));
                            for (int tempInt = 1; tempInt <= MAX_PLAYERS; tempInt++)
                            {
                                STATovertaken[i, tempInt] = Convert.ToInt32(ArgAt(tempStr, tempInt));
                            }
                            tempStr = Arg(NetString, (i * 3) + 1);
                            for (int tempInt = 1; tempInt <= MAX_PLAYERS; tempInt++)
                            {
                                STATkilled[i, tempInt] = Convert.ToInt32(ArgAt(tempStr, tempInt));
                            }
                        }
                        break;
                    case NM_PORT_STATS:
                        // Update a single country's port status.  Used for random port destruction.
                        Acknowledge(NetIndex, mySeqNum);
                        TempNum1 = Convert.ToInt32(Arg(NetString, 2));
                        TempInt = Convert.ToInt32(Arg(NetString, 3));
                        MyMap!.CountryType(TempNum1, TempInt);
                        Land.Refs.RedrawScreen();
                        break;
                    case NM_RANDOM_EVENT:
                        TempTurn = Convert.ToInt32(Arg(NetString, 2));
                        if (TempTurn != Land.Refs.GetTurn()) return result;
                        Acknowledge(NetIndex, mySeqNum);
                        TempInt = Convert.ToInt32(Arg(NetString, 3));  // Event Number.
                        TempNum1 = Convert.ToInt32(Arg(NetString, 4)); // Random Country Number.
                        TempInt2 = Convert.ToInt32(Arg(NetString, 5)); // Message Number.
                        // Finally, perform the event ourself.
                        ActivateRandomEvent(TempTurn, TempInt, TempNum1, TempInt2);
                        DrawMap();
                        Land.Refs.UpdateMessages();
                        Land.Refs.SetupIndicators();
                        SNDBonusTwinkles();
                        break;
                    default:
                        break;
                }
            }

        }

        return result;
    }

    public static void InitNetwork()
    {
        // This sub just initializes everything.

        NetworkArbitrationInProgress = false;
        TurnInProgress = false;
        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            for (int j = 1; j <= MSG_Q_SIZE; j++)
            {
                MsgQ[i, j] = string.Empty;
            }
            StringQ[i] = string.Empty;
            Qtimer[i] = 0;
            NetArray[i] = 0;
            DataArray[i] = 0;
        }
    }

    private static void BuildAndSendPlayerList()
    {
        // Server. This function builds and sends the string to tell all
        // clients what the player distribution looks like.

        string TempStr = BuildPlayerList();
        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            if (NetArray[i] > 0)
            {
                SendMsg(i, NM_PLAYER_LIST + "," + TempStr);
            }
        }
    }

    private static string BuildPlayerList()
    {

        string TempStr = string.Empty;
        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            // Each player has a name, a color, and a type.
            TempStr = TempStr + PlayerName[i] + ",";
            TempStr = TempStr + Player[i] + ",";
            if (PlayerType[i] == PTYPE_NETWORK)
            {
                if (MyNetIndex(i) == 0)
                {
                    // This slot hasn't been filled yet.
                    TempStr = TempStr + PTYPE_NET_AVAIL;
                }
                else
                {
                    // Someone's already snagged this color.
                    TempStr = TempStr + PTYPE_NETWORK;
                }
            }
            else
            {
                TempStr = TempStr + PlayerType[i];
            }
            if (i < 6) TempStr = TempStr + ",";
        }

        return TempStr;
    }

    public static int MyNetIndex(int PlayerNum)
    {
        // This function searches NetArray for the passed player number.
        // If it's in the array, we return the network index of that player.
        int MyNetIndex = 0;
        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            if (NetArray[i] == PlayerNum)
            {
                MyNetIndex = i;
                break;
            }
        }
        return MyNetIndex;
    }

    public static void PlayerChoseColor(int Index, string MyName)
    {

        // The player clicked on one of the colors in the list.  If we are
        // a client, in the picking stage, and that color is available,
        // then that's who we'll be!  Send the server a message saying so.
        if (MyNetworkRole == NW_CLIENT && NetworkState == NS_WAITING_FOR_USER_COLOR)
        {
            // We are a client in the right state.  See if this color is valid.
            if (PlayerType[Index] == PTYPE_NET_AVAIL)
            {
                // This is us!  Send the server a message, including our name.
                SendMsg(0, NM_I_CHOOSE_COLOR + "," + Index.ToString().Trim() + "," + MyName);
            }
            else
            {
                // Error message in status line.
                SNDBooBoo();
                NetworkForm.Refs.StatusText.Text = "That color is unavailable.  Choose another.";
            }
        }
    }

    public static void KillWinsockControl(int Index)
    {
        // This sub closes the specified Winsock connection and unloads it if
        // it's not index 0 (0 is always present).

        //Get out if we're trying to mess with a control that doesn't exist for a client.
        if (MyNetworkRole == NW_CLIENT && Index > 0) return;

        if (NetworkForm.FracasSock.Count <= Index || NetworkForm.FracasSock[Index] == null) return;

        if (NetworkForm.FracasSock[Index].Connected)
        {
            //Close the connection.
            NetworkForm.FracasSock[Index].Close();
            Application.DoEvents();
        }

        //Unload this client's connection to us.  (We're obviously the server)
        if (Index > 0)
        {
            NetworkForm.FracasSock.RemoveAt(Index);
        }
    }

    public static void SendStartupDataToAll()
    {
        // Server. Send each of our clients all of the necessary data to start
        // up a game.

        string TempStr = string.Empty;

        //Change our state...
        NetworkState = NS_SENDING_SETUP_DATA;

        //Put our progress bar up...
        NetworkForm.Refs.NetProg.Value = 0;
        NetworkForm.Refs.NetProg.Visible = true;

        // Send every bit of pertinent setup data.  But not preference data.
        // Also include the map dimensions so clients can initialize properly.
        TempStr = CFGCountrySize.ToString().Trim() + "," +
                  CFGLandPct.ToString().Trim() + "," +
                  CFGIslands.ToString().Trim() + "," +
                  CFGLakeSize.ToString().Trim() + "," +
                  CFGProportion.ToString().Trim() + "," +
                  CFGShape.ToString().Trim() + "," +
                  CFGBorders.ToString().Trim() + "," +
                  CFGInitTroopPl.ToString().Trim() + "," +
                  CFGInitTroopCt.ToString().Trim() + "," +
                  CFGBonusTroops.ToString().Trim() + "," +
                  CFGShips.ToString().Trim() + "," +
                  CFGPorts.ToString().Trim() + "," +
                  CFGConquer.ToString().Trim() + "," +
                  CFGAISpeed.ToString().Trim() + "," +
                  CFG1st.ToString().Trim() + "," +
                  CFGHQSelect.ToString().Trim() + "," +
                  CFGUnoccupiedColor.ToString().Trim() + "," +
                  CFGResolution.ToString().Trim() + "," +
                  CFGConquer.ToString().Trim() + "," +
                  CFGCountrySize.ToString().Trim() + "," +
                  MyMap!.NumberOfCountries.ToString().Trim() + "," +
                  MyMap.LakeCode.ToString().Trim() + "," +
                  Land.Refs.GetTurn().ToString().Trim();

        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            if (NetArray[i] > 0)
            {
                //This control is in use.  Send them the startup data.
                SendMsg(i, NM_SETUP_DATA_1 + "," + TempStr);
            }
        }
    }

    private static string BuildMapLine(int LineNum)
    {

        // Server.  This function packs one line of map data into a string for
        // transmission to a client.
        string TempStr = string.Empty;
        //The first argument is the line number...
        TempStr = NM_SETUP_DATA_MAP + "," + LineNum.ToString().Trim() + ",";
        //Then each piece of map data.
        for (int i = 1; i <= MyMap!.Xsize; i++)
        {
            TempStr = TempStr + MyMap.Grid(i, LineNum).ToString().Trim();
            if (i < MyMap.Xsize) TempStr = TempStr + ",";
        }

        return TempStr;
    }

    private static string BuildCountryData()
    {
        // This function puts all country data for this map into a string.
        string TempStr = string.Empty;
        for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            TempStr = TempStr + MyMap.CountryColor(i).ToString().Trim() + "@" +
                              MyMap.CountryType(i).ToString().Trim() + "@" +
                              MyMap.Owner(i).ToString().Trim() + "@" +
                              MyMap.TroopCount(i).ToString().Trim();
            if (i < MyMap.NumberOfCountries) TempStr = TempStr + ",";
        }

        return TempStr;
    }

    private static string BuildLandNameData()
    {
        // This function puts all land name information for this map into a string.
        string TempStr = string.Empty;
        for (int i = 1; i <= MyMap!.NumberOfCountries; i++)
        {
            TempStr = TempStr + MyMap.CountryName(i);
            if (i < MyMap.NumberOfCountries) TempStr = TempStr + ",";
        }

        return TempStr;
    }

    private static string BuildWaterNameData()
    {
        // This function puts all water name information for this map into a string.
        string TempStr = string.Empty;
        for (int i = 1001; i <= MyMap!.LakeCode; i++)
        {
            TempStr = TempStr + MyMap.WaterName(i - 1000);
            if (i < MyMap.LakeCode) TempStr = TempStr + ",";
        }
        return TempStr;
    }

    public static void SendClickToNetwork(int Turn, int Phase, int CountryNum, int NotThisPlayer)
    {
        // This sub determines if we need to send this click to any of our clients

        if (MyNetworkRole == NW_SERVER)
        {
            // We need to send this click to all clients.
            // Either a human at the server just moved, or we are propagating a client's
            // move to the other clients.
            for (int i = 1; i <= MAX_PLAYERS; i++)
            {
                if ((NetArray[i] > 0) && (i != NotThisPlayer))
                {
                    SendMsg(i, NM_PLAYER_MOVE + "," + Turn.ToString().Trim() + "," +
                                Phase.ToString().Trim() + "," + CountryNum.ToString().Trim());
                }
            }
        }
        else if (MyNetworkRole == NW_CLIENT)
        {
            //Send this click to the server for distribution to everyone.
            SendMsg(0, NM_PLAYER_MOVE + "," + Turn.ToString().Trim() + "," +
                        Phase.ToString().Trim() + "," + CountryNum.ToString().Trim());
        }

    }

    public static void SendPassToNetwork(int Turn, int Phase, int NotThisPlayer)
    {
        // This sub determines if we need to send this pass to any of our clients

        if (MyNetworkRole == NW_SERVER)
        {
            //We need to send this pass to all clients.
            //Either a human at the server just passed, or we are propagating a client's
            //pass to the other clients.
            for (int i = 1; i <= MAX_PLAYERS; i++)
            {
                if ((NetArray[i] > 0) && (i != NotThisPlayer))
                {
                    SendMsg(i, NM_PLAYER_PASS + "," + Turn.ToString().Trim() + "," +
                                Phase.ToString().Trim());
                }
            }
        }
        else if (MyNetworkRole == NW_CLIENT)
        {
            //Send this pass to the server for distribution to everyone.
            SendMsg(0, NM_PLAYER_PASS + "," + Turn.ToString().Trim() + "," +
                        Phase.ToString().Trim());
        }
    }

    public static void SendResignToNetwork(int Turn, int Phase, int NotThisPlayer)
    {
        // This sub determines if we need to send this resign to any of our clients
        // or to the server.

        if (MyNetworkRole == NW_SERVER)
        {
            //We need to send this resign to all clients.
            //Either a human at the server just resigned, or we are propagating a client's
            //resign to the other clients.
            for (int i = 1; i <= MAX_PLAYERS; i++)
            {
                if ((NetArray[i] > 0) && (i != NotThisPlayer))
                {
                    SendMsg(i, NM_PLAYER_RESIGN + "," + Turn.ToString().Trim() + "," +
                                Phase.ToString().Trim());
                }
            }
        }
        else if (MyNetworkRole == NW_CLIENT)
        {
            //Send this resign to the server for distribution to everyone.
            SendMsg(0, NM_PLAYER_RESIGN + "," + Turn.ToString().Trim() + "," +
                        Phase.ToString().Trim());
        }
    }

    public static void SendRenameToNetwork(int Entity, string NewName, int NotThisPlayer)
    {
        // This sub determines if we need to send this rename request to any of our clients
        // or to the server.  Note that this sub handles renames of both land and water.
        if (MyNetworkRole == NW_SERVER)
        {
            //We need to send this rename to all clients.
            //Either a human at the server just renamed something, or we are propagating a client's
            //rename request to the other clients.
            for (int i = 1; i <= MAX_PLAYERS; i++)
            {
                if ((NetArray[i] > 0) && (i != NotThisPlayer))
                {
                    SendMsg(i, NM_RENAME + "," + Entity.ToString().Trim() + "," + NewName);
                }
            }
        }
        else if (MyNetworkRole == NW_CLIENT)
        {
            //Send this rename request to the server for distribution to everyone.
            SendMsg(0, NM_RENAME + "," + Entity.ToString().Trim() + "," + NewName);
        }
    }

    public static void SendQuitToNetwork()
    {

        // This sub gets run when someone cancels the network form.
        // Send out the quit message if anyone is listening.

        if (MyNetworkRole == NW_SERVER)
        {
            // We're the server.  Tell all clients that we're gone.
            for (int i = 1; i <= MAX_PLAYERS; i++)
            {
                if (NetArray[i] > 0)
                {
                    if (NetworkForm.FracasSock[i].Connected)
                    {
                        SendMsg(i, "$" + AddCRC(NM_I_AM_QUITTING) + "%");
                        Application.DoEvents();
                    }
                }
            }
        }
        else
        {
            // We're a client.  Tell the server we're quitting.
            if (NetworkForm.FracasSock.Count > 0 && NetworkForm.FracasSock[0].Connected)
            {
                SendMsg(0, "$" + AddCRC(NM_I_AM_QUITTING) + "%");
                Application.DoEvents();
            }
        }
    }

    public static void SendChatToNetwork(int PlayerNum, string ChatText, int NotThisPlayer)
    {
        // This sub determines if we need to send this chat line to any of our clients
        // or to the server.

        if (MyNetworkRole == NW_SERVER)
        {
            //We need to send this line of chat to all clients.
            for (int i = 1; i <= MAX_PLAYERS; i++)
            {
                if ((NetArray[i] > 0) && (i != NotThisPlayer))
                {
                    SendChatString(i, PlayerNum, ChatText);
                }
            }
        }
        else if (MyNetworkRole == NW_CLIENT)
        {
            //Send this line of chat to the server for distribution to everyone.
            SendChatString(0, PlayerNum, ChatText);
        }
    }

    private static void SendChatString(int Index, int PlayerNum, string ChatText)
    {
        string ChatStr = string.Empty;

        if (NetworkForm.FracasSock[Index].Connected)
        {
            // @ signs instead of commas so we can send commas in our chats!
            ChatStr = NM_CHAT + "@" + PlayerNum.ToString().Trim() + "@" + ChatText;
            ChatStr = AddCRC(ChatStr);
            byte[] sendBytes = Encoding.ASCII.GetBytes("$" + ChatStr + "%");
            NetworkForm.FracasSock[Index].GetStream().Write(sendBytes, 0, sendBytes.Length);
            Application.DoEvents();
        }

    }

    private static void SendMsg(int Index, string Msg)
    {

        // This sub adds the passed message to the appropriate message queue.
        // We rely on the net timer to send the first message in the queue, and
        // messages are only removed from the queue when an ACK is received.

        int Pnum;
        bool Done;
        if (Msg == string.Empty) return;

        // First, get a new sequence number and append it to the start of the message.
        SeqNumber = SeqNumber + 1;
        if (SeqNumber > MAX_SEQ_NUM) SeqNumber = 1;
        Msg = SeqNumber + "@" + Msg;

        // Then calculate a checksum for this message and append it to the front.
        Msg = AddCRC(Msg);

        // Now add this message to the passed player's queue.
        Pnum = Index;
        if (MyNetworkRole == NW_CLIENT) Pnum = 1;
        Done = false;

        // Loop forever and ever until we can put this message in the queue!
        // Hopefully, we only go through this loop once.  But if a queue is full to the
        // brim, we *CAN'T* drop the message.  So we'll freeze ourselves until the
        // recipient acknowledges something in the queue and frees up space.

        do
        {
            for (int i = 1; i <= MSG_Q_SIZE; i++)
            {
                if (MsgQ[Pnum, i] == string.Empty)
                {
                    // We found the first empty slot, so put the message there.
                    MsgQ[Pnum, i] = Msg;
                    Done = true;
                    // If this is the first message we're sending, make sure it goes out quick.
                    if (i == 1) Qtimer[Pnum] = 0;
                    break;
                }
            }
            if (Done == false) Application.DoEvents();   //Give our NetTimer time to process sends.
        } while (Done == false);
    }

    private static void AckMsg(int Index, int SeqNum)
    {

        // This sub removes the message with the given sequence number from the
        // passed player's message queue.  ONLY THE FIRST message in a queue can
        // be acked!  If this message is never acked, we have a PROBLEM.

        int Pnum;
        string TempStri;
        TempStri = string.Empty;

        Pnum = Index;

        if (MyNetworkRole == NW_CLIENT) Pnum = 1;

        // We check the second @ argument for the seq num because the first is the CRC!
        if (ArgAt(MsgQ[Pnum, 1], 2) == SeqNum.ToString())
        {
            //Get this message text.
            TempStri = ArgAt(MsgQ[Pnum, 1], 3);
            //Let's bump the whole queue up until this message text is no longer first.
            //This will ACK all messages in the queue that are retries of the same message.
            do
            {
                for (int i = 2; i <= MSG_Q_SIZE; i++)
                {
                    MsgQ[Pnum, i - 1] = MsgQ[Pnum, i];
                }
                MsgQ[Pnum, MSG_Q_SIZE] = string.Empty;
            } while (ArgAt(MsgQ[Pnum, 1], 3) == TempStri);
            //Also reset our queue timer to immediately send the next one!
            Qtimer[Pnum] = 0;
        }
        else
        {
            //Tried to ACK another message besides the first.  Bad, bad, bad!
            MessageBox.Show("An Acknowledgement error occurred.  Please Email jmerlo@austin.rr.com and report this bug!", "ACK error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void HeartBeat()
    {
        // First, we see if our StringQ has a complete message in it.
        // If so, strip it off the front.
        if (MyNetworkRole == NW_SERVER)
        {
            // We need to check every one of our Queues.
            for (int i = 1; i <= MAX_PLAYERS; i++)
            {
                string NetString = StringQMaintenance(i);
                if (NetString != string.Empty)
                {
                    // Now do whatever this message is telling us to do.  If we get a RETRY return code,
                    // it means that we are lagging behind our partner and should keep trying until
                    // our turn and phase catch up.
                    string ReturnCode;
                    do
                    {
                        ReturnCode = ParseIncomingMessage(i, NetString);
                        if (ReturnCode == NM_RETRY) Application.DoEvents();
                    } while (ReturnCode == NM_RETRY);
                    NetworkForm.ActOnReturnCode(ReturnCode);
                }
            }
        }
        else if (MyNetworkRole == NW_CLIENT)
        {
            // Our queue is number 1.
            string NetString = StringQMaintenance(1);
            if (NetString != string.Empty)
            {
                // Retry until we can successfully parse this message.
                string ReturnCode;
                do
                {
                    ReturnCode = ParseIncomingMessage(1, NetString);
                    if (ReturnCode == NM_RETRY) Application.DoEvents();
                } while (ReturnCode == NM_RETRY);
                NetworkForm.ActOnReturnCode(ReturnCode);
            }
        }


        // This is where we send the next message in each queue we hold.
        if (MyNetworkRole == NW_SERVER)
        {
            for (int i = 1; i <= MAX_PLAYERS; i++)
            {
                if (NetArray[i] > 0)
                {
                    //Check that the Winsock connection is good before sending.
                    if (NetworkForm.FracasSock[i].Connected)
                    {
                        Qtimer[i] = Qtimer[i] - 1;
                        if (Qtimer[i] < 1)
                        {
                            Qtimer[i] = MSG_TIMEOUT;
                            if (MsgQ[i, 1] != string.Empty)
                            {
                                // Send the data.
                                byte[] sendBytes = Encoding.ASCII.GetBytes("$" + MsgQ[i, 1] + "%");
                                NetworkForm.FracasSock[i].GetStream().Write(sendBytes, 0, sendBytes.Length);
                                Application.DoEvents();
                            }
                        }
                    }
                }
            }
        }
        else if (MyNetworkRole == NW_CLIENT)
        {
            //Check that the Winsock connection is good before sending.
            if (NetworkForm.FracasSock.Count > 0 && NetworkForm.FracasSock[0].Connected)
            {
                Qtimer[1] = Qtimer[1] - 1;
                if (Qtimer[1] < 1)
                {
                    Qtimer[1] = MSG_TIMEOUT;
                    if (MsgQ[1, 1] != string.Empty)
                    {
                        // Send the data.
                        byte[] sendBytes = Encoding.ASCII.GetBytes("$" + MsgQ[1, 1] + "%");
                        NetworkForm.FracasSock[0].GetStream().Write(sendBytes, 0, sendBytes.Length);
                        Application.DoEvents();
                    }
                }
            }
        }
    }

    private static string StringQMaintenance(int Index)
    {
        //  This function strips the first complete message from the front of the queue.
        // Complete messages start with a dollar sign ($) and end with a percent (%).
        string MyStr;
        int Pos;

        //Get out if no string.
        string result = string.Empty;
        MyStr = StringQ[Index];
        if (MyStr == string.Empty) return result;

        //Find where the first $ is.
        Pos = MyStr.IndexOf("$");
        if (Pos == -1)
        {
            //No dollar sign at all.  That's bad.  Clean this string out.
            StringQ[Index] = string.Empty;
            return result;
        }
        else if (Pos > 0)
        {
            //The first dollar sign is *not* at the front of the queue.
            //We need to chop off the leading stuff and start there.
            StringQ[Index] = MyStr.Substring(Pos);
            MyStr = StringQ[Index];
        }

        //If we got here then the first dollar sign is at the front of the queue.
        //This is normal.  Now find if we have an end cap in there somewhere.
        Pos = MyStr.IndexOf("%");
        if (Pos == -1)
        {
            //No end cap.  The rest of this line will be coming soon.
            return result;
        }

        //If we got here then we have a complete message between 1 and Pos.
        result = MyStr.Substring(1, Pos - 1);
        StringQ[Index] = MyStr.Substring(Pos + 1);
        return result;
    }

    private static void Acknowledge(int Index, int SeqNum)
    {
        // This function sends an ACK out to a networked machine.
        int Pnum;
        string AckStr = string.Empty;
        Pnum = Index;
        if (MyNetworkRole == NW_CLIENT) Pnum = 0;

        if (NetworkForm.FracasSock[Pnum].Connected)
        {
            AckStr = NM_ACK + "," + SeqNum.ToString().Trim();
            AckStr = AddCRC(AckStr);
            byte[] sendBytes = Encoding.ASCII.GetBytes("$" + AckStr + "%");
            NetworkForm.FracasSock[Pnum].GetStream().Write(sendBytes, 0, sendBytes.Length);
            Application.DoEvents();
        }
    }
    private static string StripOffFirstAt(string MyStr)
    {
        // This sub strips the leading sequence number off of the passed string.
        // There is an AT sign (@) between the sequence number and the rest of the
        // message.
        int Pos;
        // Default return value is the passed string itself.
        string result = MyStr;
        Pos = MyStr.IndexOf("@");
        if (Pos > -1)
        {
            result = MyStr.Substring(Pos + 1);
        }
        return result;
    }

    private static string AddCRC(string MyStr)
    {
        // This function calculates a CRC for the passed string and prepends it to
        // the same string, which then gets returned.

        int CRC = CalcCRC(MyStr);
        return CRC.ToString().Trim() + "@" + MyStr;
    }

    private static int CalcCRC(string MyStr)
    {
        int CRC = 0;
        for (int i = 0; i < MyStr.Length; i++)
        {
            CRC = CRC + (int)MyStr[i];
        }
        return CRC;
    }

    public static void ConcatenateStr(int Index, string Frag)
    {
        // This sub just sticks the passed fragment on the end of the indicated queue.
        StringQ[Index] = StringQ[Index] + Frag;
    }

    public static void SendNewCountryData()
    {
        // Server. This sub sends out all country data to all clients.  Used after a calamatous
        // random event like killing an HQ with the 'chaos erupts' option.

        string TempStr = string.Empty;
        TempStr = BuildCountryData();

        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            if (NetArray[i] > 0)
            {
                SendMsg(i, NM_NEW_COUNTRY_DATA + "," + TempStr);
            }
        }
    }

    public static void SendPortStatus(int ThisCountry)
    {
        // Server.  This sub updates a single country's port status.  Used for random port destruction.

        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            if (NetArray[i] > 0)
            {
                SendMsg(i, NM_PORT_STATS + "," + ThisCountry.ToString().Trim() + "," + MyMap!.CountryType(ThisCountry).ToString().Trim());
            }
        }
    }

    public static void SendUpdatedStats()
    {
        // Server. This sub updates the overtaken stats on the stat screen after someone's HQ
        // goes down with the 'chaos erupts' option chosen.

        string TempStrg = string.Empty;
        TempStrg = BuildStats();

        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            if (NetArray[i] > 0)
            {
                SendMsg(i, NM_NEW_STATS + "," + TempStrg);
            }
        }
    }

    private static string BuildStats()
    {
        // Package up our overtaken stats.
        string TempStr = string.Empty;

        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            TempStr = TempStr + STATattacked[i, 1].ToString().Trim() + "@" + STATattacked[i, 2].ToString().Trim() + "@" +
                              STATattacked[i, 3].ToString().Trim() + "@" + STATattacked[i, 4].ToString().Trim() + "@" +
                              STATattacked[i, 5].ToString().Trim() + "@" + STATattacked[i, 6].ToString().Trim() + ",";
            TempStr = TempStr + STATovertaken[i, 1].ToString().Trim() + "@" + STATovertaken[i, 2].ToString().Trim() + "@" +
                              STATovertaken[i, 3].ToString().Trim() + "@" + STATovertaken[i, 4].ToString().Trim() + "@" +
                              STATovertaken[i, 5].ToString().Trim() + "@" + STATovertaken[i, 6].ToString().Trim() + ",";
            TempStr = TempStr + STATkilled[i, 1].ToString().Trim() + "@" + STATkilled[i, 2].ToString().Trim() + "@" +
                              STATkilled[i, 3].ToString().Trim() + "@" + STATkilled[i, 4].ToString().Trim() + "@" +
                              STATkilled[i, 5].ToString().Trim() + "@" + STATkilled[i, 6].ToString().Trim();
            if (i < MAX_PLAYERS) TempStr = TempStr + ",";
        }

        return TempStr;
    }

    public static void SendRandomEvent(int Turn, int EventNumber, long RandomCountryNumber, int MessageNumber)
    {
        // Server.  Sends random event information down to all clients for immedate processing
        // before a person's turn can get too far underway.

        for (int i = 1; i <= MAX_PLAYERS; i++)
        {
            if (NetArray[i] > 0)
            {
                SendMsg(i, NM_RANDOM_EVENT + "," + Turn.ToString().Trim() + "," + EventNumber.ToString().Trim() + "," +
                                   RandomCountryNumber.ToString().Trim() + "," + MessageNumber.ToString().Trim());
            }
        }
    }
}
