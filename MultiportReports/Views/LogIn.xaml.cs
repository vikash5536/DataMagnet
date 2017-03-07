using SecureApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataMagnetApp
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        public static Logger logger = new Logger(typeof(LogIn));
        public LogIn()
        {
            InitializeComponent();
        }
        #region LogIn Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLogIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbUserType.SelectedIndex == 0)
                {
                    MessageBox.Show("Please Select User Type.");
                    return;
                }
                Common cmn = new Common();
                ComboBoxItem SelectedItem = (ComboBoxItem)cbUserType.SelectedItem;
                bool result = cmn.IsValidUser(SelectedItem.Content.ToString(), txtUserName.Text, txtPassword.Password.ToString());
                if (result)
                {
                    Dashboard ds = new Dashboard();
                    ds.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("User is Unautherised, Please check logIn credentials.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in ButtonLogIn_Click , Message : " + ex.Message);
            }
        }

        private void btnmin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VRodLooP";
            Secure scr = new Secure();
            bool logic = scr.Algorithm("vikash", RegistryPath);
            if (logic == true)
            {
                GridLogIn.IsEnabled = true;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            {
                logger.LogInfo(ex.ToString());
            }
        }
    }
}
