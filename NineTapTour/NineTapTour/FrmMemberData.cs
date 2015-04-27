using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour
{
    public partial class FrmMemberData : Form
    {
        public FrmMemberData()
        {
            InitializeComponent();
        }

        private void MemberDataForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Are You Sure?", "Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(confirm == DialogResult.Yes)
            {
                Member temp = new Member();
                temp.MemberNumber = Convert.ToInt32(txtMemberNumber.Text);
                temp.LastName = txtLastName.Text;
                temp.FirstName = txtFirstName.Text;
                temp.MiddleInitial = txtMiddleInitial.Text;
                
                foreach(object itemChecked in memberActive.CheckedItems)
                {
                    if (itemChecked.ToString() == "Active")
                    {
                        temp.IsActive = true;
                    }
                    else
                    {
                        temp.IsActive = false;
                    }
                }
                foreach(object itemChecked in isSenior.CheckedItems)
                {
                    if(itemChecked.ToString() == "Senior")
                    {
                        temp.IsSenior = true;
                    }
                    else
                    {
                        temp.IsSenior = false;
                    }
                }
                foreach(object itemChecked in memberGender.CheckedItems)
                {
                    if(itemChecked.ToString() == "Female")
                    {
                        temp.Gender = MemberGenders.Female;
                    }
                    else
                    {
                        temp.Gender = MemberGenders.Male;
                    }
                }

                temp.Notes = txtNotes.Text;
                temp.StreetAddress = txtAdress.Text;
                temp.Email = txtEmail.Text;
                temp.City = txtCity.Text;
                temp.PostalCode = txtZip.Text;
                temp.JoinDate = DateTime.Now;
                temp.Referals = Convert.ToInt16(txtRefferals.Text);
                temp.PrimaryPhone = txtPhoneNumber.Text;
                temp.SecondaryPhone = txtPhoneNumber2.Text;

                MemberDB.addMember(temp);

            }
        }

    }
}
