using System;
using System.Collections.Generic;
using Toy.Robot.Robot;

namespace Toy.Robot
{
    public interface ICoordinator
    {
        
        IList<IRobot> Robots { get; }
        int NumberOfRobotsAllowed { get; set; }

        /// <summary>
        ///     Initialize the board with width and height to create play area.
        ///     Can be re-run to clear the board
        /// </summary>
        /// <exception cref="System.Exception">Throws when width and height are invalid </exception>
        void InitializeBoard(int width, int height);
        
        /// <summary>place a robot with a given id and direction on the board </summary>
        /// <exception cref="System.Exception">Throws when allowed robot limit is reached </exception>
        /// <exception cref="System.Exception">Throws when x and y position are invalid </exception>
        void Place(Guid robotId, int x, int y, Direction direction);

        /// <summary>place a robot on the board, id will be generated</summary>
        /// <exception cref="System.Exception">Throws when allowed robot limit is reached </exception>
        /// <exception cref="System.Exception">Throws when x and y position are invalid </exception>
        Guid Place(int x, int y, Direction direction);
        
        /// <summary> move the specified robot in the direction it is currently facing</summary>
        /// <exception cref="System.Exception">Throws when specified robot does not exist</exception>
        /// <exception cref="System.Exception">Throws when moving to invalid location</exception>
        void Move(Guid robotId);
        /// <exception cref="System.Exception">Throws when specified robot does not exist</exception>
        void Left(Guid robotId);
        /// <exception cref="System.Exception">Throws when specified robot does not exist</exception>
        void Right(Guid robotId);
        
        /// <summary> Report on current state of robots on the board</summary>
        /// <exception cref="System.Exception">Throws when a robot in the list does not exist on the board </exception>
        List<(Guid robotId, int x, int y, Direction direction)> Report();
    }
}