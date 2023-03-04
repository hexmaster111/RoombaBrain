namespace roombaBrain.NeuralNetwork.ActivationFunctions;

public class Swish : IActivationFunction
{
    public ActivationFunctionType Type => ActivationFunctionType.Swish;

    public double Activate(double x)
    {
        return x / (1 + Math.Pow(Math.E, -x));
    }
}