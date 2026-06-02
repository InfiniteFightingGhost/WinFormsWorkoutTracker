using System.Drawing;
using System.Windows.Forms;

namespace WorkoutTracker.RealView.Controls
{
    public class ModernColorTable : ProfessionalColorTable
    {
        // The main background of the dropdown menu
        public override Color ToolStripDropDownBackground => UIStyle.Surface;

        // The single-pixel outer border of the menu
        public override Color MenuBorder => UIStyle.Border;

        // Removes the border around the currently hovered item
        public override Color MenuItemBorder => Color.Transparent;

        // The solid color used when hovering over an item
        public override Color MenuItemSelected => UIStyle.SurfaceVariant;
        public override Color MenuItemSelectedGradientBegin => UIStyle.SurfaceVariant;
        public override Color MenuItemSelectedGradientEnd => UIStyle.SurfaceVariant;

        // Flattens out the left-side margin where icons usually sit
        public override Color ImageMarginGradientBegin => UIStyle.Surface;
        public override Color ImageMarginGradientMiddle => UIStyle.Surface;
        public override Color ImageMarginGradientEnd => UIStyle.Surface;
    }
}
