using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PhotoBrushProject
{
    /// <summary>
    /// Builder Design Pattern Implemented
    /// </summary>
    public partial interface IFormBuilder
    {
        Rectangle Bounds { get; set; }
        FormBorderStyle FormBorderStyled { get; set; }
        Color BackColor { get; set; }
        string Text { get; set; }
        bool TopLevel { get; set; }
    }
}
