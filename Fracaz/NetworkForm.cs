using Fracaz.Helpers;
using System.Net.Sockets;

using static Fracaz.Declarations;
using static Fracaz.NetworkPlay;

namespace Fracaz;

public partial class NetworkForm : Form
{
    private static NetworkForm? Me { get; set; }

    private static Label[] ColorPool = new Label[7];
    private static Label[] ColorStatus = new Label[7];

    public static TcpListener? FracasSockListener;
    public static List<TcpClient> FracasSock = new List<TcpClient>(7);

    //public static NetworkFormRefs Refs { get; private set; } = new ();

    public static class Refs
    {
        public static Label StatusText => Me!.StatusText;
        public static Button CancelBut => Me!.CancelBut;
        public static ProgressBar NetProg => Me!.NetProg;
        public static Label lblName => Me!.lblName;
        public static TextBox MyPlayerName => Me!.MyPlayerName;
    }

    public NetworkForm()
    {
        InitializeComponent();

        Me = this;

        explicitClose = false;

        ColorPool = [
            null!,
            ColorPool1,
            ColorPool2,
            ColorPool3,
            ColorPool4,
            ColorPool5,
            ColorPool6
        ];

        ColorStatus = [
            null!,
            ColorStatus1,
            ColorStatus2,
            ColorStatus3,
            ColorStatus4,
            ColorStatus5,
            ColorStatus6
        ];

        //Refs.StatusText = StatusText;
        //Refs.CancelBut = CancelBut;
        //Refs.NetProg = NetProg;
        //Refs.lblName = lblName;
        //Refs.MyPlayerName = MyPlayerName;
    }

    private void CancelBut_Click(object sender, EventArgs e)
    {
        CancelBut_Click();
    }

    internal static void CancelBut_Click()
    {
        // This network form has not been instantiated yet.
        if (Me == null) return;

        // Restore our PlayerType settings so that the Players dialog will be normal
        // the next time this player goes there.
        if (MyNetworkRole == NW_CLIENT)
        {
            for (var i = 1; i <= MAX_PLAYERS; i++)
            {
                PlayerType[i] = TempPlayerType[i];
            }
        }

        // We can't join or start any games until this is done!
        Land.Refs.MenuNew.Enabled = false;
        Land.Refs.MenuJoinGame.Enabled = false;
        Land.Refs.MenuSame.Enabled = false;
        Land.Refs.MenuLoad.Enabled = false;

        Land.Refs.MenuLoadSavedGame.Enabled = false;
        Land.Refs.MenuGetOptionsFromMapFile.Enabled = false;

        NetworkArbitrationInProgress = true;

        // Make us immediately invisible.
        Me?.Hide();

        // Get rid of the chat window.
        ChatForm.Unload();

        // Tell anyone who's listening that we're gone!
        SendQuitToNetwork();

        // Give the Winsock controls time to send that message.
        for (var i = 1; i <= 30; i++)
        {
            Application.DoEvents();
            Thread.Sleep(20);
        }

        NetworkArbitrationInProgress = false;

        // Make me a normal app, and get out of networking.
        ActOnReturnCode(NM_I_AM_QUITTING);
    }

    static bool formLoaded = false;
    private void Form_Load(object sender, EventArgs e)
    {
        if (formLoaded) return;

        formLoaded = true;

        InitNetwork();

        // Set up the network form based on our type.
        if (MyNetworkRole == NW_SERVER)
        {
            // We're the server.
            lblHost.Visible = false;
            lblName.Visible = false;
            HostName.Visible = false;
            MyPlayerName.Visible = false;
            JoinBut.Visible = false;
            CancelBut.Visible = true;

            SetUpColorList();

            StatusText.Text = "Waiting for players to join...";

            //Start listening for clients.
            NetworkState = NS_WAITING_FOR_CONNECTIONS;
            ListenForClients();
        }
        else
        {
            // We're a client.
            lblHost.Visible = true;
            lblName.Visible = true;
            HostName.Visible = true;
            MyPlayerName.Visible = true;
            JoinBut.Visible = true;
            CancelBut.Visible = true;
            NetworkState = NS_IDLE;

            StatusText.Text = "Enter server name or TCP/IP address to connect to and click Join.";


            for (var i = 1; i <= 6; i++)
            {
                ColorPool[i].Visible = false;
                ColorStatus[i].Visible = false;
            }
        }

        NetProg.Visible = false;
    }


