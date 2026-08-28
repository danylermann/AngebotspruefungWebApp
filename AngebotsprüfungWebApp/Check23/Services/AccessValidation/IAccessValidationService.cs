using Check23.Models;

namespace Check23.Services.AccessValidation
{
    public interface IAccessValidationService
    {
        bool User(AccessGroup accessGroup);

        bool AccessGroup(AccessGroup accessGroup);

        bool Client(AccessGroup accessGroup);

        bool QuoteEvaluation(AccessGroup accessGroup);

        bool Requirement(AccessGroup accessGroup);

        bool Solution(AccessGroup accessGroup);

        bool Estimation(AccessGroup accessGroup);
    }
}
