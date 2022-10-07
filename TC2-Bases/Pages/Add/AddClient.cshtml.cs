using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TC2_Bases.Models;

namespace TC2_Bases.Pages.Add
{
    public class AddClientModel : PageModel
    {
        // Attributes to populate information on the website and to get the user input and update the DB
        public ClientInfo clntInfo = new ClientInfo();
        public ClientPhoneInfo clntPhnInfo = new ClientPhoneInfo();

        public String errorMessage = "";
        public String successMessage = "";

        // Method to SEND information to the DB
        public void OnPost()
        {
            // Asign the data from the website input into an object/variables
            clntInfo.id = Request.Form["client-id"];
            clntInfo.name = Request.Form["client-name"];
            clntInfo.flastname = Request.Form["client-flastname"];
            clntInfo.slastname = Request.Form["client-slastname"];
            clntInfo.address = Request.Form["client-address"];
            clntPhnInfo.clientId = Request.Form["client-id"];
            clntPhnInfo.phoneNumber = Request.Form["client-phone"];

            // Verify that the necessary fileds have information
            if (clntInfo.id.Length == 0 || clntInfo.name.Length == 0 ||
                clntInfo.flastname.Length == 0 || clntInfo.slastname.Length == 0 || clntInfo.address.Length == 0 ||
                clntPhnInfo.phoneNumber.Length == 0)
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
                    String sqlInsert = "INSERT INTO Cliente (cedula, nombre, apellido1, apellido2, direccion) \r\nVALUES \r\n" +
                        "(@id, @name, @flastname, @slastname, @address);\n\t " +
                        "INSERT INTO TelefonoClientes (cedulaCliente, telefonoCliente) VALUES\n\t " +
                        "(@cId, @phoneNumber);";

                    using (MySqlCommand command = new MySqlCommand(sqlInsert, connection))
                    {
                        // Add the data from the input to the query parameters
                        command.Parameters.AddWithValue("@id", clntInfo.id);
                        command.Parameters.AddWithValue("@name", clntInfo.name);
                        command.Parameters.AddWithValue("@flastname", clntInfo.flastname);
                        command.Parameters.AddWithValue("@slastname", clntInfo.slastname);
                        command.Parameters.AddWithValue("@address", clntInfo.address);
                        command.Parameters.AddWithValue("@cId", clntPhnInfo.clientId);
                        command.Parameters.AddWithValue("@phoneNumber", clntPhnInfo.phoneNumber);

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
            clntInfo.id = "";
            clntInfo.name = "";
            clntInfo.flastname = "";
            clntInfo.slastname = "";
            clntInfo.address = "";
            clntPhnInfo.clientId = "";
            clntPhnInfo.phoneNumber = "";

            successMessage = "La informacion se agrego con exito a la base de datos";
        }
    }
}
