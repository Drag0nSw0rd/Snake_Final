using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SnakeGame_v2
{
    class Program
    {
        static int x = 90;
        static int y = 30;

        public static int dif;

        public static char a = 'y';

        static void Menu() //Menu gry
        {
            Console.SetWindowSize(x + 1, y + 1);
            Console.SetBufferSize(x + 1, y + 1);
            Console.CursorVisible = false;

            Walls walls = new Walls(x, y);
            walls.Draw();

            string m1 = "Welcome to the Snake Game";
            string m2 = "Start game y/n?";
            string m3 = "Press n to exit";
            string m4 = "Choose difficult: 1) begginer; 2) normal; 3) pro.";

            Console.SetCursorPosition((Console.WindowWidth - m1.Length) / 2, (Console.WindowHeight - 4) / 2); //formatowanie po centrum
            Console.WriteLine(m1);

            Console.SetCursorPosition((Console.WindowWidth - m2.Length) / 2, Console.WindowHeight / 2);
            Console.WriteLine(m2);

            Console.SetCursorPosition((Console.WindowWidth - m3.Length) / 2, (Console.WindowHeight + 4) / 2);
            Console.WriteLine(m3);

            Console.SetCursorPosition(Console.WindowWidth / 2, (Console.WindowHeight + 7) / 2);
            try
            {
                a = Convert.ToChar(Console.ReadLine());
            }
            catch
            {
            }

            if (a == 'y')
            {
                Difficult(); //Wybór trudności
                Console.SetCursorPosition((Console.WindowWidth - m4.Length) / 2, (Console.WindowHeight + 11) / 2);
                Console.WriteLine(m4);

                Console.SetCursorPosition(Console.WindowWidth / 2, (Console.WindowHeight + 15) / 2);
                int.TryParse(Console.ReadLine(), out dif);
            }
            Console.Clear();
        }

        static void Difficult() //trudność
        {
            if (dif == 1)
            {
                Thread.Sleep(120); //prędkość węża
            }

            else if (dif == 2)
            {
                Thread.Sleep(90);
            }

            else if (dif == 3)
            {
                Thread.Sleep(60);
            }
        }

        static void Main(string[] args)
        {

            int count = 0;

            Menu();

            while (a == 'y')
            {
                string _Score = $"Score: {count}"; //licznik 

                int _bottomY = Console.WindowHeight - 1; // formatowanie licznika po lewej dole
                Console.SetCursorPosition(0, _bottomY);
                Console.Write(_Score);

                Console.SetWindowSize(x + 1, y + 1); //rozmiar okna w zależności od rozmiara pola
                Console.SetBufferSize(x + 1, y + 1); //utworzyć pole
                Console.CursorVisible = false;

                Walls walls = new Walls(x, y); //utworzyć mury
                walls.Draw();

                Point p = new Point(x / 2, y / 2, '*'); //toczka początku gry
                Snake snake = new Snake(p, 4, Direction.RIGHT);
                snake.Draw();

                FoodCreator foodCreator = new FoodCreator(x, y, '@'); //utworzyć jedzenie
                Point food = foodCreator.CreateFood();
                food.Draw();

                while (true)
                {
                    if (walls.IsHit(snake) || snake.IsHitTail()) //warunki konca gry
                    {
                        break;
                    }

                    else if (snake.Eat(food))
                    {
                        food = foodCreator.CreateFood(); //jeśli wąż zje @ to utworza nowe @
                        food.Draw();

                        count = count + 1; // + 1 rachunku do licznika

                        string Score = $"Score: {count}"; // licznik z nowym rachunkiem

                        int bottomY = Console.WindowHeight - 1;
                        Console.SetCursorPosition(0, bottomY);
                        Console.Write(Score);
                    }

                    else
                    {
                        snake.Move();
                    }

                    Difficult(); // zmiana prędkości węża

                    if (Console.KeyAvailable) //kontrola węża
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        snake.HandleKey(key.Key);
                    }
                }

                if (true) // menu po zakończeniu gry
                {
                    string str = $"Game Over, Your Score: {count}";

                    Console.SetCursorPosition((Console.WindowWidth - str.Length) / 2, (Console.WindowHeight - 4) / 2);
                    Console.WriteLine(str);

                    string str1 = "Play Again y/n?";

                    Console.SetCursorPosition((Console.WindowWidth - str1.Length) / 2, Console.WindowHeight / 2);
                    Console.WriteLine(str1);

                    Console.SetCursorPosition(Console.WindowWidth / 2, (Console.WindowHeight + 3) / 2);
                    try
                    {
                        a = Convert.ToChar(Console.ReadLine());
                    }
                    catch
                    {
                        Console.Clear();
                        break;
                    }

                    if (a == 'y') // nowa gra
                    {
                        string m4 = "Choose difficult: 1) begginer; 2) normal; 3) pro";

                        Console.SetCursorPosition((Console.WindowWidth - m4.Length) / 2, (Console.WindowHeight + 7) / 2);
                        Console.WriteLine(m4);

                        Console.SetCursorPosition(Console.WindowWidth / 2, (Console.WindowHeight + 11) / 2);
                        int.TryParse(Console.ReadLine(), out dif); // Wybór poziomu trudności 
                    }

                    Console.Clear();

                    count = 0; // zerowanie licznika
                }
            }
        }
    }

    class Snake : Figure
    {
        public Direction direction;

        public Snake(Point tail, int length, Direction _direction)
        {
            direction = _direction;
            pList = new List<Point>();

            for (int i = 0; i < length; i++)
            {
                Point p = new Point(tail);
                p.Move(i, direction);
                pList.Add(p);
            }
        }

        public void Move()
        {
            Point tail = pList.First();
            pList.Remove(tail);
            Point head = GetNextPoint();
            pList.Add(head);

            tail.Clear();
            head.Draw();
        }

        public Point GetNextPoint()
        {
            Point head = pList.Last();
            Point nextPoint = new Point(head);
            nextPoint.Move(1, direction);
            return nextPoint;
        }

        public bool IsHitTail()
        {
            var head = pList.Last();

            for (int i = 0; i < pList.Count - 2; i++)
            {
                if (head.IsHit(pList[i]))
                    return true;
            }

            return false;
        }

        public void HandleKey(ConsoleKey key) //kontrol węża
        {
            switch (direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                    if (key == ConsoleKey.DownArrow)
                        direction = Direction.DOWN;

                    else if (key == ConsoleKey.UpArrow)
                        direction = Direction.UP;

                    break;

                case Direction.UP:
                case Direction.DOWN:
                    if (key == ConsoleKey.LeftArrow)
                        direction = Direction.LEFT;

                    else if (key == ConsoleKey.RightArrow)
                        direction = Direction.RIGHT;

                    break;
            }
        }

        public bool Eat(Point food)
        {
            Point head = GetNextPoint();

            if (head.IsHit(food)) // + 1 punkt węża
            {
                pList.Add(head);
                head.Draw();

                return true;
            }

            return false;
        }
    }

    enum Direction //klawisze do kontroli węża
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    class Point
    {
        public int x;
        public int y;
        public char sym;

        public Point(int _x, int _y, char _sym)
        {
            x = _x;
            y = _y;
            sym = _sym;
        }

        public Point(Point p)
        {
            x = p.x;
            y = p.y;
            sym = p.sym;
        }

        public void Move(int offset, Direction direction) //ruch węża
        {
            if (direction == Direction.RIGHT)
            {
                x = x + offset;
            }
            else if (direction == Direction.LEFT)
            {
                x = x - offset;
            }
            else if (direction == Direction.UP)
            {
                y = y - offset;
            }
            else if (direction == Direction.DOWN)
            {
                y = y + offset;
            }
        }

        public bool IsHit(Point p)
        {
            return p.x == this.x && p.y == this.y;
        }

        public void Draw() // narysować symbol
        {
            Console.SetCursorPosition(x, y);
            Console.Write(sym);
        }

        public void Clear() //wymazać symbol
        {
            sym = ' ';
            Draw();
        }
    }

    class Figure
    {
        protected List<Point> pList;

        public void Draw()
        {
            foreach (Point p in pList)
            {
                p.Draw();
            }
        }

        public bool IsHit(Figure figure)
        {
            foreach (var p in pList)
            {
                if (figure.IsHit(p))
                    return true;
            }
            return false;
        }

        private bool IsHit(Point point)
        {
            foreach (var p in pList)
            {
                if (p.IsHit(point))
                    return true;
            }
            return false;
        }
    }

    class HorizontalLine : Figure   //poziome linie pola
    {
        public HorizontalLine(int xLeft, int xRight, int y, char sym)
        {
            pList = new List<Point>();

            for (int x = xLeft; x <= xRight; x++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class VerticalLine : Figure   //pionowe linie pola
    {
        public VerticalLine(int yUp, int yDown, int x, char sym)
        {
            pList = new List<Point>();

            for (int y = yUp; y <= yDown; y++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class Block : Figure //dodatkowe przeszkody
    {
        public Block(int yB, int xB, int x, char sym)
        {
            pList = new List<Point>();

            for (int y = yB; y <= xB; y++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class Block1 : Figure
    {
        public Block1(int yB, int xB, int x, char sym)
        {
            pList = new List<Point>();

            for (int y = yB; y <= xB; y++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }

        }
    }

    class Block2 : Figure
    {
        public Block2(int xB, int yB, int y, char sym)
        {
            pList = new List<Point>();

            for (int x = xB; x <= yB; x++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class Block3 : Figure
    {
        public Block3(int xB, int yB, int y, char sym)
        {
            pList = new List<Point>();

            for (int x = xB; x <= yB; x++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class Block4 : Figure
    {
        public Block4(int yB, int xB, int x, char sym)
        {
            pList = new List<Point>();

            for (int y = yB; y <= xB; y++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class Block5 : Figure
    {
        public Block5(int xB, int yB, int y, char sym)
        {
            pList = new List<Point>();

            for (int x = xB; x <= yB; x++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class Block6 : Figure
    {
        public Block6(int yB, int xB, int x, char sym)
        {
            pList = new List<Point>();

            for (int y = yB; y <= xB; y++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class Block7 : Figure
    {
        public Block7(int xB, int yB, int y, char sym)
        {
            pList = new List<Point>();

            for (int x = xB; x <= yB; x++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class Block8 : Figure
    {
        public Block8(int xB, int yB, int y, char sym)
        {
            pList = new List<Point>();

            for (int x = xB; x <= yB; x++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class Block9 : Figure
    {
        public Block9(int yB, int xB, int x, char sym)
        {
            pList = new List<Point>();

            for (int y = yB; y <= xB; y++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }
    }

    class Walls //pole gry
    {
        List<Figure> wallList; // lista murów i przeszkód

        public Walls(int mapWidth, int mapHeight)
        {
            wallList = new List<Figure>();

            HorizontalLine upLine = new HorizontalLine(0, mapWidth - 2, 0, '#');
            HorizontalLine downLine = new HorizontalLine(0, mapWidth - 2, mapHeight - 1, '#');
            VerticalLine leftLine = new VerticalLine(0, mapHeight - 1, 0, '#');
            VerticalLine rightLine = new VerticalLine(0, mapHeight - 1, mapWidth - 2, '#');

            wallList.Add(upLine);
            wallList.Add(downLine);
            wallList.Add(leftLine);
            wallList.Add(rightLine);

            if (Program.dif == 2) // zmiana lista przeszkód w zalezności od trudności
            {
                Block block = new Block(5, mapHeight - 15, mapWidth - 10, '#');
                Block1 block1 = new Block1(0, mapHeight - 27, mapWidth - 75, '#');
                Block2 block2 = new Block2(10, mapWidth - 55, mapHeight - 8, '#');
                Block3 block3 = new Block3(15, mapWidth - 35, mapHeight - 23, '#');
                Block4 block4 = new Block4(8, mapHeight - 17, mapWidth - 75, '#');

                wallList.Add(block);
                wallList.Add(block1);
                wallList.Add(block2);
                wallList.Add(block3);
                wallList.Add(block4);
            }

            else if (Program.dif == 3)
            {
                Block block = new Block(5, mapHeight - 15, mapWidth - 10, '#');
                Block1 block1 = new Block1(0, mapHeight - 27, mapWidth - 75, '#');
                Block2 block2 = new Block2(10, mapWidth - 55, mapHeight - 8, '#');
                Block3 block3 = new Block3(15, mapWidth - 35, mapHeight - 23, '#');
                Block4 block4 = new Block4(8, mapHeight - 17, mapWidth - 75, '#');
                Block5 block5 = new Block5(0, mapHeight - 25, mapWidth - 85, '#');
                Block6 block6 = new Block6(5, mapHeight - 19, mapWidth - 19, '#');
                Block7 block7 = new Block7(67, mapWidth - 10, mapHeight - 25, '#');
                Block8 block8 = new Block8(70, mapWidth - 2, mapHeight - 5, '#');
                Block9 block9 = new Block9(22, mapHeight - 1, mapWidth - 55, '#');

                wallList.Add(block);
                wallList.Add(block1);
                wallList.Add(block2);
                wallList.Add(block3);
                wallList.Add(block4);
                wallList.Add(block5);
                wallList.Add(block6);
                wallList.Add(block7);
                wallList.Add(block8);
                wallList.Add(block9);
            }
        }

        public bool IsHit(Figure figure) // warunok porazki 
        {
            foreach (var wall in wallList)
            {
                if (wall.IsHit(figure))
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw() // narysować mury 
        {
            foreach (var wall in wallList)
            {
                wall.Draw();
            }
        }
    }

    class FoodCreator // utworzyć jedzenie
    {
        private int mapWidth;
        private int mapHeight;
        private char sym;

        Random random = new Random(); // wybór losowych koordynat pojawy jedzenia

        public FoodCreator(int mapWidth, int mapHeight, char sym)  // przepisać wartość losowego punktu pojawy jedzenia
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.sym = sym;
        }

        public Point CreateFood() // narysować nowe jedzenia
        {
            int x = random.Next(2, mapWidth - 2);
            int y = random.Next(2, mapHeight - 2);
            return new Point(x, y, sym);
        }
    }
}