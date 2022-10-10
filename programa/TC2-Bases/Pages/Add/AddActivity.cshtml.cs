using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TC2_Bases.Models;

namespace TC2_Bases.Pages.Add
{
    public class AddActivityModel : PageModel
    {
        // Attributes to populate information on the website and to get the user input and update the DB
        public SpareInfo sprInfo = new SpareInfo();

        // List for the catalogs
        public List<String> repairsList = new List<String>();

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
            sprInfo.code = Request.Form["labor-code"];
            sprInfo.name = Request.Form["labor-name"];
            sprInfo.price = float.Parse(Request.Form["labor-price"]);
            //string repairConsec = Request.Form["labor-repair-consec"];

            if (sprInfo.code.Length == 0 || sprInfo.name.Length == 0 || sprInfo.price < 1 /*|| repairConsec.Length == 0*/)
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
                    String sqlInsert = "INSERT INTO ActividadManoObra (codigo, nombre, precio) " +
                        "VALUES (@codigo, @nombre, @precio); " /*+
                        "INSERT INTO ManoObraXReparacion (consecutivRepa, codigoActividadManoObra) " +
                        "VALUES (@consecutivRepa, @codigo)"*/;

                    using (MySqlCommand command = new MySqlCommand(sqlInsert, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@codigo", sprInfo.code);
                        command.Parameters.AddWithValue("@nombre", sprInfo.name);
                        command.Parameters.AddWithValue("@precio", sprInfo.price);
                        //command.Parameters.AddWithValue("@consecutivRepa", repairConsec);

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
            sprInfo.code = "";
            sprInfo.name = "";
            sprInfo.price = 0;
            //repairConsec = "";

            successMessage = "La informacion se agrego con exito a la base de datos";
        }
    }
}
