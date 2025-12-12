using NUnit.Framework;
using Lab08.GameDesign;
using Lab08.Aliens;
using Lab08.Items;
using Lab08.Displays;
using System.Text;
using System.IO;

namespace Lab08.Tests
{
    [TestFixture]
    public class BossFightManualTest
    {
        /// simulates reaching the boss fight and verifies that the Queen ASCII is loaded and would be printed
        [Test]
        public void ManualTest_QueenAsciiLoadsCorrectly()
        {
            var game = new Game();
            var mechBayLocation = game.Map.GetRoomLocation(RoomType.MechBay);
            game.Player.Location = mechBayLocation;
            var powerSupply = new PowerSupply { Quantity = 1 };
            game.Player.Inventory.AddItem(powerSupply);

            var originalOut = Console.Out;
            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);

                try
                {
                    // manually load the Queen ASCII the same way StartBossFight does
                    string queenAscii = "";
                    try
                    {
                        string[] possiblePaths = new[]
                        {
                            Path.Combine("Aliens", "Queen.txt"),
                            Path.Combine("..", "Lab08", "Aliens", "Queen.txt"),
                            Path.Combine("Lab08", "Aliens", "Queen.txt"),
                            Path.Combine("..", "..", "..", "..", "Lab08", "Aliens", "Queen.txt"),
                            Path.Combine("..", "..", "..", "Aliens", "Queen.txt"),
                        };

                        foreach (var path in possiblePaths)
                        {
                            if (File.Exists(path))
                            {
                                queenAscii = File.ReadAllText(path);
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(queenAscii))
                        {
                            queenAscii = "[Alien Queen]";
                        }
                    }
                    catch
                    {
                        queenAscii = "[Alien Queen]";
                    }

                    // check the Queen ASCII was loaded (should be more than just the placeholder!!)
                    Assert.That(queenAscii, Is.Not.Null, "Queen ASCII should not be null");
                    Assert.That(queenAscii.Length, Is.GreaterThan(0), "Queen ASCII should not be empty");
                    
                    // if it's the full ASCII art, it should be much longer than the placeholder :)
                    if (queenAscii != "[Alien Queen]")
                    {
                        Assert.That(queenAscii.Length, Is.GreaterThan(100), 
                            "Queen ASCII file should contain substantial content (not just placeholder)");
                        Assert.That(queenAscii.Contains("\n"), Is.True, 
                            "Queen ASCII should be multi-line");
                    }
                    
                    // check it has ASCII art characters
                    bool hasArtCharacters = queenAscii.Contains("#") || queenAscii.Contains("|") || 
                                          queenAscii.Contains("*") || queenAscii.Contains("=");
                    Assert.That(hasArtCharacters, Is.True, 
                        "Queen ASCII should contain drawing characters");

                    Console.WriteLine("✓ Queen ASCII loaded successfully");
                    Console.WriteLine($"✓ ASCII length: {queenAscii.Length} characters");
                    Console.WriteLine($"✓ ASCII lines: {queenAscii.Split('\n').Length}");
                }
                finally
                {
                    Console.SetOut(originalOut);
                    string output = writer.ToString();
                    TestContext.WriteLine("Test output:");
                    TestContext.WriteLine(output);
                }
            }
        }

        /// checks all possible paths to Queen.txt cause I copied it like a fool
        [Test]
        public void ManualTest_QueenAsciiPathResolution()
        {
            string queenPath = null;
            
            string[] possiblePaths = new[]
            {
                Path.Combine("Aliens", "Queen.txt"),
                Path.Combine("..", "Lab08", "Aliens", "Queen.txt"),
                Path.Combine("Lab08", "Aliens", "Queen.txt"),
                Path.Combine("..", "..", "..", "..", "Lab08", "Aliens", "Queen.txt"),
                Path.Combine("..", "..", "..", "Aliens", "Queen.txt"),
            };

            TestContext.WriteLine("Checking Queen.txt paths:");
            TestContext.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
            TestContext.WriteLine("");

            foreach (var path in possiblePaths)
            {
                bool exists = File.Exists(path);
                string fullPath = Path.GetFullPath(path);
                TestContext.WriteLine($"  {path}: {(exists ? "FOUND" : "not found")}");
                TestContext.WriteLine($"    Full path: {fullPath}");
                
                if (exists && queenPath == null)
                {
                    queenPath = path;
                }
            }

            Assert.That(queenPath, Is.Not.Null, 
                "Queen.txt should be found in at least one of the expected paths");
            
            TestContext.WriteLine($"\n✓ Queen.txt found at: {queenPath}");
        }
    }
}
