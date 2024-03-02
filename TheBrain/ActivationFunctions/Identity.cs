namespace TheBrain.ActivationFunctions;

public class Identity : IActivationFunction
{
    public ActivationFunctionType Type => ActivationFunctionType.Identity;

    public double Activate(double x)
    {
        return x;
    }
}