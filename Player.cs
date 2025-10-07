
using System;
using System.Collections.Generic;

namespace LineUpGame
{
    class Player
    {
        public char Symbol { get; }
        public string Name { get; }
        public Dictionary<string,int> Inventory { get; } = new()
        {
            {"ordinary", 0},
            {"boring", 2},
            {"magnet", 2},
            {"explode", 2},
        };

        public Player(char symbol, string name) { Symbol = symbol; Name = name; }

        public void ResetDefaultInventory()
        {
            Inventory["boring"] = 2;
            Inventory["magnet"] = 2;
            Inventory["explode"] = 2;
        }

        public bool HasDisc(string type) => Inventory.TryGetValue(type, out var n) && n > 0;
        public Disc CreateDisc(string type) => type switch
        {
            "boring" => new BoringDisc(Symbol),
            "magnet" => new MagnetDisc(Symbol),
            "explode" => new ExplodingDisc(Symbol),
            _ => new OrdinaryDisc(Symbol)
        };
        public void Consume(string type){ if (Inventory.ContainsKey(type)) Inventory[type]--; }

        public void ReturnFromChar(char ch)
        {
            switch (ch)
            {
                case '@': case '#': Inventory["ordinary"]++; break;
                case 'B': case 'b': Inventory["boring"]++; break;
                case 'M': case 'm': Inventory["magnet"]++; break;
                case 'E': case 'e': Inventory["explode"]++; break;
            }
        }

        public override string ToString() => Name;
    }

    class AIPlayer : Player
    {
        private readonly Random rng;

        public AIPlayer(char symbol, string name, Random? random = null)
            : base(symbol, name)
        {
            rng = random ?? new Random();
        }

        private static bool SimWin(char[,] grid, int col, char sym, int winN)
        {
            if (!Board.CanDropOnGrid(grid, col)) return false;
            int row = Board.FindDropRowOnGrid(grid, col);
            if (row < 0) return false;

            grid[row, col] = sym;
            bool win = Board.CheckWinOnGrid(grid, row, col, winN, sym);
            grid[row, col] = ' ';
            return win;
        }

        public int ChooseColumn(char[,] grid, int winN, char opponent)
        {
            int cols = grid.GetLength(1);

            for (int c = 0; c < cols; c++)
                if (SimWin(grid, c, this.Symbol, winN))
                    return c;

            for (int c = 0; c < cols; c++)
                if (SimWin(grid, c, opponent, winN))
                    return c;

            var candidates = new List<(int col, double dist)>();
            double center = (cols - 1) / 2.0;
            for (int c = 0; c < cols; c++)
            {
                if (!Board.CanDropOnGrid(grid, c)) continue;
                candidates.Add((c, Math.Abs(c - center)));
            }
            if (candidates.Count == 0) return -1;

            var strong = new List<(int col, double dist)>();
            foreach (var item in candidates)
            {
                if (RemainingEmptySlots(grid, item.col) >= winN)
                    strong.Add(item);
            }

            int PickByCenter(List<(int col, double dist)> list)
            {
                double best = double.MaxValue;
                var bestCols = new List<int>();
                foreach (var (col, dist) in list)
                {
                    if (dist < best - 1e-9) { best = dist; bestCols.Clear(); bestCols.Add(col); }
                    else if (Math.Abs(dist - best) <= 1e-9) { bestCols.Add(col); }
                }
                return bestCols[rng.Next(bestCols.Count)];
            }

            if (strong.Count > 0)
                return PickByCenter(strong);

            return PickByCenter(candidates);
        }

        public void MakeMove(Board board, int winN, char opponent)
        {
            if (!HasDisc("ordinary")) return;

            var grid = board.ExportGrid();
            int col = ChooseColumn(grid, winN, opponent);
            if (col < 0) return;

            if (board.DropDisc(col, CreateDisc("ordinary")))
                Consume("ordinary");
        }
        private static int RemainingEmptySlots(char[,] grid, int col)
        {
            int rows = grid.GetLength(0);
            int count = 0;
            for (int r = 0; r < rows; r++)
            {
                if (grid[r, col] == ' ') count++;
                else break;
            }
            return count;
        }
    }
    class HumanPlayer : Player
    {
        public HumanPlayer(char symbol, string name) : base(symbol, name) { }
    }
}
