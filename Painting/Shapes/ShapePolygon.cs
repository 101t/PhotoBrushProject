////////////////////////////////////////////////////////////////
//                Created By Richard Blythe 2008
//   There are no licenses or warranty attached to this object.
//   If you distribute the code as part of your application, please
//   be courteous enough to mention the assistance provided you.
////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Painting.Painter;
using System.Drawing.Drawing2D;

namespace Painting.Shapes
{
    [Serializable]
    public class ShapePolygon : Shape, IDisposable
    {
        //************  NOTE1 ******************************
        //There is a quirk in the construction of a graphics
        //path object that must be compensated for.
        //When a beizer curve is created from the last anchor
        //point, the next node in line is the starting node
        //which must remain as type: 0.  However the beizer
        //must have three types of type: 3 in order to complete
        //the graphicsPath.  This will cause two points to be
        //created with the same coordinates.  When moving one
        //of these points, the other must be moved also.
        //***************************************************

        const byte STARTPOINT = 0;
        const byte LINE = 1;
        const byte BEZIER = 3;
        const byte BEZIER_END = 131;

        List<PointF> points = new List<PointF>(20);
        List<byte> types = new List<byte>(20);
        [NonSerialized]
        List<short> selectedNodes = new List<short>(8);
        List<bool> isAnchor = new List<bool>(20);
        //List<bool> isVisible
        [NonSerialized]
        byte beizerCounter = 0;
        [NonSerialized]
        bool isLeftAnchor;
        bool hasDragged;

        //**  used in for loops **
        [NonSerialized]
        int i = 0; 
        [NonSerialized]
        int intCount = 0;
        Rectangle rectNode = new Rectangle(0, 0, 6, 6);


        /// <summary>
        /// This pen is only used to hit test the outline.
        /// It is not used in any drawing operations.
        /// </summary>
        [NonSerialized]
        Pen hitTestPen;
        [NonSerialized]
        private bool isInitializing;
        public bool IsInitializing
        {
            get { return isInitializing; }
        }

        public ShapePolygon() :base(0)
        {
            //init the shape
            painter = new Painters(new Font("Times New Roman", 10), 2);
            painter.State.AcceptsEditing = true;
            hitTestPen = new Pen(Color.Black, 2);
            editingOn = true;
            painter.State.IsEditing = true;
            isInitializing = true;
        }

        public override bool HasNodes
        {
            get { return true; }
        }

        protected override void SetProperties()
        { /*No properties to set*/}


        protected override void GeneratePath()
        {
            if (points.Count == 0)
            {
                //init the polygon, but with coords tha won't be visible right now.
                _Path = new System.Drawing.Drawing2D.GraphicsPath();
                _Path.AddLine(-1000, -1000, -1001, -1001);
            }
            else if (points.Count <= 2)
            {
                //if the user has established one node, write a dummy command
                //so the first node will be rendered to the user.
                _Path = new System.Drawing.Drawing2D.GraphicsPath();
                _Path.AddLine(points[0], new Point((int)(points[points.Count - 1].X + 1), (int)points[points.Count - 1].Y));
            }
           else
            {
                //refresh the path with the updated points and types
                _Path = new GraphicsPath(points.ToArray(), types.ToArray());
                _Path.CloseAllFigures();
                InvalidationArea = Rectangle.Round(_Path.GetBounds());    
            }

        }
        protected override void GeneratePath(Rectangle bounds)
        {
            //is used when resizing
            _Path = new GraphicsPath();
            _Path.AddRectangle(bounds);
        }

        public override EventData MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            //reset variables
            beizerCounter = 0;
            eventData.NeedsPainted = false;
            eventData.FinalizeShape = false;
            
