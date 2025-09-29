
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
        private readonly Random rng = new();
        public AIPlayer(char symbol, string name) : base(symbol, name) { }

        public void MakeMove(Board board)
        {
            if (!HasDisc("ordinary")) return;
            for (int t = 0; t < 1000; t++)
            {
                int col = rng.Next(board.Columns);
                if (board.DropDisc(col, new OrdinaryDisc(Symbol)))
                {
                    Consume("ordinary");
                    return;
                }
            }
        }
    }

    class HumanPlayer : Player
    {
        public HumanPlayer(char symbol, string name) : base(symbol, name) { }
    }
}
