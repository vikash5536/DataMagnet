using DataMagnetApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

namespace DataMagnetApp
{
    /// <summary>
    /// 
    /// </summary>
    public class Common
    {
        public static List<MachinesViewModel> MachineList = new List<MachinesViewModel>();
        public static List<MachinesViewModel> ComMachineList = new List<MachinesViewModel>();
        public static List<MachinesViewModel> TelMachineList = new List<MachinesViewModel>();
        public static List<UserViewModel> UserList = new List<UserViewModel>();
        public static List<EMailViewModel> EmailServerList = new List<EMailViewModel>();
        public static List<EMailHistoryViewModel> EmailHistoryList = new List<EMailHistoryViewModel>();
        public static List<MachineDataViewModel> MachineDataList = new List<MachineDataViewModel>();
        public static List<EmailTemplateViewModel> EmailTemplateList = new List<EmailTemplateViewModel>();
        public static UserViewModel CurrentUser = new UserViewModel();
        public enum ConnectionType { Telnet = 0 };
        //private static System.Timers.Timer aTimer;
        public static Logger logger = new Logger(typeof(Common));
        #region Email Methods      
        public static List<EMailHistoryViewModel> GetEmailHistory()
        {
            EmailHistoryList.Clear();
            try
            {
                DataTable dt = Dalc.Dalc.GetEmailHistory();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    EMailHistoryViewModel emailHistoryvm = new EMailHistoryViewModel();
                    emailHistoryvm.MachineName= dt.Rows[i]["ServerName"].ToString();
                    emailHistoryvm.EmailFrom = dt.Rows[i]["EmailFrom"].ToString();
                    emailHistoryvm.EmailTo = dt.Rows[i]["EmailTo"].ToString();
                    emailHistoryvm.EmailTo1 = dt.Rows[i]["EmailTo1"].ToString();
                    emailHistoryvm.EmailTo2 = dt.Rows[i]["EmailTo2"].ToString();
                    emailHistoryvm.EmailTo3 = dt.Rows[i]["EmailTo3"].ToString();
                    emailHistoryvm.EmailTo4 = dt.Rows[i]["EmailTo4"].ToString();
                    emailHistoryvm.EmailTo5 = dt.Rows[i]["EmailTo5"].ToString();
                    emailHistoryvm.EmailSubject = dt.Rows[i]["EmailSubject"].ToString();
                    emailHistoryvm.EmailBody = dt.Rows[i]["EmailBody"].ToString();
                    emailHistoryvm.EntryDate = Convert.ToDateTime(dt.Rows[i]["EntryDate"]);
                    emailHistoryvm.EntryDateGroup = Convert.ToDateTime(dt.Rows[i]["EntryDate"]).ToShortDateString();
                    EmailHistoryList.Add(emailHistoryvm);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in GetEmailDetailsByServerIP method, Message : " + ex.Message);
            }
            return EmailHistoryList;
        }
        internal static List<EmailTemplateViewModel> LoadEmailTemplate()
        {
            Common.EmailTemplateList.Clear();
            try
            {
                DataTable dt = Dalc.Dalc.LoadEmailTemplate();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    EmailTemplateViewModel template = new EmailTemplateViewModel();
                    template.EmailSubject = dt.Rows[i]["EmailSubject"].ToString();
                    template.EmailBody = dt.Rows[i]["EmailBody"].ToString();
                    Common.EmailTemplateList.Add(template);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in LoadEmailTemplate, Message : " + ex.Message);
            }
            return EmailTemplateList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailAdd"></param>
        /// <returns></returns>
        public bool IsValidEmail(string mailAdd)
        {
            try
            {
                MailAddress EmailAdd = new MailAddress(mailAdd);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogInfo("Email is not in proper format." + ex.Message);
                return false;
            }
        }
        internal static ErrorMessage ManageEmailConfig(EMailViewModel emailviewmodel, CommandTypeEnum commandtype)
        {
            return Dalc.Dalc.ManageEmailConfig(emailviewmodel, commandtype);
        }
        #endregion
        #region LogIn Methods
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public List<EMailViewModel> GetEmailDetails()
        //{
        //    EMailViewModel emailvm = new EMailViewModel();
        //    try
        //    {
        //        DataTable dt = Dalc.Dalc.GetEmailDetails();
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            emailvm.EmailFrom = dt.Rows[i]["EmailFrom"].ToString();
        //            emailvm.EmailTo = dt.Rows[i]["EmailTo"].ToString();
        //            emailvm.EmailCC = dt.Rows[i]["EmailCC"].ToString();
        //            emailvm.ServerIds = dt.Rows[i]["ServerIds"].ToString();
        //            emailvm.EmailSubject = dt.Rows[i]["EmailSubject"].ToString();
        //            emailvm.EmailBody = dt.Rows[i]["EmailBody"].ToString();
        //            EmailServerList.Add(emailvm);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError("Exception in GetEmailDetails method, Message : " + ex.Message);
        //    }
        //    return EmailServerList;
        //}
        public bool IsValidUser(string UserType, string UserName, string Password)
        {
            bool result = false;
            try
            {
                DataTable dt = Dalc.Dalc.IsValidUser(UserType, UserName, Password);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CurrentUser.UserId = Convert.ToInt16(dt.Rows[i]["Id"].ToString());
                    CurrentUser.FullName = dt.Rows[i]["FullName"].ToString();
                    CurrentUser.Designation = dt.Rows[i]["Designation"].ToString();
                    CurrentUser.Email = dt.Rows[i]["Email"].ToString();
                    CurrentUser.Mobile = dt.Rows[i]["Mobile"].ToString();
                    CurrentUser.UserType = (UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), dt.Rows[i]["UserType"].ToString());
                    CurrentUser.UserName = dt.Rows[i]["UserName"].ToString();
                    CurrentUser.Password = dt.Rows[i]["Password"].ToString();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                result = false;
            }
            return result;
        }
        #endregion
        #region Machine Methods
        /// <summary>
        /// Method is used to add server in database
        /// </summary>
        /// <param name="ServerName">Server Name</param>
        /// <param name="ConnectionType">Telnet or COM Port</param>
        /// <param name="ServerIP">Server IP</param>
        /// <param name="ServerPort">Server Port Number</param>
        /// <param name="COMPortName">COM Port Name</param>
        /// <param name="COMPortParity"></param>
        /// <param name="COMPortBoadRate"></param>
        /// <returns></returns>
        public static ErrorMessage ManageMachine(int MachineId, MachinesViewModel MachineVM, CommandTypeEnum CommandType)
        {
            return Dalc.Dalc.ManageMachine(MachineId, MachineVM, CommandType);
        }
        /// <summary>
        /// Method is used to log all data in to data base.
        /// </summary>
        /// <param name="ServerName"></param>
        public static void AddDataInDatabase(string machineName, string machineData)
        {
            try
            {
                Dalc.Dalc.AddDataInDatabase(machineName, machineData);
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in AddDataInDatabase method, Message :" + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<MachineDataViewModel> GetAllMachineData()
        {
            MachineDataList.Clear();
            try
            {
                DataTable dt = Dalc.Dalc.GetAllMachineData();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    MachineDataList.Add(new MachineDataViewModel
                    {
                        MachineName = dt.Rows[i]["MachineName"].ToString(),
                        MachineData = dt.Rows[i]["MachineData"].ToString(),
                        EntryDate = Convert.ToDateTime(dt.Rows[i]["EntryDate"])
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in GetAllMachineData method, Message : " + ex.Message);
            }
            return MachineDataList;
        }

        public static List<MachineDataViewModel> GetMachineDataByMachineId(string machineName)
        {
            MachineDataList.Clear();
            try
            {
                DataTable dt = Dalc.Dalc.GetMachineDataByMachineName(machineName);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    MachineDataList.Add(new MachineDataViewModel
                    {
                        MachineName = dt.Rows[i]["MachineName"].ToString(),
                        MachineData = dt.Rows[i]["MachineData"].ToString(),
                        EntryDate = Convert.ToDateTime(dt.Rows[i]["EntryDate"])
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in GetAllMachineData method, Message : " + ex.Message);
            }
            return MachineDataList;
        }
        #endregion
        #region User methods
        internal static ErrorMessage ManageUser(UserViewModel userviewmodel, CommandTypeEnum ct)
        {
            return Dalc.Dalc.ManageUser(userviewmodel,ct);
        }

        internal List<UserViewModel> LoadUserDetail()
        {
            UserList.Clear();
            try
            {

                DataTable dt = new Dalc.Dalc().LoadUserDetail();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    UserViewModel userviewmodel = new UserViewModel();
                    userviewmodel.UserId = Convert.ToInt16(dt.Rows[i]["Id"].ToString());
                    userviewmodel.FullName = dt.Rows[i]["FullName"].ToString();
                    userviewmodel.Designation = dt.Rows[i]["Designation"].ToString();
                    userviewmodel.Email = dt.Rows[i]["Email"].ToString();
                    userviewmodel.Mobile = dt.Rows[i]["Mobile"].ToString();
                    userviewmodel.UserType = (UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), dt.Rows[i]["UserType"].ToString());
                    userviewmodel.UserName = dt.Rows[i]["UserName"].ToString();
                    userviewmodel.Password = dt.Rows[i]["Password"].ToString();
                    UserList.Add(userviewmodel);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in LoadUserDetail, Message : " + ex.Message);
            }
            return UserList;
        }

        internal UserViewModel LoadUserDetailByUserName(string username)
        {
            UserViewModel userviewmodel = new UserViewModel();
            try
            {
                DataTable dt = new Dalc.Dalc().LoadUserDetailByUserName(username);
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    userviewmodel.UserId = Convert.ToInt16(dt.Rows[i]["Id"].ToString());
                    userviewmodel.FullName = dt.Rows[i]["FullName"].ToString();
                    userviewmodel.Designation = dt.Rows[i]["Designation"].ToString();
                    userviewmodel.Email = dt.Rows[i]["Email"].ToString();
                    userviewmodel.Mobile = dt.Rows[i]["Mobile"].ToString();
                    userviewmodel.UserType = (UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), dt.Rows[i]["UserType"].ToString());
                    userviewmodel.UserName = dt.Rows[i]["UserName"].ToString();
                    userviewmodel.Password = dt.Rows[i]["Password"].ToString();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in LoadUserDetail, Message : " + ex.Message);
            }
            return userviewmodel;
        }

        internal static int SendEmail(MachinesViewModel selectedMachine)
        {
            int result = -1;
            try
            {
                List<EMailViewModel> emailConfigList = Dalc.Dalc.LoadEmailConfiguration();
                
                if (emailConfigList.Count > 0)
                {                  
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress(emailConfigList[0].EmailFrom);
                        if (selectedMachine.EmailTo != "")
                            mail.To.Add(selectedMachine.EmailTo);
                        else
                            mail.To.Add(emailConfigList[0].EmailTo1);
                        mail.Subject = emailConfigList[0].EmailSubject;
                        mail.Body = emailConfigList[0].EmailBody;
                        mail.IsBodyHtml = true;
                        // Can set to false, if you are sending pure text.
                        //mail.Attachments.Add(new Attachment(@"D:\Projects\WPFProject\IDFI_IVMS\IVMSLatest_1_oct\IVMS\IVMS\Images\information.png"));
                        //mail.Attachments.Add(new Attachment("C:\\SomeZip.zip"));
                        using (SmtpClient smtp = new SmtpClient(emailConfigList[0].SMTPAddress, emailConfigList[0].SMTPPort))
                        {
                            smtp.Credentials = new NetworkCredential(emailConfigList[0].EmailFrom, emailConfigList[0].EmailPassword);
                            smtp.EnableSsl = emailConfigList[0].EnableSSL;
                            //smtp.Send(mail);
                            result = Dalc.Dalc.AddEmailToDatabase(0, selectedMachine, emailConfigList[0]);
                            if (result != -1)
                            {
                                logger.LogInfo("Email added successfuly in database.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = -1;
                logger.LogError("Exception in method SendEmail, for server : " + selectedMachine.MachineName + "  Message : " + ex.Message);
            }
            return result;
        }
   
        #endregion
    }
    /// <summary>
    /// Class for worker thread, to get data from server.
    /// </summary>
    public class Worker
    {
        private volatile bool _shouldStop;
        private volatile bool _shouldStopAll;
        //Common cmn = new Common();
        public static Logger logger = new Logger(typeof(Worker));
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        TcpClient[] clientSockets = new TcpClient[5];
        public static double ChartYValue = 0;
        public void DoWork(object obj)
        {
            try
            {
                string[] serverDetails = obj.ToString().Split(' ');
                while (!_shouldStop)
                {
                    if (!clientSocket.Connected)
                    {
                        clientSocket.Connect(serverDetails[0], Convert.ToInt32(serverDetails[1]));
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
                        if (replacement.Trim() == "Hello")
                        {
                            // Common.SendEmail(serverDetails[0]);
                        }
                    }
                }
                logger.LogInfo("worker thread: terminating gracefully.");
            }
            catch (Exception ex)
            {
                logger.LogInfo("worker thread: not terminated gracefully." + ex.Message);
            }

        }

        public void DoWorkAll()
        {
            try
            {
                if (Common.MachineList.Count == 0)
                {
                    Common.MachineList = Dalc.Dalc.LoadAllMachines();
                }
                while (!_shouldStopAll)
                {
                    for (int i = 0; i < Common.MachineList.Count; i++)
                    {
                        if (clientSockets[i] == null)
                            clientSockets[i] = new TcpClient();
                        if (!clientSockets[i].Connected)
                        {
                            clientSockets[i].Connect(Common.MachineList[i].MachineIP, Convert.ToInt32(Common.MachineList[i].MachinePort));
                        }
                        else
                        {
                            NetworkStream serverStream = clientSockets[i].GetStream();
                            logger.LogInfo("Client Socket Program - Server Connected ...");
                            logger.LogInfo("worker thread: working...");
                            byte[] inStream = new byte[10025];
                            serverStream.Read(inStream, 0, (int)clientSockets[i].ReceiveBufferSize);
                            string output = System.Text.Encoding.ASCII.GetString(inStream);
                            string replacement = Regex.Replace(output, @"\t|\n|\r|\0", "");
                            if (replacement.Trim() == "Hello")
                            {
                                //Common.SendEmail(Common.MachineList[i].MachineIP);
                            }
                        }
                        logger.LogInfo("worker thread: terminating gracefully. for server : " + Common.MachineList[i].MachineName);
                    }
                    Thread.Sleep(60000);
                }
            }
            catch (Exception ex)
            {
                logger.LogInfo("Worker thread: not terminated gracefully, Exception : " + ex.Message);
            }
        }

        /// <summary>
        /// Method is used to stop all Server.
        /// </summary>
        public void RequestStopAll()
        {
            _shouldStopAll = true;
            logger.LogInfo("All worker thread: terminated gracefully.");
        }
        /// <summary>
        /// 
        /// </summary>
        public void RequestStop()
        {
            _shouldStop = true;
            logger.LogInfo("Worker thread: terminated gracefully.");
        }

        public bool CheckServerConnectivity(string ServerIP, string ServerPort)
        {
            bool result = false;
            try
            {
                if (clientSocket == null)
                    clientSocket = new TcpClient();
                if (!clientSocket.Connected)
                {
                    clientSocket.Connect(ServerIP, Convert.ToInt32(ServerPort));
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in Common Class,method CheckServerConnectivity, Message : " + ex.Message);
            }
            return result;
        }
    }
}
