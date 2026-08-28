using Check23.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Check23.Services.DatabankAccess
{
    public class QuoteEvaluationDAO
    {
        //Order Category struct
        struct OrderCategory
        {
            public string name;
            public int dbValue;
        }
        private readonly OrderCategory[] OrderCategories = { new OrderCategory{ name = "angebotprüfung", dbValue = 1}, new OrderCategory { name = "entwicklungsauftrag", dbValue = 2 } };
        public List<QuoteEvaluation> GetAllQuouteEvaluations(string connectionString)
        {
            List<QuoteEvaluation> foundQuoteEvaluations = new List<QuoteEvaluation> ();

            string sqlStatement = "SELECT * FROM check23.quote_evaluation";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand (sqlStatement, connection);
                
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader ();

                    while (reader.Read ())
                    {
                        foundQuoteEvaluations.Add(new QuoteEvaluation (
                        
                            (int)reader[0],
                            (string)reader[1],
                            (string)reader[2],
                            (DateTime)reader[3],
                            reader[4] as string,
                            reader[5] as string,
                            (int)reader[6],
                            (int)reader[7],
                            Convert.IsDBNull(reader[8]) ? null : (int?)reader[8]
                        ));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine (ex.Message);
                }
            }
            return foundQuoteEvaluations;
        }

        public int InsertQuoteEvaluation(QuoteEvaluation quoteEval, string connectionString)
        {
            int newId = -1;
            string sqlStatement = "INSERT INTO check23.quote_evaluation " +
                "(name, creator, date, legal_guidelines, external_contact, order_category, status, client_id) " +
                "" +
                "VALUES (@name, @creator, @date, @legal_guidelines, @external_contact, @order_category, @status, @client_id)";
            using(MySqlConnection connection = new MySqlConnection (connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", quoteEval.Name);
                command.Parameters.AddWithValue("@creator", quoteEval.Creator);
                command.Parameters.AddWithValue("@date", quoteEval.Date);
                if (quoteEval.LegalGuidelines == null || quoteEval.LegalGuidelines == string.Empty)
                {
                    command.Parameters.AddWithValue("@legal_guidelines", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@legal_guidelines", quoteEval.LegalGuidelines);
                }
                if (quoteEval.ExternalContact == null || quoteEval.ExternalContact == string.Empty)
                {
                    command.Parameters.AddWithValue("@external_contact", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@external_contact", quoteEval.ExternalContact);
                }                
                command.Parameters.AddWithValue("@order_category", quoteEval.OrderCategory);
                command.Parameters.AddWithValue("@status", quoteEval.Status);
                if(quoteEval.Client_id == null)
                {
                    command.Parameters.AddWithValue("@client_id", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@client_id", quoteEval.Client_id);
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
                    Console.WriteLine (ex.Message);
                }
            }
            return newId;
        }

        public void UpdateQuoteEvaluation(QuoteEvaluation quoteEval, string connectionString)
        {
            string sqlStatement = "UPDATE check23.quote_evaluation " +
                "SET name = @name, " +
                "creator = @creator, " +
                "date = @date, " +
                "legal_guidelines = @legal_guidelines, " +
                "external_contact = @external_contact, " +
                "order_category = @order_category, " +
                "client_id = @client_id " +
                "WHERE id = @id";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@id", quoteEval.Id);
                command.Parameters.AddWithValue("@name", quoteEval.Name);
                command.Parameters.AddWithValue("@creator", quoteEval.Creator);
                command.Parameters.AddWithValue("@date", quoteEval.Date);
                if (quoteEval.LegalGuidelines == null || quoteEval.LegalGuidelines == string.Empty)
                {
                    command.Parameters.AddWithValue("@legal_guidelines", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@legal_guidelines", quoteEval.LegalGuidelines);
                }
                if (quoteEval.ExternalContact == null || quoteEval.ExternalContact == string.Empty)
                {
                    command.Parameters.AddWithValue("@external_contact", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@external_contact", quoteEval.ExternalContact);
                }
                command.Parameters.AddWithValue("@order_category", quoteEval.OrderCategory);
                if (quoteEval.Client_id == null)
                {
                    command.Parameters.AddWithValue("@client_id", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@client_id", quoteEval.Client_id);
                }

                try
                {
                    connection.Open();
                    command.ExecuteScalar();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }


        }

        public QuoteEvaluation GetQuoteEvaluationById(int quoteEvalId, string connectionString)
        {
            QuoteEvaluation foundQuoteEvaluation = new QuoteEvaluation();

            string sqlStatement = "SELECT * FROM check23.quote_evaluation " +
                "WHERE id = @id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@id", quoteEvalId);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        foundQuoteEvaluation = new QuoteEvaluation(
                            (int)reader[0],
                            (string)reader[1],
                            (string)reader[2],
                            (DateTime)reader[3],
                            reader[4] as string,
                            reader[5] as string,
                            (int)reader[6],
                            (int)reader[7],
                            Convert.IsDBNull(reader[8]) ? null : (int?)reader[8]);
                    }
                    connection.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());                    
                }


            }
            return foundQuoteEvaluation;
        }

        public List<QuoteEvaluation> GetAllQuouteEvaluationsOrdered(string orderBy, bool desc, string connectionString)
        {
            List<QuoteEvaluation> orderedQuoteEvaluations = new List<QuoteEvaluation>();
            string sqlStatement = "";
            if (desc)
            {
                sqlStatement = "SELECT * FROM check23.quote_evaluation ORDER BY " + orderBy + " DESC";
            }
            else
            {
                sqlStatement = "SELECT * FROM check23.quote_evaluation ORDER BY " + orderBy + " ASC";
            }

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        orderedQuoteEvaluations.Add(new QuoteEvaluation(
                            (int)reader[0],
                            (string)reader[1],
                            (string)reader[2],
                            (DateTime)reader[3],
                            reader[4] as string,
                            reader[5] as string,
                            (int)reader[6],
                            (int)reader[7],
                            Convert.IsDBNull(reader[8]) ? null : (int?)reader[8]
                        ));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return orderedQuoteEvaluations;
        }

        public List<QuoteEvaluation> GetQuoteEvaluationsByListOrdered(HashSet<int> quoteEvaluationIds, string orderBy, bool desc, string connectionString)
        {
            List<QuoteEvaluation> foundQuoteEvaluations = new List<QuoteEvaluation>();
            if (quoteEvaluationIds != null && quoteEvaluationIds.Count != 0)
            {
                string sqlStatement = "SELECT * FROM check23.quote_evaluation WHERE id IN(";
                sqlStatement += quoteEvaluationIds.First();
                quoteEvaluationIds.Remove(quoteEvaluationIds.First());
                foreach (int quoteEvaluationId in quoteEvaluationIds)
                {
                    sqlStatement += "," + quoteEvaluationId.ToString();
                }
                if (desc)
                {
                    sqlStatement += ") ORDER BY " + orderBy + " DESC";
                }
                else
                {
                    sqlStatement += ") ORDER BY " + orderBy + " ASC";
                }
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                    try
                    {
                        connection.Open();
                        MySqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            foundQuoteEvaluations.Add(new QuoteEvaluation(
                                (int)reader[0],
                                (string)reader[1],
                                (string)reader[2],
                                (DateTime)reader[3],
                                reader[4] as string,
                                reader[5] as string,
                                (int)reader[6],
                                (int)reader[7],
                                Convert.IsDBNull(reader[8]) ? null : (int?)reader[8]
                            ));
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            return foundQuoteEvaluations;
        }

        public List<QuoteEvaluation> GetQuoteEvaluationsByList(HashSet<int> quoteEvaluationIds, string connectionString)
        {
            List<QuoteEvaluation> foundQuoteEvaluations = new List<QuoteEvaluation>();
            if (quoteEvaluationIds != null && quoteEvaluationIds.Count != 0)
            {
                string sqlStatement = "SELECT * FROM check23.quote_evaluation WHERE id IN(";
                sqlStatement += quoteEvaluationIds.First();
                quoteEvaluationIds.Remove(quoteEvaluationIds.First());
                foreach (int quoteEvaluationId in quoteEvaluationIds)
                {
                    sqlStatement += "," + quoteEvaluationId.ToString();
                }
                sqlStatement += ")";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                    try
                    {
                        connection.Open();
                        MySqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            foundQuoteEvaluations.Add(new QuoteEvaluation(
                                (int)reader[0],
                                (string)reader[1],
                                (string)reader[2],
                                (DateTime)reader[3],
                                reader[4] as string,
                                reader[5] as string,
                                (int)reader[6],
                                (int)reader[7],
                                Convert.IsDBNull(reader[8]) ? null : (int?)reader[8]
                            ));
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

            }
            return foundQuoteEvaluations;

        }

        public HashSet<int> SearchFor(string searchTerm, string connectionString)
        {
            HashSet<int> matches = new HashSet<int>();
            string lowerSearchTerm = searchTerm.ToLower();
            string sqlStatement = "SELECT id from check23.quote_evaluation " +
                "WHERE name LIKE @searchTerm OR creator LIKE @searchTerm OR legal_guidelines LIKE @searchTerm OR external_contact LIKE @searchTerm";
            foreach(OrderCategory category in OrderCategories)
            {
                if(category.name == lowerSearchTerm)
                {
                    switch(category.name)
                    {
                        case "angebotsprüfung":
                            sqlStatement += " OR order_category = 1";
                            break;
                        case "entwicklungsauftrag":
                            sqlStatement += " OR order_category = 2";
                            break;
                    }
                }
            }
            if(lowerSearchTerm == "new" || lowerSearchTerm == "neu")
            {
                sqlStatement += " OR status = 1";
            }
            else if (lowerSearchTerm.Contains("abschätzung"))
            {
                sqlStatement += " OR status = 2";
            }
            else if (lowerSearchTerm.Contains("frei"))
            {
                sqlStatement += " OR status = 3";
            }
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

        public HashSet<int> SearchViaOrderCategory(int orderCategory, string connectionString)
        {
            HashSet<int> quoteEvaluationIds = new HashSet<int>();            
            string sqlStatement = "SELECT id FROM check23.quote_evaluation " +
                "WHERE order_category = " + orderCategory;

            using( MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        quoteEvaluationIds.Add((int)reader[0]);
                    }
                    connection.Close();
                }
                catch(Exception ex) { Console.WriteLine(ex.ToString()) ; }
            }

            return quoteEvaluationIds;
        }

        public HashSet<int> GetQuoteEvaluationIdsByClientIdList(HashSet<int> clientIds, string connectionString)
        {
            HashSet<int> quoteEvaluationIds = new HashSet<int>();

            string sqlStatement = "SELECT id FROM check23.quote_evaluation WHERE client_id IN(";
            sqlStatement += clientIds.First();
            clientIds.Remove(clientIds.First());
            foreach(int id in clientIds)
            {
                sqlStatement += "," + id.ToString();
            }
            sqlStatement += ")";

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        quoteEvaluationIds.Add((int)reader[0]);
                    }
                    connection.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }

            return quoteEvaluationIds;
        }

        public bool QuoteEvaluationNameAlreadyExists(string name, string connectionString) 
        {
            bool exists;
            string sqlStatement = "SELECT EXISTS(SELECT name FROM check23.quote_evaluation WHERE name = @name)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement,connection);
                command.Parameters.AddWithValue("@name", name);

                try
                {
                    connection.Open();
                    int temp = (int)(long)command.ExecuteScalar();
                    exists = temp == 1;
                    connection.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    exists = true;
                }
            }
            return exists;
        }

        public void UpdateStatus(int quoteEvalId, int newStatus, string connectionString)
        {
            string sqlStatement = "UPDATE check23.quote_evaluation " +
                "SET status = @status " +
                "WHERE id = @id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@status", newStatus);
                command.Parameters.AddWithValue("@id", quoteEvalId);

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
