using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Toy.Robot.Board;
using Toy.Robot.Extensions;
using Toy.Robot.Robot;

namespace Toy.Robot.Tests.Coordinator
{
    [TestFixture]
    public class CoordinatorTests
    {

        private ICoordinator _coordinator;
        private Mock<IBoard> _mockBoard;
        
        [SetUp]
        public void Setup()
        {
            _mockBoard = new Mock<IBoard>();
            _coordinator = new Toy.Robot.Coordinator(_mockBoard.Object);
        }
        
        [Test]
        public void Place_NewRobot1X1YNorth_RobotPlaced()
        {
            _mockBoard.Setup(m => m.Place(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()));
            _coordinator = new Toy.Robot.Coordinator(_mockBoard.Object);
            _coordinator.InitializeBoard(5, 5);

            var robotId = Guid.NewGuid();
            
            Assert.DoesNotThrow(() =>
            {
                _coordinator.Place(robotId, 1, 1, Direction.North);
            });
            
            _mockBoard.Verify(m => m.Place(robotId, 1, 1), Times.Once);
            
            Assert.AreEqual(1, _coordinator.Robots.Count);
        }

        [Test]
        public void Place_RobotLimitReached_ExceptionThrown()
        {
            _coordinator.InitializeBoard(5, 5);
            _coordinator.Place(Guid.NewGuid(), 1, 1, Direction.North);

            var ex = Assert.Throws<Exception>(() => { _coordinator.Place(Guid.NewGuid(), 1, 1, Direction.North); });
            
            Assert.AreEqual("Unable to place any more robots on the board as you have reached the allowed limit", ex.Message);
            Assert.AreEqual(1, _coordinator.Robots.Count);
        }

        [Test]
        public void Place_NewRobotAtExistingRobotLocation_ExceptionThrownRobotNotAddedToList()
        {
            _coordinator.InitializeBoard(5, 5);
            var robotId = Guid.NewGuid();
            _coordinator.Place(robotId, 1, 1, Direction.North);

            Assert.Throws<Exception>(() =>
            {
                _coordinator.Place(Guid.NewGuid(), 1, 1, Direction.North);
            });
            
            Assert.AreEqual(1, _coordinator.Robots.Count);
            Assert.AreEqual(robotId, _coordinator.Robots.First().RobotId);
        }

        [Test]
        public void Place_ExistingRobotNewLocationNewDirection_RobotPlaced()
        {
            _coordinator.InitializeBoard(5, 5);
            var robotId = Guid.NewGuid();
            _coordinator.Place(robotId, 1, 1, Direction.North);

            try
            {
                _coordinator.Place(robotId, 2, 2, Direction.South);
                
                Assert.AreEqual(1, _coordinator.Robots.Count);
                Assert.AreEqual(robotId, _coordinator.Robots.First().RobotId);
                Assert.AreEqual(Direction.South, _coordinator.Robots.First().Direction);
            }
            catch (Exception)
            {
                Assert.Fail("Throws Exception");
            }
        }

        [Test]
        public void Place_At1X1YNorth_ReturnsGuid()
        {
            _coordinator.InitializeBoard(5, 5);

            try
            {
                var output = _coordinator.Place(1, 1, Direction.North);
                
                Assert.That(output, Is.InstanceOf<Guid>());
                Assert.AreNotEqual(Guid.Empty, output);
            }
            catch (Exception)
            {
                Assert.Fail("Exception Thrown");
            }
        }


        [Test]
        public void Move_RobotDoesNotExistInList_ExceptionThrown()
        {
            _coordinator.InitializeBoard(5, 5);

            var ex = Assert.Throws<Exception>(() =>
            {
                _coordinator.Move(Guid.NewGuid());
            });
            
            Assert.AreEqual("Robot for specified id does not exist, place robot on the board again", ex.Message);
        }

        [Test]
        public void Move_RobotDoesNotExistOnBoard_ExceptionThrown()
        {
            var board = new Toy.Robot.Board.Board();
            board.InitializePlayArea(5, 5);

            var robotId = Guid.NewGuid();
            
            _coordinator = new Toy.Robot.Coordinator(board);
            _coordinator.Robots.Add(new Toy.Robot.Robot.Robot(Direction.North, robotId));

            var ex = Assert.Throws<Exception>(() =>
            {
                _coordinator.Move(robotId);
            });
            
            Assert.AreEqual("Cannot find robot on the board, place same robot on the board again", ex.Message);
        }

        [Test]
        public void Move_RobotExistsAt1X1YNorthFacing_BoardPlaceIsFiredWith1X2Y()
        {
            var playArea = new Guid[5,5];
            playArea.Initialize();
            
            var robotId = Guid.NewGuid();

            playArea[1, 1] = robotId;
            
            _mockBoard = new Mock<IBoard>();
            _mockBoard.Setup(m => m.Place(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()));
            _mockBoard.Setup(m => m.PlayArea).Returns(playArea);

            _coordinator = new Toy.Robot.Coordinator(_mockBoard.Object);
            _coordinator.Robots.Add(new Toy.Robot.Robot.Robot(Direction.North, robotId));
            
            Assert.DoesNotThrow(() =>
            {
                _coordinator.Move(robotId);
            });
            
            _mockBoard.Verify(m => m.Place(robotId, 1, 2), Times.Once(), "Place was not called with correct parameters");
        }

