namespace roombaBrain.NeuralNetwork.ActivationFunctions;

public class BinaryStepWithBias : IActivationFunction
{
    public ActivationFunctionType Type => ActivationFunctionType.BinaryStepWithBias;

    public double Activate(double x)
    {
        return x > 0.5 ? 1 : 0;
    }
}