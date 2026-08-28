using Check23.Models;
using MySqlConnector;

namespace Check23.Services.DatabankAccess
{
    public class UserDAO
    {
        public List<User> GetAllUsers(string connectionString)
        {
            List<User> foundUsers = new List<User>();

            string sqlStatement = "SELECT * FROM check23.user";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        foundUsers.Add(new User
                        (
                            (int)reader[0],
                            (string)reader[1],
                            (string)reader[2],
                            (int)reader[3]
                        ));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return foundUsers;
        }

        public int InsertUser(User user, string connectionString)
        {
            int newId = -1;
            string sqlStatement = "INSERT INTO check23.user " +
                "(name, email, access_group_id) " +
                "VALUES (@name, @email, @access_group_id)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@access_group_id", user.AccessGroup_Id);

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

        public void UpdateUser(User user, string connectionString)
        {
            string sqlStatement = "UPDATE check23.user " +
                "SET name = @name, " +
                "email = @email, " +
                "access_group_id = @access_group_id " +
                "WHERE id = " + user.Id;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@access_group_id", user.AccessGroup_Id);

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

        public User GetUserById(int userId, string connectionString)
        {
            User foundUser = new User();

            string sqlStatement = "SELECT * FROM check23.User " +
                "WHERE id = " + userId;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundUser = new User
                            (
                                (int)reader[0],
                                (string)reader[1],
                                (string)reader[2],
                                (int)reader[3]

                            );
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return foundUser;
        }

        public bool UsernameExists(string username, string connectionString)
        {
            bool usernameExists = false;

            string sqlStatement = "SELECT EXISTS(SELECT name FROM check23.user WHERE name = @name)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", username);

                try
                {
                    connection.Open();
                    usernameExists = Convert.ToBoolean(command.ExecuteScalar());
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return usernameExists;
        }

        public List<AreaOfResponsibility> GetAllAreasOfResponsibility(string connectionString)
        {
            List<AreaOfResponsibility> foundResponsibillities = new List<AreaOfResponsibility>();

            string sqlStatement = "SELECT * FROM check23.area_of_responsibility";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(@sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        foundResponsibillities.Add(new AreaOfResponsibility(
                        (int)reader[0],
                        (string)reader[1]));
                    }
                    connection.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }

            return foundResponsibillities;
        }

        public void InsertUserHasAreaOfResponsibility(int userId, int areaOfResponsibilityId, string connectionString)
        {
            string sqlStatement = "INSERT INTO check23.user_has_area_of_responsibility " +
                "VALUES (@userId, @areaOfResponsibilityId)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@areaOfResponsibilityId", areaOfResponsibilityId);

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

        public User GetUserByName(string name, string connectionString)
        {
            User foundUser = new User();

            string sqlStatement = "SELECT * FROM check23.user WHERE name = @name";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", name);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        foundUser = new User(
                            (int)reader[0],
                            (string)reader[1],
                            (string)reader[2],
                            (int)reader[3]
                            );
                    }
                    connection.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
            return foundUser;
        }

        public List<int> GetAreaOfResponsibilityIdsByUserId(int userId, string connectionString)
        {
            List<int> foundIds = new List<int>();

            string sqlStatement = "SELECT area_of_responsibility.id FROM area_of_responsibility " +
                "JOIN user_has_area_of_responsibility ON area_of_responsibility.id = user_has_area_of_responsibility.area_of_responsibility_id " +
                "JOIN user ON user_has_area_of_responsibility.user_id = user.id " +
                "WHERE user.id = @id";

            using(MySqlConnection connection = new MySqlConnection( connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@id", userId);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        foundIds.Add((int)reader[0]);
                    }
                    connection.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }

            return foundIds;
        }

        public void DeleteUserHasAreOfResponsibility(int userId, int areaOfResponsibilityId, string connectionString)
        {
            string sqlStatement = "DELETE FROM check23.user_has_area_of_responsibility WHERE user_id = @userId and area_of_responsibility_id = @areaOfResponsibilityId";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@areaOfResponsibilityId", areaOfResponsibilityId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
        }

        public HashSet<string> GetEmailadressesByAreaOfResponsibilityId(int areaOfResponsibilityId, string connectionString)
        {
            HashSet<string> foundEmailadresses = new HashSet<string>();
            string sqlStatement = "SELECT email FROM check23.user " +
                "JOIN check23.user_has_area_of_responsibility ON user.id = user_has_area_of_responsibility.user_id " +
                "JOIN check23.area_of_responsibility ON user_has_area_of_responsibility.area_of_responsibility_id = area_of_responsibility.id " +
                "WHERE area_of_responsibility.id = @areaOfResponsibilityId";

            using (MySqlConnection connection = new MySqlConnection(connectionString)) 
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@areaOfResponsibilityId", areaOfResponsibilityId);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read()) 
                    {
                        foundEmailadresses.Add((string)reader[0]);
                    }
                    connection.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
            return foundEmailadresses;
        }

        public string GetEmailAddressByUsername(string username, string connectionString)
        {
            string emailAddress = "";
            string sqlStatement = "SELECT email FROM check23.user WHERE name = @name";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", username);

                try
                {
                    connection.Open();
                    emailAddress = (string)command.ExecuteScalar();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return emailAddress;
        }
    }
}