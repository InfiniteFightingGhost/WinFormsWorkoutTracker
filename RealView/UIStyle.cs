using System;
using System.Drawing;
using System.Windows.Forms;

namespace WorkoutTracker.RealView
{
    public enum ThemeType
    {
        Light,
        Dark
    }

    public static class UIStyle
    {
        public static event EventHandler? ThemeChanged;
        public static ThemeType CurrentTheme { get; private set; } = ThemeType.Light;

        // Core Palette
        public static Color Primary { get; private set; } = Color.FromArgb(0, 95, 184); 
        public static Color PrimaryHover { get; private set; } = Color.FromArgb(0, 75, 150);
        public static Color Success { get; private set; } = Color.FromArgb(40, 167, 69);
        public static Color Danger { get; private set; } = Color.FromArgb(220, 53, 69);
        public static Color Warning { get; private set; } = Color.FromArgb(255, 193, 7);
        public static Color Info { get; private set; } = Color.FromArgb(0, 123, 255);

        // Backgrounds & Surfaces
        public static Color Background { get; private set; } = Color.FromArgb(245, 247, 251);
        public static Color Surface { get; private set; } = Color.White;
        public static Color SurfaceVariant { get; private set; } = Color.FromArgb(240, 242, 245);
        public static Color Sidebar { get; private set; } = Color.FromArgb(32, 33, 36);
        public static Color SidebarHover { get; private set; } = Color.FromArgb(60, 64, 67);
        
        // Text
        public static Color TextPrimary { get; private set; } = Color.FromArgb(32, 33, 36);
        public static Color TextSecondary { get; private set; } = Color.FromArgb(110, 110, 110);
        public static Color TextTertiary { get; private set; } = Color.FromArgb(160, 160, 160);
        public static Color TextOnPrimary { get; private set; } = Color.White;
        public static Color TextOnSidebar { get; private set; } = Color.White;

        // Interaction & Selection
        public static Color Selection { get; private set; } = Color.FromArgb(235, 245, 255);
        public static Color HoverOverlay { get; private set; } = Color.FromArgb(10, 0, 0, 0);
        
        // Borders
        public static Color Border { get; private set; } = Color.FromArgb(230, 230, 230);
        public static Color BorderLight { get; private set; } = Color.FromArgb(240, 240, 240);

        // Specialized
        public static Color WarmupSet { get; private set; } = Color.DarkGoldenrod;
        public static Color DropSet { get; private set; } = Color.DarkOrchid;
        public static Color ChartBlue { get; private set; } = Color.FromArgb(0, 120, 215);
        public static Color Highlight { get; private set; } = Color.FromArgb(255, 235, 245);
        public static Color HighlightText { get; private set; } = Color.FromArgb(255, 105, 180);

        // Metrics
        public const int BorderRadius = 12;
        public const int Padding = 20;
        public const int TitleBarHeight = 40;
        public const int SidebarWidth = 250;
        public const int SidebarMiniWidth = 80;

        // Fonts
        private static string _fontFamily = "Segoe UI Variable Display";
        public static Font Header { get; private set; } = null!;
        public static Font SubHeader { get; private set; } = null!;
        public static Font Body { get; private set; } = null!;
        public static Font BodySemibold { get; private set; } = null!;
        public static Font Caption { get; private set; } = null!;
        public static Font CaptionBold { get; private set; } = null!;

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

            Header = new Font(_fontFamily, 24, FontStyle.Bold);
            SubHeader = new Font(_fontFamily, 14, FontStyle.Bold);
            Body = new Font(_fontFamily, 11, FontStyle.Regular);
            BodySemibold = new Font(_fontFamily, 11, FontStyle.Bold);
            Caption = new Font(_fontFamily, 9, FontStyle.Regular);
            CaptionBold = new Font(_fontFamily, 9, FontStyle.Bold);

            SetTheme(ThemeType.Light);
        }

        public static void SetTheme(ThemeType theme)
        {
            CurrentTheme = theme;

            if (theme == ThemeType.Light)
            {
                Background = Color.FromArgb(245, 247, 251);
                Surface = Color.White;
                SurfaceVariant = Color.FromArgb(240, 242, 245);
                Sidebar = Color.FromArgb(32, 33, 36);
                SidebarHover = Color.FromArgb(60, 64, 67);
                TextPrimary = Color.FromArgb(32, 33, 36);
                TextSecondary = Color.FromArgb(110, 110, 110);
                TextTertiary = Color.FromArgb(160, 160, 160);
                Selection = Color.FromArgb(235, 245, 255);
                Border = Color.FromArgb(230, 230, 230);
                BorderLight = Color.FromArgb(240, 240, 240);
                Highlight = Color.FromArgb(255, 235, 245);
                HighlightText = Color.FromArgb(255, 105, 180);
            }
            else
            {
                Background = Color.FromArgb(18, 18, 18);
                Surface = Color.FromArgb(30, 30, 30);
                SurfaceVariant = Color.FromArgb(45, 45, 45);
                Sidebar = Color.FromArgb(24, 24, 24);
                SidebarHover = Color.FromArgb(50, 50, 50);
                TextPrimary = Color.FromArgb(230, 230, 230);
                TextSecondary = Color.FromArgb(170, 170, 170);
                TextTertiary = Color.FromArgb(110, 110, 110);
                Selection = Color.FromArgb(40, 60, 80);
                Border = Color.FromArgb(60, 60, 60);
                BorderLight = Color.FromArgb(40, 40, 40);
                Highlight = Color.FromArgb(60, 30, 45);
                HighlightText = Color.FromArgb(255, 150, 200);
            }

            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
