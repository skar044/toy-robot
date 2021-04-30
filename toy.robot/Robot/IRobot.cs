using System;

namespace Toy.Robot.Robot
{
    public interface IRobot
    {
        Guid RobotId { get; }
        Direction Direction { get; set; }
        void Left();
        void Right();
        (int x, int y) Move();
    }
}