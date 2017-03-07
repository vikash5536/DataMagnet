using DataMagnetApp.ViewModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DataMagnetApp.Dalc
{

    public class Dalc
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["myCon"].ConnectionString;
        public static Logger logger = new Logger(typeof(Dalc));

        #region LogIn Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserType"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static DataTable IsValidUser(string UserType, string UserName, string Password)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM tblusers where UserType ='" + UserType + "' and UserName = '" + UserName + "' and Password = '" + Password + "';", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in IsValidUser method, Message : " + ex.Message);
            }
            return dt;
        }
        #endregion
        #region Machines Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="serverVM"></param>
        /// <param name="CommandType"></param>
        /// <returns></returns>
        public static ErrorMessage ManageMachine(int MachineId, MachinesViewModel machineVM, CommandTypeEnum CommandType)
        {
            ErrorMessage errmsg = new ErrorMessage();
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("spManageMachine", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    MySqlParameter pId = new MySqlParameter();
                    pId.ParameterName = "in_Id";
                    pId.Value = MachineId;
                    cmd.Parameters.Add(pId);

                    MySqlParameter pMachineName = new MySqlParameter();
                    pMachineName.ParameterName = "in_MachineName";
                    pMachineName.Value = machineVM.MachineName;
                    cmd.Parameters.Add(pMachineName);

                    MySqlParameter pMachineLocation = new MySqlParameter();
                    pMachineLocation.ParameterName = "in_MachineLocation";
                    pMachineLocation.Value = machineVM.MachineLocation;
                    cmd.Parameters.Add(pMachineLocation);

                    MySqlParameter pProductName = new MySqlParameter();
                    pProductName.ParameterName = "in_ProductName";
                    pProductName.Value = machineVM.ProductName;
                    cmd.Parameters.Add(pProductName);

                    MySqlParameter pSupervisorName = new MySqlParameter();
                    pSupervisorName.ParameterName = "in_SupervisorName";
                    pSupervisorName.Value = machineVM.SupervisorName;
                    cmd.Parameters.Add(pSupervisorName);

                    MySqlParameter pEmailTo = new MySqlParameter();
                    pEmailTo.ParameterName = "in_EmailTo";
                    pEmailTo.Value = machineVM.EmailTo;
                    cmd.Parameters.Add(pEmailTo);

                    MySqlParameter pConnectionType = new MySqlParameter();
                    pConnectionType.ParameterName = "in_ConnectionType";
                    pConnectionType.Value = machineVM.ConnectionType;
                    cmd.Parameters.Add(pConnectionType);

                    MySqlParameter pServerIP = new MySqlParameter();
                    pServerIP.ParameterName = "in_MachineIP";
                    pServerIP.Value = machineVM.MachineIP;
                    cmd.Parameters.Add(pServerIP);

                    MySqlParameter pServerPort = new MySqlParameter();
                    pServerPort.ParameterName = "in_MachinePort";
                    pServerPort.Value = machineVM.MachinePort;
                    cmd.Parameters.Add(pServerPort);

                    MySqlParameter pComportName = new MySqlParameter();
                    pComportName.ParameterName = "in_ComportName";
                    pComportName.Value = machineVM.COMPortName;
                    cmd.Parameters.Add(pComportName);

                    MySqlParameter pComportParity = new MySqlParameter();
                    pComportParity.ParameterName = "in_ComportParity";
                    pComportParity.Value = machineVM.COMPortParity;
                    cmd.Parameters.Add(pComportParity);

                    MySqlParameter pin_ComPortBaudRate = new MySqlParameter();
                    pin_ComPortBaudRate.ParameterName = "in_ComPortBaudRate";
                    pin_ComPortBaudRate.Value = machineVM.COMPortBoadRate;
                    cmd.Parameters.Add(pin_ComPortBaudRate);

                    MySqlParameter pCommandType = new MySqlParameter();
                    pCommandType.ParameterName = "in_CommandType";
                    pCommandType.Value = CommandType.ToString();
                    cmd.Parameters.Add(pCommandType);

                    MySqlParameter outPutErrorId = new MySqlParameter();
                    outPutErrorId.ParameterName = "out_ErrorId";
                    outPutErrorId.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(outPutErrorId);

                    MySqlParameter outPutErrorMessage = new MySqlParameter();
                    outPutErrorMessage.ParameterName = "out_ErrorMessage";
                    outPutErrorMessage.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(outPutErrorMessage);

                    cmd.ExecuteNonQuery();
                    errmsg.ErrorId = (int)outPutErrorId.Value;
                    errmsg.ErrorDescription = outPutErrorMessage.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in DALC CLASS, Method Name AddMachine, Message : " + ex.Message);
                errmsg.ErrorId = 0;
                errmsg.ErrorDescription = ex.Message;
            }
            return errmsg;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllMachineData()
        {
            DataTable dt = new DataTable("Reports");
            try
            {
                string CmdString = string.Empty;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    CmdString = "SELECT MachineName, MachineData, EntryDate from tblmachinedata order by EntryDate desc";
                    MySqlCommand cmd = new MySqlCommand(CmdString, con);
                    MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                    sda.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in GetAllMachineData method, Message : " + ex.Message);
            }
            return dt;
        }

        public static DataTable GetMachineDataByMachineName(string machineName)
        {
            DataTable dt = new DataTable("Reports");
            try
            {
                string CmdString = string.Empty;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    CmdString = "SELECT MachineName, MachineData, EntryDate from tblmachinedata where MachineName =" + machineName + "order by EntryDate desc";
                    MySqlCommand cmd = new MySqlCommand(CmdString, con);
                    MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                    sda.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in GetMachineDataByMachineName method, Message : " + ex.Message);
            }
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="machineData"></param>
        public static void AddDataInDatabase(string machineName, string machineData)
        {
            try
            {
                string Query = "insert into tblmachinedata(MachineName,MachineData,EntryDate) values('" + machineName + "','" + machineData + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "');";
                //This is  MySqlConnection here i have created the object and pass my connection string.  
                MySqlConnection MyConn2 = new MySqlConnection(connectionString);
                //This is command class which will handle the query and connection object.  
                MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                MyConn2.Open();
                MyCommand2.ExecuteNonQuery();
                MyConn2.Close();
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in AddDataInDatabase , Message : " + ex.Message);
            }
        }
        /// <summary>
        /// Method is used to get all server records.
        /// </summary>
        /// <returns></returns>
        public static List<MachinesViewModel> LoadAllMachines()
        {
            Common.MachineList.Clear();
            try
            {
                using (var sqlConnection = new MySqlConnection(connectionString))
                {

                    if (sqlConnection.State != ConnectionState.Open)
                    {
                        sqlConnection.Open();
                    }
                    using (var sqlCommand = sqlConnection.CreateCommand())
                    {

                        sqlCommand.CommandText = "SELECT * FROM tblservers";
                        using (var sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            while (sqlDataReader.Read())
                            {
                                var machineId = sqlDataReader.GetUInt16(sqlDataReader.GetOrdinal("Id"));
                                var machineName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("MachineName"));
                                var machineLocation = sqlDataReader.GetString(sqlDataReader.GetOrdinal("MachineLocation"));
                                var productName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("ProductName"));
                                var supervisorName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("SupervisorName"));
                                var emailto = sqlDataReader.GetString(sqlDataReader.GetOrdinal("EmailTo"));
                                var connectionType = (Common.ConnectionType)Enum.Parse(typeof(Common.ConnectionType), sqlDataReader.GetString(sqlDataReader.GetOrdinal("ConnectionType")));
                                var machineIP = sqlDataReader.GetString(sqlDataReader.GetOrdinal("MachineIP"));
                                var machinePort = sqlDataReader.GetUInt16(sqlDataReader.GetOrdinal("MachinePort"));
                                var comPortName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("ComportName"));
                                var comPortParity = sqlDataReader.GetString(sqlDataReader.GetOrdinal("ComportParity"));
                                var comPortBoadRate = sqlDataReader.GetString(sqlDataReader.GetOrdinal("ComPortBaudRate"));
                                Common.MachineList.Add(new MachinesViewModel { MachineId = machineId, MachineName = machineName, MachineLocation = machineLocation, ProductName = productName, SupervisorName = supervisorName, EmailTo = emailto, ConnectionType = connectionType, MachineIP = machineIP, MachinePort = machinePort, COMPortName = comPortName, COMPortBoadRate = comPortBoadRate, COMPortParity = comPortParity });
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                logger.LogError("Exception in GetAllServer method, Message : " + ex.Message);
            }
            return Common.MachineList;
        }
        /// <summary>
        /// Method is used to get all server records.
        /// </summary>
        /// <returns></returns>
        public static List<MachinesViewModel> LoadAllMachines(string connType)
        {
            if (connType == "telnet")
                Common.TelMachineList.Clear();
            else
                Common.ComMachineList.Clear();
            try
            {
                using (var sqlConnection = new MySqlConnection(connectionString))
                {

                    if (sqlConnection.State != ConnectionState.Open)
                    {
                        sqlConnection.Open();
                    }
                    using (var sqlCommand = sqlConnection.CreateCommand())
                    {

                        sqlCommand.CommandText = "SELECT * FROM tblservers where connectionType = '" + connType + "'";
                        using (var sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            while (sqlDataReader.Read())
                            {
                                var machineId = sqlDataReader.GetUInt16(sqlDataReader.GetOrdinal("Id"));
                                var machineName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("MachineName"));
                                var machineLocation = sqlDataReader.GetString(sqlDataReader.GetOrdinal("MachineLocation"));
                                var productName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("ProductName"));
                                var supervisorName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("SupervisorName"));
                                var emailto = sqlDataReader.GetString(sqlDataReader.GetOrdinal("EmailTo"));
                                var connectionType = (Common.ConnectionType)Enum.Parse(typeof(Common.ConnectionType), sqlDataReader.GetString(sqlDataReader.GetOrdinal("ConnectionType")));
                                var machineIP = sqlDataReader.GetString(sqlDataReader.GetOrdinal("MachineIP"));
                                var machinePort = sqlDataReader.GetUInt16(sqlDataReader.GetOrdinal("MachinePort"));
                                var comPortName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("ComportName"));
                                var comPortParity = sqlDataReader.GetString(sqlDataReader.GetOrdinal("ComportParity"));
                                var comPortBoadRate = sqlDataReader.GetString(sqlDataReader.GetOrdinal("ComPortBaudRate"));
                                if (connType == "telnet")
                                    Common.TelMachineList.Add(new MachinesViewModel { MachineId = machineId, MachineName = machineName, MachineLocation = machineLocation, ProductName = productName, SupervisorName = supervisorName, EmailTo = emailto, ConnectionType = connectionType, MachineIP = machineIP, MachinePort = machinePort, COMPortName = comPortName, COMPortBoadRate = comPortBoadRate, COMPortParity = comPortParity });
                                else
                                    Common.ComMachineList.Add(new MachinesViewModel { MachineId = machineId, MachineName = machineName, MachineLocation = machineLocation, ProductName = productName, SupervisorName = supervisorName, EmailTo = emailto, ConnectionType = connectionType, MachineIP = machineIP, MachinePort = machinePort, COMPortName = comPortName, COMPortBoadRate = comPortBoadRate, COMPortParity = comPortParity });

                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                logger.LogError("Exception in GetAllServer method, Message : " + ex.Message);
            }
            if (connType == "telnet")
                return Common.TelMachineList;
            else
                return Common.ComMachineList;
        }
        #endregion
        #region Email Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GetEmailHistory()
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM  tblemailhistory", con))
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in GetEmailHistory , Message : " + ex.Message);
            }
            return dt;
        }

        internal static List<EMailViewModel> LoadEmailConfiguration()
        {
            List<EMailViewModel> emaillist = new List<EMailViewModel>();
            DataTable dt = new DataTable("Reports");
            try
            {
                string CmdString = string.Empty;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    CmdString = "select mailconfig.id, mailconfig.EmailName, mailconfig.EmailFrom, mailconfig.EmailPassword,mailconfig.SMTPAddress,mailconfig.SMTPPort, mailconfig.EnableSSL,mailconfig.EmailTo1, mailconfig.EmailTo2, mailconfig.EmailTo3, mailconfig.EmailTo4, mailconfig.EmailTo5, mailconfig.EntryDate,emltmplt.EmailSubject,emltmplt.EmailBody from tblemailconfig  mailconfig join tblemailtemplet emltmplt on emltmplt.id = mailconfig.EmailTemplateId";
                    MySqlCommand cmd = new MySqlCommand(CmdString, con);
                    MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                    sda.Fill(dt);
                }
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        EMailViewModel emailvm = new EMailViewModel();
                        emailvm.Id = Convert.ToInt16(dt.Rows[i]["Id"]);
                        emailvm.EmailFrom = dt.Rows[i]["EmailFrom"].ToString();
                        emailvm.EmailName = dt.Rows[i]["EmailName"].ToString();
                        emailvm.EmailPassword = dt.Rows[i]["EmailPassword"].ToString();
                        emailvm.SMTPAddress = dt.Rows[i]["SMTPAddress"].ToString();
                        emailvm.SMTPPort = Convert.ToInt16(dt.Rows[i]["SMTPPort"]);
                        emailvm.EnableSSL = Convert.ToBoolean(dt.Rows[i]["SMTPPort"]);
                        emailvm.EntryDate = Convert.ToDateTime(dt.Rows[i]["EntryDate"].ToString());
                        emailvm.EmailTo1 = dt.Rows[i]["EmailTo1"].ToString();
                        emailvm.EmailTo2 = dt.Rows[i]["EmailTo2"].ToString();
                        emailvm.EmailTo3 = dt.Rows[i]["EmailTo3"].ToString();
                        emailvm.EmailTo4 = dt.Rows[i]["EmailTo4"].ToString();
                        emailvm.EmailTo5 = dt.Rows[i]["EmailTo5"].ToString();
                        emailvm.EmailSubject = dt.Rows[i]["EmailSubject"].ToString();
                        emailvm.EmailBody = dt.Rows[i]["EmailBody"].ToString();
                        emaillist.Add(emailvm);
                    }                    
                }            
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in LoadEmailConfiguration method, Message : " + ex.Message);
                return null;
            }
            return emaillist;
        }

