using RoombaBrain.Rendering;
using SDL2;

namespace RoombaBrain.RoomSim;

public struct Room
{
    public float Width_ft;
    public float Height_ft;
    public List<Dirt> Dirts;
    public Obstacle[] Obstacles;
    public Obstacle[] Walls;

    public static Room GenerateTestRoom(int width_ft, int height_ft)
    {
        ObstacleGenerator.GenerateChairLegs(5, 5, out var legs);
        ObstacleGenerator.GenerateWalls(width_ft, height_ft, 0.5f, out var walls);

        List<Obstacle> obstacles = new List<Obstacle>();
        obstacles.AddRange(legs);
        obstacles.AddRange(walls);
        Dirt.GenerateDirts(1000, obstacles.ToArray(), width_ft, height_ft, out var dirts);

        return new Room()
        {
            Width_ft = width_ft,
            Height_ft = height_ft,
            Dirts = dirts.ToList(),
            Obstacles = legs,
            Walls = walls
        };
    }


    private static readonly SDL.SDL_Color floorColor = new SDL.SDL_Color()
    {
        r = 0x80,
        g = 0x40,
        b = 0x00,
        a = 0xFF
    };


    public void Render(RenderArgs args, float pixelsPerFt)
    {

        SDL.SDL_SetRenderDrawColor(args.Renderer, floorColor.r, floorColor.g, floorColor.b, floorColor.a);
        SDL.SDL_Rect floorRect = new SDL.SDL_Rect()
        {
            x = 0,
            y = 0,
            w = args.ScreenWidth_Px,
            h = args.ScreenHeight_px
        };

        SDL.SDL_RenderFillRect(args.Renderer, ref floorRect);

        foreach (var obstacle in Obstacles)
        {
            Obstacle.RenderObstacle(obstacle, args, pixelsPerFt);
        }

        foreach (var wall in Walls)
        {
            Obstacle.RenderWall(wall, args, pixelsPerFt);
        }

        foreach (var dirt in Dirts)
        {
            Dirt.RenderDirt(dirt, args, pixelsPerFt);
        }
    }
}