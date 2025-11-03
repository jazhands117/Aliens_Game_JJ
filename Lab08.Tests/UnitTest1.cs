using NUnit.Framework;
using Lab08.GameDesign;

namespace Lab08.Tests;

// This file has been split into:
// - MapTests.cs - map functionality tests
// - ItemTests.cs - item placement and weapon usage tests
// - AlienTests.cs - alien behavior tests
// - BandageTests.cs - bandage healing tests
public class TestInfo
{
    [Test]
    public void Info()
    {
        Assert.Pass("Tests have been organized into specific files by domain area.");
    }
}