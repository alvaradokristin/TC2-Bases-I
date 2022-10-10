namespace TC2_Bases.Models
{
    public class ConnStr
    {
        // This will be the connection to the database, copy and paste from the connection on the Server Explorer
        private String connectionString = "Server=localhost;User=root;Database=tallermecanicot2;Port=3306;Password=XXX";
        public String ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }
    }
}
