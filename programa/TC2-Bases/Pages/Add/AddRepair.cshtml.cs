using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using TC2_Bases.Models;

namespace TC2_Bases.Pages.Add
{
    public class AddRepairModel : PageModel
    {
        // Attributes to populate information on the website and to get the user input and update the DB
        // List of objects for the catalogs
        public List<ClientInfo> clientsList = new List<ClientInfo>();
        public List<ShopBasic> shopsList = new List<ShopBasic>();
        public List<String> carsList = new List<String>(); 
        public List<MechanicInfo> mechsList = new List<MechanicInfo>();

        // Strings to create the error or success messages
        public String errorMessage = "";
        public String successMessage = "";

        // Model to store the data
        public RepairInfo rprInfo = new RepairInfo();

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
                    String sqlSelectIdNameClient = "SELECT cedula, CONCAT(`nombre`, ' ', `apellido1`, ' ', `apellido2`) AS nombreCompleto FROM Cliente;";
                    String sqlSelectIdNameShop = "SELECT cedulaJuridica, nombre FROM TallerMecanico;";
                    String sqlSelectIdCar = "SELECT placa FROM Vehiculo;";
                    String sqlSelectIdNameMechanic = "SELECT cedula, CONCAT(`nombre`, ' ', `apellido1`, ' ', `apellido2`) AS nombreCompleto FROM Mecanico;";

                    // Get Client info
                    using (MySqlCommand command = new MySqlCommand(sqlSelectIdNameClient, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            List<ClientInfo> tempClientsList = new List<ClientInfo>();
                            while (reader.Read())
                            {
                                ClientInfo tempClient = new ClientInfo();
                                tempClient.id = "" + reader["cedula"];
                                tempClient.name = "" + reader["nombreCompleto"];

                                // Add the object to the list
                                clientsList.Add(tempClient);
                            }
                            reader.Close();
                        }
                    }

                    // Get Shop info
                    using (MySqlCommand command = new MySqlCommand(sqlSelectIdNameShop, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            List<ShopBasic> tempShopsList = new List<ShopBasic>();
                            while (reader.Read())
                            {
                                ShopBasic tempSop = new ShopBasic();
                                tempSop.id = "" + reader["cedulaJuridica"];
                                tempSop.name = "" + reader["nombre"];

                                // Add the object to the list
                                shopsList.Add(tempSop);
                            }
                            reader.Close();
                        }
                    }

                    // Get Car info
                    using (MySqlCommand command = new MySqlCommand(sqlSelectIdCar, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            List<String> tempCarsList = new List<String>();
                            while (reader.Read())
                            {
                                string tempPlate;
                                tempPlate = "" + reader["placa"];

                                // Add the object to the list
                                carsList.Add(tempPlate);
                            }
                            reader.Close();
                        }
                    }

                    // Get Mechanic info
                    using (MySqlCommand command = new MySqlCommand(sqlSelectIdNameMechanic, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            List<MechanicInfo> tempMechList = new List<MechanicInfo>();
                            while (reader.Read())
                            {
                                MechanicInfo tempMech = new MechanicInfo();
                                tempMech.id = "" + reader["cedula"];
                                tempMech.name = "" + reader["nombreCompleto"];

                                // Add the object to the list
                                mechsList.Add(tempMech);
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
            rprInfo.consecutive = Request.Form["repair-cons"];
            rprInfo.clientId = Request.Form["repair-client"];
            rprInfo.shopId = Request.Form["repair-shop"];
            rprInfo.carLicensePlate = Request.Form["repair-car"];
            rprInfo.repairLeader = Request.Form["repair-mech"];
            rprInfo.repairDate = Request.Form["repair-date"];

            // Verify that the necessary fileds have information
            if (rprInfo.consecutive.Length == 0 || rprInfo.clientId.Length == 0 ||
                rprInfo.shopId.Length == 0 || rprInfo.carLicensePlate.Length == 0 || rprInfo.repairLeader.Length == 0)
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

                    // Query to send/edit the data to the DB
                    String sqlInsert = "INSERT INTO Reparacion (consecutivo, cliente, tallerMecanico, vehiculo, mecanicoPrincipal, fechaReparacion) " +
                        "VALUES (@consecutivo, @cliente, @tallerMecanico, @vehiculo, @mecanicoPrincipal, @fechaReparacion); " +
                        "INSERT INTO MecanicoXReparacion (consecutivRepa, cedulaMecanico, mecanicoRol)" +
                        "VALUES (@consecutivo, @mecanicoPrincipal, @mecanicoRol)";

                    using (MySqlCommand command = new MySqlCommand(sqlInsert, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@consecutivo", rprInfo.consecutive);
                        command.Parameters.AddWithValue("@cliente", rprInfo.clientId);
                        command.Parameters.AddWithValue("@tallerMecanico", rprInfo.shopId);
                        command.Parameters.AddWithValue("@vehiculo", rprInfo.carLicensePlate);
                        command.Parameters.AddWithValue("@mecanicoPrincipal", rprInfo.repairLeader);
                        command.Parameters.AddWithValue("@fechaReparacion", rprInfo.repairDate);
                        command.Parameters.AddWithValue("@mecanicoRol", "Lider");

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
            rprInfo.consecutive = "";
            rprInfo.clientId = "";
            rprInfo.shopId = "";
            rprInfo.carLicensePlate = "";
            rprInfo.repairLeader = "";
            rprInfo.repairDate = "";

            successMessage = "La informacion se agrego con exito a la base de datos";
        }
    }
}
