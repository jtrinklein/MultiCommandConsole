using System;

namespace MultiCommandConsole.Commands
{
    /// <summary>
    /// Options for commands that alter data.  See AlterDataOptionWithDryRun for a dry run option.
    /// </summary>
    [ArgSet("alter-data", "Options for commands that can alter data")]
    public class AlterDataOptions
    {
        [Arg("noPrompt", "Prevents the command from prompting the user for confirmation.")]
        public bool NoPrompt { get; set; }

        public UserInteractiveOptions UserInteractiveOptions { get; set; }

        /// <summary>
        /// Prompts the user to alter data, including the count of objects to be modified
        /// </summary>
        /// <returns>true if the user responds affirmative </returns>
        public bool ContinueFor(int count, string objectType)
        {
            return Continue("You are about to alter data for " + count + " " + objectType + ".  Would you like to continue?");
        }

        /// <summary>
        /// Prompts the user to alter data
        /// </summary>
        /// <returns>true if the user responds affirmative </returns>
        public bool Continue(string prompt = "You are about to alter data.  Would you like to continue?")
        {
            if (NoPrompt)
            {
                return true;
            }

            var response = UserInteractiveOptions.Prompt(prompt + " type 'y' or 'yes' to continue");
            return response != null
                   && (
                          response.Equals("y", StringComparison.OrdinalIgnoreCase)
                          || response.Equals("yes", StringComparison.OrdinalIgnoreCase)
                      );
        }
    }
}