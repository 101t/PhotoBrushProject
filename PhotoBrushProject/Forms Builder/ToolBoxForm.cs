using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PhotoBrushProject
{
    /// <summary>
    /// Singleton Design Pattern Implemented
    /// </summary>
    public partial class ToolBoxForm : Form, IFormBuilder
    {
        private static ToolBoxForm instance = null;
        public static ToolBoxForm Instance { get { return instance == null ? instance = new ToolBoxForm() : instance; } }
        Rectangle IFormBuilder.Bounds { get { return this.Bounds; } set { this.Bounds = value; } }
        FormBorderStyle IFormBuilder.FormBorderStyled { get { return this.FormBorderStyle; } set { this.FormBorderStyle = value; } }
        Color IFormBuilder.BackColor { get { return this.BackColor; } set { this.BackColor = value; } }
        string IFormBuilder.Text { get { return this.Text; } set { this.Text = value; } }
        bool IFormBuilder.TopLevel { get { return this.TopLevel; } set { this.TopLevel = value; } }
        private ToolBoxForm()
        {
            InitializeComponent();
        }
    }
}