        [Test]
        public void Left_NonExistentRobot_ExceptionThrown()
        {
            _coordinator.InitializeBoard(5, 5);

            var ex = Assert.Throws<Exception>(() =>
            {
                _coordinator.Left(Guid.NewGuid());
            });
            
            Assert.AreEqual("Robot for specified id does not exist", ex.Message);
        }

        [Test]
        public void Left_ExistingRobot_RobotLeftIsFired()
        {
            var robotId = Guid.NewGuid();
            var mockRobot = new Mock<IRobot>();
            mockRobot.Setup(m => m.Left());
            mockRobot.Setup(m => m.RobotId).Returns(robotId);
            
            _coordinator.InitializeBoard(5, 5);
            
            _coordinator.Robots.Add(mockRobot.Object);
            
            Assert.DoesNotThrow(() =>
            {
                _coordinator.Left(robotId);
            });
            
            mockRobot.Verify(m => m.Left(), Times.Once());
        }
        
        [Test]
        public void Right_NonExistentRobot_ExceptionThrown()
        {
            _coordinator.InitializeBoard(5, 5);

            var ex = Assert.Throws<Exception>(() =>
            {
                _coordinator.Right(Guid.NewGuid());
            });
            
            Assert.AreEqual("Robot for specified id does not exist", ex.Message);
        }

        [Test]
        public void Right_ExistingRobot_RobotLeftIsFired()
        {
            var robotId = Guid.NewGuid();
            var mockRobot = new Mock<IRobot>();
            mockRobot.Setup(m => m.Right());
            mockRobot.Setup(m => m.RobotId).Returns(robotId);
            
            _coordinator.InitializeBoard(5, 5);
            
            _coordinator.Robots.Add(mockRobot.Object);
            
            Assert.DoesNotThrow(() =>
            {
                _coordinator.Right(robotId);
            });
            
            mockRobot.Verify(m => m.Right(), Times.Once());
        }

        [Test]
        public void Report_RobotExistsInListNotOnBoard_ExceptionThrown()
        {
            var board = new Toy.Robot.Board.Board();
            _coordinator = new Toy.Robot.Coordinator(board);
            
            _coordinator.InitializeBoard(5, 5);
            
            _coordinator.Robots.Add(new Toy.Robot.Robot.Robot(Direction.North));

            var ex = Assert.Throws<Exception>(() =>
            {
                _coordinator.Report();
            });
            
            Assert.AreEqual("Cannot find robot on the board, place same robot on the board again", ex.Message);
        }

        [Test]
        public void Report_NewRobotAt1X1YNorth_Returns1X1YNorth()
        {
            var board = new Toy.Robot.Board.Board();
            _coordinator = new Toy.Robot.Coordinator(board);
            
            _coordinator.InitializeBoard(5, 5);

            var robotId = Guid.NewGuid();
            
            _coordinator.Place(robotId, 1, 1, Direction.North);

            try
            {
                var report = _coordinator.Report();
                
                Assert.AreEqual(1, report.Count);
                
                var firstRobot = report.First();
                
                Assert.AreEqual(robotId, firstRobot.robotId);
                Assert.AreEqual(1, firstRobot.x);
                Assert.AreEqual(1, firstRobot.y);
                Assert.AreEqual(Direction.North, firstRobot.direction);
                
            }
            catch (Exception)
            {
                Assert.Fail("Exception Thrown");
            }
        }

        [Test]
        public void Report_MultipleRobots_NoExceptionsResultsValid()
        {
            var board = new Toy.Robot.Board.Board();
            _coordinator = new Toy.Robot.Coordinator(board);
            
            _coordinator.InitializeBoard(5, 5);
            _coordinator.NumberOfRobotsAllowed = 4;

            var expectedResults = new List<(Guid robotId, int x, int y, Direction direction)>
            {
                (Guid.NewGuid(), 0, 0, Direction.North),
                (Guid.NewGuid(), 0, 4, Direction.East),
                (Guid.NewGuid(), 4, 0, Direction.West),
                (Guid.NewGuid(), 4, 4, Direction.South)
            };
            
            _coordinator.Place(expectedResults[0].robotId, expectedResults[0].x, expectedResults[0].y, expectedResults[0].direction);
            _coordinator.Place(expectedResults[1].robotId, expectedResults[1].x, expectedResults[1].y, expectedResults[1].direction);
            _coordinator.Place(expectedResults[2].robotId, expectedResults[2].x, expectedResults[2].y, expectedResults[2].direction);
            _coordinator.Place(expectedResults[3].robotId, expectedResults[3].x, expectedResults[3].y, expectedResults[3].direction);
            
            try
            {
                var reportList = _coordinator.Report();
                
                Assert.AreEqual(4, reportList.Count);

                foreach (var expectedResult in expectedResults)
                {
                    var report = reportList.FirstOrDefault(r => r.robotId == expectedResult.robotId);
                    if (report == default)
                    {
                        Assert.Fail($"Robot {expectedResult.robotId} not included in report");
                    }
                    
                    Assert.AreEqual(expectedResult, report);
                }
            }
            catch (Exception)
            {
                Assert.Fail("Exception Thrown");
            }
        }
    }
}