using System;
using NUnit.Framework;
using Toy.Robot.Board;
using Toy.Robot.Extensions;

namespace Toy.Robot.Tests.Board
{
    [TestFixture]
    public class BoardTests
    {
        private IBoard _board;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _board = new Toy.Robot.Board.Board();
        }

        [TestCase(5, 5)]
        [TestCase(10, 10)]
        public void InitializePlayArea_TestCaseParam_ValidBoard(int width, int height)
        {
            _board.InitializePlayArea(width, height);

            var boardWidth = _board.PlayArea.GetLength(0);
            var boardHeight = _board.PlayArea.GetLength(1);
            
            Assert.AreEqual(width, boardWidth);
            Assert.AreEqual(height, boardHeight);
        }
        
        
        [TestCase(-1, 5)]
        [TestCase(5, -1)]
        public void InitializePlayArea_TestCaseParam_ThrowsException(int width, int height)
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                _board.InitializePlayArea(width, height);
            });
            
            Assert.AreEqual("Width and Height values must be greater than 0", ex.Message);
        }

        [TestCase(0, 0)]
        [TestCase(3, 3)]
        public void IsValidPosition_TestCaseParam_True(int x, int y)
        {
            _board.InitializePlayArea(5, 5);
            var isValid = _board.IsValidPosition(x, y);
            
            Assert.True(isValid);
        }

        [TestCase(10, 3)]
        [TestCase(3, 10)]
        [TestCase(6, 6)]
        [TestCase(-1, -1)]
        public void IsValidPosition_TestCaseParam_False(int x, int y)
        {
            _board.InitializePlayArea(5, 5);
            var isValid = _board.IsValidPosition(x, y);
            
            Assert.False(isValid);
        }
        
        [Test]
        public void Place_ParamNewRobot1X1Y_RobotPlaced()
        {
            _board.InitializePlayArea(5, 5);
            var robotGuid = Guid.NewGuid();
            _board.Place(robotGuid, 1, 1);
            var coords = _board.PlayArea.IndexOf(robotGuid);
            
            Assert.AreEqual((1, 1), coords, $"expected {(1,1)}, returned {coords}");
        }
        
        [TestCase(6, 1)]
        [TestCase(1, 6)]
        [TestCase(-1, -1)]
        public void Place_TestCaseParamNewRobot_ExceptionThrown(int x, int y)
        {
            _board.InitializePlayArea(5, 5);
            var robotGuid = Guid.NewGuid();
            
            var ex = Assert.Throws<Exception>(() => { _board.Place(robotGuid, x, y); });

            var expectedMessage =
                $"Unable to place robot at ({x}, {y}). X must be within 0 - 4, and Y must be within 0 - 4";
            
            Assert.AreEqual(expectedMessage, ex.Message, $"expected message: {expectedMessage}, returned message: {ex.Message}");
        }

        [Test]
        public void Place_NewRobotAtExistingRobotLocation_ExceptionThrown()
        {
            _board.InitializePlayArea(5, 5);
            _board.Place(Guid.NewGuid(), 1, 1);

            var ex = Assert.Throws<Exception>(() => { _board.Place(Guid.NewGuid(), 1, 1); });
            
            Assert.AreEqual("Another robot exists in the same location. Please choose a different location for robot", ex.Message);
        }

        [Test]
        public void Place_ParamExistingRobotAt1X1YMoveTo1X2Y_RobotPlaced()
        {
            _board.InitializePlayArea(5, 5);
            var robotGuid = Guid.NewGuid();
            _board.Place(robotGuid, 1, 1);
            _board.Place(robotGuid, 1, 2);
            var coords = _board.PlayArea.IndexOf(robotGuid);
            
            Assert.AreEqual((1, 2), coords, $"expected {(1,2)}, returned {coords}");
            Assert.AreEqual(Guid.Empty, _board.PlayArea[1, 1]);
        }

        [Test]
        public void Place_ParamExistingRobotAt0X0YMoveToNeg1X0Y_ExceptionThrown()
        {
            _board.InitializePlayArea(5, 5);
            var robotGuid = Guid.NewGuid();
            _board.Place(robotGuid, 0, 0);
            
            var ex = Assert.Throws<Exception>(() =>
            {
                _board.Place(robotGuid, -1, 0);
            });
            
            var expectedMessage =
                "Unable to move robot. Moving robot in current direction will move it off the board.";
            
            Assert.AreEqual(expectedMessage, ex.Message, $"expected message: {expectedMessage}, returned message: {ex.Message}");
        }
    }
}