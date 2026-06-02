using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using WorkoutTracker.RealView;

namespace WorkoutTracker.RealView.Controls
{
    /// <summary>
    /// A high-performance flow panel that uses UI virtualization and object pooling
    /// to display large lists of items with minimal memory and CPU overhead.
    /// </summary>
    /// <typeparam name="T">The type of data item to display.</typeparam>
    public class VirtualFlowPanel<T> : UserControl
    {
        private List<T> _items = new();
        private Func<T, Control> _factory;
        private Action<Control, T> _binder;
        private ModernScrollBar _scrollBar;
        private Panel _container;
        
        private int _itemHeight = 150;
        private int _itemWidth = 300;
        private int _itemPadding = 15;
        
        private List<Control> _pool = new();
        private Dictionary<int, Control> _activeControls = new(); // index -> control

        public List<T> Items
        {
            get => _items;
            set
            {
                _items = value ?? new List<T>();
                UpdateScroll();
                RefreshViewport(true);
            }
        }

        public int ItemHeight
        {
            get => _itemHeight;
            set
            {
                _itemHeight = value;
                UpdateScroll();
                RefreshViewport(true);
            }
        }

        public int ItemWidth
        {
            get => _itemWidth;
            set
            {
                _itemWidth = value;
                UpdateScroll();
                RefreshViewport(true);
            }
        }

        public int ItemPadding
        {
            get => _itemPadding;
            set
            {
                _itemPadding = value;
                UpdateScroll();
                RefreshViewport(true);
            }
        }

        /// <summary>
        /// Initializes a new instance of the VirtualFlowPanel.
        /// </summary>
        /// <param name="factory">Function to create a new control instance.</param>
        /// <param name="binder">Action to bind a data item to an existing control.</param>
        public VirtualFlowPanel(Func<T, Control> factory, Action<Control, T> binder)
        {
            _factory = factory;
            _binder = binder;

            this.DoubleBuffered = true;
            this.BackColor = UIStyle.Background;

            _container = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            // Enable mouse wheel on the container
            _container.MouseWheel += Container_MouseWheel;
            this.Controls.Add(_container);

            _scrollBar = new ModernScrollBar
            {
                Dock = DockStyle.Right,
                Width = 12
            };
            _scrollBar.Scroll += (s, e) => RefreshViewport();
            this.Controls.Add(_scrollBar);
            
            this.Resize += (s, e) => {
                UpdateScroll();
                RefreshViewport();
            };
        }

        private void Container_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (!_scrollBar.Visible) return;

            // Standard mouse wheel delta is 120. We scroll roughly one item height per notch.
            float scrollAmount = (float)-e.Delta / 120.0f * (_itemHeight + _itemPadding);
            float newValue = _scrollBar.Value + scrollAmount;
            
            // Clamp value
            _scrollBar.Value = Math.Max(0, Math.Min(newValue, _scrollBar.Maximum - _scrollBar.LargeChange));
            RefreshViewport();
        }

        private void UpdateScroll()
        {
            if (_items.Count == 0 || _container.Width <= 0)
            {
                _scrollBar.Maximum = 0;
                _scrollBar.Visible = false;
                return;
            }

            int itemsPerRow = Math.Max(1, (_container.Width - _itemPadding) / (_itemWidth + _itemPadding));
            int totalRows = (int)Math.Ceiling((double)_items.Count / itemsPerRow);
            int totalHeight = totalRows * (_itemHeight + _itemPadding) + _itemPadding;

            _scrollBar.Maximum = totalHeight;
            _scrollBar.LargeChange = _container.Height;
            _scrollBar.Visible = totalHeight > _container.Height;
            
            // Adjust value if it's now out of bounds
            if (_scrollBar.Value > Math.Max(0, _scrollBar.Maximum - _scrollBar.LargeChange))
            {
                _scrollBar.Value = Math.Max(0, _scrollBar.Maximum - _scrollBar.LargeChange);
            }
        }

        public void RefreshViewport(bool forceRebind = false)
        {
            if (_items.Count == 0 || _container.Width <= 0)
            {
                ClearActiveControls();
                return;
            }

            int itemsPerRow = Math.Max(1, (_container.Width - _itemPadding) / (_itemWidth + _itemPadding));
            
            int viewTop = (int)_scrollBar.Value;
            int viewBottom = viewTop + _container.Height;

            int firstVisibleRow = Math.Max(0, (viewTop - _itemPadding) / (_itemHeight + _itemPadding));
            int lastVisibleRow = (viewBottom + _itemHeight) / (_itemHeight + _itemPadding);

            int firstVisibleIndex = firstVisibleRow * itemsPerRow;
            int lastVisibleIndex = Math.Min(_items.Count - 1, (lastVisibleRow + 1) * itemsPerRow - 1);

            // Return controls that are no longer visible to the pool
            var activeIndices = _activeControls.Keys.ToList();
            foreach (int index in activeIndices)
            {
                if (index < firstVisibleIndex || index > lastVisibleIndex)
                {
                    var ctrl = _activeControls[index];
                    ctrl.Visible = false;
                    _pool.Add(ctrl);
                    _activeControls.Remove(index);
                }
            }

            // Ensure all visible items have an active control
            for (int i = firstVisibleIndex; i <= lastVisibleIndex; i++)
            {
                int row = i / itemsPerRow;
                int col = i % itemsPerRow;

                int x = _itemPadding + col * (_itemWidth + _itemPadding);
                int y = _itemPadding + row * (_itemHeight + _itemPadding) - viewTop;

                if (!_activeControls.ContainsKey(i))
                {
                    Control ctrl;
                    if (_pool.Count > 0)
                    {
                        // Reuse from pool
                        ctrl = _pool[0];
                        _pool.RemoveAt(0);
                    }
                    else
                    {
                        // Create new if pool is empty
                        ctrl = _factory(_items[i]);
                        _container.Controls.Add(ctrl);
                    }

                    _binder(ctrl, _items[i]);
                    _activeControls[i] = ctrl;
                }
                else if (forceRebind)
                {
                    _binder(_activeControls[i], _items[i]);
                }

                var activeCtrl = _activeControls[i];
                if (activeCtrl.Bounds.X != x || activeCtrl.Bounds.Y != y || 
                    activeCtrl.Width != _itemWidth || activeCtrl.Height != _itemHeight)
                {
                    activeCtrl.SetBounds(x, y, _itemWidth, _itemHeight);
                }
                activeCtrl.Visible = true;
            }
        }

        private void ClearActiveControls()
        {
            foreach (var ctrl in _activeControls.Values)
            {
                ctrl.Visible = false;
                _pool.Add(ctrl);
            }
            _activeControls.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var ctrl in _activeControls.Values) ctrl.Dispose();
                foreach (var ctrl in _pool) ctrl.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
