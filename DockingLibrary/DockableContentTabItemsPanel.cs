using System.Windows;

namespace DockingLibrary
{
    class DockableContentTabItemsPanel : System.Windows.Controls.Panel
    {
        double totChildWidth = 0.0;

        protected override Size MeasureOverride(Size availableSize)
        {
            Size childSize = availableSize;
            Size totalDesideredSize = new Size(availableSize.Width, 0.0);
            totChildWidth = 0.0;
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(childSize);

                totChildWidth += child.DesiredSize.Width;
                if (totalDesideredSize.Height < child.DesiredSize.Height)
                    totalDesideredSize.Height = child.DesiredSize.Height;
            }

            return totalDesideredSize;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            Size inflate = new Size();

            if (finalSize.Width < totChildWidth)
                inflate.Width = (totChildWidth - finalSize.Width) / InternalChildren.Count;
  
            Point offset = new Point();
            if (finalSize.Width > totChildWidth)
                offset.X = -(finalSize.Width - totChildWidth)/2;

            double totalFinalWidth = 0.0;
            foreach (UIElement child in InternalChildren)
            {
                Size childFinalSize = child.DesiredSize;
                childFinalSize.Width -= inflate.Width;
                childFinalSize.Height = finalSize.Height;

                child.Arrange(new Rect(offset, childFinalSize));

                offset.Offset(childFinalSize.Width, 0);
                totalFinalWidth += childFinalSize.Width;
            }

            return new Size(totalFinalWidth, finalSize.Height);
        }



    }
}
