using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealView.Controls
{
    public class ModernColorTable : ProfessionalColorTable
    {
        // The main background of the dropdown menu
        public override Color ToolStripDropDownBackground => Color.White;

        // The single-pixel outer border of the menu
        public override Color MenuBorder => Color.FromArgb(210, 214, 219);

        // Removes the border around the currently hovered item
        public override Color MenuItemBorder => Color.Transparent;

        // The solid color used when hovering over an item
        public override Color MenuItemSelected => Color.FromArgb(240, 242, 245);
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(240, 242, 245);
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(240, 242, 245);

        // Flattens out the left-side margin where icons usually sit
        public override Color ImageMarginGradientBegin => Color.White;
        public override Color ImageMarginGradientMiddle => Color.White;
        public override Color ImageMarginGradientEnd => Color.White;
    }
}
