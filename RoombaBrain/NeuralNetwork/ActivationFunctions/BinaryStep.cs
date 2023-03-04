

namespace roombaBrain.NeuralNetwork.ActivationFunctions;

public class BinaryStep : IActivationFunction
{
    public ActivationFunctionType Type => ActivationFunctionType.BinaryStep;

    public double Activate(double x)
    {
        return x > 0 ? 1 : 0;
    }
}