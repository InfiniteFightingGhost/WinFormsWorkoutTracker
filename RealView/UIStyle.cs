using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealView
{
    public static class UIStyle
    {
        // Colors
        public static readonly Color Primary = Color.FromArgb(0, 95, 184); // Muted Blue
        public static readonly Color PrimaryHover = Color.FromArgb(0, 75, 150); // Darker Blue
        public static readonly Color Background = Color.FromArgb(245, 247, 251);
        public static readonly Color Sidebar = Color.FromArgb(32, 33, 36);
        public static readonly Color SidebarHover = Color.FromArgb(60, 64, 67);
        public static readonly Color Card = Color.White;
        public static readonly Color Border = Color.FromArgb(230, 230, 230);
        public static readonly Color TextPrimary = Color.FromArgb(32, 33, 36);
        public static readonly Color TextSecondary = Color.FromArgb(140, 140, 140);
        public static readonly Color Success = Color.FromArgb(40, 167, 69);
        public static readonly Color Danger = Color.FromArgb(220, 53, 69);

        // Metrics
        public static readonly int BorderRadius = 12;
        public static readonly int Padding = 20;
        public static readonly int TitleBarHeight = 40;

        // Fonts
        private static string _fontFamily = "Segoe UI Variable Display";

        static UIStyle()
        {
            // Fallback if Segoe UI Variable isn't available
            using (Font f = new Font(_fontFamily, 10))
            {
                if (f.Name != _fontFamily)
                {
                    _fontFamily = "Segoe UI";
                }
            }
        }

        public static Font Header = new Font(_fontFamily, 24, FontStyle.Bold);
        public static Font SubHeader = new Font(_fontFamily, 14, FontStyle.Bold);
        public static Font Body = new Font(_fontFamily, 11, FontStyle.Regular);
        public static Font BodySemibold = new Font(_fontFamily, 11, FontStyle.Bold);
        public static Font Caption = new Font(_fontFamily, 9, FontStyle.Regular);
        public static Font CaptionBold = new Font(_fontFamily, 9, FontStyle.Bold);
    }
}
