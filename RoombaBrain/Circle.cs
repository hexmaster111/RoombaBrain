using System.Numerics;
using RoombaBrain.Rendering;
using static SDL2.SDL;

namespace RoombaBrain;

public class Circle
{
    public Vector2 Center;
    public float Raidus_ft;
    public float Rotation_deg;

    public Circle(Vector2 center, float raidus_ft)
    {
        Center = center;
        Raidus_ft = raidus_ft;
    }

    public void Render(RenderArgs args, float pixelsPerFt, SDL_Color color)
    {
        int radius_px = (int)(Raidus_ft * pixelsPerFt);
        int x_px = (int)(Center.X * pixelsPerFt);
        int y_px = (int)(Center.Y * pixelsPerFt);

        SDL_SetRenderDrawColor(args.Renderer, color.r, color.g, color.b, color.a);

        //Rotate the crosshair with the circles rotation value
        SDL_RenderDrawLine(args.Renderer, x_px, y_px, (int)(x_px + radius_px * Math.Cos(Rotation_deg * Math.PI / 180)),
                         (int)(y_px + radius_px * Math.Sin(Rotation_deg * Math.PI / 180)));


        //draw the circle outline
        for (int i = 0; i < 360; i++)
        {
            SDL_RenderDrawPoint(args.Renderer,
             (int)(x_px + radius_px * Math.Cos(i * Math.PI / 180)),
             (int)(y_px + radius_px * Math.Sin(i * Math.PI / 180))
            );
        }
    }

    public bool Contains(Vector2 point)
    {
        return Vector2.Distance(point, Center) < Raidus_ft;
    }

    public bool Intersects(Vector2 rectCornerPos, float width_ft, float height_ft)
    {
        //Check if the center of the circle is within the rectangle
        if (Center.X > rectCornerPos.X && Center.X < rectCornerPos.X + width_ft &&
            Center.Y > rectCornerPos.Y && Center.Y < rectCornerPos.Y + height_ft)
        {
            return true;
        }

        Vector2[] rectPoints = new Vector2[4];
        rectPoints[0] = rectCornerPos;
        rectPoints[1] = new Vector2(rectCornerPos.X + width_ft, rectCornerPos.Y);
        rectPoints[2] = new Vector2(rectCornerPos.X, rectCornerPos.Y + height_ft);
        rectPoints[3] = new Vector2(rectCornerPos.X + width_ft, rectCornerPos.Y + height_ft);

        //Check if any of the rectangle's points are within the circle
        foreach (Vector2 point in rectPoints)
        {
            if (Contains(point))
            {
                return true;
            }
        }

        //Check if any of the edges of the rectangle intersect with the circle
        for (int i = 0; i < 4; i++)
        {
            if (Intersects(rectPoints[i], rectPoints[(i + 1) % 4]))
            {
                return true;
            }
        }


        // check if the circles outer edge is within the rectangle
        if (Center.X + Raidus_ft > rectCornerPos.X && Center.X - Raidus_ft < rectCornerPos.X + width_ft &&
            Center.Y + Raidus_ft > rectCornerPos.Y && Center.Y - Raidus_ft < rectCornerPos.Y + height_ft)
        {
            return true;
        }

        return false;
    }

    public bool Intersects(Vector2 p1, Vector2 p2)
    {
        float A = p2.Y - p1.Y;
        float B = p1.X - p2.X;
        float C = A * p1.X + B * p1.Y;

        float dist = Math.Abs(A * Center.X + B * Center.Y - C) / (float)Math.Sqrt(A * A + B * B);

        if (dist <= Raidus_ft)
        {
            //Get the closest point on the line to the circle's center
            float t = -(A * Center.X + B * Center.Y - C) / (A * A + B * B);
            Vector2 closestPoint = new Vector2(A * t + Center.X, B * t + Center.Y);

            //Check if the closest point is within the line segment
            if (closestPoint.X > p1.X && closestPoint.X < p2.X &&
                closestPoint.Y > p1.Y && closestPoint.Y < p2.Y)
            {
                return true;
            }
        }

        return false;
    }
}

