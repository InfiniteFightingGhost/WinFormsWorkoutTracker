using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WorkoutTracker.View.Services
{
    public class NavigationService
    {
        private readonly Panel _container;
        private readonly Dictionary<Type, UserControl> _viewCache = new Dictionary<Type, UserControl>();
        private UserControl _currentView;

        public NavigationService(Panel container)
        {
            _container = container;
        }

        public void NavigateTo<T>(params object[] args) where T : UserControl
        {
            var viewType = typeof(T);
            
            if (!_viewCache.TryGetValue(viewType, out var view))
            {
                view = (T)Activator.CreateInstance(viewType, args);
                _viewCache[viewType] = view;
            }

            if (_currentView == view) return;

            _container.Controls.Clear();
            view.Dock = DockStyle.Fill;
            _container.Controls.Add(view);
            _currentView = view;

            if (view is Views.BaseView baseView)
            {
                baseView.OnNavigatedTo();
            }
        }

        public void NavigateTo(UserControl view)
        {
            _container.Controls.Clear();
            view.Dock = DockStyle.Fill;
            _container.Controls.Add(view);
            _currentView = view;

            if (view is Views.BaseView baseView)
            {
                baseView.OnNavigatedTo();
            }
        }
    }
}