            //if the shape is being created...
            if (this.isInitializing)
            {
                this.AddPoint(e.Location);
                eventData.NeedsPainted = true;
                eventData.WasHit = true;
            }
            //if the shape has been double clicked..
            else if (editingOn && e.Clicks > 1 
                     && Path.IsOutlineVisible(e.Location,hitTestPen))
            {
                this.AddPoint(e.Location);
                eventData.NeedsPainted = true;
                eventData.WasHit = true;
            }
            else
            {
               
                if ((isSelected || editingOn) && InvalidationArea.Contains(e.Location))
                    eventData.WasHit= true;
                else //if (!isEditing)
                    eventData.WasHit = Path.IsVisible(e.Location);

                if (eventData.WasHit)
                {
                    mouseOffset = new PointF(Path.GetBounds().X-e.X  , Path.GetBounds().Y - e.Y);
                    pntMoveStartPos = Point.Round(Path.GetBounds().Location);
                    selectedNodes.Clear();
                    isSelected = true;
                    painter.State.IsSelected = true;
                    rectOldBounds = GetShapeBounds(true);

                    #region Polygon Editing Code
                    //**********************************************
                    //******    POLYGON EDITING CODE     ***********
                    if (!editingOn)
                    {

                        isSelected = true;
                        isResizing = (ResizeHandles.HitTest(e.Location));
                        painter.State.IsSelected = true;
                        painter.State.IsResizing = isResizing;
                    }
                    else
                    {
                        RectangleF rectF = new RectangleF(0, 0, 6, 6);
                        int intNum = -1;
                        short intCount = (short)points.Count;
                        for (short i = 0; i < intCount; i++)
                        {
                            if (isBeizer(types[i]))
                            {
                                beizerCounter++;
                                if (beizerCounter == BEZIER)
                                {
                                    isLeftAnchor = !isLeftAnchor;
                                    beizerCounter = 0;
                                }
                            } 

                            rectF.Location = new PointF(points[i].X - 3, points[i].Y - 3);
                            if (rectF.Contains(e.Location))
                            {
                                /////OnCursorChange(eCursor.HandGrip);

                                intNum = i + 1 < points.Count ? (i + 1) : 0;
                                if (!nodeIsSelected(i))
                                {
                                    selectedNodes.Add(i);
                                    if (!isAnchor[i])
                                    {
                                        if (isAnchor[i - 1])
                                        {
                                            selectedNodes.Add((short)(i - 1));
                                            intNum = i + 2 < points.Count-1 ? (i + 2) : 0;
                                            selectedNodes.Add((short)(intNum));
                                        }
                                        else
                                        {   //Read: Note1 at the top of the page for more info
                                            if (types[i + 1] == BEZIER_END)
                                                selectedNodes.Add(0);
                                            else
                                                 selectedNodes.Add((short)(i + 1));
                                            selectedNodes.Add((short)(i - 2));

                                        }
                                    }
                                    else
                                    {
                                        intNum = i - 1;

                                        //Find the point behind selected node and see if 
                                        //it is a bezier node.
                                        //If we have cycled behind zero...                
                                        if (intNum == -1 && (types[types.Count - 1] == BEZIER_END))
                                        {
                                            selectedNodes.Add((short)(types.Count - 4));
                                            //selectedNodes.Add((short)(types.Count - 3));
                                            //selectedNodes.Add((short)(types.Count - 1));
                                        }
                                        else
                                        {
                                            intNum = i - 1 < 0 ? points.Count-1  : (i - 1);
                                            if (intNum != i && isBeizer(types[intNum]) && !isAnchor[intNum])
                                            {
                                                selectedNodes.Add((short)(intNum-2));
                                            }    
                                        }

                                        //If anchor point in front is a beizer, turn it on
                                        intNum = i + 3;
                                        if (i != intNum && intNum < intCount)
                                        {
                                            if (isBeizer(types[intNum]))
                                            {
                                                if (types[intNum] == BEZIER_END)
                                                    selectedNodes.Add(0);
                                                else
                                                    selectedNodes.Add((short)(intNum));
                                            }
                                        }
                                    }
                                }
                                i = intCount;  
                            }
                        }
                    }
                    //END Polygon Editing Code
                    #endregion 
                    mouseIsPressed = true;
                    painter.State.IsEditing = editingOn;
                }
                else if (isSelected)
                {
                    selectedNodes.Clear();
                    isSelected = false;
                    EditingOn = false;
                    eventData.NeedsPainted = true;
                }
            }
            if (eventData.WasHit) eventData.NeedsPainted = true;
            return eventData;
        }

        public override bool HitTest(System.Windows.Forms.MouseEventArgs e)
        {
            //we only need the Location of the mouse but by passing
            //the MouseEventArgs, no value copying has to occur.
            //Only the memory reference will be passed

            if (isSelected && InvalidationArea.Contains(e.Location)|| isInitializing)
                return true;
            else if (Path.IsVisible(e.Location))
                return true;
            else if (Path.IsOutlineVisible(e.Location, hitTestPen))
                return true;

            return false;
        }

