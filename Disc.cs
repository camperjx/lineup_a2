using System;

namespace LineUpGame
{
    public abstract class Disc
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
            // 1. Initially drop on the board
            Decorator.Render(new Parameters(board, "Initial placement (Boring)", 500));

            // 2. Activate effect
            Decorator.Render(new Parameters(board, "Effect activated (Boring)", 500));

            var returned = board.ClearColumnAndReturn(c);
            foreach (var ch in returned) GameRegistry.ReturnToOwner(ch);
            board.SetCell(board.Rows - 1, c, OwnerOrdinary);

            // 3. Final state
            Decorator.Render(new Parameters(board, "Final state (Boring)", 500));
        }
    }

    class MagnetDisc : Disc
    {
        public MagnetDisc(char ownerOrdinary) : base(ownerOrdinary) { }
        public override char InitialChar() => IsP1 ? 'M' : 'm';

        public override void Apply(Board board, int r, int c)
        {
            Decorator.Render(new Parameters(board, "Initial placement (Magnet)", 500));

            Decorator.Render(new Parameters(board, "Effect activated (Magnet)", 500));

            int r1 = r + 1, r2 = r + 2;
            if (r2 < board.Rows)
            {
                char a = board.GetCell(r1, c);
                char b = board.GetCell(r2, c);

                board.SetCell(r1, c, b);
                board.SetCell(r2, c, a);
            }

            board.SetCell(r, c, OwnerOrdinary);

            Decorator.Render(new Parameters(board, "Final state (Magnet)", 500));
        }
    }

    class ExplodingDisc : Disc
    {
        public ExplodingDisc(char ownerOrdinary) : base(ownerOrdinary) { }
        public override char InitialChar() => IsP1 ? 'E' : 'e';

        public override void Apply(Board board, int r, int c)
        {
            // 1. Initially drop on the board
            Decorator.Render(new Parameters(board, "Initial placement (Exploding)", 500));

            // 2. Activate effect
            Decorator.Render(new Parameters(board, "Effect activated (Exploding)", 500));

            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    int rr = r + dr, cc = c + dc;
                    if (rr >= 0 && rr < board.Rows && cc >= 0 && cc < board.Columns)
                        board.SetCell(rr, cc, ' ');
                }
            board.ApplyGravity();

            // 3. Final state
            Decorator.Render(new Parameters(board, "Final state (Exploding)", 500));
        }
    }
}
