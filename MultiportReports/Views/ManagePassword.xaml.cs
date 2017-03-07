using DataMagnetApp;
using DataMagnetApp.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace DataMagnet
{
    /// <summary>
    /// Interaction logic for ManagePassword.xaml
    /// </summary>
    public partial class ManagePassword : Window
    {
        public ManagePassword()
        {
            InitializeComponent();            
            CanOperate();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void CanOperate()
        {
            btnChangePassword.IsEnabled = IsValidData();
        }
        private bool IsValidData()
        {
            if (string.IsNullOrEmpty(txtCurrentPassword.Password))
            {
                txtCurrentPassword.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d80027"));
                txtCurrentPassword.ToolTip = "Current Password Can't be Empty";
                return false;
            }
            else
            {
                txtCurrentPassword.BorderBrush = new SolidColorBrush(Colors.Black);
                txtCurrentPassword.ToolTip = "Current Password";
            }
            if (string.IsNullOrEmpty(txtNewPassword.Password))
            {
                txtNewPassword.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d80027"));
                txtNewPassword.ToolTip = "New Password Can't be Empty";
                return false;
            }
            else
            {
                txtNewPassword.BorderBrush = new SolidColorBrush(Colors.Black);
                txtNewPassword.ToolTip = " New Password";
            }
            if (string.IsNullOrEmpty(txtReNewPassword.Password))
            {
                txtReNewPassword.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d80027"));
                txtReNewPassword.ToolTip = "Re-entered Password can't be Empty";
                return false;
            }
            else
            {
                txtReNewPassword.BorderBrush = new SolidColorBrush(Colors.Black);
                txtReNewPassword.ToolTip = "Reenterd Password";
            }
            return true;
        }
        private void txtCurrentPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CanOperate();
        }

        private void txtReNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CanOperate();
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (txtCurrentPassword.Password == Common.CurrentUser.Password)
            {
                if (txtCurrentPassword.Password == txtNewPassword.Password)
                {
                    txtNewPassword.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d80027"));
                    txtNewPassword.ToolTip = "New Password and Current Password can't be same.";
                    return;
                }
                else if (txtNewPassword.Password == txtReNewPassword.Password)
                {
                    ErrorMessage result = Common.ManageUser(new UserViewModel()
                    {
                        Designation = Common.CurrentUser.Designation,
                        Email = Common.CurrentUser.Email,
                        FullName = Common.CurrentUser.FullName,
                        Mobile = Common.CurrentUser.Mobile,
                        Password = txtNewPassword.Password.ToString(),
                        UserName = Common.CurrentUser.UserName,
                        UserId = Common.CurrentUser.UserId,
                        UserType = Common.CurrentUser.UserType
                    }, CommandTypeEnum.Update);
                    if (result.ErrorId == 2)
                    {
                        MessageBox.Show("Password changed successfully");
                        LogIn login = new LogIn();
                        login.Show();
                        this.Close();
                    }
                }
                else
                {
                    txtReNewPassword.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d80027"));
                    txtReNewPassword.ToolTip = "New Password and Reenterd Password must be same.";
                    return;
                }                
            }
            else
            {
                txtCurrentPassword.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d80027"));
                txtCurrentPassword.ToolTip = "Password doesn't match";
                MessageBox.Show("You are not autherized");
                return;
            }
        }

        private void txtNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CanOperate();
        }
    }
}
