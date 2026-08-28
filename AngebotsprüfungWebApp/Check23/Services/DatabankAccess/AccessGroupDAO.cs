using Check23.Models;
using MySqlConnector;

namespace Check23.Services.DatabankAccess
{
    public class AccessGroupDAO
    {
        public List<AccessGroup> GetAllAccessGroupsExceptDefault(string connectionString)
        {
            List<AccessGroup> foundAccessGroups = new List<AccessGroup>();

            string sqlStatement = "SELECT * FROM check23.access_group WHERE id > 0";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        foundAccessGroups.Add(new AccessGroup(
                            (int)reader[0],
                            (string)reader[1],
                            Convert.ToBoolean(reader[2]),
                            Convert.ToBoolean(reader[3]),
                            Convert.ToBoolean(reader[4]),
                            Convert.ToBoolean(reader[5]),
                            Convert.ToBoolean(reader[6]),
                            Convert.ToBoolean(reader[7]),
                            Convert.ToBoolean(reader[8])));
                    }
                    connection.Close();
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.ToString());
                }
                
            }
            return foundAccessGroups;
        }

        public void InsertAccessGroup(AccessGroup accessGroup, string connectionString) 
        {
            string sqlStatement = "INSERT INTO check23.access_group" +
                "(name, create_user, create_access_group, create_client, create_quote_evaluation, create_requirement, create_solution, create_estimation)" +
                "VALUES(@name, @create_user, @create_access_group, @create_client, @create_quote_evaluation, @create_requirement, @create_solution, @create_estimation)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", accessGroup.Name);
                command.Parameters.AddWithValue("@create_user", accessGroup.CreateUser);
                command.Parameters.AddWithValue("@create_access_group", accessGroup.CreateAccessGroup);
                command.Parameters.AddWithValue("@create_client", accessGroup.CreateClient);
                command.Parameters.AddWithValue("@create_quote_evaluation", accessGroup.CreateQuoteEvaluation);
                command.Parameters.AddWithValue("@create_requirement", accessGroup.CreateRequirement);
                command.Parameters.AddWithValue("@create_solution", accessGroup.CreateSolution);
                command.Parameters.AddWithValue("@create_estimation", accessGroup.CreateEstimation);

                try
                {
                    connection.Open();

                    command.ExecuteNonQuery();
                    connection.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void UpdateAccessGroup(AccessGroup accessGroup, string connectionString)
        {
            string sqlStatement = "UPDATE check23.access_group " +
                "SET name = @name, " +
                "create_user = @create_user, " +
                "create_access_group = @create_access_group, " +
                "create_client = @create_client, " +
                "create_quote_evaluation = @create_quote_evaluation, " +
                "create_requirement = @create_requirement, " +
                "create_solution = @create_solution, " +
                "create_estimation = @create_estimation " +
                "WHERE id = @id";

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", accessGroup.Name);
                command.Parameters.AddWithValue("@create_user", accessGroup.CreateUser);
                command.Parameters.AddWithValue("@create_access_group", accessGroup.CreateAccessGroup);
                command.Parameters.AddWithValue("@create_client", accessGroup.CreateClient);
                command.Parameters.AddWithValue("@create_quote_evaluation", accessGroup.CreateQuoteEvaluation);
                command.Parameters.AddWithValue("@create_requirement", accessGroup.CreateRequirement);
                command.Parameters.AddWithValue("@create_solution", accessGroup.CreateSolution);
                command.Parameters.AddWithValue("@create_estimation", accessGroup.CreateEstimation);
                command.Parameters.AddWithValue("@id", accessGroup.Id);

                try
                {
                    connection.Open();

                    command.ExecuteNonQuery();

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public AccessGroup GetAccessGroupById(int accessGroupId, string connectionString)
        {
            AccessGroup foundAccessGroup = new AccessGroup();

            string sqlStatement = "SELECT * FROM check23.access_group " +
                "WHERE id = @id";

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement,connection);
                command.Parameters.AddWithValue("@id", accessGroupId);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundAccessGroup = new AccessGroup(
                            (int)reader[0],
                            (string)reader[1],
                            Convert.ToBoolean(reader[2]),
                            Convert.ToBoolean(reader[3]),
                            Convert.ToBoolean(reader[4]),
                            Convert.ToBoolean(reader[5]),
                            Convert.ToBoolean(reader[6]),
                            Convert.ToBoolean(reader[7]),
                            Convert.ToBoolean(reader[8]));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return foundAccessGroup;
        }
    }
}
