using Microsoft.AspNetCore.Mvc.RazorPages;
using TC2_Bases.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Linq;

namespace TC2_Bases.Pages.Add
{
    public class AddCarModel : PageModel
    {
        // List for the catalogs
        public List<String> brandsList = new List<String>();
        public List<ModelInfo> modelsList = new List<ModelInfo>();
        public List<String> colorList = new List<String>();

        // Strings to create the error or success messages
        public String errorMessage = "";
        public String successMessage = "";

        // Method to GET information from the DB
        public void OnGet()
        {
            try
            {
                // Use the connection String to connect the web site to the DB
                var connString = new ConnStr();
                String connectStr = connString.ConnectionString;

                using (MySqlConnection connection = new MySqlConnection(connectStr))
                {
                    connection.Open();

                    // Queries to be use
                    String sqlSelectAll = "SELECT * FROM Modelo;";

                    using (MySqlCommand command = new MySqlCommand(sqlSelectAll, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ModelInfo modelInf = new ModelInfo();

                                modelInf.brand = "" + reader["marca"];
                                modelInf.model = "" + reader["nombre"];

                                // Add the object to the list
                                modelsList.Add(modelInf);
                            }
                            reader.Close();
                        }
                    }
                }

                // Create a list of brands
                foreach(var item in modelsList) {
                    // If the value haven't been added before
                    if (!brandsList.Contains(item.brand))
                    {
                        brandsList.Add(item.brand);
                    }
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.ToString());
            }
        }
    }
}
