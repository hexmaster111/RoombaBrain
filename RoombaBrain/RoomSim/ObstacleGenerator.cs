using System.Numerics;

namespace roombaBrain.RoomSim;

public static class ObstacleGenerator
{
    public static void GenerateChairLegs(float centerX_ft, float centerY_ft, out Obstacle[] legs)
    {
        const float legSize_ft = 0.333333f;
        const float legOffset_ft = 0.5f;

        legs = new Obstacle[4];
        legs[0] = new Obstacle()
        {
            Position = new Vector2(centerX_ft - legOffset_ft, centerY_ft - legOffset_ft),
            Width_ft = legSize_ft,
            Height_ft = legSize_ft
        };
        legs[1] = new Obstacle()
        {
            Position = new Vector2(centerX_ft + legOffset_ft, centerY_ft - legOffset_ft),
            Width_ft = legSize_ft,
            Height_ft = legSize_ft
        };
        legs[2] = new Obstacle()
        {
            Position = new Vector2(centerX_ft - legOffset_ft, centerY_ft + legOffset_ft),
            Width_ft = legSize_ft,
            Height_ft = legSize_ft
        };
        legs[3] = new Obstacle()
        {
            Position = new Vector2(centerX_ft + legOffset_ft, centerY_ft + legOffset_ft),
            Width_ft = legSize_ft,
            Height_ft = legSize_ft
        };
    }

    public static void GenerateWalls(float roomWidth_ft, float roomHeight_ft, float wallThickness_ft, out Obstacle[] walls)
    {
        walls = new Obstacle[4];
        walls[0] = new Obstacle()
        {
            Position = new Vector2(0, 0),
            Width_ft = roomWidth_ft,
            Height_ft = wallThickness_ft
        };
        walls[1] = new Obstacle()
        {
            Position = new Vector2(0, 0),
            Width_ft = wallThickness_ft,
            Height_ft = roomHeight_ft
        };
        walls[2] = new Obstacle()
        {
            Position = new Vector2(0, roomHeight_ft - wallThickness_ft),
            Width_ft = roomWidth_ft,
            Height_ft = wallThickness_ft
        };
        walls[3] = new Obstacle()
        {
            Position = new Vector2(roomWidth_ft - wallThickness_ft, 0),
            Width_ft = wallThickness_ft,
            Height_ft = roomHeight_ft
        };
    }
}