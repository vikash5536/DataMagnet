using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMagnetApp
{
   public class BoardData
    {
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public double ProductWeight { get; set; }
        public DateTime EntryTime { get; set; }
        
        MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["myCon"].ConnectionString);  
        /// <summary>
        /// List of Authors
        /// </summary>
        /// <returns></returns>
        public DataSet LoadCollectionData()
        {
            DataSet ds = null;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                MySqlCommand cmd = new MySqlCommand("Select ProductNo,ProductName,ProductWeight,EntryTime from ProductInfo", conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                ds = new DataSet();
                adp.Fill(ds, "LoadDataBinding");
                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);      
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }

        public void InsertData(int i)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                string Query = "insert into ProductInfo (ProductNo,ProductName,ProductWeight,EntryTime) values('" + i.ToString() + "','Product','112','"+ DateTime.Now.ToString()+"');";
                MySqlCommand MyCommand2 = new MySqlCommand(Query, conn);
                MyCommand2.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }  
        }
    }

   //public class CollectionViewModel : ViewModelBase
   //{
   //    private ObservableCollection<BoardData> _contentList;
   //    public ObservableCollection<BoardData> ContentList
   //    {
   //        get { return _contentList; }
   //    }

   //    public CollectionViewModel()
   //    {
   //        _contentList = new ObservableCollection<BoardData>();
   //        _contentList.CollectionChanged += ContentCollectionChanged;
   //    }

   //    public void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
   //    {
   //        //This will get called when the collection is changed
   //    }
   //}
}
