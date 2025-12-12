using System.Drawing;

namespace Lab08.Displays
{
    public static class DisplayMap
    {
        private static bool[,]? discoveredTiles;
        private static RoomType[,]? knownRoomTypes;
        private static bool[,]? knownAlienLocations;

        public static void InitializeMap(int rows, int columns)
        {
            discoveredTiles = new bool[rows, columns];
            knownRoomTypes = new RoomType[rows, columns];
            knownAlienLocations = new bool[rows, columns];
        }

        public static void MarkTileDiscovered(Location location, RoomType roomType)
        {
            discoveredTiles![location.Row, location.Column] = true;
            knownRoomTypes![location.Row, location.Column] = roomType;
        }

        public static void MarkMonsterHit(Location location)
        {
            knownAlienLocations![location.Row, location.Column] = true;
        }

        public static void ClearMonsterMark(Location location)
        {
            knownAlienLocations![location.Row, location.Column] = false;
        }

        public static void ShowMap(Game game)
        {
            Location playerLoc = game.Player.Location;
            int rows = game.Map.Height;
            int cols = game.Map.Width;
            int tileWidth = 4; 

            int legendStartX = cols * (tileWidth + 1) + 6;
            int legendStartY = 1;

            int lineY = 0;
            Console.SetCursorPosition(0, lineY);
            Console.ForegroundColor = ConsoleColor.Red;
            string topBorder = " ";
            for (int col = 0; col < cols; col++)
            {
                topBorder += "---";
                if (col < cols - 1)
                    topBorder += "+";
            }
            
            if (topBorder.Length < Console.WindowWidth) topBorder = topBorder.PadRight(Console.WindowWidth);
            Console.WriteLine(topBorder);

            for (int row = 0; row < rows; row++)
            {
                lineY = row * 2 + 1;
                Console.SetCursorPosition(0, lineY);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("|");
                Console.ResetColor();
                for (int col = 0; col < cols; col++)
                {
                    Location currentLoc = new Location(row, col);
                    RoomType roomType = game.Map.GetRoomTypeAt(currentLoc);
                    bool discovered = game.Map.IsDiscovered(currentLoc);

                    ConsoleColor bgColor = ConsoleColor.Black;
                    if (!discovered)
                    {
                        bgColor = ConsoleColor.Black;
                    }
                    else
                    {
                        switch (roomType)
                        {
                            case RoomType.Pit:
                                bgColor = ConsoleColor.Yellow;
                                break;
                            case RoomType.Airlock:
                                bgColor = ConsoleColor.Blue;
                                break;
                            case RoomType.MechBay:
                                bgColor = ConsoleColor.DarkYellow;
                                break;
                            case RoomType.MedBay:
                                bgColor = ConsoleColor.Green;
                                break;
                            case RoomType.Normal:
                                bgColor = ConsoleColor.DarkGray;
                                break;
                        }
                        if (knownAlienLocations![row, col])
                            bgColor = ConsoleColor.Red;
                    }

                    Console.BackgroundColor = bgColor;
                    string glyph = "   "; 
                    Console.ForegroundColor = ConsoleColor.Black;

                    if (row == playerLoc.Row && col == playerLoc.Column)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        glyph = " P ";
                    }
                    else if (knownAlienLocations![row,col])
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        glyph = " A ";
                    }

                    Console.Write(glyph);
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("|");
                    Console.ResetColor();
                }
                int currentCol = Console.CursorLeft;
                if (currentCol < Console.WindowWidth)
                {
                    Console.Write(new string(' ', Console.WindowWidth - currentCol));
                }

                lineY = row * 2 + 2;
                Console.SetCursorPosition(0, lineY);
                var sepLine = " ";
                for (int col = 0; col < cols; col++)
                {
                    sepLine += "---";
                    if (col < cols - 1) sepLine += "+";
                }
                if (sepLine.Length < Console.WindowWidth) sepLine = sepLine.PadRight(Console.WindowWidth);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(sepLine);
                Console.ResetColor();
            }

            Console.ResetColor();

            string[] legendLines = new string[] {
                "=== Legend ===",
                "██  Airlock",
                "██  MedBay",
                "██  MechBay",
                "██  Pit",
                "██  Alien",
                "██  Player",
                "██  Normal Room",
                "----  Walls"
            };

            for (int i = 0; i < legendLines.Length; i++)
            {
                Console.SetCursorPosition(legendStartX, legendStartY + i);
                string line = legendLines[i];
                ConsoleColor c = ConsoleColor.White;
                if (line.Contains("Airlock")) c = ConsoleColor.Blue;
                else if (line.Contains("MedBay")) c = ConsoleColor.Green;
                else if (line.Contains("MechBay")) c = ConsoleColor.DarkYellow;
                else if (line.Contains("Pit")) c = ConsoleColor.Yellow;
                else if (line.Contains("Alien")) c = ConsoleColor.DarkRed;
                else if (line.Contains("Player")) c = ConsoleColor.Magenta;
                else if (line.Contains("Walls")) c = ConsoleColor.Red;

                Console.ForegroundColor = c;
                string outLine = line;
                if (outLine.Length < Console.WindowWidth - legendStartX) outLine = outLine.PadRight(Console.WindowWidth - legendStartX);
                Console.Write(outLine);
                Console.ResetColor();
            }

            Console.SetCursorPosition(0, rows * 2 + 3);
        }
    }
}
