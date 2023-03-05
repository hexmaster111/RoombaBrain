using System.Diagnostics;
using roombaBrain.NeuralNetwork;

namespace roombaBrain;

public readonly struct AgentNetworkWrapper
{
    private readonly Network _network;

    public AgentNetworkWrapper(NetworkDto network)
    {
        //Verify the network takes in and provides the correct amount of inputs and outputs
        if (network.Layers[0].Neurons.Length != Agent.NumInputs)
            throw new ArgumentException("Network does not have the correct number of inputs");

        if (network.Layers[^1].Neurons.Length != Agent.NumOutputs)
            throw new ArgumentException("Network does not have the correct number of outputs");

        _network = Network.New(network);
    }

    public Agent.NetworkOutputCommand Run(Agent.NetworkInput input)
    {
        var inputs = new double[Agent.NumInputs];
        inputs[0] = input.distanceToLeftObstetricalOrWall;
        inputs[1] = input.distanceToRightObstetricalOrWall;
        inputs[2] = input.distanceToTopObstetricalOrWall;
        inputs[3] = input.distanceToBottemObstetricalOrWall;
        inputs[4] = input.dirtDetector;
        var output = _network.RunNetwork(inputs);
        return new Agent.NetworkOutputCommand(
            (float)output[0],
            (float)output[1],
            (float)output[2],
            (float)output[3]
        );
    }
}