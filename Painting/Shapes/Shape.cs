////////////////////////////////////////////////////////////////
//                Created By Richard Blythe 2008
//   There are no licenses or warranty attached to this object.
//   If you distribute the code as part of your application, please
//   be courteous enough to mention the assistance provided you.
//   Modified By Tarek Kala'ajy 2013
////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using Painting.Painter;

namespace Painting.Shapes
{
    public struct EventData
    {
        public bool WasHit;
        public bool NeedsPainted;
        public bool FinalizeShape;
        public bool HoldPosition;
    }

    public struct sGDIProperty
    {
        public const byte EditColor = 0;
        public const byte EditBounds = 1;
        public const byte EditBlend = 2;
        public const byte AllowScaling = 3;
    }

    [Serializable]
    public abstract class Shape : IComparable, IRestoreNonSerializable
    {      
        public Painters painter;
        protected GDIProperties GdiProperties;

        #region GDI+ Property Accessors
        public int GDIPropertyCount
        {
            get { return GdiProperties.PropertyCount; }
        }

        public string GetPropertyName(int index)
        {
            return GdiProperties.GetPropertyName(index);
        }

        public string GetPropertyDescription(int index)
        {
            return GdiProperties.GetDescription(index);
        }

        public object GetGDIValue(string propertyName)
        {
           return GdiProperties.GetValue(propertyName);
        }

        public object GetGDIValue(int index)
        {
            return GdiProperties.GetValue(index);
        }

        public void SetGDIValue(string propertyName, object value)
        {
            GdiProperties.SetValue(propertyName, value);
        }

        public void SetGDIValue(int index, object value)
        {
            GdiProperties.SetValue(index, value);
        }
        #endregion

        [NonSerialized]
        public Handle ResizeHandles = new Handle();
        public short zOrder;

        protected abstract void GeneratePath();
        protected abstract void GeneratePath(Rectangle bounds);
        public abstract string EmitGDICode(string graphicsName, GDIGenerationTool helper);
        public abstract string ShapeType { get;}
        public abstract bool HasNodes { get;}
        public abstract Shape Clone();
        /// <summary>
        /// When serializing (saving) a shape, there are certain variables
        /// that do not need to be saved.  However, they must be initialized
        /// when the shape is deserialized because the constructor method does not
        /// run when deserialization occurs.  This method is to be used in restoring
        /// non serializable variables.
        /// </summary>
        protected abstract void Restore_NonSerialized();

        public event EventHandler EditorRequested;
        protected void OnEditorRequested()
        {
            if (EditorRequested != null)
            {
                //The shape manager will reference us by the selected index,
                //so there is no need to pass any args
                EditorRequested(null, null);
            }
        }

        #region Protected Vars
        [NonSerialized]
        protected PointF mouseOffset;
        [NonSerialized]
        protected bool mouseIsPressed;
        [NonSerialized]
        protected Bitmap bmpMove = null;
        [NonSerialized]
        protected Point pntBitmapPos;
        [NonSerialized]
        protected Point pntMoveStartPos;
        [NonSerialized]
        protected Rectangle rectOldBounds;
        [NonSerialized]
        protected bool isResizing;
        [NonSerialized]
        protected bool doOperation;
        [NonSerialized]
        protected bool blnSuppressInflate;
        [NonSerialized]
        protected bool blnRefreshInvalidate = true;
        [NonSerialized]
        public EventData eventData = new EventData();
        #endregion


        #region Properties
        private string _Name;
        public string Name
        {
            get {return _Name;}
            set {_Name = value;}
        }

