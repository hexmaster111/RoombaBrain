namespace TheBrain.ActivationFunctions;

public class Sigmoid : IActivationFunction
{
    public ActivationFunctionType Type => ActivationFunctionType.Sigmoid;

    public double Activate(double x)
    {
        return 1 / (1 + Math.Exp(-x));
    }
}