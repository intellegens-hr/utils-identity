using IdentityUtils.Core.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace IdentityUtils.Api.Extensions.Cli.Commons
{
    internal static class ConsoleResultExtensions
    {
        internal static List<ConsoleMessage> GetConsoleErrorMessages(this IdentityManagementResult result)
            => result.ErrorMessages.Select(x => new ConsoleMessage(MessageTypes.ERROR, x)).ToList();

        internal static ConsoleResult ToConsoleResult(this IdentityManagementResult result)
            => new ConsoleResult
            {
                Messages = result.GetConsoleErrorMessages()
            };

        internal static ConsoleResult ToConsoleResultWithDefaultMessages(this IdentityManagementResult result)
        {
            var consoleResult = new ConsoleResult();

            if (result.Success)
            {
                consoleResult.AddMessage("SUCCESS", MessageTypes.SUCCESS);
            }
            else
            {
                consoleResult.AddErrorMessage("ERROR");
                consoleResult.AddMessages(result.GetConsoleErrorMessages());
            }

            return consoleResult;
        }
    }
}