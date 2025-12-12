using NUnit.Framework;
using Lab08.GameDesign;

namespace Lab08.Tests;

// This file has been split into:
// - MapTests.cs - map functionality tests
// - ItemTests.cs - item placement and weapon usage tests
// - AlienTests.cs - alien behavior tests
// - BandageTests.cs - bandage healing tests
// - PowerSupplyTests.cs - power supply usage tests
// - FacehuggerTeleportTests.cs - facehugger teleportation and map discovery
// - BossFightTests.cs - boss fight activation tests and initialization
// - BossFightManualTest.cs - manual test for boss fight ASCII loading cause IT DIDNT WORK
public class TestInfo
{
    [Test]
    public void Info()
    {
        Assert.Pass("Tests have been organized into specific files by domain area.");
    }
}