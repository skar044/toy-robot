namespace Toy.Robot
{
    public static class ToyRobot
    {
        public static ICoordinator InitializeGame(int boardWidth = 5, int boardHeight = 5, int numberOfRobots = 1)
        {
            var board = new Board.Board();
            var coordinator = new Coordinator(board);
            
            coordinator.InitializeBoard(boardWidth, boardHeight);
            coordinator.NumberOfRobotsAllowed = numberOfRobots;

            return coordinator;
        }
    }
}