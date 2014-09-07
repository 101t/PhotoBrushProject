using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DockingLibrary
{
    interface IDropSurface
    {
        Rect SurfaceRectangle { get;}
        void OnDragEnter(Point point);
        void OnDragOver(Point point);
        void OnDragLeave(Point point);
        bool OnDrop(Point point);
    }
}
