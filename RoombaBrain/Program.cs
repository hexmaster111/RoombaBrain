using System.Diagnostics;
using System.Numerics;
using roombaBrain.NeuralNetwork;
using roombaBrain.NeuralNetwork.ActivationFunctions;
using roombaBrain.Rendering;
using roombaBrain.RoomSim;
using static SDL2.SDL;

namespace roombaBrain;

internal static class Program
{
    public static SDLRenderer? Renderer;


    public static void Main(string[] args)
    {
        const int testsToRun = 10_000;

        Task<(bool, string)>[] tasks = new Task<(bool, string)>[testsToRun];
        for (int i = 0; i < testsToRun; i++)
        {
            int inputCount = Random.Shared.Next(1, 100);
            int outputCount = Random.Shared.Next(1, 100);
            int maxHiddenLayerCount = Random.Shared.Next(4, 10);
            tasks[i] = (Task.Run(() => TestNetwork(inputCount, outputCount, maxHiddenLayerCount)));
        }

        Task.WaitAll(tasks);
        foreach (var task in tasks.Where(task => !task.Result.Item1))
        {
            Console.WriteLine(task.Result.Item2);
            Debugger.Break();
        }


        Renderer = new SDLRenderer(HandleEvents, Render, 512, 512);
        // testAgent = new Agent(new Vector2(1, 1));
        Renderer.Run(60);
        Renderer.Dispose();
    }

    public static void TestNetworkForValidity
    (
        in NetworkDto nDto,
        out bool isValid,
        out string errorMessage
    )
    {
        isValid = true;
        errorMessage = string.Empty;

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
                        isValid = false;
                        errorMessage =
                            $"Neuron {j} in layer {i} has {neuron.Weights.Length} weights, but is the input layer and should have 1";
                        return;
                    }

