using System.Numerics;
using roombaBrain.NeuralNetwork;
using roombaBrain.Rendering;
using roombaBrain.RoomSim;
using static SDL2.SDL;

namespace roombaBrain;

internal static class Program
{
    private static Room testRoom = Room.GenerateTestRoom(10, 10);
    private static Agent testAgent;
    public static SDLRenderer? Renderer;


    const int maxHiddenLayers = 4;
    const int maxRoomSize = 20;
    const int minRoomSize = 10;


    public static void Main(string[] args)
    {
        NetworkFactory.BuildRandom(Agent.NumInputs, Agent.NumOutputs, maxHiddenLayers, out var network);
        Renderer = new SDLRenderer(HandleEvents, Render, 512, 512);
        testAgent = new Agent(new Vector2(2, 2), network);
        Renderer.Run(60);
        Renderer.Dispose();
    }

    private static void HandleEvents(SDL_Event e)
    {
        switch (e.type)
        {
            case SDL_EventType.SDL_KEYDOWN when e.key.keysym.sym == SDL_Keycode.SDLK_r:
            {
                var size = Random.Shared.Next(minRoomSize, maxRoomSize);
                testRoom = Room.GenerateTestRoom(size, size);
                NetworkFactory.BuildRandom(Agent.NumInputs, Agent.NumOutputs, maxHiddenLayers, out var network);
                testAgent = new Agent(new Vector2(2, 2), network);
                break;
            }
        }
    }

    private static void Render(RenderArgs args)
    {
        float pixelsPerFt = args.ScreenWidth_Px / testRoom.Width_ft;
        testAgent.Update(args.DeltaTime, testRoom);
        testRoom.Render(args, pixelsPerFt);
        testAgent.Render(args, pixelsPerFt);
    }
}