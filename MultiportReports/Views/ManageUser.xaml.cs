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

namespace DataMagnetApp
{
    /// <summary>
    /// Interaction logic for ManageUser.xaml
    /// </summary>
    public partial class ManageUser : Window
    {
        public static ManageUser _manageUser;
        public static UserViewModel _userVieModel;
        //TextWriter _writerCom = null;
        public static Logger logger = new Logger(typeof(ManageUser));
        public bool AddUservalidationCheckFlag { get; private set; }

        //private static bool _continue = false;
        Common commonClass = new Common();
        public static string connectionString = ConfigurationManager.ConnectionStrings["myCon"].ConnectionString;

        private static bool _isCreate = true;
        public static bool _isSuccess = false;
        public ManageUser()
        {
            InitializeComponent();
            if (!_isCreate)
            {
                lbTitle.Content = "Edit User";
                btnAddUser.Content = lbTitle.Content;
                FillDataFromUser(_userVieModel);
            }
            else
            {
                lbTitle.Content = "Create User";
            }
        }

        private void FillDataFromUser(UserViewModel _userVieModel)
        {
            txtAddUserDesig.Text = _userVieModel.Designation;
            txtAddUserEmail.Text = _userVieModel.Email;
            txtAddUserFullName.Text = _userVieModel.FullName;
            txtAddUserMobile.Text = _userVieModel.Mobile;
            txtAddUserName.Text = _userVieModel.UserName;
            txtAddUserPassword.Password = _userVieModel.Password;
        }
        public static void ShowCreate()
        {
            _userVieModel = new UserViewModel();
            _isCreate = true;
            Show();
        }

        public static void ShowEdit(UserViewModel userviewmodel)
        {
            _userVieModel = new UserViewModel();
            _isCreate = false;
            _userVieModel = userviewmodel;
            Show();
        }
        private new static void Show()
        {
            _manageUser = new ManageUser();
            _manageUser.ShowDialog();
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ErrorMessage result = null;
                if (AddUserValidation())
                {
                    UserViewModel userviewmodel = new UserViewModel();
                    userviewmodel.UserId = 0;
                    userviewmodel.FullName = txtAddUserFullName.Text;
                    userviewmodel.Designation = txtAddUserDesig.Text;
                    userviewmodel.Email = txtAddUserEmail.Text;
                    userviewmodel.Mobile = txtAddUserMobile.Text;
                    userviewmodel.UserName = txtAddUserName.Text;
                    userviewmodel.Password = txtAddUserPassword.Password;
                    if (_isCreate)
                        result = Common.ManageUser(userviewmodel, CommandTypeEnum.Insert);
                    else
                        result = Common.ManageUser(userviewmodel, CommandTypeEnum.Update);
                    if (result.ErrorId == 1 || result.ErrorId == 2)
                    {
                        _isSuccess = true;
                    }
                    MessageBox.Show(result.ErrorDescription);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in btnAddUser_Click, Message : " + ex.Message);
            }
        }

        #region Add User Validation
        /// <summary>
        /// Validation method for add user
        /// </summary>
        /// <returns></returns>
        public bool AddUserValidation()
        {
            try
            {
                if (txtAddUserFullName.Text != "" && txtAddUserName.Text != "" && txtAddUserPassword.Password != "")
                {
                    AddUservalidationCheckFlag = true;
                    AddUserFullNameValidation();
                    AddUserNameValidation();
                    AddUserPasswordValidation();
                }
                else
                {
                    AddUservalidationCheckFlag = false;
                    AddUserFullNameValidation();
                    AddUserNameValidation();
                    AddUserPasswordValidation();
                    AddUserEmptyFieldValidation();
                    MessageBox.Show("Field Can't  be  Empty");
                    return false;
                }

            }
            catch (Exception ex)
            {
                AddUservalidationCheckFlag = false;
                logger.LogDebug("Inside FormValidation Method");
                logger.LogError("File: MainWindow Method: AddUserValidation . Message : " + ex.Message);
            }
            return AddUservalidationCheckFlag;
        }
        /// <summary>
        /// Textbox  txtUserPassword lenght validation
        /// </summary>
        private void AddUserPasswordValidation()
        {
            try
            {
                if ((!Dalc.Dalc.IsLength(txtAddUserPassword.Password, 5, 20)))
                {
                    AddUservalidationCheckFlag = false;
                    txtAddUserPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtAddUserPassword.ToolTip = "User Password must be from 5 to 20 in length";
                }
                else
                {
                    txtAddUserPassword.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtAddUserPassword.ToolTip = "User Password";
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Inside AddUserNameValidation Method");
                logger.LogError("File: MainWindow Method: AddUserPasswordValidation . Message : " + ex.Message);
            }
        }
        /// <summary>
        /// Textbox txtAddUserName lenght validation
        /// </summary>
        private void AddUserNameValidation()
        {
            try
            {
                if ((!Dalc.Dalc.IsLength(txtAddUserName.Text, 5, 15)))
                {
                    AddUservalidationCheckFlag = false;
                    txtAddUserName.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtAddUserName.ToolTip = "User Name must be alphabetic & must be 5 to 15 in length";
                }
                else
                {
                    txtAddUserName.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtAddUserName.ToolTip = "User Name";
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Inside AddUserNameValidation Method");
                logger.LogError("File: MainWindow Method: AddUserNameValidation . Message : " + ex.Message);
            }
        }
        /// <summary>
        /// Textbox txtUserFullName lenght validation
        /// </summary>
        private void AddUserFullNameValidation()
        {
            try
            {
                if ((!Dalc.Dalc.IsLength(txtAddUserFullName.Text, 5, 50)))
                {
                    AddUservalidationCheckFlag = false;
                    txtAddUserFullName.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtAddUserFullName.ToolTip = "User Full Name must be alphabetic & must be 5 to 20 in length";
                }
                else
                {
                    txtAddUserFullName.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtAddUserFullName.ToolTip = "User Full Name";
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Inside UserNameValidation Method");
                logger.LogError("File: MainWindow Method: UserFullNameValidation . Message : " + ex.Message);
            }
        }
        /// <summary>
        /// Add User Emplty field Validations
        /// </summary>
        private void AddUserEmptyFieldValidation()
        {
            try
            {
                if (txtAddUserName.Text == "")
                {
                    txtAddUserName.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtAddUserName.ToolTip = "User Name can't be empty";
                }
                if (txtAddUserPassword.Password == "")
                {
                    txtAddUserPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtAddUserPassword.ToolTip = "Password can't be empty";
                }
                if (txtAddUserFullName.Text == "")
                {
                    txtAddUserFullName.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtAddUserFullName.ToolTip = "User Full Name can't be empty";
                }
            }
            catch (Exception ex)
            {
                logger.LogError("File: MainWindow Method: AddUserEmptyFieldValidation . Message : " + ex.Message);
            }

        }
        #endregion

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            _isSuccess = false;
            this.Close();
        }
    }


}
