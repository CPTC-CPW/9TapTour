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
    public partial class PlayerHistoryForm : Form
    {
        private int id;

        public PlayerHistoryForm(int id)
        {
            InitializeComponent();
            this.id = id;
            Member currentMember = MemberDb.GetMember(id);

            lblFullName.Text = ($"Name : {currentMember.FirstName} {currentMember.LastName}");
            lblMemberNumber.Text = ($"MemberNumber: {currentMember.Number}");
            lblMemberSrartAvg.Text = ($"Start avg : {currentMember.StartAvg}");


           

        }

    }
}
