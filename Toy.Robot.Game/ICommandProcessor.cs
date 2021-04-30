namespace Toy.Robot.Game
{
    public interface ICommandProcessor
    {
        (bool isRobotPlaced, string report) ProcessCommand(string[] input);
    }
}