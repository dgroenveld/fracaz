using static Fracaz.Declarations;
using static Fracaz.NetworkPlay;

namespace Fracaz;

public partial class ChatForm : Form
{

    const int NUM_CHAT_LINES = 12;

    Label?[] NameSlot = new Label[NUM_CHAT_LINES + ONE_ARRAY_FIX];
    Label?[] TextSlot = new Label[NUM_CHAT_LINES + ONE_ARRAY_FIX];

    private static ChatForm? Me;

    public ChatForm()
    {
        InitializeComponent();

        Me = this;

        NameSlot = new[] { null!, NameSlot1, NameSlot2, NameSlot3, NameSlot4, NameSlot5, NameSlot6, NameSlot7, NameSlot8, NameSlot9, NameSlot10, NameSlot11, NameSlot12 };

        TextSlot = new[] { null!, TextSlot1, TextSlot2, TextSlot3, TextSlot4, TextSlot5, TextSlot6, TextSlot7, TextSlot8, TextSlot9, TextSlot10, TextSlot11, TextSlot12 };
    }

    private void Form_Load(object sender, EventArgs e)
    {
        for (int i = 1; i <= NUM_CHAT_LINES; i++)
        {
            NameSlot[i]!.Text = string.Empty;
            TextSlot[i]!.Text = string.Empty;
            NameSlot[i]!.BackColor = this.BackColor;
            TextSlot[i]!.BackColor = this.BackColor;

            ChatText.Text = string.Empty;
            AcceptButton = SendBut;
        }
    }

    private void ExitBut_Click(object sender, EventArgs e)
    {
        Me!.Hide();
    }

    private void SendBut_Click(object sender, EventArgs e)
    {

        // Leave if not in a network game
        if (MyNetworkRole == NW_NONE) return;

        // We are in a network game.  Send this text on out.
        if (ChatText.Text == string.Empty) return;

        int Sender = 0;
        if (MyClientIndex == 0)
        {
            // We're a server, and we could have *several* humans on this machine chatting.
            // If there's only one human, then that's the sender.  Otherwise, the sender is HOST.
            int HumanCt = 0;
            for (int i = 1; i <= MAX_PLAYERS; i++)
            {
                if (PlayerType[i] == PTYPE_HUMAN)
                {
                    HumanCt++;
                    Sender = i;
                }
            }
            if (HumanCt != 1) Sender = 0;
        }
        else
        {
            // We're a client, the sender is us.
            Sender = MyClientIndex;
        }

        // Send it to the network.
        SendChatToNetwork(Sender, RemoveLeadingSpace(ChatText.Text), 0);
        // Update our own chat lines.
        ChatTextReceived(Sender, RemoveLeadingSpace(ChatText.Text));

        ChatText.Text = string.Empty;
        ChatText.Focus();
    }

    internal static void ChatTextReceived(int PlayerNum, string TextLine)
    {
        string ChName;
        Color ChBackColor;
        Color ChForeColor;
        Color TxBackColor;
        Color TxForeColor;
        bool TxBold;


        BumpChatLines();

        if (PlayerNum == 0)
        {
            ChName = "Host";
            ChBackColor = Color.White;
            ChForeColor = Color.Black;
            TxBackColor = Me!.BackColor;
            TxForeColor = Color.Black;
        }
        else
        {
            ChName = PlayerName[PlayerNum];
            ChBackColor = Color.FromArgb(PlayerColorCodes[Player[PlayerNum]]);
            ChForeColor = Color.FromArgb(PlayerTextColor[Player[PlayerNum]]);
            TxBackColor = Me!.BackColor;
            TxForeColor = Color.Black;
        }

        TxBold = false;
        if (TextLine.ToLower().StartsWith("/me"))
        {
            TextLine = TextLine.Substring(3);
            TextLine = Me!.RemoveLeadingSpace(TextLine);
            if (TextLine == string.Empty) TextLine = "...";
            TextLine = ChName + " " + TextLine;
            TxBold = true;
            TxBackColor = ChBackColor;
            ChName = string.Empty;
            ChBackColor = Me.BackColor;
        }

        Me!.NameSlot[NUM_CHAT_LINES]!.Text = ChName;
        Me.NameSlot[NUM_CHAT_LINES]!.BackColor = ChBackColor;
        Me.NameSlot[NUM_CHAT_LINES]!.ForeColor = ChForeColor;
        Me.TextSlot[NUM_CHAT_LINES]!.Font = new Font(Me.TextSlot[NUM_CHAT_LINES]!.Font, TxBold ? FontStyle.Bold : FontStyle.Regular);
        Me.TextSlot[NUM_CHAT_LINES]!.Text = TextLine;
        Me.TextSlot[NUM_CHAT_LINES]!.BackColor = TxBackColor;
        Me.TextSlot[NUM_CHAT_LINES]!.ForeColor = TxForeColor;
    }

    private static void BumpChatLines()
    {
        for (int i = 2; i <= NUM_CHAT_LINES; i++)
        {
            Me!.NameSlot[i - 1]!.Text = Me.NameSlot[i]!.Text;
            Me.NameSlot[i - 1]!.BackColor = Me.NameSlot[i]!.BackColor;
            Me.NameSlot[i - 1]!.ForeColor = Me.NameSlot[i]!.ForeColor;
            Me.TextSlot[i - 1]!.Font = new Font(Me.TextSlot[i - 1]!.Font, Me.TextSlot[i]!.Font.Bold ? FontStyle.Bold : FontStyle.Regular);
            Me.TextSlot[i - 1]!.Text = Me.TextSlot[i]!.Text;
            Me.TextSlot[i - 1]!.BackColor = Me.TextSlot[i]!.BackColor;
            Me.TextSlot[i - 1]!.ForeColor = Me.TextSlot[i]!.ForeColor;
        }
    }

    private string RemoveLeadingSpace(string text)
    {
        bool Done = false;

        while (!Done)
        {
            if (text.Length > 0 && text[0] == ' ')
            {
                text = text.Substring(1);
            }
            else
            {
                Done = true;
            }
        }

        return text;
    }

    internal static void Unload()
    {
        var _this = Me!;
        if (_this != null)
            _this.Close();
        Me = null;
    }
}
