using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TC2_Bases.Models;

namespace TC2_Bases.Pages.Add
{
    public class AddSpareModel : PageModel
    {
        // Attributes to populate information on the website and to get the user input and update the DB
        public SpareInfo sprInfo = new SpareInfo();

        public String errorMessage = "";
        public String successMessage = "";

        // Method to SEND information to the DB
        public void OnPost()
        {
            // Asign the data from the website input into an object/variables
            sprInfo.code = Request.Form["spare-code"];
            sprInfo.name = Request.Form["spare-name"];
            sprInfo.price = float.Parse(Request.Form["spare-price"]);

            if (sprInfo.code.Length == 0 || sprInfo.name.Length == 0 || sprInfo.price < 1)
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
                    String sqlInsert = "INSERT INTO Repuesto (codigo, nombre, precio) " +
                        "VALUES (@codigo, @nombre, @precio)";

                    using (MySqlCommand command = new MySqlCommand(sqlInsert, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@codigo", sprInfo.code);
                        command.Parameters.AddWithValue("@nombre", sprInfo.name);
                        command.Parameters.AddWithValue("@precio", sprInfo.price);

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

            successMessage = "La informacion se agrego con exito a la base de datos";
            return;
        }
    }
}
