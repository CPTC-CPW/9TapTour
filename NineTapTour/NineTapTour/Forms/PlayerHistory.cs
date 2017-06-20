using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Configuration;
using System.Data.Entity.Core.Objects;

namespace NineTapTour.Database
{
    public partial class PlayerHistory : Form
    {
        private int id;


        public PlayerHistory()
        {
            InitializeComponent();
        }

        public PlayerHistory(int id)
        {
            this.id = id;

            Member currentMember = MemberDb.GetMember(id);
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void lblFullName_Click(object sender, EventArgs e)
        {

        }
    }
}
