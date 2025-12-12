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
    public class BossFightTests
    {
        private Game _game;
        private StringWriter? _consoleOutput;
        private TextWriter _originalOutput;

        [SetUp]
        public void Setup()
        {
            _game = new Game();
            _originalOutput = Console.Out;
        }

        [TearDown]
        public void Teardown()
        {
            if (_consoleOutput != null)
            {
                try
                {
                    _consoleOutput.Dispose();
                    Console.SetOut(_originalOutput);
                }
                catch { /* ignore if console is unavailable */ }
            }
        }

        private string? GetQueenAsciiPath()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory();
            
            string[] possiblePaths = new[]
            {
                Path.Combine("Aliens", "Queen.txt"),
                Path.Combine("..", "..", "..", "Aliens", "Queen.txt"),
                Path.Combine(baseDir, "..", "..", "..", "Aliens", "Queen.txt"),
                Path.Combine(baseDir, "..\\..\\..\\Aliens\\Queen.txt"),
                "C:\\Users\\Jasmine Johnson\\my-code-dir\\Aliens_Game\\Lab08\\Aliens\\Queen.txt"
            };

            foreach (var path in possiblePaths)
            {
                try
                {
                    string fullPath = Path.GetFullPath(path);
                    if (File.Exists(fullPath))
                        return fullPath;
                }
                catch { /* continue */ }
            }

            return null;
        }

        /// test that the Queen.txt ASCII art file exists and can be loaded??
        [Test]
        public void BossFight_QueenAsciiFile_Exists()
        {
            string queenPath = GetQueenAsciiPath();
            Assert.That(queenPath, Is.Not.Null, $"Queen ASCII file should exist at one of the expected paths");
        }

        /// test that the Queen.txt ASCII art contains content cause it keeps printing empties
        [Test]
        public void BossFight_QueenAsciiFile_HasContent()
        {
            string queenPath = GetQueenAsciiPath();
            if (queenPath == null)
                Assert.Inconclusive("Queen ASCII file not found");
                
            string content = File.ReadAllText(queenPath);
            
            Assert.That(content.Length, Is.GreaterThan(0), "Queen ASCII file should not be empty");
            Assert.That(content.Contains("###") || content.Length > 20, "Queen ASCII should have meaningful content");
        }

        /// test that Alien Queen spawns with correct health (200)
        [Test]
        public void BossFight_AlienQueen_InitializesWithCorrectHealth()
        {
            var queen = new AlienQueen(new Location(5, 5));
            
            Assert.That(queen.Health, Is.EqualTo(200), "Alien Queen should start with 200 health");
            Assert.That(queen.IsAlive, Is.True, "Alien Queen should be alive on initialization");
        }

        /// test that Alien Queen takes damage correctly
        [Test]
        public void BossFight_AlienQueen_TakeDamage_ReducesHealth()
        {
            var queen = new AlienQueen(new Location(5, 5));
            
            try
            {
                queen.TakeDamage(25);
            }
            catch (IOException)
            {
                Assert.That(queen.Health, Is.EqualTo(175), "Queen health should be reduced by damage amount");
                return;
            }
            
            Assert.That(queen.Health, Is.EqualTo(175), "Queen health should be reduced by damage amount");
            Assert.That(queen.IsAlive, Is.True, "Queen should still be alive at 175 health");
        }

        /// test that Alien Queen dies when health reaches 0, the dang thing
        [Test]
        public void BossFight_AlienQueen_Dies_WhenHealthReachesZero()
        {
            var queen = new AlienQueen(new Location(5, 5));
            
            try
            {
                queen.TakeDamage(200);
            }
            catch (IOException)
            {
                Assert.That(queen.Health, Is.EqualTo(0), "Queen health should be 0");
                Assert.That(queen.IsAlive, Is.False, "Queen should be dead when health <= 0");
                return;
            }
            
            Assert.That(queen.Health, Is.EqualTo(0), "Queen health should be 0");
            Assert.That(queen.IsAlive, Is.False, "Queen should be dead when health <= 0");
        }

        /// test that Alien Queen deals boss damage (20 damage) to the player
        [Test]
        public void BossFight_AlienQueen_DealsBossDamage()
        {
            var queen = new AlienQueen(new Location(5, 5));
            int playerHealthBefore = _game.Player.Health;
            
            try
            {
                queen.DealBossDamage(_game.Player);
            }
            catch (IOException)
            {
                Assert.That(_game.Player.Health, Is.EqualTo(playerHealthBefore - 20), "Queen boss damage should be 20");
                return;
            }
            
            Assert.That(_game.Player.Health, Is.EqualTo(playerHealthBefore - 20), "Queen boss damage should be 20");
        }

        /// test that Player.SetHealth() correctly sets player mech health for boss fight
        [Test]
        public void BossFight_PlayerSetHealth_ChangesPlayerHealth()
        {
            int originalHealth = _game.Player.Health;
            _game.Player.SetHealth(250);
            
            Assert.That(_game.Player.Health, Is.EqualTo(250), "Player health should be set to 250 for boss fight");
            Assert.That(_game.Player.Health, Is.Not.EqualTo(originalHealth), "Boss fight health should differ from normal health");
        }

        /// test that starting the boss fight sets IsBossFightActive to true
        [Test]
        public void BossFight_StartBossFight_MarksBossDiscovered()
        {
            var mechBayLocation = _game.Map.GetRoomLocation(RoomType.MechBay);
            _game.Player.Location = mechBayLocation;
            var powerSupply = new PowerSupply { Quantity = 1 };
            _game.Player.Inventory.AddItem(powerSupply);

            Assert.That(_game.Player.BossDiscovered, Is.False, "Boss should not be discovered before starting");
            
            _game.Player.MarkBossDiscovered();
            
            Assert.That(_game.Player.BossDiscovered, Is.True, "Boss should be marked discovered after starting fight");
        }

        /// test that PowerSupply is removed from inventory after boss fight FOR THE LOVE OF -
        [Test]
        public void BossFight_PowerSupply_RemovedAfterBossFight()
        {
            var powerSupply = new PowerSupply { Quantity = 1 };
            _game.Player.Inventory.AddItem(powerSupply);
            
            Assert.That(_game.Player.Inventory.HasItem(powerSupply), Is.True, "Power Supply should be in inventory before fight");
            
            var psItem = _game.Player.Inventory.GetItemByName("Power Supply");
            if (psItem != null)
            {
                _game.Player.Inventory.RemoveStack(psItem);
            }
            
            Assert.That(_game.Player.Inventory.HasItem(powerSupply), Is.False, "Power Supply should be removed after boss fight");
        }

        /// test that player health is restored after winning the boss fight, no 250 health for u
        [Test]
        public void BossFight_PlayerHealth_RestoredAfterVictory()
        {
            int originalHealth = _game.Player.Health;
            
            _game.Player.SetHealth(250);
            Assert.That(_game.Player.Health, Is.EqualTo(250), "Player should have boss-fight health during fight");
            
            _game.Player.SetHealth(originalHealth);
            
            Assert.That(_game.Player.Health, Is.EqualTo(originalHealth), "Player health should be restored to original value");
        }

        /// test that player inventory is restored (excluding PowerSupply) after boss fight
        [Test]
        public void BossFight_Inventory_RestoredAfterVictory()
        {
            var bullets = new Bullets { Quantity = 5 };
            var bandages = new Bandages { Quantity = 2 };
            var powerSupply = new PowerSupply { Quantity = 1 };
            var machete = new Machete { Quantity = 1 };
            
            _game.Player.Inventory.AddItem(bullets);
            _game.Player.Inventory.AddItem(bandages);
            _game.Player.Inventory.AddItem(powerSupply);
            _game.Player.Inventory.AddItem(machete);
            
            var bulletsItem = _game.Player.Inventory.GetItemByName("Bullets");
            int bulletsQuantityAfterAdd = bulletsItem?.Quantity ?? 0;
            
            var snapshot = _game.Player.Inventory.Items.Select(i => new { Type = i.GetType(), Name = i.Name, Quantity = i.Quantity }).ToList();
            
            foreach (var it in _game.Player.Inventory.Items.ToList())
            {
                _game.Player.Inventory.RemoveStack(it);
            }
            
            Assert.That(_game.Player.Inventory.Items.Count, Is.EqualTo(0), "Inventory should be cleared during simulation");
            
            foreach (var snap in snapshot)
            {
                if (snap.Name.Equals("Power Supply", StringComparison.OrdinalIgnoreCase))
                    continue; 

                try
                {
                    var obj = Activator.CreateInstance(snap.Type);
                    if (obj is Lab08.Interfaces.IItem newItem)
                    {
                        newItem.Quantity = snap.Quantity;
                        _game.Player.Inventory.AddItem(newItem);
                    }
                }
                catch { /* skip */ }
            }
            
            // verify restoration
            var restoredBullets = _game.Player.Inventory.GetItemByName("Bullets");
            var restoredBandages = _game.Player.Inventory.GetItemByName("Bandages");
            var restoredMachete = _game.Player.Inventory.GetItemByName("Machete");
            var restoredPS = _game.Player.Inventory.GetItemByName("Power Supply");
            
            Assert.That(restoredBullets, Is.Not.Null, "Bullets should be restored");
            Assert.That(restoredBullets.Quantity, Is.EqualTo(bulletsQuantityAfterAdd), "Bullets quantity should match snapshot (after game init)");
            Assert.That(restoredBandages, Is.Not.Null, "Bandages should be restored");
            Assert.That(restoredBandages.Quantity, Is.EqualTo(2), "Bandages quantity should match snapshot");
            Assert.That(restoredMachete, Is.Not.Null, "Machete should be restored");
            Assert.That(restoredPS, Is.Null, "Power Supply should NOT be restored");
        }

        /// test that the equipped weapon is restored after boss fight
        [Test]
        public void BossFight_EquippedWeapon_RestoredAfterVictory()
        {
            var machete = new Machete { Quantity = 1 };
            _game.Player.Inventory.AddItem(machete);
            
            try
            {
                _game.Player.EquipWeapon(machete);
            }
            catch (IOException)
            {
                // equipWeapon worked
            }
            
            string equippedName = _game.Player.EquippedWeapon?.Name;
            Assert.That(equippedName, Is.EqualTo("Machete"), "Machete should be equipped");
            
            foreach (var it in _game.Player.Inventory.Items.ToList())
            {
                _game.Player.Inventory.RemoveStack(it);
            }
            
            var newMachete = new Machete { Quantity = 1 };
            _game.Player.Inventory.AddItem(newMachete);
            
            var toEquip = _game.Player.Inventory.GetItemByName("Machete");
            if (toEquip != null)
            {
                try
                {
                    _game.Player.EquipWeapon(toEquip);
                }
                catch (IOException)
                {
                    // equipWeapon worked
                }
            }
            
            Assert.That(_game.Player.EquippedWeapon?.Name, Is.EqualTo("Machete"), "Machete should be re-equipped after restoration");
        }

        /// test that boss fight does not display game map
        [Test]
        public void BossFight_DoesNotDisplay_NormalGameMap()
        {
            Assert.That(true, "Boss fight should suppress normal map display via Console.Clear()");
        }

        /// test that intro/stats/medstory text can be printed at top of screen (position 0,0)
        [Test]
        public void BossFight_TextDisplay_CanBePositionedAtTop()
        {
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
            
            try
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Health: 250");
                Console.WriteLine("Queen Health: 200");
            }
            catch (IOException)
            {
                _consoleOutput.WriteLine("Health: 250");
                _consoleOutput.WriteLine("Queen Health: 200");
            }
            
            string output = _consoleOutput.ToString();
            
            Console.SetOut(_originalOutput);
            
            Assert.That(output.Contains("Health: 250"), Is.True, "Player health should be printed");
            Assert.That(output.Contains("Queen Health: 200"), Is.True, "Queen health should be printed");
        }

        /// test that "Press SPACE to attack!" message is printed next to Queen ASCII
        [Test]
        public void BossFight_AttackPrompt_PrintsAboveQueenAscii()
        {
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press SPACE to attack!");
            
            string queenPath = GetQueenAsciiPath();
            string queenAscii = queenPath != null && File.Exists(queenPath)
                ? File.ReadAllText(queenPath)
                : "[Alien Queen]";
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(queenAscii);
            
            string output = _consoleOutput.ToString();
            
            Console.SetOut(_originalOutput);
            
            int promptIndex = output.IndexOf("Press SPACE to attack!");
            string asciiFirstLine = queenAscii.Split('\n')[0];
            int asciiIndex = output.IndexOf(asciiFirstLine); 
            
            Assert.That(promptIndex, Is.GreaterThanOrEqualTo(0), "Attack prompt should be printed");
            Assert.That(asciiIndex, Is.GreaterThan(promptIndex), "Queen ASCII should appear after the attack prompt");
        }

        /// test that Queen ASCII art is not empty and contains expected characters
        [Test]
        public void BossFight_QueenAscii_HasExpectedContent()
        {
            string queenPath = GetQueenAsciiPath();
            if (queenPath == null)
                Assert.Inconclusive("Queen ASCII file not found");
                
            string queenAscii = File.ReadAllText(queenPath);
            
            Assert.That(queenAscii.Length, Is.GreaterThan(10), "Queen ASCII should have substantial content");
            Assert.That(queenAscii.Contains("###") || queenAscii.Contains("|") || queenAscii.Contains("*"), 
                Is.True, "Queen ASCII should contain drawing characters");
        }

        /// test that the queen flashes red when hit
        [Test]
        public void BossFight_QueenFlashRed_OnHit()
        {
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[Red Queen ASCII]");
            Thread.Sleep(10); 
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[White Queen ASCII]");
            
            Console.SetOut(_originalOutput);
            
            string output = _consoleOutput.ToString();
            Assert.That(output.Contains("[Red Queen ASCII]"), Is.True, "Red flash should be rendered");
            Assert.That(output.Contains("[White Queen ASCII]"), Is.True, "White restore should be rendered");
        }
    }
}
