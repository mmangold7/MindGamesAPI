using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace MindGamesApi.Training.Trainer;

public class LbfgsMaximumEntropyTrainer : TrainerBase<MaximumEntropyModelParameters>
{
    public LbfgsMaximumEntropyTrainer(string modelName) : base($"{modelName}_{nameof(LbfgsMaximumEntropyTrainer)}")
    {
        this.Name = "LBFGS Maximum Entropy";
        this._model = this.MlContext.MulticlassClassification.Trainers
                          .LbfgsMaximumEntropy();
    }
}

public class NaiveBayesTrainer : TrainerBase<NaiveBayesMulticlassModelParameters>
{
    public NaiveBayesTrainer(string modelName) : base($"{modelName}_{nameof(NaiveBayesTrainer)}")
    {
        this.Name = "Naive Bayes";
        this._model = this.MlContext.MulticlassClassification.Trainers
                          .NaiveBayes();
    }
}

public class OneVersusAllTrainer : TrainerBase<OneVersusAllModelParameters>
{
    public OneVersusAllTrainer(string modelName) : base($"{modelName}_{nameof(OneVersusAllTrainer)}")
    {
        this.Name = "One Versus All";
        this._model = this.MlContext.MulticlassClassification.Trainers
                          .OneVersusAll(this.MlContext.BinaryClassification.Trainers.SgdCalibrated());
    }
}

public class SdcaMaximumEntropyTrainer : TrainerBase<MaximumEntropyModelParameters>
{
    public SdcaMaximumEntropyTrainer(string modelName) : base($"{modelName}_{nameof(SdcaMaximumEntropyTrainer)}")
    {
        this.Name = "Sdca Maximum Entropy";
        this._model = this.MlContext.MulticlassClassification.Trainers
                          .SdcaMaximumEntropy();
    }
}

public class SdcaNonCalibratedTrainer : TrainerBase<LinearMulticlassModelParameters>
{
    public SdcaNonCalibratedTrainer(string modelName) : base($"{modelName}_{nameof(SdcaNonCalibratedTrainer)}")
    {
        this.Name = "Sdca NonCalibrated";
        this._model = this.MlContext.MulticlassClassification.Trainers
                          .SdcaNonCalibrated();
    }
}