        public override bool MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            doOperation = false;
            eventData.WasHit = false;
            if (isSelected && !editingOn && !mouseIsPressed && !isInitializing)
            {
                //if the cursor is over a resize handle, we will set the
                //cursor to the appropriate image.
                ResizeHandles.HitTest(e.Location);
            }

            //if we are not moving the shape...
            if (!isInitializing && isSelected && bmpMove != null)
            {
                for (i = 0; i < points.Count; i++)
                {
                    rectNode.X = (int)points[i].X - 3;
                    rectNode.Y = (int)points[i].Y - 3;
                    if (rectNode.Contains(e.Location))
                    {
                        eventData.WasHit = true;
                        i = (short)points.Count; //terminate the loop
                    }
                }
            }
            //if we ARE moving the shape...
            if (mouseIsPressed && !isInitializing)
            {
                if (editingOn && selectedNodes.Count >= 1)
                {
                    RectangleF oldBounds = Path.GetBounds();

                    //Read NOTE1 at the top of the class for more
                    //info about this code
                    if ((selectedNodes[0] == 0 || selectedNodes[0] ==
                        points.Count-1) &&
                        points[points.Count - 1].Equals(points[0]))
                        points[points.Count - 1] = e.Location;

                    if (selectedNodes.Count > 1 && isAnchor[selectedNodes[0]])
                    {
                        // if the node we're dragging is an anchor point for
                        //a beizer curve, we also want to drag the tension
                        //points with it. Note we are peforming index range validation
                        int intNum = 0;
                        if(selectedNodes[0] - 1 != -1)
                            intNum = selectedNodes[0] - 1;
                        else
                        {
                            if (types[types.Count - 1] == BEZIER_END)
                                intNum = types.Count - 2;
                        }
                        
                        
                        //store the delta amount that the point has moved
                        PointF delta = new PointF(e.X - points[selectedNodes[0]].X,
                                                  e.Y - points[selectedNodes[0]].Y);

                        if (!isAnchor[intNum])
                            points[intNum] = new PointF(points[intNum].X + delta.X,
                                                        points[intNum].Y + delta.Y);
                        int intNum2 = selectedNodes[0] + 1 < points.Count ? selectedNodes[0] + 1 : 0;
                        //make sure that we're not referencing the same point as intNum
                        if (intNum2 != intNum && (!isAnchor[intNum2]))
                                points[intNum2] = new PointF(points[intNum2].X + delta.X,
                                                            points[intNum2].Y + delta.Y);
                    }

                    points[selectedNodes[0]] = e.Location;
                    hasDragged = true;

                    _Path = null;
                    if (Path.GetBounds().Contains(oldBounds))
                        InvalidationArea = GetShapeBounds(true);
                    else
                        InvalidationArea = Rectangle.Round(oldBounds);
                    doOperation = true;
                }
                else if (!editingOn)
                {
                    if (isResizing)
                    {
                        Rectangle oldBounds = InvalidationArea;
                        Resize(e.Location);
                        InvalidationArea = Rectangle.Union(oldBounds,
                            GetShapeBounds(true));
                        doOperation = true;
                    }
                    else 
                    {
                        if (bmpMove == null)
                        {
                            RectangleF rectBounds = Path.GetBounds();
                            PointF tmp = new PointF(0 - rectBounds.X,
                                                    0 - rectBounds.Y);
                            short intCount = (short)points.Count;
                            for (short i = 0; i < intCount; i++)
                            {
                                points[i] = new PointF((points[i].X + tmp.X),
                                                       (points[i].Y + tmp.Y));
                            }
                            _Path = null;


                            bmpMove = new Bitmap((int)Path.GetBounds().Width, (int)Path.GetBounds().Height);
                            Graphics g = Graphics.FromImage(bmpMove);
                           bool blnTemp = painter.PaintFill;
                           painter.PaintFill = false;
                           painter.Paint(g, Path);
                           painter.PaintFill = blnTemp;

                           g.Dispose(); g = null;

                            //return to the original coords
                           tmp = new PointF(rectBounds.X, rectBounds.Y);
                           for (short i = 0; i < intCount; i++)
                           {
                               points[i] = new PointF((points[i].X + tmp.X),
                                                      (points[i].Y + tmp.Y));
                           }
                           _Path = null;
                        }


                        Rectangle oldBounds = new Rectangle(pntBitmapPos, bmpMove.Size);

                        pntBitmapPos = new Point((int)(e.X + mouseOffset.X), (int)(e.Y + mouseOffset.Y));
                        InvalidationArea = Rectangle.Union(oldBounds,
                             new Rectangle(pntBitmapPos, bmpMove.Size));
                        doOperation = true;
                    }

                }


            }
            return doOperation;
        }

        public override EventData MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            mouseIsPressed = false;
            eventData.NeedsPainted = false;
            eventData.WasHit = HitTest(e);
            hasDragged = false;

            if (isResizing)
            {
                isResizing = false;
                painter.State.IsResizing = false;
                Rectangle rectNew = GetShapeBounds(true);
                int intCount = points.Count;
                for (int i = 0; i < intCount; i++)
                {
                    points[i] = new PointF(
                       rectNew.X + (rectNew.Width * ((points[i].X - rectOldBounds.X) / rectOldBounds.Width)),
                       rectNew.Y + (rectNew.Height * ((points[i].Y - rectOldBounds.Y) / rectOldBounds.Height)));
                }

                _Path = null;
                this.ResizeHandles.SetHandlePositions(Path.GetBounds()); 

                eventData.NeedsPainted = true;
            }

            if (bmpMove != null)
            {
                bmpMove = null;
                mouseOffset.X = pntBitmapPos.X - pntMoveStartPos.X;
                mouseOffset.Y = pntBitmapPos.Y - pntMoveStartPos.Y;
                short intCount = (short)points.Count;
                for (short i =0;i<intCount;i++)
                {
                    points[i] = new PointF((points[i].X + mouseOffset.X),
                                           (points[i].Y + mouseOffset.Y));
                }
                _Path = null;
                this.ResizeHandles.SetHandlePositions(Path.GetBounds());
                eventData.NeedsPainted = true;
            }
            return eventData;
        }


        public override void Render(Graphics g)
        {
            if (bmpMove != null)
            {
                g.DrawImageUnscaled(bmpMove, pntBitmapPos);
                return;
            }
           if (isResizing)
           {
               bool blnTmp = painter.PaintFill;
               painter.PaintFill = false;
               painter.Paint(g, Path);
               painter.PaintFill = blnTmp;
           }
           painter.Paint(g, Path);
        }

        public override void RenderHandles(Graphics g)
        {
            if (bmpMove != null) return;
            if (editingOn && (isSelected || isInitializing))
            {
                int intNum = 0;
                short intCount = (short)points.Count;
                for (short i = 0; i < intCount; i++)
                {
                    if (isAnchor[i]) //IS ANCHOR
                    {
                        g.FillRectangle(Brushes.White, points[i].X - 3, points[i].Y - 3, 6, 6);
                        if (nodeIsSelected(i))
                            g.DrawRectangle(Pens.Red, points[i].X - 3, points[i].Y - 3, 6, 6);
                        else
                            g.DrawRectangle(Pens.Black, points[i].X - 3, points[i].Y - 3, 6, 6);
                    }
                    else if (types[i] == BEZIER)
                    {
                        if (isAnchor[i - 1])
                            intNum = i - 1;
                        else
                            intNum = i + 1;
                        //Read: Note1 at the top of the page for more info
                        if (types[intNum] == BEZIER_END) intNum = 0;

                        if (nodeIsSelected((short)intNum))
                        {
                            g.DrawLine(Pens.Black, points[intNum], points[i]);
                            //g.DrawLine(Pens.White, points[intNum].X + 2, points[intNum].Y+2, points[i].X + 2, points[i].Y+2);
                            g.FillEllipse(Brushes.White, points[i].X - 3, points[i].Y - 3, 6, 6);
                            g.DrawEllipse(Pens.Black, points[i].X - 3, points[i].Y - 3, 6, 6);
                        }
                     }
                }
            }
            else if (_isSelected && !editingOn)
            {
                //if the shape is selected, paint the resize handles
                ResizeHandles.Render(g);
            }
        }


        public override void FinalizeEdit()
        {
            Path.CloseAllFigures();
            isInitializing = false;
        }


        private bool nodeIsSelected(short index)
        {
            if (selectedNodes.Count == 0) return false;

            short intCount = (short)selectedNodes.Count;
            for (short i =0;i<intCount;i++)
            {
                if (selectedNodes[i] == index) return true;
            }
            return false;
        }


        public bool ConvertPointToCurve()
        {
            if (selectedNodes.Count == 0) return false;
            //if a control node is selected we won't convert anything
            if (!isAnchor[selectedNodes[0]]) return false;
            //Read: Note1 at the top for more info on this code
            short i = types[selectedNodes[0]] == BEZIER_END? (short)0 : selectedNodes[0];
            short intTemp = i;
            selectedNodes.Clear();

            //We want to convert the selected anchor and the two adjacent anchors
            //to curves. To acomplish this, we will check the node directly behind 
            //and in front of the selected anchor to determine if they are beizer 
            //control nodes.  If one is a control node, we will not convert 
            //that side


            //if subtracting the current node by 1 will sink into negative
            //numbers, keep the loop going by setting the value to the last
            //index position.
            int intNum1 = i - 1 < 0 ? points.Count -1 : i - 1;
            //CONVERT POINT BEHIND SELECTED NODE
            if (!isBeizer(types[intNum1]) || isAnchor[intNum1])
            {
                //we may be evaluating index: 0.  If so, the 0 index will never be a bezier
                //type so we need to check the node in front to see if it is a bezier
                if (intNum1 != 0 || (intNum1 == 0 && types[intNum1 + 1] != BEZIER))
                {
                    PointF ctrP1 = new PointF(points[i].X - 5.0f, points[i].Y - 5.0f);
                    PointF ctrP2 = new PointF(points[intNum1].X + 5.0f, points[intNum1].Y + 5.0f);

                    points.Insert(intNum1 + 1, ctrP2);
                    points.Insert(intNum1 + 2, ctrP1);
                    isAnchor.Insert(intNum1 + 1, false);
                    isAnchor.Insert(intNum1 + 2, false);
                    types.Insert(intNum1 + 1, BEZIER);
                    types.Insert(intNum1 + 2, BEZIER);

                    if (i == 0) // if selected node = zero
                    {   //Read: Note1 at the top for more info about this code
                        points.Insert(intNum1 + 3, points[0]);
                        isAnchor.Insert(intNum1 + 3, false);
                        types.Insert(intNum1 + 3, BEZIER_END);
                    }
                    else
                    {   //to complete a cubic beizer, you must have three
                        //BEZIER types. This converts a standard LINE type.
                        types[intNum1 + 3] = BEZIER;
                    }
                    //selectedNodes.Add((short)intNum1);
                    //being that we have just inserted some values, we need to refresh the
                    //selected node's index position
                    if (i != 0)
                    {
                        i += 2;
                        intTemp = i;
                        intNum1 = i;
                       // selectedNodes.Add((short)intNum1);
                    }

                }
                else 
                    intNum1 = -1;
            }
            else
                intNum1 = -1;

            int intNum2 = i + 1 > points.Count-1 ? 0 : i + 1;
            //CONVERT POINT IN FRONT OF SELECT NODE
            if (!isBeizer(types[intNum2]) || isAnchor[intNum2])
            {
                if (intNum2 != 0 || (intNum2 == 0 && types[types.Count - 1] != BEZIER_END))
                {
                    PointF ctrP1 = new PointF(points[i].X + 5.0f, points[i].Y + 5.0f);
                    PointF ctrP2 = new PointF(points[intNum2].X - 5.0f, points[intNum2].Y - 5.0f);

                    points.Insert(i + 1, ctrP1);
                    points.Insert(i + 2, ctrP2);
                    isAnchor.Insert(i + 1, false);
                    isAnchor.Insert(i + 2, false);
                    types.Insert(i + 1, BEZIER);
                    types.Insert(i + 2, BEZIER);

                    if (i + 3 >= types.Count)
                    {   //Read: Note1 at the top for more info about this code
                        points.Insert(i + 3, points[0]);
                        isAnchor.Insert(i + 3, false);
                        types.Insert(i + 3, BEZIER_END);
                    }
                    else
                    {   //to complete a cubic beizer, you must have three
                        //BEZIER types. This converts a standard LINE type.
                        types[i + 3] = BEZIER;
                    }
                }
                else
                    intNum2 = -1;
            }
            else
                intNum2 = -1;

            //SET THE SELECTED NODES
            if (intTemp == 0)
            {
                if (types[types.Count - 1] == BEZIER_END)
                    selectedNodes.Add((short)(types.Count - 4));
                else
                    selectedNodes.Add((short)(types.Count - 3));
            }
            else
                selectedNodes.Add((short)(intTemp - 3));
            selectedNodes.Add(intTemp);
            selectedNodes.Add((short)(types[intTemp + 3] == BEZIER_END ? 0 : intTemp+3));



            if (intNum1 == -1 && intNum2 == -1)
                return false; //skip code at bottom

            _Path = null; //flush path for update
            return true;
        }

        public bool ConvertPointToLine()
        {
            if (selectedNodes.Count == 0) return false;
            int intNum = selectedNodes[0];
            if (!isAnchor[intNum])
            {
               intNum = isAnchor[intNum - 1]? intNum -1:intNum-2;
            }

            //We want to only straighten any curves ahead of the selected node
            if (intNum + 1 < points.Count && !isAnchor[intNum+1])
            {
                //remove the beizer control points while keeping
                //the anchor point
                types.RemoveRange(intNum + 1, 2);
                points.RemoveRange(intNum + 1, 2);
                isAnchor.RemoveRange(intNum + 1, 2);


                //Check for the the "quirk" node that may be present.
                //Read: NOTE1 at the top for info about the quirk
                if (types[intNum + 1] == BEZIER_END)
                {
                    types.RemoveAt(intNum + 1);
                    points.RemoveAt(intNum + 1);
                    isAnchor.RemoveAt(intNum + 1);
                }
                else if (types[intNum + 1] != 0)
                    types[intNum + 1] = 1; //line type

                _Path = null;
                blnRefreshInvalidate = true;
                selectedNodes.Clear();
                selectedNodes.Add((short)intNum);
            }
            else return false;

            return true;
        }


        private bool isBeizer(byte type)
        {
            return (type == BEZIER || type == BEZIER_END);
        }

        public void AddPoint(PointF pnt)
        {
            if (isInitializing)
            {
                //if the shape is initializing, we only add points
                //not insert points.
                points.Add(pnt);
                //
                if (types.Count == 0) types.Add(STARTPOINT); else types.Add(LINE);
                isAnchor.Add(true);
                selectedNodes.Clear();
            }
            else // INSERT POINT
            {
                //Find the place to insert
                int intNum = 1;
                int i = 0;
                bool blnExit = false;
                float minX = 0;
                float maxX = 0;
                float minY = 0;
                float maxY = 0;

                while (!blnExit)
                {
                    //provides not special functionality, just makes cleaner code
                    Painting.Shapes.Mathematics.SetMinMax(points[i].X, points[intNum].X, out minX, out maxX);
                    Painting.Shapes.Mathematics.SetMinMax(points[i].Y, points[intNum].Y, out minY, out maxY);

                    if (types[i + 1] == BEZIER)
                    {
                        //if the node directly in front of "i" is a beizer 
                        //control node, we will consider it and the control
                        //node after it to be sure that the range vars: minX,
                        //maxX, minY and maxY are set properly ("i" is an anchor)
                        minX = Math.Min(minX, Math.Min(points[i + 1].X,points[i+2].X));
                        maxX = Math.Max(maxX, Math.Max(points[i + 1].X, points[i+2].X));
                        minY = Math.Min(minY, Math.Min(points[i + 1].Y,points[i+2].Y));
                        maxY = Math.Max(maxY, Math.Max(points[i + 1].Y, points[i+2].Y));
                    }


                    if ((pnt.X >= minX && pnt.X <= maxX) &&
                        pnt.Y >= minY && pnt.Y <= maxY)
                    {
                        intNum = i + 1; //set the insert point
                        blnExit = true; //terminate the loop
                    }
                    else
                    {
                        //increment the loop
                        if (!isAnchor[i + 1])
                        {
                            i += 3;
                            if (i + 1 < points.Count && !isAnchor[i + 1])
                                intNum = i + 3;
                            else 
                                intNum = i + 1 < points.Count? i+1 : 0;
                        }
                        else
                        {
                            i++;
                            if (!isAnchor[i + 1 < (points.Count - 1) ? i + 1 : 0])
                                intNum = i == (points.Count - 1) ? 0 : i + 3;
                            else
                                intNum = i == (points.Count - 1) ? 0 : i + 1;
                        }

                        if (intNum == 0 || types[intNum] == BEZIER_END)
                            blnExit = true;
                    }

                }
                intNum = i + 1;

                //Determine if we are inserting into a beizer
                if ((isBeizer(types[i])&& !isAnchor[i]) || isBeizer(types[i+1>=points.Count? 0:i+1]))
                {
                    PointF[] newPoints = {
                        new PointF(pnt.X - 10, pnt.Y + 5), //the new beizer handle to the left of the new anchor
                        pnt, //the new anchor
                        i+1 == points.Count? points[0]:
                                             new PointF(pnt.X+10,pnt.Y+5)}; // the beizer handle to the right of the new anchor

                    byte[] newTypes = { BEZIER, BEZIER, (i + 1 == points.Count ? BEZIER_END : BEZIER) };
                    bool[] newIsAnchors = { false, true, false };

                    //Perform the insertion
                    points.InsertRange(intNum+1, newPoints);
                    types.InsertRange(intNum+1, newTypes);
                    isAnchor.InsertRange(intNum+1, newIsAnchors);
                }
                else
                {
                    points.Insert(intNum, pnt);
                    types.Insert(intNum, 1);
                    isAnchor.Insert(intNum, true);
                }
                
            }
            _Path = null;
            InvalidationArea = GetShapeBounds(true);
        }


        public bool DeleteNode()
        {
            if (selectedNodes.Count == 0) return false;
            if (!isAnchor[selectedNodes[0]]) return false;
            int intNum = GetAnchorCount();
            if (intNum == 3) return false;

            intNum = selectedNodes[0];

            int intMin = intNum - 1 > 0? intNum-1:0;
            int intMax = intNum + 1 < points.Count?intNum+1:0;

            if (intNum == STARTPOINT)
            {
                if (isAnchor[1])
                {

                    if (types[types.Count - 1] == BEZIER_END)
                        points[points.Count - 1] = points[1];

                        points.RemoveAt(0);
                        types.RemoveAt(0);
                        isAnchor.RemoveAt(0);
                        types[0] = STARTPOINT;
                }
                else
                {
                    if (types[types.Count - 1] == BEZIER_END)
                    {
                        points[points.Count - 2] = points[2];
                        points[points.Count - 1] = points[3];

                        points.RemoveRange(0, 3);
                        types.RemoveRange(0, 3);
                        isAnchor.RemoveRange(0, 3);

                        types[0] = STARTPOINT;
                    }
                    else
                    {
                        PointF[] newpoints = {points[1], points[2], points[3]};
                        byte[] newtypes = { BEZIER, BEZIER, BEZIER_END };
                        bool[] newisAnchors = { false, false, false };

                        points.RemoveRange(0, 3);
                        types.RemoveRange(0, 3);
                        isAnchor.RemoveRange(0, 3);

                        points.InsertRange(points.Count, newpoints);
                        types.InsertRange(types.Count, newtypes);
                        isAnchor.InsertRange(isAnchor.Count, newisAnchors);
                        types[0] = STARTPOINT;
                    }
                }
            }
            else if ((types[intMin] == BEZIER && !isAnchor[intMin]) &&
                (types[intMax] == BEZIER && !isAnchor[intMax]))
            {
                points.RemoveRange(intNum - 1, 3);
                types.RemoveRange(intNum - 1, 3);
                isAnchor.RemoveRange(intNum - 1, 3);
            }

            else if (types[intMin] == BEZIER && !isAnchor[intMin])
            {
                
                if (types[intMax] != 0)
                {
                    types[intMax] = BEZIER;
                    points.RemoveAt(intNum);
                    types.RemoveAt(intNum);
                    isAnchor.RemoveAt(intNum);
                }
                else
                {
                    points.Insert(points.Count, points[0]);
                    types.Insert(types.Count, BEZIER_END);
                    isAnchor.Insert(isAnchor.Count, false);
                }
            }
            else
            {   //removes a single line anchor
                points.RemoveAt(intNum);
                types.RemoveAt(intNum);
                isAnchor.RemoveAt(intNum);
            }
            selectedNodes.Clear();
            //selectedNodes.Add((short)(intNum - 1 > 0 ? intNum - 1 : 0));
            

            _Path = null;
            return true;

        }

        private int GetAnchorCount()
        {
            int intCount = isAnchor.Count;
            int count = 0;
            for (int i =0;i<intCount;i++)
            {
                if (isAnchor[i]) count++;
            }

            return count;
        }


        public override string EmitGDICode(string graphicsName, GDIGenerationTool helper)
        {

           string str = "//Polygon: " + this.Name + "\r\n" +
           helper.GeneratePaintToolInit(this) +

           generatePointArray(helper)+
           generateTypeArray() +

           "//Create the path\r\npath = new GraphicsPath("+this.Name + "Points,"+this.Name + "Types);\r\n"+
           "path.CloseAllFigures();\r\n"+

           "//Paint the shape\r\n";
            if (painter.PaintFill)
                str += graphicsName + ".FillPath(" + this.Name + "Brush, path );\r\n";
            if (painter.PaintBorder)
                str += graphicsName + ".DrawPath(" + this.Name + "Pen, path);\r\n";

            str += helper.GeneratePaintToolCleanup(this);
            return str;
        }

        private string generatePointArray(GDIGenerationTool helper)
        {
            StringBuilder sb = new StringBuilder(points.Count * 25);

            int intCount = points.Count-1;
            int intRem = 0;
            short i = 0;
            for (i =0;i<intCount;i++)
            {
                Math.DivRem(i, 4, out intRem);
                if (intRem == 0)
                    sb.Append("new PointF(" + (points[i].X - helper.AreaBounds.X) + "f," + 
                              (points[i].Y - helper.AreaBounds.Y) + "f),\r\n");
                else
                    sb.Append("new PointF(" + (points[i].X - helper.AreaBounds.X) + "f," +
          (points[i].Y - helper.AreaBounds.Y) + "f), ");
            }
            sb.Append("new PointF(" + (points[i].X - helper.AreaBounds.X) + "f," + 
                      (points[i].Y  - helper.AreaBounds.Y) + "f)\r\n};\r\n");

            string str = "//Create the point array\r\nPointF[] " +
                this.Name + "Points = {" + sb.ToString() + "\r\n" +
                "ScalePoints(ref " + this.Name + "Points, shapeBoundsOld, shapeBoundsNew);\r\n";
            sb = null;
            return str;
        }

        private string generateTypeArray()
        {
            StringBuilder sb = new StringBuilder(types.Count * 4);

            int intCount = types.Count-1;
            int tmpOut = 0;
            short i = 0;
            for (i = 0; i < intCount; i++)
            {
                Math.DivRem(i, 8,out tmpOut);
                if (tmpOut == 0)
                    sb.Append(types[i]+ ",\r\n");
                else
                    sb.Append(types[i] + ", ");
            }
            sb.Append(types[i] + " ");

            string str = "//Create the type array\r\nbyte[] " +
                this.Name + "Types = {" + sb.ToString() + "};\r\n";
            sb = null;
            return str;
        }


        public override Shape Clone()
        {
            ShapePolygon newShape = new ShapePolygon();

            //copy the properties
            newShape.points.AddRange(this.points.ToArray());
            newShape.types.AddRange(this.types.ToArray());
            newShape.isAnchor.AddRange(this.isAnchor.ToArray());
            newShape.isInitializing = false;
            newShape.painter.CopyProperties(ref this.painter);

            //offset the new polygon
            int intCount = points.Count;
            for (int i =0;i<intCount;i++)
            {
                newShape.points[i] = new PointF(points[i].X + 15f, points[i].Y + 15f);
            }

            newShape.EditingOn = false;
            return newShape;
        }

        protected override void Restore_NonSerialized()
        {
            selectedNodes = new List<short>(8);
            hitTestPen = new Pen(Color.Black, 2);

        }

        public override string ShapeType
        {
            get { return "Polygon"; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            hitTestPen.Dispose();
            if (bmpMove != null) bmpMove.Dispose();
            if (_Path != null) _Path.Dispose();
            painter = null;
        }

        #endregion

    }
}
