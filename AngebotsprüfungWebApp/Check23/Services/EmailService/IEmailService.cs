using Check23.Models;

namespace Check23.Services.EmailService
{
    public interface IEmailService
    {
        void SendEmail(string subject, string body, HashSet<string> emailaddresses);
        List<int> GetAreaOfResponsibilityIdsByRequirement(Requirement requirement);
    }
}
