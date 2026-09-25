using System.ComponentModel;
using static Fracaz.Declarations;
using static Fracaz.NetworkPlay;

namespace Fracaz;

public partial class RenameCountry : Form
{
    public RenameCountry()
    {
        InitializeComponent();

        this.RefOldName = this.OldName;
        this.RefNewName = this.NewName;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Label? RefOldName { get; set; }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TextBox? RefNewName { get; set; }

    private void CancelButt_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void OKButt_Click(object sender, EventArgs e)
    {
        // See if our name is valid.
        if (NewName.Text != string.Empty)
        {
            // Assign this name to our country!
            if (LastRightClick > TILEVAL_COASTLINE)
            {
                MyMap!.WaterName(LastRightClick - 1000, NewName.Text);
            }
            else
            {
                MyMap!.CountryName(LastRightClick, NewName.Text);
            }

            Land.Refs.CntryName.Text = NewName.Text;
            // Also send this over the network.
            if (MyNetworkRole != NW_NONE)
            {
                SendRenameToNetwork(LastRightClick, NewName.Text, 0);
            }
            this.Close();
        }
    }
}
