using IdentityUtils.Api.Extensions.Cli.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IdentityUtils.Api.Extensions.Cli
{
    internal class ServicesConfigurationLoader
    {
        private const string configFileName = "identityutils-cli.config";

        private static List<ConsoleMessage> FileNotFoundInfoMessage(List<string> configFilePaths)
        {
            List<ConsoleMessage> messages = new List<ConsoleMessage> {
                 new ConsoleMessage(MessageTypes.INFO, "Info: Configuration file not found!")
            };

            string detailsMessage = @$"Current executing directory or directory containing exe file must contain config file: {configFileName}
Following locations where searched:
{string.Join("\r\n", configFilePaths)}";

            messages.Add(new ConsoleMessage(MessageTypes.REGULAR, detailsMessage));
            return messages;
        }

        private static List<ConsoleMessage> ConfigFileMessage(string configFileContent = "")
        {
            string infoMessage = @"
Configuration file must contain following content
HOSTNAME: <IS4 HOSTNAME (https://host)>
CLIENT ID: <Your client ID>
CLIENT SECRET: <Your client secret>
SCOPE: <authentication scope>

Your configuration file content:";

            List<ConsoleMessage> messages = new List<ConsoleMessage> {
                 new ConsoleMessage(MessageTypes.REGULAR, infoMessage)
            };

            if (string.IsNullOrEmpty(configFileContent))
            {
                messages.Add(new ConsoleMessage(MessageTypes.ERROR, "NOT FOUND!"));
            }
            else
            {
                messages.Add(new ConsoleMessage(MessageTypes.REGULAR, configFileContent));
            }

            return messages;
        }

        private static ConsoleResult<ServicesConfiguration> ParseConfigFile(string fileContent)
        {
            var result = new ConsoleResult<ServicesConfiguration>();

            var lines = fileContent.Split(Environment.NewLine).ToList();

            var hasAllData = lines[0].StartsWith("HOSTNAME: ")
                && lines[1].StartsWith("CLIENT ID: ")
                && lines[2].StartsWith("CLIENT SECRET: ")
                && lines[3].StartsWith("SCOPE: ");

            if (!hasAllData)
                result.AddErrorMessage("Error: Authentication parameters not defined");
            else
            {
                result.Data = new ServicesConfiguration
                {
                    Is4Hostname = lines[0].Replace("HOSTNAME: ", "").Trim(),
                    ClientId = lines[1].Replace("CLIENT ID: ", "").Trim(),
                    ClientSecret = lines[2].Replace("CLIENT SECRET: ", "").Trim(),
                    ClientScope = lines[3].Replace("SCOPE: ", "").Trim()
                };
            }

            return result;
        }

        internal static ConsoleResult<ServicesConfiguration> GetServicesConfigurationCommandParams()
        {
            var result = new ConsoleResult<ServicesConfiguration>();

            var consoleArgsUndefined = string.IsNullOrEmpty(IS4Manager.AuthHostname)
                || string.IsNullOrEmpty(IS4Manager.AuthClientId)
                || string.IsNullOrEmpty(IS4Manager.AuthClientSecret)
                || string.IsNullOrEmpty(IS4Manager.AuthScope);

            if (consoleArgsUndefined)
            {
                result.AddInfoMessage("Info: Authorization configuration not set in arguments");
            }
            else
            {
                result.AddInfoMessage("Info: Authorization configuration set in arguments");

                result.Data = new ServicesConfiguration
                {
                    Is4Hostname = IS4Manager.AuthHostname,
                    ClientId = IS4Manager.AuthClientId,
                    ClientSecret = IS4Manager.AuthClientSecret,
                    ClientScope = IS4Manager.AuthScope
                };
            }

            return result;
        }

        internal static ConsoleResult<ServicesConfiguration> GetServicesConfigurationFromFiles()
        {
            var result = new ConsoleResult<ServicesConfiguration>();

            string currentDirectory = Environment.CurrentDirectory;
            string exeLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            //Need to take distinct in case console app is running from exe directory
            List<string> configFilePaths = new List<string>() {
                Path.Combine(currentDirectory, configFileName),
                Path.Combine(exeLocation, configFileName)
            }
            .Distinct()
            .ToList();

            string pathToUse = configFilePaths.Where(x => File.Exists(x)).FirstOrDefault();

            if (string.IsNullOrEmpty(pathToUse))
            {
                result.AddMessages(FileNotFoundInfoMessage(configFilePaths));
                result.AddMessages(ConfigFileMessage());
            }
            else
            {
                string text = File.ReadAllText(pathToUse);
                var configFile = ParseConfigFile(text);

                if (configFile.HasErrors)
                {
                    result.AddMessages(ConfigFileMessage(text));
                }
                else
                {
                    result.AddInfoMessage($"Info: Using configuration file '{pathToUse}'");
                    result.Data = configFile.Data;
                }
            }

            return result;
        }
    }
}