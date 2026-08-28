using Check23.Models;

namespace Check23.Services.AccessValidation
{
    public class AccessValidationService : IAccessValidationService
    {
        public bool AccessGroup(AccessGroup accessGroup)
        {
            return accessGroup.CreateAccessGroup;
        }

        public bool Client(AccessGroup accessGroup)
        {
            return accessGroup.CreateClient;
        }

        public bool Estimation(AccessGroup accessGroup)
        {
            return accessGroup.CreateEstimation;
        }

        public bool QuoteEvaluation(AccessGroup accessGroup)
        {
            return accessGroup.CreateQuoteEvaluation;
        }

        public bool Requirement(AccessGroup accessGroup)
        {
            return accessGroup.CreateRequirement;
        }

        public bool Solution(AccessGroup accessGroup)
        {
            return accessGroup.CreateSolution;
        }

        public bool User(AccessGroup accessGroup)
        {
            return accessGroup.CreateUser;
        }
    }
}
