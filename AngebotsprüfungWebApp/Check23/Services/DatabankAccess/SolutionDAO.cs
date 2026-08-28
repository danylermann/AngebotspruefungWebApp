using Check23.Models;
using MySqlConnector;

namespace Check23.Services.DatabankAccess
{
    public class SolutionDAO
    {
        public List<Solution> GetAllSolutions(string connectionString)
        {
            List<Solution> foundSolutions = new List<Solution> ();

            string sqlStatement = "SELECT * FROM check23.solution";

            using(MySqlConnection connection = new MySqlConnection (connectionString))
            {
                MySqlCommand command = new MySqlCommand (sqlStatement, connection);

                try
                {
                    connection.Open();

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read ())
                    {
                        foundSolutions.Add(new Solution
                        (
                            (int)reader[0], 
                            (string)reader[1], 
                            (string)reader[2],
                            Convert.ToBoolean(reader[3]),
                            (int)reader[4]
                        ));
                    }
                    connection.Close();
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine(ex.ToString ());
                }
                
            }

            return foundSolutions;
        }

        public int InsertSolution(Solution solution, int reqId, string connectionString)
        {
            int newId = -1;
            string sqlStatement = "INSERT INTO check23.solution" +
                "(name, description, is_selected, requirement_id)" +
                "VALUES (@name, @description, @is_selected, @requirement_id)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand ( sqlStatement, connection);
                command.Parameters.AddWithValue("@name", solution.Name);
                command.Parameters.AddWithValue("@description", solution.Description);
                command.Parameters.AddWithValue("@is_selected", solution.IsSelected);
                command.Parameters.AddWithValue("@requirement_id", reqId);

                try
                {
                    connection.Open ();
                    command.ExecuteScalar();
                    MySqlCommand getLastId = new MySqlCommand("SELECT LAST_INSERT_ID()", connection);
                    newId = (int)(ulong)getLastId.ExecuteScalar();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString ());
                }
            }
            return newId;
        }

        public void UpdateSolution(Solution solution, string connectionString)
        {
            string sqlStatement = "UPDATE check23.solution " +
                "SET name = @name, " +
                "description = @description, " +
                "is_selected = @is_selected " +
                "WHERE id = @id";

            using ( MySqlConnection connection = new MySqlConnection( connectionString))
            {
                MySqlCommand command = new MySqlCommand (sqlStatement, connection);
                command.Parameters.AddWithValue("@name", solution.Name);
                command.Parameters.AddWithValue("@description", solution.Description);
                command.Parameters.AddWithValue("@is_selected", solution.IsSelected);
                command.Parameters.AddWithValue("@id", solution.Id);

                try
                {
                    connection.Open(); 
                    command.ExecuteScalar();
                    connection.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString ());
                }
            }
        }

        public List<Solution> GetSolutionsByRequirementId(int reqId, string connectionString)
        {
            List<Solution> foundSolutions = new List<Solution>();

            string sqlStatement = "SELECT * FROM check23.solution WHERE requirement_id = " + reqId;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        foundSolutions.Add(new Solution
                        {
                            Id = (int)reader[0],
                            Name = (string)reader[1],
                            Description = (string)reader[2],
                            IsSelected = Convert.ToBoolean(reader[3])
                        });
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }

            return foundSolutions;

        }

        internal Solution GetSolutionById(int solutionId, string connectionString)
        {
            Solution foundSolution = new Solution();

            string sqlStatement = "SELECT * FROM check23.solution " +
                "WHERE id = @id";

            using(MySqlConnection connection = new MySqlConnection( connectionString))
            {
                MySqlCommand command = new MySqlCommand (sqlStatement, connection);
                command.Parameters.AddWithValue("@id", solutionId);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        foundSolution = new Solution
                        (
                            (int)reader[0],
                            (string)reader[1],
                            (string)reader[2],
                            Convert.ToBoolean(reader[3]),
                            (int)reader[4]
                        );
                    }
                    connection.Close();
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.ToString());
                }

            }
            return foundSolution; 
        }

        public HashSet<int> SearchFor(string searchTerm, string connectionString)
        {
            HashSet<int> matches = new HashSet<int>();

            string sqlStatement = "SELECT quote_evaluation.id FROM check23.quote_evaluation " +
                "JOIN requirement ON requirement.quote_evaluation_id = quote_evaluation.id " +
                "JOIN solution ON solution.requirement_id = solution.id " +
                "WHERE solution.name LIKE @searchTerm OR solution.description LIKE @searchTerm";
            string advancedSearchTerm = "%" + searchTerm + "%";

            using( MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand (sqlStatement , connection);
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

        public bool SolutionNameAlreadyExists(string name, int requirementId, string connectionString)
        {
            bool exists;
            string sqlStatement = "SELECT EXISTS(SELECT name FROM check23.solution WHERE name = @name AND requirement_id = @id)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@id", requirementId);

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

        public void SetSelected(int solutionId, string connectionString)
        {
            string sqlStatement = "Update check23.solution " +
                "SET is_selected = 1 " +
                "WHERE id = " + solutionId;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
