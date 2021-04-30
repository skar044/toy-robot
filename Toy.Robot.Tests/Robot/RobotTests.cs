using NUnit.Framework;
using Toy.Robot.Robot;

namespace Toy.Robot.Tests.Robot
{
    [TestFixture]
    public class RobotTests
    {
        private IRobot _robot; 
            
        [SetUp]
        public void Setup()
        {
            _robot = new Toy.Robot.Robot.Robot(Direction.North);
        }

        [Test]
        public void Left_DefaultNorth_DirectionWest()
        {
            _robot.Left();

            Assert.AreEqual(Direction.West, _robot.Direction, $"expected {Direction.West} got {_robot.Direction}");
        }
        
        [Test]
        public void Right_DefaultNorth_DirectionEast()
        {
            _robot.Right();

            Assert.AreEqual(Direction.East, _robot.Direction, $"expected {Direction.East} got {_robot.Direction}");
        }
        
        [Test]
        public void Right3x_DefaultNorth_DirectionWest()
        {
            _robot.Right();
            _robot.Right();
            _robot.Right();

            Assert.AreEqual(Direction.West, _robot.Direction, $"expected {Direction.West} got {_robot.Direction}");
        }
        
        [Test]
        public void Left3x_DefaultNorth_DirectionEast()
        {
            _robot.Left();
            _robot.Left();
            _robot.Left();

            Assert.AreEqual(Direction.East, _robot.Direction, $"expected {Direction.East} got {_robot.Direction}");
        }
        
        [Test]
        public void RightLeftRight_DefaultNorth_DirectionEast()
        {
            _robot.Right();
            _robot.Left();
            _robot.Right();

            Assert.AreEqual(Direction.East, _robot.Direction, $"expected {Direction.East} got {_robot.Direction}");
        }
        
        [Test]
        public void LeftRightLeft_DefaultNorth_DirectionEast()
        {
            _robot.Left();
            _robot.Right();
            _robot.Left();

            Assert.AreEqual(Direction.West, _robot.Direction, $"expected {Direction.West} got {_robot.Direction}");
        }
        
        [Test]
        public void Left_FacingSouth_DirectionEast()
        {
            _robot = new Toy.Robot.Robot.Robot(Direction.South);
            _robot.Left();

            Assert.AreEqual(Direction.East, _robot.Direction, $"expected {Direction.East} got {_robot.Direction}");
        }
        
        [Test]
        public void Right_FacingSouth_DirectionWest()
        {
            _robot = new Toy.Robot.Robot.Robot(Direction.South);
            _robot.Right();

            Assert.AreEqual(Direction.West, _robot.Direction, $"expected {Direction.West} got {_robot.Direction}");
        }

        static object[] Move_TestCaseSource =
        {
            new object[] {Direction.North, (0, 1)},
            new object[] {Direction.East, (1, 0)},
            new object[] {Direction.South, (0, -1)},
            new object[] {Direction.West, (-1, 0)}
        };

        [TestCaseSource(nameof(Move_TestCaseSource))]
        public void Move_TestCaseParam_TestCaseExpectedResult(Direction direction, (int x, int y) expectedResult)
        {
            _robot = new Toy.Robot.Robot.Robot(direction);

            var moveDelta = _robot.Move();
            
            Assert.AreEqual(expectedResult, moveDelta);
        }
    }
}