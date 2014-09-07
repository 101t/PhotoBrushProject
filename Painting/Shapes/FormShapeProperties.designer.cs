namespace Painting.Shapes
{
    partial class FormShapeProperties
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Rectangle",
            "Rectangle1"}, -1);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpSelectedShape = new System.Windows.Forms.TabPage();
            this.txtPropExplain1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkLstProperties1 = new System.Windows.Forms.CheckedListBox();
            this.btnUpdate1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtShapeName1 = new System.Windows.Forms.TextBox();
            this.tpAllShapes = new System.Windows.Forms.TabPage();
            this.txtPropExplain2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkLstProperties2 = new System.Windows.Forms.CheckedListBox();
            this.btnCancel2 = new System.Windows.Forms.Button();
            this.btnUpdate2 = new System.Windows.Forms.Button();
            this.lv = new System.Windows.Forms.ListView();
            this.chShapeType = new System.Windows.Forms.ColumnHeader();
            this.chShapeName = new System.Windows.Forms.ColumnHeader();
            this.label2 = new System.Windows.Forms.Label();
            this.txtShapeName2 = new System.Windows.Forms.TextBox();
            this.pnlSelectedShapeBack = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tpSelectedShape.SuspendLayout();
            this.tpAllShapes.SuspendLayout();
            this.pnlSelectedShapeBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpSelectedShape);
            this.tabControl1.Controls.Add(this.tpAllShapes);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(402, 252);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tpSelectedShape
            // 
            this.tpSelectedShape.Controls.Add(this.pnlSelectedShapeBack);
            this.tpSelectedShape.Location = new System.Drawing.Point(4, 22);
            this.tpSelectedShape.Name = "tpSelectedShape";
            this.tpSelectedShape.Padding = new System.Windows.Forms.Padding(3);
            this.tpSelectedShape.Size = new System.Drawing.Size(394, 226);
            this.tpSelectedShape.TabIndex = 0;
            this.tpSelectedShape.Text = "Selected Shape";
            this.tpSelectedShape.UseVisualStyleBackColor = true;
            // 
            // txtPropExplain1
            // 
            this.txtPropExplain1.BackColor = System.Drawing.Color.White;
            this.txtPropExplain1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPropExplain1.Location = new System.Drawing.Point(186, 111);
            this.txtPropExplain1.Multiline = true;
            this.txtPropExplain1.Name = "txtPropExplain1";
            this.txtPropExplain1.ReadOnly = true;
            this.txtPropExplain1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPropExplain1.Size = new System.Drawing.Size(178, 79);
            this.txtPropExplain1.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "GDI Generation Properties";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(301, 46);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(51, 22);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkLstProperties1
            // 
            this.chkLstProperties1.FormattingEnabled = true;
            this.chkLstProperties1.Items.AddRange(new object[] {
            "Write Color Variable",
            "Write Bounds Scalable",
            "Write Bounds Variable",
            "Write Blend Variable"});
            this.chkLstProperties1.Location = new System.Drawing.Point(16, 111);
            this.chkLstProperties1.Name = "chkLstProperties1";
            this.chkLstProperties1.Size = new System.Drawing.Size(161, 79);
            this.chkLstProperties1.TabIndex = 8;
            this.chkLstProperties1.SelectedIndexChanged += new System.EventHandler(this.chkLstProperties1_SelectedIndexChanged);
            // 
            // btnUpdate1
            // 
            this.btnUpdate1.Location = new System.Drawing.Point(244, 46);
            this.btnUpdate1.Name = "btnUpdate1";
            this.btnUpdate1.Size = new System.Drawing.Size(51, 22);
            this.btnUpdate1.TabIndex = 6;
            this.btnUpdate1.Text = "Update";
            this.btnUpdate1.UseVisualStyleBackColor = true;
            this.btnUpdate1.Click += new System.EventHandler(this.btnUpdate1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // txtShapeName1
            // 
            this.txtShapeName1.Location = new System.Drawing.Point(66, 48);
            this.txtShapeName1.Name = "txtShapeName1";
            this.txtShapeName1.Size = new System.Drawing.Size(161, 20);
            this.txtShapeName1.TabIndex = 0;
            // 
            // tpAllShapes
            // 
            this.tpAllShapes.Controls.Add(this.txtPropExplain2);
            this.tpAllShapes.Controls.Add(this.label4);
            this.tpAllShapes.Controls.Add(this.chkLstProperties2);
            this.tpAllShapes.Controls.Add(this.btnCancel2);
            this.tpAllShapes.Controls.Add(this.btnUpdate2);
            this.tpAllShapes.Controls.Add(this.lv);
            this.tpAllShapes.Controls.Add(this.label2);
            this.tpAllShapes.Controls.Add(this.txtShapeName2);
            this.tpAllShapes.Location = new System.Drawing.Point(4, 22);
            this.tpAllShapes.Name = "tpAllShapes";
            this.tpAllShapes.Padding = new System.Windows.Forms.Padding(3);
            this.tpAllShapes.Size = new System.Drawing.Size(394, 226);
            this.tpAllShapes.TabIndex = 1;
            this.tpAllShapes.Text = "All Shapes";
            this.tpAllShapes.UseVisualStyleBackColor = true;
            // 
            // txtPropExplain2
            // 
            this.txtPropExplain2.BackColor = System.Drawing.Color.White;
            this.txtPropExplain2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPropExplain2.Location = new System.Drawing.Point(206, 135);
            this.txtPropExplain2.Multiline = true;
            this.txtPropExplain2.Name = "txtPropExplain2";
            this.txtPropExplain2.ReadOnly = true;
            this.txtPropExplain2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPropExplain2.Size = new System.Drawing.Size(178, 79);
            this.txtPropExplain2.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "GDI Generation Properties";
            // 
            // chkLstProperties2
            // 
            this.chkLstProperties2.FormattingEnabled = true;
            this.chkLstProperties2.Items.AddRange(new object[] {
            "Write Color Variable",
            "Write Bounds Scalable",
            "Write Bounds Variable",
            "Write Blend Variable"});
            this.chkLstProperties2.Location = new System.Drawing.Point(39, 135);
            this.chkLstProperties2.Name = "chkLstProperties2";
            this.chkLstProperties2.Size = new System.Drawing.Size(161, 79);
            this.chkLstProperties2.TabIndex = 11;
            this.chkLstProperties2.SelectedIndexChanged += new System.EventHandler(this.chkLstProperties2_SelectedIndexChanged);
            // 
            // btnCancel2
            // 
            this.btnCancel2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel2.Location = new System.Drawing.Point(313, 88);
            this.btnCancel2.Name = "btnCancel2";
            this.btnCancel2.Size = new System.Drawing.Size(51, 22);
            this.btnCancel2.TabIndex = 10;
            this.btnCancel2.Text = "Cancel";
            this.btnCancel2.UseVisualStyleBackColor = true;
            // 
            // btnUpdate2
            // 
            this.btnUpdate2.Location = new System.Drawing.Point(256, 88);
            this.btnUpdate2.Name = "btnUpdate2";
            this.btnUpdate2.Size = new System.Drawing.Size(51, 22);
            this.btnUpdate2.TabIndex = 5;
            this.btnUpdate2.Text = "Update";
            this.btnUpdate2.UseVisualStyleBackColor = true;
            this.btnUpdate2.Click += new System.EventHandler(this.btnUpdate2_Click);
            // 
            // lv
            // 
            this.lv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chShapeType,
            this.chShapeName});
            this.lv.FullRowSelect = true;
            this.lv.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.lv.Location = new System.Drawing.Point(48, 4);
            this.lv.MultiSelect = false;
            this.lv.Name = "lv";
            this.lv.Size = new System.Drawing.Size(294, 78);
            this.lv.TabIndex = 4;
            this.lv.UseCompatibleStateImageBehavior = false;
            this.lv.View = System.Windows.Forms.View.Details;
            this.lv.SelectedIndexChanged += new System.EventHandler(this.lv_SelectedIndexChanged);
            // 
            // chShapeType
            // 
            this.chShapeType.Text = "Shape Type";
            this.chShapeType.Width = 104;
            // 
            // chShapeName
            // 
            this.chShapeName.Text = "Shape Name";
            this.chShapeName.Width = 100;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Name:";
            // 
            // txtShapeName2
            // 
            this.txtShapeName2.Location = new System.Drawing.Point(63, 88);
            this.txtShapeName2.Name = "txtShapeName2";
            this.txtShapeName2.Size = new System.Drawing.Size(161, 20);
            this.txtShapeName2.TabIndex = 2;
            // 
            // pnlSelectedShapeBack
            // 
            this.pnlSelectedShapeBack.Controls.Add(this.txtShapeName1);
            this.pnlSelectedShapeBack.Controls.Add(this.txtPropExplain1);
            this.pnlSelectedShapeBack.Controls.Add(this.label1);
            this.pnlSelectedShapeBack.Controls.Add(this.label3);
            this.pnlSelectedShapeBack.Controls.Add(this.btnUpdate1);
            this.pnlSelectedShapeBack.Controls.Add(this.chkLstProperties1);
            this.pnlSelectedShapeBack.Controls.Add(this.btnCancel);
            this.pnlSelectedShapeBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSelectedShapeBack.Location = new System.Drawing.Point(3, 3);
            this.pnlSelectedShapeBack.Name = "pnlSelectedShapeBack";
            this.pnlSelectedShapeBack.Size = new System.Drawing.Size(388, 220);
            this.pnlSelectedShapeBack.TabIndex = 16;
            // 
            // frmShapeProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 252);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmShapeProperties";
            this.Text = "Shape Properties";
            this.Load += new System.EventHandler(this.frmShapeProperties_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmShapeProperties_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tpSelectedShape.ResumeLayout(false);
            this.tpAllShapes.ResumeLayout(false);
            this.tpAllShapes.PerformLayout();
            this.pnlSelectedShapeBack.ResumeLayout(false);
            this.pnlSelectedShapeBack.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpSelectedShape;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtShapeName1;
        private System.Windows.Forms.TabPage tpAllShapes;
        private System.Windows.Forms.ListView lv;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtShapeName2;
        private System.Windows.Forms.Button btnUpdate1;
        private System.Windows.Forms.Button btnUpdate2;
        private System.Windows.Forms.ColumnHeader chShapeType;
        private System.Windows.Forms.ColumnHeader chShapeName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckedListBox chkLstProperties1;
        private System.Windows.Forms.Button btnCancel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckedListBox chkLstProperties2;
        private System.Windows.Forms.TextBox txtPropExplain1;
        private System.Windows.Forms.TextBox txtPropExplain2;
        private System.Windows.Forms.Panel pnlSelectedShapeBack;
    }
}