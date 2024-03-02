using System.Collections;
using System.Threading.Tasks.Sources;
using Newtonsoft.Json;
using TheBrain;
using TheBrain.ActivationFunctions;

namespace WrongCalcualtor;

static class Program
{
    private const double ActivatedValue = 0.0;
    private static readonly int MaxActivationFunction = (int)Enum.GetValues<ActivationFunctionType>().Max();

    static int Setbit(int intValue, int position) => intValue |= 1 << position;

    static int GetNetworkOutput(double[] v)
    {
        int a = 0b00000;

        for (int i = 0; i < 4; i++)
        {
            var d = v[i];

            int bitV = d > ActivatedValue ? 1 : 0;
            if (bitV == 1) a = Setbit(a, i);
        }

        return a;
    }

    static MiniNetworkStats RunItterations(int its, NetworkDto starting, double bestScore = 0)
    {
        var bestNet = starting;

        int networkCount = 0;

        List<SingleNetworkOutputStatistic> bestOuts = new();

        var currNetwork = starting;
        var score = 0.0;

        do
        {
            var network = Network.New(currNetwork);
            networkCount++;
            List<SingleNetworkOutputStatistic> outs = new();

            score = 0;


            for (int i = 0; i < 128; i++)
            for (int j = 0; j < 128; j++)
            {
                var res = network.RunNetwork(GetInputs(i, j));
                var correctAns = i + j;
                var netRes = GetNetworkOutput(res);
                outs.Add(new SingleNetworkOutputStatistic(i, j, netRes, correctAns));
            }

            foreach (var l in outs)
            {
                score += 1.0 / (Math.Abs(l.iValue + l.jValue - l.networkOutput) + 1);
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestNet = currNetwork;
                bestOuts = outs;
                Console.WriteLine($"Curr Best Score: {bestScore:0.000} @ itter {networkCount}");
            }

            currNetwork = EvolveToNext(bestNet);
        } while (networkCount <= its);

        return new MiniNetworkStats(bestScore, bestNet, bestOuts);
    }

    static Task<MiniNetworkStats> RunIttersAsync(int its, NetworkDto starting, double bestScore = 0)
    {
        return Task.Run(() => RunItterations(its, starting, bestScore));
    }


    static void Main(string[] args)
    {
        var bestScore = 0.0;
        NetworkDto bestNetwork = NetworkFactory.BuildRandom(8, 5, 5);
        List<SingleNetworkOutputStatistic> bestOuts = new();
        const int agents = 16;
        const int agentItterations = 100;
        const int generations = 5;

        for (int generation = 0; generation < generations; generation++)
        {
            Task<MiniNetworkStats>[] tasks = new Task<MiniNetworkStats>[agents];
            for (int i = 0; i < agents; i++)
            {
                tasks[i] = RunIttersAsync(agentItterations, bestNetwork, bestScore);
            }


            Task.WaitAll(tasks);

            foreach (var res in tasks)
            {
                if (!res.IsCompletedSuccessfully) throw new Exception("why tho?!");
                if (res.Result.best > bestScore)
                {
                    bestScore = res.Result.best;
                    bestNetwork = res.Result.network;
                    bestOuts = res.Result.highlights;
                }
            }

            Console.WriteLine($"end generation {generation} best {bestScore}");
        }

        File.WriteAllText("best.json", JsonConvert.SerializeObject(bestNetwork, Formatting.Indented));
    }


    private static NetworkDto EvolveToNext(NetworkDto currBestNetwork)
    {
        var r = new Random();
        foreach (var l in currBestNetwork.Layers)
        {
            foreach (var n in l.Neurons)
            {
                var thingToTweak = r.Next(0, 3); //3 params we can tune, ActivationFunc, Gain, offsets

                switch (thingToTweak)
                {
                    case 0:
                        n.ActivationFunction =
                            (ActivationFunctionType)r.Next(0, MaxActivationFunction);
                        break;
                    case 1:
                        n.Bias = r.NextDouble(-1, 1);
                        break;
                    case 2:
                        for (var j = 0; j < n.Weights.Length; j++)
                        {
                            n.Weights[j] = Random.Shared.NextDouble(-1, 1);
                        }

                        break;
                }
            }
        }

        return currBestNetwork;
    }

    private static double[] GetInputs(int i, int j)
    {
        var res = new double[8];
        // Console.WriteLine(Convert.ToString(i, toBase: 2));
        // Console.WriteLine(Convert.ToString(j, toBase: 2));
        //NetworkFactory.BuildRandom(8, 5, 5);

        for (int k = 0; k < 3; k++)
        {
            if (IsBitSet((byte)j, k))
            {
                res[k] = 1.0;
            }
            else
            {
                res[k] = 0.0;
            }
        }

        for (int k = 4; k < 7; k++)
        {
            if (IsBitSet((byte)i, k - 3))
            {
                res[k] = 1.0;
            }
            else
            {
                res[k] = 0.0;
            }
        }

        return res;
    }

    static bool IsBitSet(byte b, int pos)
    {
        return (b & (1 << pos)) != 0;
    }
}

struct SingleNetworkOutputStatistic
{
    public int iValue, jValue, networkOutput, correctAns;

    public SingleNetworkOutputStatistic(int i, int i1, int netRes, int correctAns1)
    {
        iValue = i;
        jValue = i1;
        networkOutput = netRes;
        correctAns = correctAns1;
    }
}

struct MiniNetworkStats
{
    public MiniNetworkStats(double best, NetworkDto network, List<SingleNetworkOutputStatistic> highlights)
    {
        this.network = network;
        this.best = best;
        this.highlights = highlights;
    }

    public double best;
    public NetworkDto network;
    public List<SingleNetworkOutputStatistic> highlights;
}