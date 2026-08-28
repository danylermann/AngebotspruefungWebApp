using Check23.Models;
using MySqlConnector;

namespace Check23.Services.DatabankAccess
{
    public class LoggingDAO
    {
        public int InsertActivityLog(int quoteEvalId, string connectionString)
        {
            int newId = -1;

            string sqlStatement = "INSERT INTO check23.activity_log (quote_evaluation_id) VALUES (" + quoteEvalId + ")";

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

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

        public int GetActivityLogIdByQuouteEvaluationId(int quoteEvalId, string connectionString)
        {
            int foundId = -1;
            string sqlStatement = "SELECT activity_log.id FROM check23.activity_log " +
                "WHERE quote_evaluation_id = " + quoteEvalId;

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundId = (int)reader[0];
                    }
                    connection.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return foundId;
        }

        public int GetActivityLogIdByRequirementId(int requirementId, string connectionString)
        {
            int foundId = -1;
            string sqlStatement = "SELECT activity_log.id FROM check23.activity_log " +
                "JOIN quote_evaluation ON activity_log.quote_evaluation_id = quote_evaluation.id " +
                "JOIN requirement ON quote_evaluation.id = requirement.quote_evaluation_id " +
                "WHERE requirement.id = " + requirementId;

            using( MySqlConnection connection = new MySqlConnection(connectionString)) 
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        foundId = (int)reader[0];
                    }
                    connection.Close();
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine(ex.ToString()); 
                }
            }

            return foundId;
        }

        public int GetActivityLogIdBySolutionId(int solutionId, string connectionString)
        {
            int foundId = -1;
            string sqlStatement = "SELECT activity_log.id FROM check23.activity_log " +
                "JOIN quote_evaluation ON activity_log.quote_evaluation_id = quote_evaluation.id " +
                "JOIN requirement ON quote_evaluation.id = requirement.quote_evaluation_id " +
                "JOIN solution ON requirement.id = solution.requirement_id " +
                "WHERE solution.id = " + solutionId;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        foundId = (int)reader[0];
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return foundId;
        }
        

        public List<LogEntry> GetAllLogEntriesByActivityLogId(int activityLogId, string connectionString)
        {
            List<LogEntry> foundLogEntries = new List<LogEntry>();
            string sqlStatement = "SELECT person, date, activity " +
                "FROM log_entry_quote_evaluation " +
                "WHERE activity_log_id = " + activityLogId +
                " UNION " +
                "SELECT person, date, activity " +
                "FROM log_entry_requirement " +
                "WHERE activity_log_id = " + activityLogId +
                " UNION " +
                "SELECT person, date, activity " +
                "FROM log_entry_solution " +
                "WHERE activity_log_id = " + activityLogId +
                " UNION " +
                "SELECT person, date, activity " +
                "FROM log_entry_estimation " +
                "WHERE activity_log_id = " + activityLogId +
                " ORDER BY date"; //Benutzen von UNION um aus allen log_entry Tabellen einen gemeinsamen Output zu generieren

            using(MySqlConnection connection = new MySqlConnection(connectionString)) 
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);


                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        foundLogEntries.Add(new LogEntry
                            (
                                (string)reader[0],
                                (DateTime)reader[1],
                                (string)reader[2]
                            ));
                    }
                    connection.Close();
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.ToString()); 
                }
            }

            return foundLogEntries;
        }

        public void InsertLogEntry(LogEntry log, string logType, string connectionString)
        {
            string sqlStatement = "INSERT INTO check23.log_entry_" + logType +
                " (person, date, activity, " + logType + "_id, activity_log_id)" +
                "VALUES (@person, @date, @activity, @foreign_key_id, @activity_log_id)";

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@person", log.Person);
                command.Parameters.AddWithValue("@date", log.Date);
                command.Parameters.AddWithValue("@activity", log.Activity);
                command.Parameters.AddWithValue("@foreign_key_id", log.ForeignKey_id);
                command.Parameters.AddWithValue("@activity_log_id", log.ActivityLog_id);

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

        public List<string> GetPersonsInEstimationLogsByEstimationId(int estimationId, string connectionString)
        {
            List<string> foundPersons = new List<string>();
            string sqlStatement = "SELECT person FROM check23.log_entry_estimation WHERE id = @id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand( sqlStatement, connection);
                command.Parameters.AddWithValue("@id", estimationId);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read()) {
                        foundPersons.Add(
                        (string)reader[0]); 
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return foundPersons;
        }
    }
}
