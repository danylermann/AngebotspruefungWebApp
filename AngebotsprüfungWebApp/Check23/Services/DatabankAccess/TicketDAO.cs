using Check23.Models;
using MySqlConnector;

namespace Check23.Services.DatabankAccess
{
    public class TicketDAO
    {
        public List<Ticket> GetAllTicketsByQuoteEvalId(int quoteEvalId, string connectionString)
        {
            List<Ticket> foundTickets = new List<Ticket>();
            string sqlStatement = "SELECT * FROM check23.ticket WHERE quote_evaluation_id = " + quoteEvalId;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundTickets.Add(new Ticket(
                            (int)reader[0],
                            (string)reader[1],
                            (string)reader[2],
                            (int)reader[3]
                        ));
                    }
                    connection.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
            return foundTickets;
        }

        public void InsertTicket(Ticket ticket, string connectionString)
        {
            string sqlStatement = "INSERT INTO check23.ticket " +
                "(number, type, quote_evaluation_id) " +
                "VALUES (@number, @type, @id)";
            using (MySqlConnection connection = new MySqlConnection(connectionString)) 
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@number", ticket.Number);
                command.Parameters.AddWithValue("@type", ticket.Type);
                command.Parameters.AddWithValue("@id", ticket.QuoteEvaluation_id);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }

        }

        public void UpdateTicket(Ticket ticket, string connectionString)
        {
            string sqlStatement = "Update check23.ticket " +
                "SET number = @number, " +
                "type = @type WHERE id = @id ";

            using (MySqlConnection connection = new MySqlConnection(connectionString)) 
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@number", ticket.Number);
                command.Parameters.AddWithValue("@type", ticket.Type);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                } 
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }

        public Ticket GetTicketById(int ticketId, string connectionString)
        {
            Ticket foundTicket = new Ticket();
            string sqlStatement = "SELECT * FROM check23.ticket WHERE id = " + ticketId;

            using (MySqlConnection connection = new MySqlConnection(connectionString)) 
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundTicket = new Ticket(
                            (int)reader[0],
                            (string)reader[1],
                            (string)reader[2],
                            (int)reader[3]
                            );
                    }
                    connection.Close();
                } 
                catch (Exception ex) { Console.WriteLine(ex.Message); }              
            }
            return foundTicket;
        }
    }
}
