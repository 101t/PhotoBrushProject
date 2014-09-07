using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AnimatorNS;
using System.Threading;

namespace PhotoBrushProject
{
    /// <summary>
    /// Singleton Design Pattern Implemented
    /// </summary>
    public partial class StartPageForm : Form, IFormBuilder
    {
        private static StartPageForm instance = null;
        MainForm MF = null;
        public static StartPageForm Instance { get { return instance == null ? instance = new StartPageForm() : instance; } }
        public void SetMainForm(MainForm MF) 
        {
            this.MF = MF;
        }

        Rectangle IFormBuilder.Bounds { get { return this.Bounds; } set { this.Bounds = value; } }
        FormBorderStyle IFormBuilder.FormBorderStyled { get { return this.FormBorderStyle; } set { this.FormBorderStyle = value; } }
        Color IFormBuilder.BackColor { get { return this.BackColor; } set { this.BackColor = value; } }
        string IFormBuilder.Text { get { return this.Text; } set { this.Text = value; } }
        bool IFormBuilder.TopLevel { get { return this.TopLevel; } set { this.TopLevel = value; } }

        private StartPageForm()
        {
            InitializeComponent();
            //this.Activated += new EventHandler(StartPageForm_Activated);
            //this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            DoAnimation();
        }

        private void DoAnimation()
        {
            Control AnimatePanel = panel1;
            new Thread(() =>
                {
                    AnimatePanel.Hide();
                    //animator1.ShowSync(AnimatePanel, true, Animation.VertSlide);

                    //wait while all animations will be completed
                    animator1.WaitAllAnimations();
                    //animator1.DefaultAnimation = Animation.Scale;
                    animator1.Show(AnimatePanel, true, Animation.ScaleAndHorizSlide);
                }).Start();
            Control GettingStartedPanel = panel2;
            new Thread(() =>
                {
                    GettingStartedPanel.Hide();
                    animator1.WaitAllAnimations();
                    animator1.Show(GettingStartedPanel, false, Animation.Transparent);
                }).Start();
        }

        private void LinkLabel01NewPaint_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MF != null)
            {
                MF.NewFile_Click(sender, e);
            }
        }

        private void LinkLabel02OpenProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MF != null)
            {
                MF.OpenFile_Click(sender, e);
            }
        }

        private void StartPageForm_Activated(object sender, System.EventArgs e)
        {
            MF.ChildFormEnableTools(FormStyle.StartPage);
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Forms_Builder.AboutPB PB = new Forms_Builder.AboutPB();
            PB.ShowDialog();
        }
    }
}
