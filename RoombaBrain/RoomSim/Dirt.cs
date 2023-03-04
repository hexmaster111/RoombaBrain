using System.Numerics;
using roombaBrain.Rendering;
using SDL2;

namespace roombaBrain.RoomSim;

public struct Dirt
{

    public Vector2 Position;
    public byte Dirtyness;

    private const float dirtSize_ft = .08f;

    private static readonly SDL.SDL_Color dirtColor = new SDL.SDL_Color()
    {
        r = 0x00,
        g = 0xFF,
        b = 0x00,
        a = 0xFF
    };

    public static void RenderDirt(Dirt dirt, RenderArgs args, float pixelsPerFt)
    {
        SDL.SDL_SetRenderDrawColor(args.Renderer, dirtColor.r, dirtColor.g, dirtColor.b, dirtColor.a);
        SDL.SDL_Rect dirtRect = new SDL.SDL_Rect()
        {
            x = (int)(dirt.Position.X * pixelsPerFt),
            y = (int)(dirt.Position.Y * pixelsPerFt),
            w = (int)(dirtSize_ft * pixelsPerFt),
            h = (int)(dirtSize_ft * pixelsPerFt)
        };

        SDL.SDL_RenderFillRect(args.Renderer, ref dirtRect);
    }

    public static void GenerateDirts
    (
        int count,
        Obstacle[] obstacles,
        float roomWidth_ft,
        float roomHeight_ft,
        out Dirt[] dirts
    )
    {
        dirts = new Dirt[count];
        float dirtSpacing_ft = (float)Math.Sqrt(roomWidth_ft * roomHeight_ft / count);
        int dirtCountX = (int)(roomWidth_ft / dirtSpacing_ft);
        int dirtCountY = (int)(roomHeight_ft / dirtSpacing_ft);
        for (int i = 0; i < dirtCountX; i++)
        {
            for (int j = 0; j < dirtCountY; j++)
            {
                dirts[i * dirtCountY + j] = new Dirt()
                {
                    Position = new Vector2(i * dirtSpacing_ft, j * dirtSpacing_ft),
                    Dirtyness = 128
                };
            }
        }
        for (int i = 0; i < dirts.Length; i++)
        {
            if (IsPositionInObstacle(dirts[i].Position, obstacles))
            {
                dirts[i] = dirts[dirts.Length - 1];
                Array.Resize(ref dirts, dirts.Length - 1);
                i--;
            }
        }
    }

    private static bool IsPositionInObstacle(Vector2 position, Obstacle[] obstacles)
    {
        foreach (var obstacle in obstacles)
        {
            if (position.X >= obstacle.Position.X && position.X <= obstacle.Position.X + obstacle.Width_ft &&
                position.Y >= obstacle.Position.Y && position.Y <= obstacle.Position.Y + obstacle.Height_ft)
            {
                return true;
            }
        }
        return false;
    }
}