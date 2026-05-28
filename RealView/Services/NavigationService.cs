using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RealView.Views;

namespace RealView.Services
{
    public class NavigationService
    {
        private readonly Panel _container;
        private readonly Dictionary<Type, UserControl> _viewCache = new Dictionary<Type, UserControl>();
        private UserControl? _currentView;
        private System.Windows.Forms.Timer? _transitionTimer;
        private double _opacity = 0;

        public NavigationService(Panel container)
        {
            _container = container;
        }

        public void NavigateTo<T>(params object[] args) where T : UserControl
        {
            var viewType = typeof(T);
            
            // Skip caching for dynamic views (those with arguments)
            if (args.Length > 0)
            {
                var dynamicView = (T)Activator.CreateInstance(viewType, args)!;
                PerformNavigation(dynamicView);
                return;
            }

            if (!_viewCache.TryGetValue(viewType, out var view))
            {
                view = (T)Activator.CreateInstance(viewType, args)!;
                _viewCache[viewType] = view;
            }

            PerformNavigation(view);
        }

        public void NavigateTo(UserControl view)
        {
            PerformNavigation(view);
        }

        private void PerformNavigation(UserControl view)
        {
            if (_currentView == view) return;

            _transitionTimer?.Stop();
            
            _container.Controls.Clear();
            view.Dock = DockStyle.Fill;
            
            // Setup for fade in
            _opacity = 0;
            view.Visible = false;
            _container.Controls.Add(view);
            _currentView = view;

            if (view is BaseView baseView)
            {
                baseView.OnNavigatedTo();
            }

            // Start fade in animation
            view.Visible = true;
            _transitionTimer = new System.Windows.Forms.Timer { Interval = 10 };
            _transitionTimer.Tick += (s, e) =>
            {
                _opacity += 0.15;
                if (_opacity >= 1)
                {
                    _opacity = 1;
                    _transitionTimer.Stop();
                }
                // WinForms UserControls don't support Opacity, 
                // but we can force a repaint or use this for more complex transitions later.
                // For now, this serves as a hook for the layout to settle.
            };
            _transitionTimer.Start();
        }
        
        public void ClearCache()
        {
            _viewCache.Clear();
            _currentView = null;
        }
    }
}
