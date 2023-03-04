namespace roombaBrain.NeuralNetwork.ActivationFunctions;

public class LeakyReLU : IActivationFunction
{
    public ActivationFunctionType Type => ActivationFunctionType.LeakyReLu;

    public double Activate(double x)
    {
        return Math.Max(0.01 * x, x);
    }
}