using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    class Game
    {
        static readonly int x = 80;
        static readonly int y = 26;

        static Walls walls;
        static Snake snake;
        static FoodFactory foodFactory;
        static Timer time;

        static void Main()
        {
            Console.SetWindowSize(x + 1, y + 1);
            Console.SetBufferSize(x + 1, y + 1);
            Console.CursorVisible = false;

            walls = new Walls(x, y, '=');
            snake = new Snake(x / 2, y / 2, 3);

            foodFactory = new FoodFactory(x, y, '*');
            foodFactory.CreateFood();

            time = new Timer(Loop, null, 0, 200);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        snake.Rotation(key.Key);
                    }
                }
            }
        }

        static void StartNewGame()
        {
            Console.Clear();
            walls = new Walls(x, y, '=');
            snake = new Snake(x / 2, y / 2, 3);
            foodFactory = new FoodFactory(x, y, '*');
            foodFactory.CreateFood();
            time = new Timer(Loop, null, 0, 200);
        }

        static void Loop(object obj)
        {
            if (walls.IsHit(snake.GetHead()) || snake.IsHit(snake.GetHead()))
            {
                time.Change(0, Timeout.Infinite);
            }
            else if (snake.Eat(foodFactory.food))
            {
                foodFactory.CreateFood();
            }
            else
            {
                snake.Move();
            }
        }
    }

    struct Point
    {
        public int x { get; set; }
        public int y { get; set; }
        public char ch { get; set; }

        public static implicit operator Point((int, int, char) value) =>
              new Point { x = value.Item1, y = value.Item2, ch = value.Item3 };

        public static bool operator ==(Point a, Point b) =>
                (a.x == b.x && a.y == b.y) ? true : false;
        public static bool operator !=(Point a, Point b) =>
                (a.x != b.x || a.y != b.y) ? true : false;

        public void Draw()
        {
            DrawPoint(ch);
        }
        public void Clear()
        {
            DrawPoint(' ');
        }

        private void DrawPoint(char _ch)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(_ch);
        }
    }

    class Walls
    {
        private char ch;
        private List<Point> wall = new List<Point>();

        public Walls(int x, int y, char ch)
        {
            this.ch = ch;

            DrawHorizontal(x, 0);
            DrawHorizontal(x, y);
            DrawVertical(0, y);
            DrawVertical(x, y);
        }

        private void DrawHorizontal(int x, int y)
        {
            for (int i = 0; i < x; i++)
            {
                Point p = (i, y, ch);
                p.Draw();
                wall.Add(p);
            }
        }

        private void DrawVertical(int x, int y)
        {
            for (int i = 0; i < y; i++)
            {
                Point p = (x, i, ch);
                p.Draw();
                wall.Add(p);
            }
        }

        public bool IsHit(Point p)
        {
            foreach (var w in wall)
            {
                if (p == w)
                {
                    return true;
                }
            }
            return false;
        }
    }

    enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    class Snake
    {
        private List<Point> snake;

        private Direction direction;
        private int step = 1;
        private Point tail;
        private Point head;

        bool rotate = true;

        public Snake(int x, int y, int length)
        {
            direction = Direction.RIGHT;

            snake = new List<Point>();
            for (int i = x - length; i < x; i++)
            {
                Point p = (i, y, '-');
                snake.Add(p);

                p.Draw();
            }
        }

        public Point GetHead() => snake.Last();

        public void Move()
        {
            head = GetNextPoint();
            snake.Add(head);

            tail = snake.First();
            snake.Remove(tail);

            tail.Clear();
            head.Draw();

            rotate = true;
        }

        public bool Eat(Point p)
        {
            head = GetNextPoint();
            if (head == p)
            {
                snake.Add(head);
                head.Draw();
                return true;
            }
            return false;
        }

        public Point GetNextPoint()
        {
            Point p = GetHead();

            switch (direction)
            {
                case Direction.LEFT:
                    p.x -= step;
                    break;
                case Direction.RIGHT:
                    p.x += step;
                    break;
                case Direction.UP:
                    p.y -= step;
                    break;
                case Direction.DOWN:
                    p.y += step;
                    break;
            }
            return p;
        }

        public void Rotation(ConsoleKey key)
        {
            if (rotate)
            {
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        direction = (direction != Direction.RIGHT) ? Direction.LEFT : direction;
                        break;
                    case ConsoleKey.RightArrow:
                        direction = (direction != Direction.LEFT) ? Direction.RIGHT : direction;
                        break;
                    case ConsoleKey.UpArrow:
                        direction = (direction != Direction.DOWN) ? Direction.UP : direction;
                        break;
                    case ConsoleKey.DownArrow:
                        direction = (direction != Direction.UP) ? Direction.DOWN : direction;
                        break;
                }
                rotate = false;
            }
        }

        public bool IsHit(Point p)
        {
            for (int i = snake.Count - 2; i > 0; i--)
            {
                if (snake[i] == p)
                {
                    return true;
                }
            }
            return false;
        }
    }

    class FoodFactory
    {
        int mapWidth;
        int mapHeight;
        char ch;
        public Point food { get; set; }
        Random rnd = new Random();

        public FoodFactory(int x, int y, char ch)
        {
            mapWidth = x;
            mapHeight = y;
            this.ch = ch;
        }

        public void CreateFood()
        {
            food = (rnd.Next(2, mapWidth - 2), rnd.Next(2, mapHeight - 2), ch);
            food.Draw();
        }
    }
}