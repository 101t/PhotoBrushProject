using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace DockingLibrary
{
    /// <summary>
    /// How group panes are splitted
    /// </summary>
    public enum SplitOrientation
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Group of one or more child groups
    /// </summary>
    class DockablePaneGroup : ILayoutSerializable
    {
        Pane _attachedPane;
 
        /// <summary>
        /// Pane directly attached
        /// </summary>
        public Pane AttachedPane {get{return _attachedPane;}}

        DockablePaneGroup _firstChildGroup;

        public DockablePaneGroup FirstChildGroup
        {
            get { return _firstChildGroup; }
            set
            {
                _firstChildGroup = value;
                value._parentGroup = this;
            }
        }

        DockablePaneGroup _secondChildGroup;

        public DockablePaneGroup SecondChildGroup
        {
            get { return _secondChildGroup; }
            set
            {
                _secondChildGroup = value;
                value._parentGroup = this;
            }
        }


        DockablePaneGroup _parentGroup;

        public DockablePaneGroup ParentGroup
        {
            get { return _parentGroup; }
            internal set 
            {
                _parentGroup = value;
            }

        }

        Dock _dock;

        public Dock Dock
        {
            get { return _dock; }
        }

        /// <summary>
        /// Needed only for deserialization
        /// </summary>
        public DockablePaneGroup()
        { }

        /// <summary>
        /// Create a group with a single pane
        /// </summary>
        /// <param name="pane">Attached pane</param>
        public DockablePaneGroup(Pane pane)
        {
            _attachedPane = pane;
        }

        /// <summary>
        /// Create a group with no panes
        /// </summary>
        public DockablePaneGroup(DockablePaneGroup firstGroup, DockablePaneGroup secondGroup, Dock groupDock)
        {
            FirstChildGroup = firstGroup;
            SecondChildGroup = secondGroup;
            _dock = groupDock;
        }

        public Pane GetPaneFromContent(DockableContent content)
        {
            if (AttachedPane != null && AttachedPane.Contents.Contains(content))
                return AttachedPane;

            if (FirstChildGroup != null)
            {
                Pane pane = FirstChildGroup.GetPaneFromContent(content);
                if (pane != null)
                    return pane;
            }

            if (SecondChildGroup != null)
                return SecondChildGroup.GetPaneFromContent(content);

            return null;
        }

        bool IsHidden
        {
            get 
            {
                if (AttachedPane != null)
                    return AttachedPane.IsHidden;

                return FirstChildGroup.IsHidden && SecondChildGroup.IsHidden;
            }
        }

        GridLength GroupWidth
        {
            get
            {
                if (AttachedPane != null)
                    return new GridLength(AttachedPane.PaneWidth, GridUnitType.Pixel);
                else
                {
                    if (Dock == Dock.Left || Dock == Dock.Right)
                        return new GridLength(FirstChildGroup.GroupWidth.Value+SecondChildGroup.GroupWidth.Value+4, GridUnitType.Pixel);
                    else
                        return FirstChildGroup.GroupWidth;
                }
            }
        }

        GridLength GroupHeight
        {
            get
            {
                if (AttachedPane != null)
                    return new GridLength(AttachedPane.PaneHeight, GridUnitType.Pixel);
                else
                {
                    if (Dock == Dock.Top || Dock == Dock.Bottom)
                        return new GridLength(FirstChildGroup.GroupHeight.Value + SecondChildGroup.GroupHeight.Value + 4, GridUnitType.Pixel);
                    else
                        return FirstChildGroup.GroupHeight;
                }
            }
        }

        public void Arrange(Grid grid)
        {
            if (AttachedPane != null)//AttachedPane.IsHidden)
                grid.Children.Add(AttachedPane);
            else if (FirstChildGroup.IsHidden && !SecondChildGroup.IsHidden)
                SecondChildGroup.Arrange(grid);
            else if (!FirstChildGroup.IsHidden && SecondChildGroup.IsHidden)
                FirstChildGroup.Arrange(grid);
            else
            {
                if (Dock == Dock.Left || Dock == Dock.Right)
                {
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    //grid.ColumnDefinitions[0].Width = (Dock == Dock.Left) ? new GridLength(AttachedPane.PaneWidth) : new GridLength(1, GridUnitType.Star);
                    //grid.ColumnDefinitions[1].Width = (Dock == Dock.Right) ? new GridLength(AttachedPane.PaneWidth) : new GridLength(1, GridUnitType.Star);
                    grid.ColumnDefinitions[0].Width = (Dock == Dock.Left) ? FirstChildGroup.GroupWidth : new GridLength(1, GridUnitType.Star);
                    grid.ColumnDefinitions[1].Width = (Dock == Dock.Right) ? SecondChildGroup.GroupWidth : new GridLength(1, GridUnitType.Star);
                    
                    //grid.ColumnDefinitions[0].MinWidth = 50;
                    //grid.ColumnDefinitions[1].MinWidth = 50;


                    Grid firstChildGrid = new Grid();
                    firstChildGrid.SetValue(Grid.ColumnProperty, 0);
                    firstChildGrid.Margin = new Thickness(0, 0, 4, 0);
                    FirstChildGroup.Arrange(firstChildGrid);
                    grid.Children.Add(firstChildGrid);

                    Grid secondChildGrid = new Grid();
                    secondChildGrid.SetValue(Grid.ColumnProperty, 1);
                    //secondChildGrid.Margin = (Dock == Dock.Right) ? new Thickness(0, 0, 4, 0) : new Thickness();
                    SecondChildGroup.Arrange(secondChildGrid);
                    grid.Children.Add(secondChildGrid);

                    //AttachedPane.SetValue(Grid.ColumnProperty, (Dock == Dock.Right) ? 1 : 0);
                    //AttachedPane.Margin = (Dock == Dock.Left) ? new Thickness(0, 0, 4, 0) : new Thickness();
                    //grid.Children.Add(AttachedPane);

                    GridSplitter splitter = new GridSplitter();
                    splitter.Width = 4;
                    splitter.HorizontalAlignment = HorizontalAlignment.Right;
                    splitter.VerticalAlignment = VerticalAlignment.Stretch;
                    grid.Children.Add(splitter);
                }
                else // if (Dock == Dock.Top || Dock == Dock.Bottom)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    //grid.RowDefinitions[0].Height = (Dock == Dock.Top) ? new GridLength(AttachedPane.PaneHeight) : new GridLength(1, GridUnitType.Star);
                    //grid.RowDefinitions[1].Height = (Dock == Dock.Bottom) ? new GridLength(AttachedPane.PaneHeight) : new GridLength(1, GridUnitType.Star);
                    grid.RowDefinitions[0].Height = (Dock == Dock.Top) ? FirstChildGroup.GroupHeight : new GridLength(1, GridUnitType.Star);
                    grid.RowDefinitions[1].Height = (Dock == Dock.Bottom) ? SecondChildGroup.GroupHeight : new GridLength(1, GridUnitType.Star);
                    
                    grid.RowDefinitions[0].MinHeight = 50;
                    grid.RowDefinitions[1].MinHeight = 50;

                    Grid firstChildGrid = new Grid();
                    //firstChildGrid.SetValue(Grid.RowProperty, (Dock == Dock.Bottom) ? 1 : 0);
                    firstChildGrid.SetValue(Grid.RowProperty, 0);
                    //firstChildGrid.Margin = (Dock == Dock.Bottom) ? new Thickness(0, 0, 0, 4) : new Thickness();
                    firstChildGrid.Margin = new Thickness(0, 0, 0, 4);
                    FirstChildGroup.Arrange(firstChildGrid);
                    grid.Children.Add(firstChildGrid);

                    Grid secondChildGrid = new Grid();
                    //secondChildGrid.SetValue(Grid.RowProperty, (Dock == Dock.Top) ? 1 : 0);
                    secondChildGrid.SetValue(Grid.RowProperty, 1);
                    //secondChildGrid.Margin = (Dock == Dock.Bottom) ? new Thickness(0, 0, 0, 4) : new Thickness();
                    SecondChildGroup.Arrange(secondChildGrid);
                    grid.Children.Add(secondChildGrid);

                    //AttachedPane.SetValue(Grid.RowProperty, (Dock == Dock.Bottom) ? 1 : 0);
                    //AttachedPane.Margin = (Dock == Dock.Top) ? new Thickness(0, 0, 0, 4) : new Thickness();
                    //grid.Children.Add(AttachedPane);

                    GridSplitter splitter = new GridSplitter();
                    splitter.Height = 4;
                    splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    splitter.VerticalAlignment = VerticalAlignment.Bottom;
                    grid.Children.Add(splitter);
                }
            }
        }

        ///// <summary>
        ///// Arrange passed grid layout
        ///// </summary>
        ///// <param name="grid">Grid to arrange</param>
        ///// <param name="addPaneToGrid">If true add atteched panes to the grid</param>
        ///// <remarks>Setting <paramref name="addToPaneToGrid"/> to false, this functions only arrange grids layout, without 
        ///// appending attached pane to grid children collection. This is useful for dragging operations.</remarks>
        //public void Arrange(Grid grid, bool addPaneToGrid)
        //{
        //    if (AttachedPane != null)
        //    {
        //        if (addPaneToGrid)
        //            grid.Children.Add(AttachedPane);
        //    }
        //    else
        //    {
        //        if (Group1.IsHidden && !Group2.IsHidden)
        //            Group2.Arrange(grid, addPaneToGrid);
        //        else if (!Group1.IsHidden && Group2.IsHidden)
        //            Group1.Arrange(grid, addPaneToGrid);
        //        else
        //        {
        //                //first child grid
        //                Grid grid1 = new Grid();
        //                //..and second one
        //                Grid grid2 = new Grid();                    
                
        //            #region Vertical orientation
        //            if (SplitOrientation == SplitOrientation.Vertical)
        //            {
        //                 //only one row
        //                grid.RowDefinitions.Add(new RowDefinition());
        //                //two cols
        //                grid.ColumnDefinitions.Add(new ColumnDefinition());
        //                grid.ColumnDefinitions.Add(new ColumnDefinition());

        //                //setup widths
        //                grid.ColumnDefinitions[0].MinWidth = 50;
        //                grid.ColumnDefinitions[0].Width = Group1.GridWidth;
        //                grid.ColumnDefinitions[1].MinWidth = 50;
        //                grid.ColumnDefinitions[1].Width = Group2.GridWidth;

        //                //ensure that at least one col has star length
        //                if (!grid.ColumnDefinitions[0].Width.IsStar &&
        //                    !grid.ColumnDefinitions[1].Width.IsStar)
        //                    grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

        //                grid1.SetValue(Grid.ColumnProperty, 0);
        //                grid2.SetValue(Grid.ColumnProperty, 1);

        //                GridSplitter splitter = new GridSplitter();
        //                splitter.VerticalAlignment = VerticalAlignment.Stretch;
        //                splitter.HorizontalAlignment = HorizontalAlignment.Left;
        //                splitter.SetValue(Grid.ColumnProperty, 1);
        //                splitter.SetValue(Grid.RowProperty, 0);
        //                splitter.Width = 5;
        //                //make room for the splitter
        //                grid2.Margin = new Thickness(5,0,0,0);

        //                //ok, now add child grids and a splitter between them to current grid
        //                grid.Children.Add(grid1);
        //                grid.Children.Add(splitter);
        //                grid.Children.Add(grid2);

        //                //finally arrange child grids
        //                Group1.Arrange(grid1, addPaneToGrid);
        //                Group2.Arrange(grid2, addPaneToGrid);
        //            }
        //            #endregion

        //            #region Horizontal Orientation
        //            else //if (SplitOrientation == SplitOrientation.Horizontal)
        //            {
        //            }
        //            #endregion
        //        }
        //    }

        //}

        //DockPanel GetChildElement(DockablePaneGroup group, Dock dock)
        //{
        //    DockPanel childPanel = new DockPanel();
        //    childPanel.SetValue(DockPanel.DockProperty, dock);
            
        //    if (SplitOrientation == SplitOrientation.Vertical)
        //        childPanel.Width = group.DockPanelWidth;
        //    else
        //        childPanel.Height = group.DockPanelHeight;

        //    group.Arrange(childPanel);

        //    //childPanels.Add(childPanel);
        //    return childPanel;
        //}

        //internal void Arrange(DockPanel panel)
        //{

        //    if (AttachedPane != null)
        //    {
        //        AttachedPane.AttachPanel(panel);
        //        panel.Children.Add(AttachedPane);
        //    }
        //    else
        //    {
        //        #region Vertical split
        //        if (SplitOrientation == SplitOrientation.Vertical)
        //        {
        //            DockPanel lastPanel = null;
                    
        //            foreach (DockablePaneGroup group in ChildGroups)
        //            {
        //                if (group.IsHidden)
        //                    continue;

        //                if (double.IsNaN(group.DockPanelWidth))
        //                {
        //                    lastPanel = GetChildElement(group, Dock.Left);
        //                }
        //                else
        //                {
        //                    DockPanel panelToAdd = GetChildElement(group, lastPanel == null ? Dock.Left : Dock.Right);
        //                    panel.Children.Add(panelToAdd);
        //                }
        //            }

        //            if (lastPanel != null)
        //                panel.Children.Add(lastPanel);

        //            Dock currentDock = Dock.Left;

        //            for (int i = 0; i < panel.Children.Count-1; i++)
        //            {
        //                currentDock = (Dock)panel.Children[i].GetValue(DockPanel.DockProperty);
        //                DockPanel prevPanel = panel.Children[i] as DockPanel;
        //                DockPanel nextPanel = null;
        //                for (int j = i+1; j < panel.Children.Count; j++)
        //                    if ((Dock)panel.Children[j].GetValue(DockPanel.DockProperty) ==
        //                        currentDock)
        //                    {
        //                        nextPanel = panel.Children[j] as DockPanel;
        //                        break;
        //                    }
        //                if (nextPanel == null)
        //                    nextPanel = panel.Children[panel.Children.Count - 1] as DockPanel;

        //                DockPanelSplitter splitter = null;
                        
        //                if (currentDock == Dock.Left)
        //                    splitter = new DockPanelSplitter(prevPanel, nextPanel, SplitOrientation.Vertical);
        //                else
        //                    splitter = new DockPanelSplitter(nextPanel, prevPanel, SplitOrientation.Vertical);
        //                splitter.SetValue(DockPanel.DockProperty, currentDock);
                        
        //                i++;
        //                panel.Children.Insert(i, splitter);
        //            }
        //        }
        //        #endregion
        //        #region Horizontal split
        //        else //if (SplitOrientation == SplitOrientation.Vertical)
        //        {
        //            DockPanel lastPanel = null;

        //            foreach (DockablePaneGroup group in ChildGroups)
        //            {
        //                if (group.IsHidden)
        //                    continue;

        //                if (double.IsNaN(group.DockPanelWidth))
        //                {
        //                    lastPanel = GetChildElement(group, Dock.Top);
        //                }
        //                else
        //                {
        //                    DockPanel panelToAdd = GetChildElement(group, lastPanel == null ? Dock.Top : Dock.Bottom);
        //                    panel.Children.Add(panelToAdd);
        //                }
        //            }

        //            if (lastPanel != null)
        //                panel.Children.Add(lastPanel);

        //            Dock currentDock = Dock.Top;

        //            for (int i = 0; i < panel.Children.Count - 1; i++)
        //            {
        //                currentDock = (Dock)panel.Children[i].GetValue(DockPanel.DockProperty);
        //                DockPanel prevPanel = panel.Children[i] as DockPanel;
        //                DockPanel nextPanel = null;
        //                for (int j = i + 1; j < panel.Children.Count; j++)
        //                    if ((Dock)panel.Children[j].GetValue(DockPanel.DockProperty) ==
        //                        currentDock)
        //                    {
        //                        nextPanel = panel.Children[j] as DockPanel;
        //                        break;
        //                    }
        //                if (nextPanel == null)
        //                    nextPanel = panel.Children[panel.Children.Count - 1] as DockPanel;

        //                DockPanelSplitter splitter = null;

        //                if (currentDock == Dock.Top)
        //                    splitter = new DockPanelSplitter(prevPanel, nextPanel, SplitOrientation.Horizontal);
        //                else
        //                    splitter = new DockPanelSplitter(nextPanel, prevPanel, SplitOrientation.Horizontal);
        //                splitter.SetValue(DockPanel.DockProperty, currentDock);

        //                i++;
        //                panel.Children.Insert(i, splitter);
        //            }
        //        }
        //        #endregion
        //    }
        
        //}

        public void ReplaceChildGroup(DockablePaneGroup groupToFind, DockablePaneGroup groupToReplace)
        {
            if (FirstChildGroup == groupToFind)
                FirstChildGroup = groupToReplace;
            else if (SecondChildGroup == groupToFind)
                SecondChildGroup = groupToReplace;
            else
            {
                System.Diagnostics.Debug.Assert(false);
            }
        }

        //public void SaveChildPanesSize()
        //{
        //    if (AttachedPane != null && ParentGroup!=null)
        //        AttachedPane.SaveSize(ParentGroup.Dock);
        //    else
        //    {
        //        FirstChildGroup.SaveChildPanesSize();
        //        SecondChildGroup.SaveChildPanesSize();
        //    }
                
        //}

        public DockablePaneGroup AddPane(DockablePane pane)
        {
            switch (pane.Dock)
            {
                case Dock.Right:
                case Dock.Bottom:
                    return new DockablePaneGroup(this, new DockablePaneGroup(pane), pane.Dock);
                    
                case Dock.Left:
                case Dock.Top:
                    return new DockablePaneGroup(new DockablePaneGroup(pane), this, pane.Dock);
            }

            return null;
            //DockablePaneGroup resGroup = null;

            //if (AttachedPane != null)
            //{
            //    DockablePaneGroup newChildGroup = new DockablePaneGroup(AttachedPane);
            //    switch (pane.Dock)
            //    {
            //        case Dock.Left:
            //            resGroup = new DockablePaneGroup(new DockablePaneGroup(pane), newChildGroup, SplitOrientation.Vertical);
            //            break;
            //        case Dock.Right:
            //            resGroup = new DockablePaneGroup(newChildGroup, new DockablePaneGroup(pane), SplitOrientation.Vertical);
            //            break;
            //        case Dock.Top:
            //            resGroup = new DockablePaneGroup(new DockablePaneGroup(pane), newChildGroup, SplitOrientation.Horizontal);
            //            break;
            //        case Dock.Bottom:
            //            resGroup = new DockablePaneGroup(newChildGroup, new DockablePaneGroup(pane), SplitOrientation.Horizontal);
            //            break;
            //    }
            //}
            //else
            //{
            //    if (SplitOrientation == SplitOrientation.Vertical)
            //    {
            //        if (pane.Dock == Dock.Left)
            //        {
            //            ChildGroups.Insert(0, new DockablePaneGroup(pane));
            //            resGroup = this;
            //        }
            //        else if (pane.Dock == Dock.Right)
            //        {
            //            int index = 0; 
            //            for (int i = 0; i < ChildGroups.Count;i++)
            //                if (ChildGroups[i].do
                            
            //            ChildGroups.Add(new DockablePaneGroup(pane));
            //            resGroup = this;
            //        }
            //        else if (pane.Dock == Dock.Bottom)
            //            resGroup = new DockablePaneGroup(this, new DockablePaneGroup(pane), SplitOrientation.Horizontal);
            //        else if (pane.Dock == Dock.Top)
            //            resGroup = new DockablePaneGroup(new DockablePaneGroup(pane), this, SplitOrientation.Horizontal);
            //    }
            //    else //if (SplitOrientation == SplitOrientation.Horizontal)
            //    {
            //        if (pane.Dock == Dock.Top)
            //        {
            //            ChildGroups.Insert(0, new DockablePaneGroup(pane));
            //            resGroup = this;
            //        }
            //        else if (pane.Dock == Dock.Bottom)
            //        {
            //            ChildGroups.Add(new DockablePaneGroup(pane));
            //            resGroup = this;
            //        }
            //        else if (pane.Dock == Dock.Right)
            //            resGroup = new DockablePaneGroup(this, new DockablePaneGroup(pane), SplitOrientation.Vertical);
            //        else if (pane.Dock == Dock.Left)
            //            resGroup = new DockablePaneGroup(new DockablePaneGroup(pane), this, SplitOrientation.Vertical);
            //    }
            //}
            
            //return resGroup;
        }

  
        public DockablePaneGroup RemovePane(DockablePane pane)
        {
            if (AttachedPane != null)
                return null;

            if (FirstChildGroup.AttachedPane==pane)
            {
                return SecondChildGroup;
            }
            else if (SecondChildGroup.AttachedPane==pane)
            {
                return FirstChildGroup;
            }
            else
            {
                DockablePaneGroup group = FirstChildGroup.RemovePane(pane);

                if (group != null)
                {
                    FirstChildGroup = group;
                    group._parentGroup = this;
                    return null;
                }


                group = SecondChildGroup.RemovePane(pane);

                if (group != null)
                {
                    SecondChildGroup = group;
                    group._parentGroup = this;
                    return null;
                }
            }

            return null;
        }

        public DockablePaneGroup GetPaneGroup(Pane pane)
        {
            if (AttachedPane == pane)
                return this;
            
            if (FirstChildGroup != null)
            {
                DockablePaneGroup paneGroup = FirstChildGroup.GetPaneGroup(pane);
                if (paneGroup!=null)
                    return paneGroup;
            }
            if (SecondChildGroup != null)
            {
                DockablePaneGroup paneGroup = SecondChildGroup.GetPaneGroup(pane);
                if (paneGroup!=null)
                    return paneGroup;
            }

            return null;
        }


        #region ILayoutSerializable Membri di

        public void Serialize(XmlDocument doc, XmlNode parentNode)
        {
            parentNode.Attributes.Append(doc.CreateAttribute("Dock"));
            parentNode.Attributes["Dock"].Value = _dock.ToString();

            if (AttachedPane != null)
            {
                XmlNode nodeAttachedPane = null;

                if (AttachedPane is DockablePane)
                    nodeAttachedPane = doc.CreateElement("DockablePane");
                else if (AttachedPane is DocumentsPane)
                    nodeAttachedPane = doc.CreateElement("DocumentsPane");

                AttachedPane.Serialize(doc, nodeAttachedPane);

                parentNode.AppendChild(nodeAttachedPane);
            }
            else
            {
                XmlNode nodeChildGroups = doc.CreateElement("ChildGroups");

                XmlNode nodeFirstChildGroup = doc.CreateElement("FirstChildGroup");
                FirstChildGroup.Serialize(doc, nodeFirstChildGroup);
                nodeChildGroups.AppendChild(nodeFirstChildGroup);

                XmlNode nodeSecondChildGroup = doc.CreateElement("SecondChildGroup");
                SecondChildGroup.Serialize(doc, nodeSecondChildGroup);
                nodeChildGroups.AppendChild(nodeSecondChildGroup);

                parentNode.AppendChild(nodeChildGroups);
            }
        }

        public void Deserialize(DockManager managerToAttach, System.Xml.XmlNode node, GetContentFromTypeString getObjectHandler)
        {
            _dock = (Dock)Enum.Parse(typeof(Dock), node.Attributes["Dock"].Value);

            if (node.ChildNodes[0].Name == "DockablePane")
            {
                DockablePane pane = new DockablePane(managerToAttach);
                pane.Deserialize(managerToAttach, node.ChildNodes[0], getObjectHandler);
                _attachedPane = pane;
            }
            else if (node.ChildNodes[0].Name == "DocumentsPane")
            {
                DocumentsPane pane = managerToAttach.GetDocumentsPane();
                pane.Deserialize(managerToAttach, node.ChildNodes[0], getObjectHandler);
                _attachedPane = pane;
            }
            else
            {
                _firstChildGroup = new DockablePaneGroup();
                _firstChildGroup._parentGroup = this;
                _firstChildGroup.Deserialize(managerToAttach, node.ChildNodes[0].ChildNodes[0], getObjectHandler);

                _secondChildGroup = new DockablePaneGroup();
                _secondChildGroup._parentGroup = this;
                _secondChildGroup.Deserialize(managerToAttach, node.ChildNodes[0].ChildNodes[1], getObjectHandler);


            }
        }

        #endregion
    }
}
