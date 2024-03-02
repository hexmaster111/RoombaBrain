using System.Numerics;
using RoombaBrain.Rendering;
using RoombaBrain.RoomSim;
using TheBrain;
using static SDL2.SDL;

namespace RoombaBrain;

public class Agent
{
    private const float Raidus_ft = 0.557291667f;

    private Circle agentCircle;
    private int Score = 0;
    AgentNetworkWrapper _networkWrapper;

    private static readonly SDL_Color color = new SDL_Color()
    {
        r = 0,
        g = 255,
        b = 0,
        a = 255
    };

    public Agent(Vector2 centerPoint, NetworkDto network)
    {
        _networkWrapper = new AgentNetworkWrapper(network);
        agentCircle = new Circle(centerPoint, Raidus_ft);
    }

    const float speedInFtPerSec = 0.918635f;
    const float speedInFtPerMs = speedInFtPerSec / 1000.00f;
    private Room? lastState;

    public void Update(double deltaTime, Room state)
    {
        var input = GetNetworkInput(state, lastState);
        var command = _networkWrapper.Run(input);

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

        var dirtToRemove = new List<Dirt>();
        foreach (var dirt in state.Dirts.Where(dirt => agentCircle.Contains(dirt.Position)))
        {
            Score++;
            dirtToRemove.Add(dirt);
        }

        dirtToRemove.ForEach(dirt => state.Dirts.Remove(dirt));
        lastState = state;
    }


    public void Render(RenderArgs args, float pixelsPerFt)
    {
        agentCircle.Render(args, pixelsPerFt, color);
    }

    public const int NumInputs = 5;
    public const int NumOutputs = 4;

    public record NetworkInput(
        float distanceToLeftObstetricalOrWall,
        float distanceToRightObstetricalOrWall,
        float distanceToTopObstetricalOrWall,
        float distanceToBottemObstetricalOrWall,
        float dirtDetector
    );

    public record NetworkOutputCommand(float up, float down, float left, float right);


    private NetworkInput GetNetworkInput(Room state, Room? previousState)
    {
        float distanceToLeftObstetricalOrWall = 0;
        float distanceToRightObstetricalOrWall = 0;
        float distanceToTopObstetricalOrWall = 0;
        float distanceToBottemObstetricalOrWall = 0;
        float dirtDetector = 0;

        return new NetworkInput(
            distanceToLeftObstetricalOrWall,
            distanceToRightObstetricalOrWall,
            distanceToTopObstetricalOrWall,
            distanceToBottemObstetricalOrWall,
            dirtDetector
        );
    }
}