using Microsoft.Data.SqlClient;

namespace PCNW.Data.ADO
{
    public class MyConnection
    {
        public SqlConnection Mycon = new SqlConnection();
        public SqlCommand cmd;
        public SqlDataAdapter adp;
        public SqlCommandBuilder cb;
        public SqlTransaction trans;
        public string server;
        public string database;
        public string uid;
        public string password;


        //Constructor
        public MyConnection(string _conString)
        {
            Initialize(_conString);
        }

        //Initialize values
        private void Initialize(string ConString)
        {
            //Mycon.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            Mycon.ConnectionString = new SqlConnection(ConString).ConnectionString;

            cmd = new SqlCommand("", Mycon);
            adp = new SqlDataAdapter("", Mycon);
            cb = new SqlCommandBuilder(adp);
            adp.InsertCommand = new SqlCommand("", Mycon);
            adp.UpdateCommand = new SqlCommand("", Mycon);
            adp.SelectCommand = new SqlCommand("", Mycon);
            adp.DeleteCommand = new SqlCommand("", Mycon);
            //cmd.CommandTimeout = 20000000;
        }

        //open connection to database
        public void Open()
        {
            if (Mycon.State == System.Data.ConnectionState.Open)
            {
                Mycon.Close();
            }
            Mycon.Open();
        }

        //Close connection
        public void Close()
        {
            if (Mycon.State == System.Data.ConnectionState.Open)
            {
                Mycon.Close();
            }
        }
    }
}