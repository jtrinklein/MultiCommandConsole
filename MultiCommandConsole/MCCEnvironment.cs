using System;

namespace MultiCommandConsole {
    public static class MCCEnvironment
    {
        public static string UserInteractiveEnvVarName = "USER_INTERACTIVE";
        public static bool UserInteractive
        {
            get
            {
                var interactiveEnvVar = Environment.GetEnvironmentVariable(UserInteractiveEnvVarName, EnvironmentVariableTarget.Process);
                var envVarSet = !string.IsNullOrEmpty(interactiveEnvVar);
                return Environment.UserInteractive || envVarSet;
            }
        }

        public static string NewLine
        {
            get
            {
                return Environment.NewLine;
            }
        }

        public static string GetFolderPath(Environment.SpecialFolder folder)
        {
            return Environment.GetFolderPath(folder);
        }
    }
}
