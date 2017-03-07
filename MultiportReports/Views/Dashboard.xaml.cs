using System;
using System.Configuration;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using DataMagnetApp.ViewModel;
using DataMagnet;

namespace DataMagnetApp
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        //TextWriter _writerCom = null;
        public static Logger logger = new Logger(typeof(Dashboard));
        private SerialPort _serialPort;
        //private static bool _continue = false;
        Common commonClass = new Common();
        public static string connectionString = ConfigurationManager.ConnectionStrings["myCon"].ConnectionString;
        BoardData bd = new BoardData();
        CollectionView view;
        PropertyGroupDescription groupDescription;
        TcpClient client1Socket = new TcpClient();
        private string appCount = "0";

        public bool AddUservalidationCheckFlag { get; private set; }
        public string AppMaxRun = "3";

        public Dashboard()
        {
            InitializeComponent();
            # region Server Config controls style
            //txtServerName.Text = "Enter Server Name";
            txtServerIP.Text = "Enter Server IP";
            txtServerPort.Text = "Enter Server Port";
            //txtComServerName.Text = "Enter Server Name";

            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#7E8387");
            //txtServerName.Foreground = brush;
            txtServerIP.Foreground = brush;
            txtServerPort.Foreground = brush;
            #endregion
        }

        #region Dashboard Methods
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Common.EmailHistoryList = Common.GetEmailHistory();
            lvDataBinding.ItemsSource = Common.EmailHistoryList;
            lvDataBinding.Items.Refresh();
            view = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource);
            groupDescription = new PropertyGroupDescription("EntryDateGroup");
            view.GroupDescriptions.Add(groupDescription);
            logger.LogInfo("Dashboard Window loaded.");

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEmailconfig_Click(object sender, RoutedEventArgs e)
        {
            if (Common.CurrentUser.UserType == UserTypeEnum.Admin)
            {
                GridEmailConfig.Visibility = Visibility.Visible;
                GridDashbord.Visibility = Visibility.Hidden;
                GridConfig.Visibility = Visibility.Visible;
                GridServersConfig.Visibility = Visibility.Hidden;
                GridUserConfig.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show("You are not authorised for this feature.");
                return;
            }
            LoadEmailConfiguration();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            GridDashbord.Visibility = Visibility.Visible;
            GridConfig.Visibility = Visibility.Hidden;
            GridServersConfig.Visibility = Visibility.Hidden;
            GridEmailConfig.Visibility = Visibility.Hidden;
            GridEmailHistory.Visibility = Visibility.Hidden;
            GridReports.Visibility = Visibility.Hidden;
            GridUserList.Visibility = Visibility.Hidden;
        }

        private void btnServer_Click(object sender, RoutedEventArgs e)
        {
            ServerConfig sd = new ServerConfig();
            sd.Show();
            this.Close();
            //// Create the thread object. This does not start the thread.
            //Worker workerObject = new Worker();
            //Thread workerThread = new Thread(workerObject.DoWork);

            //// Start the worker thread.
            //workerThread.Start("192.168.1.15 8888");
            //logger.LogInfo("main thread: Starting worker thread...");

            //// Loop until worker thread activates. 
            //while (!workerThread.IsAlive) ;

            //// Put the main thread to sleep for 1 millisecond to 
            //// allow the worker thread to do some work:
            //Thread.Sleep(1);

            //// Request that the worker thread stop itself:
            ////workerObject.RequestStop();

            //// Use the Join method to block the current thread  
            //// until the object's thread terminates.
            ////workerThread.Join();
            //logger.LogInfo("main thread: Worker thread has terminated.");

        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            GridDashbord.Visibility = Visibility.Hidden;
            GridConfig.Visibility = Visibility.Visible;
            GridConfigTelnet.Visibility = Visibility.Visible;
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

        private void btnConfig_MouseEnter(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/settings-r.png";
            imgConfig.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnConfig_MouseLeave(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/settings-w.png";
            imgConfig.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnServer_MouseEnter(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/weighing-r.png";
            imgServer.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnServer_MouseLeave(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/weighing-w.png";
            imgServer.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnNotes_MouseEnter(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/report-r.png";
            imgReport.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnNotes_MouseLeave(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/report-w.png";
            imgReport.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnUserconfig_MouseEnter(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/UserConfig.png";
            imgUserConfig.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnUserconfig_MouseLeave(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/UserConfig.png";
            imgUserConfig.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnUserList_MouseEnter(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/avatar-r.png";
            imgUserProfile.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnUserList_MouseLeave(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/avatar-w.png";
            imgUserProfile.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnServerconfig_MouseEnter(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/ServerConfig.png";
            imgServerConfig.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnServerconfig_MouseLeave(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/ServerConfig.png";
            imgServerConfig.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            LogIn login = new LogIn();
            login.Show();
            this.Close();
        }

        private void btnLogOut_MouseEnter(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/logout-r.png";
            imgLogOut.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void btnLogOut_MouseLeave(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/logout-w.png";
            imgLogOut.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        private void ShowHideMenu(string Storyboard, Button btnHide, Button btnShow, StackPanel pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(pnl);

            if (Storyboard.Contains("Show"))
            {
                btnHide.Visibility = System.Windows.Visibility.Visible;
                btnShow.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (Storyboard.Contains("Hide"))
            {
                btnHide.Visibility = System.Windows.Visibility.Hidden;
                btnShow.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void StartAllServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //ServerConfig sd = new ServerConfig();
                //sd.Show();
                //this.Close();
                // Create the thread object. This does not start the thread.
                Worker workerObject = new Worker();
                Thread workerThread = new Thread(workerObject.DoWorkAll);
                //Thread workerThread = new Thread(Worker.DoWorkAll);
                // Start the worker thread.
                workerThread.Start();
                logger.LogInfo("main thread: Starting worker thread...");

                // Loop until worker thread activates. 
                while (!workerThread.IsAlive) ;

                // Put the main thread to sleep for 1 millisecond to 
                // allow the worker thread to do some work:
                Thread.Sleep(1);

                // Request that the worker thread stop itself:
                //workerObject.RequestStop();

                // Use the Join method to block the current thread  
                // until the object's thread terminates.
                //workerThread.Join();
                logger.LogInfo("main thread: Worker thread has terminated.");
            }
            catch (Exception ex)
            {
                logger.LogError("" + ex.Message);
            }
        }

        private void StopAllServer_Click(object sender, RoutedEventArgs e)
        {
            Worker workerObject = new Worker();
            workerObject.RequestStopAll();
        }

        private void HideAutheriseFeatures()
        {
            if (Common.CurrentUser.UserType == UserTypeEnum.Admin)
            {
                btnUserList.IsEnabled = false;
                btnUserList.Style = this.FindResource("CircleGrayButtonTemplate") as Style;
            }
            else
            {
                btnUserList.IsEnabled = true;
                btnUserList.Style = this.FindResource("CircleRedButtonTemplate") as Style;
            }

        }
        #endregion        
        #region Reports
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GridDashbord.Visibility = Visibility.Hidden;
                GridReports.Visibility = Visibility.Visible;
                GetMachineData();
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in btnReports_Click , Message : " + ex.Message);
            }

        }
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            // dataGridCustomers.DataContext = bd.LoadCollectionData();
        }
        public void GetMachineData()
        {
            Common.MachineDataList = Common.GetAllMachineData();
            dgReport.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                dgReport.ItemsSource = Common.MachineDataList;
                dgReport.Items.Refresh();
            }));
        }
        #endregion
        #region User Config Methods
        private void btnUserList_Click(object sender, RoutedEventArgs e)
        {
            dgUserList.ItemsSource = new Common().LoadUserDetail();
            dgUserList.Items.Refresh();
            GridUserList.Visibility = Visibility.Visible;
            GridDashbord.Visibility = Visibility.Hidden;
        }
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ManageUser.ShowCreate();
                if (ManageUser._isSuccess)
                {
                    dgUserList.ItemsSource = new Common().LoadUserDetail();
                    dgUserList.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in btnAddUser_Click, Message : " + ex.Message);
            }
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = dgUserList.SelectedItem as UserViewModel;
                if (MessageBox.Show("Do you really want to Delete?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (selectedRow.UserName != "admin")
                    {
                        var message = Common.ManageUser(selectedRow, CommandTypeEnum.Delete);
                        if (message.ErrorId == 0)
                        {
                            MessageBox.Show("Unable to Delete user, Message : " + message.ErrorDescription);
                        }
                        else if (message.ErrorId == 3)
                        {
                            MessageBox.Show(message.ErrorDescription);
                            dgUserList.ItemsSource = new Common().LoadUserDetail();
                            dgUserList.Items.Refresh();
                        }
                    }
                    else { MessageBox.Show("Admin can not be delete"); }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void dgUserList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selectedRow = dgUserList.SelectedItem as UserViewModel;
                ManageUser.ShowEdit(selectedRow);
                if (ManageUser._isSuccess)
                {
                    dgUserList.ItemsSource = new Common().LoadUserDetail();
                    dgUserList.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in btnEditUser_Click , Message : " + ex.Message);
            }
        }

        private void dgUserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUserList.SelectedItems.Count > 0)
            {
                btnEditUser.IsEnabled = true;
                btnDeleteUser.IsEnabled = true;
                ImageEditUser.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/EditOn.png") as ImageSource;
                ImageDeleteUser.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/DeleteOn.png") as ImageSource;
            }
            else
            {
                btnEditUser.IsEnabled = false;
                btnDeleteUser.IsEnabled = false;
                ImageEditUser.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/EditOff.png") as ImageSource;
                ImageDeleteUser.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/DeleteOff.png") as ImageSource;
            }
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRow = dgUserList.SelectedItem as UserViewModel;
                ManageUser.ShowEdit(selectedRow);
                if (ManageUser._isSuccess)
                {
                    dgUserList.ItemsSource = new Common().LoadUserDetail();
                    dgUserList.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in btnEditUser_Click , Message : " + ex.Message);
            }
        }


        #endregion
        #region Email Notifications
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMail_Click(object sender, RoutedEventArgs e)
        {
            GridDashbord.Visibility = Visibility.Hidden;
            GridEmailHistory.Visibility = Visibility.Visible;
            GetEmailHistory();
        }
        /// <summary>
        /// 
        /// </summary>
        public void GetEmailHistory()
        {
            Common.EmailHistoryList.Clear();
            view.GroupDescriptions.Remove(groupDescription);
            Common.EmailHistoryList = Common.GetEmailHistory();
            lvDataBinding.ItemsSource = Common.EmailHistoryList;
            lvDataBinding.Items.Refresh();
            view = (CollectionView)CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource);
            groupDescription = new PropertyGroupDescription("EntryDateGroup");
            view.GroupDescriptions.Add(groupDescription);
        }
        private bool UserFilter(object item)
        {

            if (String.IsNullOrEmpty(txtEmailFilter.Text))
                return true;
            else
                return ((item as EMailHistoryViewModel).EmailSubject.IndexOf(txtEmailFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void txtEmailFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvDataBinding.ItemsSource).Refresh();
            view.Filter = UserFilter;
        }

        private void btnMail_MouseEnter(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/email-r.png";
            imgMailNotify.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }
        private void btnMail_MouseLeave(object sender, MouseEventArgs e)
        {
            string packUri = @"pack://application:,,,/images/email-w.png";
            imgMailNotify.Source = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

        #endregion
        #region Trial Version

        private bool CreateTrailfunctionality()
        {
            try
            {
                DateTime date = new DateTime();
                Microsoft.Win32.RegistryKey EncryptedKey;
                Microsoft.Win32.RegistryKey EncryptedAppCount;
                RSACryptoServiceProvider crypto = new RSACryptoServiceProvider();
                EncryptedKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\EncryptedDate");
                EncryptedAppCount = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\EncryptedAppCount");

                object value = EncryptedKey.GetValue("EncryptedDate");
                object EncryptedAppCountvalue = EncryptedAppCount.GetValue("EncryptedAppCount");

                if (value == null)
                {

                    EncryptedKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\EncryptedDate");
                    string keyvalue = DateTime.Now.ToString("dd-MMM-yyyy");
                    byte[] messageBytes = Encoding.Unicode.GetBytes(keyvalue);
                    string encryptedMessage = Convert.ToBase64String(messageBytes);
                    EncryptedKey.SetValue("EncryptedDate", encryptedMessage);

                    EncryptedAppCount = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\EncryptedAppCount");
                    byte[] messageCountBytes = Encoding.Unicode.GetBytes(appCount);
                    string encryptedCountMessage = Convert.ToBase64String(messageCountBytes);
                    EncryptedAppCount.SetValue("EncryptedAppCount", encryptedCountMessage);
                    return true;
                }
                else
                {
                    byte[] encryptedMessage1 = Convert.FromBase64String(value.ToString());
                    string key1 = System.Text.Encoding.Unicode.GetString(encryptedMessage1);
                    date = Convert.ToDateTime(key1);

                    byte[] encryptedMessage2 = Convert.FromBase64String(EncryptedAppCountvalue.ToString());
                    string key2 = System.Text.Encoding.Unicode.GetString(encryptedMessage2);
                    appCount = key2;

                    //Increase app run by 1
                    int temp = Convert.ToInt32(appCount);
                    temp++;
                    byte[] messageCountBytes = Encoding.Unicode.GetBytes(temp.ToString());
                    string encryptedCountMessage = Convert.ToBase64String(messageCountBytes);
                    EncryptedAppCount.SetValue("EncryptedAppCount", encryptedCountMessage);

                    //MessageBox.Show(date.ToString("dd-MMM-yyyy"));
                }
                if (DateTime.Now <= date.AddDays(5) && Convert.ToInt32(appCount) < Convert.ToInt32(AppMaxRun))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in CreateTrailfunctionality, Message : " + ex.Message);
            }
            return false;
        }
        #endregion
        #region Configuration      
        #region Server Config Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPorts_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get a list of serial port names.
                string[] ports = SerialPort.GetPortNames();
                // ... Get the ComboBox reference.
                var cmbPorts = sender as ComboBox;
                if (ports.Length == 0)
                {
                    _lblCOMPortLog.Text = "No Com Port detected.";
                }
                else
                {
                    ComboBoxItem[] comboitem = new ComboBoxItem[ports.Length];
                    for (int i = 0; i < ports.Length; i++)
                    {
                        cmbPorts.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                        {
                            comboitem[i] = new ComboBoxItem();
                            comboitem[i].Content = ports[i];
                            comboitem[i].Name = "_" + ports[i];
                            cmbPorts.Items.Add(comboitem[i]);
                        }));
                    }
                }

            }
            catch (Exception ex)
            {
                if (_lblCOMPortLog.Text == "")
                    _lblCOMPortLog.Text = ex.Message;
                else
                    _lblCOMPortLog.Text = _lblCOMPortLog.Text + Environment.NewLine + ex.Message;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestTelnetConn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (btnTestTelnetConn.Content.ToString().Trim() == "Test Connection")
                {
                    if (!client1Socket.Connected)
                    {
                        client1Socket.Connect(txtServerIP.Text, Convert.ToInt32(txtServerPort.Text));
                        _lbltelnetPortLog.Text = "Client Socket Program - Server Connected ...";
                        logger.LogInfo("Client Socket Program - Server Connected ...");
                        btnTestTelnetConn.Content = "Read Data";
                    }
                }
                else
                {
                    if (!client1Socket.Connected)
                    {
                        btnTestTelnetConn.Content = "Test Connection";
                        _lbltelnetPortLog.Text = _lbltelnetPortLog.Text + Environment.NewLine + "Server is not connected, Please try again ...";
                        logger.LogInfo("Server is not connected, Please try again ...");
                    }
                    else
                    {
                        NetworkStream serverStream = client1Socket.GetStream();
                        byte[] inStream = new byte[10025];
                        serverStream.Read(inStream, 0, (int)client1Socket.ReceiveBufferSize);
                        string replacement = Regex.Replace(System.Text.Encoding.ASCII.GetString(inStream), @"\t|\0", "");
                        logger.LogInfo("Received message from server : " + replacement);
                        _lbltelnetPortLog.Text = _lbltelnetPortLog.Text + Environment.NewLine + "Received message from server : " + replacement;
                        if (replacement == "1")
                        {
                            Common.AddDataInDatabase(txtServerIP.Text, replacement);
                            int result = 0;//Common.SendEmail(txtServerIP.Text);
                            if (result != -1)
                                _lbltelnetPortLog.Text = _lbltelnetPortLog.Text + Environment.NewLine + "Email sent.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _lbltelnetPortLog.Text = _lbltelnetPortLog.Text + Environment.NewLine + ex.Message;
                logger.LogError("Exception in btnTestTelnetConn click method, Message : " + ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerConType_Checked(object sender, RoutedEventArgs e)
        {
            if (ServerConType.UncheckedContent.ToString().Trim() == "COM Port")
            {
                GridConfigCOM.Visibility = Visibility.Visible;
                GridConfigTelnet.Visibility = Visibility.Hidden;
                btnOpen.Content = "Open Port";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerConType_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ServerConType.CheckedContent.ToString().Trim() == "Telnet")
            {
                GridConfigCOM.Visibility = Visibility.Hidden;
                GridConfigTelnet.Visibility = Visibility.Visible;
                btnTestTelnetConn.Content = "Test Connection";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnOpen.Content.ToString().Trim() == "Open Port")
                {
                    string[] CurrrentPort = cmbPorts.SelectedItem.ToString().Split(' ');
                    StringBuilder PortOpenlog = new StringBuilder();
                    if (_serialPort == null && CurrrentPort[1] != "")
                    {
                        _serialPort = new SerialPort(CurrrentPort[1]);
                        _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                        _serialPort.DtrEnable = true;
                        _serialPort.Handshake = Handshake.None;
                        _lblCOMPortLog.Text = "Serial Port initialization for port :" + CurrrentPort[1] + ".";
                        logger.LogInfo("Serial Port initialization for port :" + CurrrentPort[1] + ".");
                    }
                    if (!_serialPort.IsOpen)
                    {
                        _lblCOMPortLog.Text = _lblCOMPortLog.Text + Environment.NewLine + "Trying to open port.";
                        logger.LogInfo("Trying to open port.");
                        ComboBoxItem BoxItem = (ComboBoxItem)_cmbPorts_BoadRate.SelectedItem;
                        _serialPort.BaudRate = Convert.ToInt32(BoxItem.Content);
                        _serialPort.Open();
                    }
                    _lblCOMPortLog.Text = _lblCOMPortLog.Text + Environment.NewLine + "Port " + CurrrentPort[1] + "is open now.";
                    logger.LogInfo("Port " + CurrrentPort[1] + "is open now.");
                    btnOpen.Content = "Read Data";
                }
                else
                {
                    if (_serialPort == null && !_serialPort.IsOpen)
                    {
                        btnOpen.Content = "Open Port";
                        _lblCOMPortLog.Text = _lblCOMPortLog.Text + Environment.NewLine + "Please open the port first";
                    }
                }

            }
            catch (Exception ex)
            {
                btnOpen.Content = "Open Port";
                logger.LogError(ex.Message);
                _lblCOMPortLog.Text = _lblCOMPortLog.Text + Environment.NewLine + "Exception Message : " + ex.Message;
            }
        }

        /// <summary>
        /// Thread to write data on serial port.
        /// </summary>
        private void WriteData()
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    string str = _serialPort.ReadLine();
                    if (i == 10)
                        _serialPort.WriteLine("FAIL");
                    else
                        _serialPort.WriteLine("SUCCESS " + i);
                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// Event handler for read data from port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadLine();
            string replacement = Regex.Replace(indata, @"\t|\0", "");
            logger.LogInfo("Data received from server, Data : " + replacement);
            //if (replacement == "1.005")
            //{
            logger.LogInfo("Unable to fetch data from server. Sending alert...");
            Common.AddDataInDatabase(sp.PortName, replacement);
            //  Common.SendEmail(sp.PortName);
            GetMachineData();
            // }
        }
        # region Server Config controls style
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtServerIP_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtServerIP.Text == "Enter Server IP")
            {
                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString("#000000");
                txtServerIP.Foreground = brush;
                txtServerIP.Text = "";

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtServerIP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtServerIP.Text == "")
            {
                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString("#7E8387");
                txtServerIP.Foreground = brush;
                txtServerIP.Text = "Enter Server IP";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtServerPort_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtServerPort.Text == "Enter Server Port")
            {
                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString("#000000");
                txtServerPort.Foreground = brush;
                txtServerPort.Text = "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtServerPort_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtServerPort.Text == "")
            {
                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString("#7E8387");
                txtServerPort.Foreground = brush;
                txtServerPort.Text = "Enter Server Port";
            }
        }

        #endregion

        #endregion
        #region Email Config

        private void AddEMailConfig_Click(object sender, RoutedEventArgs e)
        {
            ManageEmailConfig.ShowCreate();
            if (ManageEmailConfig._isSuccess)
            {
                LoadEmailConfiguration();
            }
        }
        private void EditContext_Click(object sender, RoutedEventArgs e)
        {
            var selectedrow = dgEmailConfiguration.SelectedItem as EMailViewModel;
            if (selectedrow != null)
            {
                ManageEmailConfig.ShowEdit(selectedrow);
                if (ManageEmailConfig._isSuccess)
                {
                    LoadEmailConfiguration();
                }
            }
        }

        private void DeleteContext_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = dgEmailConfiguration.SelectedItem as EMailViewModel;
            if (selectedRow != null)
            {
                if (MessageBox.Show("Do you really want to Delete?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    var message = Common.ManageEmailConfig(selectedRow, CommandTypeEnum.Delete);
                    if (message.ErrorId == 0)
                    {
                        MessageBox.Show("Unable to Delete user, Message : " + message.ErrorDescription);
                    }
                    else if (message.ErrorId == 3)
                    {
                        MessageBox.Show(message.ErrorDescription);
                        LoadEmailConfiguration();
                    }
                }
            }
        }

        private void LoadEmailConfiguration()
        {
            dgEmailConfiguration.ItemsSource = Dalc.Dalc.LoadEmailConfiguration(); ;
            dgEmailConfiguration.Items.Refresh();
        }
        #endregion
        #region Change Username Password
        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            ManagePassword changePassword= new ManagePassword();
            changePassword.Show();
        }

        private void btnChangeName_Click(object sender, RoutedEventArgs e)
        {
            ManageUserName changeUsername = new ManageUserName();
            changeUsername.Show();
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnServerconfig_Click(object sender, RoutedEventArgs e)
        {
            GridDashbord.Visibility = Visibility.Hidden;
            GridConfig.Visibility = Visibility.Visible;
            GridServersConfig.Visibility = Visibility.Visible;
            GridEmailConfig.Visibility = Visibility.Hidden;
            GridUserConfig.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUserconfig_Click(object sender, RoutedEventArgs e)
        {
            GridDashbord.Visibility = Visibility.Hidden;
            GridConfig.Visibility = Visibility.Visible;
            GridServersConfig.Visibility = Visibility.Hidden;
            GridEmailConfig.Visibility = Visibility.Hidden;
            GridUserConfig.Visibility = Visibility.Visible;
        }
        #endregion     
    }
}