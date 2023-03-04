using roombaBrain.Rendering;
using roombaBrain.RoomSim;
using static SDL2.SDL;

namespace roombaBrain;

internal static class Program
{
    public static SDLRenderer? Renderer;


    public static void Main(string[] args)
    {
        Renderer = new SDLRenderer(HandleEvents, Render, 512, 512);
        // testAgent = new Agent(new Vector2(1, 1));
        Renderer.Run(60);
        Renderer.Dispose();
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
}