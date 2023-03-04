using roombaBrain.NeuralNetwork.ActivationFunctions;

namespace roombaBrain.NeuralNetwork.Neuron;


public interface INeuron
{
    IActivationFunction ActivationFunction { get; set; }
    double[] Inputs { get; set; }
    double[] Weights { get; set; }
    double Output { get; set; }
    double Bias { get; set; }
    double CalculateOutput();
}
