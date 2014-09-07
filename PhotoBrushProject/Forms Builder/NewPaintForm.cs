using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Painting.Shapes;

namespace PhotoBrushProject
{
    public partial class NewPaintForm : Form
    {
        string FileName = "";
        MainForm MF = null;
        PaintHandlerForm PHF = null;

        public NewPaintForm(MainForm MF)
        {
            this.MF = MF;
            PHF = new PaintHandlerForm(MF);
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog SFD = new SaveFileDialog();
                SFD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                SFD.Filter = "Photo Brush Project File (*.pbp)|*.pbp|All Files (*.*)|*.*";
                SFD.DefaultExt = ".pbp";
                if (SFD.ShowDialog(this) == DialogResult.OK)
                {
                    FileName = SFD.FileName;
                    // Save Algorithm
                    FileSerializer.Serialize<ShapeManager>(SFD.FileName, new ShapeManager(PHF.drawingCanvas1));
                    PHF.FilePath = SFD.FileName;
                    FileLocationtextBox1.Text = SFD.InitialDirectory;
                    FileNametextBox2.Text = SFD.FileName;
                }
            }
            catch (Exception E) { MessageBox.Show(E.Message, "Error File", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (FileLocationtextBox1.Text == "" || FileNametextBox2.Text == "")
            {
                DialogResult DR = MessageBox.Show("Do you want to continue without saving file?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                PHF.Text = "Untitled";
                if (DR == System.Windows.Forms.DialogResult.No)
                    return;
            }
            else
                PHF.Text = FileNametextBox2.Text.Remove(0, FileLocationtextBox1.Text.Length + 1);
            MF.Initialize(PHF, FormStyle.PaintHandleForm);
            this.Dispose(true);
        }
    }
}
