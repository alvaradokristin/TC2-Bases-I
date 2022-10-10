using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TC2_Bases.Models;

namespace TC2_Bases.Pages.Add
{
    public class AddMechanicModel : PageModel
    {
        // Attributes to populate information on the website and to get the user input and update the DB
        public MechanicInfo mchInfo = new MechanicInfo();

        public String errorMessage = "";
        public String successMessage = "";
        
        // Method to SEND information to the DB
        public void OnPost()
        {
            // Asign the data from the website input into an object/variables
            mchInfo.id = Request.Form["mechanic-id"];
            mchInfo.name = Request.Form["mechanic-name"];
            mchInfo.flastname = Request.Form["mechanic-flastname"];
            mchInfo.slastname = Request.Form["mechanic-slastname"];

            // Verify that the necessary fileds have information
            if (mchInfo.id.Length == 0 || mchInfo.name.Length == 0 ||
                mchInfo.flastname.Length == 0 || mchInfo.slastname.Length == 0)
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
                    String sqlInsert = "INSERT INTO Mecanico (cedula, nombre, apellido1, apellido2) " +
                        "VALUES (@cedula, @nombre, @apellido1, @apellido2)";

                    using (MySqlCommand command = new MySqlCommand(sqlInsert, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@cedula", mchInfo.id);
                        command.Parameters.AddWithValue("@nombre", mchInfo.name);
                        command.Parameters.AddWithValue("@apellido1", mchInfo.flastname);
                        command.Parameters.AddWithValue("@apellido2", mchInfo.slastname);

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
            mchInfo.id = "";
            mchInfo.name = "";
            mchInfo.flastname = "";
            mchInfo.slastname = "";

            successMessage = "La informacion se agrego con exito a la base de datos";
        }
    }
}
