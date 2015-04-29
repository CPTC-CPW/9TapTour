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

            foreach (Member m in membersList)
            {
                if (m.MemberNumber == Convert.ToInt32(txtMemberNumber.Text))
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
                    rdoActive.Checked = true;
                }
                else
                {
                    rdoInActive.Checked = true;
                }

                if (currentMem.IsSenior)
                {
                    rdoSenior.Checked = true;
                }

                if (currentMem.Gender.ToString() == MemberGenders.Female.ToString())
                {
                    rdoFemale.Checked = true;
                }
                else
                {
                    rdoMale.Checked = true;
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
            else if (membersList.Count != 0)
            {
                this.Controls.Clear();
                this.InitializeComponent();
                UpdateMemberInfo();
            }
        }

        //public static string ShowDialog(string text, string caption)
        //{
        //    Form prompt = new Form();
        //    prompt.Width = 500;
        //    prompt.Height = 150;
        //    prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
        //    prompt.Text = caption;
        //    prompt.StartPosition = FormStartPosition.CenterScreen;
        //    Label lblInfo = new Label() { Left = 50, Top = 20, Text = text };
        //    TextBox txtSearch = new TextBox() { Left = 50, Top = 50, Width = 400 };
        //    Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
        //    confirmation.Click += (sender, e) => { prompt.Close(); };
        //    prompt.Controls.Add(txtSearch);
        //    prompt.Controls.Add(confirmation);
        //    prompt.Controls.Add(lblInfo);
        //    prompt.AcceptButton = confirmation;
        //    prompt.ShowDialog();
        //    return txtSearch.Text;
        //}

        private void MemberDataForm_Load(object sender, EventArgs e)
        {
            UpdateMemberInfo();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Are You Sure?", "Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                Member temp = new Member();
                temp.MemberNumber = Convert.ToInt32(txtMemberNumber.Text);
                temp.LastName = txtLastName.Text;
                temp.FirstName = txtFirstName.Text;
                temp.MiddleInitial = txtMiddleInitial.Text;
                if (rdoActive.Checked)
                {
                    temp.IsActive = true;
                }
                else if(rdoInActive.Checked)
                {
                    temp.IsActive = false;
                }


                if (rdoSenior.Checked)
                {
                    temp.IsSenior = true;
                }
                else
                {
                    temp.IsSenior = false;
                }

                if (rdoFemale.Checked)
                {
                    temp.Gender = MemberGenders.Female;
                }
                else if(rdoMale.Checked)
                {
                    temp.Gender = MemberGenders.Male;
                }


                temp.Notes = txtNotes.Text;
                temp.StreetAddress = txtAdress.Text;
                temp.Email = txtEmail.Text;
                temp.City = txtCity.Text;
                temp.PostalCode = txtZip.Text;
                temp.JoinDate = DateTime.Now;
                if (txtRefferals.Text == "")
                {
                    temp.Referals = 0;
                }
                temp.Referals = Convert.ToInt16(txtRefferals.Text);
                temp.PrimaryPhone = txtPhoneNumber.Text;
                temp.SecondaryPhone = txtPhoneNumber2.Text;

                if (MemberDB.addMember(temp))
                {
                    MessageBox.Show("Bowler Added Successfully.");
                    membersList = MemberDB.getMember();
                }


            }
        }

        private void btnArrowLeft_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtMemberNumber.Text) != 1)
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
            if (Convert.ToInt32(txtMemberNumber.Text) < membersList.Count())
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

        private void btnMemberNumber_Click(object sender, EventArgs e)
        {
            //string schNumber = ShowDialog("Seach By Number", "Member Number To Search:");
        }

    }
}
