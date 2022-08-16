using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Driving_School
{
    public partial class Instructor_Update : Form
    {
        public static List<string> details = new List<string>();
        Connection databaseConnection;
        public Instructor_Update()
        {
            InitializeComponent();
            databaseConnection = new Connection();
        }

        private void Instructor_Update_Load(object sender, EventArgs e)
        {
            populate();
        }

        private void populate()
        {
            txtFName.Text = details[1];
            txtLName.Text = details[2];
            txtPhone.Text = details[3];
            txtEmail.Text = details[4];
            txtGender.Text = details[5];
            txtAddress.Text = details[6];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtAddress.Text.Equals("") || txtEmail.Text.Equals("") || txtFName.Text.Equals("") || txtGender.Text.Equals("") || txtLName.Text.Equals("") || txtPhone.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all required information");
                return;
            }
            else if (!(txtGender.Text.Equals("Male") || txtGender.Text.Equals("Female") || txtGender.Text.Equals("Other")))
            {
                MessageBox.Show("Select the correct gender from the list provided");
                return;
            }

            if (databaseConnection.isInstructorEmailEqual(txtEmail.Text, int.Parse(details[0])))
            {
                if (!validateEmail(txtEmail.Text))
                {
                    return;
                }
            }

            if (databaseConnection.isIntructorPhoneEqual(txtPhone.Text, int.Parse(details[0])))
            {
                if (!validatePhone(txtPhone.Text))
                {
                    return;
                }
            }

            //Update Instructor Details
            try
            {
                if (databaseConnection.UpdateInstructor(int.Parse(details[0]), txtFName.Text, txtLName.Text, txtPhone.Text, txtEmail.Text, txtGender.Text, txtAddress.Text))
                {
                    MessageBox.Show("Instructor details updated successfully");
                    this.Hide();
                    return;
                }
                MessageBox.Show("The system failed to update instructor details. Please try again....");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Boolean validatePhone(string phone)
        {
            if (databaseConnection.doesInstructorPhoneExist(phone))
            {
                MessageBox.Show("This Phone number already exists");
                return false;
            }
            if (phone.Length != 10)
            {
                MessageBox.Show("A phone number must be 10 digits long");
                return false;
            }
            else if (!phone[0].Equals('0'))
            {
                MessageBox.Show("A phone number must start with a 0 (Zero)");
                return false;
            }
            else
            {
                int x;
                try
                {
                    x = int.Parse(phone);
                }
                catch
                {
                    MessageBox.Show("A phone number must only contain digits");
                    return false;
                }
            }
            return true;
        }

        private Boolean validateEmail(string email)
        {
            if (databaseConnection.doesInstructorEmailExist(email))
            {
                MessageBox.Show("This email address already exists");
                return false;
            }
            int count = 0;
            foreach (char x in email)
            {
                if (x.Equals('@'))
                    count++;
            }
            if (count != 1)
            {
                MessageBox.Show("Incorrect Email Address");
                return false;
            }
            return true;
        }
    }
}