    public static void SetUpColorList()
    {

        // Set up the colors.
        for (var i = 1; i <= MAX_PLAYERS; i++)
        {
            ColorPool[i].BackColor = Color.FromArgb(PlayerColorCodes[Player[i]]);
            ColorPool[i].ForeColor = Color.FromArgb(PlayerTextColor[Player[i]]);
            ColorPool[i].Text = PlayerName[i];
            switch (PlayerType[i])
            {
                case PTYPE_INACTIVE:
                    ColorPool[i].Visible = false;
                    ColorStatus[i].Visible = false;
                    break;
                case PTYPE_HUMAN:
                    ColorPool[i].Visible = true;
                    ColorStatus[i].Visible = true;
                    if (MyNetworkRole == NW_SERVER)
                    {
                        ColorStatus[i].Text = "Local";
                    }
                    else
                    {
                        ColorStatus[i].Text = "Host";
                    }
                    break;
                case PTYPE_COMPUTER:
                    ColorPool[i].Visible = true;
                    ColorStatus[i].Visible = true;
                    ColorStatus[i].Text = "Computer";
                    break;
                case PTYPE_NETWORK:
                    ColorPool[i].Visible = true;
                    ColorStatus[i].Visible = true;
                    if (MyNetworkRole == NW_SERVER)
                    {
                        if (MyNetIndex(i) == 0)
                        {
                            ColorStatus[i].Text = "Waiting...";
                        }
                        else
                        {
                            ColorStatus[i].Text = "Joined";
                        }
                    }
                    else
                    {
                        ColorStatus[i].Text = "Taken";
                    }
                    break;
                case PTYPE_NET_AVAIL:
                    //Used to tell clients when a player slot is available.
                    ColorPool[i].Visible = true;
                    ColorStatus[i].Visible = true;
                    ColorStatus[i].Text = "Available";
                    break;
            }
        }

        Me?.Refresh();

        Application.DoEvents();
    }

    static bool explicitClose = false;
    private void Form_QueryUnload(object sender, FormClosingEventArgs e)
    {
        if (!explicitClose)
        {
            e.Cancel = true;
        }
    }

    private static bool HBtimerRunning;

    private void HBtimer_Timer(object sender, EventArgs e)
    {
        if (HBtimerRunning) return;

        HBtimerRunning = true;
        try
        {
            HBtimer_Timer_Tick(sender, e);
        }
        finally
        {
            HBtimerRunning = false;
        }
    }

    private void HBtimer_Timer_Tick(object sender, EventArgs e)
    {
        // This is the entry point for the heartbeat timer.  This function is
        // responsible for sending out the next message in each queue.  Important!

        HeartBeat();
    }

    private void JoinBut_Click(object sender, EventArgs e)
    {
        // Tell the server that we're ready to go!
        if (ConnectToHost(HostName.Text))
        {
            //Connection was accepted.
            NetworkState = NS_WAITING_FOR_JOIN_ACK;
            StatusText.Text = "Enter your name and click on an available color.";
            JoinBut.Visible = false;
            HostName.Visible = false;
            lblHost.Visible = false;
        }
        else
        {
            //Connection refused or bad host name, etc.
            StatusText.Text = "Unable to connect.  Check Hostname and try again.";
        }
    }


    private void ColorPool_Click(object sender, EventArgs e)
    {
        var Index = Array.IndexOf(ColorPool, sender);

        //The player clicked on one of the colors.  See if they're choosing.
        if (MyPlayerName.Text != string.Empty)
        {
            PlayerChoseColor(Index, MyPlayerName.Text);
        }
        else
        {
            PlayerChoseColor(Index, ColorPool[Index].Text);
        }
    }

    private static void AdjustPlayerTypesForClient()
    {
        // This sub takes the current Player Types and adjusts them for this
        // client.  Basically, this means making us a HUMAN and every other
        // active player a SERVER controlled player.

        // Make every active player a SERVER player.  This means that we rely
        // on the server to make this player's moves.  As a client, we don't care
        // if the player is really a computer or another networked player --
        // We wait on them regardless.
        for (var i = 1; i <= MAX_PLAYERS; i++)
        {
            if (PlayerType[i] > PTYPE_INACTIVE)
            {
                PlayerType[i] = PTYPE_SERVER;
            }
        }
        //Mark US as a HUMAN.  This is so we can get input from this machine!
        PlayerType[MyClientIndex] = PTYPE_HUMAN;
    }

