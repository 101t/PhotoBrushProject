using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DockingLibrary
{
    /// <summary>
    /// Rappresenta una griglia di oggetti DockPane con al centro un DocumentsPane
    /// </summary>
    /// <remarks>Questa classe mantiene lo stato di ancoraggio di ogni pane, anche quando la finestra
    /// non è visibile perche' ancorata ad un bordo della finestra che ospita il controllo DockManager</remarks>
    class DockingGrid
    {
        IPane _rootPane = null;

        public DockingGrid()
        {
            
        }

        private PaneGroup FindParentGroup(PaneGroup group, IPane pane)
        {
            if (group.First == pane || group.Second == pane)
                return group;

            PaneGroup childGroup = group.First as PaneGroup;
            PaneGroup foundGroup = null;
            if (childGroup != null)
                foundGroup = FindParentGroup(childGroup, pane);
            if (foundGroup != null)
                return foundGroup;

            childGroup = group.Second as PaneGroup;
            if (childGroup != null)
                foundGroup = FindParentGroup(childGroup, pane);
            if (foundGroup != null)
                return foundGroup;

            return null;
        }

        public void Move(Pane source, Pane destination, Dock dock)
        {
            IPane resultPane = Remove(_rootPane, source);
            if (resultPane != null)
                _rootPane = resultPane; 
            
            PaneGroup parentGroup = FindParentGroup(_rootPane as PaneGroup, destination);

            if (parentGroup.First == destination)
            {
                switch (dock)
                {
                    case Dock.Left:
                        parentGroup.First = new PaneGroup(source, destination, SplitOrientation.Vertical);
                        break;
                    case Dock.Right:
                        parentGroup.First = new PaneGroup(destination, source, SplitOrientation.Vertical);
                        break;
                    case Dock.Top:
                        parentGroup.First = new PaneGroup(source, destination, SplitOrientation.Horizontal);
                        break;
                    case Dock.Bottom:
                        parentGroup.First = new PaneGroup(destination, source, SplitOrientation.Horizontal);
                        break;
                }
            }
            else
            {
                switch (dock)
                {
                    case Dock.Left:
                        parentGroup.Second = new PaneGroup(source, destination, SplitOrientation.Vertical);
                        break;
                    case Dock.Right:
                        parentGroup.Second = new PaneGroup(destination, source, SplitOrientation.Vertical);
                        break;
                    case Dock.Top:
                        parentGroup.Second = new PaneGroup(source, destination, SplitOrientation.Horizontal);
                        break;
                    case Dock.Bottom:
                        parentGroup.Second = new PaneGroup(destination, source, SplitOrientation.Horizontal);
                        break;
                }
            }

            if (source is DockablePane)
                (source as DockablePane).Dock = dock;
            
        }

        public void MoveInto(Pane source, Pane destination)
        {
            IPane resultPane = Remove(_rootPane, source);
            if (resultPane != null)
                _rootPane = resultPane;
            
            List<ManagedContent> ar = new List<ManagedContent>();
            foreach (ManagedContent content in source.Contents)
            {
                ar.Add(content);
                destination.Add(content);
            }

            foreach (ManagedContent content in ar)
                source.Remove(content);

        }

        public void ChangeDock(DockablePane pane, Dock dock)
        {
            //if (dock == pane.Dock)
            //    return;

            //rimuovo innanizitutto il pane dalla griglia
            IPane resultPane = Remove(_rootPane, pane);
            if (resultPane != null)
                _rootPane = resultPane;

            pane.Dock = dock;
            //(pane.Parent as Grid).Children.Remove(pane);
            Add(pane);

        }

        private IPane Remove(IPane parent, IPane pane)
        {
            if (parent is PaneGroup)
            {
                PaneGroup group = parent as PaneGroup;
                if (group.First == pane)
                    return group.Second;
                if (group.Second == pane)
                    return group.First;

                IPane resultPane = Remove(group.First, pane);
                if (resultPane != null)
                    group.First = resultPane;
                else
                {
                    resultPane = Remove(group.Second, pane);
                    if (resultPane != null)
                        group.Second = resultPane;
                }
            }

            return null;
        }

        public Pane Add(ManagedContent content, Dock dock)
        {
            DockablePane pane = new DockablePane(content, dock);

            Add(pane);

            return pane;
        }

        public void Add(DockablePane pane)
        {
            switch (pane.Dock)
            {
                case Dock.Right:
                    _rootPane = new PaneGroup(_rootPane, pane, SplitOrientation.Vertical);
                    break;
                case Dock.Left:
                    _rootPane = new PaneGroup(pane, _rootPane, SplitOrientation.Vertical);
                    break;
                case Dock.Bottom:
                    _rootPane = new PaneGroup(_rootPane, pane, SplitOrientation.Horizontal);
                    break;
                case Dock.Top:
                    _rootPane = new PaneGroup(pane, _rootPane, SplitOrientation.Horizontal);
                    break;
            }
        }

        public void SetRoot(DocumentsPane pane)
        {
            if (_rootPane == null)
                _rootPane = pane;
        }

        public void AdjustPanesSize()
        {
            _rootPane.AdjustSize();
        }
        

        /// <summary>
        /// Organizza le righe e colonne di una griglia e dispone in modo opportuno i dockpane e il documentspane
        /// </summary>
        /// <param name="gridToArrange"></param>
        public void Arrange(Grid grid)
        {
            AdjustPanesSize();

            Clear(grid);

            InternalArrange(grid, _rootPane);
        }

        private void InternalArrange(Grid grid, IPane pane)
        {
            if (pane is UIElement)
            {
                grid.Children.Add(pane as UIElement);
            }
            else if (pane is PaneGroup)
            {
                PaneGroup group = pane as PaneGroup;
                if (group.First.Hidden && !group.Second.Hidden)
                    InternalArrange(grid, group.Second);
                else if (!group.First.Hidden && group.Second.Hidden)
                    InternalArrange(grid, group.First);
                else
                {
                    Grid firstGrid = new Grid();
                    Grid secondGrid = new Grid();

                    if (group.Split == SplitOrientation.Horizontal)
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        grid.RowDefinitions.Add(new RowDefinition());
                        grid.RowDefinitions.Add(new RowDefinition());

                        grid.RowDefinitions[0].Height = group.First.GridHeight;
                        grid.RowDefinitions[1].Height = group.Second.GridHeight;

                        //if (!grid.RowDefinitions[0].Height.IsStar &&
                        //    !grid.RowDefinitions[1].Height.IsStar)
                        //    grid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);   
                        


                        firstGrid.SetValue(Grid.ColumnProperty, 0);
                        firstGrid.SetValue(Grid.RowProperty, 0);
                        secondGrid.SetValue(Grid.ColumnProperty, 0);
                        secondGrid.SetValue(Grid.RowProperty, 1);

                        GridSplitter splitter = new GridSplitter();
                        splitter.VerticalAlignment = VerticalAlignment.Top;
                        splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                        splitter.SetValue(Grid.ColumnProperty, 0);
                        splitter.SetValue(Grid.RowProperty, 1);
                        splitter.Height = 5;
                        secondGrid.Margin = new Thickness(0, 5, 0, 0);

                        grid.Children.Add(firstGrid);
                        grid.Children.Add(splitter);
                        grid.Children.Add(secondGrid);

                        InternalArrange(firstGrid, group.First);
                        InternalArrange(secondGrid, group.Second);
                    }
                    else
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        grid.ColumnDefinitions.Add(new ColumnDefinition());
                        grid.RowDefinitions.Add(new RowDefinition());

                        grid.ColumnDefinitions[0].Width = group.First.GridWidth;
                        grid.ColumnDefinitions[1].Width = group.Second.GridWidth;
                        if (!grid.ColumnDefinitions[0].Width.IsStar &&
                            !grid.ColumnDefinitions[1].Width.IsStar)
                            grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);   

                        firstGrid.SetValue(Grid.ColumnProperty, 0);
                        firstGrid.SetValue(Grid.RowProperty, 0);
                        secondGrid.SetValue(Grid.ColumnProperty, 1);
                        secondGrid.SetValue(Grid.RowProperty, 0);

                        GridSplitter splitter = new GridSplitter();
                        splitter.VerticalAlignment = VerticalAlignment.Stretch;
                        splitter.HorizontalAlignment = HorizontalAlignment.Left;
                        splitter.SetValue(Grid.ColumnProperty, 1);
                        splitter.SetValue(Grid.RowProperty, 0);
                        splitter.Width = 5;
                        secondGrid.Margin = new Thickness(5, 0, 0, 0);

                        grid.Children.Add(firstGrid);
                        grid.Children.Add(splitter);
                        grid.Children.Add(secondGrid);

                        InternalArrange(firstGrid, group.First);
                        InternalArrange(secondGrid, group.Second);
                    }
                }
            }
        }

        private void Clear(Grid grid)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Grid)
                    Clear(child as Grid);
            }

            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
        }

        public IPane FindPaneFromContent(ManagedContent content)
        {
            return _rootPane.FindPaneFromContent(content);
        }
    }
}
