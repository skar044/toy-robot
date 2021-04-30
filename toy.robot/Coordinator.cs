using System;
using System.Collections.Generic;
using System.Linq;
using Toy.Robot.Board;
using Toy.Robot.Extensions;
using Toy.Robot.Robot;

namespace Toy.Robot
{
    public class Coordinator : ICoordinator
    {
        private readonly IBoard _board;

        public Coordinator(IBoard board)
        {
            _board = board;
            Robots = new List<IRobot>();
        }
        
        public IList<IRobot> Robots { get; }
        public int NumberOfRobotsAllowed { get; set; } = 1;
        
        /// <summary>
        ///     Initialize the board with width and height to create play area.
        ///     Can be re-run to clear the board
        /// </summary>
        /// <exception cref="System.Exception">Throws when width and height are invalid </exception>
        public void InitializeBoard(int width, int height)
        {
            Robots.Clear();
            _board.InitializePlayArea(width, height);
        }

        /// <summary>place a robot with a given id and direction on the board </summary>
        /// <exception cref="System.Exception">Throws when allowed robot limit is reached </exception>
        /// <exception cref="System.Exception">Throws when x and y position are invalid </exception>
        public void Place(Guid robotId, int x, int y, Direction direction)
        {
            var robot = Robots.FirstOrDefault(r => r.RobotId == robotId);
            if (robot == null)
            {
                if (Robots.Count >= NumberOfRobotsAllowed)
                {
                    throw new Exception("Unable to place any more robots on the board as you have reached the allowed limit");
                }
                robot = new Robot.Robot(direction, robotId);
                Robots.Add(robot);
            }
            else
            {
                robot.Direction = direction;
            }
            _board.Place(robotId, x, y);
        }

        /// <summary>place a robot on the board, id will be generated</summary>
        /// <exception cref="System.Exception">Throws when allowed robot limit is reached </exception>
        /// <exception cref="System.Exception">Throws when x and y position are invalid </exception>
        public Guid Place(int x, int y, Direction direction)
        {
            var robotId = Guid.NewGuid();

            Place(robotId, x, y, direction);

            return robotId;
        }

        /// <exception cref="System.Exception">Throws when specified robot does not exist</exception>
        /// <exception cref="System.Exception">Throws when moving to invalid location</exception>
        public void Move(Guid robotId)
        {
            var robot = Robots.FirstOrDefault(r => r.RobotId == robotId);
            if (robot == null)
            {
                throw new Exception("Robot for specified id does not exist, place robot on the board again");
            }

            var currentCoords = _board.PlayArea.IndexOf(robotId);
            if (currentCoords == (-1, -1))
            {
                throw new Exception("Cannot find robot on the board, place same robot on the board again");
            }
            
            var deltaCoords = robot.Move();
            var newX = currentCoords.x + deltaCoords.x;
            var newY = currentCoords.y + deltaCoords.y;
            
            _board.Place(robotId, newX, newY);
        }

        /// <exception cref="System.Exception">Throws when specified robot does not exist</exception>
        public void Left(Guid robotId)
        {
            var robot = Robots.FirstOrDefault(r => r.RobotId == robotId);
            if (robot == null)
            {
                throw new Exception("Robot for specified id does not exist");
            }
            
            robot.Left();
        }

        /// <exception cref="System.Exception">Throws when specified robot does not exist</exception>
        public void Right(Guid robotId)
        {
            var robot = Robots.FirstOrDefault(r => r.RobotId == robotId);
            if (robot == null)
            {
                throw new Exception("Robot for specified id does not exist");
            }
            
            robot.Right();
        }

        /// <exception cref="System.Exception">Throws when a robot in the list does not exist on the board </exception>
        public List<(Guid robotId, int x, int y, Direction direction)> Report()
        {
            var outList = new List<(Guid robotId, int x, int y, Direction direction)>();

            foreach (var robot in Robots)
            {
                var currentCoords = _board.PlayArea.IndexOf(robot.RobotId);
                if (currentCoords == (-1, -1))
                {
                    throw new Exception("Cannot find robot on the board, place same robot on the board again");
                }
                
                outList.Add((robot.RobotId, currentCoords.x, currentCoords.y, robot.Direction));
            }

            return outList;
        }
    }
}