    public static void ActOnReturnCode(string ReturnCode)
    {

        // This sub performs actions on the network form based on messages we receive
        // from other machines.

        if (ReturnCode == string.Empty) return;

        switch (ReturnCode)
        {
            case NM_I_AM_QUITTING:
                //Make me a normal app, and get out of networking.
                for (var i = 0; i <= MAX_PLAYERS; i++)
                {
                    KillWinsockControl(i);
                }
                InitNetwork();
                NetworkState = NS_IDLE;
                MyNetworkRole = NW_NONE;
                FracasSockListener?.Stop();
                // TODO:  Set up the title screen if we aren't already there.
                Land.Refs.SetupMenusForGameOver();
                MapMade = false;
                GameMode = GM_BUILDING_TITLE;

                explicitClose = true;
                Me?.Close();

                break;

            case NM_YOU_ARE_GOOD_TO_GO:
                if (MyNetworkRole == NW_SERVER)
                {
                    //We just connected with all network players.  It's time to start.
                    //Hide this form so our Winsock controls still work.
                    Me?.Hide();
                }
                else if (MyNetworkRole == NW_CLIENT)
                {
                    //We just got our main config packet.  Need to leave so that our
                    //map can be set up normally.  We'll be right back to get country
                    //data though.
                    Me?.Hide();
                }
                break;
            case NM_SETUP_COMPLETE:
                //Store off the PlayerType array, since it can change quite a bit depending on whether
                //we're a server or a client.  These will be restored when leaving a network game so
                //that we don't have any weirdness on the Players dialog.
                for (var i = 1; i <= MAX_PLAYERS; i++)
                {
                    TempPlayerType[i] = PlayerType[i];
                }
                if (MyNetworkRole == NW_CLIENT)
                {
                    //We've got all of our data and are ready to start!
                    //We need to adjust player types here (this machine is human,
                    //everyone else is controlled by the server) and then
                    //return back to setup.
                    AdjustPlayerTypesForClient();
                    Me?.Hide();
                }
                else
                {
                    //We're the server and everyone is here.  Just start.
                    Me?.Hide();
                }
                break;
        }
    }

    public static void UpdateProgressBar(int Prog)
    {
        // This network form has not been instantiated yet.
        if (Me == null) return;

        //This sub updates the progress bar we see during the transmission of map data.
        //If Prog is 0, we're the server and we need to add up data for all clients.
        int MapVert = MyMap!.Ysize;
        float Progress;
        int NumClients = 0;
        int TotalLinesSent = 0;
        if (Prog == 0)
        {
            //Count the clients and add up sent lines.
            for (int i = 1; i <= MAX_PLAYERS; i++)
            {
                if (NetArray[i] > 0)
                {
                    NumClients++;
                    TotalLinesSent += DataArray[i];
                }
            }
            //Progress is the percentage of everything sent to all clients.
            Progress = (TotalLinesSent / (float)(MapVert * NumClients)) * 100;
        }
        else
        {
            //Client.  Prog is how many lines of the map we've received.
            Progress = (Prog / (float)MapVert) * 100;
        }

        Me.NetProg.Value = (int)Progress;
    }
    private void Form_Paint(object sender, PaintEventArgs e)
    {
        const int Xbase = 6;
        const int Ybase = 13;
        const int Dist = 24;

        if (NetworkState == NS_WAITING_FOR_USER_COLOR)
        {
            using (var gdiOperations = new GDIOperations(e.Graphics, Land.LandMap))
            {

                for (var i = 1; i <= MAX_PLAYERS; i++)
                {
                    if (PlayerType[i] == PTYPE_NET_AVAIL)
                    {
                        gdiOperations.BitBlt(Xbase, Ybase + ((i - 1) * Dist), GFX_GRID, GFX_GRID,
                            GFX_ICONS_X * GFX_GRID, (GFX_ICONS_Y + 1) * GFX_GRID, SRCAND);
                        gdiOperations.BitBlt(Xbase, Ybase + ((i - 1) * Dist), GFX_GRID, GFX_GRID,
                            GFX_ICONS_X * GFX_GRID, GFX_ICONS_Y * GFX_GRID, SRCINVERT);
                    }
                }
            }
        }
    }

    internal static void ShowMe(Land land)
    {
        if (Me != null)
        {
            Me.ShowDialog(land);
        }
        else
        {
            var networkForm = new NetworkForm();
            networkForm.ShowDialog(land);
        }
    }
}
