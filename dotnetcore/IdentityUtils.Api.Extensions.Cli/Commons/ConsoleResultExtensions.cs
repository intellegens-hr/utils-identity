using IdentityUtils.Core.Contracts.Commons;
using System.Collections.Generic;
using System.Linq;

namespace IdentityUtils.Api.Extensions.Cli.Commons
{
    internal static class ConsoleResultExtensions
    {
        internal static IEnumerable<ConsoleMessage> GetConsoleErrorMessages(this IdentityUtilsResult result)
            => result.ErrorMessages.Select(x => new ConsoleMessage(MessageTypes.ERROR, x));

        internal static ConsoleResult ToConsoleResult(this IdentityUtilsResult result)
            => new ConsoleResult
            {
                Messages = result.GetConsoleErrorMessages().ToList()
            };

        internal static ConsoleResult ToConsoleResultWithDefaultMessages(this IdentityUtilsResult result)
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