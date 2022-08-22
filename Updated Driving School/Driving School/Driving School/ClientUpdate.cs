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
    public partial class ClientUpdate : Form
    {
        public static List<string> details = new List<string>();
        Connection databaseConnection;
        public ClientUpdate()
        {
            InitializeComponent();
            databaseConnection = new Connection();
        }

        private void ClientUpdate_Load(object sender, EventArgs e)
        {
            populate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(txtAddress.Text.Equals("") || txtEmail.Text.Equals("") || txtFName.Text.Equals("") || txtGender.Text.Equals("") || txtLName.Text.Equals("") || txtPhone.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all required information");
                return;
            }
            else if(!(txtGender.Text.Equals("Male") || txtGender.Text.Equals("Female") || txtGender.Text.Equals("Other")))
            {
                MessageBox.Show("Select the correct gender from the list provided");
                return;
            }
            
            if(!databaseConnection.isClientEmailEqual(txtEmail.Text, int.Parse(details[0])))
            {
                if(!validateEmail(txtEmail.Text))
                {
                    MessageBox.Show("Email Address is not valid");
                    return;
                }
            }

            if(!databaseConnection.isClientPhoneEqual(txtPhone.Text, int.Parse(details[0])))
            {
                if(!validatePhone(txtPhone.Text))
                {
                    MessageBox.Show("Phone Number is not valid");
                    return;
                }
            }

            if(isChanged())
            {
                MessageBox.Show("Not details have been updated as no data was changed.");
                return;
            }

            //Update Client Details
            try
            {
                if(databaseConnection.UpdateClient(int.Parse(details[0]), txtFName.Text, txtLName.Text, txtPhone.Text, txtEmail.Text, txtGender.Text, txtAddress.Text))
                {
                    MessageBox.Show("Client details updated successfully");
                    this.Hide();
                    return;
                }
                MessageBox.Show("The system failed to update client details. Please try again....");
                return;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Boolean isChanged()
        {
            if (txtFName.Text == details[1] && txtLName.Text == details[2] && txtPhone.Text == details[3] && txtEmail.Text == details[4] && txtGender.Text == details[5] && txtAddress.Text == details[6])
                return true;
            return false;
        }

        private Boolean validatePhone(string phone)
        {
            if (databaseConnection.doesClientPhoneExist(phone))
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
            if (databaseConnection.doesClientEmailExist(email))
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

        private void populate()
        {
            txtFName.Text = details[1];
            txtLName.Text = details[2];
            txtPhone.Text = details[3];
            txtEmail.Text = details[4];
            txtGender.Text = details[5];
            txtAddress.Text = details[6];
        }
    }
}
