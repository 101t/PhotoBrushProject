////////////////////////////////////////////////////////////////
//                Created By Richard Blythe 2008
//   There are no licenses or warranty attached to this object.
//   If you distribute the code as part of your application, please
//   be courteous enough to mention the assistance provided you.
//   Enhanced and Re-Developed By Tarek MOH Omer Kala'ajy
////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using Painting.Painter;
using System.IO;

namespace Painting.Shapes
{
    [Serializable]
    public partial class DrawingCanvas : UserControl
    {
        public ShapeManager shapeManager;

        public DrawingCanvas()
        {
            InitializeComponent();
            shapeManager = new ShapeManager(this);

            this.SetStyle(ControlStyles.DoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint,
                          true);
            this.UpdateStyles();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            shapeManager.PaintShapes(e.Graphics);
        }

        public bool Save(bool blnSaveAs)
        {
            bool blnSuccess = false;
            string strFileName = shapeManager.FileName;
            try
            {
                if (blnSaveAs || shapeManager.FileName == "")
                {
                    using (SaveFileDialog SFD = new SaveFileDialog())
                    {
                        SFD.Filter = "Photo Brush Project File (*.pbp)|*.pbp";
                        SFD.DefaultExt = ".pbp";
                        SFD.Title = "Save Photo Brush Project File";
                        SFD.ShowDialog();
                        shapeManager.FileName = SFD.FileName;
                    }
                }
                if (shapeManager.FileName != "")
                {
                    if (shapeManager.FileName == "" || shapeManager.FileName == null)
                        return false;
                    using (FileStream FS = new FileStream(shapeManager.FileName, FileMode.Create, FileAccess.Write, FileShare.Write))
                    {
                        BinaryFormatter BF = new BinaryFormatter();
                        BF.Serialize(FS, shapeManager);
                        blnSuccess = true;
                    }
                }
                if (!blnSuccess)
                    shapeManager.FileName = strFileName;
                else if (blnSaveAs)
                    MessageBox.Show("File was saved", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving the file.\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (blnSuccess)
                shapeManager.IsDirty = false;
            return blnSuccess;
        }

        //public bool Open()
        //{
        //    bool blnSuccess = false;
        //    using (OpenFileDialog OFD = new OpenFileDialog())
        //    {
        //        try
        //        {
        //            OFD.Filter = "Photo Brush Project File (*.pbp)|*.pbp";
        //            OFD.DefaultExt = ".pbp";
        //            OFD.Title = "Open Photo Brush Project File";
        //            OFD.ShowDialog();

        //            if (OFD.FileName != "")
        //            {
        //                shapeManager.FileName = OFD.FileName;
        //                using (FileStream FS = new FileStream(shapeManager.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        //                {
        //                    BinaryFormatter BF = new BinaryFormatter();
        //                    ShapeManager TMP = (ShapeManager)BF.Deserialize(FS);
        //                    TMP.RestoreNonSerializable(this);
        //                    shapeManager.Dispose();
        //                    shapeManager = null;
        //                    shapeManager = TMP;
        //                }
        //                blnSuccess = true;
        //                this.Invalidate();
        //            }
        //        }
        //        catch (Exception E) { MessageBox.Show(E.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        //    }
        //    return blnSuccess;
        //}
        
        public bool Open()
        {
            bool blnSuccess = false;
            try
            {
                if (shapeManager.FileName != "")
                {
                    //shapeManager.FileName = FilePath;
                    using (FileStream FS = new FileStream(shapeManager.FileName, FileMode.OpenOrCreate))//new FileStream(shapeManager.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        BinaryFormatter BF = new BinaryFormatter();
                        ShapeManager TMP = (ShapeManager)BF.Deserialize(FS);
                        TMP.RestoreNonSerializable(this);
                        shapeManager.Dispose();
                        shapeManager = null;
                        shapeManager = TMP;
                    }
                    blnSuccess = true;
                    this.Invalidate();
                }
            }
            catch (Exception E) { MessageBox.Show(E.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return blnSuccess;
        }

        public void ShowPropertyEditor()
        {
            FormShapeProperties FSP = new FormShapeProperties(shapeManager);
            FSP.ShowDialog();
            FSP.Dispose();
        }

        public void ShowColorEditor()
        {
            if (shapeManager.SelectedIndex != -1 && !(shapeManager.GetSelectedShape() is ShapeClipRectangle))
            {
                FormColorEditor FCE = new FormColorEditor(shapeManager.GetSelectedShape().painter);
                FCE.ShowDialog();
                FCE.Dispose();
            }
        }

        private void DrawingCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                case Keys.Enter:
                    shapeManager.FinalizeShape();
                    break;
                case Keys.Delete:
                    shapeManager.Delete();
                    break;
            }
            if (e.KeyCode == Keys.C && Control.ModifierKeys == Keys.Control)
            {
                shapeManager.Copy();
            }
            else if (e.KeyCode == Keys.V && Control.ModifierKeys == Keys.Control)
            {
                shapeManager.Paste();
            }
        }
    }

    public class ShapeMenuItem
    {
        public string ShapeName;
        public byte ShapeType;
        public ShapeMenuItem(string name, byte type)
        {
            ShapeName = name;
            ShapeType = type;
        }
    }
}
