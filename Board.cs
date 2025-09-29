
using System;
using System.Threading;
using System.Collections.Generic;

namespace LineUpGame
{
    class Board
    {
        private char[,] grid;
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public Board(int rows, int cols)
        {
            if (rows < 6 || cols < 7) throw new ArgumentException("Minimum board is 6x7.");
            if (rows > cols) throw new ArgumentException("Rows cannot exceed columns.");
            Rows = rows;
            Columns = cols;
            grid = new char[Rows, Columns];
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    grid[r, c] = ' ';
        }

        public bool IsValidColumn(int col) => col >= 0 && col < Columns && grid[0, col] == ' ';

        public bool IsFull()
        {
            for (int c = 0; c < Columns; c++)
                if (grid[0, c] == ' ') return false;
            return true;
        }

        public void Display()
        {
            Console.WriteLine();
            for (int r = 0; r < Rows; r++)
            {
                Console.Write("|");
                for (int c = 0; c < Columns; c++)
                {
                    char ch = grid[r, c];
                    Console.Write($" {(ch == ' ' ? ' ' : ch)} |");
                }
                Console.WriteLine();
            }
            Console.Write(" ");
            for (int c = 0; c < Columns; c++) Console.Write($" {c}  ");
            Console.WriteLine("\n");
        }

        public bool DropDisc(int col, Disc disc)
        {
            if (!IsValidColumn(col)) return false;
            int row = -1;
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (grid[r, col] == ' ')
                {
                    row = r;
                    break;
                }
            }
            if (row == -1) return false;
            grid[row, col] = disc.InitialChar();
            disc.Apply(this, row, col);
            return true;
        }

        public char GetCell(int r, int c) => grid[r, c];
        public void SetCell(int r, int c, char ch) => grid[r, c] = ch;

        public List<char> ClearColumnAndReturn(int col)
        {
            var returned = new List<char>();
            for (int r = 0; r < Rows; r++)
            {
                if (grid[r, col] != ' ')
                {
                    returned.Add(grid[r, col]);
                    grid[r, col] = ' ';
                }
            }
            return returned;
        }

        public void ApplyGravity()
        {
            for (int c = 0; c < Columns; c++)
            {
                int write = Rows - 1;
                for (int r = Rows - 1; r >= 0; r--)
                {
                    if (grid[r, c] != ' ')
                    {
                        char tmp = grid[r, c];
                        grid[r, c] = ' ';
                        grid[write, c] = tmp;
                        write--;
                    }
                }
            }
        }

        public void RotateClockwise()
        {
            var newGrid = new char[Columns, Rows];
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    newGrid[c, Rows - 1 - r] = grid[r, c];
            int newRows = Columns, newCols = Rows;
            grid = new char[newRows, newCols];
            Rows = newRows; Columns = newCols;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    grid[r, c] = newGrid[r, c];
        }

        public bool CheckWin(char owned, int need)
        {
            int[] dr = new[] { 1, 0, 1, 1 };
            int[] dc = new[] { 0, 1, 1, -1 };
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (grid[r, c] != owned) continue;
                    for (int k = 0; k < 4; k++)
                    {
                        int cnt = 0, rr = r, cc = c;
                        while (rr >= 0 && rr < Rows && cc >= 0 && cc < Columns && grid[rr, cc] == owned)
                        {
                            cnt++;
                            if (cnt >= need) return true;
                            rr += dr[k]; cc += dc[k];
                        }
                    }
                }
            }
            return false;
        }

        public void TriggerExplosion(int r, int c)
        {
            int[] dr = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dc = { -1, 0, 1, -1, 1, -1, 0, 1 };
            grid[r, c] = ' ';
            for (int i = 0; i < 8; i++)
            {
                int nr = r + dr[i], nc = c + dc[i];
                if (nr >= 0 && nr < Rows && nc >= 0 && nc < Columns)
                {
                    grid[nr, nc] = ' ';
                }
            }
            ApplyGravity();
        }


        public string Serialize()
        {
            var lines = new List<string>();
            for (int r = 0; r < Rows; r++)
            {
                var row = "";
                for (int c = 0; c < Columns; c++)
                    row += grid[r, c];
                lines.Add(row);
            }
            return string.Join("\n", lines);
        }

        public void Deserialize(string data)
        {
            var lines = data.Split('\n');
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    grid[r, c] = lines[r][c];
        }



        public void ShowFrame(string message = "")
        {
            Display();
            if (!string.IsNullOrEmpty(message))
                Console.WriteLine(message);
            System.Threading.Thread.Sleep(500); // 延迟0.5秒模拟动画帧
        }


        private void ApplyMagnetEffect(int row, int col, Disc disc)
        {
            // ✅ 初始落下已在 DropDisc 中处理 ShowFrame("Initial placement")

            // ✅ 激活帧
            ShowFrame("Effect activated (Magnet)");

            char ownerOrd = char.IsUpper(disc.Symbol) ? '@' : '#';

            // 从下方搜索同阵营最近的普通棋子
            int target = -1;
            for (int r = row + 1; r < Rows; r++)
            {
                if (grid[r, col] == ownerOrd) { target = r; break; }
            }

            // 如果找到并且不是紧邻，则上提一格
            if (target != -1 && target > row + 1)
            {
                grid[target - 1, col] = ownerOrd;
                grid[target, col] = ' ';
            }

            // 磁铁自身变成普通棋子
            grid[row, col] = ownerOrd;

            // 最终帧
            ShowFrame("Final state (Magnet)");
        }
    

private void ApplyMagnetEffect(int row, int col, char placedToken)
        {
            ShowFrame("Effect activated (Magnet)");
            char ownerOrd = char.IsUpper(placedToken) ? '@' : '#';
            int target = -1;
            for (int r = row + 1; r < Rows; r++)
            {
                if (grid[r, col] == ownerOrd) { target = r; break; }
            }
            if (target != -1 && target > row + 1)
            {
                grid[target - 1, col] = ownerOrd;
                grid[target, col] = ' ';
            }
            grid[row, col] = ownerOrd;
            ShowFrame("Final state (Magnet)");
        }
    }
}