using System;

namespace LineUpGame
{
    class LineUpTest : Game
    {
        public override void Run()
        {
            SetupPlayers();
            int rows = 6, cols = 7;
            board = new Board(rows, cols);
            winCondition = 4;

            // ✅ 初始化足够的棋子库存
            int maxDiscs = rows * cols;
            p1.Inventory["ordinary"] = maxDiscs;
            p2.Inventory["ordinary"] = maxDiscs;
            p1.Inventory["boring"] = p1.Inventory["magnet"] = p1.Inventory["explode"] = maxDiscs;
            p2.Inventory["boring"] = p2.Inventory["magnet"] = p2.Inventory["explode"] = maxDiscs;

            Console.WriteLine("Enter test sequence (e.g., O4,M5,B2,O6):");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) return;

            var moves = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
            board.Display(); // 初始显示棋盘
            foreach (var move in moves)
            {
                string trimmed = move.Trim();
                if (trimmed.Length < 2) continue;

                char typeChar = char.ToUpper(trimmed[0]);
                if (!int.TryParse(trimmed.Substring(1), out int col)) continue;

                // ⚠️ 如果你希望用户输入 1-based 列号，这里要 -1
                col -= 1;

                string discType = typeChar switch
                {
                    'O' => "ordinary",
                    'B' => "boring",
                    'M' => "magnet",
                    'E' => "explode",
                    _ => "ordinary"
                };

                Disc d = current.CreateDisc(discType);
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
