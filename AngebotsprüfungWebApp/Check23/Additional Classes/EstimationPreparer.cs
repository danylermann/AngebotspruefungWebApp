using Check23.Models;

namespace Check23.Additional_Classes
{
    public class EstimationPreparer
    {
        private readonly Dictionary<string, string> TimeFrameMap = new Dictionary<string, string>()
        {
            { "h", "Stunde/n" },
            { "d", "Tag/e" },
            { "w", "Woche/n" },
            { "m", "Monat/e" }
        };
        private string MapTimeFrame(string timeFrame)
        {
            return  TimeFrameMap.TryGetValue(timeFrame, out string value) ? value : string.Empty;
        }
        public (Estimation resultingEstimation, string timeFrameESW, string timeFrameEHW, string timeFrameDocumentation, string timeFrameService) SetupEstimationForView(Estimation estimation)
        {
            Estimation resultingEstimation = estimation;
            string timeFrameESW = String.Empty;
            string timeFrameEHW = String.Empty;
            string timeFrameDocumentation = String.Empty;
            string timeFrameService = String.Empty;
            if (!String.IsNullOrEmpty(estimation.EHW_time))
            {
                timeFrameEHW = MapTimeFrame(estimation.EHW_time.Substring(estimation.EHW_time.Length - 1));
                resultingEstimation.EHW_time = estimation.EHW_time.Substring(0, estimation.EHW_time.Length - 1);
            }
            if (!String.IsNullOrEmpty(estimation.ESW_time))
            {
                timeFrameESW = MapTimeFrame(estimation.ESW_time.Substring(estimation.ESW_time.Length - 1));
                resultingEstimation.ESW_time = estimation.ESW_time.Substring(0, estimation.ESW_time.Length - 1);
            }
            if (!String.IsNullOrEmpty(estimation.Documentation_time))
            {
                timeFrameDocumentation = MapTimeFrame(estimation.Documentation_time.Substring(estimation.Documentation_time.Length - 1));
                resultingEstimation.Documentation_time = estimation.Documentation_time.Substring(0, estimation.Documentation_time.Length - 1);
            }
            if (!String.IsNullOrEmpty(estimation.Service_time))
            {
                timeFrameService = MapTimeFrame(estimation.Service_time.Substring(estimation.Service_time.Length - 1));
                resultingEstimation.Service_time = estimation.Service_time.Substring(0, estimation.Service_time.Length - 1);
            }

            return (resultingEstimation, timeFrameESW, timeFrameEHW, timeFrameDocumentation, timeFrameService);
        }
    }
}
