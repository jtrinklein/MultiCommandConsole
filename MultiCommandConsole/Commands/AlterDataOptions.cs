using System;

namespace MultiCommandConsole.Commands
{
    [ArgSet("AlterData", "Options for commands that can alter data")]
    public class AlterDataOptions
    {
        [Arg("noPrompt", "Prevents the command from prompting the user for confirmation.")]
        public bool NoPrompt { get; set; }

        public bool ContinueFor(int count, string objectType)
        {
            return Continue("You are about to alter data for " + count + " " + objectType + ".  Would you like to continue?");
        }

        public bool Continue(string prompt = "You are about to alter data.  Would you like to continue?")
        {
            if (NoPrompt)
            {
                return true;
            }

            Console.WriteLine(prompt + " type 'y' or 'yes' to continue");
            var response = Console.ReadLine();
            return response != null
                   && (
                          response.Equals("y", StringComparison.OrdinalIgnoreCase)
                          || response.Equals("yes", StringComparison.OrdinalIgnoreCase)
                      );
        }
    }
}