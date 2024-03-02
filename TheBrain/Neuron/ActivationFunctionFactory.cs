using TheBrain.ActivationFunctions;

namespace TheBrain.Neuron
{
    internal class ActivationFunctionFactory
    {
        internal IActivationFunction GetActivationFunction(ActivationFunctionType aft) => aft switch
        {
            ActivationFunctionType.Sigmoid => new Sigmoid(),
            ActivationFunctionType.ReLu => new ReLU(),
            ActivationFunctionType.LeakyReLu => new LeakyReLU(),
            ActivationFunctionType.BinaryStep => new BinaryStep(),
            ActivationFunctionType.BinaryStepWithBias => new BinaryStepWithBias(),
            ActivationFunctionType.Identity => new Identity(),
            ActivationFunctionType.TanH => new TanH(),
            ActivationFunctionType.Swish => new Swish(),
            _ => throw new System.NotImplementedException()
        };
    }
}