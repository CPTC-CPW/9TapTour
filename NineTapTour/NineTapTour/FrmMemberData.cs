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
        List<Member> membersList = MemberDB.getMember();

        public FrmMemberData()
        {
            InitializeComponent(); 
        }

        public void UpdateMemberInfo()
        {
            Member currentMem = new Member();
            foreach(Member m in membersList)
            {
                if(m.MemberNumber == Convert.ToInt32(txtMemberNumber.Text))
                {
                    currentMem = m;
                }
            }

            if (currentMem.MemberNumber != 0)
            {
                txtMemberNumber.Text = currentMem.MemberNumber.ToString();
                txtLastName.Text = currentMem.LastName;
                txtFirstName.Text = currentMem.FirstName;
                txtMiddleInitial.Text = currentMem.MiddleInitial;
                if (currentMem.IsActive)
                {
                    memberActive.SetItemCheckState(0, CheckState.Checked);
                }
                else
                {
                    memberActive.SetItemCheckState(1, CheckState.Checked);
                }

                if (currentMem.IsSenior)
                {
                    isSenior.SetItemCheckState(0, CheckState.Checked);
                }

                if (currentMem.Gender.ToString() == MemberGenders.Female.ToString())
                {
                    memberGender.SetItemCheckState(0, CheckState.Checked);
                }
                else
                {
                    memberGender.SetItemCheckState(1, CheckState.Checked);
                }
                txtNotes.Text = currentMem.Notes;
                txtAdress.Text = currentMem.StreetAddress;
                txtEmail.Text = currentMem.Email;
                txtCity.Text = currentMem.City;
                txtZip.Text = currentMem.PostalCode;
                txtDateJoined.Text = currentMem.JoinDate.ToString();
                txtRefferals.Text = currentMem.Referals.ToString();
                txtPhoneNumber.Text = currentMem.PrimaryPhone;
                txtPhoneNumber2.Text = currentMem.SecondaryPhone;
            }
            else
            {
                this.Controls.Clear();
                this.InitializeComponent();
                UpdateMemberInfo();
            }
        }

        private void MemberDataForm_Load(object sender, EventArgs e)
        {
            UpdateMemberInfo();
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

               if(MemberDB.addMember(temp))
               {
                   MessageBox.Show("Bowler Added Successfully.");
                   membersList = MemberDB.getMember();
               }
                

            }
        }

        private void btnArrowLeft_Click(object sender, EventArgs e)
        {
            if(Convert.ToInt32(txtMemberNumber.Text) != 1)
            {
                txtMemberNumber.Text = (Convert.ToInt32(txtMemberNumber.Text) - 1).ToString();
                UpdateMemberInfo();
            }
            else
            {
                MessageBox.Show("Beginning of file.", "Notice");
            }
            
        }

        private void btnRightArrow_Click(object sender, EventArgs e)
        {
            if(Convert.ToInt32(txtMemberNumber.Text) < membersList.Count())
            {
                txtMemberNumber.Text = (Convert.ToInt32(txtMemberNumber.Text) + 1).ToString();
                UpdateMemberInfo();
            }
            else
            {
                MessageBox.Show("Please Create New Member before advancing Member Number.", "Notice");
            }
            
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.InitializeComponent();
            txtMemberNumber.Text = (membersList.Count + 1).ToString();
        }

        private void btnFirstRecord_Click(object sender, EventArgs e)
        {
            txtMemberNumber.Text = "1";
            UpdateMemberInfo();
        }

        private void btnLastRecord_Click(object sender, EventArgs e)
        {
            txtMemberNumber.Text = membersList.Count().ToString();
            UpdateMemberInfo();
        }

    }
}
