using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Toy.Robot.Game.Tests;

namespace Toy.Robot.Game
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameCoordinator = ToyRobot.InitializeGame();
            var commandProcessor = new CommandProcessor(gameCoordinator);
            
            string launchMessage = 
                @"  
                  **************************************
                  **                                  **
                  **          TOY ROBOT GAME          **
                  **                                  **
                  **************************************
                  

                  Place the toy robot (using the command below)
                  on the 5 x 5 grid to begin the game:

                     PLACE X,Y,F 
                     (X and Y must be between 0 - 4)
                     (F is either NORTH, SOUTH, EAST or WEST)

                  Once the Robot is placed the following Commands 
                  can be issued to operate the game:
                                
                     REPORT – Shows the current status of the toy. 
                     LEFT   – turns the toy 90 degrees left.
                     RIGHT  – turns the toy 90 degrees right.
                     MOVE   – Moves the toy 1 unit in the facing direction.
                     TEST   - Perform tests based on the test cases provided
                     EXIT   – Exits the game.
                ";
            
            var stopApplication = false;
            var robotPlaced = false;
            Console.WriteLine(launchMessage);
            do
            {
                Console.Write(robotPlaced ? "Robot>" : "Game>");

                var command = Console.ReadLine();
                if (command == null) continue;

                switch (command.ToLower())
                {
                    case "exit":
                        stopApplication = true;
                        break;
                    case "test":
                        RunTests();
                        break;
                    default:
                        try
                        {
                            command = command.Trim();

                            var input = CalculateInputCommand(command);

                            string report;
                            (robotPlaced, report) = commandProcessor.ProcessCommand(input);

                            if (report != string.Empty)
                            {
                                Console.WriteLine(report);
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        break;
                }
            } while (!stopApplication);
        }

        public static string[] CalculateInputCommand(string command)
        {
            string[] input;
            if (command.IndexOf(' ') > 0)
            {
                var action = command[..command.IndexOf(' ')];
                var arguments = command[command.IndexOf(' ')..];
                input = new[] {action, arguments};
            }
            else
            {
                input = new[] {command};
            }

            return input;
        }

        public static void RunTests()
        {
            Console.WriteLine("Running tests..");
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), @"Tests/TestCases.json");
            var testCases = JsonConvert.DeserializeObject<List<TestCase>>(File.ReadAllText(path));

            if (testCases == null || testCases.Count == 0)
            {
                Console.WriteLine("No tests found");
                return;
            }
            
            Console.WriteLine($"Found {testCases.Count} tests");

            var testNumber = 1;
            foreach (var test in testCases)
            {
                var gameCoordinator = ToyRobot.InitializeGame();
                var commandProcessor = new CommandProcessor(gameCoordinator);
                
                Console.WriteLine($"Running Test {testNumber}");

                try
                {
                    foreach (var action in test.Actions)
                    {
                        var input = CalculateInputCommand(action);

                        commandProcessor.ProcessCommand(input);
                    }

                    var (robotPlaced, report) = commandProcessor.ProcessCommand(new[] {"report"});
                    
                    if (report != string.Empty)
                    {
                        Console.Write($"Test {testNumber}: ");
                        if(report == test.ExpectedResult)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("PASS");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("FAIL");
                            Console.WriteLine();
                            Console.WriteLine($"expected {test.ExpectedResult}, received: {report}");
                        }
                        Console.ResetColor();
                    }
                }
                catch (Exception exception)
                {
                    Console.Write($"Test {testNumber}: ");
                    if(exception.Message == test.ExpectedResult)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("PASS");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("FAIL");
                        Console.WriteLine();
                        Console.WriteLine($"expected {test.ExpectedResult}, received: {exception.Message}");
                    }
                    Console.ResetColor();
                }
                
                testNumber++;
            }
        }
    }
}