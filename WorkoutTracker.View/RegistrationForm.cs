using Controller;
using Data.DTOs;
using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    public partial class RegistrationForm : Form
    {
        AuthController _auth;
        public RegistrationForm(AuthController auth)
        {
            InitializeComponent();
            _auth = auth;
            comboBox1.Items.Add(Gender.Male);
            comboBox1.Items.Add(Gender.Female);
            comboBox1.Items.Add(Gender.NonBinary);
            comboBox1.Items.Add(Gender.PreferNotToSay);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string email = textBox2.Text;
            string password = textBox3.Text;
            string repeat = textBox4.Text;
            Gender gender = (Gender)comboBox1.SelectedItem;
            decimal height = numericUpDown1.Value;
            decimal weight = numericUpDown2.Value;
            CreateUserDTO user = new CreateUserDTO()
            {
                Username = username,
                Email = email,
                Password = password,
                Gender = gender,
                Height = height,
                Weight = weight
            };
            try
            {
                if(password != repeat)
                {
                    throw new Exception("Passwords do not match.");
                }
                await _auth.RegisterAsync(user);
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message);
                return;
            }
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;

            DialogResult = DialogResult.OK;
            this.Close();
        }

        public override bool Equals(object? obj)
        {
            return obj is RegistrationForm form &&
                   EqualityComparer<AuthController>.Default.Equals(_auth, form._auth);
        }
    }
}
