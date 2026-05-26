using System.Windows.Forms;

namespace RealView.Views
{
    public class BaseView : UserControl
    {
        public virtual void OnNavigatedTo() { }
        
        protected void ShowLoading(bool show)
        {
            if (this.ParentForm is MainForm shell)
            {
                // shell.SetLoading(show); // Implementation in MainForm
            }
        }
    }
}
