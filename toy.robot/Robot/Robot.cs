using System;

namespace Toy.Robot.Robot
{
    public class Robot : IRobot
    {
        public Robot(Direction direction)
        {
            Direction = direction;
            RobotId = Guid.NewGuid();
        }

        public Robot(Direction direction, Guid robotId)
        {
            Direction = direction;
            RobotId = robotId;
        }
        
        public Guid RobotId { get; }
        public Direction Direction { get; set; }
        
        public void Left()
        {
            var direction = Direction - 1;

            Direction = (int)direction == 0 ? Direction.West : direction;
        }

        public void Right()
        {
            var direction = Direction + 1;

            Direction = (int)direction == 5 ? Direction.North : direction;
        }

        public (int x, int y) Move()
        {
            return Direction switch
            {
                Direction.North => (0, 1),
                Direction.East => (1, 0),
                Direction.South => (0, -1),
                Direction.West => (-1, 0),
                _ => (0, 0)
            };
        }
    }
}