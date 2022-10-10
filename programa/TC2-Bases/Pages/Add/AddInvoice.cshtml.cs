using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Dynamic;
using TC2_Bases.Models;

namespace TC2_Bases.Pages.Add
{
    public class AddInvoiceModel : PageModel
    {
        // List for the catalogs
        public List<String> repairsList = new List<String>();

        // Model to store the data
        public InvoiceInfo invcInfo = new InvoiceInfo();

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
                    String sqlSelectAllRepairConsec = "SELECT consecutivo FROM Reparacion;";

                    using (MySqlCommand command = new MySqlCommand(sqlSelectAllRepairConsec, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tempConsec;

                                tempConsec = "" + reader["consecutivo"];

                                // Add the object to the list
                                repairsList.Add(tempConsec);
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
            invcInfo.repairConsec = Request.Form["invoice-repair-consec"];

            // Verify that the necessary fileds have information
            if (invcInfo.consecutive.Length == 0 || invcInfo.year.Length == 0 ||
                invcInfo.name.Length == 0 || invcInfo.name.Length == 0)
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
            invcInfo.price = 0;

            successMessage = "La informacion se agrego con exito a la base de datos";
        }
    }
}
