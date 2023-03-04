namespace roombaBrain.NeuralNetwork.ActivationFunctions;

public interface IActivationFunction
{
    double Activate(double x);
    ActivationFunctionType Type { get; }
}

public enum ActivationFunctionType
{
    Sigmoid,
    ReLu,
    LeakyReLu,
    BinaryStep,
    BinaryStepWithBias,
    Identity,
    TanH,
    Swish
}