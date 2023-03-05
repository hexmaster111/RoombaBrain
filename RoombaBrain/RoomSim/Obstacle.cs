using System.Numerics;
using roombaBrain.Rendering;
using SDL2;

namespace roombaBrain.RoomSim;

public struct Obstacle
{
    public Vector2 Position;
    public double Width_ft;
    public double Height_ft;


    private static readonly SDL.SDL_Color obstacleColor = new SDL.SDL_Color()
    {
        r = 0x00,
        g = 0x00,
        b = 0x00,
        a = 0xFF
    };

    private static readonly SDL.SDL_Color wallColor = new SDL.SDL_Color()
    {
        r = 0x80,
        g = 0x80,
        b = 0x80,
        a = 0xFF
    };

    public static void RenderWall(Obstacle wall, RenderArgs args, float pixelsPerFt)
    {
        RenderObstacle(wall, args, pixelsPerFt, wallColor);
    }

    public static void RenderObstacle(Obstacle obstacle, RenderArgs args, float pixelsPerFt)
    {
        RenderObstacle(obstacle, args, pixelsPerFt, obstacleColor);
    }

    private static void RenderObstacle(Obstacle obstacle, RenderArgs args, float pixelsPerFt, SDL.SDL_Color color)
    {
        SDL.SDL_SetRenderDrawColor(args.Renderer, color.r, color.g, color.b, color.a);
        SDL.SDL_Rect wallRect = new SDL.SDL_Rect()
        {
            x = (int)(obstacle.Position.X * pixelsPerFt),
            y = (int)(obstacle.Position.Y * pixelsPerFt),
            w = (int)(obstacle.Width_ft * pixelsPerFt),
            h = (int)(obstacle.Height_ft * pixelsPerFt)
        };

        SDL.SDL_RenderFillRect(args.Renderer, ref wallRect);
    }
    
}