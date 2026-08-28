using Check23.Models;
using Humanizer;
using Microsoft.CodeAnalysis;
using MySqlConnector;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Check23.Services.DatabankAccess
{
    public class Check23DAO
    {
        public string connectionString = @"Server=127.0.0.1;Database=check23;Uid=root;Pwd=SvRoot21.02;";
        public QuoteEvaluationDAO quoteEvaluation = new QuoteEvaluationDAO();
        public RequirementDAO requirement = new RequirementDAO();
        public SolutionDAO solution = new SolutionDAO();
        public EstimationDAO estimation = new EstimationDAO();
        public CommentDAO comment = new CommentDAO();
        public LoggingDAO logging = new LoggingDAO();
        public UserDAO user = new UserDAO();
        public ClientDAO client = new ClientDAO();
        public FileUploadDAO fileUpload = new FileUploadDAO();
        public TicketDAO ticket = new TicketDAO();
        public AccessGroupDAO accessGroup = new AccessGroupDAO();
        

        public List<string> GetEmailaddresses(string sqlStatement)
        {
            List<string> addresses = new List<string>();

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(sqlStatement, connection);
                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        addresses.Add((string)reader[0]);
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return addresses;
        }

        public HashSet<int> SearchDatabaseFor(string searchTerm, bool mustIncludeAll)
        {
            HashSet<int> quoteEvaluationIds = new HashSet<int>(); 
            string[] separatedWords = searchTerm.Split(); //Splits the searchTerm into different words on whitespaces
            HashSet<int>[] idsPerWord = new HashSet<int>[separatedWords.Length]; //Creates an Array of Hashsets
            //For each word in the searchTerm search the database if any components of a quoteEvaluation contains that word and save the ids into its Hashset
            for (int i = 0; i < separatedWords.Length; i++)
            {
                idsPerWord[i] = new HashSet<int>(); //Creates the Hashset for the word
                //Add all Quote Evaluation Ids that have a match with the search term
                idsPerWord[i].UnionWith(quoteEvaluation.SearchFor(separatedWords[i], connectionString));
                //Get All Client Ids that have a match with the search term
                HashSet<int> clientIds = client.SearchFor(separatedWords[i], connectionString);
                //Add all Quote Evaluation Ids that match the client Ids
                if (clientIds.Count > 0)
                {
                    idsPerWord[i].UnionWith(quoteEvaluation.GetQuoteEvaluationIdsByClientIdList(clientIds, connectionString));
                }
                //Add all Quote Evaluation Ids whose requirements have a match with the search term
                idsPerWord[i].UnionWith(requirement.SearchFor(separatedWords[i], connectionString));
                //Add all Quote Evaluation Ids whose solutions have a match with the search term
                idsPerWord[i].UnionWith(solution.SearchFor(separatedWords[i], connectionString));
                //Add all Quote Evaluation Ids whose estimations have a match with the search term
                idsPerWord[i].UnionWith(estimation.SearchFor(separatedWords[i], connectionString));
                
            }
            //If the option mustIncludeAll was ticked during search and more than one word was searched for, check all ids in the first Hashset.            
            if (mustIncludeAll && separatedWords.Length > 1)
            {
                //If an id is not in any of the other Hashsets of this searchTerm than remove it from the first Hasshset. 
                for (int j = 1; j < separatedWords.Length; j++) //Starts at 1 to go through all Hashset past the first
                {
                    foreach(int id in idsPerWord[0])
                    {
                        if (!idsPerWord[j].Contains(id))
                        {
                            idsPerWord[0].Remove(id);
                        }
                    }
                }
                //After all ids in the first Hashset were checked any leftover ids will be merged into the solution
                quoteEvaluationIds.UnionWith(idsPerWord[0]);
                }
            else
                {
                foreach (HashSet<int> ids in idsPerWord)
                {
                    quoteEvaluationIds.UnionWith(ids);
                }
            }
            return quoteEvaluationIds;
        }
    }
}
