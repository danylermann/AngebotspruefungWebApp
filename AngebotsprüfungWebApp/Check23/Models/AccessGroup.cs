using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Check23.Models
{
    public class AccessGroup
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name wird benötigt")]
        public string Name { get; set; }
        [DisplayName("Nutzer")]
        public bool CreateUser { get; set; }
        [DisplayName("Zugriffsgruppen")]
        public bool CreateAccessGroup { get; set; }
        [DisplayName("Kunden")]
        public bool CreateClient { get; set; }
        [DisplayName("Angebotsprüfungen")]
        public bool CreateQuoteEvaluation {  get; set; }
        [DisplayName("Anforderungen")]
        public bool CreateRequirement { get; set; }
        [DisplayName("Lösungen")]
        public bool CreateSolution { get; set; }
        [DisplayName("Abschätzen")]
        public bool CreateEstimation { get; set; }

        public AccessGroup(int id, string name, bool createUser, bool createAccessGroup, bool createClient, bool createQuoteEvaluation, bool createRequirement, bool createSolution, bool createEstimation)
        {
            Id = id;
            Name = name;
            CreateUser = createUser;
            CreateAccessGroup = createAccessGroup;
            CreateClient = createClient;
            CreateQuoteEvaluation = createQuoteEvaluation;
            CreateRequirement = createRequirement;
            CreateSolution = createSolution;
            CreateEstimation = createEstimation;
        }

        public AccessGroup(string name, bool createUser, bool createAccessGroup, bool createClient, bool createQuoteEvaluation, bool createRequirement, bool createSolution, bool createEstimation)
        {
            Name = name;
            CreateUser = createUser;
            CreateClient = createClient;
            CreateAccessGroup = createAccessGroup;
            CreateQuoteEvaluation = createQuoteEvaluation;
            CreateRequirement = createRequirement;
            CreateSolution = createSolution;
            CreateEstimation = createEstimation;
        }

        public AccessGroup() { }
    }
}
