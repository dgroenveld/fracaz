namespace Fracaz.Helpers;

internal class ToolStripMenuItems
{
    internal static ToolStripMenuItem[] CreateToolStripMenuItemArray(ToolStripMenuItem menuItem, Action<ToolStripMenuItem>? itemClicked = null, int indexModifier = 1)
    {
        //loop through subitems of menuItem. Add each to array. Add event handler to each which will allow only one to be checked at a time.
        ToolStripMenuItem[] toolStripMenuItems = new ToolStripMenuItem[menuItem.DropDownItems.Count + 1];
        for (int i = 0; i < menuItem.DropDownItems.Count; i++)
        {
            toolStripMenuItems[i + indexModifier] = (ToolStripMenuItem)menuItem.DropDownItems[i];
            toolStripMenuItems[i + indexModifier].CheckOnClick = true;
            toolStripMenuItems[i + indexModifier].Click += new EventHandler(ToolStripMenuItem_Click);

            if (itemClicked != null)
            {
                toolStripMenuItems[i + indexModifier].Click += (sender, e) => itemClicked((ToolStripMenuItem)sender!); // Invoke the action when clicked
            }

        }

        return toolStripMenuItems;
    }

    private static void ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        //This method should make sure that all siblings of the clicked item are unchecked.
        ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender!;
        ToolStripMenuItem[] siblings = clickedItem.GetCurrentParent()!.Items.OfType<ToolStripMenuItem>().ToArray();
        foreach (ToolStripMenuItem sibling in siblings)
        {
            if (sibling != clickedItem)
            {
                sibling.Checked = false;
            }
        }
    }
}