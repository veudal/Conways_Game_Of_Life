using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Conways_Game_Of_Life
{
    internal class Program
    {
        static int dimesions = 0;
        static int nextFrameDelay = 0;
        static void Main(string[] args)
        {
            //Maximize the window
            SendKeys.SendWait("% x");

            try
            {
                while (true)
                {
                    MainLoop();
                }
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine("Error\n\n" + e.Message + "\n\nPress enter to try again.");
                Console.ReadLine();
                Console.Clear();
                Main(new string[0]);
            }
        }

        private static void MainLoop()
        {
            //Map is made from bools, if the bool is true, the cell is alive, otherwise it is dead
            Console.CursorVisible = true;

            SetDimensions();
            SetNextFrameDelay();

            bool[,] map = new bool[dimesions, dimesions];
            SetLivingCells(map);
            StartSimulation(map);
        }

        private static void HandleEnd()
        {
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(450);
                Console.Clear();
                Thread.Sleep(450);
                Console.WriteLine("  ▄████  ▄▄▄       ███▄ ▄███▓▓█████     ▒█████   ██▒   █▓▓█████  ██▀███  \r\n ██▒ ▀█▒▒████▄    ▓██▒▀█▀ ██▒▓█   ▀    ▒██▒  ██▒▓██░   █▒▓█   ▀ ▓██ ▒ ██▒\r\n▒██░▄▄▄░▒██  ▀█▄  ▓██    ▓██░▒███      ▒██░  ██▒ ▓██  █▒░▒███   ▓██ ░▄█ ▒\r\n░▓█  ██▓░██▄▄▄▄██ ▒██    ▒██ ▒▓█  ▄    ▒██   ██░  ▒██ █░░▒▓█  ▄ ▒██▀▀█▄  \r\n░▒▓███▀▒ ▓█   ▓██▒▒██▒   ░██▒░▒████▒   ░ ████▓▒░   ▒▀█░  ░▒████▒░██▓ ▒██▒\r\n ░▒   ▒  ▒▒   ▓▒█░░ ▒░   ░  ░░░ ▒░ ░   ░ ▒░▒░▒░    ░ ▐░  ░░ ▒░ ░░ ▒▓ ░▒▓░\r\n  ░   ░   ▒   ▒▒ ░░  ░      ░ ░ ░  ░     ░ ▒ ▒░    ░ ░░   ░ ░  ░  ░▒ ░ ▒░\r\n░ ░   ░   ░   ▒   ░      ░      ░      ░ ░ ░ ▒       ░░     ░     ░░   ░ \r\n      ░       ░  ░       ░      ░  ░       ░ ░        ░     ░  ░   ░     \r\n                                                     ░                   ");
            }
            Thread.Sleep(1000);
            Console.WriteLine("\n\n\nSimulation finished because either all cells died or because nothing happened anymore.\n\nPress enter to try again!");
            Console.ReadLine();
            Console.Clear();
        }

        private static void SetNextFrameDelay()
        {
            Console.WriteLine("Enter time in milliseconds for the delay between each frame (200 Recommended)");
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out nextFrameDelay))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Input not valid. Try again.");
                }
            }
            Console.Clear();
        }

        private static void SetDimensions()
        {
            Console.WriteLine("Enter dimensions of the grid (30-40 is recommended)");
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out dimesions))
                {
                    if (dimesions != 0)
                    {
                        if (dimesions < 51)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Dimensions can not be bigger than 50.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Dimensions can not be 0.");
                    }
                }
                else if (input != string.Empty)
                {
                    Console.WriteLine("Input not valid. Try again.");
                }
                else
                {
                    Console.CursorTop--;
                }
            }
            Console.Clear();
        }

        private static void StartSimulation(bool[,] map)
        {
            Console.CursorVisible = false;
            bool[,] previousMap = new bool[dimesions, dimesions];
            bool userQuit = false;
            while (AtLeastOneAlive(map) && AreDifferentMaps(previousMap, map))
            {
                PrintCurrentState(map);
                previousMap = map;
                map = GetNextFrame(map);
                Thread.Sleep(nextFrameDelay);

                if(UserCancels())
                {
                    Console.Clear();
                    Console.WriteLine("You started a new simulation by pressing enter");
                    Thread.Sleep(2500);
                    Console.Clear();
                    userQuit = true;
                    break;
                }
            }
            if (userQuit == false)
                HandleEnd();
        }

        private static bool UserCancels()
        {
            if (Console.KeyAvailable)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool AreDifferentMaps(bool[,] previousMap, bool[,] map)
        {
            for (int y = 0; y < dimesions; y++)
            {
                for (int x = 0; x < dimesions; x++)
                {
                    if (previousMap[x, y] != map[x, y])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void SetLivingCells(bool[,] map)
        {
            int middle = dimesions / 2;
            int x = middle * 3, y = middle;

            PrintPreview(map, x, y);
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key != ConsoleKey.Enter)
                {
                    if (key == ConsoleKey.LeftArrow && Console.CursorLeft > 0)
                    {
                        x -= 3;
                        Console.CursorLeft = x;
                    }
                    else if (key == ConsoleKey.RightArrow && Console.CursorLeft / 3 < dimesions - 1)
                    {
                        x += 3;
                        Console.CursorLeft = x;
                    }
                    else if (key == ConsoleKey.UpArrow && Console.CursorTop > 0)
                    {
                        Console.CursorTop = --y;
                    }
                    else if (key == ConsoleKey.DownArrow && Console.CursorTop < dimesions - 1)
                    {
                        Console.CursorTop = ++y;
                    }
                    else if (key == ConsoleKey.Spacebar)
                    {
                        map[x / 3, y] = !map[x / 3, y];
                        PrintPreview(map, x, y);
                    }
                }
                else
                {
                    Console.Clear();
                    break;
                }
            }
        }

        private static void PrintPreview(bool[,] map, int x, int y)
        {
            PrintCurrentState(map);
            Console.WriteLine("\n\nRevive cells by navigating with the arrow keys and pressing space. Start the simulation by pressing enter. In case of glitching, zoom out by pressing 'CTRL' + '-'");
            Console.CursorLeft = x;
            Console.CursorTop = y;
        }

        private static bool[,] GetNextFrame(bool[,] oldMap)
        {
            bool[,] newMap = new bool[dimesions, dimesions];
            IterateCells(oldMap, newMap);
            return newMap;
        }

        private static void IterateCells(bool[,] oldMap, bool[,] newMap)
        {
            for (int y = 0; y < dimesions; y++)
            {
                for (int x = 0; x < dimesions; x++)
                {
                    int neighbours = CountNeighbours(oldMap, x, y);

                    //Applying Conway's game of life rules
                    if (oldMap[x, y] == false)
                    {
                        if (neighbours == 3)
                            newMap[x, y] = true;
                    }
                    else if (neighbours == 2 || neighbours == 3)
                    {
                        newMap[x, y] = true;
                    }
                }
            }
        }

        private static int CountNeighbours(bool[,] map, int x, int y)
        {
            int neighbours = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        int col = (x + i + dimesions) % dimesions;
                        int row = (y + j + dimesions) % dimesions;
                        if (map[col, row])
                        {
                            neighbours++;
                        }
                    }
                }
            }
            return neighbours;
        }

        private static void PrintCurrentState(bool[,] map)
        {
            for (int y = 0; y < dimesions; y++)
            {
                StringBuilder builder = new StringBuilder();
                for (int x = 0; x < dimesions; x++)
                {
                    if (map[x, y] == true)
                    {
                        builder.Append(" ■ ");
                    }
                    else
                    {
                        builder.Append(" · ");
                    }
                }
                Console.CursorTop = y;
                Console.CursorLeft = 0;
                Console.Write(builder.ToString());
            }
        }

        private static bool AtLeastOneAlive(bool[,] map)
        {
            foreach (bool cell in map)
            {
                if (cell)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
