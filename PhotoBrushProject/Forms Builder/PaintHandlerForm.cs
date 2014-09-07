using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Painting.Shapes;
using AnimatorNS;
using System.Threading;

namespace PhotoBrushProject
{
    public partial class PaintHandlerForm : Form, IFormBuilder
    {
        MainForm MF = null;
        public string FilePath = "";

        Rectangle IFormBuilder.Bounds { get { return this.Bounds; } set { this.Bounds = value; } }
        FormBorderStyle IFormBuilder.FormBorderStyled { get { return this.FormBorderStyle; } set { this.FormBorderStyle = value; } }
        Color IFormBuilder.BackColor { get { return this.BackColor; } set { this.BackColor = value; } }
        string IFormBuilder.Text { get { return this.Text; } set { this.Text = value; } }
        bool IFormBuilder.TopLevel { get { return this.TopLevel; } set { this.TopLevel = value; } }

        private Animator animator1;
        public Painting.Shapes.DrawingCanvas drawingCanvas1;
        private ShapeControlPanel shapeControlPanel1;

        public PaintHandlerForm(MainForm MF)
        {
            this.MF = MF;
            InitializeComponent();
            animator1 = new Animator();
            //this.MF.DockContainer01Panel.
            this.MouseEnter += new EventHandler(PaintHandlerForm_MouseEnter);
            this.FormClosing += new FormClosingEventHandler(PaintHandlerForm_FormClosing);
            this.drawingCanvas1.MouseMove += new MouseEventHandler(drawingCanvas1_MouseMove);
            this.drawingCanvas1.MouseClick += new MouseEventHandler(drawingCanvas1_MouseClick);
            this.Shown += new EventHandler(PaintHandlerForm_Shown);
        }

        void PaintHandlerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (drawingCanvas1.shapeManager.IsDirty)
            {
                DialogResult DR = MessageBox.Show("Would you like to save your work?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                switch (DR)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        drawingCanvas1.Save(false);
                        MF.EnableActivatedEventDictionary[FormStyle.PaintHandleForm]--;
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        MF.EnableActivatedEventDictionary[FormStyle.PaintHandleForm]--;
                        break;
                    //case System.Windows.Forms.DialogResult.Cancel:
                    //    e.Cancel = true;
                    //    return;
                }
            }
        }

        void PaintHandlerForm_Shown(object sender, EventArgs e)
        {
            DoAnimation();
        }

        private void DoAnimation()
        {
            Control AnimatePanel = shapeControlPanel1;
            new Thread(() =>
            {
                Invoke((MethodInvoker)delegate
                {
                    AnimatePanel.Hide();
                });
                //animator1.ShowSync(AnimatePanel, true, Animation.VertSlide);

                //wait while all animations will be completed
                animator1.WaitAllAnimations();
                //animator1.DefaultAnimation = Animation.Scale;
                animator1.Show(AnimatePanel, true, Animation.HorizSlide);
                
            }).Start();
        }

        void drawingCanvas1_MouseMove(object sender, MouseEventArgs e)
        {
            Int32 S32X = e.X;
            Int32 S32Y = e.Y;
            MF.PositionToolStripStatusLabel.Text = S32X + " x " + S32Y;
        }

        void drawingCanvas1_MouseClick(object sender, MouseEventArgs e)
        {
            MF.ToolStripMenuItem13Color.Enabled = drawingCanvas1.shapeManager.ShapeIsSelected();
        }

        public bool EnableActive = true;
        void PaintHandlerForm_MouseEnter(object sender, EventArgs e)
        {
            MF.ChildFormEnableTools(FormStyle.PaintHandleForm);
            MF.ActivePHF = this;
            if (EnableActive)
            {
                MF.ChildFormDesigner(FormStyle.PaintHandleForm);
                EnableActive = false;
            }
        }

        public void AntiAlias_Click(object sender, EventArgs e)
        {
            if (!drawingCanvas1.shapeManager.AntiAlias)
            {
                drawingCanvas1.shapeManager.AntiAlias = true;
            }
            else
            {
                drawingCanvas1.shapeManager.AntiAlias = false;
            }
        }

        public void ShowClippingRectangleIsOn_Click(object sender, EventArgs e)
        {
            if (!drawingCanvas1.shapeManager.ClipRectIsOn)
            {
                drawingCanvas1.shapeManager.ClipRectIsOn = true;
            }
            else
            {
                drawingCanvas1.shapeManager.ClipRectIsOn = false;
            }
        }

        public void DeleteShape_Click(object sender, EventArgs e)
        {
            drawingCanvas1.shapeManager.DoOperation(eOperation.Delete);
        }

        public void ShapesProperties_Click(object sender, EventArgs e)
        {
            drawingCanvas1.ShowPropertyEditor();
        }

        public void ColorEditor_Click(object sender, EventArgs e)
        {
            drawingCanvas1.ShowColorEditor();
        }

        public void ConvertToCurve_Click(object sender, EventArgs e)
        {
            drawingCanvas1.shapeManager.DoOperation(eOperation.ConvertToCurve);
        }

        public void ConvertToLine_Click(object sender, EventArgs e)
        {
            drawingCanvas1.shapeManager.DoOperation(eOperation.ConvertToLine);
        }

        public void DeleteNode_Click(object sender, EventArgs e)
        {
            drawingCanvas1.shapeManager.DoOperation(eOperation.Delete);
        }

        public void SelectionTool_Click(object sender, EventArgs e)
        {
            if (!MF.ToolStripButton17SelectionTool.Checked)
            {
                MF.ToolStripButton18NodeTool.Checked = false;
                MF.ToolStripButton17SelectionTool.Checked = true;
            }
            drawingCanvas1.shapeManager.CurrentTool = Tools.eTool.Selection;
        }

        public void NodeTool_Click(object sender, EventArgs e)
        {
            if (!MF.ToolStripButton18NodeTool.Checked)
            {
                MF.ToolStripButton18NodeTool.Checked = true;
                MF.ToolStripButton17SelectionTool.Checked = false;
            }
            drawingCanvas1.shapeManager.CurrentTool = Tools.eTool.Edit;
        }

        public void BringToFront_Click(object sender, EventArgs e)
        {
            drawingCanvas1.shapeManager.DoOperation(eOperation.BringToFront);
        }

        public void SendToBack_Click(object sender, EventArgs e)
        {
            drawingCanvas1.shapeManager.DoOperation(eOperation.SendToBack);
        }

        public void CopyShape_Click(object sender, EventArgs e)
        {
            drawingCanvas1.shapeManager.Copy();
        }

        public void PastShape_Click(object sender, EventArgs e)
        {
            drawingCanvas1.shapeManager.Paste();
        }

        public void SaveFile_Click(object sender, EventArgs e)
        {
            drawingCanvas1.Save(false);
        }

        public void SaveAsFile_Click(object sender, EventArgs e)
        {
            drawingCanvas1.Save(true);
        }
    }
}
