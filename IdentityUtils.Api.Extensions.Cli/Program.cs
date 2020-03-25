using IdentityUtils.Api.Extensions.Cli.Commands;
using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;

namespace IdentityUtils.Api.Extensions.Cli
{
    /// <summary>
    /// In this example, each command a nested class type.
    /// </summary>
    [Command(Name = "is4manager", Description = "Identity server 4 managment CLI"),
     Subcommand(typeof(Tenants), typeof(Users), typeof(Roles))]
    internal class IS4Manager
    {
        [Option(ShortName = "ahost", LongName = "Auth.Hostname", Description = "Authentication - Hostname")]
        public static string AuthHostname { get; set; }

        [Option(ShortName = "aid", LongName = "Auth.ClientId", Description = "Authentication - Client Id")]
        public static string AuthClientId { get; set; }

        [Option(ShortName = "asecret", LongName = "Auth.ClientSecret", Description = "Authentication - Client secret")]
        public static string AuthClientSecret { get; set; }

        [Option(ShortName = "ascope", LongName = "Auth.Scope", Description = "Authentication - Scope")]
        public static string AuthScope { get; set; }

        public static void Main(string[] args)
        {
            CommandLineApplication.Execute<IS4Manager>(args);
        }

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.WriteLine("You must specify at a subcommand.");
            app.ShowHelp();
            return 1;
        }

        /// <summary>
        /// <see cref="HelpOptionAttribute"/> must be declared on each type that supports '--help'.
        /// Compare to the inheritance example, in which <see cref="GitCommandBase"/> delcares it
        /// once so that all subcommand types automatically support '--help'.
        /// </summary>

        [Command("image", Description = "Manage images"),
         Subcommand(typeof(List))]
        private class Images
        {
            private int OnExecute(IConsole console)
            {
                console.Error.WriteLine("You must specify an action. See --help for more details.");
                return 1;
            }

            [Command("ls", Description = "List images",
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
            private class List
            {
                [Option(Description = "Show all containers (default shows just running)")]
                public bool All { get; }

                private IReadOnlyList<string> RemainingArguments { get; }

                private void OnExecute(IConsole console)
                {
                    console.WriteLine(string.Join("\n",
                        "IMAGES",
                        "--------------------",
                        "microsoft/dotnet:2.0"));
                }
            }
        }
    }
}