/// <summary>
/// 
/// </summary>
/// <param name="ServerName"></param>
/// <param name="EmailVM"></param>
public static int AddEmailToDatabase(int Id, MachinesViewModel SelectedMachine, EMailViewModel EmailVM)
{
    int result = -1;
    try
    {
        using (MySqlConnection con = new MySqlConnection(connectionString))
        {
            con.Open();
            MySqlCommand cmd = new MySqlCommand("spAddEmailLog", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            MySqlParameter pId = new MySqlParameter();
            pId.ParameterName = "in_Id";
            pId.Value = Id;
            cmd.Parameters.Add(pId);

            MySqlParameter pEmailFrom = new MySqlParameter();
            pEmailFrom.ParameterName = "in_EmailFrom";
            pEmailFrom.Value = EmailVM.EmailFrom;
            cmd.Parameters.Add(pEmailFrom);

            MySqlParameter pServerName = new MySqlParameter();
            pServerName.ParameterName = "in_ServerName";
            pServerName.Value = SelectedMachine.MachineName;
            cmd.Parameters.Add(pServerName);

            MySqlParameter pEmailTo = new MySqlParameter();
            pEmailTo.ParameterName = "in_EmailTo";
            pEmailTo.Value = SelectedMachine.EmailTo;
            cmd.Parameters.Add(pEmailTo);

            MySqlParameter pEmailTo1 = new MySqlParameter();
            pEmailTo1.ParameterName = "in_EmailTo1";
            pEmailTo1.Value = EmailVM.EmailTo1;
            cmd.Parameters.Add(pEmailTo1);

            MySqlParameter pEmailTo2 = new MySqlParameter();
            pEmailTo2.ParameterName = "in_EmailTo2";
            pEmailTo2.Value = EmailVM.EmailTo2;
            cmd.Parameters.Add(pEmailTo2);

            MySqlParameter pEmailTo3 = new MySqlParameter();
            pEmailTo3.ParameterName = "in_EmailTo3";
            pEmailTo3.Value = EmailVM.EmailTo3;
            cmd.Parameters.Add(pEmailTo3);

            MySqlParameter pEmailTo4 = new MySqlParameter();
            pEmailTo4.ParameterName = "in_EmailTo4";
            pEmailTo4.Value = EmailVM.EmailTo4;
            cmd.Parameters.Add(pEmailTo4);
            MySqlParameter pEmailTo5 = new MySqlParameter();
            pEmailTo5.ParameterName = "in_EmailTo5";
            pEmailTo5.Value = EmailVM.EmailTo5;
            cmd.Parameters.Add(pEmailTo5);

            MySqlParameter pEmailSubject = new MySqlParameter();
            pEmailSubject.ParameterName = "in_EmailSubject";
            pEmailSubject.Value = EmailVM.EmailSubject;
            cmd.Parameters.Add(pEmailSubject);

            MySqlParameter pEmailBody = new MySqlParameter();
            pEmailBody.ParameterName = "in_EmailBody";
            pEmailBody.Value = EmailVM.EmailBody;
            cmd.Parameters.Add(pEmailBody);

            MySqlParameter pEntryDate = new MySqlParameter();
            pEntryDate.ParameterName = "in_EntryDate";
            pEntryDate.Value = DateTime.Now;
            cmd.Parameters.Add(pEntryDate);

            result = cmd.ExecuteNonQuery();
        }
    }
    catch (Exception ex)
    {
        result = -1;
        logger.LogError("Exception in AddEmailToDatabase method,Message : " + ex.Message);
    }
    return result;
}
/// <summary>
/// 
/// </summary>
/// <param name="emailSubject"></param>
/// <param name="emailBody"></param>
/// <returns></returns>
public static int AddEmailTemplate(string emailSubject, string emailBody)
{
    int result = -1;
    try
    {
        using (MySqlConnection con = new MySqlConnection(connectionString))
        {
            con.Open();
            MySqlCommand cmd = new MySqlCommand("spEmailTemplate", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            MySqlParameter pMailSub = new MySqlParameter();
            pMailSub.ParameterName = "in_mailSub";
            pMailSub.Value = emailSubject;
            cmd.Parameters.Add(pMailSub);

            MySqlParameter pMailBody = new MySqlParameter();
            pMailBody.ParameterName = "in_mailBody";
            pMailBody.Value = emailBody;
            cmd.Parameters.Add(pMailBody);

            MySqlParameter outPutParameter = new MySqlParameter();
            outPutParameter.ParameterName = "out_result";
            outPutParameter.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(outPutParameter);

            cmd.ExecuteNonQuery();
            result = (int)outPutParameter.Value;
        }
    }
    catch (Exception ex)
    {
        logger.LogError("Exception in AddEmailTemplate, Message : " + ex.Message);
    }
    return result;
}

public static DataTable LoadEmailTemplate()
{
    DataTable dt = new DataTable();
    try
    {
        using (MySqlConnection con = new MySqlConnection(connectionString))
        {
            con.Open();
            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM  tblemailtemplet", con))
            {
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            con.Close();
        }
    }
    catch (Exception ex)
    {
        logger.LogError("Exception in GetEmailHistory , Message : " + ex.Message);
    }
    return dt;
}

internal static ErrorMessage ManageEmailConfig(EMailViewModel EmailVM, CommandTypeEnum commandtype)
{
    ErrorMessage errmsg = new ErrorMessage();
    string connectionString = ConfigurationManager.ConnectionStrings["myCon"].ConnectionString;
    try
    {
        using (MySqlConnection con = new MySqlConnection(connectionString))
        {
            con.Open();
            MySqlCommand cmd = new MySqlCommand("spEmailConfig", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            MySqlParameter pId = new MySqlParameter();
            pId.ParameterName = "in_Id";
            pId.Value = EmailVM.Id;
            cmd.Parameters.Add(pId);

            MySqlParameter pEmailName = new MySqlParameter();
            pEmailName.ParameterName = "in_EmailName";
            pEmailName.Value = EmailVM.EmailName;
            cmd.Parameters.Add(pEmailName);

            MySqlParameter pEmailFrom = new MySqlParameter();
            pEmailFrom.ParameterName = "in_EmailFrom";
            pEmailFrom.Value = EmailVM.EmailFrom;
            cmd.Parameters.Add(pEmailFrom);

            MySqlParameter pEmailPassword = new MySqlParameter();
            pEmailPassword.ParameterName = "in_EmailPassword";
            pEmailPassword.Value = EmailVM.EmailPassword;
            cmd.Parameters.Add(pEmailPassword);

            MySqlParameter pSMTPAddress = new MySqlParameter();
            pSMTPAddress.ParameterName = "in_SMTPAddress";
            pSMTPAddress.Value = EmailVM.SMTPAddress;
            cmd.Parameters.Add(pSMTPAddress);

            MySqlParameter pSMTPPort = new MySqlParameter();
            pSMTPPort.ParameterName = "in_SMTPPort";
            pSMTPPort.Value = EmailVM.SMTPPort;
            cmd.Parameters.Add(pSMTPPort);

            MySqlParameter pEnableSSL = new MySqlParameter();
            pEnableSSL.ParameterName = "in_EnableSSL";
            pEnableSSL.Value = EmailVM.EnableSSL;
            cmd.Parameters.Add(pEnableSSL);

            MySqlParameter pEmailTo1 = new MySqlParameter();
            pEmailTo1.ParameterName = "in_EmailTo1";
            pEmailTo1.Value = EmailVM.EmailTo1;
            cmd.Parameters.Add(pEmailTo1);

            MySqlParameter pEmailTo2 = new MySqlParameter();
            pEmailTo2.ParameterName = "in_EmailTo2";
            pEmailTo2.Value = EmailVM.EmailTo2;
            cmd.Parameters.Add(pEmailTo2);

            MySqlParameter pEmailTo3 = new MySqlParameter();
            pEmailTo3.ParameterName = "in_EmailTo3";
            pEmailTo3.Value = EmailVM.EmailTo3;
            cmd.Parameters.Add(pEmailTo3);

            MySqlParameter pEmailTo4 = new MySqlParameter();
            pEmailTo4.ParameterName = "in_EmailTo4";
            pEmailTo4.Value = EmailVM.EmailTo4;
            cmd.Parameters.Add(pEmailTo4);

            MySqlParameter pEmailTo5 = new MySqlParameter();
            pEmailTo5.ParameterName = "in_EmailTo5";
            pEmailTo5.Value = EmailVM.EmailTo5;
            cmd.Parameters.Add(pEmailTo5);

            MySqlParameter pEntryDate = new MySqlParameter();
            pEntryDate.ParameterName = "in_EntryDate";
            pEntryDate.Value = EmailVM.EntryDate;
            cmd.Parameters.Add(pEntryDate);

            MySqlParameter pCommandType = new MySqlParameter();
            pCommandType.ParameterName = "in_CommandType";
            pCommandType.Value = commandtype.ToString();
            cmd.Parameters.Add(pCommandType);

            MySqlParameter outPutErrorId = new MySqlParameter();
            outPutErrorId.ParameterName = "out_ErrorId";
            outPutErrorId.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(outPutErrorId);

            MySqlParameter outPutErrorMessage = new MySqlParameter();
            outPutErrorMessage.ParameterName = "out_ErrorMessage";
            outPutErrorMessage.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(outPutErrorMessage);

            cmd.ExecuteNonQuery();
            errmsg.ErrorId = (int)outPutErrorId.Value;
            errmsg.ErrorDescription = outPutErrorMessage.Value.ToString();
        }
    }
    catch (Exception ex)
    {
        errmsg.ErrorId = 0;
        errmsg.ErrorDescription = ex.Message;
    }
    return errmsg;
}

#endregion
#region User Methods
public static bool IsLength(String data, int minLength, int maxLength)
{
    if ((data.Length >= minLength) && data.Length <= maxLength)
        return true;
    else
        return false;
}

internal static ErrorMessage ManageUser(UserViewModel userviewmodel, CommandTypeEnum commandtype)
{
    ErrorMessage errmsg = new ErrorMessage();
    string connectionString = ConfigurationManager.ConnectionStrings["myCon"].ConnectionString;
    try
    {
        using (MySqlConnection con = new MySqlConnection(connectionString))
        {
            con.Open();
            MySqlCommand cmd = new MySqlCommand("sp_ManageUser", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            MySqlParameter pin_UserId = new MySqlParameter();
            pin_UserId.ParameterName = "in_UserId";
            pin_UserId.Value = userviewmodel.UserId;
            cmd.Parameters.Add(pin_UserId);

            MySqlParameter pin_FullName = new MySqlParameter();
            pin_FullName.ParameterName = "in_FullName";
            pin_FullName.Value = userviewmodel.FullName;
            cmd.Parameters.Add(pin_FullName);

            MySqlParameter pin_Designation = new MySqlParameter();
            pin_Designation.ParameterName = "in_Designation";
            pin_Designation.Value = userviewmodel.Designation;
            cmd.Parameters.Add(pin_Designation);

            MySqlParameter pin_Email = new MySqlParameter();
            pin_Email.ParameterName = "in_Email";
            pin_Email.Value = userviewmodel.Email;
            cmd.Parameters.Add(pin_Email);

            MySqlParameter pin_Mobile = new MySqlParameter();
            pin_Mobile.ParameterName = "in_Mobile";
            pin_Mobile.Value = userviewmodel.Mobile;
            cmd.Parameters.Add(pin_Mobile);

            MySqlParameter pin_UserName = new MySqlParameter();
            pin_UserName.ParameterName = "in_UserName";
            pin_UserName.Value = userviewmodel.UserName;
            cmd.Parameters.Add(pin_UserName);

            MySqlParameter pin_Password = new MySqlParameter();
            pin_Password.ParameterName = "in_Password";
            pin_Password.Value = userviewmodel.Password;
            cmd.Parameters.Add(pin_Password);

            MySqlParameter pCommandType = new MySqlParameter();
            pCommandType.ParameterName = "in_CommandType";
            pCommandType.Value = commandtype.ToString();
            cmd.Parameters.Add(pCommandType);

            MySqlParameter outPutErrorId = new MySqlParameter();
            outPutErrorId.ParameterName = "out_ErrorId";
            outPutErrorId.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(outPutErrorId);

            MySqlParameter outPutErrorMessage = new MySqlParameter();
            outPutErrorMessage.ParameterName = "out_ErrorMessage";
            outPutErrorMessage.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(outPutErrorMessage);

            cmd.ExecuteNonQuery();
            errmsg.ErrorId = (int)outPutErrorId.Value;
            errmsg.ErrorDescription = outPutErrorMessage.Value.ToString();
        }
    }
    catch (Exception ex)
    {
        errmsg.ErrorId = 0;
        errmsg.ErrorDescription = "Exception in ManageUser method, Message" + ex.Message;
    }
    return errmsg;
}
public DataTable LoadUserDetail()
{
    DataTable dt = new DataTable("UserDetail");
    try
    {
        string _connectionString = ConfigurationManager.ConnectionStrings["myCon"].ConnectionString;
        using (var sqlConnection = new MySqlConnection(_connectionString))
        {

            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            string CmdString = "select * from tblusers";
            MySqlCommand cmd = new MySqlCommand(CmdString, sqlConnection);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            sda.Fill(dt);
        }
    }
    catch (Exception ex)
    {
        logger.LogError("File: Dalc Method: LoadUserDetail . Message : " + ex.Message);
    }
    return dt;
}
public DataTable LoadUserDetailByUserName(string username)
{
    DataTable dt = new DataTable("UserDetail");
    try
    {
        string _connectionString = ConfigurationManager.ConnectionStrings["myCon"].ConnectionString;
        using (var sqlConnection = new MySqlConnection(_connectionString))
        {

            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            string CmdString = "select * from tblusers where UserName =  '" + username + "'";
            MySqlCommand cmd = new MySqlCommand(CmdString, sqlConnection);
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            sda.Fill(dt);
        }
    }
    catch (Exception ex)
    {
        logger.LogError("File: Dalc Method: LoadUserDetail . Message : " + ex.Message);
    }
    return dt;
}

        #endregion
    }
}
