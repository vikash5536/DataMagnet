using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DataMagnetApp.ViewModel
{
    public class CBViewModel : ViewModelBase
    {

        private Dictionary<string, object> _items;
        private Dictionary<string, object> _selectedItems;


        public Dictionary<string, object> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }

        public Dictionary<string, object> SelectedItems
        {
            get
            {
                return _selectedItems;
            }
            set
            {
                _selectedItems = value;
                NotifyPropertyChanged("SelectedItems");
            }
        }


        public CBViewModel()
        {
            Items = new Dictionary<string, object>();
            Items.Add("Server1", "1");
            Items.Add("Server2", "2");
            Items.Add("Server3", "3");
            Items.Add("Server4", "4");

            //SelectedItems = new Dictionary<string, object>();
            //SelectedItems.Add("Chennai", "MAS");
            //SelectedItems.Add("Trichy", "TPJ");
        }

    }
    public class EmailTemplateViewModel
    {
        public string Id { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
    }
    public class EMailViewModel
    {
        public int Id { get; set; }
        public string EmailName { get; set; }
        public string EmailFrom { get; set; }
        public string EmailPassword { get; set; }
        public string SMTPAddress { get; set; }
        public int SMTPPort { get; set; }
        public bool EnableSSL { get; set; }
        public string EmailTo1 { get; set; }
        public string EmailTo2 { get; set; }
        public string EmailTo3 { get; set; }
        public string EmailTo4 { get; set; }
        public string EmailTo5 { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public DateTime EntryDate { get; set; }
    }
    public class EMailHistoryViewModel
    {
        public string MachineName { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailTo1 { get; set; }
        public string EmailTo2 { get; set; }
        public string EmailTo3 { get; set; }
        public string EmailTo4 { get; set; }
        public string EmailTo5 { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EntryDateGroup { get; set; }
        public DateTime EntryDate { get; set; }
    }
    public class MachinesViewModel
    {
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public string MachineLocation { get; set; }
        public string ProductName { get; set; }
        public string SupervisorName { get; set; }
        public string EmailTo { get; set; }
        public Common.ConnectionType ConnectionType { get; set; }
        public string MachineIP { get; set; }
        public int MachinePort { get; set; }
        public string COMPortName { get; set; }
        public string COMPortParity { get; set; }
        public string COMPortBoadRate { get; set; }
    }
    public class LogInViewModel
    {
        public static string UserType { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
    }
    public enum UserTypeEnum : int { Admin = 0, User = 1 };
    public enum CommandTypeEnum : int { Insert = 1, Update = 2, Delete = 3 };
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public UserTypeEnum UserType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class MachineDataViewModel
    {
        public string MachineName { get; set; }
        public string MachineData { get; set; }
        public DateTime EntryDate { get; set; }
    }
    public class ErrorMessage
    {

        public int ErrorId { get; set; }
        public string ErrorDescription { get; set; }
    }
}
