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

        public event EventHandler<UserControl>? Navigated;

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
            if (_currentView == view && view.Visible) return;

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

            Navigated?.Invoke(this, view);

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
            };
            _transitionTimer.Start();
        }
        
        public void RefreshCurrentView()
        {
            if (_currentView == null) return;

            var viewType = _currentView.GetType();
            
            // Clear cache so it gets recreated with new theme colors
            _viewCache.Clear();

            // Re-instantiate based on type
            // Note: This only works for views with parameterless constructors.
            // For views with parameters, they are usually one-off navigations anyway.
            try
            {
                var newView = (UserControl)Activator.CreateInstance(viewType)!;
                PerformNavigation(newView);
            }
            catch
            {
                // If it fails (e.g. requires params), just re-navigate to Dashboard as fallback
                NavigateTo<DashboardView>();
            }
        }

        public void ClearCache()
        {
            _viewCache.Clear();
            _currentView = null;
        }
    }
}
