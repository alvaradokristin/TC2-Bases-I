using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Dynamic;
using TC2_Bases.Models;

namespace TC2_Bases.Pages.Associate
{
    public class AsoMechanicModel : PageModel
    {
        // Model to store the data
        public MxR mechXRep = new MxR();

        // List for the catalogs
        public List<String> consecsList = new List<String>();
        public List<ShopBasic> mechsList = new List<ShopBasic>();
        public List<String> rolesList = new List<String>();

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
                    String sqlSelectIdNameMechanic = "SELECT cedula, CONCAT(`nombre`, ' ', `apellido1`, ' ', `apellido2`) AS nombreCompleto FROM Mecanico;";
                    String sqlSelectAllRoles = "SELECT * FROM Rol;";

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

                    // Get the Roles
                    using (MySqlCommand command = new MySqlCommand(sqlSelectAllRoles, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string temp;

                                temp = "" + reader["nombre"];

                                // Add the object to the list
                                rolesList.Add(temp);
                            }
                            reader.Close();
                        }
                    }

                    // Get the Mechanics
                    using (MySqlCommand command = new MySqlCommand(sqlSelectIdNameMechanic, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ShopBasic temp = new ShopBasic();

                                temp.id = "" + reader["cedula"];
                                temp.name = "" + reader["nombreCompleto"];

                                // Add the object to the list
                                mechsList.Add(temp);
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
            mechXRep.consecutive = Request.Form["mechxrep-repair-consec"];
            mechXRep.mechanicId = Request.Form["mechxrep-mech-id"];
            mechXRep.mechRole = Request.Form["mechxrep-mech-role"];

            // Verify that the necessary fileds have information
            if (mechXRep.consecutive.Length == 0 || mechXRep.mechanicId.Length == 0 ||
                mechXRep.mechRole.Length == 0)
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
                    String sqlInsert = "INSERT INTO MecanicoXReparacion (consecutivRepa, cedulaMecanico, mecanicoRol)" +
                        "VALUES (@consecutivo, @cedulaMecanico, @mecanicoRol)";

                    using (MySqlCommand command = new MySqlCommand(sqlInsert, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@consecutivo", mechXRep.consecutive);
                        command.Parameters.AddWithValue("@cedulaMecanico", mechXRep.mechanicId);
                        command.Parameters.AddWithValue("@mecanicoRol", mechXRep.mechRole);

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
            mechXRep.consecutive = "";
            mechXRep.mechanicId = "" ;
            mechXRep.mechRole = "";

            successMessage = "La informacion se agrego con exito a la base de datos";

            Response.Redirect("/Associate/AsoMechanic");
        }
    }
}
