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
        string connectionString = "Data Source=(localdb)\\DBTest;Integrated Security=True";
        [HttpGet(Name = "GetCliente")]
        public IEnumerable<Clienti> GetClienti(int numeroClienti)
        {
            List<Clienti> clientiList = new List<Clienti>();
            try
            {
                string query = "SELECT TOP " + numeroClienti + " [Name]\r\n      ,[Surname]      \r\n  FROM [master].[dbo].[clienti] clienti\r\n  WHERE clienti.Stato_Record = 1\r\n  ORDER BY Eta";

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

        [HttpPut(Name = "InsertNewCliente")]
        public IActionResult InsertNewCliente(string nome, string cognome, int eta, string? pIva, int codCapelli, bool statoCliente)
        {
            try
            {
                string query = "INSERT INTO [dbo].[clienti] \r\nVALUES (@nome, @cognome, @eta, @pIva, @codCapelli, @data, @statoCliente);";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@cognome", cognome);
                    cmd.Parameters.AddWithValue("@eta", eta);
                    if (pIva != null)
                        cmd.Parameters.AddWithValue("@pIva", pIva);
                    else
                        cmd.Parameters.AddWithValue("@pIva", "none");
                    cmd.Parameters.AddWithValue("@codCapelli", codCapelli);
                    DateTime date = DateTime.UtcNow;
                    cmd.Parameters.AddWithValue("@data", date);
                    cmd.Parameters.AddWithValue("@statoCliente", statoCliente);
                    int isCreated = cmd.ExecuteNonQuery();
                    if (isCreated > 0) 
                    {
                        return Ok();
                    }
                    else 
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpDelete(Name = "DeleteClienteById")]
        public IActionResult DeleteClienteById(int clienteId)
        {
            try
            {
                string query = "DELETE FROM [master].[dbo].[clienti] WHERE ID = @ID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ID", clienteId);
                    int isPresent = command.ExecuteNonQuery();

                    if (isPresent > 0)
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }

                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
