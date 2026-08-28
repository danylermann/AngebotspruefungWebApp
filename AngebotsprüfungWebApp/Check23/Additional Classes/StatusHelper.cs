using Check23.Models;

namespace Check23.Additional_Classes
{
    public class StatusHelper
    {
        public int New { get; } = 1;
        public int FullyEstimated { get; } = 2;
        public int ApprovalGranted { get; } = 3;

        public bool EHWRequired(Requirement requirement)
        {
            if(requirement.EHW_Construction || requirement.EHW_HV_Tester || requirement.EHW_InterneTools || requirement.EHW_LV_Tester || requirement.EHW_Other || requirement.EHW_Other)
            {
                return true;
            }
            return false;
        }

        public bool ESWRequired(Requirement requirement)
        {
            if(requirement.ESW_CEETIS || requirement.ESW_InterneTools || requirement.ESW_IVISionStudio || requirement.ESW_Other || requirement.ESW_Netstar)
            {
                return true;
            }
            return false;
        }

        public bool DocumentationRequired(Requirement requirement)
        {
            if (requirement.Documentation)
            {
                return true;
            }
            return false;
        }

        public bool ServiceRequired(Requirement requirement)
        {
            if (requirement.Service)
            {
                return true;
            }
            return false;
        }
        public bool CheckFullyEstimated(Requirement requirement, Estimation estimation)
        {
            bool fullyEstimated = true;            

            if (EHWRequired(requirement) && string.IsNullOrEmpty(estimation.EHW_time))
            {
                fullyEstimated = false;
            }
            else if (ESWRequired(requirement) && string.IsNullOrEmpty(estimation.ESW_time))
            {
                fullyEstimated = false;
            }
            else if(DocumentationRequired(requirement) && string.IsNullOrEmpty(estimation.Documentation_time))
            {
                fullyEstimated = false;
            }
            //else if(cDERequired && string.IsNullOrEmpty(estimation.CDE_time))
            //{
            //    fullyEstimated = false;
            //}
            else if(ServiceRequired(requirement) && string.IsNullOrEmpty(estimation.Service_time))
            {
                fullyEstimated = false;
            }

            return fullyEstimated;
        }
    }
}
