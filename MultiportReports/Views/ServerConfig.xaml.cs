using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
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
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Threading;
using System.ComponentModel;
using LiveCharts;
using LiveCharts.Configurations;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using DataMagnetApp.ViewModel;

namespace DataMagnetApp
{
    /// <summary>
    /// Interaction logic for ServerConfig.xaml
    /// </summary>
    public partial class ServerConfig : Window, INotifyPropertyChanged
    {
        // That's our custom TextWriter class
        //private static SerialPort _serialPort;
        //private static bool _continue = false;
        public static Logger logger = new Logger(typeof(ServerConfig));
        static SerialPort _serialPort;
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        TcpClient client1Socket = new TcpClient();
        public static string SelectedMachineName = "";
        BoardData bd = new BoardData();
        Image img = new Image();
        Button btn = new Button();
        // int count = 0;
        public ServerConfig()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComports();
            // LoadLiveCharts();            
        }
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (TabAddMachine.IsSelected)
                {
                    SetServerGridVisibility();
                }
                else if (TabLiveView.IsSelected)
                {

                }
                else if (TabReportServer.IsSelected)
                {
                    if (Common.MachineList.Count > 0)
                    {
                        SelectedMachineName = Common.MachineList[0].MachineName;
                        GetMachineDataByMachineId(SelectedMachineName);
                    }

                }
                else if (TabMachineList.IsSelected)
                {
                    dataGridTelnetMachineList.ItemsSource = Dalc.Dalc.LoadAllMachines("telnet");
                    dataGridTelnetMachineList.Items.Refresh();
                    dataGridComMachineList.ItemsSource = Dalc.Dalc.LoadAllMachines("COMPort");
                    dataGridComMachineList.Items.Refresh();
                }
                else if (TabDashboard.IsSelected)
                {
                    LoadServers();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        #region Chart Control
        public class MeasureModel
        {
            public DateTime DateTime { get; set; }
            public double Value { get; set; }
        }
        private double _axisMax;
        private double _axisMin;
        public ChartValues<MeasureModel> ChartValues { get; set; }
        Worker wrkr = new Worker();
        private static bool _continueth1;

        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }
        public DispatcherTimer Timer { get; set; }
        public bool IsDataInjectionRunning { get; set; }
        public Random R { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunDataOnClick(object sender, RoutedEventArgs e)
        {
            if (IsDataInjectionRunning)
            {
                Timer.Stop();
                IsDataInjectionRunning = false;
            }
            else
            {
                Timer.Start();
                IsDataInjectionRunning = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            var now = DateTime.Now;
            if (!clientSocket.Connected)
            {
                clientSocket.Connect("192.168.1.15", 8888);
                //Thread.Sleep(60000);
            }
            else
            {
                NetworkStream serverStream = clientSocket.GetStream();
                logger.LogInfo("Client Socket Program - Server Connected ...");
                logger.LogInfo("worker thread: working...");
                byte[] inStream = new byte[10025];
                serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                string output = System.Text.Encoding.ASCII.GetString(inStream);
                string replacement = Regex.Replace(output, @"\t|\n|\r|\0", "");
                ChartValues.Add(new MeasureModel
                {
                    DateTime = now,
                    Value = Convert.ToDouble(replacement) // R.Next(0, 10)
                });
            }


            SetAxisLimits(now);

            //lets only use the last 30 values
            if (ChartValues.Count > 60)
                ChartValues.RemoveAt(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="now"></param>
        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(9).Ticks; //we only care about the last 8 seconds
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadLiveCharts()
        {
            try
            {
                //To handle live data easily, in this case we built a specialized type
                //the MeasureModel class, it only contains 2 properties
                //DateTime and Value
                //We need to configure LiveCharts to handle MeasureModel class
                //The next code configures MEasureModel  globally, this means
                //that livecharts learns to plot MeasureModel and will use this config every time
                //a ChartValues instance uses this type.
                //this code ideally should only run once, when application starts is reccomended.
                //you can configure series in many ways, learn more at http://lvcharts.net/App/examples/v1/wpf/Types%20and%20Configuration

                var mapper = Mappers.Xy<MeasureModel>()
                    .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                    .Y(model => model.Value);           //use the value property as Y

                //lets save the mapper globally.
                Charting.For<MeasureModel>(mapper);


                //the values property will store our values array
                ChartValues = new ChartValues<MeasureModel>();

                //lets set how to display the X Labels
                DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

                AxisStep = TimeSpan.FromSeconds(1).Ticks;
                SetAxisLimits(DateTime.Now);

                //The next code simulates data changes every 300 ms
                Timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1000)
                };
                Timer.Tick += TimerOnTick;
                IsDataInjectionRunning = false;
                R = new Random();

                DataContext = this;
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in LoadLiveCharts, Message : " + ex.Message);
            }

        }
        #endregion
        #region Dashboard methods
        private void btnDclose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDmin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion
        #region Server Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddServer_Click(object sender, RoutedEventArgs e)
        {
            if (Common.MachineList.Count == 5)
                MessageBox.Show("You can't add more then 5 machines.");
            else
            {
                btnAddMachine.Visibility = Visibility.Visible;
                btnAddMachinePre.Visibility = Visibility.Visible;
                GridAddServer.Visibility = Visibility.Visible;
                GridOpenServer.Visibility = Visibility.Hidden;
                imgServerConfiguredOK.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenServer_Click(object sender, RoutedEventArgs e)
        {
            GridOpenServer.Visibility = Visibility.Hidden;
            GridConnectedServer.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadServers()
        {
            try
            {
                Common.MachineList = Dalc.Dalc.LoadAllMachines();
                switch (Common.MachineList.Count)
                {
                    case 1:
                        btnServer1.Visibility = Visibility.Visible;
                        btnServer2.Visibility = Visibility.Hidden;
                        btnServer3.Visibility = Visibility.Hidden;
                        btnServer4.Visibility = Visibility.Hidden;
                        btnServer5.Visibility = Visibility.Hidden;
                        break;
                    case 2:
                        btnServer1.Visibility = Visibility.Visible;
                        btnServer2.Visibility = Visibility.Visible;
                        btnServer3.Visibility = Visibility.Hidden;
                        btnServer4.Visibility = Visibility.Hidden;
                        btnServer5.Visibility = Visibility.Hidden;
                        break;
                    case 3:
                        btnServer1.Visibility = Visibility.Visible;
                        btnServer2.Visibility = Visibility.Visible;
                        btnServer3.Visibility = Visibility.Visible;
                        btnServer4.Visibility = Visibility.Hidden;
                        btnServer5.Visibility = Visibility.Hidden;
                        break;
                    case 4:
                        btnServer1.Visibility = Visibility.Visible;
                        btnServer2.Visibility = Visibility.Visible;
                        btnServer3.Visibility = Visibility.Visible;
                        btnServer4.Visibility = Visibility.Visible;
                        btnServer5.Visibility = Visibility.Hidden;
                        break;
                    case 5:
                        btnServer1.Visibility = Visibility.Visible;
                        btnServer2.Visibility = Visibility.Visible;
                        btnServer3.Visibility = Visibility.Visible;
                        btnServer4.Visibility = Visibility.Visible;
                        btnServer5.Visibility = Visibility.Visible;
                        break;
                    default:
                        btnServer1.Visibility = Visibility.Hidden;
                        btnServer2.Visibility = Visibility.Hidden;
                        btnServer3.Visibility = Visibility.Hidden;
                        btnServer4.Visibility = Visibility.Hidden;
                        btnServer5.Visibility = Visibility.Hidden;
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("" + ex.Message);
            }
        }
        private void btnConnectedServerPrevious_Click(object sender, RoutedEventArgs e)
        {
            GridConnectedServer.Visibility = Visibility.Hidden;
            GridOpenServer.Visibility = Visibility.Visible;
            GridServers.Visibility = Visibility.Visible;
            imgOpenServerDark.Visibility = Visibility.Visible;

        }
        private void btnOpenMachinePrevious_Click(object sender, RoutedEventArgs e)
        {
            GridServers.Visibility = Visibility.Visible;
            GridOpenServer.Visibility = Visibility.Hidden;
            GridAddServer.Visibility = Visibility.Visible;
            MiddleImage.Visibility = Visibility.Visible;
            btnAddServer.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Add Machine Basic details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddMachineNext_Click(object sender, RoutedEventArgs e)
        {
            if (txtMachineName.Text == "")
            {
                MessageBox.Show("Machine name can't be empty");
                return;
            }
            else
            {
                GridAddServer.Visibility = Visibility.Hidden;
                MiddleImage.Visibility = Visibility.Hidden;
                GridOpenServer.Visibility = Visibility.Visible;
                btnAddServer.Visibility = Visibility.Hidden;
            }
        }

        private void btnMachineServerNext_Click(object sender, RoutedEventArgs e)
        {
            GridOpenServer.Visibility = Visibility.Hidden;
            MiddleImage.Visibility = Visibility.Hidden;
            GridConnectedServer.Visibility = Visibility.Visible;
            imgOpenServerDark.Visibility = Visibility.Visible;
            btnAddServer.Visibility = Visibility.Hidden;

        }

        private void SetServerGridVisibility()
        {
            GridAddServer.Visibility = Visibility.Hidden;
            GridOpenServer.Visibility = Visibility.Hidden;
            GridConnectedServer.Visibility = Visibility.Hidden;
            MiddleImage.Visibility = Visibility.Visible;
            FinalImage.Visibility = Visibility.Visible;
            btnAddServer.Visibility = Visibility.Visible;
        }

        private void btnServer5_Click(object sender, RoutedEventArgs e)
        {
            SelectedMachineName = Common.MachineList[4].MachineName;
            GetMachineDataByMachineId(SelectedMachineName);
            TabReportServer.IsSelected = true;
        }

        private void btnServer4_Click(object sender, RoutedEventArgs e)
        {
            SelectedMachineName = Common.MachineList[3].MachineName;
            GetMachineDataByMachineId(SelectedMachineName);
            TabReportServer.IsSelected = true;
        }

        private void btnServer3_Click(object sender, RoutedEventArgs e)
        {
            SelectedMachineName = Common.MachineList[2].MachineName;
            GetMachineDataByMachineId(SelectedMachineName);
            TabReportServer.IsSelected = true;
        }

        private void btnServer2_Click(object sender, RoutedEventArgs e)
        {
            SelectedMachineName = Common.MachineList[1].MachineName;
            GetMachineDataByMachineId(SelectedMachineName);
            TabReportServer.IsSelected = true;
        }

        private void btnServer1_Click(object sender, RoutedEventArgs e)
        {
            SelectedMachineName = Common.MachineList[0].MachineName;
            GetMachineDataByMachineId(SelectedMachineName);
            TabReportServer.IsSelected = true;
        }
        private void CMMachineStart1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedMachineName = Common.MachineList[0].MachineName;
                MachinesViewModel FirstMachine = Common.MachineList[0];
                _continueth1 = true;
                Thread readThread = new Thread(ReadFromth1);
                readThread.Start(FirstMachine);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ReadFromth1(object firstmachine)
        {
            MachinesViewModel FirstMachine = (MachinesViewModel)firstmachine;
            while (_continueth1)
            {
                try
                {
                    if (FirstMachine.ConnectionType == Common.ConnectionType.Telnet)
                    {
                        if (!client1Socket.Connected)
                        {
                            client1Socket.Connect(FirstMachine.MachineIP, Convert.ToInt32(FirstMachine.MachinePort));
                        }
                        NetworkStream serverStream = client1Socket.GetStream();
                        byte[] inStream = new byte[10025];
                        serverStream.Read(inStream, 0, (int)client1Socket.ReceiveBufferSize);
                        string replacement = Regex.Replace(System.Text.Encoding.ASCII.GetString(inStream), @"\t|\0", "");
                        logger.LogInfo("Received message from server : " + replacement);
                        //if (replacement == "1")
                       // {
                            Common.AddDataInDatabase(SelectedMachineName, replacement);
                            int result = Common.SendEmail(FirstMachine);
                       // }

                    }
                    else
                    {
                        if (_serialPort != null && _serialPort.IsOpen)
                            _serialPort.Close();
                        if (_serialPort == null)
                        {
                            _serialPort = new SerialPort(FirstMachine.COMPortName);
                            //_serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandlerForFirstMachine);
                            _serialPort.DtrEnable = true;
                            _serialPort.Handshake = Handshake.None;
                            _serialPort.BaudRate = Convert.ToInt32(FirstMachine.COMPortBoadRate);
                            _serialPort.Open();
                            string indata = _serialPort.ReadLine();
                            string message = Regex.Replace(indata, @"\t|\0", "");
                            Common.AddDataInDatabase(SelectedMachineName, message);
                            //Common.SendEmail(Common.MachineList[0]);
                        }
                        else
                        {
                            if (!_serialPort.IsOpen)
                            {
                                _serialPort.Open();
                            }
                            string indata = _serialPort.ReadLine();
                            string message = Regex.Replace(indata, @"\t|\0", "");
                            Common.AddDataInDatabase(SelectedMachineName, message);
                            //Common.SendEmail(Common.MachineList[0]);
                        }
                    }
                }
                catch (TimeoutException tex) { }
                catch (Exception ex)
                {
                    logger.LogError("" + ex.Message);
                }
            }
        }
        private void DataReceivedHandlerForFirstMachine(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string indata = sp.ReadLine();
                string replacement = Regex.Replace(indata, @"\t|\0", "");
                logger.LogInfo("Data received from server, Data : " + replacement);
                Common.AddDataInDatabase(SelectedMachineName, replacement);
                Common.SendEmail(Common.MachineList[0]);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
        private void CMMachineStop1_Click(object sender, RoutedEventArgs e)
        {
            _continueth1 = false;
            if (_serialPort != null && _serialPort.IsOpen)
            {
               // _serialPort.DataReceived -= DataReceivedHandlerForFirstMachine;
                _serialPort.Close();
            }
        }
        private void rbTelnet_Checked(object sender, RoutedEventArgs e)
        {
            GridConTypeTelnet.Visibility = Visibility.Visible;
            GridConTypeCom.Visibility = Visibility.Hidden;
        }
        private void rbCOMPort_Checked(object sender, RoutedEventArgs e)
        {
            GridConTypeCom.Visibility = Visibility.Visible;
            GridConTypeTelnet.Visibility = Visibility.Hidden;
        }
        private void LoadComports()
        {
            try
            {
                // Get a list of serial port names.
                string[] ports = SerialPort.GetPortNames();
                if (ports.Length > 0)
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
                logger.LogError("Exception " + ex.Message);
            }
        }

        private void btnOpenMachineNext_Click(object sender, RoutedEventArgs e)
        {
            if (rbTelnet.IsChecked == true)
            {
                if (txtMachineIP.Text != "" || txtMachinePort.Text != "")
                {
                    lblMachineConType.Content = "telnet";
                    lblMachineIP.Content = txtMachineIP.Text;
                    lblMachinePort.Content = txtMachinePort.Text;
                }
                else
                {
                    MessageBox.Show("Field Can't be Emplty");
                    return;
                }
            }
            else
            {
                ComboBoxItem cbPortsItem = (ComboBoxItem)cmbPorts.SelectedItem;
                ComboBoxItem cbPortsBoadRateItem = (ComboBoxItem)cmbPorts_BoadRate.SelectedItem;
                if (cbPortsItem != null && cbPortsBoadRateItem.Content.ToString() != "")
                {
                    lblMachineConType.Content = "COMPort";
                    lblMachineIP.Content = cbPortsItem.Content.ToString();
                    lblMachinePort.Content = cbPortsBoadRateItem.Content.ToString();
                }
                else
                {
                    MessageBox.Show("Field Can't be Emplty");
                    return;
                }
            }
            GridOpenServer.Visibility = Visibility.Hidden;
            GridConnectedServer.Visibility = Visibility.Visible;
            lblMachineName.Content = txtMachineName.Text;
            lblMachineLocation.Content = txtMachineLocation.Text;
            lblProductName.Content = txtProductName.Text;
            lblSupervisorName.Content = txtSupervisorName.Text;
            lblEmailTo.Content = txtEmailTo.Text;
        }

        private void btnAddMachinePre_Click(object sender, RoutedEventArgs e)
        {
            GridConnectedServer.Visibility = Visibility.Hidden;
            GridOpenServer.Visibility = Visibility = Visibility.Visible;
            if (lblMachineConType.Content.ToString() == "telnet")
            {
                GridConTypeTelnet.Visibility = Visibility.Visible;
                GridConTypeCom.Visibility = Visibility.Hidden;
                rbTelnet.IsChecked = true;
            }
            else
            {
                GridConTypeCom.Visibility = Visibility.Visible;
                GridConTypeTelnet.Visibility = Visibility.Hidden;
                rbCOMPort.IsChecked = true;
            }
        }

        /// <summary>
        /// Method is used to add machine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddMachine_Click(object sender, RoutedEventArgs e)
        {
            MachinesViewModel machine = new MachinesViewModel();
            machine.MachineName = lblMachineName.Content.ToString();
            machine.MachineLocation = lblMachineLocation.Content.ToString();
            machine.ProductName = lblProductName.Content.ToString();
            machine.SupervisorName = lblSupervisorName.Content.ToString();
            machine.ConnectionType = (Common.ConnectionType)lblMachineConType.Content;
            machine.EmailTo = lblEmailTo.Content.ToString();
            if (machine.ConnectionType == Common.ConnectionType.Telnet)
            {
                machine.MachineIP = lblMachineIP.Content.ToString();
                machine.MachinePort = Convert.ToUInt16(lblMachinePort.Content.ToString());
                machine.COMPortName = "";
                machine.COMPortBoadRate = "";
                machine.COMPortParity = "";
            }
            else
            {
                machine.COMPortParity = "";
                machine.MachineIP = "";
                machine.MachinePort = 0;
                machine.COMPortName = lblMachineIP.Content.ToString();
                machine.COMPortBoadRate = lblMachinePort.Content.ToString();
                machine.COMPortParity = "";
            }
            imgServerConfiguredDark.Visibility = Visibility.Hidden;
            ErrorMessage errmsg = Common.ManageMachine(0, machine, CommandTypeEnum.Insert);
            if (errmsg.ErrorId == 1)
            {
                MessageBox.Show("Machine : " + machine.MachineName + " Added successfuly");
                imgServerConfiguredOK.Visibility = Visibility.Visible;
                FinalImage.Visibility = Visibility.Hidden;
                btnAddMachinePre.Visibility = Visibility.Hidden;
                btnAddMachine.Visibility = Visibility.Hidden;
                ClearAddMachineControls();
                TabDashboard.IsSelected = true;
            }
            else if (errmsg.ErrorId == 100)
            {
                MessageBox.Show(errmsg.ErrorDescription);
                imgServerConfiguredOK.Visibility = Visibility.Hidden;
            }

        }
        private void ClearAddMachineControls()
        {
            txtMachineIP.Text = "";
            txtMachineLocation.Text = "";
            txtMachineName.Text = "";
            txtMachinePort.Text = "";
            txtProductName.Text = "";
            lblMachineConType.Content = "";
            lblMachineIP.Content = "";
            lblMachineName.Content = "";
            lblMachinePort.Content = "";
            lblProductName.Content = "";
            lblSupervisorName.Content = "";
            lblEmailTo.Content = "";
            txtSupervisorName.Text = "";
            txtEmailTo.Text = "";
            lblMachineLocation.Content = "";
        }
        private void GridbtndlttelnetMachine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do you really want to delete?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var selectedRow = dataGridTelnetMachineList.SelectedItem as MachinesViewModel;
                    var message = Common.ManageMachine(selectedRow.MachineId, selectedRow, CommandTypeEnum.Delete);
                    if (message.ErrorId == 3)
                    {
                        dataGridTelnetMachineList.ItemsSource = Dalc.Dalc.LoadAllMachines("telnet");
                        dataGridTelnetMachineList.Items.Refresh();
                        MessageBox.Show(message.ErrorDescription);
                    }
                    else
                        MessageBox.Show(message.ErrorDescription);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("File: MainWindow Method: GridbtndlttelnetMachine_Click . Message : " + ex.Message);
            }
        }
        private void GridbtndltComMachine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do you really want to delete?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var selectedRow = dataGridComMachineList.SelectedItem as MachinesViewModel;
                    var message = Common.ManageMachine(selectedRow.MachineId, selectedRow, CommandTypeEnum.Delete);
                    if (message.ErrorId == 3)
                    {
                        dataGridComMachineList.ItemsSource = Dalc.Dalc.LoadAllMachines("COMPort");
                        dataGridComMachineList.Items.Refresh();
                        MessageBox.Show(message.ErrorDescription);
                    }
                    else
                        MessageBox.Show(message.ErrorDescription);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("File: MainWindow Method: GridbtndltComMachine_Click . Message : " + ex.Message);
            }
        }
        #endregion
        #region Reports Method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetMachineDataByMachineId(SelectedMachineName);
        }

        public void GetMachineDataByMachineId(string machineName)
        {
            Common.MachineDataList = Common.GetMachineDataByMachineId(machineName);
            dgReport.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                dgReport.ItemsSource = Common.MachineDataList;
                dgReport.Items.Refresh();
            }));
        }

        private void btnDback_Click(object sender, RoutedEventArgs e)
        {
            _continueth1 = false;
            Dashboard db = new Dashboard();
            db.Show();
            this.Close();
        }



        #endregion

        //public class NameValidator : IDataErrorInfo
        //{
        //    public string ServerName
        //    {
        //        get;
        //        set;
        //    }
        //    public string this[string propertyName]
        //    {
        //        get
        //        {
        //            string result = null;
        //            if (propertyName == "FirstName")
        //            {
        //                if (string.IsNullOrEmpty(this.ServerName))
        //                {
        //                    result = "Cannot be null or empty";
        //                }
        //            }
        //            return result;
        //        }
        //    }
        //}
        //        public override ValidationResult Validate
        //          (object value, System.Globalization.CultureInfo cultureInfo)
        //        {
        //            if (value == null)
        //                return new ValidationResult(false, "value cannot be empty.");
        //            else
        //            {
        //                if (value.ToString().Length > 3)
        //                    return new ValidationResult
        //                    (false, "Name cannot be more than 3 characters long.");
        //            }
        //            return ValidationResult.ValidResult;
        //        }
        //    }
        //}
    }
}
