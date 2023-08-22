using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using wc3_fate_west_data_access_layer.Extensions;
using wc3_fate_west_discord_bot.Services;

namespace wc3_fate_west_discord_bot
{
    public class DiscordBot
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        IConfigurationRoot cfg;

        static void Main(string[] args)
        {
            //=> new DiscordBot().InitDiscordBot().GetAwaiter().GetResult();
        }
        public async Task InitDiscordBot(
            IConfigurationRoot cfg,
            CancellationTokenSource cts, 
            int reconnectCounter = 0, 
            Dictionary<int, Tuple<IUserMessage, wc3_fate_west_data_access_layer.Data.Wc3_Ghost.Lobby.GameStatus>> initEmdedMsgs = null)
        {
            int? reconnectMaxAttempts = Int32.TryParse(cfg.GetSection("DiscordBotSection")["ReconnectMaxiumAttempts"], out var tempR) ? tempR : 10;
            reconnectCounter = (int)reconnectMaxAttempts;
            int? reconnectDelayInSeconds = Int32.TryParse(cfg.GetSection("DiscordBotSection")["ReconnectDelayIsSeconds"], out var tempD) ? tempD : 20;

            DiscordSocketClient client = null;
            this.cfg = cfg;
            logger.Info("[Discord-bot] Init.");

            try
            {
                using (var services = ConfigureServices())
                {
                    client = services.GetRequiredService<DiscordSocketClient>();

                    await client.LoginAsync(TokenType.Bot, cfg.GetSection("DiscordBotSection")["Token"]);
                    await client.StartAsync();

                    //await services.GetRequiredService<CommandService>().AddModulesAsync(Assembly.GetEntryAssembly(), services);

                    // Here we initialize the logic required to register our commands.
                    await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                    await Task.Delay(Timeout.Infinite, cts.Token);
                }
            }
            catch(OperationCanceledException)
            {
                logger.Info("[Discord bot] Client was cancelled.");

                // Handle bot disconnects

                if (client != null)
                {
                    logger.Info("[Discord bot] Client status: " + client.Status + " " + client.ConnectionState + " " + client.LoginState);
                }
                else
                {
                    logger.Info("[Discord bot] Client is null.");
                }

                logger.Info("[Discord bot] Attempting to reconnect. Reconnect max attempts: " + reconnectMaxAttempts + " delay: " + reconnectDelayInSeconds);

                for (int i = 0; i < reconnectMaxAttempts; i++)
                {
                    client.Dispose();
                    cts = new CancellationTokenSource();

                    logger.Info("[Discord bot] Re init discord bot. " + reconnectCounter);

                    await InitDiscordBot(cfg, cts, reconnectCounter - 1, Wc3LobbyService.embedMessages);

                    await Task.Delay((int)reconnectDelayInSeconds);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            
        }
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine("[Discord bot] " + log.ToString());

            return Task.CompletedTask;
        }
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<IConfigurationRoot>(provider => cfg)
                .AddSingleton<DiscordSocketClient>()
                .AddSqliteDatabaseConnectorForGhost()
                //.AddSingleton<CommandService>()
                //.AddSingleton<MessageService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<Wc3LobbyService>()
                .BuildServiceProvider();
        }
    }
}
