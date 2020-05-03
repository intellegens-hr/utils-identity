using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityUtils.Api.Extensions.Cli.Commons
{
    internal class ConsoleResult
    {
        internal List<ConsoleMessage> Messages { get; set; } = new List<ConsoleMessage>();
        internal bool HasErrors => Messages.Any(x => x.MessageType == MessageTypes.ERROR);

        internal void AddMessages(IEnumerable<ConsoleMessage> messages)
        {
            Messages.AddRange(messages);
        }

        internal void AddErrorMessage(string message)
        {
            Messages.Add(new ConsoleMessage(MessageTypes.ERROR, message));
        }

        internal void AddInfoMessage(string message)
        {
            Messages.Add(new ConsoleMessage(MessageTypes.INFO, message));
        }

        internal void AddMessage(string message, MessageTypes messageType = MessageTypes.REGULAR)
        {
            Messages.Add(new ConsoleMessage(messageType, message));
        }

        internal void WriteMessages(IConsole console)
        {
            Dictionary<MessageTypes, ConsoleColor> messageTypeColorMapping = new Dictionary<MessageTypes, ConsoleColor>
            {
                { MessageTypes.ERROR, ConsoleColor.Red },
                { MessageTypes.INFO, ConsoleColor.Yellow },
                { MessageTypes.REGULAR, ConsoleColor.White },
                { MessageTypes.SUCCESS, ConsoleColor.Green }
            };

            foreach (var message in Messages)
            {
                console.ForegroundColor = messageTypeColorMapping[message.MessageType];
                console.WriteLine(message.Content);
            }

            console.ForegroundColor = ConsoleColor.White;
        }
    }

    internal class ConsoleResult<T>: ConsoleResult
    {
        internal T Data { get; set; }
        
    }
}