using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace wc3_fate_west_discord_bot.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly IConfigurationRoot cfg;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public CommandHandlingService(IServiceProvider services, IConfigurationRoot cfg)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();

            _services = services;

            this.cfg = cfg;

            // Hook CommandExecuted to handle post-command-execution logic.
            _commands.CommandExecuted += CommandExecutedAsync;
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            _discord.MessageReceived += MessageReceivedAsync;
            //_discord.Ready += Client_Ready;
        }
        public async Task InitializeAsync()
        {
            logger.Info("[Discord bot] Init CommandHandlingService.");

            // Register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);

            logger.Info("[Discord bot] Assembly: " + Assembly.GetExecutingAssembly());
            //logger.Info("[Discord bot] Commands: " + _commands.);
        }
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            // This value holds the offset where the prefix ends
            var argPos = 0;

            // Perform prefix check.
            string commandTrigger = cfg.GetSection("DiscordBotSection")["commandTrigger"];
            char prefix = Char.Parse(string.IsNullOrEmpty(commandTrigger) ? "!" : commandTrigger);

            if (!message.HasCharPrefix(prefix, ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);

            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
            {
                logger.Info("[Discord bot] Command failed to execute.");

                logger.Info("[Discord bot] " + command.GetValueOrDefault().Summary);
                logger.Info("[Discord bot] " + command.Value.Name);

                return;
            }
                
            if (result.IsSuccess)
            {
                System.Console.WriteLine($"Command [] executed for -> []");
                return;
            }
                
            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}