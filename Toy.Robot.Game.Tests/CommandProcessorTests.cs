using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Toy.Robot.Robot;

namespace Toy.Robot.Game.Tests
{
    public class CommandProcessorTests
    {
        private ICommandProcessor _commandProcessor;
        private Mock<ICoordinator> _mockCoordinator;
        
        [SetUp]
        public void Setup()
        {
            _mockCoordinator = new Mock<ICoordinator>();
            _commandProcessor = new CommandProcessor(_mockCoordinator.Object);
        }

        [Test]
        public void ProcessCommand_UnknownCommand_ThrowsException()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                _commandProcessor.ProcessCommand(new[] {"test"});
            });
            
            Assert.AreEqual("Command not recognized please try again", ex.Message);
        }

        [Test]
        public void ProcessCommand_PlaceIncorrectNumberOfArguments_ExceptionThrown()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                _commandProcessor.ProcessCommand(new[] {"place"});
            });
            
            Assert.AreEqual("Please provide X, Y location and Facing direction for robot to be placed on board", ex.Message);
        }


        [Test]
        public void ProcessCommand_PlaceIncorrectNumberOfArgumentParameters_ExceptionThrown()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                _commandProcessor.ProcessCommand(new[] {"place", "1,1"});
            });
            
            Assert.AreEqual("Incorrect number of arguments provided for PLACE command. Please provide X, Y location and Facing direction for robot to be placed on board", ex.Message);
        }

        [Test]
        public void ProcessCommand_PlaceInvalidDirection1_ExceptionThrown()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                _commandProcessor.ProcessCommand(new[] {"place", "1,1,test"});
            });
            
            Assert.AreEqual("Please provide valid values for X, Y and Facing direction", ex.Message);
        }
        
        [Test]
        public void ProcessCommand_PlaceInvalidDirection2_ExceptionThrown()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                _commandProcessor.ProcessCommand(new[] {"place", "1,1,5"});
            });
            
            Assert.AreEqual("Invalid format for Facing direction. Please enter from the following directions (North, South, East, West)", ex.Message);
        }
        
        [Test]
        public void ProcessCommand_PlaceInvalidX_ExceptionThrown()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                _commandProcessor.ProcessCommand(new[] {"place", "s,1,north"});
            });
            
            Assert.AreEqual("Please provide valid values for X, Y and Facing direction", ex.Message);
        }
        
        [Test]
        public void ProcessCommand_PlaceInvalidY_ExceptionThrown()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                _commandProcessor.ProcessCommand(new[] {"place", "1,s,north"});
            });
            
            Assert.AreEqual("Please provide valid values for X, Y and Facing direction", ex.Message);
        }

        [Test]
        public void ProcessCommand_PlaceValidInputNewRobot_CoordinatorPlaceFiredReturnsTrue()
        {
            _mockCoordinator.Setup(m =>
                m.Place(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Direction>())).Returns(Guid.NewGuid);

            _commandProcessor = new CommandProcessor(_mockCoordinator.Object);

            try
            {
                var output = _commandProcessor.ProcessCommand(new[] {"place", "1,1,north"});
                
                Assert.AreEqual(true, output.isRobotPlaced);
                Assert.AreEqual(string.Empty, output.report);
                
                _mockCoordinator.Verify(m => m.Place(1, 1, Direction.North));

            }
            catch (Exception)
            {
                Assert.Fail("Exception thrown");
            }
        }

        [Test]
        public void ProcessCommand_PlaceValidInputExistingRobot_CoordinatorPlaceFiredReturnsTrue()
        {
            _mockCoordinator.Setup(m =>
                m.Place(It.IsAny<Guid>(),It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Direction>()));
            
            _mockCoordinator.Setup(m =>
                m.Place(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Direction>())).Returns(Guid.NewGuid);
            
            _commandProcessor = new CommandProcessor(_mockCoordinator.Object);
            
            var output1 = _commandProcessor.ProcessCommand(new[] {"place", "1,1,north"});
            
            try
            {
                var output2 = _commandProcessor.ProcessCommand(new[] {"place", "2,2,south"});
                
                Assert.AreEqual(true, output2.isRobotPlaced);
                Assert.AreEqual(string.Empty, output2.report);
                
                _mockCoordinator.Verify(m => m.Place(It.IsAny<Guid>(),2, 2, Direction.South));

            }
            catch (Exception)
            {
                Assert.Fail("Exception thrown");
            }
        }

        [TestCase("move")]
        [TestCase("left")]
        [TestCase("right")]
        [TestCase("report")]
        public void ProcessCommand_MoveLeftRightReportRobotNotPlaced_ExceptionThrown(string action)
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                _commandProcessor.ProcessCommand(new[] {action});
            });
            
            Assert.AreEqual("Please PLACE robot on board", ex.Message);
        }

        [Test]
        public void ProcessCommand_LeftRobotPlacedValidInput_CoordinatorLeftFiredReturnsTrue()
        {
            _mockCoordinator.Setup(m =>
                m.Place(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Direction>())).Returns(Guid.NewGuid);
            _mockCoordinator.Setup(m => m.Left(It.IsAny<Guid>()));

            _commandProcessor = new CommandProcessor(_mockCoordinator.Object);
            
            _commandProcessor.ProcessCommand(new[] {"place", "1,1,north"});

            try
            {
                var output = _commandProcessor.ProcessCommand(new[] {"left"});
                
                Assert.AreEqual(true, output.isRobotPlaced);
                Assert.AreEqual(string.Empty, output.report);
                
                _mockCoordinator.Verify(m => m.Left(It.IsAny<Guid>()));
            }
            catch (Exception)
            {
                Assert.Fail("Exception thrown");
            }
        }
        
        [Test]
        public void ProcessCommand_RightRobotPlacedValidInput_CoordinatorRightFiredReturnsTrue()
        {
            _mockCoordinator.Setup(m =>
                m.Place(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Direction>())).Returns(Guid.NewGuid);
            _mockCoordinator.Setup(m => m.Right(It.IsAny<Guid>()));

            _commandProcessor = new CommandProcessor(_mockCoordinator.Object);
            
            _commandProcessor.ProcessCommand(new[] {"place", "1,1,north"});

            try
            {
                var output = _commandProcessor.ProcessCommand(new[] {"right"});
                
                Assert.AreEqual(true, output.isRobotPlaced);
                Assert.AreEqual(string.Empty, output.report);
                
                _mockCoordinator.Verify(m => m.Right(It.IsAny<Guid>()));
            }
            catch (Exception)
            {
                Assert.Fail("Exception thrown");
            }
        }
        
        [Test]
        public void ProcessCommand_MoveRobotPlacedValidInput_CoordinatorMoveFiredReturnsTrue()
        {
            _mockCoordinator.Setup(m =>
                m.Place(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Direction>())).Returns(Guid.NewGuid);
            _mockCoordinator.Setup(m => m.Move(It.IsAny<Guid>()));

            _commandProcessor = new CommandProcessor(_mockCoordinator.Object);
            
            _commandProcessor.ProcessCommand(new[] {"place", "1,1,north"});

            try
            {
                var output = _commandProcessor.ProcessCommand(new[] {"move"});
                
                Assert.AreEqual(true, output.isRobotPlaced);
                Assert.AreEqual(string.Empty, output.report);
                
                _mockCoordinator.Verify(m => m.Move(It.IsAny<Guid>()));
            }
            catch (Exception)
            {
                Assert.Fail("Exception thrown");
            }
        }

        [Test]
        public void ProcessCommand_ReportReturnsGuid1X1YNorth_Output1X1YNorth()
        {
            _mockCoordinator.Setup(m =>
                m.Place(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Direction>())).Returns(Guid.NewGuid);
            _mockCoordinator.Setup(m => m.Report()).Returns(new List<(Guid, int, int, Direction)>()
                {(Guid.NewGuid(), 1, 1, Direction.North)});
            
            _commandProcessor = new CommandProcessor(_mockCoordinator.Object);
            
            _commandProcessor.ProcessCommand(new[] {"place", "1,1,north"});

            try
            {
                var output = _commandProcessor.ProcessCommand(new[] {"report"});
                
                _mockCoordinator.Verify(m => m.Report());
                
                Assert.AreEqual(true, output.isRobotPlaced);
                Assert.AreEqual("Output: 1,1,North", output.report);
            }
            catch (Exception)
            {
                Assert.Fail("Exception Thrown");
            }
        }
    }
}