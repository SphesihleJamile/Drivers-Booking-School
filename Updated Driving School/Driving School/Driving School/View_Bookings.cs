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
    public partial class View_Bookings : Form
    {
        Connection databaseConnection;
        public View_Bookings()
        {
            InitializeComponent();
            databaseConnection = new Connection();
        }

        private void bookingBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bookingBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.groupDataset);

        }

        private void View_Bookings_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'groupDataset.Booking' table. You can move, or remove it, as needed.
            this.bookingTableAdapter.Fill(this.groupDataset.Booking);
            try
            {
                dateTimePicker1.MinDate = Convert.ToDateTime(bookingDataGridView.Rows[0].Cells[5].Value.ToString());
            }
            catch
            {
                dateTimePicker1.MinDate = DateTime.Now;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Start x = new Start();
            x.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!dateTimePicker1.Visible && !comboBox1.Visible)
            {
                if (bookingDataGridView.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Select a row to update");
                    return;
                }
                //dateTimePicker1.MinDate = Convert.ToDateTime(bookingDataGridView.SelectedRows[0].Cells[5].Value.ToString());
                DateTime booked = Convert.ToDateTime((bookingDataGridView.SelectedRows[0].Cells[5].Value.ToString()));
                dateTimePicker1.MinDate = new DateTime(booked.Year, booked.Month, booked.Day + 1);
                dateTimePicker1.Visible = true;
                comboBox1.Visible = true;
                return;
            }
            //update the date in the database
            int st;
            int en;
            try
            {
                st = int.Parse(comboBox1.Text);
                en = st + 2;
                if(st > 14 || st < 8)
                {
                    MessageBox.Show("The start time should be a digit value selected from the dropdown list provided");
                    comboBox1.Text = "";
                    return;
                }
            }
            catch
            {
                MessageBox.Show("The start time should be a digit value selected from the dropdown list provided");
                comboBox1.Text = "";
                return;
            }
            try
            {
                TimeSpan start = new TimeSpan(st, 0, 0);
                TimeSpan end = new TimeSpan(en, 0, 0);
                DateTime newDate = Convert.ToDateTime(dateTimePicker1.Value.ToShortDateString());
                int inst_id = int.Parse(bookingDataGridView.SelectedRows[0].Cells[2].Value.ToString());
                int booking_id = int.Parse(bookingDataGridView.SelectedRows[0].Cells[0].Value.ToString());
                bool val = databaseConnection.isDateDoubleBooked(newDate, start, end, inst_id);
                if (val)
                {
                    MessageBox.Show("Select a new Date/Time as the current time has been booked");
                    comboBox1.Text = "";
                    dateTimePicker1.MinDate = Convert.ToDateTime(bookingDataGridView.SelectedRows[0].Cells[5].Value.ToString());
                    return;
                }
                bool success1 = databaseConnection.updateBookingDate(booking_id, newDate);
                bool success2 = databaseConnection.updateBookingTime(booking_id, start, end);
                if (success1 && success2)
                {
                    MessageBox.Show("The date and time of the booking has been updated successfully");
                    bookingTableAdapter.Fill(groupDataset.Booking);
                    dateTimePicker1.Visible = false;
                    comboBox1.Visible = false;
                    return;
                }
                MessageBox.Show("Failed to update date and time");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bookingDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DateTime booked = Convert.ToDateTime(bookingDataGridView.Rows[e.RowIndex].Cells[5].Value.ToString());
            dateTimePicker1.MinDate = new DateTime(booked.Year, booked.Month, booked.Day+1);
        }

        private void bookingDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DateTime booked = Convert.ToDateTime(bookingDataGridView.Rows[e.RowIndex].Cells[5].Value.ToString());
            dateTimePicker1.MinDate = new DateTime(booked.Year, booked.Month, booked.Day + 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime startDate = Convert.ToDateTime(searchStartDate.Value.ToShortDateString());
            DateTime endDate = Convert.ToDateTime(searchEndDate.Value.ToShortDateString());
            if (checkDate(startDate, endDate))
                bookingDataGridView.DataSource = databaseConnection.searchBookingWithDate(startDate, endDate);
            else
            {
                MessageBox.Show("End search date cannot be before the start search date");
                button5_Click_1(sender, e);
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            bookingDataGridView.DataSource = groupDataset.Booking;
            bookingTableAdapter.Fill(groupDataset.Booking);
        }

        private Boolean checkDate(DateTime start, DateTime end)
        {
            if (start.Year == end.Year)
            {
                if (start.Month == end.Month)
                {
                    if (start.Day == end.Day)
                        return true;
                    else if (start.Day < end.Day)
                        return true;
                    return false;
                }
                else if (start.Month < end.Month)
                    return true;
                else
                    return false;
            }
            else if (start.Year < end.Year)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
