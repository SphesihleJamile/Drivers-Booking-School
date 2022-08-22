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
    public partial class ClientRegistration : Form
    {
        Connection databaseConnection;
        public ClientRegistration()
        {
            InitializeComponent();
            databaseConnection = new Connection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtFName.Text.Equals("") || txtLName.Text.Equals("") || txtPhone.Text.Equals("") || txtEmail.Text.Equals("") || txtAddress.Text.Equals("") || comGender.Text.Equals(""))
            {
                MessageBox.Show("Please Provide All Required Information");
            }
            else if (!validatePhone(txtPhone.Text))
                return;
            else if (!validateEmail(txtEmail.Text))
                return;
            else if(!(comGender.Text.Equals("Male") || comGender.Text.Equals("Female") || comGender.Text.Equals("Other")))
            {
                MessageBox.Show("Select the correct gender from the list provided");
            }
            else
            {
                //now we have to insert the new client into the database
                bool result;
                try
                {
                    string password = txtFName.Text.Substring(0, 1).ToUpper() + txtLName.Text.Substring(0, 1).ToUpper() + txtPhone.Text.Substring(0, 3);
                    result = databaseConnection.InsertClient(txtFName.Text, txtLName.Text, txtPhone.Text, txtEmail.Text, comGender.Text, txtAddress.Text, password);
                }
                catch
                {
                    MessageBox.Show("There was an error inserting the new client");
                    return;
                }
                if (result)
                { MessageBox.Show("Client Registered Successfully"); this.Hide(); }
                else
                    MessageBox.Show("There was an error inserting the new client");
            }
        }

        private Boolean validatePhone(string phone)
        {
            if(databaseConnection.doesClientPhoneExist(phone))
            {
                MessageBox.Show("This Phone number already exists");
                return false;
            }
            if(phone.Length != 10)
            {
                MessageBox.Show("A phone number must be 10 digits long");
                return false;
            }
            else if(!phone[0].Equals('0'))
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
            if(databaseConnection.doesClientEmailExist(email))
            {
                MessageBox.Show("This email address already exists");
                return false;
            }
            int count = 0;
            foreach(char x in email)
            {
                if (x.Equals('@'))
                    count++;
            }
            if(count != 1)
            {
                MessageBox.Show("Incorrect Email Address");
                return false;
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
