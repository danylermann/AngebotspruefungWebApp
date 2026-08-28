using Check23.Models;
using MySqlConnector;

namespace Check23.Services.DatabankAccess
{
    public class FileUploadDAO
    {
        public void InsertFolder(Folder folder, string connectionString)
        {
            string sqlStatement = "INSERT INTO check23.folder " +
                "(name, folder_path, quote_evaluation_id) " +
                "VALUES (@name, @folder_path, @quote_evaluation_id)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", folder.Name);
                command.Parameters.AddWithValue("@folder_path", folder.FolderPath);
                command.Parameters.AddWithValue("@quote_evaluation_id", folder.QuoteEvaluationId);

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

        public int GetSpecifiedFolderIdByAccessorId(string foldername, int accessorId, string accessor, string connectionString)
        {
            int foundId = -1;
            string sqlStatement = "";
            switch (accessor)
            {
                case "quoteEval":
                    sqlStatement = "SELECT folder.id FROM check23.folder " +
                        "WHERE folder.quote_evaluation_id = " + accessorId + " AND folder.name = @foldername";

                    break;
                case "requirement":
                    sqlStatement = "SELECT folder.id FROM check23.folder " +
                        "JOIN quote_evaluation ON quote_evaluation.id = folder.quote_evaluation_id " +
                        "JOIN requirement ON quote_evaluation.id = requirement.quote_evaluation_id " +
                        "WHERE requirement.id = " + accessorId +
                        " AND folder.name = @foldername";
                    break;
                case "solution":
                    sqlStatement = "SELECT folder.id FROM check23.folder " +
                        "JOIN quote_evaluation ON quote_evaluation.id = folder.quote_evaluation_id " +
                        "JOIN requirement ON quote_evaluation.id = requirement.quote_evaluation_id " +
                        "JOIN solution ON requirement.id = solution.requirement_id " +
                        "WHERE solution.id = " + accessorId +
                        " AND folder.name = @foldername";
                    break;
            }
            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@foldername", foldername);
                try
                {
                    connection.Open();
                    foundId = (int)command.ExecuteScalar();
                    connection.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return foundId;
        }
        public void InsertFile(DatabaseFile file, string connectionString)
        {
            string sqlStatement = "INSERT INTO check23.file " +
                "(name, file_path, folder_id) " +
                "VALUES (@name, @file_path, @folder_id)";
            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement,connection);
                command.Parameters.AddWithValue("@name", file.Name);
                command.Parameters.AddWithValue("@file_path", file.DataPath);
                command.Parameters.AddWithValue("@folder_id", file.Folder_id);
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

        public List<DatabaseFile> GetAllUploadedFiles(int folderId, string connectionString)
        {
            List<DatabaseFile> foundFiles = new List<DatabaseFile>();
            string sqlStatement = "SELECT * FROM check23.file " +
                "WHERE folder_id = " + folderId;

            using(MySqlConnection connection = new MySqlConnection(connectionString)) 
            {
                MySqlCommand command = new MySqlCommand(sqlStatement,connection);
                try
                {
                    connection.Open();

                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundFiles.Add(new DatabaseFile(
                            (int)reader[0],
                            (string)reader[1],
                            (string)reader[2],
                            (int)reader[3]                           
                            ));
                    }
                    connection.Close();
                }
                catch(Exception ex) 
                {
                    Console.WriteLine();
                }
            }

            return foundFiles;
        }

        public void UpdateFolderPath(string newFolderPath, int folderId , string connectionString)
        {
            string sqlStatement = "UPDATE check23.folder " +
                "SET folder_path = @folder_path" +
                " WHERE id = " + folderId;

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@folder_path", newFolderPath);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }

        public Folder GetFolderById(int folderId, string connectionString)
        {
            Folder foundFolder = new Folder();
            string sqlStatement = "SELECT * FROM check23.folder " +
                "WHERE id = " + folderId;

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundFolder = new Folder(
                            (int)reader[0],
                            (string)reader[1],
                            (string)reader[2],
                            (int)reader[3]
                            );
                    }
                    connection.Close();
                }
                catch(Exception ex) { Console.WriteLine(ex.ToString()); }
            }

            return foundFolder;
        }

        public DatabaseFile GetFileById(int fileId, string connectionString)
        {
            DatabaseFile foundFile = new DatabaseFile();
            string sqlStatement = "SELECT * FROM check23.file " +
                "WHERE id = " + fileId;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundFile = new DatabaseFile
                            (
                            (int)reader[0],
                            reader[1] as string,
                            reader[2] as string,
                            (int)reader[3]
                            );
                    }
                    connection.Close();
                }
                catch (Exception ex) { Console.WriteLine( ex.ToString()); }
            }

            return foundFile;
        }

        public void DeleteFile(int fileId, string connectionString)
        {
            string sqlStatement = "DELETE FROM check23.file " +
                "WHERE id = " + fileId;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            }
        }
    }
}
