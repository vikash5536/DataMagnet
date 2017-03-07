using DataMagnetApp;
using DataMagnetApp.Dalc;
using DataMagnetApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace DataMagnet
{
    /// <summary>
    /// Interaction logic for ManageEmailConfig.xaml
    /// </summary>
    public partial class ManageEmailConfig : Window
    {
        public static ManageEmailConfig _manageEmail;
        public static EMailViewModel _emailViewModel;
        public static Logger logger = new Logger(typeof(ManageEmailConfig));
        public static string connectionString = ConfigurationManager.ConnectionStrings["myCon"].ConnectionString;

        private static bool _isCreate = true;
        public static bool _isSuccess = false;
        public static bool IsAllEmailValid { get; private set; }
        public ManageEmailConfig()
        {
            InitializeComponent();
            if (!_isCreate)
            {
                lbTitle.Content = "Update Email";
                btnAddEmailConfig.Content = lbTitle.Content;
                FillDataFromEmail(_emailViewModel);
            }
            else
            {
                lbTitle.Content = "Create Email";
            }
        }
        private void FillDataFromEmail(EMailViewModel emailviewmodel)
        {
            txtEmailName.Text = emailviewmodel.EmailName;
            txtEmailFrom.Text = emailviewmodel.EmailFrom;
            txtEmailPassword.Password = emailviewmodel.EmailPassword;
            txtEmailTo1.Text = emailviewmodel.EmailTo1;
            txtEmailTo2.Text = emailviewmodel.EmailTo2;
            txtEmailTo3.Text = emailviewmodel.EmailTo3;
            txtEmailTo4.Text = emailviewmodel.EmailTo4;
            txtEmailTo5.Text = emailviewmodel.EmailTo5;
            txtSMTPAddress.Text = emailviewmodel.SMTPAddress;
            txtSMTPPort.Text = emailviewmodel.SMTPPort.ToString();
        }

        public static void ShowCreate()
        {
            _emailViewModel = new EMailViewModel();
            _isCreate = true;
            Show();
        }

        public static void ShowEdit(EMailViewModel emailviewmodel)
        {
            _emailViewModel = new EMailViewModel();
            _isCreate = false;
            _emailViewModel = emailviewmodel;
            Show();
        }
        private new static void Show()
        {
            _manageEmail = new ManageEmailConfig();
            _manageEmail.ShowDialog();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            _isSuccess = false;
            this.Close();
        }
        #region Email Config       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMailTemplate_Click(object sender, RoutedEventArgs e)
        {

            EmailTemplate Promt = new EmailTemplate();
            Promt.Show();
        }
        private void btnEmailFromNext_Click(object sender, RoutedEventArgs e)
        {
            if (EmailFromValidation())
            {
                GridEmailTo.Visibility = Visibility.Visible;
                GridEmailFrom.Visibility = Visibility.Hidden;
            }
        }
        private bool EmailFromValidation()
        {
            if (txtEmailFrom.Text != "" && txtEmailPassword.Password != "" && txtSMTPAddress.Text != "" && txtSMTPPort.Text != "")
            {
                if (!new Common().IsValidEmail(txtEmailFrom.Text))
                {
                    txtEmailFrom.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtEmailFrom.ToolTip = "Email From must be in proper format";
                    return false;
                }
                else
                    return true;
            }
            else
            {
                MessageBox.Show("Fields Can't be Empty");
                return false;
            }
        }
        private void btnEmailFromPre_Click(object sender, RoutedEventArgs e)
        {
            GridEmailTo.Visibility = Visibility.Hidden;
            GridEmailFrom.Visibility = Visibility.Visible;
        }
        private void btnAddEmailConfig_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage errmsg = null;
            if (EmailToValidation())
            {
                EMailViewModel emailviewmodel = new EMailViewModel();
                emailviewmodel.Id = 0;
                emailviewmodel.EmailName = txtEmailName.Text;
                emailviewmodel.EmailFrom = txtEmailFrom.Text;
                emailviewmodel.EmailPassword = txtEmailPassword.Password;
                emailviewmodel.SMTPAddress = txtSMTPAddress.Text;
                emailviewmodel.SMTPPort = Convert.ToInt16(txtSMTPPort.Text);
                emailviewmodel.EnableSSL = cbEnableSsl.IsChecked.Value;
                emailviewmodel.EmailTo1 = txtEmailTo1.Text;
                emailviewmodel.EmailTo2 = txtEmailTo2.Text;
                emailviewmodel.EmailTo3 = txtEmailTo3.Text;
                emailviewmodel.EmailTo4 = txtEmailTo4.Text;
                emailviewmodel.EmailTo5 = txtEmailTo5.Text;
                emailviewmodel.EntryDate = DateTime.Now;
                if (_isCreate)
                     errmsg = Dalc.ManageEmailConfig(emailviewmodel, CommandTypeEnum.Insert);
                else
                    errmsg = Dalc.ManageEmailConfig(emailviewmodel, CommandTypeEnum.Update);
                if (errmsg.ErrorId == 1 || errmsg.ErrorId == 2)
                {
                    _isSuccess = true;
                }
                MessageBox.Show(errmsg.ErrorDescription);
                this.Close();
            }
        }

        private bool EmailToValidation()
        {
            if (txtEmailTo1.Text == "" && txtEmailTo2.Text == "" && txtEmailTo3.Text == "" && txtEmailTo4.Text == "" && txtEmailTo5.Text == "")
                IsAllEmailValid = true;
            if (txtEmailTo1.Text != "")
            {
                if (new Common().IsValidEmail(txtEmailTo1.Text))
                {
                    txtEmailTo1.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtEmailTo1.ToolTip = txtEmailTo1.Text;
                    IsAllEmailValid = true;
                }
                else
                {
                    txtEmailTo1.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtEmailTo1.ToolTip = "Email To 1 must be in proper format";
                    IsAllEmailValid = false;
                }
            }
            if (txtEmailTo2.Text != "")
            {
                if (!new Common().IsValidEmail(txtEmailTo2.Text))
                {
                    txtEmailTo2.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtEmailTo2.ToolTip = "Email To 2 must be in proper format";
                    IsAllEmailValid = false;
                }
                else
                {
                    txtEmailTo2.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtEmailTo2.ToolTip = txtEmailTo2.Text;
                    IsAllEmailValid = true;
                }
            }
            if (txtEmailTo3.Text != "")
            {
                if (!new Common().IsValidEmail(txtEmailTo3.Text))
                {
                    txtEmailTo3.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtEmailTo3.ToolTip = "Email To 3 must be in proper format";
                    IsAllEmailValid = false;
                }
                else
                {
                    txtEmailTo3.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtEmailTo3.ToolTip = txtEmailTo3.Text;
                    IsAllEmailValid = true;
                }
            }
            if (txtEmailTo4.Text != "")
            {
                if (!new Common().IsValidEmail(txtEmailTo4.Text))
                {
                    txtEmailTo4.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtEmailTo4.ToolTip = "Email To 4 must be in proper format";
                    IsAllEmailValid = false;
                }
                else
                {
                    IsAllEmailValid = true;
                    txtEmailTo4.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtEmailTo4.ToolTip = txtEmailTo4.Text;
                }
            }
            if (txtEmailTo5.Text != "")
            {
                if (!new Common().IsValidEmail(txtEmailTo5.Text))
                {
                    txtEmailTo5.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtEmailTo5.ToolTip = "Email To 5 must be in proper format";
                    IsAllEmailValid = false;
                }
                else
                {
                    txtEmailTo5.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtEmailTo3.ToolTip = txtEmailTo5.Text;
                    IsAllEmailValid = true;
                }
            }
            return IsAllEmailValid;
        }

        #endregion
    }
}
