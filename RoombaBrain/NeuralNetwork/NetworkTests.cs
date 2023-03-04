using System.Diagnostics;
using roombaBrain.NeuralNetwork.ActivationFunctions;
using roombaBrain.NeuralNetwork.Neuron;

namespace roombaBrain.NeuralNetwork;

public class NetworkTests
{
    public static void TestNetwork()
    {
        var bias = 1;

        var inputLayer = new Layer(new List<INeuron>
        {
            new InputNeuron(new Sigmoid(), new[] { 1.00 }, bias),
        }, LayerType.Input);


        var hiddenLayerOne = new Layer(new List<INeuron>
        {
            new HiddenNeuron(new Sigmoid(), new[] { 2.00 }, bias),
            new HiddenNeuron(new Sigmoid(), new[] { 3.00 }, bias),
            new HiddenNeuron(new Sigmoid(), new[] { 4.00 }, bias),
        }, LayerType.Hidden);

        var hiddenLayerTwo = new Layer(new List<INeuron>
        {
            new HiddenNeuron(new Sigmoid(), new[] { 5.00, 6.00, 7.00 }, bias),
            new HiddenNeuron(new Sigmoid(), new[] { 8.00, 9.00, 10.00 }, bias),
            new HiddenNeuron(new Sigmoid(), new[] { 11.00, 12.00, 13.00 }, bias),
        }, LayerType.Hidden);

        var outputLayer = new Layer(new List<INeuron>
        {
            new OutputNeuron(new Sigmoid(), new[] { 14.00, 15.00, 16.00 }, bias),
            new OutputNeuron(new Sigmoid(), new[] { 16.00, 17.00, 18.00 }, bias),
        }, LayerType.Output);

        var network = new Network(inputLayer,
            new List<Layer> { hiddenLayerOne, hiddenLayerTwo },
            outputLayer);

        var result = network.RunNetwork(new double[] { .5 });
        Debug.Assert(result.Length == outputLayer.Neurons.Count);

        var networkDto = network.ToDto();

        var networkFromDto = Network.New(networkDto);
        var resultFromDto = networkFromDto.RunNetwork(new double[] { .5 });

        Debug.Assert(resultFromDto.Length == outputLayer.Neurons.Count);
        Debug.Assert(resultFromDto[0] == result[0]);
        Debug.Assert(resultFromDto[1] == result[1]);
    }
}
