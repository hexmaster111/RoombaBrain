namespace roombaBrain.NeuralNetwork.ActivationFunctions;

public class TanH : IActivationFunction
{
    public ActivationFunctionType Type => ActivationFunctionType.TanH;

    public double Activate(double x)
    {
        return Math.Tanh(x);
    }
}