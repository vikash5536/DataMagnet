using MySql.Data.MySqlClient;
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
    /// Interaction logic for CustomPrompt.xaml
    /// </summary>
    public partial class EmailTemplate : Window
    {

        bool status;
        public static string connectionString = ConfigurationManager.ConnectionStrings["myCon"].ConnectionString;
        //This is  MySqlConnection here i have created the object and pass my connection string.  
        MySqlConnection MySqlConn = new MySqlConnection(connectionString);
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        
        /// <summary>
        /// Constructor of window
        /// </summary>
        public EmailTemplate()
        {
            InitializeComponent();
            Common.EmailTemplateList = Common.LoadEmailTemplate();
            if (Common.EmailTemplateList.Count > 0)
            {
                tbSubject.Text = Common.EmailTemplateList[0].EmailSubject;
                tbBody.Text = Common.EmailTemplateList[0].EmailBody;
            }
        }

        #region Private Methods
        /// <summary>
        /// This event will close the popup dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            if (tbSubject.Text != "" && tbBody.Text != "")
            {
                status = true;
                int result = Dalc.Dalc.AddEmailTemplate(tbSubject.Text,tbBody.Text);
                if (result == 0)
                {
                    MessageBox.Show("Email template added successfuly");
                }
                else if (result == 1)
                {
                    MessageBox.Show("Email template updated successfuly");
                }
                else
                {
                    MessageBox.Show("Error in Add Template.");
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Please Subject and Body for Email.");
            }
        }

        
        public bool ShowPopup()
        {
            this.ShowDialog();
            return status;
        }

        #endregion

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            status = false;
            this.Close();
        }
    }
}
