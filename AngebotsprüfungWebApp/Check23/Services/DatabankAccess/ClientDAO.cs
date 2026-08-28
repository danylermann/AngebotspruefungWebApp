using Check23.Models;
using MySqlConnector;
using NuGet.Protocol.Plugins;

namespace Check23.Services.DatabankAccess
{
    public class ClientDAO
    {
        public List<Client> GetAllClients(string connectionString)
        {
            List<Client> foundClients = new List<Client>();

            string splStatement = "SELECT * FROM check23.client";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(splStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundClients.Add(new Client
                            (
                               (int)reader[0],
                               (string)reader[1],
                               reader[2] as string
                            ));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return foundClients;
        }

        public Client GetClientById(int? clientId, string connectionString)
        {
            Client foundClient = new Client();
            string sqlStatement = "SELECT * FROM check23.client " +
                "WHERE id = " + clientId;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundClient = new Client
                            (
                                (int)reader[0],
                                (string)reader[1],
                                reader[2] as string
                            );
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return foundClient;
        }

        public int InsertClient(Client client, string connectionString)
        {
            int newId = -1;
            string sqlStatement = "INSERT INTO check23.client " +
                "(name, location) " +
                "VALUES (@name, @location)";

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", client.Name);
                if (client.Location == null || client.Location == string.Empty)
                {
                    command.Parameters.AddWithValue("@location", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@location", client.Location);
                }
                try
                {
                    connection.Open();
                    command.ExecuteScalar();
                    MySqlCommand getLastId = new MySqlCommand("SELECT LAST_INSERT_ID()", connection);
                    newId = (int)(ulong)getLastId.ExecuteScalar();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }                
            }
            return newId;
        }

        public void UpdateClient(Client client, string connectionString)
        {
            string sqlStatement = "UPDATE check23.client " +
                "SET name = @name, " +
                "location = @location " +
                "WHERE id = " + client.Id;

            using(MySqlConnection connection = new MySqlConnection(connectionString)) 
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", client.Name);
                if(client.Location == null || client.Location == string.Empty)
                {
                    command.Parameters.AddWithValue("@location", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@location", client.Location);
                }

                try
                {
                    connection.Open ();
                    command.ExecuteScalar();
                    connection.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public HashSet<int> SearchFor(string searchTerm, string connectionString)
        {
            HashSet<int> matches = new HashSet<int>();
            string sqlStatement = "SELECT id FROM check23.client " +
                "WHERE name LIKE @searchTerm OR location LIKE @searchTerm";
            string advancedSearchTerm = "%" + searchTerm + "%";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@searchTerm", advancedSearchTerm);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        matches.Add((int)reader[0]);
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return matches;
        }

        public bool ClientAlreadyExists(Client client, string connectionString)
        {
            bool exists;
            string sqlStatement = "SELECT EXISTS(SELECT name FROM check23.client WHERE name = @name AND location = @location)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", client.Name);
                command.Parameters.AddWithValue("@location", client.Location);

                try
                {
                    connection.Open();
                    int temp = (int)(long)command.ExecuteScalar();
                    exists = temp == 1;
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    exists = true;
                }
            }
            return exists;
        }
    }
}
//string sqlStatementStart = "SELECT id FROM check23.client WHERE ";
//MySqlCommand command = new MySqlCommand(sqlStatementStart, connection);
//for (int i = 0; i < separatedWords.Length; i++)
//{
//    string advancedSearchTerm = "%" + separatedWords[i] + "%";
//    string atSearchTerm = "@searchTerm" + i;
//    if (i > 0)
//    {
//        command.CommandText += " OR ";
//    }
//    string sqlStatementPartPerWord = "(name LIKE " + atSearchTerm + " OR location LIKE " + atSearchTerm + ")";
//    command.CommandText += sqlStatementPartPerWord;
//    command.Parameters.AddWithValue(atSearchTerm, advancedSearchTerm);
//}
