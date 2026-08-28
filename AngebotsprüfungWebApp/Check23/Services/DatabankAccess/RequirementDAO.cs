using Check23.Models;
using MySqlConnector;

namespace Check23.Services.DatabankAccess
{
    public class RequirementDAO
    {
        public List<Requirement> GetAllRequirements(string connectionString)
        {
            List<Requirement> foundRequirements = new List<Requirement>();

            string sqlStatement = "SELECT * FROM check23.requirement";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundRequirements.Add(new Requirement(                        
                            (int)reader[0],
                            (string)reader[1],
                            reader[2] as string,
                            reader[3] as string,
                            (bool)reader[4],
                            (bool)reader[5],
                            (bool)reader[6],
                            (bool)reader[7],
                            (bool)reader[8],
                            (bool)reader[9],
                            (bool)reader[10],
                            (bool)reader[11],
                            (bool)reader[12],
                            (bool)reader[13],
                            (bool)reader[14],
                            (bool)reader[15],
                            (bool)reader[16],
                            (bool)reader[17],
                            (bool)reader[18],
                            (int)reader[19]
                        ));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return foundRequirements;
        }

        public int InsertRequirement(Requirement requirement, int quoteEvalId, string connectionString)
        {
            int newId = -1;
            string sqlStatement = "INSERT INTO check23.requirement" +
                "(name, description, order_number, " +
                "ESW_CEETIS, ESW_IVISion_Studio, ESW_Netstar, ESW_internel_tools, ESW_other, " +
                "EHW_HV_Tester, EHW_construction, EHW_TPMs, EHW_LV_Tester, EHW_internel_tools, EHW_other, " +
                "CDE, Documentation, Service, not_to_be_carried_out, quote_evaluation_id)" +
                "VALUES (@name, @description, @order_number, " +
                "@ESW_CEETIS, @ESW_IVISion_Studio, @ESW_Netstar, @ESW_internel_tools, @ESW_other," +
                "@EHW_HV_Tester, @EHW_construction, @EHW_TPMs, @EHW_LV_Tester, @EHW_internel_tools, @EHW_other, " +
                "@CDE, @Documentation, @Service, @not_to_be_carried_out, @quote_evaluation_id)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", requirement.Name);
                if(requirement.Description == null || requirement.Description == string.Empty)
                {
                    command.Parameters.AddWithValue("@description", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@description", requirement.Description);
                }
                if (requirement.OrderNumber == null || requirement.OrderNumber == string.Empty)
                {
                    command.Parameters.AddWithValue("@order_number", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@order_number", requirement.OrderNumber);
                }
                command.Parameters.AddWithValue("@ESW_CEETIS", requirement.ESW_CEETIS);
                command.Parameters.AddWithValue("@ESW_IVISion_Studio", requirement.ESW_IVISionStudio);
                command.Parameters.AddWithValue("@ESW_Netstar", requirement.ESW_Netstar);
                command.Parameters.AddWithValue("@ESW_internel_tools", requirement.ESW_InterneTools);
                command.Parameters.AddWithValue("@ESW_other", requirement.ESW_Other);
                command.Parameters.AddWithValue("@EHW_HV_Tester", requirement.EHW_HV_Tester);
                command.Parameters.AddWithValue("@EHW_construction", requirement.EHW_Construction);
                command.Parameters.AddWithValue("@EHW_TPMs", requirement.EHW_TPMs);
                command.Parameters.AddWithValue("@EHW_LV_Tester", requirement.EHW_LV_Tester);
                command.Parameters.AddWithValue("@EHW_internel_tools", requirement.EHW_InterneTools);
                command.Parameters.AddWithValue("@EHW_other", requirement.EHW_Other);
                command.Parameters.AddWithValue("@CDE", requirement.CDE);
                command.Parameters.AddWithValue("@Documentation", requirement.Documentation);
                command.Parameters.AddWithValue("@Service", requirement.Service);
                command.Parameters.AddWithValue("@not_to_be_carried_out", requirement.NotToBeCarriedOut);
                command.Parameters.AddWithValue("@quote_evaluation_id", quoteEvalId);
                

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

        public void UpdateRequirement(Requirement anforderung, string connectionString)
        {
            string sqlStatement = "UPDATE check23.requirement " +
                "SET name = @name, " +
                "description= @description, " +
                "order_number = @order_number, " +
                "ESW_CEETIS = @ESW_CEETIS, " +
                "ESW_IVISion_Studio = @ESW_IVISion_Studio, " +
                "ESW_Netstar = @ESW_Netstar, " +
                "ESW_internel_tools = @ESW_internel_tools, " +
                "ESW_other = @ESW_other, " +
                "EHW_HV_Tester = @EHW_HV_Tester, " +
                "EHW_construction = @EHW_construction, " +
                "EHW_TPMs = @EHW_TPMs, " +
                "EHW_LV_Tester = @EHW_LV_Tester, " +
                "EHW_internel_tools = @EHW_internel_tools, " +
                "EHW_other = @EHW_other, " +
                "CDE = @CDE, " +
                "Documentation = @Documentation, " +
                "Service = @Service, " +
                "not_to_be_carried_out = @not_to_be_carried_out " +
                "WHERE id = @id";              
            
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", anforderung.Name);
                if(anforderung.Description == null || anforderung.Description == string.Empty)
                {
                    command.Parameters.AddWithValue("@description", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@description", anforderung.Description);
                }
                if (anforderung.OrderNumber == null || anforderung.OrderNumber == string.Empty)
                {
                    command.Parameters.AddWithValue("@order_number", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@order_number", anforderung.OrderNumber);
                }
                command.Parameters.AddWithValue("@ESW_CEETIS", anforderung.ESW_CEETIS);
                command.Parameters.AddWithValue("@ESW_IVISion_Studio", anforderung.ESW_IVISionStudio);
                command.Parameters.AddWithValue("@ESW_Netstar", anforderung.ESW_Netstar);
                command.Parameters.AddWithValue("@ESW_internel_tools", anforderung.ESW_InterneTools);
                command.Parameters.AddWithValue("@ESW_other", anforderung.ESW_Other);
                command.Parameters.AddWithValue("@EHW_HV_Tester", anforderung.EHW_HV_Tester);
                command.Parameters.AddWithValue("@EHW_construction", anforderung.EHW_Construction);
                command.Parameters.AddWithValue("@EHW_TPMs", anforderung.EHW_TPMs);
                command.Parameters.AddWithValue("@EHW_LV_Tester", anforderung.EHW_LV_Tester);
                command.Parameters.AddWithValue("@EHW_internel_tools", anforderung.EHW_InterneTools);
                command.Parameters.AddWithValue("@EHW_other", anforderung.EHW_Other);
                command.Parameters.AddWithValue("@CDE", anforderung.CDE);
                command.Parameters.AddWithValue("@Documentation", anforderung.Documentation);
                command.Parameters.AddWithValue("@Service", anforderung.Service);
                command.Parameters.AddWithValue("@not_to_be_carried_out", anforderung.NotToBeCarriedOut);
                command.Parameters.AddWithValue("@id", anforderung.Id);

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
        public List<Requirement> GetRequirementsByQuoteEvaluationId(int quoteEvalId, string connectionString)
        {
            List<Requirement> foundRequirements = new List<Requirement>();

            string sqlStatement = "SELECT * FROM check23.requirement WHERE quote_evaluation_id = " + quoteEvalId;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundRequirements.Add(new Requirement(
                            (int)reader[0],
                            (string)reader[1],
                            reader[2] as string,
                            reader[3] as string,
                            Convert.ToBoolean(reader[4]),
                            Convert.ToBoolean(reader[5]),
                            Convert.ToBoolean(reader[6]),
                            Convert.ToBoolean(reader[7]),
                            Convert.ToBoolean(reader[8]),
                            Convert.ToBoolean(reader[9]),
                            Convert.ToBoolean(reader[10]),
                            Convert.ToBoolean(reader[11]),
                            Convert.ToBoolean(reader[12]),
                            Convert.ToBoolean(reader[13]),
                            Convert.ToBoolean(reader[14]),
                            Convert.ToBoolean(reader[15]),
                            Convert.ToBoolean(reader[16]),
                            Convert.ToBoolean(reader[17]),
                            Convert.ToBoolean(reader[18]),
                            (int)reader[19]                                
                            ));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return foundRequirements;
        }

        public Requirement GetRequirementById(int reqId, string connectionString)
        {
            Requirement foundRequirement = new Requirement();

            string sqlStatement = "SELECT * FROM check23.requirement " +
                "WHERE id = " + reqId;

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);


                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foundRequirement = new Requirement
                            (
                                (int)reader[0],
                                (string)reader[1],
                                reader[2] as string,
                                reader[3] as string,
                                Convert.ToBoolean(reader[4]),
                                Convert.ToBoolean(reader[5]),
                                Convert.ToBoolean(reader[6]),
                                Convert.ToBoolean(reader[7]),
                                Convert.ToBoolean(reader[8]),
                                Convert.ToBoolean(reader[9]),
                                Convert.ToBoolean(reader[10]),
                                Convert.ToBoolean(reader[11]),
                                Convert.ToBoolean(reader[12]),
                                Convert.ToBoolean(reader[13]),
                                Convert.ToBoolean(reader[14]),
                                Convert.ToBoolean(reader[15]),
                                Convert.ToBoolean(reader[16]),
                                Convert.ToBoolean(reader[17]),
                                Convert.ToBoolean(reader[18]),
                                (int)reader[19]


                            ); 
                    }
                    connection.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

                return foundRequirement;
        }

        public HashSet<int> SearchFor(string searchTerm, string connectionString)
        {
            HashSet<int> matches = new HashSet<int>();

            string sqlStatement = "SELECT quote_evaluation.id FROM check23.quote_evaluation " +
                "JOIN requirement ON requirement.quote_evaluation_id = quote_evaluation.id " +
                "WHERE requirement.name LIKE @searchTerm OR description LIKE @searchTerm OR order_number LIKE @searchTerm";
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

        public bool RequirementNameAlreadyExists(string name, int quoteEvalId, string connectionString)
        {
            bool exists;
            string sqlStatement = "SELECT EXISTS(SELECT name FROM check23.requirement WHERE name = @name AND quote_evaluation_id = @id)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@id", quoteEvalId);

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
