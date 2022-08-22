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
    public partial class LogIn : Form
    {
        Connection databaseConnection;
        public LogIn()
        {
            InitializeComponent();
            databaseConnection = new Connection();
            email.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(databaseConnection.LogIn(email.Text, password.Text))
            {
                MessageBox.Show("LogIn Successful. Welcome to MK Driving School");
                Start x = new Start();
                x.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Login Details");
                email.Clear();
                password.Clear();
                email.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            databaseConnection.Exit();
        }
    }
}
