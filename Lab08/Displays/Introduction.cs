namespace Lab08.Displays
{
    public static class Introduction
    {
        public static void DisplayIntro()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("=========================================");
            Console.WriteLine("                A L I E N S              ");
            Console.WriteLine("=========================================");
            Console.WriteLine();
            // have a slight delay time here
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();

            Console.Clear();
            Console.WriteLine("SOS RECEIVED FROM DEEP SPACE MINING VESSEL 'THE COVENANT'");
            Console.WriteLine();
            Console.WriteLine("\"THIS IS THE COVENANT. WE ARE UNDER A-------- SEND H------ SIGNAL NOT CLEAR ---- SEND HELP ----\"");
            Console.WriteLine();
            System.Threading.Thread.Sleep(3000);
            Console.ResetColor();
            Console.WriteLine("The last transmission from the Covenant was garbled and incomplete.");
            Console.WriteLine("You are a simple radio transmitter technician, sent to investigate the disturbance.");
            Console.WriteLine();
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine("As you dock with the Covenant, you notice that all lights on the ship are off.");
            Console.WriteLine("The airlock is functioning, so you cautiously make your way inside.");
            Console.WriteLine();
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine("The ship's interior is dark and silent. Your flashlight barely penetrates the gloom.");
            Console.WriteLine("However, your motion detector indicates movement in the ship's cavernous interior.");
            Console.WriteLine();
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine("Your mission is to locate the crew and determine the cause of the strange signal.");
            Console.WriteLine();
            System.Threading.Thread.Sleep(2000);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-----------------------------");
            Console.WriteLine("----- GAME INSTRUCTIONS -----");
            Console.WriteLine("-----------------------------");
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.WriteLine("  'w'  - Move one room forward");
            Console.WriteLine("  's'  - Move one room backward");
            Console.WriteLine("  'd'  - Move one room to the right");
            Console.WriteLine("  'a'  - Move one room to the left");
            Console.WriteLine("  'r'  - Roll a bullet to check a nearby room");
            Console.WriteLine("  'i'  - Open Inventory");
            Console.WriteLine("  'h'  - Show commands message");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press ENTER to begin...");
            Console.ResetColor();
            Console.ReadLine();
        }
    }
}