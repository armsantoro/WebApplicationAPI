using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteResultController : ControllerBase
    {
        [HttpGet(Name = "GetCliente")]
        public IEnumerable<Clienti> GetClienti(int numeroClienti)
        {
            List<Clienti> clientiList = new List<Clienti>();
            try
            {
                string query = "SELECT TOP " + numeroClienti + " [Name]\r\n      ,[Surname]      \r\n  FROM [master].[dbo].[clienti] clienti\r\n  WHERE clienti.Stato_Record = 1\r\n  ORDER BY Eta";
                string connectionString = "Data Source=(localdb)\\DBTest;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Clienti cliente = new Clienti();
                                cliente.Nome = reader.GetString(0);
                                cliente.Cognome = reader.GetString(1);
                                clientiList.Add(cliente);
                            }
                        }
                    }

                }
                return clientiList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        [HttpPost(Name ="UpdateClienteById")]
        public bool UpdateClienteById (int clienteId)
        {
            bool rtrn = false;
            try
            {

                string connectionString = "Data Source=(localdb)\\DBTest;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("dbo.updateClienti", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", clienteId);
                    SqlParameter retValue = new SqlParameter("@result", System.Data.SqlDbType.Bit);
                    retValue.Direction = System.Data.ParameterDirection.ReturnValue;
                    command.Parameters.Add(retValue);
                    command.ExecuteNonQuery();
                    rtrn = Convert.ToBoolean(command.Parameters["@result"].Value);
                }
                return rtrn;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
