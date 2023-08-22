using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_data_access_layer.Db;
using wc3_fate_west_parser_replay_parser.Data;
using wc3_fate_west_parser_replay_parser.Parser;
using wc3_fate_west_parser_replay_parser.Validators;
using wc3_fate_west_data_access_layer.Extensions;
using wc3_fate_west_discord_bot;

namespace wc3_fate_west_main
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private const string CMD_TERMINATE = "/e" + "";
        private const string CMD_MAINTENANCE = "/Maintenance";
        private const string CMD_RELOAD_CONFIG = "/ReloadConfig";
        private const string CMD_RESTART = "/Restart";

        private const string CMD_INIT_PARSER = "/ip";
        private const string CMD_SHUTDOWN_PARSER = "/sp";

        private const string CMD_INIT_DISCORD_BOT = "/id";
        private const string CMD_SHUTDOWN_DISCORD_BOT = "/sd";

        public static IConfigurationRoot cfg;

        private static IConfigurationRoot GetConfig()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            return configuration;
        }
        static void Main(string[] args)
        {
            bool isServerRunning = true;
            bool isRestartCmdPending = false;

            CancellationTokenSource ctsForParser = null;
            CancellationTokenSource ctsForDiscordBot = null;

            IServiceCollection services = new ServiceCollection();
            services.AddSqliteDatabaseConnectorForFateWest();
            //services.AddSqliteDatabaseConnectorForGhost();

            logger.Info("[FW-MAIN] Init.");
            cfg = GetConfig();
            string fateWestReplayDir = cfg.GetSection("FwParserSection")["ReplaySourceDir"];
            int refreshRateInMilliseconds = int.Parse(cfg.GetSection("FwParserSection")["ReplayParserTimerInSeconds"]) * 1000;

            ulong channelId = ulong.Parse(cfg.GetSection("DiscordBotSection")["ChannelId"]);
            string discordToken = cfg.GetSection("DiscordBotSection")["Token"];
            string discordImgPath = cfg.GetSection("DiscordBotSection")["ImagePath"];

            while (isServerRunning)
            {
                Console.Write("> ");
                string cmd = Console.ReadLine();

                switch (cmd)
                {
                    case CMD_TERMINATE:
                        isServerRunning = false;
                        break;
                    case CMD_RELOAD_CONFIG:
                        //LoadConfig();
                        break;
                    case CMD_INIT_PARSER:
                        ctsForParser = new CancellationTokenSource();
                        Task.Run(() => InitFateWestParser(fateWestReplayDir, refreshRateInMilliseconds, services, ctsForParser.Token), ctsForParser.Token);
                        break;
                    case CMD_SHUTDOWN_PARSER:
                        if(ctsForParser != null)
                            ctsForParser.Cancel();
                        else
                            logger.Info("[FW-MAIN] Parser has already been disabled.");
                        break;
                    case CMD_INIT_DISCORD_BOT:
                        ctsForDiscordBot = new CancellationTokenSource();
                        Task.Run(() => InitDiscordBot(discordToken, channelId, discordImgPath, services, ctsForDiscordBot), ctsForDiscordBot.Token);
                        break;
                    case CMD_SHUTDOWN_DISCORD_BOT:
                        if (ctsForDiscordBot != null)
                            ctsForDiscordBot.Cancel();
                        else
                            logger.Info("[FW-MAIN] Discord bot has already been disabled.");
                        break;
                    case CMD_RESTART:
                        isRestartCmdPending = true;
                        isServerRunning = false;
                        break;
                }
            }
        }
        private static async Task InitDiscordBot(string token, ulong channelId, string imgPath, IServiceCollection sc, CancellationTokenSource cts)
        {
            logger.Info("[FW-MAIN] Init discord bot for channel: " + channelId);

            try
            {
                var provider = sc.BuildServiceProvider();
                var receiver = provider.GetService<IGhostDataReceiver>();

                DiscordBot db = new DiscordBot();
                await db.InitDiscordBot(cfg, cts);


            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
        private static async Task InitFateWestParser(string fateWestReplayDir, int refreshRateInMilliseconds, IServiceCollection sc, CancellationToken ct)
        {
            logger.Info("[FW-MAIN] Init fate west parser for next catalog: " + fateWestReplayDir);

            //ServiceManager.InitServiceManager();

            try
            {
                while (true)
                {
                    if (ct.IsCancellationRequested)
                        ct.ThrowIfCancellationRequested();

                    // Parse
                    foreach(FileInfo file in new DirectoryInfo(fateWestReplayDir).GetFiles())
                    {
                        FateReplayParser fateReplayParser = new FateReplayParser(file.FullName);
                        ReplayData fateReplayData = fateReplayParser.ParseReplayData();

                        // Validate replay
                        if (FateGameValidator.IsFateGameValid(fateReplayData))
                        {
                            logger.Info("[FW-MAIN] Game is valid. Saving to DB. " + fateReplayData.GameName + " " + fateReplayData.GameDateTime);

                            var provider = sc.BuildServiceProvider();
                            var writer = provider.GetService<IReplayDataWriter>();
                            writer.WriteReplayToDb(fateReplayData, "test");


                            //new DbWriter().WriteToDb(fateReplayData, "test");

                            //new DbWriter(ServiceManager.sp.GetService<FateWestDbContext>()).WriteToDb(fateReplayData, "test");
                            //WriteDataToDb("test", fateReplayData);
                            // new DbWriter(DbController.GetDbContext()).WriteToDb(fateReplayData, "test");

                        }
                        else
                        {
                            logger.Info("[FW-MAIN] Game is not valid.");
                        }
                    }

                    await Task.Delay(refreshRateInMilliseconds);
                }
            }
            catch (OperationCanceledException e)
            {
                logger.Info("[FW-MAIN] Cancelling parsing thread ...", e);
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}