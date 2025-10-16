namespace LineUpGame
{
    class LineUpTest : Game
    {
        private const int ROWS = 6;
        private const int COLS = 7;

        protected override void ConfigureBoard()
        {
            board = new Board(ROWS, COLS);
        }

        protected override void ConfigureInventory()
        {
            // Unlimited inventory for testing
            int maxDiscs = ROWS * COLS;
      p1.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("@"), maxDiscs),
        InventoryFactory.CreateInventory("boring", char.Parse("@"), maxDiscs),
        InventoryFactory.CreateInventory("magnet", char.Parse("@"), maxDiscs),
        InventoryFactory.CreateInventory("exploding", char.Parse("@"), maxDiscs)
      };
      p2.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("#"), maxDiscs),
        InventoryFactory.CreateInventory("boring", char.Parse("#"), maxDiscs),
        InventoryFactory.CreateInventory("magnet", char.Parse("#"), maxDiscs),
        InventoryFactory.CreateInventory("exploding", char.Parse("#"), maxDiscs)
      };
        }

        protected override void ConfigureRules()
        {
            winCondition = 4;
        }

        protected override bool UseOnlyOrdinary() => false;
        protected override bool EnableSpin() => false;

        // Override Run() to execute scripted test sequence
        public new void Run()
        {
            SetupPlayers();
            ConfigureBoard();
            ConfigureInventory();
            ConfigureRules();

            Console.WriteLine("Enter test sequence (e.g., O4,M5,B2,O6):");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) return;

            var moves = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
            board.Display(); // Initial board display

            foreach (var move in moves)
            {
                string trimmed = move.Trim();
                if (trimmed.Length < 2) continue;

                char typeChar = char.ToUpper(trimmed[0]);
                if (!int.TryParse(trimmed.Substring(1), out int col)) continue;

                // Convert from 1-based to 0-based column index
                col -= 1;

                string discType = typeChar switch
                {
                    'O' => "ordinary",
                    'B' => "boring",
                    'M' => "magnet",
          'E' => "exploding",
                    _ => "ordinary"
                };

                Disc d = DiscFactory.CreateDisc(discType, current.Symbol);
                if (board.DropDisc(col, d))
                {
                    current.Consume(discType);
                    board.Display();

                    if (board.CheckWin(current.Symbol, winCondition))
                    {
                        Console.WriteLine($"{current.Name} wins!");
                        return;
                    }

                    current = (current == p1) ? p2 : p1;
                }
                else
                {
                    Console.WriteLine($"Invalid move or column full: {trimmed}");
                }
            }

            Console.WriteLine("Test sequence complete.");
        }
    }
}
