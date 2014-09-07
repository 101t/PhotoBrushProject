////////////////////////////////////////////////////////////////
//                Created By Richard Blythe 2008
//   There are no licenses or warranty attached to this object.
//   If you distribute the code as part of your application, please
//   be courteous enough to mention the assistance provided you.
//   Enhanced and Re-developed By Tarek MOH Omer Kala'ajy 2013
////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Painting.Shapes
{
    [Serializable]
    public enum eOperation
    {
        ConvertToCurve, ConvertToLine, Delete,
        BringToFront, SendToBack, Copy, Paste
    }

    [Serializable]
    public class ShapeManager : IDisposable
    {
        List<Shape> shapes = new List<Shape>(10);
        //is used to store the index pos of shapes
        //that have intersecting coords with the mouse down pos
        [NonSerialized]
        List<short> invalidationList = new List<short>(10);
        [NonSerialized]
        List<short> copyIndexes = new List<short>(2);
        [NonSerialized]
        bool blnSuspendHitTest, blnFoundHit, blnShapeHasMoved, blnEndShortLoop;
        [NonSerialized]
        short intCurIndex = -1;
        short intPrevIndex = -1;
        [NonSerialized]
        private EventData eventData;

        [NonSerialized]
        private DrawingCanvas refControl;
        [NonSerialized]
        private Bitmap backBmp = null;

        Rectangle rectSelection = Rectangle.Empty;
        private string strBackBmpPath = "";
        private Rectangle rectBackBmpBounds = Rectangle.Empty;

        public ShapeManager(DrawingCanvas parent)
        {
            parent.MouseDown += new MouseEventHandler(parent_MouseDown);
            parent.MouseUp += new MouseEventHandler(parent_MouseUp);
            parent.MouseMove += new MouseEventHandler(parent_MouseMove);

            refControl = parent;
            intCurIndex = -1;

            //set the properties of shapeClipRect
            shapeClipRect.Name = "clipRectangle@@**@@123";//ensure a unique name
        }

        private string _FileName = "";
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        private bool _AntiAlias;
        public bool AntiAlias
        {
            get { return _AntiAlias; }
            set
            {
                if (_AntiAlias != value)
                {
                    _AntiAlias = value;
                    refControl.Invalidate();
                }
            }
        }

        /// <summary>
        /// Will paint the background with the bitmap passed in.
        /// (Commonly used for tracing out a shape from an image)
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="pnt"></param>
        public void SetBackBitmap(Bitmap bmp, string filePath)
        {
            if (bmp == null && backBmp != null)
            {
                backBmp.Dispose();
                backBmp = null;
            }
            else
            {
                backBmp = bmp;
                strBackBmpPath = filePath;
                refControl.Invalidate();
            }
        }

        public string BackBmpFilePath
        {
            get { return strBackBmpPath; }
        }

        public Rectangle BackBitmapBounds
        {
            get { return rectBackBmpBounds; }
            set { rectBackBmpBounds = value; }
        }

        private bool _isAddingShape;
        public bool isAddingShape
        {
            get { return _isAddingShape; }
        }

        [NonSerialized]
        private bool isDirty;
        public bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        private Tools.eTool curTool = Tools.eTool.Selection;
        public Tools.eTool CurrentTool
        {
            get { return curTool; }
            set
            {
                //Rectangle rectInvalidate;
                invalidationList.Clear();

                switch (value)
                {
                    case Tools.eTool.Selection:
                        if (intCurIndex != -1)
                        {
                            shapes[intCurIndex].EditingOn = false;
                            invalidationList.Add(intCurIndex);
                        }
                        refControl.Cursor = Cursors.Default;
                        break;
                    case Tools.eTool.Edit:
                        if (intCurIndex != -1)
                        {
                            shapes[intCurIndex].EditingOn = true;
                            invalidationList.Add(intCurIndex);
                        }
                        break;
                }
                curTool = value;
                FinalizeShape();
                DoInvalidation();
            }
        }

        private ShapeClipRectangle shapeClipRect = new ShapeClipRectangle();
        
        private bool blnClipRectIsOn;

        public bool ClipRectIsOn
        {
            get { return blnClipRectIsOn; }
            set
            {
                if (blnClipRectIsOn != value)
                {
                    blnClipRectIsOn = value;
                   
                    if (value && shapes.Count == 0)
                        shapes.Add(shapeClipRect);
                    else  if (shapes.Count > 0 && value &&  !(shapes[shapes.Count - 1] is ShapeClipRectangle))
                        shapes.Add(shapeClipRect);
                    else
                    {
                        if (shapes.Count > 0 && !value && shapes[shapes.Count - 1] is ShapeClipRectangle)
                        {
                            if (intCurIndex == shapes.Count - 1)
                            {
                                intCurIndex = -1; intPrevIndex = -1;
                            } 
                            shapes.RemoveAt(shapes.Count - 1);
                        }
                    }
                    shapeClipRect.isSelected = true;
                    shapeClipRect.InvalidationArea = shapeClipRect.GetShapeBounds(true);
                    refControl.Invalidate(shapeClipRect.InvalidationArea);  
                }
            }
        }

        public Rectangle ClipBounds
        {
            get { return shapeClipRect.GetShapeBounds(true); }
        }

        public void FinalizeShape()
        {
            _isAddingShape = false;
            if (intCurIndex != -1)
            {
                //Rectangle rectInvalidate;
                shapes[intCurIndex].FinalizeEdit();
                switch (curTool)
                {
                    case Tools.eTool.Selection:

                        shapes[intCurIndex].isSelected = true;
                        shapes[intCurIndex].EditingOn = false;
                        break;
                    case Tools.eTool.Edit:
                        //Rectangle rect = shapes[intCurIndex].InvalidationArea;
                        shapes[intCurIndex].EditingOn = true;
                        break;
                }
                refControl.Invalidate(shapes[intCurIndex].InvalidationArea);
            }
        }

        void parent_MouseDown(object sender, MouseEventArgs e)
        {
            invalidationList.Clear();
            if (e.Button == MouseButtons.Left)
            {
                blnFoundHit = false;
                blnSuspendHitTest = false;
                if (intCurIndex != -1)
                {
                    eventData = shapes[intCurIndex].MouseDown(e);
                    if (eventData.NeedsPainted || eventData.FinalizeShape)
                        invalidationList.Add((short)intCurIndex);
                    blnSuspendHitTest = eventData.WasHit;
                    if (eventData.FinalizeShape)
                        FinalizeShape();
                }
                if (!blnSuspendHitTest)
                {
                    for (short i = (short)(shapes.Count - 1); i > -1; i--)
                    {
                        if (!blnSuspendHitTest && shapes[i].HitTest(e))
                        {
                            intCurIndex = i;
                            shapes[intCurIndex].EditingOn = curTool == Tools.eTool.Edit;
                            shapes[intCurIndex].MouseDown(e);
                            invalidationList.Add(intCurIndex);
                            blnSuspendHitTest = true;
                            i = -1; //terminate the loop
                        }
                    }
                }
            }
            DoInvalidation();
        }

        void parent_MouseMove(object sender, MouseEventArgs e)
        {
            invalidationList.Clear();
            if (intCurIndex != -1 && shapes[intCurIndex].MouseMove(e))
            {
                invalidationList.Add(intCurIndex);
                blnShapeHasMoved = true;
            }
            else
                blnShapeHasMoved = false;
            if (blnShapeHasMoved) isDirty = true;
                DoInvalidation();
        }

        void parent_MouseUp(object sender, MouseEventArgs e)
        {
            invalidationList.Clear();
            if (e.Button == MouseButtons.Right) return;
            blnSuspendHitTest = false;
            blnFoundHit = false;
            blnEndShortLoop = false;
            if (intCurIndex != -1)// && !blnShapeHasMoved)
            {
                eventData = shapes[intCurIndex].MouseUp(e);
                if (eventData.NeedsPainted)
                    invalidationList.Add((short)intCurIndex);
                blnFoundHit = (eventData.WasHit);
                blnSuspendHitTest = (_isAddingShape || blnShapeHasMoved || (intPrevIndex != intCurIndex)
                                    || shapes[intCurIndex].EditingOn || eventData.HoldPosition);
                for (short i = (short)(intCurIndex == 0? (shapes.Count-1) : intCurIndex); i > -1; i--)
                {
                    if (!blnSuspendHitTest && i != intCurIndex && shapes[i].HitTest(e))
                    {
                        intCurIndex = i;
                        blnSuspendHitTest = true;
                        blnFoundHit = true;
                        shapes[intCurIndex].isSelected = true;
                        shapes[intCurIndex].EditingOn = curTool == Tools.eTool.Edit;
                        invalidationList.Add(intCurIndex); 
                        i = -1; //exit loop
                    }
                    if (i - 1 == -1 && !blnEndShortLoop)
                    {
                        blnEndShortLoop = true;
                        i = (short)(shapes.Count); //loop will decrement it to a zero based index
                    }
                    if (blnSuspendHitTest) i = -1; 
                }
            }
            if (!blnFoundHit) intCurIndex = -1;

            if (intPrevIndex != -1 && intPrevIndex != intCurIndex)
            {
                shapes[intPrevIndex].isSelected = false;
                invalidationList.Add((short)intPrevIndex);
            }
            if (intCurIndex != -1 && !blnFoundHit && !shapes[intCurIndex].EditingOn)
            {
                intCurIndex = -1;
                intPrevIndex = -1;
            }
            else
                intPrevIndex = intCurIndex;
            DoInvalidation();
        }

        public void DoOperation(eOperation operation)
        {
            switch (operation)
            {
                case eOperation.ConvertToCurve:
                    if (intCurIndex != -1 && shapes[intCurIndex] is ShapePolygon)
                    {
                        if (((ShapePolygon)shapes[intCurIndex]).ConvertPointToCurve())
                            refControl.Invalidate(shapes[intCurIndex].InvalidationArea);
                    }
                    break;
                case eOperation.ConvertToLine:
                    if (intCurIndex != -1 && shapes[intCurIndex] is ShapePolygon)
                    {
                        if (((ShapePolygon)shapes[intCurIndex]).ConvertPointToLine())
                            refControl.Invalidate(shapes[intCurIndex].InvalidationArea);
                    }
                    break;
                case eOperation.Delete:
                    Delete();
                    break;
                case eOperation.BringToFront:
                    BringToFront();
                    break;
                case eOperation.SendToBack:
                    SendToBack();
                    break;
            }
            isDirty = true;
        }

        private void DoInvalidation()
        {
            if (invalidationList.Count == 1)
            {
                refControl.Invalidate(shapes[invalidationList[0]].InvalidationArea);
            }
            else if (invalidationList.Count > 1)
            {
                Rectangle rectInvalidate = shapes[invalidationList[0]].InvalidationArea;
                int intCount = invalidationList.Count;
                for (short i =1;i<intCount;i++)
                {
                    rectInvalidate = Rectangle.Union(rectInvalidate,
                        shapes[invalidationList[i]].InvalidationArea);
                }
                refControl.Invalidate(rectInvalidate);
            }
        }

        public bool AddShape(Shape s)
        {
            invalidationList.Clear();
            if (blnClipRectIsOn)
                shapes.Insert(shapes.Count - 1, s);
            else
                shapes.Add(s);
            //wire up events
            s.EditorRequested += new EventHandler(shape_EditorRequested);

            if (intCurIndex != -1)
            {
                shapes[intCurIndex].isSelected = false;
                invalidationList.Add(intCurIndex);
            }
            if (blnClipRectIsOn)
            {
                intCurIndex = (short)(shapes.Count - 2);
                shapes[intCurIndex].zOrder = (short)(intCurIndex);
                shapes[intCurIndex].isSelected = true;
                shapeClipRect.zOrder++;
                invalidationList.Add(intCurIndex);
            }
            else
            {
                intCurIndex = (short)(shapes.Count - 1);
                shapes[intCurIndex].zOrder = (short)intCurIndex;
                shapes[intCurIndex].isSelected = true;
                invalidationList.Add(intCurIndex);
            }


            if (s is ShapePolygon)
            {
                if (((ShapePolygon)s).IsInitializing)
                    shapes[intCurIndex].EditingOn = true;
                _isAddingShape = true;
            }
            else
                _isAddingShape = false;
            
            string strCurSearchName = shapes[intCurIndex].ShapeType + "1";
            int intCount = shapes.Count;
            short shapeNum = 1;
            for (short i =0;i<intCount;i++)
            {
                //shapeNum = 1;
                if (shapes[i].GetType() == shapes[intCurIndex].GetType())
                {
                    if (shapes[i].Name == strCurSearchName)
                    {
                        strCurSearchName = shapes[intCurIndex].ShapeType + (++shapeNum);
                        i = -1; //reset the loop
                    }
                }
            }
            shapes[intCurIndex].Name = strCurSearchName;
            DoInvalidation();
            isDirty = true;
            return true;
        }

        void shape_CursorChange(Cursor c, bool isDefault)
        {
            if (isDefault)
                refControl.Cursor = Cursors.Default;
            else
                refControl.Cursor = c;
        }

        void shape_EditorRequested(object sender, EventArgs e)
        {
            if (intCurIndex == -1) return;

            if (shapes[intCurIndex] is ShapeText)
            {
                ShowTextEditor();
             }
        }

        public void ShowTextEditor()
        {
            if (!(shapes[intCurIndex] is ShapeText)) return;
            FormTextProperties frm = new FormTextProperties((ShapeText)shapes[intCurIndex]);
            frm.ShowDialog();
            if (frm.txtChanged || frm.fontChanged)
                refControl.Invalidate(shapes[intCurIndex].InvalidationArea);
        }

        public void Copy()
        {
            if (intCurIndex == -1) return;
            copyIndexes.Clear();
            copyIndexes.Add(intCurIndex);
        }

        public bool Paste()
        {
            if (copyIndexes.Count == 0) return false;
            if (AddShape(shapes[copyIndexes[0]].Clone()))
            {
                _isAddingShape = false;
                refControl.Invalidate(shapes[copyIndexes[0]].InvalidationArea);
                return true;
            }
            _isAddingShape = false;
            return false;
        }

        public bool Delete()
        {
            if (intCurIndex != -1)
            {
                for (int i = 0; i < copyIndexes.Count; i++)
                {
                    if (intCurIndex == copyIndexes[i])
                    {
                        copyIndexes.RemoveAt(i);
                        i = copyIndexes.Count; //terminate the loop
                    }
                }

                if (shapes[intCurIndex] is ShapeClipRectangle) return false;
                Rectangle rectBounds = shapes[intCurIndex].InvalidationArea;
                
                if (shapes[intCurIndex] is ShapePolygon && shapes[intCurIndex].EditingOn)
                {
                    ((ShapePolygon)shapes[intCurIndex]).DeleteNode();
                }
                else
                {
                    shapes[intCurIndex].EditorRequested -= shape_EditorRequested;
                    shapes.RemoveAt(intCurIndex);
                    intCurIndex = -1;
                    intPrevIndex = -1;
                }

                refControl.Invalidate(rectBounds);
                isDirty = true;
                return true;
            }
            return false;
        }

        public void PaintShapes(Graphics g)
        {
           if (backBmp != null && rectBackBmpBounds.Width != 0 && rectBackBmpBounds.Height != 0)
                g.DrawImage(backBmp, rectBackBmpBounds);

            if (_AntiAlias) g.SmoothingMode = SmoothingMode.AntiAlias;
            short intCount = ((short)shapes.Count);
            for (short i = 0; i < intCount; i++)
            {
                if (shapes[i].NeedsPainted(g.ClipBounds))
                    shapes[i].Render(g);
            }

            for (short i = 0; i < intCount; i++)
            {
                shapes[i].RenderHandles(g);
            }
      }

        public int ShapeCount
        {
            get { return shapes.Count; }
        }

        public bool ShapeIsSelected()
        {
            return (intCurIndex > -1);
        }

        public Shape GetSelectedShape()
        {
            if (intCurIndex != -1)
                return shapes[intCurIndex];
            else
                return null;
        }

        public short SelectedIndex
        {
            get { return intCurIndex; }
        }

        public Shape GetShape(int index)
        {
            if (index != -1 || index < shapes.Count-1)
                return shapes[index];
            else
                return null;
        }

        public bool SendToBack()
        {
            if (intCurIndex > 0)
            {
                if (shapes[intCurIndex] is ShapeClipRectangle) return false;
                shapes[intCurIndex].zOrder = 0;
                for (short i = 0; i < intCurIndex; i++)
                    shapes[i].zOrder++;

                shapes.Sort();
                intCurIndex = 0;
                refControl.Invalidate(shapes[0].InvalidationArea);
                isDirty = true;
                return true;
            }
            return false;
        }

        public bool BringToFront()
        {
            if (intCurIndex != -1 && intCurIndex < shapes.Count - 1)
            {
                if (shapes[intCurIndex] is ShapeClipRectangle) return false;
                shapes[intCurIndex].zOrder = (short)(shapes.Count - 1);
                int intCount = shapes.Count;
                for (short i = (short)(intCurIndex + 1); i < intCount; i++)
                    shapes[i].zOrder--;

                shapes.Sort();
                if (blnClipRectIsOn)
                { //always have the clipping rectangle on top
                    Shape tmp = shapes[shapes.Count - 1];
                    shapes[shapes.Count - 2] = shapes[shapes.Count - 1];
                    shapes[shapes.Count - 2] = tmp;
                    intCurIndex = (short)(intCount - 2);
                }
                else
                    intCurIndex = (short)(intCount - 1);
                refControl.Invalidate(shapes[intCurIndex].InvalidationArea);

                isDirty = true;
                return true;
            }
            return false;
        }

        private void SortShapes()
        {
            shapes.Sort();
        }

        public short NameExists(string strName)
        {
            for (short i = 0; i < shapes.Count; i++)
            {
                if (shapes[i].Name == strName)
                    return i;
            }
            return -1;
        }

        public bool ChangeName(short index, string strName, out string strError )
        {
            if (strName == "" || strName == null)
            {
                strError = "You must enter a name for the shape!";
                return false;
            }
            strName = strName.Trim();

            if (!Char.IsLetter(strName,0))
            {
                strError = "GDI+ Generator does not allow any character except "+
                           "a letter to start the name. Any other character is rejected.";
                return false;
            }

            for (short i =0;i<strName.Length;i++)
            {
                if (Char.IsWhiteSpace(strName, i))
                {
                    strError = "White spaces are not allowed in naming a shape!";
                    return false;
                }
            }

            short value = NameExists(strName);
            if (value != -1 && value != index)
            {
                strError = "The name: " + strName + " already exists!";
                return false;
            }
            if (value != index)
                shapes[index].Name = strName;

            strError = null;
            isDirty = true;
            return true;
        }

        public ShapeMenuItem[] GetShapeList()
        {
            ShapeMenuItem[] shapemenus = new ShapeMenuItem[6];
            shapemenus[0] = new ShapeMenuItem("Rectangle", 0);
            shapemenus[1] = new ShapeMenuItem("Triangle", 1);
            shapemenus[2] = new ShapeMenuItem("Circle", 2);
            shapemenus[3] = new ShapeMenuItem("Polygon", 3);
            shapemenus[4] = new ShapeMenuItem("Line", 4);
            shapemenus[5] = new ShapeMenuItem("Text", 5);
            return shapemenus;
        }

        public void AddShape(byte intShapeID)
        {
            switch (intShapeID)
            {
                case 0: //Rectangle
                    ShapeRectangle rect = new ShapeRectangle();
                    rect.Location = new Point(50, 50);
                    AddShape(rect);
                    break;
                case 1: //Triangle
                    ShapeTriangle triangle = new ShapeTriangle();
                    triangle.Location = new Point(50, 50);
                    AddShape(triangle);
                    break;
                case 2:
                    ShapeCircle circle = new ShapeCircle();
                    circle.Location = new Point(50, 50);
                    AddShape(circle);
                    break;
                case 3:
                    ShapePolygon poly = new ShapePolygon();
                    poly.Location = new Point(50, 50);
                    AddShape(poly);
                    break;
                case 4:
                    ShapeLine line = new ShapeLine();
                    line.Location = new Point(50, 50);
                    AddShape(line);
                    break;
                case 5:
                    ShapeText text = new ShapeText();
                    text.Location = new Point(50, 50);
                    AddShape(text);
                    break;
                default:
                    throw new Exception("Could not find the associated shape type!");
            }
        }

        /// <summary>
        /// Loop through all the shapes and grab the maximum 
        /// width and the maximum height.
        /// </summary>
        public Rectangle GetTotalShapeArea()
        {
            short intCount = (short)this.shapes.Count;
            if (this.ClipRectIsOn) intCount--;
            int maxLeft = int.MaxValue;
            int maxRight = 0;
            int maxTop = int.MaxValue;
            int maxBottom = 0;

            Rectangle rect = Rectangle.Empty;
            for (short i =0;i<intCount;i++)
            {
                rect = shapes[i].GetShapeBounds(true);
                if (rect.Left < maxLeft) maxLeft = rect.Left;
                if (rect.Right > maxRight) maxRight = rect.Right;
                if (rect.Top < maxTop) maxTop = rect.Top;
                if (rect.Bottom > maxBottom) maxBottom = rect.Bottom;
            }
            return new Rectangle(maxLeft, maxTop, maxRight - maxLeft, maxBottom - maxTop);
        }

        public void RestoreNonSerializable(DrawingCanvas parent)
        {
            invalidationList = new List<short>(10);
            copyIndexes = new List<short>(2);
            isDirty = false;
            parent.MouseDown += new MouseEventHandler(parent_MouseDown);
            parent.MouseUp += new MouseEventHandler(parent_MouseUp);
            parent.MouseMove += new MouseEventHandler(parent_MouseMove);
            shapeClipRect = new ShapeClipRectangle();

            refControl = parent;
            intCurIndex = -1;

            for (short i = 0; i < shapes.Count; i++)
            {
                this.shapes[i].RestoreNonSerializable();
            }
            try
            {
                if (strBackBmpPath != "" &&
                    strBackBmpPath != null)
                {
                    backBmp = new Bitmap(strBackBmpPath);
                    refControl.Invalidate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Background Image Load Error");
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            //unwire the listeners
            refControl.MouseDown -= parent_MouseDown;
            refControl.MouseUp -= parent_MouseUp;
            refControl.MouseMove -= parent_MouseMove;
            refControl = null;

            short intCount = (short)shapes.Count;
            for (short i =0;i<intCount;i++)
            {
                shapes[i].EditorRequested -= shape_EditorRequested;
                shapes[i] = null;
            }
            shapes.Clear();
        }
        #endregion
    }
}
