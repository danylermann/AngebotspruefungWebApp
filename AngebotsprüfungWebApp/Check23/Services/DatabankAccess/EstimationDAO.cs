using Check23.Models;
using MySqlConnector;

namespace Check23.Services.DatabankAccess
{
    public class EstimationDAO
    {
        public List<Estimation> GetAllEstimations(string conncetionString)
        {
            List<Estimation> foundAbschätzungen = new List<Estimation>();

            string sqlStatement = "SELECT * FROM check23.estimation";

            using(MySqlConnection connection = new MySqlConnection(conncetionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();

                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundAbschätzungen.Add(new Estimation
                        {
                            Id = (int)reader[0],
                            ESW_time = reader[1] as string,
                            ESW_cost = reader[2] as string,
                            EHW_time = reader[3] as string,
                            EHW_cost = reader[4] as string,
                            CDE_time = reader[5] as string,
                            CDE_cost = reader[6] as string,
                            Documentation_time = reader[7] as string,
                            Documentation_cost = reader[8] as string
                        });
                    }
                    connection.Close();
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return foundAbschätzungen;
        }

        //Jedesmal wenn eine Lösung hinzugefügt wird, wird automatisch eine leere Abschätzung hinzugefügt die dann später über Update befüllt wird
        public void InsertEstimation( int solutionId, string connectionString)
        {
            string sqlStatement = "INSERT INTO check23.estimation " +
                "(ESW_time, ESW_time_description, ESW_cost, ESW_cost_description, EHW_time, EHW_time_description, EHW_cost, EHW_cost_description, CDE_time, CDE_cost, Documentation_time, Documentation_time_description, Documentation_cost, Documentation_cost_description, Service_time, Service_time_description, Service_cost, Service_cost_description, solution_id) " +
                "VALUES (@ESW_time, @ESW_time_description, @ESW_cost, @ESW_cost_description, @EHW_time, @EHW_time_description, @EHW_cost, @EHW_cost_description, @CDE_time, @CDE_cost, @Documentation_time, @Documentation_time_description, @Documentation_cost, @Documentation_cost_description, @Service_time, @Service_time_description, @Service_cost, @Service_cost_description, @solution_id)";

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@ESW_time", DBNull.Value);
                command.Parameters.AddWithValue("@ESW_time_description", DBNull.Value);
                command.Parameters.AddWithValue("@ESW_cost", DBNull.Value);
                command.Parameters.AddWithValue("@ESW_cost_description", DBNull.Value);
                command.Parameters.AddWithValue("@EHW_time", DBNull.Value);
                command.Parameters.AddWithValue("@EHW_time_description", DBNull.Value);
                command.Parameters.AddWithValue("@EHW_cost", DBNull.Value);
                command.Parameters.AddWithValue("@EHW_cost_description", DBNull.Value);
                command.Parameters.AddWithValue("@CDE_time", DBNull.Value);
                command.Parameters.AddWithValue("@CDE_cost", DBNull.Value);
                command.Parameters.AddWithValue("@Documentation_time", DBNull.Value);
                command.Parameters.AddWithValue("@Documentation_time_description", DBNull.Value);
                command.Parameters.AddWithValue("@Documentation_cost", DBNull.Value);
                command.Parameters.AddWithValue("@Documentation_cost_description", DBNull.Value);
                command.Parameters.AddWithValue("@Service_time", DBNull.Value);
                command.Parameters.AddWithValue("@Service_time_description", DBNull.Value);
                command.Parameters.AddWithValue("@Service_cost", DBNull.Value);
                command.Parameters.AddWithValue("@Service_cost_description", DBNull.Value);
                command.Parameters.AddWithValue("@solution_id", solutionId);

                try
                {
                    connection.Open();
                    command.ExecuteScalar();
                    connection.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }


        public Estimation GetEstimationById(int estimationId, string connectionString)
        {
            Estimation foundEstimation = new Estimation();

            string sqlStatement = "SELECT * FROM check23.estimation " +
                "WHERE id = @id";

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@id", estimationId);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundEstimation = new Estimation
                            (
                                (int)reader[0],
                                reader[1] as string,
                                reader[2] as string,
                                reader[3] as string,
                                reader[4] as string,
                                reader[5] as string,
                                reader[6] as string,
                                reader[7] as string,
                                reader[8] as string,
                                reader[9] as string,
                                reader[10] as string,
                                reader[11] as string,
                                reader[12] as string,
                                reader[13] as string,
                                reader[14] as string,
                                reader[15] as string,
                                reader[16] as string,
                                reader[17] as string,
                                reader[18] as string,
                                (int)reader[19]
                            );
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return foundEstimation;
        }

        public Estimation GetEstimationBySolutionId(int solutionId, string connectionString)
        {
            Estimation foundEstimation = new Estimation();

            string sqlStatement = "SELECT * FROM check23.estimation WHERE solution_id =" + solutionId;

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundEstimation = new Estimation
                            (
                                (int)reader[0],
                                reader[1] as string,
                                reader[2] as string,
                                reader[3] as string,
                                reader[4] as string,
                                reader[5] as string,
                                reader[6] as string,
                                reader[7] as string,
                                reader[8] as string,
                                reader[9] as string,
                                reader[10] as string,
                                reader[11] as string,
                                reader[12] as string,
                                reader[13] as string,
                                reader[14] as string,
                                reader[15] as string,
                                reader[16] as string,
                                reader[17] as string,
                                reader[18] as string,
                                (int)reader[19]
                            );
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

                return foundEstimation;
        }

        public void UpdateEstimationByDepartment(Estimation estimation, string department, string connectionString)
        {
            string sqlStatement = "Update check23.estimation " +
                "SET " + department + "_time = @department_time, " + 
                department + "_time_description = @department_time_description, " +
                department + "_cost = @department_cost, " +
                department + "_cost_description = @department_cost_description " +
                "WHERE id = @id";

            using( MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                switch (department)
                {
                    case "ESW":
                        if (string.IsNullOrEmpty(estimation.ESW_time))
                        {
                            command.Parameters.AddWithValue("@department_time", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_time", estimation.ESW_time);
                        }
                        if (string.IsNullOrEmpty(estimation.ESW_time_description))
                        {
                            command.Parameters.AddWithValue("@department_time_description", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_time_description", estimation.ESW_time_description);
                        }
                        if (string.IsNullOrEmpty(estimation.ESW_cost))
                        {
                            command.Parameters.AddWithValue("@department_cost", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_cost", estimation.ESW_cost);
                        }
                        if (string.IsNullOrEmpty(estimation.ESW_cost_description))
                        {
                            command.Parameters.AddWithValue("@department_cost_description", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_cost_description", estimation.ESW_cost_description);
                        }
                        break;
                    case "EHW":
                        if (string.IsNullOrEmpty(estimation.EHW_time))
                        {
                            command.Parameters.AddWithValue("@department_time", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_time", estimation.EHW_time);
                        }
                        if (string.IsNullOrEmpty(estimation.EHW_time_description))
                        {
                            command.Parameters.AddWithValue("@department_time_description", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_time_description", estimation.EHW_time_description);
                        }
                        if (string.IsNullOrEmpty(estimation.EHW_cost))
                        {
                            command.Parameters.AddWithValue("@department_cost", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_cost", estimation.EHW_cost);
                        }
                        if (string.IsNullOrEmpty(estimation.EHW_cost_description))
                        {
                            command.Parameters.AddWithValue("@department_cost_description", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_cost_description", estimation.EHW_cost_description);
                        }
                        break;
                    case "CDE":
                        if (string.IsNullOrEmpty(estimation.CDE_time))
                        {
                            command.Parameters.AddWithValue("@department_time", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_time", estimation.CDE_time);
                        }
                        if (string.IsNullOrEmpty(estimation.CDE_cost))
                        {
                            command.Parameters.AddWithValue("@department_cost", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_cost", estimation.CDE_cost);
                        }
                        break;
                    case "Documentation":
                        if (string.IsNullOrEmpty(estimation.Documentation_time))
                        {
                            command.Parameters.AddWithValue("@department_time", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_time", estimation.Documentation_time);
                        }
                        if (string.IsNullOrEmpty(estimation.Documentation_time_description))
                        {
                            command.Parameters.AddWithValue("@department_time_description", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_time_description", estimation.Documentation_time_description);
                        }
                        if (string.IsNullOrEmpty(estimation.Documentation_cost))
                        {
                            command.Parameters.AddWithValue("@department_cost", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_cost", estimation.Documentation_cost);
                        }
                        if (string.IsNullOrEmpty(estimation.Documentation_cost_description))
                        {
                            command.Parameters.AddWithValue("@department_cost_description", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_cost_description", estimation.Documentation_cost_description);
                        }
                        break;
                    case "Service":
                        if (string.IsNullOrEmpty(estimation.Service_time))
                        {
                            command.Parameters.AddWithValue("@department_time", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_time", estimation.Service_time);
                        }
                        if (string.IsNullOrEmpty(estimation.Service_time_description))
                        {
                            command.Parameters.AddWithValue("@department_time_description", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_time_description", estimation.Service_time_description);
                        }
                        if (string.IsNullOrEmpty(estimation.Service_cost))
                        {
                            command.Parameters.AddWithValue("@department_cost", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_cost", estimation.Service_cost);
                        }
                        if (string.IsNullOrEmpty(estimation.Service_cost_description))
                        {
                            command.Parameters.AddWithValue("@department_cost_description", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@department_cost_description", estimation.Service_cost_description);
                        }
                        break;
                }
                command.Parameters.AddWithValue("@id", estimation.Id);

                try
                {
                    connection.Open();
                    command.ExecuteScalar();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public HashSet<int> SearchFor(string searchTerm, string connectionString)
        {
            HashSet<int> matches = new HashSet<int>();
            string sqlStatement = "SELECT quote_evaluation.id FROM check23.quote_evaluation " +
                "JOIN requirement ON requirement.quote_evaluation_id = quote_evaluation.id " +
                "JOIN solution ON solution.requirement_id = requirement.id " +
                "JOIN estimation ON estimation.solution_id = solution.id " +
                "WHERE ESW_time LIKE @searchTerm OR ESW_time_description LIKE @searchTerm OR ESW_cost LIKE @searchTerm OR ESW_cost_description LIKE @searchTerm " +
                "OR EHW_time LIKE @searchTerm OR EHW_time_description LIKE @searchTerm OR EHW_cost LIKE @searchTerm OR EHW_cost_description LIKE @searchTerm " +
                "OR CDE_time LIKE @searchTerm OR CDE_cost LIKE @searchTerm " +
                "OR Documentation_time LIKE @searchTerm OR Documentation_time_description LIKE @searchTerm OR Documentation_cost LIKE @searchTerm OR Documentation_cost_description LIKE @searchTerm " +
                "OR Service_time LIKE @searchTerm OR Service_time_description LIKE @searchTerm OR Service_cost LIKE @searchTerm OR Service_cost_description LIKE @searchTerm";
            string advancedSearchTerm = "%" + searchTerm + "%";

            using(MySqlConnection connection = new MySqlConnection(connectionString))
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
    }
}