        protected Size _Size;
        public Size Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
                _Path = null;  //set to null so we can refresh
            }
        }

        protected Point _Location;
        public Point Location
        {
            get { return _Location; }
            set 
            {
                _Location = value; 
                _Path = null;  //set to null so we can refresh
            }
        }
       

        //We should Always access the path through this property
        //so that the path can be regenerated if it is null
        [NonSerialized]
        protected GraphicsPath _Path;
        protected GraphicsPath Path
        {
            get 
            { 
                if (_Path == null)
                    GeneratePath();
                return  _Path; 
            }
        }

        [NonSerialized]
        protected bool _isSelected;
        public bool isSelected
        {
            get 
            {
                return _isSelected; 
            }
            set 
            {
                if (!_isSelected && value == true)
                {
                    ResizeHandles.SetHandlePositions(GetShapeBounds(true));
                    InvalidationArea = GetShapeBounds(true);
                }

                painter.State.IsSelected = value;
                _isSelected = value;
             }
        }
        [NonSerialized]
        protected bool editingOn;
        public virtual bool EditingOn
        {
            get 
            { return editingOn; }
            set
            {
                if (editingOn && value == false)
                {
                    ResizeHandles.SetHandlePositions(GetShapeBounds(true));
                }
                //this will suspend initial attempts to shrink the
                //Invalidation rectangle so we can erase the resize handles.
                blnRefreshInvalidate = true;  
                            
                editingOn = value;
                painter.State.IsEditing = value;
            }
        }

        protected void ResetEventData()
        {
            eventData.NeedsPainted = false;
            eventData.WasHit = false;
            eventData.FinalizeShape = false;
        }

        [NonSerialized]
        protected Rectangle invalidationArea;
        public Rectangle InvalidationArea
        {
            get
            {
                //if (_Path == null) GeneratePath();
                if (blnRefreshInvalidate)
                {
                    Rectangle rectBounds = invalidationArea;
                    InvalidationArea = GetShapeBounds(true);
                    invalidationArea.Inflate(painter.IntBorderThickness * 2, painter.IntBorderThickness * 2);
                    blnRefreshInvalidate = false;
                    return rectBounds;
                }
                else if (_Path == null)
                    InvalidationArea = GetShapeBounds(true);

                return invalidationArea;
            }
            set
            {
                if (value.X != int.MaxValue)
                {
                    if (blnSuppressInflate)
                    {
                        invalidationArea = value;
                    }
                    else
                    {
                        if (!blnRefreshInvalidate && editingOn)
                            invalidationArea = new Rectangle(value.X - 4, value.Y - 4,
                                  value.Width + 8 + painter.IntBorderThickness, 
                                  value.Height + 8 + painter.IntBorderThickness);
                        else if (blnRefreshInvalidate || isSelected)
                            invalidationArea = new Rectangle(value.X - 8, value.Y - 8,
                                 value.Width + 16 + painter.IntBorderThickness, 
                                 value.Height + 16 + painter.IntBorderThickness);
                        //invalidationArea.Inflate(borderWidth * 2, borderWidth * 2);
                    }
                }

            }
        }


        //---  End Properties  ----//
        #endregion

        #region Constructor
        public Shape(byte inheritedShapeProperties)
        {
            //4 is the amount of properties that this base class contains
            GdiProperties = new GDIProperties(4 + inheritedShapeProperties);
            
            //Set the base class properties
            GdiProperties.AddProperty("Edit Color",false,
                "Will generate the shape's color as variables for you to edit via " +
                "a class property. Set the value to false if you do not " +
                "wish to edit the shape's color after code generation.");

           GdiProperties.AddProperty("Edit Bounds", false,
                "Will generate a bounds variable that you will be able " +
                "to get or set the the bounds of the shape.  This applies " +
                "only to the generated code");
           
            GdiProperties.AddProperty("Edit Blend", false,
                "Will generate a custom blend variable that allows you " +
                "to edit the 'Coverage' and 'BlendSmoothness' of the " +
                "shape's gradient color.  Value will be ignored if the shape " +
                "does not have a gradient color.");
           
            GdiProperties.AddProperty("Allow Scaling", true,
                "Scaling occurs after code generation when the user " +
                "resizes the shape collection via the 'CurrentSize' " +
                "property. Set value to true if the shape is allowed " +
                "to scale itself");

            //Ask the inherited shape to add any additional properties
            SetProperties();
        }

        #endregion

        //this will tell the inherited shape to set it's additional
        //properties if it has any.
        protected abstract void SetProperties();

        public virtual void FinalizeEdit()
        { }

        public virtual Rectangle GetShapeBounds(bool inflated)
        {
            Rectangle rect = Rectangle.Ceiling(Path.GetBounds());
            if (inflated)
                rect.Inflate(painter.IntBorderThickness, painter.IntBorderThickness);
            return rect;
        }

        public void Resize(Point mousePos)
        {

            Rectangle bounds = GetShapeBounds(false);
            _Path = null;
            Painting.Shapes.Handle.eHandle handle = ResizeHandles.selectedHandle;
            switch (handle)
            {
                case Handle.eHandle.UpperLeft:
                    if (bounds.Height + (bounds.Bottom - mousePos.Y) > 5 &&
                        (bounds.Right - mousePos.X) > 5)
                    {
                        GeneratePath(new Rectangle((mousePos.X - ResizeHandles.MouseOffsetX),
                             mousePos.Y - ResizeHandles.MouseOffsetY,
                             bounds.Right - (mousePos.X - ResizeHandles.MouseOffsetX),
                             bounds.Bottom - (mousePos.Y - ResizeHandles.MouseOffsetY))); 
                    }
                    break;
                case Handle.eHandle.UpperMiddle:
                    if (bounds.Height + (bounds.Bottom - mousePos.Y ) > 5)
                        GeneratePath(new Rectangle(bounds.Left,
                             mousePos.Y - ResizeHandles.MouseOffsetY,
                             bounds.Width,bounds.Bottom - (mousePos.Y - ResizeHandles.MouseOffsetY))); 
                    break;
                case Handle.eHandle.UpperRight:
                    if (bounds.Height + (bounds.Bottom - mousePos.Y) > 5 &&
                        bounds.Width + (mousePos.X - bounds.Right) > 5)
                    {
                        GeneratePath(new Rectangle(bounds.Left, mousePos.Y - ResizeHandles.MouseOffsetY,
                        (mousePos.X + ResizeHandles.MouseOffsetX) - bounds.Left,
                        bounds.Bottom - (mousePos.Y - ResizeHandles.MouseOffsetY)));
                    }
                    break;
                case Handle.eHandle.MiddleRight:
                    if (bounds.Width + (mousePos.X - bounds.Right) > 5)
                        GeneratePath(new Rectangle(bounds.Location,
                                     new Size((mousePos.X + ResizeHandles.MouseOffsetX) - bounds.Left, bounds.Height))); 
                    break;
                case Handle.eHandle.LowerRight:
                    if ((bounds.Width + (mousePos.X - bounds.Right)) > 5 &&
                        (bounds.Height + (mousePos.Y - bounds.Bottom)) >5)
                    {
                        GeneratePath(new Rectangle(bounds.Location, new Size(
                        (mousePos.X + ResizeHandles.MouseOffsetX) - bounds.Left, 
                        (mousePos.Y + ResizeHandles.MouseOffsetY) - bounds.Top))); 
                    }
                    break;
                case Handle.eHandle.LowerMiddle:
                    if (bounds.Height + (mousePos.Y - bounds.Bottom) > 5)
                        GeneratePath(new Rectangle(bounds.Location, new Size(
                             bounds.Width, (mousePos.Y + ResizeHandles.MouseOffsetY)- bounds.Top))); 
                    break;
                case Handle.eHandle.Lowerleft:
                    if ((bounds.Height + (mousePos.Y - bounds.Bottom) > 5 &&
                        (bounds.Right - mousePos.X) > 5))
                    {
                        GeneratePath(new Rectangle((mousePos.X-ResizeHandles.MouseOffsetX),bounds.Y,
                             bounds.Right - (mousePos.X - ResizeHandles.MouseOffsetX), 
                             (mousePos.Y + ResizeHandles.MouseOffsetY) - bounds.Top)); 
                    }

                    break;
                case Handle.eHandle.MiddleLeft:
                    if ((bounds.Right - mousePos.X) > 5)
                        GeneratePath(new Rectangle((mousePos.X-ResizeHandles.MouseOffsetX),bounds.Y,
                                     bounds.Right - (mousePos.X - ResizeHandles.MouseOffsetX), bounds.Height)); 
                    break;
            }
            ResizeHandles.SetHandlePositions(GetShapeBounds(true));
            
        }

        

        public abstract void Render(Graphics g);
        public virtual void RenderHandles(Graphics g)
        {
            if (_isSelected && !editingOn && bmpMove == null)
            {
                //if the shape is selected, paint the resize handles
                ResizeHandles.Render(g);
            }
        }

        public bool NeedsPainted(RectangleF clipBounds)
        {
            if (bmpMove != null)
                return true;
            else
               return (InvalidationArea.IntersectsWith(Rectangle.Round(clipBounds))) ;
        }

        public bool NeedsPainted(Rectangle clipBounds)
        {
            if (bmpMove != null)
                return true;
            else
                return (InvalidationArea.IntersectsWith(clipBounds));
        }

        public virtual bool HitTest(MouseEventArgs e)
        {
            //we only need the Location of the mouse but by passing
            //the MouseEventArgs, no value copying has to occur.
            //Only the memory reference will be passed

            if (isSelected && InvalidationArea.Contains(e.Location))
                return true;
            else if (Path.IsVisible(e.Location))
                return true;

            return false;
        }

        public virtual EventData MouseDown(MouseEventArgs e)
        {
            eventData.WasHit = HitTest(e);

            if (eventData.WasHit)
            {
                isSelected = true;
                if (!(editingOn && !painter.State.AcceptsEditing))
                {
                    mouseOffset = new PointF(Path.GetBounds().X - e.X, Path.GetBounds().Y - e.Y);
                    pntMoveStartPos = Point.Round(Path.GetBounds().Location);
                    pntBitmapPos = pntMoveStartPos;
                    rectOldBounds = GetShapeBounds(false);
                    mouseIsPressed = true;
                    isResizing = (ResizeHandles.HitTest(e.Location));

                }
                eventData.NeedsPainted = true;
            }
            else if (isSelected)
            {
                isSelected = false;
                painter.State.IsSelected = false;
                eventData.NeedsPainted = true;
            }
            return eventData;
        }
        public virtual bool MouseMove(MouseEventArgs e)
        {
            if (isSelected && !editingOn && !mouseIsPressed)
            {
                ResizeHandles.HitTest(e.Location);
            }
            doOperation = false;
            if (mouseIsPressed)
            {
                eventData.NeedsPainted = true;
                if (isResizing)
                {
                    Rectangle oldBounds = ResizeHandles.Bounds;//InvalidationArea;
                    Resize(e.Location);
                    blnSuppressInflate = true;
                    InvalidationArea = Rectangle.Union(oldBounds,
                        GetShapeBounds(false));
                    blnSuppressInflate = false;
                    doOperation = true;
                }
                else
                {
                    if (bmpMove == null)
                    {
                        RectangleF rectBounds = Path.GetBounds();
                        Point tmpLocation = Point.Round(rectBounds.Location);
                        Size tmpSize = Size.Round(rectBounds.Size);
                        GeneratePath(new Rectangle(0, 0, (int)rectBounds.Width, (int)rectBounds.Height));

                        bmpMove = new Bitmap((int)rectBounds.Width+2, (int)rectBounds.Height+2);
                        Graphics g = Graphics.FromImage(bmpMove);
                        bool blnTemp = painter.PaintFill;
                        painter.PaintFill = false;
                        painter.Paint(g, Path);
                        painter.PaintFill = blnTemp;
                        g.Dispose(); g = null;

                        Location = tmpLocation;
                        Size = tmpSize;
                        _Path = null;
                        //sbLog.Append("Init and Paint the drag bitmap...\r\n");
                    }


                    Rectangle oldBounds = new Rectangle(pntBitmapPos, bmpMove.Size);

                    pntBitmapPos = new Point((int)(e.X + mouseOffset.X), (int)(e.Y + mouseOffset.Y));
                    InvalidationArea = Rectangle.Union(oldBounds,
                         new Rectangle(pntBitmapPos, bmpMove.Size));
                    doOperation = true;
                   // sbLog.Append("BitmapPos: " + pntBitmapPos.X + "," + pntBitmapPos.Y + ";   Invalidation: " + _InvalidationArea.ToString() + "\r\n");
                }
            }
            return doOperation;
        }

        public virtual EventData MouseUp(MouseEventArgs e)
        {
            mouseIsPressed = false;
            eventData.WasHit = HitTest(e);

            if (isResizing)
            {
                isResizing = false;
                painter.State.IsResizing = false;
                this.ResizeHandles.SetHandlePositions(Path.GetBounds());
                InvalidationArea = (Rectangle.Ceiling(Path.GetBounds()));
                eventData.NeedsPainted = true;
            }

            if (bmpMove != null)
            {
                bmpMove = null;
                _Path = null;

                _Location = pntBitmapPos;
                _Size = rectOldBounds.Size;
                GeneratePath(new Rectangle(pntBitmapPos.X, pntBitmapPos.Y,
                             rectOldBounds.Width, rectOldBounds.Height));

                this.ResizeHandles.SetHandlePositions(Path.GetBounds());
                InvalidationArea = (Rectangle.Ceiling(Path.GetBounds()));

                eventData.NeedsPainted = true;

            }

            return eventData;
        }


        #region IComparable Members

        public int CompareTo(object obj)
        {
            return zOrder.CompareTo(((Shape)obj).zOrder);
        }

        #endregion

        #region IRestoreNonSerializable Members

        public void RestoreNonSerializable()
        {
            ResizeHandles = new Handle();
            eventData = new EventData();
            invalidationArea = GetShapeBounds(true);
            Restore_NonSerialized();
        }

        #endregion
    }
}
