using System;

namespace LineUpGame
{
    abstract class Disc
    {
        public char OwnerOrdinary { get; }
        protected bool IsP1 => OwnerOrdinary == '@';
        protected Disc(char ownerOrdinary) { OwnerOrdinary = ownerOrdinary; }
        public abstract char InitialChar();
        public abstract void Apply(Board board, int r, int c);
        public char Symbol { get; }
    }

    class OrdinaryDisc : Disc
    {
        public OrdinaryDisc(char ownerOrdinary) : base(ownerOrdinary) { }
        public override char InitialChar() => OwnerOrdinary;
        public override void Apply(Board board, int r, int c) { }
    }

    class BoringDisc : Disc
    {
        public BoringDisc(char ownerOrdinary) : base(ownerOrdinary) { }
        public override char InitialChar() => IsP1 ? 'B' : 'b';

        public override void Apply(Board board, int r, int c)
        {
            // 初始落下
            board.ShowFrame("Initial placement (Boring)");

            // 激活效果
            board.ShowFrame("Effect activated (Boring)");

            var returned = board.ClearColumnAndReturn(c);
            foreach (var ch in returned) GameRegistry.ReturnToOwner(ch);
            board.SetCell(board.Rows - 1, c, OwnerOrdinary);

            // 最终状态
            board.ShowFrame("Final state (Boring)");
        }
    }

    class MagnetDisc : Disc
    {
        public MagnetDisc(char ownerOrdinary) : base(ownerOrdinary) { }
        public override char InitialChar() => IsP1 ? 'M' : 'm';

        public override void Apply(Board board, int r, int c)
        {
            // 初始落下
            board.ShowFrame("Initial placement (Magnet)");

            // 激活效果
            board.ShowFrame("Effect activated (Magnet)");

            int target = -1;
            for (int rr = r + 1; rr < board.Rows; rr++)
            {
                if (board.GetCell(rr, c) == OwnerOrdinary)
                {
                    target = rr;
                    break;
                }
            }

            if (target != -1 && target > r + 1)
            {
                char above = board.GetCell(target - 1, c);
                board.SetCell(target - 1, c, OwnerOrdinary);
                board.SetCell(target, c, above);
            }

            board.SetCell(r, c, OwnerOrdinary);

            // 最终状态
            board.ShowFrame("Final state (Magnet)");
        }
    }

    class ExplodingDisc : Disc
    {
        public ExplodingDisc(char ownerOrdinary) : base(ownerOrdinary) { }
        public override char InitialChar() => IsP1 ? 'E' : 'e';

        public override void Apply(Board board, int r, int c)
        {
            // 初始落下
            board.ShowFrame("Initial placement (Exploding)");

            // 激活效果
            board.ShowFrame("Effect activated (Exploding)");

            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    int rr = r + dr, cc = c + dc;
                    if (rr >= 0 && rr < board.Rows && cc >= 0 && cc < board.Columns)
                        board.SetCell(rr, cc, ' ');
                }
            board.ApplyGravity();

            // 最终状态
            board.ShowFrame("Final state (Exploding)");
        }
    }
}
