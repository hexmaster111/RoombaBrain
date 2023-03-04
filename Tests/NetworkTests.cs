using roombaBrain.NeuralNetwork;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }


    private void TestNetworkForValidate(in NetworkDto nDto, out Network builtNetwork)
    {
        //Verify all of the neurons have as many weights as the previous layer has neurons
        for (var i = 0; i < nDto.Layers.Length; i++)
        {
            var layer = nDto.Layers[i];
            for (var j = 0; j < layer.Neurons.Length; j++)
            {
                //The first layer [0] is the input layer, so it doesn't have a previous layer, but dose have one weight per input
                var neuron = layer.Neurons[j];
                if (i == 0)
                {
                    if (neuron.Weights.Length != 1)
                    {
                        Assert.Fail($"Neuron {j} in layer {i} has {neuron.Weights.Length} weights, but is the " +
                                    $"input layer and should have 1");
                    }

                    continue;
                }

                var inputLayer = nDto.Layers[i - 1];
                if (neuron.Weights.Length == inputLayer.Neurons.Length) continue;
                Assert.Fail($"Neuron {j} in layer {i} has {neuron.Weights.Length} weights," +
                            $" but the previous layer has {inputLayer.Neurons.Length} neurons");
            }
        }

        builtNetwork = Network.New(nDto);

        double[] lastOutput = Array.Empty<double>();

        //verify changing the input changes the output
        for (var i = 0; i < 100; i++)
        {
            var testInput = new double[nDto.Layers[0].Neurons.Length];
            for (var j = 0; j < testInput.Length; j++)
            {
                testInput[j] = 1;
            }

            var testOutput = builtNetwork.RunNetwork(testInput);
            if (lastOutput != Array.Empty<double>() && testOutput == lastOutput)
            {
                Assert.Fail("Network output is not changing with different input");
            }

            lastOutput = testOutput;
        }
    }


    private void TestNetworkTurnsIntoDtoCorrectly(NetworkDto generatedNetwork, Network builtNetwork)
    {
        var convertedBackDto = builtNetwork.ToDto();
        Assert.AreEqual(generatedNetwork.Layers.Length, convertedBackDto.Layers.Length);
        for (var i = 0; i < generatedNetwork.Layers.Length; i++)
        {
            var layer = generatedNetwork.Layers[i];
            var convertedLayer = convertedBackDto.Layers[i];
            Assert.AreEqual(layer.Neurons.Length, convertedLayer.Neurons.Length);
            for (var j = 0; j < layer.Neurons.Length; j++)
            {
                var neuron = layer.Neurons[j];
                var convertedNeuron = convertedLayer.Neurons[j];
                Assert.AreEqual(neuron.Weights.Length, convertedNeuron.Weights.Length);
                for (var k = 0; k < neuron.Weights.Length; k++)
                {
                    Assert.AreEqual(neuron.Weights[k], convertedNeuron.Weights[k]);
                }
            }
        }
    }

    private void TestNetwork(int inputCount, int outputCount, int maxHiddenLayerCount)
    {
        RandomNetworkFactory.CreateRandomNetwork(
            inputCount,
            outputCount,
            maxHiddenLayerCount,
            4,
            20,
            5,
            true,
            null,
            out var generatedNetwork
        );
        TestNetworkForValidate(generatedNetwork, out var liveNetwork);
        TestNetworkTurnsIntoDtoCorrectly(generatedNetwork, liveNetwork);
    }


    [Test]
    public void CreateAndRunRandomNetworks()
    {
        const int testsToRun = 10_000;

        var tasks = new Task[testsToRun];
        for (int i = 0; i < testsToRun; i++)
        {
            int inputCount = Random.Shared.Next(1, 100);
            int outputCount = Random.Shared.Next(1, 100);
            int maxHiddenLayerCount = Random.Shared.Next(4, 10);
            tasks[i] = (Task.Run(() => TestNetwork(inputCount, outputCount, maxHiddenLayerCount)));
        }

        Task.WaitAll(tasks);
        Assert.Pass();
    }
}