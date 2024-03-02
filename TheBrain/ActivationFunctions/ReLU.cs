namespace TheBrain.ActivationFunctions;

public class ReLU : IActivationFunction
{
    public ActivationFunctionType Type => ActivationFunctionType.ReLu;

    public double Activate(double x)
    {
        return Math.Max(0, x);
    }
}