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
    public partial class Start : Form
    {
        Connection databaseConnection;
        DateTime bookingDate;

        public Start()
        {
            InitializeComponent();
            databaseConnection = new Connection();
            addInstructorEmails();
            DateTime tomorrow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1);
            bookingCalender.MinDate = tomorrow;
            bookingDate = tomorrow;
            numOfClients.Text = "Clients : " + databaseConnection.getNumberOfClients();
            numOfInstructors.Text = "Instructors : " + databaseConnection.getNumberOfInstructors();
        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {

        }

        //Problem : if the client already has a booking thet is in place, then it will be an issue as they shouldn't be able to make another booking
        //Solution : Check if the client has any bookings
        //           if the client does have a booking, check if lesson_count = number_of_lessons using the package_id
        
        //Confirm Booking
        private void button13_Click(object sender, EventArgs e)
        {
            if (instructorID.Text == "")
            {
                MessageBox.Show("Select an instructor from the provided list");
                return;
            }
            else if (comStart.Text == "")
            {
                MessageBox.Show("Select a start time");
                return;
            }
            else if(txtCID.Text.Length == 0)
            {
                MessageBox.Show("Select a client before confirming a booking");
                return;
            }
            //Check the start time
            try
            {
                int x = int.Parse(comStart.Text);
                if (x < 8 || x > 14)
                {
                    MessageBox.Show("Select a start time from the list provided");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Select a start time from the list provided");
                return;
            }
            //Check the number of lessons
            try
            {
                int x = int.Parse(pack.Text);
                if (!(x == 5 || x == 10 || x == 15 || x == 20))
                {
                    MessageBox.Show("Select number of lessons from the list provided");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Select number of lessons from the list provided");
                return;
            }
            //Check the lisence code
            try
            {
                int x = int.Parse(liscenseCode.Text);
                if (!(x == 8 || x == 10))
                {
                    MessageBox.Show("Select a lisence code from the list provided");
                    MessageBox.Show("Here");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Select a lisence code from the list provided");
                return;
            }
            //Check For Totals
            if(txtTotal.Text == "0" || txtSubtotal.Text == "0")
            {
                MessageBox.Show("Calculate the transaction before confirm");
                return;
            }
            //Check Method of payment
            if(!(!ProofOfPayment.Text.Equals("") || ProofOfPayment.Text.Equals("CASH") || ProofOfPayment.Text.Equals("CARD") || ProofOfPayment.Text.Equals("EFT")))
            {
                MessageBox.Show("Select a method of payment from the provided list");
                return;
            }
            //Check discount
            try
            {
                int discount = int.Parse(comDiscount.Text);
                if(discount < 0)
                {
                    MessageBox.Show("A discount must be a digit greater or equals to 0");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("A discount must be a digit greater or equals to 0");
                return;
            }
            //Check Booking Date
            if (bookingDate == null)
            {
                MessageBox.Show("Select a booking date");
                return;
            }
            //check if the client selected today as a booking date
            if (bookingDate.Year == DateTime.Now.Year && bookingDate.Month == DateTime.Now.Month && bookingDate.Day == DateTime.Now.Day)
            {
                MessageBox.Show("Cannot Book For Today, Choose A Different Day");
                return;
            }
            //Check if the client has any active bookings
            TimeSpan start = new TimeSpan(int.Parse(comStart.Text), 0, 0);
            TimeSpan end = new TimeSpan((int.Parse(comStart.Text) + 2), 0, 0);

            if (databaseConnection.doesClientHaveABooking(int.Parse(txtCID.Text)) && databaseConnection.isClientBookingActive(int.Parse(txtCID.Text)))
            {
                DialogResult r = MessageBox.Show("This client currently has an incomplete booking that is still active, do you wish to book the client for the current selected date as an additional booking?", "Booking System", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {

                    int day = databaseConnection.isDayAfter(int.Parse(txtCID.Text), bookingDate);
                    if (day == -1)
                    {
                        MessageBox.Show("Cannot book a day before your final booked date, choose a different day.");
                        return;
                    }
                    else
                    {
                        //increment the lesson counter for the client booked date

                        if (isDoubleBooked()) //Checks if the client has a double booking with the instructor_id included
                        {
                            //but the client may want to change the instructor id
                            //so to counter that we will check if the client has a double booking excluding the instructor_id (This is if the client selected a similar day as a day of the last booking they did)

                            MessageBox.Show("Select a different date/time, this date and time has been booked");
                            return;
                        }
                        if (databaseConnection.incrementClientLessonCounter(int.Parse(txtCID.Text), start, end, bookingDate))
                        {
                            MessageBox.Show("The client has been booked for the " + bookingDate.ToShortDateString() + " at " + start.Hours);
                            clean();
                            return;
                        }
                        MessageBox.Show("There was a problem incrementing the lesson counter, please try again...");
                        return;
                    }
                }
                clean();
                return;
            }



            //Confirm Booking
            if(isDoubleBooked())
            {
                MessageBox.Show("Double Booking Occured");
                return;
            }
                //Insert Booking
            
            int packID = databaseConnection.getPackageId(int.Parse(liscenseCode.Text), int.Parse(pack.Text));
            bool result = databaseConnection.insertBooking(int.Parse(txtCID.Text), int.Parse(instructorID.Text), int.Parse(liscenseCode.Text), packID, bookingDate, start, end);
            if(result)
            {
                //Process Payment
                int bID = databaseConnection.getBookingId(int.Parse(txtCID.Text));
                bool res = databaseConnection.processPayment(bID, int.Parse(txtCID.Text), DateTime.Now, decimal.Parse(txtTotal.Text), ProofOfPayment.Text);
                if(res)
                {
                    MessageBox.Show("Booking Successful");
                    databaseConnection.refresh();
                    clean();
                    return;
                }
            }
            MessageBox.Show("Booking Unsuccessful, please try again...");
        }

        private Boolean isDoubleBooked()
        {
            //check for double booking
            TimeSpan startTime = new TimeSpan(int.Parse(comStart.Text), 0, 0);
            TimeSpan endTime = new TimeSpan((int.Parse(comStart.Text) + 2), 0, 0);
            return databaseConnection.isDateDoubleBooked(bookingDate, startTime, endTime, int.Parse(instructorID.Text));
        }

        //Calculate
        private void button12_Click(object sender, EventArgs e)
        {
            if (pack.Text == "")
            {
                MessageBox.Show("Select the number of lessons");
                return;
            }
            else if (liscenseCode.Text == "")
            {
                MessageBox.Show("Select the lisence code");
                return;
            }
            else if (!(liscenseCode.Text == "8" || liscenseCode.Text == "10"))
            {
                MessageBox.Show("Select a lisence code from the list provided");
                liscenseCode.Text = "";
                return;
            }
            else if (!(pack.Text == "5" || pack.Text == "10" || pack.Text == "15" || pack.Text == "20"))
            {
                MessageBox.Show("Select number of lessons from the list provided");
                pack.Text = "";
                return;
            }
            try
            {
                int packID = databaseConnection.getPackageId(int.Parse(liscenseCode.Text), int.Parse(pack.Text));
                decimal licensePrice = databaseConnection.getLicensePrice(int.Parse(liscenseCode.Text));
                decimal packagePrice = databaseConnection.getPackagePrice(packID);
                decimal discount = decimal.Parse(comDiscount.Text);
                decimal total = licensePrice + packagePrice - discount;
                txtSubtotal.Text = (total - (total * (decimal)0.15)).ToString();
                txtTotal.Text = total.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                liscenseCode.Text = "";
                pack.Text = "";
                txtSubtotal.Text = "0";
                txtTotal.Text = "0";
            }
        }

        private void clean()
        {
            //Client Information
            txtCAddress.Clear();
            txtCEmail.Clear();
            txtCFName.Clear();
            txtCGender.Clear();
            txtCID.Clear();
            txtCLName.Clear();
            txtCPhone.Clear();

            //Instructor Information
            instructorEmail.Text = "";
            instructorFName.Clear();
            instructorID.Clear();

            //Booking Information
            bookingCalender.SetDate(DateTime.Now);
            bookingDate = DateTime.Now;
            comStart.Text = "";
            pack.Text = "";
            liscenseCode.Text = "";

            //Payments
            comDiscount.Text = "0";
            ProofOfPayment.Text = "";
            txtSubtotal.Text = "0";
            txtTotal.Text = "0";

            //database
            databaseConnection.refresh();
        }

        private void addInstructorEmails()
        {
            List<String> emails = databaseConnection.getInstructorEmailList();
            foreach(string email in emails)
            {
                instructorEmail.Items.Add(email);
            }
        }

        private void instructorEmail_SelectedIndexChanged(object sender, EventArgs e)
        {
            instructorID.Text = databaseConnection.getInstructorId(instructorEmail.Text).ToString();
            instructorFName.Text = databaseConnection.getInstructorFirstName(instructorEmail.Text);
        }

        private void liscenseCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void pack_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void bookingCalender_DateChanged(object sender, DateRangeEventArgs e)
        {
            bookingDate = DateTime.Parse(bookingCalender.SelectionStart.ToShortDateString());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string client_email = txtCEmail.Text;
            if(client_email.Length == 0)
            {
                MessageBox.Show("Enter an email address for a client");
                return;
            }
            List<Object> client = databaseConnection.getClientDetails(client_email);
            if(client == null)
            {
                MessageBox.Show("This client does not exist. Register a new Client");
                //After this, the client registration page can be immediately opened so that the admin can start registering the client
                ClientRegistration x = new ClientRegistration();
                x.Show();
                return;
            }
            txtCID.Text = client[0].ToString();
            txtCFName.Text = client[1].ToString();
            txtCLName.Text = client[2].ToString();
            txtCPhone.Text = client[3].ToString();
            txtCGender.Text = client[4].ToString();
            txtCAddress.Text = client[5].ToString();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //Open the client registration page so that a new client can be registered into the system
            ClientRegistration x = new ClientRegistration();
            x.Show();
        }

        private void Start_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'groupDataset.Payment' table. You can move, or remove it, as needed.
            this.paymentTableAdapter.Fill(this.groupDataset.Payment);
            // TODO: This line of code loads data into the 'groupDataset.Packages' table. You can move, or remove it, as needed.
            this.packagesTableAdapter.Fill(this.groupDataset.Packages);
            // TODO: This line of code loads data into the 'groupDataset.License' table. You can move, or remove it, as needed.
            this.licenseTableAdapter.Fill(this.groupDataset.License);
            // TODO: This line of code loads data into the 'groupDataset.Client' table. You can move, or remove it, as needed.
            this.clientTableAdapter.Fill(this.groupDataset.Client);
            // TODO: This line of code loads data into the 'groupDataset.Instructor' table. You can move, or remove it, as needed.
            this.instructorTableAdapter.Fill(this.groupDataset.Instructor);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            clean();
        }

        private void instructorBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.instructorBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.groupDataset);

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Do you want to update the details of '" + txtIFName.Text + " " + txtILName.Text + "'", "Booking System", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(res == DialogResult.Yes)
            {
                Instructor_Update.details.Clear();
                Instructor_Update.details.Add(txtIID.Text);
                Instructor_Update.details.Add(txtIFName.Text);
                Instructor_Update.details.Add(txtILName.Text);
                Instructor_Update.details.Add(txtIPhone.Text);
                Instructor_Update.details.Add(txtIEmail.Text);
                Instructor_Update.details.Add(txtIGender.Text);
                Instructor_Update.details.Add(txtIAddress.Text);
                Instructor_Update x = new Instructor_Update();
                x.Show();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Do you want to register a new instructor?", "Booking System", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Instructor_Registration x = new Instructor_Registration();
                x.Show();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                instructorTableAdapter.Fill(groupDataset.Instructor);
                txtISearch.Clear();
                numOfInstructors.Text = "Instructors : " + databaseConnection.getNumberOfInstructors();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void txtCSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                clientTableAdapter.FillBySearch(groupDataset.Client, txtCSearch.Text);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtCSearch.Clear();
            }
        }

        private void txtISearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                instructorTableAdapter.FillBySearch(groupDataset.Instructor, txtISearch.Text);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtISearch.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                clientTableAdapter.Fill(groupDataset.Client);
                txtCSearch.Clear();
                numOfClients.Text = "Clients : " + databaseConnection.getNumberOfClients();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Do you want to register a new client?", "Booking System", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                ClientRegistration x = new ClientRegistration();
                x.Show();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Do you want to update the details of '" + txtCCFName.Text + " " + txtCCLName.Text + "'", "Booking System", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                ClientUpdate.details.Clear();
                ClientUpdate.details.Add(txtCCID.Text);
                ClientUpdate.details.Add(txtCCFName.Text);
                ClientUpdate.details.Add(txtCCLName.Text);
                ClientUpdate.details.Add(txtCCPhone.Text);
                ClientUpdate.details.Add(txtCCEmail.Text);
                ClientUpdate.details.Add(txtCCGender.Text);
                ClientUpdate.details.Add(txtCCAddress.Text);
                ClientUpdate x = new ClientUpdate();
                x.Show();
            }
        }

        private void btnLicAdd_Click(object sender, EventArgs e)
        {
            if(pricePanel.Visible)
            {
                if(licenseDataGridView.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Select a license row from the data tableto continue with the update");
                    return;
                }
                int lic_code = int.Parse(licenseDataGridView.SelectedRows[0].Cells[0].Value.ToString());
                DialogResult res = MessageBox.Show("Are you sure you want to update the price for lisence code : " + lic_code, "Booking System", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        decimal price = decimal.Parse(txtLicPrice.Text);
                        if(price <= 0)
                        {
                            MessageBox.Show("The lisence price cannot be less than or equals to 0 (Zero)");
                            return;
                        }
                        if (databaseConnection.updateLicensePrice(lic_code, price))
                        {
                            MessageBox.Show("The price has been updated successfully");
                            licenseTableAdapter.Fill(groupDataset.License);
                            pricePanel.Visible = false;
                            txtLicPrice.Clear();
                            return;
                        }
                        MessageBox.Show("The system was unable to update the price of the license. Please try again...");
                        return;
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }
                pricePanel.Visible = false;
                txtLicPrice.Clear();
                return;
            }
            pricePanel.Visible = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(pricePanel2.Visible)
            {
                try
                {
                    int packID = int.Parse(txtPackage_ID.Text);
                    DialogResult res = MessageBox.Show("Are you sure you want to update the price for package id : " + packID, "Booking System", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        decimal price = decimal.Parse(txtPackPrice.Text);
                        if (databaseConnection.updatePackagePrice(packID, price))
                        {
                            MessageBox.Show("The price has been updated successfully");
                            packagesTableAdapter.Fill(groupDataset.Packages);
                            pricePanel2.Visible = false;
                            txtPackPrice.Clear();
                            return;
                        }
                        MessageBox.Show("The system was unable to update the price of the license. Please try again...");
                        return;
                    }
                    pricePanel2.Visible = false;
                    txtPackPrice.Clear();
                    return;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            pricePanel2.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(paymentDataGridView.SelectedRows.Count <= 0)
            {
                MessageBox.Show("Select a row that you need details of");
            }
            else
            {
                DataGridViewRow row = paymentDataGridView.SelectedRows[0];
                int booking_id = int.Parse(row.Cells[1].Value.ToString());
                int client_id = int.Parse(row.Cells[2].Value.ToString());
                List<Object> details = databaseConnection.getPaymentDetails(client_id, booking_id);
                if(details == null)
                {
                    MessageBox.Show("The system was unable to load the data, please try again");
                    return;
                }
                payClientName.Text = details[0].ToString();
                payClientEmail.Text = details[1].ToString();
                payInstructorID.Text = details[2].ToString();
                payInstructorName.Text = details[3].ToString();
                payInstructorEmail.Text = details[4].ToString();
                payLisenceCode.Text = details[5].ToString();
                payPackageID.Text = details[6].ToString();
                payNumberOfLessons.Text = details[7].ToString();
                payFinalDateBooked.Text = details[8].ToString();
            }
        }

        private void button6_ChangeUICues(object sender, UICuesEventArgs e)
        {
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            payClientName.Clear();
            payClientEmail.Clear();
            payInstructorID.Clear();
            payInstructorName.Clear();
            payInstructorEmail.Clear();
            payLisenceCode.Clear();
            payPackageID.Clear();
            payNumberOfLessons.Clear();
            payFinalDateBooked.Clear();

            paymentTableAdapter.Fill(groupDataset.Payment);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            View_Bookings x = new View_Bookings();
            x.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                View_Client_Bookings.client_id = int.Parse(txtCCID.Text);
                View_Client_Bookings.full_name = txtCCFName.Text + " " + txtCCLName.Text;
                View_Client_Bookings.email = txtCCEmail.Text;
            }catch
            {
                MessageBox.Show("Select a client");
                return;
            }
            View_Client_Bookings x = new View_Client_Bookings();
            x.Show();
        }
    }
}
