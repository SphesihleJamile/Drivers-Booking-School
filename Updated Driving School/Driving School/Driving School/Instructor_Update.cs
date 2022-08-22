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
            //Checks if all the inputs have data.
            if (txtAddress.Text.Equals("") || txtEmail.Text.Equals("") || txtFName.Text.Equals("") || txtGender.Text.Equals("") || txtLName.Text.Equals("") || txtPhone.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all required information");
                return;
            }
            //Checks if the gender input has reelevent  data
            else if (!(txtGender.Text.Equals("Male") || txtGender.Text.Equals("Female") || txtGender.Text.Equals("Other")))
            {
                MessageBox.Show("Select the correct gender from the list provided");
                return;
            }
            //checks if the current email is equal to the already existing email
            //This is so that we can properly update the information
            if (!databaseConnection.isInstructorEmailEqual(txtEmail.Text, int.Parse(details[0])))
            {
                //if this return true, then it means the email address was not changed and doesn't have to be validated
                //if it returns false, then it means the email was changed, needs to be validated, and it has to be updated.
                if (!validateEmail(txtEmail.Text))
                {
                    return;
                }
            }
            //Checks if the phoe number has been changed by checking if it already exists in the database for the currently specified instructor
            if (!databaseConnection.isIntructorPhoneEqual(txtPhone.Text, int.Parse(details[0])))
            {
                //if this returns true, then the phone number has not been changed, and it does not need to be validated
                //if this returns false, then the phone number needs to be validated to see if it correct.
                if (!validatePhone(txtPhone.Text))
                {
                    return;
                }
            }
            //Update Instructor Details
            try
            {
                //Since all the details have been wel updated, now we need to update all the details in the database.
                //But, before we update, we first need to check if any of the details have been updated. We do not want to make a connection to the database unless it is absolutely neccessary
                if (isAnyUpdated())
                {
                    //if this statement block is executed, then none of the details have been updated, so there is not reason to continue with execution
                    MessageBox.Show("No details have been updated. Click CANCEL or X to exit, or change the data and cllick UPDATE");
                    return;
                }
                MessageBox.Show("1");
                if (databaseConnection.UpdateInstructor(int.Parse(details[0]), txtFName.Text, txtLName.Text, txtPhone.Text, txtEmail.Text, txtGender.Text, txtAddress.Text))
                {
                    MessageBox.Show("2");
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

        private Boolean isAnyUpdated()
        {
            if (txtFName.Text == details[1] && txtLName.Text == details[2] && txtPhone.Text == details[3] && txtEmail.Text == details[4]  && txtGender.Text == details[5] && txtAddress.Text == details[6])
                return true;
            return false;
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
