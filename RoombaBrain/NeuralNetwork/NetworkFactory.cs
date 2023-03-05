using roombaBrain.NeuralNetwork.ActivationFunctions;

namespace roombaBrain.NeuralNetwork;

public static class NetworkFactory
{
    public static void BuildRandom
    (
        int inputCount,
        int outputCount,
        int maxHiddenLayerCount,
        out NetworkDto generatedNetwork)
    {
        if (maxHiddenLayerCount < 4)
            throw new ArgumentException("maxHiddenLayerCount must be at least 4");

        if (inputCount < 1)
            throw new ArgumentException("inputCount must be at least 1");

        if (outputCount < 1)
            throw new ArgumentException("outputCount must be at least 1");


        NetworkFactory.BuildRandom(
            inputCount,
            outputCount,
            maxHiddenLayerCount,
            4,
            10,
            5,
            true,
            null,
            out generatedNetwork
        );
    }

    public static void BuildRandom
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