                    continue;
                }

                var inputLayer = nDto.Layers[i - 1];
                if (neuron.Weights.Length == inputLayer.Neurons.Length) continue;
                isValid = false;
                errorMessage = $"Neuron {j} in layer {i} has {neuron.Weights.Length} weights," +
                               $" but the previous layer has {inputLayer.Neurons.Length} neurons";
                return;
            }
        }

        var testNetwork = Network.New(nDto);

        double[] lastOutput = Array.Empty<double>();

        //verify changing the input changes the output
        for (var i = 0; i < 100; i++)
        {
            var testInput = new double[nDto.Layers[0].Neurons.Length];
            for (var j = 0; j < testInput.Length; j++)
            {
                testInput[j] = 1;
            }

            var testOutput = testNetwork.RunNetwork(testInput);
            if (lastOutput != Array.Empty<double>() && testOutput == lastOutput)
            {
                isValid = false;
                errorMessage = "Network output is not changing with different input";
            }

            lastOutput = testOutput;
        }
    }

    public static (bool, string) TestNetwork(int inputCount, int outputCount, int maxHiddenLayerCount)
    {
        RandomNetworkFactory(
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
        TestNetworkForValidity(generatedNetwork, out var isValid, out var errorMessage);
        return (isValid, errorMessage);
    }

    private static void HandleEvents(SDL_Event e)
    {
    }

    private static void Render(RenderArgs args)
    {
        float pixelsPerFt = args.ScreenWidth_Px / testRoom.Width_ft;
        testAgent.Update(args.DeltaTime, testRoom);
        testRoom.Render(args, pixelsPerFt);
        testAgent.Render(args, pixelsPerFt);
    }

    private static readonly Room testRoom = Room.GenerateTestRoom(10, 10);
    private static readonly Agent testAgent;

    public static void RandomNetworkFactory
    (
        int inputCount,
        int outputCount,
        int maxHiddenLayerCount,
        int minHiddenLayerCount,
        int maxNeuronsPerHiddenLayer,
        int minNeuronsPerHiddenLayer,
        bool useRandomActivationFunction,
        ActivationFunctionType? functionTypeToUse,
        out NetworkDto networkDto
    )
    {
        if (!useRandomActivationFunction && !functionTypeToUse.HasValue)
            throw new ArgumentException(
                "If useRandomActivationFunction is false, functionTypeToUse must be specified");


        const int inputNeuronsBiasMax = 1;
        const int inputNeuronsBiasMin = -1;
        const int inputNeuronsWeightMax = 1;
        const int inputNeuronsWeightMin = -1;
        const int hiddenNeuronsBiasMax = 1;
        const int hiddenNeuronsBiasMin = -1;
        const int hiddenNeuronsWeightMax = 1;
        const int hiddenNeuronsWeightMin = -1;
        const int outputNeuronsBiasMax = 1;
        const int outputNeuronsBiasMin = -1;
        const int outputNeuronsWeightMax = 1;
        const int outputNeuronsWeightMin = -1;


        #region Input Layer

        var inputNeurons = new NeruonDto[inputCount];
        for (var i = 0; i < inputCount; i++)
        {
            inputNeurons[i] = new NeruonDto()
            {
                Bias = Random.Shared.NextDouble(inputNeuronsBiasMin, inputNeuronsBiasMax),
                Weights = new[] { Random.Shared.NextDouble(inputNeuronsWeightMin, inputNeuronsWeightMax) },
                ActivationFunction = useRandomActivationFunction
                    ? Random.Shared.NextActivationFunctionType()
                    : functionTypeToUse.Value
            };
        }

        #endregion

        #region Hidden Layers

        var hiddenLayerCount = Random.Shared.Next(minHiddenLayerCount, maxHiddenLayerCount + 1);
        var hiddenLayerSizes = new int[hiddenLayerCount];
        for (var i = 0; i < hiddenLayerCount; i++)
        {
            hiddenLayerSizes[i] = Random.Shared.Next(minNeuronsPerHiddenLayer, maxNeuronsPerHiddenLayer + 1);
        }

        var hiddenLayers = new NeruonDto[hiddenLayerCount][];

        for (var i = 0; i < hiddenLayerCount; i++)
        {
            var hiddenLayerSize = hiddenLayerSizes[i];
            var hiddenLayer = new NeruonDto[hiddenLayerSize];
            for (var j = 0; j < hiddenLayerSize; j++)
            {
                var weights = i == 0 ? new double[inputCount] : new double[hiddenLayerSizes[i - 1]];
                for (var k = 0; k < weights.Length; k++)
                {
                    weights[k] = Random.Shared.NextDouble(hiddenNeuronsWeightMin, hiddenNeuronsWeightMax);
                }

                hiddenLayer[j] = new NeruonDto()
                {
                    Bias = Random.Shared.NextDouble(hiddenNeuronsBiasMin, hiddenNeuronsBiasMax),
                    Weights = weights,
                    ActivationFunction = useRandomActivationFunction
                        ? Random.Shared.NextActivationFunctionType()
                        : functionTypeToUse.Value
                };
            }

            hiddenLayers[i] = hiddenLayer;
        }

        #endregion

        #region Output Layer

        var outputNeurons = new NeruonDto[outputCount];
        var outputWeights = hiddenLayerCount == 0 ? new double[inputCount] : new double[hiddenLayerSizes[^1]];
        for (var i = 0; i < outputCount; i++)
        {
            for (var j = 0; j < outputWeights.Length; j++)
            {
                outputWeights[j] = Random.Shared.NextDouble(outputNeuronsWeightMin, outputNeuronsWeightMax);
            }

            outputNeurons[i] = new NeruonDto()
            {
                Bias = Random.Shared.NextDouble(outputNeuronsBiasMin, outputNeuronsBiasMax),
                Weights = outputWeights,
                ActivationFunction = useRandomActivationFunction
                    ? Random.Shared.NextActivationFunctionType()
                    : functionTypeToUse.Value
            };
        }

        #endregion

        List<LayerDto> layers = new();
        layers.Add(new LayerDto(inputNeurons, LayerType.Input));
        for (var i = 0; i < hiddenLayerCount; i++)
        {
            layers.Add(new LayerDto(hiddenLayers[i], LayerType.Hidden));
        }

        layers.Add(new LayerDto(outputNeurons, LayerType.Output));
        networkDto = new NetworkDto(layers.ToArray());
    }

    public static double NextDouble(this Random random, double min, double max)
    {
        return random.NextDouble() * (max - min) + min;
    }

    public static ActivationFunctionType NextActivationFunctionType(this Random random)
    {
        var values = Enum.GetValues(typeof(ActivationFunctionType));
        return (ActivationFunctionType)values.GetValue(random.Next(values.Length));
    }
}
