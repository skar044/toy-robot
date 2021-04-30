using System;
using System.Linq;
using Toy.Robot.Robot;

namespace Toy.Robot.Game
{
    public class CommandProcessor : ICommandProcessor 
    {
        private readonly ICoordinator _coordinator;
        private Guid _currentRobot = Guid.Empty;

        public CommandProcessor(ICoordinator coordinator)
        {
            _coordinator = coordinator;
        }
        
        public (bool isRobotPlaced, string report) ProcessCommand(string[] input)
        {
            var command = input[0].ToLower();

            switch (command)
            {
                case "place":
                    if (input.Length == 1)
                        throw new Exception(
                            "Please provide X, Y location and Facing direction for robot to be placed on board");
                    var args = CalculatePlaceArguments(input[1]);
                    if (_currentRobot == Guid.Empty)
                    {
                        _currentRobot = _coordinator.Place(args.x, args.y, args.direction);
                    }
                    else
                    {
                        _coordinator.Place(_currentRobot, args.x, args.y, args.direction);
                    }
                    break;
                case "left":
                    if (_currentRobot == Guid.Empty)
                        throw new Exception("Please PLACE robot on board");
                    _coordinator.Left(_currentRobot);
                    break;
                case "right":
                    if (_currentRobot == Guid.Empty)
                        throw new Exception("Please PLACE robot on board");
                    _coordinator.Right(_currentRobot);
                    break;
                case "move":
                    if (_currentRobot == Guid.Empty)
                        throw new Exception("Please PLACE robot on board");
                    _coordinator.Move(_currentRobot);
                    break;
                case "report":
                    if (_currentRobot == Guid.Empty)
                        throw new Exception("Please PLACE robot on board");
                    var report = _coordinator.Report().First();
                    var reportString = $"Output: {report.x},{report.y},{report.direction}";
                    return (_currentRobot != Guid.Empty, reportString);
                default:
                    throw new Exception("Command not recognized please try again");
            }

            return (_currentRobot != Guid.Empty, string.Empty);
        }

        private (int x, int y, Direction direction) CalculatePlaceArguments(string args)
        {
            var argArray = args.Split(',');
            if (argArray.Length != 3)
                throw new Exception(
                    "Incorrect number of arguments provided for PLACE command. Please provide X, Y location and Facing direction for robot to be placed on board");
            
            
            var isValidX = Int32.TryParse(argArray[0].Trim(), out var x);
            var isValidY = Int32.TryParse(argArray[1].Trim(), out var y);

            if (Int32.TryParse(argArray[2].Trim(), out var dir))
            {
                throw new Exception("Invalid format for Facing direction. Please enter from the following directions (North, South, East, West)");
            }
            
            var isValidDirection = Enum.TryParse<Direction>(argArray[2].Trim(), true, out var direction);


            if (isValidDirection && isValidX && isValidY)
            {
                return (x, y, direction);
            }

            throw new Exception("Please provide valid values for X, Y and Facing direction");
        }
    }
}