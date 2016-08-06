using System.Windows.Forms;

namespace JG.Forms.Controls
{
    // This class extends ToolStripSystemRenderer in order to remove the border it adds to ToolStrips.
    // To use, simply set the Renderer property of a ToolStrip to a new ToolStripSystemRendererEx().
    public class ToolStripSystemRendererEx : ToolStripSystemRenderer
    {
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
        }
    }
}