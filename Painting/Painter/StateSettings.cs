using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;

namespace Painting.Painter
{
    /// <summary>
    /// Stores a collection of values that control the state of the painter.
    /// </summary>
    [Serializable]
    public class StateSettings
    {
        private bool isSelected;
        /// <summary>
        /// gets or sets whether or not the parent control is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        private bool isEditing;
        /// <summary>
        /// gets or sets whether or not the parent control is being edited.
        /// </summary>
        public bool IsEditing
        {
            get { return isEditing; }
            set { isEditing = value; }
        }

        private bool acceptsEditing = false;
        /// <summary>
        /// Gets or sets if the parent shape can be rendered as a shape
        /// in edit mode.  Some shapes such as ShapeRectangle cannot be
        /// edited. They can only be resized.
        /// </summary>
        public bool AcceptsEditing
        {
            get { return acceptsEditing; }
            set { acceptsEditing = value; }
        }

        private bool isResizing;
        /// <summary>
        /// gets or sets whether or not we are resizing.
        /// </summary>
        public bool IsResizing
        {
            get {return isResizing;}
            set {isResizing = value;}
        }


        private bool paintOutline = true;
        /// <summary>
        /// gets or sets whether or not the outline will be painted.
        /// By default it is true
        /// </summary>
        public bool PaintOutline
        {
            get {return paintOutline ;}
            set {paintOutline = value;}
        }

        private DashStyle dashStyle = DashStyle.Solid;
        /// <summary>
        /// gets or sets whether or not the outline will be
        /// drawn as a solid line or a dashed line. By default
        /// it will draw the resize bounds as a dashed line
        /// </summary>
        public System.Drawing.Drawing2D.DashStyle DashStyle
        {
            get {return dashStyle;}
            set {dashStyle = value;}
        }

        private bool paintFill;
        /// <summary>
        /// gets or sets whether or not the fillColor will be painted.
        /// By default it is false
        /// </summary>
        public bool PaintFill
        {
            get {return paintFill;}
            set {paintFill = value;}
        }

    }
}
