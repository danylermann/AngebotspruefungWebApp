using Check23.Models;
using MySqlConnector;

namespace Check23.Services.DatabankAccess
{
    public class CommentDAO
    {
        public List<Comment> GetRequirementCommentsByRequirementId(int requirementId, string connectionString)
        {
            List<Comment> foundComments = new List<Comment>();

            string sqlStatement = "SELECT * FROM check23.comment_requirement " +
                "WHERE requirement_id = " + requirementId;

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundComments.Add(new Comment(
                            (int)reader[0],
                            (string)reader[1],
                            (DateTime)reader[2],
                            (string)reader[3],
                            (int)reader[4]
                        ));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return foundComments;
        }

        public List<Comment> GetSolutionCommentsBySolutionId(int solutionId, string connectionString)
        {
            List<Comment> foundComments = new List<Comment>();

            string sqlStatement = "SELECT * FROM check23.comment_solution " +
                "WHERE solution_id = " + solutionId;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundComments.Add(new Comment(
                            (int)reader[0],
                            (string)reader[1],
                            (DateTime)reader[2],
                            (string)reader[3],
                            (int)reader[4]
                        ));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return foundComments;
        }

        public void InsertRequirementComment(Comment comment, string connectionString)
        {
            string sqlStatement = "INSERT INTO check23.comment_requirement " +
                "(creator, date, message, requirement_id)" +
                "VALUES (@creator, @date, @message, @requirement_id)";

            using(MySqlConnection connection = new MySqlConnection(connectionString)) 
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@creator", comment.Creator);
                command.Parameters.AddWithValue("@date", comment.Date);
                command.Parameters.AddWithValue("@message", comment.Message);
                command.Parameters.AddWithValue("@requirement_id", comment.ForeignKey_id);

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

        public void InsertSolutionComment(Comment comment, string connectionString)
        {
            string sqlStatement = "INSERT INTO check23.comment_solution " +
                "(creator, date, message, solution_id)" +
                "VALUES (@creator, @date, @message, @solution_id)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@creator", comment.Creator);
                command.Parameters.AddWithValue("@date", comment.Date);
                command.Parameters.AddWithValue("@message", comment.Message);
                command.Parameters.AddWithValue("@solution_id", comment.ForeignKey_id);

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
    }
}
