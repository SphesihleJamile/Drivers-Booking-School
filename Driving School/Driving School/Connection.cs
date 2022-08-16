using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Driving_School
{
    class Connection
    {
        //This class will handle all database related queries
        private SqlConnection con;
        private SqlCommand com;
        private SqlDataAdapter da;
        DataTable dt;

        public Connection()
        {
            Open();
        }

        //General Administration Section
        public Boolean LogIn(string username, string password)
        {
            string query = "SELECT COUNT(1) AS LogIn FROM Instructor WHERE email='" + username + "' AND inst_password='" + password + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int res = dt.Rows[0].Field<int>("LogIn");
            return true; //change this to =>  return (res > 0)
        }
        public void Exit()
        {
            con.Close();
            con.Dispose();
            System.Windows.Forms.Application.Exit();
        }

        private void Open()
        {
            string connectionString = "Data Source=146.230.177.46;Initial Catalog=GroupWst4;User ID=GroupWst4;Password=**************";
            con = new SqlConnection(connectionString);
            con.Open();
        }

        public void refresh()
        {
            con.Close();
            con.Dispose();
            Open();
        }
        

        //Customer Section
        public Boolean InsertClient(string fName, string lName, string phone, string email, string gender, string address, string password)
        {
            
            string query = "INSERT INTO Client VALUES(@first_name, @last_name, @phone, @email, @gender, @client_address, @client_password)";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@first_name", fName);
            com.Parameters.AddWithValue("@last_name", lName);
            com.Parameters.AddWithValue("@phone", phone);
            com.Parameters.AddWithValue("@email", email);
            com.Parameters.AddWithValue("@gender", gender);
            com.Parameters.AddWithValue("@client_address", address);
            com.Parameters.AddWithValue("@client_password", password);
            int result = com.ExecuteNonQuery();
            //Check Error
            if (result < 0)
                return false;
            return true;
        }

        public Boolean UpdateClient(int id, string fname, string lname, string phone, string email, string gender, string address)
        {
            string query = "UPDATE Client SET (first_name=@fname, last_name=@lname, phone=@call, email=@mail, gender=@gender, client_address=@add) WHERE client_id=@id";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@fname", fname);
            com.Parameters.AddWithValue("@lname", lname);
            com.Parameters.AddWithValue("@call", phone);
            com.Parameters.AddWithValue("@mail", email);
            com.Parameters.AddWithValue("@gender", gender);
            com.Parameters.AddWithValue("@add", address);
            com.Parameters.AddWithValue("@id", id);
            int res = com.ExecuteNonQuery();
            return !(res <= 0);
        }

        public DataTable getClientBookings(int client_id)
        {
            string query = "SELECT * FROM Booking WHERE client_id='" + client_id + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public List<Object> getClientBookingInfo(int client_id)
        {
            string query = "SELECT * FROM Booking WHERE client_id='" + client_id + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int num_of_bookings = dt.Rows.Count;
            int active_bookings = 0;
            if(num_of_bookings > 0)
            {
                if (isClientBookingActive(client_id))
                    active_bookings = 1;
            }
            //get the total that the customer has paid to the booking school
            decimal total_paid = 0;
            query = "SELECT amount_due FROM Payment WHERE client_id='" + client_id + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            foreach(DataRow row in dt.Rows)
            {
                total_paid += row.Field<decimal>("amount_due");
            }
            List<Object> info = new List<object>();
            info.Add(num_of_bookings);
            info.Add(active_bookings);
            info.Add(total_paid);
            return info;
        }

        public Boolean UpdateInstructor(int id, string fname, string lname, string phone, string email, string gender, string address)
        {
            string query = "UPDATE Instructor SET (first_name=@fname, last_name=@lname, phone=@call, email=@mail, gender=@gender, inst_address=@add) WHERE inst_id=@id";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@fname", fname);
            com.Parameters.AddWithValue("@lname", lname);
            com.Parameters.AddWithValue("@call", phone);
            com.Parameters.AddWithValue("@mail", email);
            com.Parameters.AddWithValue("@gender", gender);
            com.Parameters.AddWithValue("@add", address);
            com.Parameters.AddWithValue("@id", id);
            int res = com.ExecuteNonQuery();
            return !(res <= 0);
        }

        public int getClientId(string email)
        {
            string query = "SELECT client_id FROM Client WHERE email='" + email + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            return dt.Rows[0].Field<int>("client_id");
        }

        public List<Object> getClientDetails(string email)
        {
            string query = "SELECT * FROM Client WHERE email='" + email + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            if(dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                List<Object> client = new List<object>();
                DataRow row = dt.Rows[0];
                client.Add(dt.Rows[0].Field<int>("client_id"));
                client.Add(dt.Rows[0].Field<string>("first_name"));
                client.Add(dt.Rows[0].Field<string>("last_name"));
                client.Add(dt.Rows[0].Field<string>("phone"));
                client.Add(dt.Rows[0].Field<string>("gender"));
                client.Add(dt.Rows[0].Field<string>("client_address"));
                return client;
            }
        }

        public Boolean isClientBooked(int id)
        {
            string query = "SELECT * FROM Booking WHERE client_id='" + id + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count <= 0)
                return false;
            return true;
        }

        public Boolean doesClientEmailExist(string email)
        {
            string query = "SELECT COUNT(1) AS EmailCount FROM Client WHERE email='" + email + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows[0].Field<int>("EmailCount");
            if (count <= 0)
                return false;
            else
                return true;
        }

        public Boolean doesClientPhoneExist(string phone)
        {
            string query = "SELECT COUNT(1) AS PhoneCount FROM Client  WHERE phone='" + phone + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows[0].Field<int>("PhoneCount");
            if (count <= 0)
                return false;
            else
                return true;
        }

        //increment client booking
        public Boolean incrementClientLessonCounter(int id, TimeSpan start, TimeSpan end, DateTime date)
        {
            string query = "SELECT booking_id FROM Booking WHERE client_id='" + id + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int booking_id = dt.Rows[dt.Rows.Count - 1].Field<int>("booking_id");

            query = "SELECT lesson_counter FROM Booking WHERE booking_id='" + booking_id + "'";
            SqlCommand command = new SqlCommand(query, con);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            int lesson_counter = dataTable.Rows[0].Field<int>("lesson_counter");

            bool c1 = updateBookingLessonCounter(id, lesson_counter, booking_id);
            if (!c1)
            {
                System.Windows.Forms.MessageBox.Show("The lesson counter was not incremented, please try again...");
                return false;
            }
            bool c2 = updateBookingDate(booking_id, date);
            if (!c2)
            {
                System.Windows.Forms.MessageBox.Show("The lesson counter was incrementer, but the booking date was not changed, please try again...");
                return false;
            }
            bool c3 = updateBookingTime(booking_id, start, end);
            if (!c3)
            {
                System.Windows.Forms.MessageBox.Show("The lesson counter was incremented, and the booking times were changed, but the booking date was not changed. Please try again later...");
                return false;
            }
            return true;
        }

        public Boolean doesClientHaveABooking(int id)
        {
            string query = "SELECT COUNT(1) AS BookingCounter FROM Booking WHERE client_id='" + id + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows[0].Field<int>("BookingCounter");
            if (count <= 0)
                return false;
            else
                return true;
        }

        public Boolean isClientBookingActive(int id)
        {
            string query = "SELECT package_id, lesson_counter FROM Booking WHERE client_id='" + id + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows.Count;
            //This client has 1 booking
            if(count == 1)
            {
                int package_id = dt.Rows[0].Field<int>("package_id");
                query = "SELECT number_of_lessons AS NumberOfLessons FROM Packages WHERE package_id='" + package_id + "'";
                SqlCommand command = new SqlCommand(query, con);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                int number_of_lessons = dataTable.Rows[0].Field<int>("NumberOfLessons");
                int lesson_counter = dt.Rows[0].Field<int>("lesson_counter");
                if (lesson_counter >= number_of_lessons)
                    return false;
                else
                    return true;
            }
            else if(count > 1)
            {
                foreach(DataRow row in dt.Rows)
                {
                    int package_id = row.Field<int>("package_id");
                    query = "SELECT number_of_lessons AS NumberOfLessons FROM Packages WHERE package_id='" + package_id + "'";
                    SqlCommand command = new SqlCommand(query, con);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    int number_of_lessons = dataTable.Rows[0].Field<int>("NumberOfLessons");
                    int lesson_counter = row.Field<int>("lesson_counter");
                    if (lesson_counter < number_of_lessons)
                        return true;
                }
                return false;
            }
            return false;
        }

        public Boolean isClientEmailEqual(string email, int cid)
        {
            string query = "SELECT COUNT(1) AS Countt FROM Client WHERE email='" + email + "' AND client_id='" + cid + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows[0].Field<int>("Countt");
            return (count == 1);
        }

        public Boolean isClientPhoneEqual(string phone, int cid)
        {
            string query = "SELECT COUNT(1) AS Countt FROM Client WHERE phone='" + phone + "' AND client_id='" + cid + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows[0].Field<int>("Countt");
            return (count == 1);
        }

        //Instructor Section

        public Boolean isInstructorEmailEqual(string email, int iid)
        {
            string query = "SELECT COUNT(1) AS Countt FROM Instructor WHERE email='" + email + "' AND inst_id='" + iid + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows[0].Field<int>("Countt");
            return (count == 1);
        }

        public Boolean isIntructorPhoneEqual(string phone, int iid)
        {
            string query = "SELECT COUNT(1) AS Countt FROM Instructor WHERE phone='" + phone + "' AND inst_id='" + iid + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows[0].Field<int>("Countt");
            return (count == 1);
        }

        public Boolean doesInstructorPhoneExist(string phone)
        {
            string query = "SELECT COUNT(1) AS PhoneCount FROM Instructor WHERE phone='" + phone + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows[0].Field<int>("PhoneCount");
            if (count <= 0)
                return false;
            else
                return true;
        }

        public Boolean doesInstructorEmailExist(string email)
        {
            string query = "SELECT COUNT(1) AS EmailCount FROM Instructor WHERE email='" + email + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int count = dt.Rows[0].Field<int>("EmailCount");
            if (count <= 0)
                return false;
            else
                return true;
        }

        public bool InsertInstructor(string first_name, string last_name, string phone, string email, string gender, string inst_address, string inst_password)
        {
            string query = "INSERT INTO Instructor VALUES(@first_name, @last_name, @phone, @email, @gender, @inst_address, @inst_password)";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@first_name", first_name);
            com.Parameters.AddWithValue("@last_name", last_name);
            com.Parameters.AddWithValue("@phone", phone);
            com.Parameters.AddWithValue("@email", email);
            com.Parameters.AddWithValue("@gender", gender);
            com.Parameters.AddWithValue("@inst_address", inst_address);
            com.Parameters.AddWithValue("@inst_password", inst_password);

            int count = com.ExecuteNonQuery();
            return !(count <= 0);
        }

        public int getInstructorId(string email)
        {
            string query = "Select inst_id FROM Instructor WHERE email='"+ email +"'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int id = dt.Rows[0].Field<int>("inst_id");
            return id;
        }

        public String getInstructorEmail(int id)
        {
            string query = "Select email FROM Instructor WHERE inst_id='"+ id +"'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            string email = dt.Rows[0].Field<string>("email");
            return email;
        }

        public String getInstructorFirstName(string email)
        {
            string query = "Select first_name FROM Instructor WHERE email='" + email + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            string name = dt.Rows[0].Field<string>("first_name");
            return name;
        }

        public List<String> getInstructorEmailList()
        {
            //return a list object populated with instructor emails for the combo box in bookings
            try
            {
                List<String> emails = new List<string>();
                string query = "SELECT email FROM Instructor";
                da = new SqlDataAdapter(query, con);
                dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    return null;
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string email = row.Field<string>("email").ToString();
                        emails.Add(email);
                    }
                    return emails;
                }
            } catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return null;
            }
        }

        public decimal getLicensePrice(int lic)
        {
            string query = "SELECT price FROM License WHERE lic_code='" + lic + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            decimal price = dt.Rows[0].Field<decimal>("price");
            return price;
        }

        public decimal getPackagePrice(int id)
        {
            string query = "SELECT price FROM Packages WHERE package_id='" + id + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            decimal price = dt.Rows[0].Field<decimal>("price");
            return price;
        }

        public int getPackageId(int code, int lessons)
        {
            string query = "SELECT package_id FROM Packages WHERE lic_code='" + code + "' AND number_of_lessons='" + lessons + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int id = dt.Rows[0].Field<int>("package_id");
            return id;
        }

        public Boolean isDateDoubleBooked(DateTime date, TimeSpan start, TimeSpan end, int instructor_id)
        {
            string query = "SELECT booking_date, start_time, end_time, inst_id FROM Booking";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 0)
                return false;
            foreach(DataRow row in dt.Rows)
            {
                DateTime bookedDate = row.Field<DateTime>("booking_date");
                TimeSpan startTime = row.Field<TimeSpan>("start_time");
                TimeSpan endTime = row.Field<TimeSpan>("end_time");
                if(bookedDate.Year == date.Year && bookedDate.Month == date.Month && bookedDate.Day == date.Day)
                {
                    if(checkTimeClash(startTime.Hours, endTime.Hours, start.Hours, end.Hours))
                    {
                        
                        //Check if the instructors are the same
                        //If double booking occured but it is for different instructors, then it is negated.
                        int inst_id = row.Field<int>("inst_id");
                        if(inst_id == instructor_id)
                        {
                            return true;
                        }
                    }
                }
                continue;
            }
            return false;
        }

        public int isDayAfter(int id, DateTime booking_date)//checks if the current booking is before, on, or after the last day booked by the client
        {
            string query = "SELECT booking_date FROM Booking WHERE client_id='" + id + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            DateTime finalBookedDate = dt.Rows[dt.Rows.Count - 1].Field<DateTime>("booking_date");

            //check Year, then month, then day
            if (booking_date.Year == finalBookedDate.Year)
            {
                //Same year, now check month
                if (booking_date.Month == finalBookedDate.Month)
                {
                    //Same month and year, now check day
                    if (booking_date.Day == finalBookedDate.Day)
                        return 0;
                    else if (booking_date.Day < finalBookedDate.Day)
                        return -1;
                    else
                        return 1;
                }
                else if (booking_date.Month < finalBookedDate.Month)
                    return -1;
                else
                    return 1;
            }
            else if (booking_date.Year < finalBookedDate.Year)
                return -1;
            else
            {
                return 1;
            }
        }

        public Boolean isActiveDoubleBooked(DateTime date, TimeSpan start, TimeSpan end)
        {
            string query = "SELECT booking_date, start_time, end_time, inst_id FROM Booking";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 0)
                return false;
            foreach (DataRow row in dt.Rows)
            {
                DateTime bookedDate = row.Field<DateTime>("booking_date");
                TimeSpan startTime = row.Field<TimeSpan>("start_time");
                TimeSpan endTime = row.Field<TimeSpan>("end_time");
                if (bookedDate.Year == date.Year && bookedDate.Month == date.Month && bookedDate.Day == date.Day)
                {
                    if (checkTimeClash(startTime.Hours, endTime.Hours, start.Hours, end.Hours))
                    {
                        return true;
                    }
                }
                continue;
            }
            return false;
        }

        private Boolean checkTimeClash(int bStart, int bEnd, int start, int end)
        {
            if(bStart == start || bEnd == end || bStart == end || bEnd == start)
            {
                return true;
            }
            else if(bStart < start && bEnd > start)
            {
                return true;
            }
            else if(bStart < end && bEnd > end)
            {
                return true;
            }
            return false;
        }

        public Boolean insertBooking(int client_id, int instructor_id, int lic_code, int package_id, DateTime booking_date, TimeSpan start, TimeSpan end)
        {
            int lesson_counter = 1;
            string query = "INSERT INTO Booking Values(@client_id, @inst_id, @lic_code, @package_id, @booking_date, @start_time, @end_time, @lesson_counter)";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@client_id", client_id);
            com.Parameters.AddWithValue("@inst_id", instructor_id);
            com.Parameters.AddWithValue("@lic_code", lic_code);
            com.Parameters.AddWithValue("@package_id", package_id);
            com.Parameters.AddWithValue("@booking_date", booking_date);
            com.Parameters.AddWithValue("@start_time", start);
            com.Parameters.AddWithValue("@end_time", end);
            com.Parameters.AddWithValue("@lesson_counter", lesson_counter);
            int result = com.ExecuteNonQuery();
            if (result <= 0)
                return false;
            return true;
        }

        private Boolean updateBookingLessonCounter(int client_id, int lesson_counter, int booking_id)
        {
            string query = "UPDATE BOOKING SET lesson_counter=@counter WHERE client_id=@cid AND booking_id=@bid";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@counter", lesson_counter + 1);
            com.Parameters.AddWithValue("@cid", client_id);
            com.Parameters.AddWithValue("bid", booking_id);
            int count = com.ExecuteNonQuery();
            return !(count <= 0);
        }
        
        public Boolean updateBookingTime(int booking_id, TimeSpan start_time, TimeSpan end_time)
        {
            string query = "UPDATE BOOKING SET start_time=@start, end_time=@end WHERE booking_id=@bid";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@start", start_time);
            com.Parameters.AddWithValue("@end", end_time);
            com.Parameters.AddWithValue("bid", booking_id);
            int count = com.ExecuteNonQuery();
            return !(count <= 0);
        }

        public Boolean updateBookingDate(int booking_id, DateTime booking_date)
        {
            string query = "UPDATE BOOKING SET booking_date=@date WHERE booking_id=@id";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@date", booking_date);
            com.Parameters.AddWithValue("@id", booking_id);
            int response = com.ExecuteNonQuery();
            return !(response<=0);
        }

        public int getBookingId(int client_id)
        {
            string query = "SELECT booking_id AS BOOKINGID FROM Booking WHERE client_id='" + client_id + "'";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            int booking_id = dt.Rows[dt.Rows.Count - 1].Field<int>("BOOKINGID");
            return booking_id;
        }

        public DataTable searchBookingWithDate(DateTime startDate, DateTime endDate)
        {
            string query = "SELECT * FROM Booking WHERE booking_date BETWEEN @startDate AND @endDate";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@startDate", startDate);
            com.Parameters.AddWithValue("@endDate", endDate);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        //Payment
        public Boolean processPayment(int booking_id, int client_id, DateTime pay_date, decimal amount_due, string paument_methods)
        {
            string query = "SELECT payment_id FROM Payment";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            if(dt.Rows.Count == 0)
            {
                //There aren't any payment yet, so the payment_id = 1
                query = "INSERT INTO Payment VALUES(@payment_id, @booking_id, @client_id, @pay_date, @amount_due, @paument_methods)";
                com = new SqlCommand(query, con);
                com.Parameters.AddWithValue("@payment_id", 1);
                com.Parameters.AddWithValue("@booking_id", booking_id);
                com.Parameters.AddWithValue("@client_id", client_id);
                com.Parameters.AddWithValue("@pay_date", pay_date);
                com.Parameters.AddWithValue("@amount_due", amount_due);
                com.Parameters.AddWithValue("@paument_methods", paument_methods);
                int c = com.ExecuteNonQuery();
                return !(c <= 0);
            }
            int pay_id = dt.Rows[dt.Rows.Count - 1].Field<int>("payment_id");

            query = "INSERT INTO Payment VALUES(@payment_id, @booking_id, @client_id, @pay_date, @amount_due, @paument_methods)";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@payment_id", pay_id + 1);
            com.Parameters.AddWithValue("@booking_id", booking_id);
            com.Parameters.AddWithValue("@client_id", client_id);
            com.Parameters.AddWithValue("@pay_date", pay_date);
            com.Parameters.AddWithValue("@amount_due", amount_due);
            com.Parameters.AddWithValue("@paument_methods", paument_methods);
            int count = com.ExecuteNonQuery();
            return !(count <= 0);
        }

        public int getNumberOfClients()
        {
            string query = "SELECT COUNT(1) AS Number FROM Client";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            return dt.Rows[0].Field<int>("Number");
        }
        public int getNumberOfInstructors()
        {
            string query = "SELECT COUNT(1) AS Number FROM Instructor";
            com = new SqlCommand(query, con);
            da = new SqlDataAdapter(com);
            dt = new DataTable();
            da.Fill(dt);
            return dt.Rows[0].Field<int>("Number");
        }

        public Boolean updateLicensePrice(int lic, decimal price)
        {
            string query = "UPDATE License SET price=@price WHERE lic_code=@lic";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@price", price);
            com.Parameters.AddWithValue("@lic", lic);
            int res = com.ExecuteNonQuery();
            return !(res <= 0);
        }

        public Boolean updatePackagePrice(int pack_id, decimal price)
        {
            string query = "UPDATE Packages SET price=@price WHERE package_id=@id";
            com = new SqlCommand(query, con);
            com.Parameters.AddWithValue("@price", price);
            com.Parameters.AddWithValue("@id", pack_id);
            int res = com.ExecuteNonQuery();
            return !(res <= 0);
        }

        public List<Object> getPaymentDetails(int client_id, int booking_id)
        {
            try
            {
                List<Object> payment_details = new List<object>();

                string query = "SELECT first_name, last_name, email FROM Client WHERE client_id='" + client_id + "'";
                com = new SqlCommand(query, con);
                da = new SqlDataAdapter(com);
                dt = new DataTable();
                da.Fill(dt);
                string client_name = dt.Rows[0].Field<string>("first_name") + " " + dt.Rows[0].Field<string>("last_name");
                string client_email = dt.Rows[0].Field<string>("email");


                query = "SELECT * FROM Booking WHERE booking_id='" + booking_id + "'";
                com = new SqlCommand(query, con);
                da = new SqlDataAdapter(com);
                dt = new DataTable();
                da.Fill(dt);
                int instructor_id = dt.Rows[0].Field<int>("inst_id");
                int lisence_code = dt.Rows[0].Field<int>("lic_code");
                int package_id = dt.Rows[0].Field<int>("package_id");
                DateTime final_booked_date = dt.Rows[0].Field<DateTime>("booking_date");

                query = "SELECT first_name, last_name, email FROM Instructor WHERE inst_id='" + instructor_id + "'";
                com = new SqlCommand(query, con);
                da = new SqlDataAdapter(com);
                dt = new DataTable();
                da.Fill(dt);
                string instructor_name = dt.Rows[0].Field<string>("first_name") + " " + dt.Rows[0].Field<string>("last_name");
                string instructor_email = dt.Rows[0].Field<string>("email");

                query = "SELECT number_of_lessons FROM Packages WHERE package_id='" + package_id + "'";
                com = new SqlCommand(query, con);
                da = new SqlDataAdapter(com);
                dt = new DataTable();
                da.Fill(dt);
                int number_of_lessons = dt.Rows[0].Field<int>("number_of_lessons");

                payment_details.Add(client_name);
                payment_details.Add(client_email);
                payment_details.Add(instructor_id);
                payment_details.Add(instructor_name);
                payment_details.Add(instructor_email);
                payment_details.Add(lisence_code);
                payment_details.Add(package_id);
                payment_details.Add(number_of_lessons);
                payment_details.Add(final_booked_date);

                return payment_details;
            }catch
            {
                return null;
            }
        }
    }
}
