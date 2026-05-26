using System.Windows.Forms;

namespace WorkoutTracker.View.Views
{
    public class BaseView : UserControl
    {
        //protected AppRuntime Runtime => null; // Just a placeholder if I were using instances, but AppRuntime is static.
        
        // We can add common methods here, like showing loading indicators.
        public virtual void OnNavigatedTo() { }
    }
}
