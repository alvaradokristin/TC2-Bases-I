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

        // Model to store the data
        public CarInfo carInfo = new CarInfo();

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
                    String sqlSelectAllModel = "SELECT * FROM Modelo;";
                    String sqlSelectAllColor = "SELECT * FROM Color;";

                    using (MySqlCommand command = new MySqlCommand(sqlSelectAllColor, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string colorName;

                                colorName = "" + reader["nombre"];

                                // Add the object to the list
                                colorList.Add(colorName);
                            }
                            reader.Close();
                        }
                    }

                    using (MySqlCommand command = new MySqlCommand(sqlSelectAllModel, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ModelInfo modelInf = new ModelInfo();

                                modelInf.brand = "" + reader["marca"];
                                modelInf.model = "" + reader["nombre"];

                                // Add the string to the list
                                modelsList.Add(modelInf);
                            }
                            reader.Close();
                        }
                    }
                }

                // Create a list of brands
                foreach(var item in modelsList) {
                    // If the value haven't been added before
                    if (brandsList.Count < 1 || (!brandsList.Contains(item.brand)))
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

        // Method to SEND information to the DB
        public void OnPost()
        {
            // Asign the data from the website input into an object/variables
            carInfo.licensePlate = Request.Form["car-license-plate"];
            carInfo.brand = Request.Form["car-brand"];
            carInfo.model = Request.Form["car-model"];
            carInfo.year = Request.Form["car-year"];
            carInfo.color = Request.Form["car-color"];

            // Verify that the necessary fileds have information
            if (carInfo.licensePlate.Length == 0 || carInfo.brand.Length == 0 ||
                carInfo.model.Length == 0 || carInfo.year.Length == 0 || carInfo.color.Length == 0)
            {
                errorMessage = "Todos los campos deben tener informacion";
                return;
            }

            // Save the new data
            try
            {
                var connString = new ConnStr();
                String connectStr = connString.ConnectionString;

                using (MySqlConnection connection = new MySqlConnection(connectStr))
                {
                    connection.Open();

                    // Query to add the data to the DB
                    String sqlInsert = "INSERT INTO Vehiculo (placa, marca, modelo, anno, color) " +
                        "VALUES (@placa, @marca, @modelo, @anno, @color)";

                    using (MySqlCommand command = new MySqlCommand(sqlInsert, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@placa", carInfo.licensePlate);
                        command.Parameters.AddWithValue("@marca", carInfo.brand);
                        command.Parameters.AddWithValue("@modelo", carInfo.model);
                        command.Parameters.AddWithValue("@anno", carInfo.year);
                        command.Parameters.AddWithValue("@color", carInfo.color);

                        // Execute the query
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            // Clear the fileds/attributes data
            carInfo.licensePlate = "";
            carInfo.brand = "";
            carInfo.model = "";
            carInfo.year = "";
            carInfo.color = "";

            successMessage = "La informacion se agrego con exito a la base de datos";
        }
    }
}
