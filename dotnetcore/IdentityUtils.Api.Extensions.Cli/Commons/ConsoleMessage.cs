using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityUtils.Api.Extensions.Cli.Commons
{
    internal enum MessageTypes { ERROR, INFO, REGULAR, SUCCESS };

    internal class ConsoleMessage
    {
        public ConsoleMessage()
        {
        }

        public ConsoleMessage(MessageTypes messageType, string content)
        {
            MessageType = messageType;
            Content = content;
        }

        internal MessageTypes MessageType { get; set; }
        internal string Content { get; set; }
    }
}