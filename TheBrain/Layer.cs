using TheBrain.Neuron;

namespace TheBrain;

public class Layer
{
    public List<INeuron> Neurons { get; set; }
    public LayerType LayerType { get; set; }

    public Layer(List<INeuron> neurons, LayerType layerType)
    {
        Neurons = neurons;
        LayerType = layerType;
    }
}

public enum LayerType
{
    Input,
    Hidden,
    Output
}