using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TC2_Bases.Models;

namespace TC2_Bases.Pages.Associate
{
    public class AsoSpareModel : PageModel
    {
        // Model to store the data
        SpareXRep sprXRep = new SpareXRep();

        // List for the catalogs
        public List<String> consecsList = new List<String>();
        public List<ShopBasic> sparesList = new List<ShopBasic>();

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
                    String sqlSelectAllRepairCons = "SELECT consecutivo FROM Reparacion;";
                    String sqlSelectIdSpare = "SELECT codigo, nombre FROM Repuesto;";

                    // Get the Repair Consecutive
                    using (MySqlCommand command = new MySqlCommand(sqlSelectAllRepairCons, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string temp;

                                temp = "" + reader["consecutivo"];

                                // Add the object to the list
                                consecsList.Add(temp);
                            }
                            reader.Close();
                        }
                    }

                    // Get the Mechanics
                    using (MySqlCommand command = new MySqlCommand(sqlSelectIdSpare, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ShopBasic temp = new ShopBasic();

                                temp.id = "" + reader["codigo"];
                                temp.name = "" + reader["nombre"];

                                // Add the object to the list
                                sparesList.Add(temp);
                            }
                            reader.Close();
                        }
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
            sprXRep.consecutiveRep = Request.Form["sparexrep-repair-consec"];
            sprXRep.code = Request.Form["sparexrep-spare-code"];
            sprXRep.quantity = int.Parse(Request.Form["sparexrep-spare-quantity"]);

            // Verify that the necessary fileds have information
            if (sprXRep.consecutiveRep.Length == 0 || sprXRep.code.Length == 0 ||
                sprXRep.quantity < 1)
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
                    String sqlInsert = "INSERT INTO RepuestoXReparacion (consecutivRepa, codigoRepuesto, cantidad)" +
                        "VALUES (@consecutivRepa, @codigoRepuesto, @cantidad)";

                    using (MySqlCommand command = new MySqlCommand(sqlInsert, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@consecutivRepa", sprXRep.consecutiveRep);
                        command.Parameters.AddWithValue("@codigoRepuesto", sprXRep.code);
                        command.Parameters.AddWithValue("@cantidad", sprXRep.quantity);

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
            sprXRep.consecutiveRep = "";
            sprXRep.code = "";
            sprXRep.quantity = 0;

            successMessage = "La informacion se agrego con exito a la base de datos";

            Response.Redirect("/Associate/AsoSpare");
        }
    }
}
