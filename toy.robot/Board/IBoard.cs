using System;

namespace Toy.Robot.Board
{
    public interface IBoard
    {
        Guid[,] PlayArea { get; }
        void InitializePlayArea(int width, int height);
        bool IsValidPosition(int x, int y);
        void Place(Guid robotId, int x, int y);
    }
}