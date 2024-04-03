using System.Numerics;
using Raylib_cs;

namespace raycaster_raylib
{
    internal class Program
    {
        private const int MapWidth = 24;
        private const int MapHeight = 24;
        private const int ScreenWidth = 1920;
        private const int ScreenHeight = 1080;
        
        static void Main()
        {
            int[,] worldMap = 
                 {
                     {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                     {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,0,0,0,0,0,2,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
                     {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1},
                     {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,0,0,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
                     {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                     {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
                 };
            
            var posX = 22.0;
            var posY = 22.0;
            var dirX = -1.0;
            var dirY = 0.0;
            var planeX = 0.0;
            var planeY = 0.66;
            
            Raylib.InitWindow(1920, 1080, "Shallom");
            
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Blank);

                for (int x = 0; x < ScreenWidth; x++)
                {
                    //ray position and direction
                    var cameraX = 2 * x / (double)ScreenWidth - 1;
                    var rayDirX = dirX + planeX * cameraX;
                    var rayDirY = dirY + planeY * cameraX;
                    
                    //what map box we're in
                    var mapX = (int)posX;
                    var mapY = (int)posY;

                    //ray length from current position to next x or y side
                    double sideDistX;
                    double sideDistY;
                    
                    //length of ray from one x or y side to next x or y side
                    var deltaDistX = rayDirX == 0 ? 1e30 : Math.Abs(1 / rayDirX);
                    var deltaDistY = rayDirY == 0 ? 1e30 : Math.Abs(1 / rayDirY);

                    double perpWallDist;

                    int stepX;
                    int stepY;

                    var hit = 0;
                    var side = 0;

                    if (rayDirX < 0)
                    {
                        stepX = -1;
                        sideDistX = (posX - mapX) * deltaDistX;
                    }
                    else
                    {
                        stepX = 1;
                        sideDistX = (mapX + 1.0 - posX) * deltaDistX;
                    }

                    if (rayDirY < 0)
                    {
                        stepY = -1;
                        sideDistY = (posY - mapY) * deltaDistY;
                    }
                    else
                    {
                        stepY = 1;
                        sideDistY = (mapY + 1.0 - posY) * deltaDistY;
                    }

                    while (hit == 0)
                    {
                        if (sideDistX < sideDistY)
                        {
                            sideDistX += deltaDistX;
                            mapX += stepX;
                            side = 0;
                        }
                        else
                        {
                            sideDistY += deltaDistY;
                            mapY += stepY;
                            side = 1;
                        }

                        if (worldMap[mapX, mapY] > 0)
                            hit = 1;
                    }

                    if (side == 0)
                    {
                        perpWallDist = (sideDistX - deltaDistX);
                    }
                    else
                    {
                        perpWallDist = (sideDistY - deltaDistY);
                    }

                    var lineHeight = (int)(ScreenHeight / perpWallDist);

                    var drawStart = -lineHeight / 2 + ScreenHeight / 2;
                    if (drawStart < 0) drawStart = 0;
                    var drawEnd = lineHeight / 2 + ScreenHeight / 2;
                    if (drawEnd >= ScreenHeight) drawEnd = ScreenHeight - 1;

                    Color colour;
                    switch (worldMap[mapX, mapY])
                    {
                        case 1:
                            colour = Color.Red;
                            break;
                        case 2:
                            colour = Color.Green;
                            break;
                        case 3:
                            colour = Color.Blue;
                            break;
                        case 4:
                            colour = Color.White;
                            break;
                        default:
                            colour = Color.Yellow;
                            break;
                    }

                    //give x and y side a different brightness
                    //if(side == 1) {color = color / 2;}
                    
                    Raylib.DrawLine(x, drawStart, x, drawEnd, colour);
                }
                
                var frameTime = Raylib.GetFrameTime();

                var moveSpeed = frameTime * 5.0;
                var rotSpeed = frameTime * 3.0;

                if (Raylib.IsKeyDown(KeyboardKey.W))
                {
                    var resultX = (int)(posX + dirX * moveSpeed);
                    var resultY = (int)(posY + dirY * moveSpeed);
                    if (worldMap[resultX, (int)posY] == 0) posX += dirX * moveSpeed;
                    if (worldMap[(int)posX, resultY] == 0) posY += dirY * moveSpeed;
                }
                if (Raylib.IsKeyDown(KeyboardKey.S))
                {
                    var resultX = (int)(posX - dirX * moveSpeed);
                    var resultY = (int)(posY - dirY * moveSpeed);
                    if (worldMap[resultX, (int)posY] == 0) posX -= dirX * moveSpeed;
                    if (worldMap[(int)posX, resultY] == 0) posY -= dirY * moveSpeed;
                }
                if (Raylib.IsKeyDown(KeyboardKey.D))
                {
                    var oldDirX = dirX;
                    dirX = dirX * Math.Cos(-rotSpeed) - dirY * Math.Sin(-rotSpeed);
                    dirY = oldDirX * Math.Sin(-rotSpeed) + dirY * Math.Cos(-rotSpeed);
                    var oldPlaneX = planeX;
                    planeX = planeX * Math.Cos(-rotSpeed) - planeY * Math.Sin(-rotSpeed);
                    planeY = oldPlaneX * Math.Sin(-rotSpeed) + planeY * Math.Cos(-rotSpeed);
                }
                if (Raylib.IsKeyDown(KeyboardKey.A))
                {
                    var oldDirX = dirX;
                    dirX = dirX * Math.Cos(rotSpeed) - dirY * Math.Sin(rotSpeed);
                    dirY = oldDirX * Math.Sin(rotSpeed) + dirY * Math.Cos(rotSpeed);
                    var oldPlaneX = planeX;
                    planeX = planeX * Math.Cos(rotSpeed) - planeY * Math.Sin(rotSpeed);
                    planeY = oldPlaneX * Math.Sin(rotSpeed) + planeY * Math.Cos(rotSpeed);
                }
                
                Raylib.DrawFPS(10, 10);
                
                Raylib.EndDrawing();
            }
            
            Raylib.CloseWindow();
        }
    }
}