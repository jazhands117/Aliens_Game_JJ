namespace Lab08.Displays
{
    public static class MedbayStory
    {
        public static void DisplayMedbayStory()
        {
            Console.Clear();
            DisplayStyle.WriteLine(" ", ConsoleColor.Black);
            DisplayStyle.WriteLine("As you enter the MedBay, the sterile scent of antiseptic fills your nostrils.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine("The flickering fluorescent lights cast eerie shadows on the walls, and the hum of medical equipment creates an unsettling ambiance.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine(" ", ConsoleColor.Black);
            System.Threading.Thread.Sleep(3000);
            DisplayStyle.WriteLine("You cautiously navigate through the room, your footsteps echoing on the cold floor.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine("Bloodstains mar the once-pristine surfaces, and overturned medical carts hint at a frantic attmept to escape.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine(" ", ConsoleColor.Black);
            System.Threading.Thread.Sleep(3000);
            DisplayStyle.WriteLine("You turn around the last medical bench and your eyes widen in shock.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine("There, still half-strapped to a gurney, is a crew member - or what remains of one.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine(" ", ConsoleColor.Black);
            System.Threading.Thread.Sleep(3000);
            DisplayStyle.WriteLine("Their chest looks to be ripped open from the inside. Blood has congealed around the dull yellow ribs.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine("Tiny, alien tracks lead away from the gurney and disappear into a vent in the wall.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine(" ", ConsoleColor.Black);
            System.Threading.Thread.Sleep(3000);
            DisplayStyle.WriteLine("Press ENTER to continue", ConsoleColor.White);
            Console.ReadLine();
            DisplayUI.ClearMessageHistory();
            DisplayStyle.WriteLine("A chill runs down your spine as you realize the horrifying truth - the alien must have emerged here.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine("You've seen enough. You know what happened to the crew.", ConsoleColor.Cyan);
            DisplayStyle.WriteLine(" ", ConsoleColor.Black);
            System.Threading.Thread.Sleep(3000);
            DisplayStyle.WriteLine("You need to leave. Return to the airlock.", ConsoleColor.Cyan);
            System.Threading.Thread.Sleep(1000);
            DisplayStyle.WriteLine("Press ENTER to continue", ConsoleColor.White);
            Console.ReadLine();
            DisplayUI.ClearMessageHistory();
        }
    }
}           