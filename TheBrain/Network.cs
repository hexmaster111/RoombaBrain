using System.Diagnostics;
using TheBrain.ActivationFunctions;
using TheBrain.Neuron;

namespace TheBrain;

public class Network
{
    private readonly List<Layer> _layers;

    public Network(Layer inputLayer, IEnumerable<Layer> hiddenLayers, Layer outputLayer)
    {
        _layers = new List<Layer> { inputLayer };
        _layers.AddRange(hiddenLayers);
        _layers.Add(outputLayer);
    }

    public double[] RunNetwork(double[] inputValues)
    {
        var inputLayer = _layers.First();
        var hiddenLayers = _layers.Skip(1).Take(_layers.Count - 2);
        var outputLayer = _layers.Last();

        Debug.Assert(inputLayer.Neurons.Count == inputValues.Length);

        for (int i = 0; i < inputLayer.Neurons.Count; i++)
        {
            inputLayer.Neurons[i].Inputs = new double[] { inputValues[i] };
        }

        inputLayer.Neurons.ForEach(x => x.CalculateOutput());


        foreach (var layer in hiddenLayers)
        {
            foreach (var neuron in layer.Neurons)
            {
                neuron.Inputs = _layers[_layers.IndexOf(layer) - 1]
                    .Neurons.Select(x => x.Output).ToArray();
            }

            layer.Neurons.ForEach(x => x.CalculateOutput());
        }

        foreach (var neuron in outputLayer.Neurons)
        {
            neuron.Inputs = _layers[^2].Neurons
                .Select(x => x.Output)
                .ToArray();
        }

        outputLayer.Neurons.ForEach(x => x.CalculateOutput());

        return outputLayer.Neurons.Select(x => x.Output).ToArray();
    }

    public NetworkDto ToDto()
    {
        var layers = new List<LayerDto>();

        for (int i = 0; i < _layers.Count; i++)
        {
            var layer = _layers[i];
            var neurons = new List<NeruonDto>();

            for (int j = 0; j < layer.Neurons.Count; j++)
            {
                var neuron = layer.Neurons[j];
                neurons.Add(new NeruonDto(neuron.Weights, neuron.Bias, neuron.ActivationFunction.Type));
            }

            layers.Add(new LayerDto(neurons.ToArray(), layer.LayerType));
        }

        return new NetworkDto(layers.ToArray());
    }

    public static Network New(NetworkDto dto)
    {
        var layers = new List<Layer>();

        for (int i = 0; i < dto.Layers.Length; i++)
        {
            var layerDto = dto.Layers[i];
            var neurons = new List<INeuron>();

            for (int j = 0; j < layerDto.Neurons.Length; j++)
            {
                var neuronDto = layerDto.Neurons[j];
                neurons.Add(new HiddenNeuron(neuronDto.Weights, neuronDto.Bias, neuronDto.ActivationFunction));
            }

            layers.Add(new Layer(neurons, layerDto.LayerType));
        }

        return new Network(layers.First(), layers.Skip(1).Take(layers.Count - 2), layers.Last());
    }
}

public class NeruonDto
{
    public NeruonDto(double[] weights, double bias, ActivationFunctionType activationFunction)
    {
        Weights = weights;
        Bias = bias;
        ActivationFunction = activationFunction;
    }

    public double[] Weights { get; set; }
    public double Bias { get; set; }
    public ActivationFunctionType ActivationFunction { get; set; }
}

public struct LayerDto
{
    public LayerDto(NeruonDto[] neurons, LayerType layerType)
    {
        Neurons = neurons;
        LayerType = layerType;
    }

    public NeruonDto[] Neurons;
    public LayerType LayerType;
}

public class NetworkDto
{
    public NetworkDto(LayerDto[] layers)
    {
        Layers = layers;
    }

    public LayerDto[] Layers;
}