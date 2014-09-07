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
    /// Copyright © Bridge Team & Tarek Itien software engineering 2013
    /// </summary>
    public partial class MottoSplash : Form
    {
        #region < Fields >
        int i = 0;
        string status = "Initializing";
        #endregion

        #region < Constructors >
        public MottoSplash()
        {
            InitializeComponent();
        }
        #endregion

        #region < Event Methods >
        private void timer1_Tick(object sender, EventArgs e)
        {
                Percentagelabel1.Text = String.Format("{0} %  {1}", i.ToString(), status);
                if (i == 2) status = "Starting ...";
                if (i == 12) status = "Starting  ...";
                if (i == 22) status = "Starting   ...";
                if (i == 32) status = "Starting    ...";
                if (i == 42) status = "Initialize Components ...";
                if (i == 52) status = "Initialize Components  ...";
                if (i == 62) status = "Initialize Tools   ...";
                if (i == 72) status = "Initialize Tools    ...";
                if (i == 82) status = "Loading File ...";
                if (i == 92) status = "Loading File   ...";
                if (i == 100) status = "Complete ";
                if (i == 100) this.Close();
                i++;
        }
        #endregion
    }
}
