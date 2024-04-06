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
        private const int GridSpacing = 21;
        
        private enum GameState
        {
            MainMenu,
            FirstPerson,
            Pause,
            Battle,
            BattleVictory
        }

        private enum DangerLevel
        {
            Low,
            Medium,
            High
        }

        private enum Turn
        {
            PlayerTurn,
            EnemyTurn
        }
        
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
            
            var posX = 22.5;
            var posY = 22.5;
            var dirX = -1.0;
            var dirY = 0.0;
            var planeX = 0.0;
            var planeY = 0.66;

            var drawMap = false;
            var currentSteps = 0;
            var encounterChance = 100;

            var currentScreen = GameState.MainMenu;
            var currentDangerLevel = DangerLevel.Low;
            var circleX = 1800;
            var circleY = 980;

            var player = new Entity
            {
                Attack = 20,
                Health = 100,
                Defence = 10,
                IsDefending = false
            };
            var enemy = new Entity
            {
                Attack = 15,
                Health = 100,
                Defence = 8,
                IsDefending = false
            };
            
            Raylib.InitWindow(1920, 1080, "Shallom");
            
            var enemyImage = Raylib.LoadTexture("../../../Resources/enemy.png");
            var playerImage = Raylib.LoadTexture("../../../Resources/player.png");
            
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {
                switch (currentScreen)
                {
                    case GameState.MainMenu:
                        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                        {
                            currentScreen = GameState.FirstPerson;
                        }
                        break;
                    case GameState.FirstPerson:
                        if (Raylib.IsKeyPressed(KeyboardKey.Tab))
                        {
                            currentScreen = GameState.Pause;
                        }
                        break;
                    case GameState.Pause:
                        if (Raylib.IsKeyPressed(KeyboardKey.Tab))
                        {
                            currentScreen = GameState.FirstPerson;
                        }
                        break;
                }
                
                Raylib.BeginDrawing();

                switch (currentScreen)
                {
                    case GameState.MainMenu:
                        Raylib.ClearBackground(Color.SkyBlue);
                        Raylib.DrawText("MAIN MENU", (ScreenWidth / 2 - 100), (ScreenHeight / 2 - 20), 40, Color.Black);
                        Raylib.DrawText("press enter to start", (ScreenWidth / 2 - 100), (ScreenHeight / 2 + 40), 20, Color.Black);
                        break;
                    case GameState.FirstPerson:
                        var random = new Random();
                        
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
                            
                            // Draw Walls
                            Raylib.DrawLine(x, drawStart, x, drawEnd, colour);
                            // Draw Ceiling
                            Raylib.DrawLine(x, 0, x, drawStart, Color.Gold);
                            //Draw Floor
                            Raylib.DrawLine(x, ScreenHeight, x, drawEnd, Color.DarkGray);
                        }
        
                        if (drawMap)
                        {
                            DrawMap(worldMap, posX, posY, dirX, dirY);
                        }
                        
                        
                        var frameTime = Raylib.GetFrameTime();
        
                        var moveSpeed = frameTime * 5.0;
                        var stepSpeed = 1;
                        var rotSpeed = frameTime * 3.0;
                        var stepTurn = Math.PI / 2;
        
                        if (Raylib.IsKeyPressed(KeyboardKey.W) || Raylib.IsKeyPressed(KeyboardKey.Up))
                        {
                            var resultX = (int)(posX + dirX * stepSpeed);
                            var resultY = (int)(posY + dirY * stepSpeed);
                            
                            if (worldMap[resultX, (int)posY] == 0) posX += dirX * stepSpeed;
                            if (worldMap[(int)posX, resultY] == 0) posY += dirY * stepSpeed;
                            if (random.Next(encounterChance) == 1) currentScreen = GameState.Battle;

                            if (currentSteps >= 10)
                            {
                                switch (currentDangerLevel)
                                {
                                    case DangerLevel.Low:
                                        currentDangerLevel = DangerLevel.Medium;
                                        currentSteps = 0;
                                        break;
                                    case DangerLevel.Medium:
                                        currentDangerLevel = DangerLevel.High;
                                        currentSteps = 0;
                                        break;
                                }
                            }

                            currentSteps++;
                        }
                        if (Raylib.IsKeyPressed(KeyboardKey.S) || Raylib.IsKeyPressed(KeyboardKey.Down))
                        {
                            var resultX = (int)(posX - dirX * stepSpeed);
                            var resultY = (int)(posY - dirY * stepSpeed);
                            
                            if (worldMap[resultX, (int)posY] == 0) posX -= dirX * stepSpeed;
                            if (worldMap[(int)posX, resultY] == 0) posY -= dirY * stepSpeed;
                            if (random.Next(encounterChance) == 1) currentScreen = GameState.Battle;
                            
                            if (currentSteps >= 10)
                            {
                                switch (currentDangerLevel)
                                {
                                    case DangerLevel.Low:
                                        currentDangerLevel = DangerLevel.Medium;
                                        break;
                                    case DangerLevel.Medium:
                                        currentDangerLevel = DangerLevel.High;
                                        break;
                                }
                            }

                            currentSteps++;
                        }
        
                        if (Raylib.IsKeyPressed(KeyboardKey.D) || Raylib.IsKeyPressed(KeyboardKey.Right))
                        {
                            var oldDirX = dirX;
                            var oldPlaneX = planeX;
        
                            dirX = dirX * Math.Cos(-stepTurn) - dirY * Math.Sin(-stepTurn);
                            dirY = oldDirX * Math.Sin(-stepTurn) + dirY * Math.Cos(-stepTurn);
        
                            planeX = planeX * Math.Cos(-stepTurn) - planeY * Math.Sin(-stepTurn);
                            planeY = oldPlaneX * Math.Sin(-stepTurn) + planeY * Math.Cos(-stepTurn);
                        }
                        if (Raylib.IsKeyPressed(KeyboardKey.A) || Raylib.IsKeyPressed(KeyboardKey.Left))
                        {
                            var oldDirX = dirX;
                            var oldPlaneX = planeX;
                            
                            dirX = dirX * Math.Cos(stepTurn) - dirY * Math.Sin(stepTurn);
                            dirY = oldDirX * Math.Sin(stepTurn) + dirY * Math.Cos(stepTurn);
                            
                            planeX = planeX * Math.Cos(stepTurn) - planeY * Math.Sin(stepTurn);
                            planeY = oldPlaneX * Math.Sin(stepTurn) + planeY * Math.Cos(stepTurn);
                        }

                        switch (currentDangerLevel)
                        {
                            case DangerLevel.Low:
                                Raylib.DrawCircleLines(circleX, circleY, 51, Color.Black);
                                Raylib.DrawCircle(circleX, circleY, 50, Color.SkyBlue);
                                encounterChance = 1000;
                                break;
                            case DangerLevel.Medium:
                                Raylib.DrawCircleLines(circleX, circleY, 51, Color.Black);
                                Raylib.DrawCircle(circleX, circleY, 50, Color.Yellow);
                                encounterChance = 100;
                                break;
                            case DangerLevel.High:
                                Raylib.DrawCircleLines(circleX, circleY, 51, Color.Black);
                                Raylib.DrawCircle(circleX, circleY, 50, Color.Red);
                                encounterChance = 10;
                                break;
                            
                        }
        
                        if (Raylib.IsKeyPressed(KeyboardKey.M)) drawMap = !drawMap;
                        
                        break;
                    case GameState.Pause:
                        Raylib.ClearBackground(Color.LightGray);
                        Raylib.DrawText("PAUSED", 200, 200, 80, Color.Black);
                        break;
                    case GameState.Battle:
                        var turn = new Random();
                        var currentTurn = Turn.PlayerTurn;
                        Raylib.ClearBackground(Color.Beige);
                        var enemyPosition = new Vector2
                        {
                            X = 800.0f,
                            Y = 200.0f
                        };
                        var playerPosition = new Vector2
                        {
                            X = 800.0f,
                            Y = 500.0f
                        };
                        Raylib.DrawTextureEx(enemyImage, enemyPosition, 0.0f, 8.0f, Color.White);
                        Raylib.DrawTextureEx(playerImage, playerPosition, 0.0f, 8.0f, Color.White);
                        
                        Raylib.DrawText("BATTLE!", 200, 200, 80, Color.Black);
                        Raylib.DrawText("Random monster has appeared", 200, 300, 40, Color.Black);
                        
                        Raylib.DrawText($"Enemy Health: {enemy.Health}", 1200, 200, 30, Color.Black);
                        Raylib.DrawText($"Player Health: {player.Health}", 1200, 500, 30, Color.Black);
                        
                        switch (currentTurn)
                        {
                            case Turn.PlayerTurn:
                                Raylib.DrawText("Press 'A' to attack", 200, 500, 40, Color.Black);
                                Raylib.DrawText("Press 'D' to Defend", 200, 600, 40, Color.Black);
                                Raylib.DrawText("Press 'X' to run away", 200, 700, 40, Color.Black);
                                if (Raylib.IsKeyPressed(KeyboardKey.A))
                                {
                                    Raylib.DrawText($"-{player.Attack}", 840, 200, 30, Color.Black);
                                    if (enemy.IsDefending)
                                    {
                                        enemy.Health -= (player.Attack - enemy.Defence);
                                    }
                                    else
                                    {
                                        enemy.Health -= player.Attack;
                                    }
                                }

                                if (Raylib.IsKeyPressed(KeyboardKey.D)) player.IsDefending = true;
                                if (Raylib.IsKeyPressed(KeyboardKey.X)) currentScreen = GameState.FirstPerson;
                                if (enemy.Health <= 0) currentScreen = GameState.BattleVictory;
                                
                                break;
                            case Turn.EnemyTurn:
                                break;
                        }
                        
                        currentSteps = 0;
                        currentDangerLevel = DangerLevel.Low;
                        break;
                    case GameState.BattleVictory:
                        Raylib.ClearBackground(Color.Gold);
                        Raylib.DrawText("YOU WON!", 800, 600, 80, Color.Black);
                        Raylib.DrawText("press enter to continue", 800, 800, 40, Color.Black);
                        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                        {
                            currentScreen = GameState.FirstPerson;
                            enemy.Health = 100;
                        }
                        break;
                }
               
                Raylib.DrawFPS(10, 10);
                Raylib.EndDrawing();
            }
            
            Raylib.CloseWindow();
        }

        public static void DrawMap(int[,] worldMap, double posX, double posY, double dirX, double dirY)
        {
            var transBlack = new Color
            {
                A = 127,
                B = 0,
                G = 0,
                R = 0
            };
            var transWhite = new Color
            {
                A = 127,
                B = 245,
                G = 245,
                R = 245
            };
            var transYellow = new Color
            {
                A = 127,
                B = 0,
                G = 249,
                R = 253
            };
            var transBlue = new Color
            {
                A = 127,
                B = 241,
                G = 121,
                R = 0
            };
            var transGray = new Color
            {
                A = 127,
                B = 200,
                G = 200,
                R = 200
            };
            var transGreen = new Color
            {
                A = 127,
                B = 48,
                G = 228,
                R = 0
            };
            
            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    var cellPosX = (j + 1) * GridSpacing;
                    var cellPosY = (i + 1) * GridSpacing;
                    
                    switch (worldMap[i, j])
                    {
                        case 0:
                           Raylib.DrawRectangle(cellPosX, cellPosY, 20, 20, transBlack);
                           break;
                        case 1:
                           Raylib.DrawRectangle(cellPosX, cellPosY, 20, 20, transGray);
                           break;
                        case 2:
                           Raylib.DrawRectangle(cellPosX, cellPosY, 20, 20, transGreen);
                           break;
                        case 3:
                           Raylib.DrawRectangle(cellPosX, cellPosY, 20, 20, transBlue);
                           break;
                        case 4:
                           Raylib.DrawRectangle(cellPosX, cellPosY, 20, 20, transWhite);
                           break;
                        case 5:
                           Raylib.DrawRectangle(cellPosX, cellPosY, 20, 20, transYellow);
                           break;
                        default:
                           Raylib.DrawRectangle(cellPosX, cellPosY, 20, 20, transBlack);
                           break;
                    }

                    if (i == (int)posY && j == (int)posX)
                    {
                        Raylib.DrawRectangle(cellPosY + 5, cellPosX + 5, 10, 10, transYellow);
                    }
                }
            }
        }
    }
}