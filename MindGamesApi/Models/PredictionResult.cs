using System.Collections.Generic;

namespace MindGamesApi.Models
{
    public class PredictionResult
    {
        public List<CwtResult> CwtResults { get; set; }

        public EyesClosedPrediction EyesClosedPrediction { get; set; }
    }
}
