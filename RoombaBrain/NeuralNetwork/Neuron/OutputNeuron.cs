using System.Diagnostics;
using roombaBrain.NeuralNetwork.ActivationFunctions;

namespace roombaBrain.NeuralNetwork.Neuron;

public class OutputNeuron : INeuron
{
    public IActivationFunction ActivationFunction { get; set; }

    public OutputNeuron(IActivationFunction activationFunction, double[] weights, double bias)
    {
        ActivationFunction = activationFunction;
        Weights = weights;
        Bias = bias;
    }

    public double[] Weights { get; set; } = Array.Empty<double>();
    public double[] Inputs { get; set; } = Array.Empty<double>();
    public double Bias { get; set; } = 0;
    public double Output { get; set; } = 0;

    public double CalculateOutput()
    {
        Debug.Assert(Inputs.Length == Weights.Length);

        double sum = 0;
        for (int i = 0; i < Inputs.Length; i++)
        {
            sum += Inputs[i] * Weights[i];
        }
        sum += Bias;
        Output = ActivationFunction.Activate(sum);
        return Output;
    }

}