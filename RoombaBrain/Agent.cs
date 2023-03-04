using System.Numerics;
using roombaBrain.NeuralNetwork;
using roombaBrain.Rendering;
using roombaBrain.RoomSim;
using static SDL2.SDL;

namespace roombaBrain;

public class Agent
{
    private const float Raidus_ft = 0.557291667f;

    private Circle agentCircle;
    private int Score = 0;
    private Network _network;

    private static readonly SDL_Color color = new SDL_Color()
    {
        r = 0,
        g = 255,
        b = 0,
        a = 255
    };

    public Agent(Vector2 centerPoint, NetworkDto network)
    {
        _network = Network.New(network);
        agentCircle = new Circle(centerPoint, Raidus_ft);
    }

    const float speedInFtPerSec = 0.918635f;
    const float speedInFtPerMs = speedInFtPerSec / 1000.00f;

    public void Update(double deltaTime, Room state)
    {
        var input = GetNetworkInput(state);


        //This command is raw output from the neural network, and is not normalized
        var command = new NetworkOutputCommand(0, 0, 0, 0);

        command = new NetworkOutputCommand(999, 1000, 19, 20);

        float total = command.up + command.down + command.left + command.right;
        float up = command.up / total;
        float down = command.down / total;
        float left = command.left / total;
        float right = command.right / total;

        Vector2 moveVector = new Vector2(0, 0);
        moveVector += new Vector2(0, -1) * up;
        moveVector += new Vector2(0, 1) * down;
        moveVector += new Vector2(-1, 0) * left;
        moveVector += new Vector2(1, 0) * right;

        moveVector = Vector2.Normalize(moveVector);
        moveVector *= (float)(speedInFtPerMs * deltaTime);

        if (float.IsNaN(moveVector.X) || float.IsNaN(moveVector.Y))
        {
            moveVector = new Vector2(0, 0);
        }

        agentCircle.Center += moveVector;

        foreach (var wall in state.Walls)
        {
            if (agentCircle.Intersects(wall.Position, (float)wall.Width_ft, (float)wall.Height_ft))
            {
                agentCircle.Center -= moveVector;
            }
        }

        foreach (var obstacle in state.Obstacles)
        {
            if (agentCircle.Intersects(obstacle.Position, (float)obstacle.Width_ft, (float)obstacle.Height_ft))
            {
                agentCircle.Center -= moveVector;
            }
        }

        List<Dirt> dirtToRemove = new List<Dirt>();
        foreach (var dirt in state.Dirts)
        {
            if (agentCircle.Contains(dirt.Position))
            {
                Score++;
                dirtToRemove.Add(dirt);
            }
        }

        dirtToRemove.ForEach(dirt => state.Dirts.Remove(dirt));
    }


    public void Render(RenderArgs args, float pixelsPerFt)
    {
        agentCircle.Render(args, pixelsPerFt, color);
    }

    public record NetworkInput(
        float leftBumper,
        float rightBumper,
        float leftWallDistanceSensor,
        float centerWallDistanceSensor,
        float rightWallDistanceSensor,
        float dirtDetector
    );


    public record NetworkOutputCommand(float up, float down, float left, float right);

    private NetworkInput GetNetworkInput(Room state)
    {
        float leftBumper = 0;
        float rightBumper = 0;
        float leftWallDistanceSensor = 0;
        float centerWallDistanceSensor = 0;
        float rightWallDistanceSensor = 0;
        float dirtDetector = 0;

        return new NetworkInput(
            leftBumper,
            rightBumper,
            leftWallDistanceSensor,
            centerWallDistanceSensor,
            rightWallDistanceSensor,
            dirtDetector
        );
    }
}