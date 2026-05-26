using System;
using System.Drawing;
using System.Windows.Forms;
using Data.DTOs;
using Data.Enums;
using Data.Entities;
using System.Threading.Tasks;

namespace RealView.Views
{
    public class RegistrationView : BaseView
    {
        private Panel _card;
        private Panel _step1Panel;
        private Panel _step2Panel;

        // Step 1
        private TextBox _usernameTxt;
        private TextBox _emailTxt;
        private TextBox _passwordTxt;
        private Button _nextBtn;

        // Step 2
        private ComboBox _genderCb;
        private NumericUpDown _heightNum;
        private NumericUpDown _weightNum;
        private Button _finishBtn;

        private User? _createdUser;

        public RegistrationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 251);

            _card = new Panel
            {
                Size = new Size(500, 500),
                BackColor = Color.White,
                Padding = new Padding(40)
            };

            this.Resize += (s, e) => {
                _card.Location = new Point((this.Width - _card.Width) / 2, (this.Height - _card.Height) / 2);
            };

            // STEP 1 PANEL
            _step1Panel = new Panel { Dock = DockStyle.Fill, Visible = true };
            var step1Layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };
            
            var title1 = new Label { Text = "Account Details", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 20) };
            step1Layout.Controls.Add(title1);

            _usernameTxt = CreateField("Username", step1Layout);
            _emailTxt = CreateField("Email", step1Layout);
            _passwordTxt = CreateField("Password", step1Layout, true);

            _nextBtn = new Button
            {
                Text = "NEXT",
                Size = new Size(420, 50),
                Margin = new Padding(0, 30, 0, 0),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            _nextBtn.FlatAppearance.BorderSize = 0;
            _nextBtn.Click += NextBtn_Click;

            step1Layout.Controls.Add(_nextBtn);
            
            var loginLink = new LinkLabel { Text = "Already have an account? Login", AutoSize = true, Margin = new Padding(0, 20, 0, 0) };
            loginLink.LinkClicked += (s, e) => AppRuntime.Navigation.NavigateTo<LoginView>();
            step1Layout.Controls.Add(loginLink);

            _step1Panel.Controls.Add(step1Layout);

            // STEP 2 PANEL
            _step2Panel = new Panel { Dock = DockStyle.Fill, Visible = false };
            var step2Layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };

            var title2 = new Label { Text = "Personal Info", Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 20) };
            
            var lblGender = new Label { Text = "Gender", AutoSize = true, Margin = new Padding(0, 10, 0, 5) };
            _genderCb = new ComboBox { Width = 420, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) };
            _genderCb.Items.AddRange(new object[] { Gender.Male, Gender.Female, Gender.NonBinary, Gender.PreferNotToSay });
            _genderCb.SelectedIndex = 0;

            var lblHeight = new Label { Text = "Height (cm)", AutoSize = true, Margin = new Padding(0, 15, 0, 5) };
            _heightNum = new NumericUpDown { Width = 420, Maximum = 300, Value = 170, Font = new Font("Segoe UI", 11), DecimalPlaces = 1 };

            var lblWeight = new Label { Text = "Weight (kg)", AutoSize = true, Margin = new Padding(0, 15, 0, 5) };
            _weightNum = new NumericUpDown { Width = 420, Maximum = 999, Value = 70, Font = new Font("Segoe UI", 11), DecimalPlaces = 1 };

            _finishBtn = new Button
            {
                Text = "FINISH",
                Size = new Size(420, 50),
                Margin = new Padding(0, 30, 0, 0),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            _finishBtn.FlatAppearance.BorderSize = 0;
            _finishBtn.Click += FinishBtn_Click;

            step2Layout.Controls.Add(title2);
            step2Layout.Controls.Add(lblGender);
            step2Layout.Controls.Add(_genderCb);
            step2Layout.Controls.Add(lblHeight);
            step2Layout.Controls.Add(_heightNum);
            step2Layout.Controls.Add(lblWeight);
            step2Layout.Controls.Add(_weightNum);
            step2Layout.Controls.Add(_finishBtn);

            _step2Panel.Controls.Add(step2Layout);

            _card.Controls.Add(_step2Panel);
            _card.Controls.Add(_step1Panel);

            this.Controls.Add(_card);

            _card.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, _card.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            };
        }

        private TextBox CreateField(string labelText, FlowLayoutPanel container, bool isPassword = false)
        {
            var lbl = new Label { Text = labelText, AutoSize = true, Margin = new Padding(0, 10, 0, 5) };
            var txt = new TextBox { Width = 420, Font = new Font("Segoe UI", 11), PasswordChar = isPassword ? '*' : '\0' };
            container.Controls.Add(lbl);
            container.Controls.Add(txt);
            return txt;
        }

        private async void NextBtn_Click(object? sender, EventArgs e)
        {
            try
            {
                _nextBtn.Enabled = false;
                
                var dto = new CreateUserDTO
                {
                    Username = _usernameTxt.Text,
                    Email = _emailTxt.Text,
                    Password = _passwordTxt.Text,
                    Gender = Gender.PreferNotToSay, // Temporary defaults
                    Height = 0,
                    Weight = 0
                };

                _createdUser = await AppRuntime.Auth.RegisterAsync(dto);
                
                _step1Panel.Visible = false;
                _step2Panel.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _nextBtn.Enabled = true;
            }
        }

        private async void FinishBtn_Click(object? sender, EventArgs e)
        {
            if (_createdUser == null) return;

            try
            {
                _finishBtn.Enabled = false;
                
                // Update the user with full info
                await AppRuntime.User.UpdateProfileAsync(
                    _createdUser.Id, 
                    (Gender)_genderCb.SelectedItem, 
                    _heightNum.Value, 
                    _weightNum.Value);
                
                MessageBox.Show("Registration complete! Please login.");
                AppRuntime.Navigation.NavigateTo<LoginView>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Profile update failed: {ex.Message}");
                // Even if step 2 fails, the account was created in step 1.
                AppRuntime.Navigation.NavigateTo<LoginView>();
            }
            finally
            {
                _finishBtn.Enabled = true;
            }
        }
    }
}
