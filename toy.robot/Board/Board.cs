using System;
using Toy.Robot.Extensions;

namespace Toy.Robot.Board
{
    public class Board : IBoard
    {
        public Guid[,] PlayArea { get; private set; }

        public void InitializePlayArea(int width, int height)
        {
            if (width < 1 || height < 1)
            {
                throw new Exception("Width and Height values must be greater than 0");
            }
            PlayArea = new Guid[width, height];
            PlayArea.Initialize();
        }

        public bool IsValidPosition(int x, int y)
        {
            try
            {
                var robot = PlayArea[x, y];
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Place(Guid robotId, int x, int y)
        {
            var currentCoords = PlayArea.IndexOf(robotId);
            var isExistingRobot = currentCoords != (-1, -1);

            if (!IsValidPosition(x, y))
            {
                if (isExistingRobot)
                    throw new Exception(
                        "Unable to move robot. Moving robot in current direction will move it off the board.");
                throw new Exception(
                    $"Unable to place robot at ({x}, {y}). X must be within 0 - {PlayArea.GetLength(0) - 1}, and Y must be within 0 - {PlayArea.GetLength(1) - 1}");
            }
            
            if (isExistingRobot)
            {
                PlayArea[currentCoords.x, currentCoords.y] = Guid.Empty;
            }
            else if (PlayArea[x, y] != Guid.Empty)
            {
                throw new Exception(
                    "Another robot exists in the same location. Please choose a different location for robot");
            }

            PlayArea[x, y] = robotId;
        }
    }
}