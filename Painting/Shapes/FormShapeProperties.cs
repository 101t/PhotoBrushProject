////////////////////////////////////////////////////////////////
//                Created By Richard Blythe 2008
//   There are no licenses or warranty attached to this object.
//   If you distribute the code as part of your application, please
//   be courteous enough to mention the assistance provided you.
////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Painting.Shapes
{
    public partial class FormShapeProperties : Form
    {
        ShapeManager refManager;
        int intSelectedIndex = -1;


        public FormShapeProperties(ShapeManager manager)
        {
            InitializeComponent();
            refManager = manager;
        }

        private void frmShapeProperties_Load(object sender, EventArgs e)
        {
            if (refManager.GetSelectedShape() is ShapeClipRectangle ||
                refManager.SelectedIndex == -1)
            {
                pnlSelectedShapeBack.Enabled = false;
                tabControl1.SelectedIndex = 1; //All Shapes
            }
            else
            {
                txtShapeName1.Text = refManager.GetSelectedShape().Name;
                UpdateChkList(chkLstProperties1, refManager.GetSelectedShape());
            
            }
       }

        #region ViewAllShapes Code
        private void lv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv.SelectedIndices.Count != 0)
            {
                Shape shape = null;
                if (intSelectedIndex != -1)
                {
                    lv.Items[intSelectedIndex].BackColor = Color.White;
                    lv.Items[intSelectedIndex].ForeColor = Color.Black;

                    shape = refManager.GetShape((short)lv.Items
                        [intSelectedIndex].SubItems[0].Tag);

                    UpdateShape(chkLstProperties2, shape);
                }

                intSelectedIndex = lv.SelectedIndices[0];
                txtShapeName2.Text = lv.SelectedItems[0].SubItems[1].Text;
                lv.Items[intSelectedIndex].BackColor = Color.FromArgb(49, 106, 197);
                lv.Items[intSelectedIndex].ForeColor = Color.White;
                //update the checked listbox with the selected shape's properties
                shape = refManager.GetShape((short)lv.SelectedItems[0].SubItems[0].Tag);
                UpdateChkList(chkLstProperties2, shape);
                txtPropExplain2.Text = "";
            }
        }

        private void btnUpdate2_Click(object sender, EventArgs e)
        {
            if (intSelectedIndex == -1)
            {
                MessageBox.Show("Please select a shape to edit.");
                return;
            }

            if (updateShapeName(
                (short)lv.Items[intSelectedIndex].SubItems[0].Tag,
                txtShapeName2.Text))
            {
                lv.Items[intSelectedIndex].SubItems[1].Text = txtShapeName2.Text;
            }

        }


        private bool updateShapeName(short index, string strName)
        {
            string strError = null;

            if (!refManager.ChangeName(index, strName, out strError))
            {
                MessageBox.Show(strError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0 && pnlSelectedShapeBack.Enabled)
            {
                txtShapeName1.Text = refManager.GetSelectedShape().Name;
                UpdateChkList(chkLstProperties1, refManager.GetSelectedShape());
            }
            if (tabControl1.SelectedIndex == 1)
            {
                lv.Items.Clear();
                Shape shape = null;
                int intCount = 0;
                if (refManager.ClipRectIsOn)
                    intCount = refManager.ShapeCount - 2;
                else
                    intCount = refManager.ShapeCount;
                for (short i = 0; i < intCount; i++)
                {
                    shape = refManager.GetShape(i);
                    ListViewItem item = new ListViewItem(shape.ShapeType);
                    item.SubItems.Add(shape.Name);
                    item.SubItems[0].Tag = i;
                    lv.Items.Add(item);
                }
                intSelectedIndex = -1;
                txtShapeName2.Text = "";
                chkLstProperties2.Items.Clear();
            }
        }
        #endregion

        private void btnUpdate1_Click(object sender, EventArgs e)
        {
            if (updateShapeName(refManager.SelectedIndex, txtShapeName1.Text))
            {
                this.Close();
            }
        }

        private void frmShapeProperties_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {   //updates the gdi properties from the checked list box
                UpdateShape(chkLstProperties1, refManager.GetSelectedShape());
            }
        }


        private void UpdateShape(CheckedListBox chkLst, Shape shape)
        {
            int intCount = shape.GDIPropertyCount;
            for (short i = 0; i < intCount; i++)
            {
               shape.SetGDIValue(i, chkLst.GetItemChecked(i));
            }
        }

        private void UpdateChkList(CheckedListBox chkLst, Shape shape)
        {
            chkLst.Items.Clear();
            int intCount = shape.GDIPropertyCount;
            for (short i = 0; i < intCount; i++)
            {
                chkLst.Items.Add(shape.GetPropertyName(i));
                chkLst.SetItemChecked(i,
                    (bool)shape.GetGDIValue(i));
            }
        }

        private void chkLstProperties1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPropExplain1.Text = refManager.GetSelectedShape().
                GetPropertyDescription(chkLstProperties1.SelectedIndex);
        }

        private void chkLstProperties2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (intSelectedIndex != -1 && chkLstProperties2.SelectedIndex != -1)
                txtPropExplain2.Text = refManager.GetShape(intSelectedIndex).
                    GetPropertyDescription(chkLstProperties2.SelectedIndex);
        }

    }
}