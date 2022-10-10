using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TC2_Bases.Models;

namespace TC2_Bases.Pages.Show
{
    public class ShowRepairsModel : PageModel
    {
        // create a list of the info that we'll received from the database
        public List<RepairInfo> listReps = new List<RepairInfo>();

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
                    // Query to get the Repairs that don't have invoice yet
                    String sqlSelectAllRepairsInfo = "SELECT *\r\nFROM basicInfoXReparacion\r\nWHERE consecutivo " +
                        "NOT IN (\r\nSELECT DISTINCT consecutivoReparacion\r\nFROM Factura);";

                    // Get the Repair Consecutive
                    using (MySqlCommand command = new MySqlCommand(sqlSelectAllRepairsInfo, connection))
                    {
                        // Execute the query
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            String tempString;
                            DateTime tempDateTime;

                            while (reader.Read())
                            {
                                RepairInfo repairs = new RepairInfo();

                                repairs.consecutive = "" + reader["consecutivo"];
                                repairs.clientId = "" + reader["cliente"];
                                repairs.carLicensePlate = "" + reader["vehiculo"];
                                repairs.shopId = "" + reader["taller"];
                                repairs.repairLeader = "" + reader["mecanicoPrincipal"];
                                tempString = "" + reader["fecha"];

                                tempDateTime = DateTime.Parse(tempString);
                                repairs.repairDate = tempDateTime.ToShortDateString();

                                // Add the object to the list
                                listReps.Add(repairs);
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
    }
}
