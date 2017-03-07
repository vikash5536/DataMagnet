using DataMagnetApp;
using DataMagnetApp.ViewModel;
using System.Windows;
using System;
using System.Windows.Media;

namespace DataMagnet
{
    /// <summary>
    /// Interaction logic for ChangeUserName.xaml
    /// </summary>
    public partial class ManageUserName : Window
    {
        public ManageUserName()
        {
            InitializeComponent();
            txtCurrentUsername.Text = Common.CurrentUser.UserName;
            CanOperate();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CanOperate()
        {
            btnChangeUser.IsEnabled = IsValidData();
        }

        private bool IsValidData()
        {
            if (string.IsNullOrEmpty(txtCurrentUsername.Text))
            {
                txtCurrentUsername.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d80027"));
                txtCurrentUsername.ToolTip = "Current Username Can't be Empty";
                return false;
            }
            else
            {
                txtCurrentUsername.BorderBrush = new SolidColorBrush(Colors.Black);
                txtCurrentUsername.ToolTip = "Current Username";
            }
            if (string.IsNullOrEmpty(txtCurrentPassword.Password))
            {
                txtCurrentPassword.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d80027"));
                txtCurrentPassword.ToolTip = "Password Can't be Empty";
                return false;
            }
            else
            {
                txtCurrentPassword.BorderBrush = new SolidColorBrush(Colors.Black);
                txtCurrentPassword.ToolTip = "Current Password";
            }
            if (string.IsNullOrEmpty(txtNewUsername.Text))
            {
                txtNewUsername.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d80027"));
                txtNewUsername.ToolTip = "New Username Can't be Empty";
                return false;
            }
            else
            {
                txtNewUsername.BorderBrush = new SolidColorBrush(Colors.Black);
                txtNewUsername.ToolTip = "New Username";
            }
            return true;
        }

        private void btnChangeUser_Click(object sender, RoutedEventArgs e)
        {
            if (txtCurrentPassword.Password == Common.CurrentUser.Password)
            {
                ErrorMessage result = Common.ManageUser(new UserViewModel()
                {
                    Designation = Common.CurrentUser.Designation,
                    Email = Common.CurrentUser.Email,
                    FullName = Common.CurrentUser.FullName,
                    Mobile = Common.CurrentUser.Mobile,
                    Password = Common.CurrentUser.Password,
                    UserName = txtNewUsername.Text,
                    UserId = Common.CurrentUser.UserId,
                    UserType = Common.CurrentUser.UserType
                }, CommandTypeEnum.Update);
                if (result.ErrorId == 2)
                {                   
                    MessageBox.Show("Username changed successfully");
                    LogIn login = new LogIn();
                    login.Show();
                    this.Close();
                }
            }
            else
            {
                txtCurrentPassword.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d80027"));
                txtCurrentPassword.ToolTip = "Password doesn't match";
                MessageBox.Show("You are not autherized");
            }
        }

        private void txtCurrentUsername_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CanOperate();
        }

        private void txtCurrentPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CanOperate();
        }

        private void txtNewUsername_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CanOperate();
        }
    }
}
