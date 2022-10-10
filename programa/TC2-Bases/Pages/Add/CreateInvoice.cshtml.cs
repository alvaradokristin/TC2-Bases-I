using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Dynamic;
using TC2_Bases.Models;

namespace TC2_Bases.Pages.Add
{
    public class CreateInvoiceModel : PageModel
    {
        // Model to store the data received from the database
        public InvoiceCreationBasic invcBasic = new InvoiceCreationBasic();

        // Model to store the data
        public InvoiceInfo invcInfo = new InvoiceInfo();

        // List for the tables
        public List<MxR> mechsList = new List<MxR>();
        public List<SxR> sparesList = new List<SxR>();
        public List<SxR> activitiesList = new List<SxR>();

        // Strings to create the error or success messages
        public String errorMessage = "";
        public String successMessage = "";

        // Method to GET information from the DB
        public void OnGet()
        {
            try
            {
                // Get the code from the URL query
                String consecutivRepa = Request.Query["code"];
                //Debug.WriteLine(consecutivRepa);

                // Use the connection String to connect the web site to the DB
                var connString = new ConnStr();
                String connectStr = connString.ConnectionString;

                using (MySqlConnection connection = new MySqlConnection(connectStr))
                {
                    connection.Open();

                    // Queries to be use
                    String sqlSelectAllBasicInvoice = "SELECT * FROM facturaBasicXReparacion WHERE consecutivo = @consecutivRepa;";
                    String sqlSelectAllMechs = "SELECT * FROM mecanicosInfoXReparacion WHERE reparacion = @consecutivRepa;";
                    String sqlSelectAllSpares = "SELECT * FROM repuestosInfoXReparacion WHERE reparacion = @consecutivRepa;";
                    String sqlSelectAllActivities = "SELECT * FROM manoObraInfoXReparacion WHERE reparacion = @consecutivRepa;";

                    // Get all the basic data for the invoice
                    using (MySqlCommand command = new MySqlCommand(sqlSelectAllBasicInvoice, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@consecutivRepa", consecutivRepa);

                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                invcBasic.clientId = "" + reader["cedula"];
                                invcBasic.clientName = "" + reader["nombreCompletoC"];
                                invcBasic.clientAddress = "" + reader["direccion"];
                                invcBasic.carLicensePlate = "" + reader["placa"];
                                invcBasic.carBrand = "" + reader["marca"];
                                invcBasic.carModel = "" + reader["modelo"];
                                invcBasic.carYear = "" + reader["anno"];
                                invcBasic.carColor = "" + reader["color"];
                                invcBasic.laborSubtotal = "" + reader["precioManoObra"];
                                invcBasic.repsSubtotal = "" + reader["precioRepuestos"];
                                invcBasic.subtotal = "" + reader["subtotal"];
                                invcBasic.total = "" + reader["total"];
                            }
                            reader.Close();
                        }
                    }

                    // Get all Mechanics
                    using (MySqlCommand command = new MySqlCommand(sqlSelectAllMechs, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@consecutivRepa", consecutivRepa);

                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MxR mechTemp = new MxR();

                                mechTemp.mechanicId = "" + reader["cedula"];
                                mechTemp.consecutive = "" + reader["nombreCompletoM"];
                                mechTemp.mechRole = "" + reader["mecanicoRol"];

                                // Add the object to the list
                                mechsList.Add(mechTemp);
                            }
                            reader.Close();
                        }
                    }

                    // Get all Spares
                    using (MySqlCommand command = new MySqlCommand(sqlSelectAllSpares, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@consecutivRepa", consecutivRepa);

                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SxR temp = new SxR();

                                temp.code = "" + reader["codigo"];
                                temp.name = "" + reader["nombre"];
                                temp.quantity = "" + reader["cantidad"];
                                temp.price = "" + reader["precio"];

                                // Add the object to the list
                                sparesList.Add(temp);
                            }
                            reader.Close();
                        }
                    }

                    // Get all Activities
                    using (MySqlCommand command = new MySqlCommand(sqlSelectAllActivities, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@consecutivRepa", consecutivRepa);

                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SxR temp = new SxR();

                                temp.code = "" + reader["codigo"];
                                temp.name = "" + reader["nombre"];
                                temp.price = "" + reader["precio"];

                                // Add the object to the list
                                activitiesList.Add(temp);
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
            invcInfo.consecutive = Request.Form["invoice-consec"];
            invcInfo.year = Request.Form["invoice-year"];
            invcInfo.name = Request.Form["invoice-name"];
            invcInfo.repairConsec = Request.Query["code"];
            //invcInfo.price = float.Parse(Request.Form["invoice-total"]);

            // Verify that the necessary fileds have information
            if (invcInfo.consecutive.Length == 0 || invcInfo.year.Length == 0 ||
                invcInfo.name.Length == 0 || invcInfo.repairConsec.Length == 0)
            {
                errorMessage = "Todos los campos deben tener informacion";
                return;
            }

            // Save the new data and get the price
            try
            {
                var connString = new ConnStr();
                String connectStr = connString.ConnectionString;

                using (MySqlConnection connection = new MySqlConnection(connectStr))
                {
                    connection.Open();

                    // Queries to be used
                    String sqlInsert = "INSERT INTO Factura (consecutivo, anno, nombre, precio, consecutivoReparacion) " +
                        "VALUES (@consecutivo, @anno, @nombre, @precio, @consecutivoReparacion)";
                    String sqlSelectTotal = "SELECT total FROM preciosFactura WHERE consecutivo = @consecutivo;";

                    // Get the total of this repair
                    using (MySqlCommand command = new MySqlCommand(sqlSelectTotal, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@consecutivo", invcInfo.repairConsec);

                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                invcInfo.price = float.Parse("" + reader["total"]);
                            }
                            reader.Close();
                        }
                    }

                    // send the data to the DB
                    using (MySqlCommand command = new MySqlCommand(sqlInsert, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@consecutivo", invcInfo.consecutive);
                        command.Parameters.AddWithValue("@anno", invcInfo.year);
                        command.Parameters.AddWithValue("@nombre", invcInfo.name);
                        command.Parameters.AddWithValue("@precio", invcInfo.price);
                        command.Parameters.AddWithValue("@consecutivoReparacion", invcInfo.repairConsec);

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
            invcInfo.consecutive = "";
            invcInfo.year = "";
            invcInfo.name = "";
            invcInfo.repairConsec = "";

            successMessage = "La informacion se agrego con exito a la base de datos";
        }
    }
}
