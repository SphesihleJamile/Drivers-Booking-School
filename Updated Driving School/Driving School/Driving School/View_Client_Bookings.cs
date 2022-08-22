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
    public partial class View_Client_Bookings : Form
    {
        public static int client_id = 0;
        public static string full_name = "";
        public static string email;
        Connection databaseConnection;
        public View_Client_Bookings()
        {
            InitializeComponent();
            databaseConnection = new Connection();
            txtCID.Text = client_id.ToString();
            txtName.Text = full_name.ToString();
            txtEmail.Text = email;
            bookingDataGridView.DataSource = databaseConnection.getClientBookings(client_id);

            List<Object> info = databaseConnection.getClientBookingInfo(client_id);
            txtTBookings.Text = info[0].ToString();
            txtABookings.Text = info[1].ToString();
            txtPaid.Text = info[2].ToString();
        }

        private void View_Client_Bookings_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'groupDataset.Booking' table. You can move, or remove it, as needed.
            this.bookingTableAdapter.Fill(this.groupDataset.Booking);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int abook = int.Parse(txtABookings.Text);
            if(abook > 0)
            {
                DialogResult res = MessageBox.Show("Are you sure you want to cancel the current booking? There will be no refunds for doing so", "MK Driving School", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(res == DialogResult.Yes)
                {
                    bool result = databaseConnection.cancelBooking(int.Parse(bookingDataGridView.Rows[0].Cells[0].Value.ToString()), int.Parse(bookingDataGridView.Rows[0].Cells[4].Value.ToString()), client_id);
                    if(result)
                    {
                        MessageBox.Show("The Clients booking have been successfuly canceled");
                        bookingDataGridView.DataSource = databaseConnection.getClientBookings(client_id);
                        List<Object> info = databaseConnection.getClientBookingInfo(client_id);
                        txtTBookings.Text = info[0].ToString();
                        txtABookings.Text = info[1].ToString();
                        txtPaid.Text = info[2].ToString();
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Could not cancel a booking as there are no active bookings in place.");
            }
        }

        private void bookingBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bookingBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.groupDataset);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(txtABookings.Text.Equals("0"))
            {
                MessageBox.Show("This client currently has no bookings to update");
            }
            else
            {
                uStart.Text = bookingDataGridView.Rows[bookingDataGridView.Rows.Count - 1].Cells[6].Value.ToString();
                DateTime bookedDate = Convert.ToDateTime(bookingDataGridView.Rows[bookingDataGridView.Rows.Count - 1].Cells[5].Value.ToString());
                if (isDateBeforeToday(bookedDate) >= 0)
                    uDate.MinDate = bookedDate;
                else
                    uDate.MinDate = DateTime.Now;
                panel2.Visible = true;
            }
        }

        private int isDateBeforeToday(DateTime date)
        {
            if(date.Year == DateTime.Now.Year)
            {
                if(date.Month == DateTime.Now.Month)
                {
                    if (date.Day == DateTime.Now.Day)
                        return 0;
                    if (date.Day > DateTime.Now.Day)
                        return 1;
                    return -1;
                }
                if(date.Month > DateTime.Now.Month)
                {
                    return 1;
                }
                return -1;
            }
            if(date.Year > DateTime.Now.Year)
            {
                return 1;
            }
            return -1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int x = int.Parse(uStart.Text);
                if(x < 8 || x > 14)
                {
                    MessageBox.Show(x.ToString());
                    MessageBox.Show("Select a time between 8 and 14");
                    return;
                }
            }catch(Exception ex) { MessageBox.Show(ex.Message); return; }
            
            int inst_id = int.Parse(bookingDataGridView.Rows[bookingDataGridView.Rows.Count - 1].Cells[2].Value.ToString());
            TimeSpan start = new TimeSpan(int.Parse(uStart.Text), 0, 0);
            TimeSpan end = new TimeSpan(int.Parse(uStart.Text) + 2, 0, 0);
            DateTime date = uDate.Value;

            if(isDateBeforeToday(date) <= 0)
            {
                MessageBox.Show("You cannot book a date before today, and you cannot book todays date");
                return;
            }
            if(databaseConnection.isDateDoubleBooked(date, start, end, inst_id))
            {
                MessageBox.Show("Double Booking Occured, Please select a different date");
                return;
            }

            int book_id = int.Parse(bookingDataGridView.Rows[bookingDataGridView.Rows.Count - 1].Cells[0].Value.ToString());
            if(databaseConnection.updateBookingDate(book_id, date) && databaseConnection.updateBookingTime(book_id, start, end))
            {
                MessageBox.Show("Booking Updated Successfully");
                bookingDataGridView.DataSource = databaseConnection.getClientBookings(client_id);
                panel2.Visible = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }
    }
}
