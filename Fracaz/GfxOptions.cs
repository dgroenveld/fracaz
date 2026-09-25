using static Fracaz.Boom;
using static Fracaz.Declarations;
using static Fracaz.GraphicFX;

namespace Fracaz;

public partial class GfxOptions : Form
{
    private int TempSpeed;

    public GfxOptions()
    {
        InitializeComponent();
    }

    private void AniSpeed_Change(object sender, EventArgs e)
    {
        Land.Refs.BallTimer.Interval = AniSpeed.Value;
    }

    private void Form_QueryUnload(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            CancelDialog();
        }
    }

    private void CancelDialog()
    {
        // CANCEL!  Put the settings back where they were.
        Land.Refs.BallTimer.Interval = TempSpeed;

        // Clean out the balls and waves arrays.
        ClearAllBalls();
        ClearAllWaves();
    }

    private void OKbutton_Click(object sender, EventArgs e)
    {
        // OK!  Take these settings and record them.

        // AniSpeed is already set.

        // If we disabled explosions, clean out the balls array.
        CFGExplosions = ExplosionsCheckbox.Checked ? 1 : 0;

        if (CFGExplosions == 0) ClearAllBalls();

        // If we disabled waves, clean out the waves array.
        CFGWaves = WavesCheckbox.Checked ? 1 : 0;
        if (CFGWaves == 0) ClearAllWaves();

        // If we disabled flashing, return each country to normal state.
        CFGFlashing = FlashingCheckbox.Checked ? 1 : 0;
        if ((CFGFlashing == 0) && (GameMode == GM_GAME_ACTIVE)) ResetFlashing();

        // Get the prompt check.
        CFGPrompt = PromptCheckbox.Checked ? 1 : 0;

        // Grab the Unoccupied Country color.
        for (var j = 1; j <= 11; j++)
        {
            if (UnOccColor.Text == PlayerColors[j])
            {
                // We have chosen color j for unoccupied countries.
                CFGUnoccupiedColor = j;
            }
        }

        // Set all unoccupied countries to this color if a game is in progress.
        if (GameMode == GM_DIALOG_OPEN) // if (GameMode == GM_GAME_ACTIVE) -- in the original source code, but that doesn't seem to work.
        {
            for (var i = 1; i <= MyMap!.NumberOfCountries; i++)
            {
                if (MyMap.Owner(i) == 0)
                {
                    // Aha!  This one is unowned.
                    MyMap!.CountryColor(i, CFGUnoccupiedColor);
                }
            }

            DrawMap();   // Update the screen immediately.
        }
        else if (GameMode == GM_TITLE_DIALOG_OPEN)
        {
            // Title screen.  Change the color of the Fracas word to the unoccupied color.
            MyMap!.CountryColor(1, CFGUnoccupiedColor);

            DrawMap();
        }

        // Clean out the balls and waves arrays.
        ClearAllBalls();
        ClearAllWaves();

        this.Close();
    }

    private void CANCELbutton_Click(object sender, EventArgs e)
    {
        CancelDialog();

        this.Close();
    }

    private void Form_Load(object sender, EventArgs e)
    {

        // Set up initial positions of controls.
        TempSpeed = Land.Refs.BallTimer.Interval;
        AniSpeed.Value = TempSpeed;
        ExplosionsCheckbox.Checked = CFGExplosions > 0;
        WavesCheckbox.Checked = CFGWaves > 0;
        FlashingCheckbox.Checked = CFGFlashing > 0;
        PromptCheckbox.Checked = CFGPrompt > 0;

        // Set up color dropdown.
        for (var j = 1; j <= 11; j++)       // Not 12 since white is reserved for flashing.
        {
            // Don't add player colors to the dropdown.
            var FoundIt = false;
            // See if this is one of the player colors.
            for (var k = 1; k <= 6; k++)
            {
                if (Player[k] == j) FoundIt = true;
            }
            if (FoundIt == false)
            {
                UnOccColor.Items.Add(PlayerColors[j]);
            }
        }

        UnOccColor.Text = PlayerColors[CFGUnoccupiedColor];

        UpdateColorDropdown();
    }
    private void UpdateColorDropdown()
    {
        // This sub just writes the chose color to the BackColor of the color drop-down.

        for (var j = 1; j <= 12; j++)
        {
            if (UnOccColor.Text == PlayerColors[j])
            {
                UnOccColor.BackColor = Color.FromArgb(PlayerColorCodes[j]);
                UnOccColor.ForeColor = Color.FromArgb(PlayerTextColor[j]);
            }
        }
    }

    private void UnOccColor_Click(object sender, EventArgs e)
    {
        UpdateColorDropdown();

        if (this.Visible)
        {
            OKbutton.Focus();
        }
    }
}
