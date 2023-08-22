using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_data_access_layer.Data.Wc3_Ghost;
using wc3_fate_west_discord_bot.Extensions;
using static wc3_fate_west_data_access_layer.Data.Wc3_Ghost.Lobby;

namespace wc3_fate_west_discord_bot.Services
{
    public class Wc3LobbyService
    {
        const int maxiumMessages = 100;
        public static Dictionary<int, Tuple<IUserMessage, GameStatus>> embedMessages;

        private readonly IServiceProvider services;
        private readonly IConfigurationRoot cfg;
        private readonly DiscordSocketClient discord;
        CancellationTokenSource tokenSourceForDiscordBotWc3LobbyService;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        string imagePath;

        public Wc3LobbyService(IServiceProvider services, DiscordSocketClient discord, IConfigurationRoot cfg)
        {
            this.services = services;
            this.cfg = cfg;
            this.discord = services.GetRequiredService<DiscordSocketClient>();

            this.discord.Ready += Client_Ready;
            this.discord.Disconnected += Discord_Disconnected;
            
        }
        public void InitService(CancellationTokenSource ctsForDiscordBotWc3LobbyService, Dictionary<int, Tuple<IUserMessage, GameStatus>> initEmbedMessages = null)
        {
            if (initEmbedMessages != null)
            {
                logger.Info("[Discord bot] Using existing embed msg's collection.");
                embedMessages = new Dictionary<int, Tuple<IUserMessage, GameStatus>>();

                foreach (var key in initEmbedMessages.Keys)
                {
                    embedMessages.Add(key, initEmbedMessages[key]);
                }
            }
            else
            {
                logger.Info("[Discord bot] Creating new embed msg's collection.");
                embedMessages = new Dictionary<int, Tuple<IUserMessage, GameStatus>>();
            }

            tokenSourceForDiscordBotWc3LobbyService = ctsForDiscordBotWc3LobbyService;
        }
        private async Task<Task> Discord_Disconnected(Exception args)
        {
            logger.Info("[Discord bot] Disconnected. ");

            if (tokenSourceForDiscordBotWc3LobbyService != null)
                tokenSourceForDiscordBotWc3LobbyService.Cancel();

            return Task.CompletedTask;
        }

        private Task Client_Ready()
        {
            logger.Info("[Discord bot] Discord client is ready.");
            logger.Info("[Discord bot] Client status: " + discord.Status + " " + discord.ConnectionState + " " + discord.LoginState);

            CancellationToken ct = tokenSourceForDiscordBotWc3LobbyService.Token;

            imagePath = cfg.GetSection("DiscordBotSection")["ImagePath"];
            ulong channelId = ulong.Parse(cfg.GetSection("DiscordBotSection")["ChannelId"]);
            int discordRequestDelay = int.Parse(cfg.GetSection("DiscordBotSection")["DiscordRequestDelayInSeconds"]) * 1000;

            Task.Run(() => StartLobbyStatusUpdater(ct, channelId, discordRequestDelay), ct);
            //await StartLobbyStatusUpdater(ct, channelId, discordRequestDelay).wa

            //EmbedAnn().Wait();

            return Task.CompletedTask;
        }
        public async Task StartLobbyStatusUpdater(CancellationToken ct, ulong channelId, int updateDelayInMilliseconds)
        {
            logger.Info("[Discord bot] Starting lobby updater with next parameters: " + imagePath + " " + channelId + " " + updateDelayInMilliseconds);

            try
            {
                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        // Clean up here, then...
                        ct.ThrowIfCancellationRequested();
                    }

                    foreach (var lobby in services.GetService<IGhostDataReceiver>().GetLobbiesFromDb())
                    {
                        
                        if(lobby.LobbySlots != null && lobby.LobbySlots.Count == 0)
                        {
                            services.GetService<IGhostDataReceiver>().ChangeGameStatusForLobby(lobby.LobbyId, GameStatus.Processed, false);
                        }
                        
                        // Generate embed
                        logger.Info(lobby.LobbyId);

                        Embed embed = CreateEmbedAnnounceForWc3LobbyV3(lobby, channelId, imagePath);
                        await ProcessEmbedMessage(lobby, embed, imagePath, channelId);
                    }

                    await Task.Delay(updateDelayInMilliseconds);
                }
            }
            catch (OperationCanceledException)
            {
                logger.Info("[Discord bot] Cancelling lobby updater ...");
                await discord.LogoutAsync();
            }
            catch (Exception e)
            {
                logger.Error(e, "[Discord bot] Lobby updater error.");
                throw;
            }
            finally
            {
                if (tokenSourceForDiscordBotWc3LobbyService != null)
                    tokenSourceForDiscordBotWc3LobbyService.Cancel();

                await discord.LogoutAsync();

                if (discord != null)
                    discord.Dispose();
            }
        }
        public async Task ProcessEmbedMessage(Lobby lobby, Embed embed, string imagePath, ulong channelId)
        {
            // Skip if private
            if (lobby.GetLobbyType == LobbyType.Private)
                return;

            // Check if msg alrdy exist.
            if (embedMessages.ContainsKey(lobby.LobbyId))
            {
                logger.Info("[Discord bot] Edit msg with id: " + lobby.LobbyId);

                // Edit
                await embedMessages[lobby.LobbyId].Item1.ModifyAsync(msg => msg.Embed = embed);
                embedMessages[lobby.LobbyId] = new Tuple<IUserMessage, GameStatus>(embedMessages[lobby.LobbyId].Item1, lobby.GetGameStatus);

                if (lobby.GetGameStatus == GameStatus.Finished)
                {
                    // TODO - Mark as processed.
                    logger.Info("[Discord bot] Game was finished: " + lobby.LobbyId);

                    embedMessages.Remove(lobby.LobbyId);
                    services.GetService<IGhostDataReceiver>().ChangeGameStatusForLobby(lobby.LobbyId, GameStatus.Processed, true);
                }
            }
            else
            {
                using var stream = File.OpenRead(imagePath);
                var filename = Path.GetFileName(imagePath);
                var chnl = discord.GetChannel(channelId) as IMessageChannel;

                // Send new
                if (embedMessages.Count > maxiumMessages)
                {
                    logger.Info("[Discord bot] Maxium msg was reached, deleting old msg's.");
                    int keyToRemove = embedMessages.Keys.Last();

                    logger.Info("[Discord bot] Removing msg with key id: " + keyToRemove);
                    embedMessages.Remove(embedMessages.Keys.FirstOrDefault());

                    logger.Info("[Discord bot] Sending new msg with id: " + lobby.LobbyId);
                    embedMessages.Add(lobby.LobbyId, new Tuple<IUserMessage, GameStatus>(await chnl.SendFileAsync(stream, filename, embed: embed), lobby.GetGameStatus));
                }
                else
                {
                    logger.Info("[Discord bot] Sending new msg with id: " + lobby.LobbyId);
                    embedMessages.Add(lobby.LobbyId, new Tuple<IUserMessage, GameStatus>(await chnl.SendFileAsync(stream, filename, embed: embed), lobby.GetGameStatus));
                }
            }
        }
        // TODO - Dynamic field processing according real lobby team and slot counters
        // TODO - Restrictions to data length
        // TODO - Extra stats from fate w
        /*
        private Embed CreateEmbedAnnounceForWc3Lobby(Lobby lobby, ulong channelId, string imagePath)
        {
            string playerNameSeparator = "=================";
            string pingSeparator = "======";
            int pingSeparatorLength = pingSeparator.Length;
            string statsSeparator = "=================";
            int statsSeparatorLength = statsSeparator.Length;

            int descriptionPadding =
                playerNameSeparator.Length
                + pingSeparatorLength
                + statsSeparatorLength
                - lobby.LobbyName.Length
                - lobby.LobbyCurrentPlayers.Length + 8;
            int totalGamesPadding = 4;
            int avgStayPadding = 3;

            var builder = new EmbedBuilder()
                    .WithTitle(lobby.Realm)
                    .WithDescription(GetDescription("```\n" + lobby.LobbyName, lobby.LobbyCurrentPlayers + "```", lobby.GetGameStatus, descriptionPadding))
                    .WithUrl(string.IsNullOrWhiteSpace(lobby.Realm) ? "https://discord.com" : "https://www.google.com/search?q=" + lobby.Realm)
                    .WithColor(GetStatusColor(lobby.GetGameStatus))
                    .WithThumbnailUrl($"attachment://{Path.GetFileName(imagePath)}")
                    .WithTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(1612520716230))
                    .WithFooter(footer =>
                    {
                        footer
                        .WithText(GetFooterString(lobby.GetGameStatus, lobby.LobbyDateTime, lobby.LobbyId))
                        //.WithIconUrl("https://cdn.discordapp.com/embed/avatars/0.png");
                        .WithIconUrl($"attachment://{Path.GetFileName(imagePath)}");
                    })
                    .AddField("PlayerName", "```" +
                    // Team 1
                    "\n" + GetPlayerName(lobby.LobbySlots[0].PlayerName, playerNameSeparator) +
                    "\n" + GetPlayerName(lobby.LobbySlots[1].PlayerName, playerNameSeparator) +
                    "\n" + GetPlayerName(lobby.LobbySlots[2].PlayerName, playerNameSeparator) +
                    "\n" + GetPlayerName(lobby.LobbySlots[3].PlayerName, playerNameSeparator) +
                    "\n" + GetPlayerName(lobby.LobbySlots[4].PlayerName, playerNameSeparator) +
                    "\n" + GetPlayerName(lobby.LobbySlots[5].PlayerName, playerNameSeparator) +
                    // Team separator
                    "\n" + playerNameSeparator +
                    // Team separator
                    // Team 2
                    "\n" + GetPlayerName(lobby.LobbySlots[6].PlayerName, playerNameSeparator) +
                    "\n" + GetPlayerName(lobby.LobbySlots[7].PlayerName, playerNameSeparator) +
                    "\n" + GetPlayerName(lobby.LobbySlots[8].PlayerName, playerNameSeparator) +
                    "\n" + GetPlayerName(lobby.LobbySlots[9].PlayerName, playerNameSeparator) +
                    "\n" + GetPlayerName(lobby.LobbySlots[10].PlayerName, playerNameSeparator) +
                    "\n" + GetPlayerName(lobby.LobbySlots[11].PlayerName, playerNameSeparator) +
                    "```", true)
                    .AddField("Ping", "```" +
                    // Team 1
                    "\n" + GetPlayerPing(lobby.LobbySlots[0].PlayerPing, pingSeparatorLength) +
                    "\n" + GetPlayerPing(lobby.LobbySlots[1].PlayerPing, pingSeparatorLength) +
                    "\n" + GetPlayerPing(lobby.LobbySlots[2].PlayerPing, pingSeparatorLength) +
                    "\n" + GetPlayerPing(lobby.LobbySlots[3].PlayerPing, pingSeparatorLength) +
                    "\n" + GetPlayerPing(lobby.LobbySlots[4].PlayerPing, pingSeparatorLength) +
                    "\n" + GetPlayerPing(lobby.LobbySlots[5].PlayerPing, pingSeparatorLength) +
                    // Team separator
                    "\n" + pingSeparator +
                    // Team separator
                    // Team 2
                    "\n" + GetPlayerPing(lobby.LobbySlots[6].PlayerPing, pingSeparatorLength) +
                    "\n" + GetPlayerPing(lobby.LobbySlots[7].PlayerPing, pingSeparatorLength) +
                    "\n" + GetPlayerPing(lobby.LobbySlots[8].PlayerPing, pingSeparatorLength) +
                    "\n" + GetPlayerPing(lobby.LobbySlots[9].PlayerPing, pingSeparatorLength) +
                    "\n" + GetPlayerPing(lobby.LobbySlots[10].PlayerPing, pingSeparatorLength) +
                    "\n" + GetPlayerPing(lobby.LobbySlots[11].PlayerPing, pingSeparatorLength) +
                    "```", true)
                    .AddField("Games - Avg s   - Avg l", "```" +
                    // Team 1
                    "\n" + lobby.LobbySlots[0].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[0].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[0].PlayerStats.AvgLoadingTime +
                    "\n" + lobby.LobbySlots[1].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[1].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[1].PlayerStats.AvgLoadingTime +
                    "\n" + lobby.LobbySlots[2].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[2].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[2].PlayerStats.AvgLoadingTime +
                    "\n" + lobby.LobbySlots[3].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[3].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[3].PlayerStats.AvgLoadingTime +
                    "\n" + lobby.LobbySlots[4].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[4].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[4].PlayerStats.AvgLoadingTime +
                    "\n" + lobby.LobbySlots[5].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[5].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[5].PlayerStats.AvgLoadingTime +
                    // Team separator
                    "\n" + statsSeparator +
                    // Team separator
                    // Team 2
                    "\n" + lobby.LobbySlots[6].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[6].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[6].PlayerStats.AvgLoadingTime +
                    "\n" + lobby.LobbySlots[7].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[7].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[7].PlayerStats.AvgLoadingTime +
                    "\n" + lobby.LobbySlots[8].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[8].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[8].PlayerStats.AvgLoadingTime +
                    "\n" + lobby.LobbySlots[9].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[9].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[9].PlayerStats.AvgLoadingTime +
                    "\n" + lobby.LobbySlots[10].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[10].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[10].PlayerStats.AvgLoadingTime +
                    "\n" + lobby.LobbySlots[11].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[11].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[11].PlayerStats.AvgLoadingTime +
                    "```", true);

            var embed = builder.Build();

            return embed;
        }
        private Embed CreateEmbedAnnounceForWc3LobbyV2(Lobby lobby, ulong channelId, string imagePath)
        {
            string teamSeparator = "===============================================";
            string playerNameSeparator = "=================";
            string pingSeparator = "======";
            int pingSeparatorLength = pingSeparator.Length;
            string statsSeparator = "=================";
            int statsSeparatorLength = statsSeparator.Length;

            int descriptionPadding =
                playerNameSeparator.Length
                + pingSeparatorLength
                + statsSeparatorLength
                - lobby.LobbyName.Length
                - lobby.LobbyCurrentPlayers.Length + 8;
            int totalGamesPadding = 4;
            int avgStayPadding = 3;

            var builder = new EmbedBuilder()
                    .WithTitle(lobby.Realm)
                    .WithDescription(GetDescription("```\n" + lobby.LobbyName, lobby.LobbyCurrentPlayers + "```", lobby.GetGameStatus, descriptionPadding))
                    .WithUrl(string.IsNullOrWhiteSpace(lobby.Realm) ? "https://discord.com" : "https://www.google.com/search?q=" + lobby.Realm)
                    .WithColor(GetStatusColor(lobby.GetGameStatus))
                    .WithThumbnailUrl($"attachment://{Path.GetFileName(imagePath)}")
                    .WithTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(1612520716230))
                    .WithFooter(footer =>
                    {
                        footer
                        .WithText(GetFooterString(lobby.GetGameStatus, lobby.LobbyDateTime, lobby.LobbyId))
                        //.WithIconUrl("https://cdn.discordapp.com/embed/avatars/0.png");
                        .WithIconUrl($"attachment://{Path.GetFileName(imagePath)}");
                    })
                    .AddField("PlayerName", "```" +
                    // Team 1
                    "\n" + GetPlayerName(lobby.LobbySlots[0].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[0].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[0].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[0].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[0].PlayerStats.AvgLoadingTime +
                    "\n" + GetPlayerName(lobby.LobbySlots[1].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[1].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[1].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[1].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[1].PlayerStats.AvgLoadingTime +
                    "\n" + GetPlayerName(lobby.LobbySlots[2].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[2].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[2].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[2].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[2].PlayerStats.AvgLoadingTime +
                    "\n" + GetPlayerName(lobby.LobbySlots[3].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[3].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[3].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[3].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[3].PlayerStats.AvgLoadingTime +
                    "\n" + GetPlayerName(lobby.LobbySlots[4].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[4].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[4].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[4].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[4].PlayerStats.AvgLoadingTime +
                    "\n" + GetPlayerName(lobby.LobbySlots[5].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[5].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[5].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[5].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[5].PlayerStats.AvgLoadingTime +
                    // Team separator
                    "\n" + teamSeparator +
                    // Team separator
                    // Team 2
                    "\n" + GetPlayerName(lobby.LobbySlots[6].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[6].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[6].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[6].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[6].PlayerStats.AvgLoadingTime +
                    "\n" + GetPlayerName(lobby.LobbySlots[7].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[7].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[7].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[7].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[7].PlayerStats.AvgLoadingTime +
                    "\n" + GetPlayerName(lobby.LobbySlots[8].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[8].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[8].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[8].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[8].PlayerStats.AvgLoadingTime +
                    "\n" + GetPlayerName(lobby.LobbySlots[9].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[9].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[9].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[9].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[9].PlayerStats.AvgLoadingTime +
                    "\n" + GetPlayerName(lobby.LobbySlots[10].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[10].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[10].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[10].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[10].PlayerStats.AvgLoadingTime +
                    "\n" + GetPlayerName(lobby.LobbySlots[11].PlayerName, playerNameSeparator) + GetPlayerPing(lobby.LobbySlots[11].PlayerPing, pingSeparatorLength)
                    + lobby.LobbySlots[11].PlayerStats.TotalGames.PadRight(totalGamesPadding) +
                    " | " + lobby.LobbySlots[11].PlayerStats.AvgStayPercent.PadRight(avgStayPadding) +
                    " | " + lobby.LobbySlots[11].PlayerStats.AvgLoadingTime +
                    "```", true);
                    

            var embed = builder.Build();

            return embed;
        }
        */
        private Embed CreateEmbedAnnounceForWc3LobbyV3(Lobby lobby, ulong channelId, string imagePath)
        {
            string teamSeparator = "===============================================";
            int playerNameLengthRestriction = 18;
            int descriptionPadding = teamSeparator.Length - lobby.LobbyName.Length - lobby.LobbyCurrentPlayers.Length + 8;
            int currentPlayerLenghtRestriction = 18;
            int gameNameRestriction = 27;
            int pingLengthRestriction = 3;
            int totalGamesRestriction = 6;
            int avgLoadRestriction = 5;

            var builder = new EmbedBuilder()
                    .WithTitle(lobby.Realm)
                    .WithUrl(string.IsNullOrWhiteSpace(lobby.Realm) ? "https://discord.com" : "https://www.google.com/search?q=" + lobby.Realm)
                    .WithColor(GetStatusColor(lobby.GetGameStatus))
                    .WithThumbnailUrl($"attachment://{Path.GetFileName(imagePath)}")
                    .WithFooter(footer =>
                    {
                        footer
                        .WithText(GetFooterString(lobby.GetGameStatus, lobby.LobbyDateTime, lobby.LobbyId))
                        .WithIconUrl($"attachment://{Path.GetFileName(imagePath)}");
                    })
                    .AddField("Game name", GetGameNameString(lobby.LobbyName, gameNameRestriction, lobby.GetGameStatus), true)
                    .AddField("Current players", GetCurrentPlayersString(lobby.LobbyCurrentPlayers, currentPlayerLenghtRestriction).CenteredString(22), true)
                    .AddField(
                "Player name".PadRight(42) + 
                "Ping".PadRight(14) + 
                "TGames".PadRight(12) +
                "AStay".PadRight(8) +
                "ALoad".PadRight(14), GetStringForField(lobby, teamSeparator, playerNameLengthRestriction, pingLengthRestriction, totalGamesRestriction, avgLoadRestriction));

            var embed = builder.Build();

            return embed;
        }
        private string GetStringForField(Lobby lobby, string teamSeparator, int playerNameLengthRestriction, int pingLengthRestriction, int totalGamesRestriction, int avgLoadRestriction)
        {
            int playerTeam = 0;
            StringBuilder sb = new StringBuilder();

            sb.Append("```");

            for(int i = 0; i < lobby.LobbySlots.Count; i++)
            {
                if (lobby.LobbySlots[i].PlayerTeam != playerTeam)
                {
                    sb.Append("\n" + teamSeparator);
                    playerTeam = lobby.LobbySlots[i].PlayerTeam;
                }

                string field =
                "\n" 
                + GetPlayerName(lobby.LobbySlots[i].PlayerName, playerNameLengthRestriction)
                
                .PadRight(playerNameLengthRestriction) + " | "

                + GetPlayerPing(lobby.LobbySlots[i].PlayerPing, pingLengthRestriction, lobby.LobbySlots[i].IsHuman)
                .CenteredString(pingLengthRestriction)
                .PadRight(pingLengthRestriction + 1) + " | "

                + GetTotalGamesString(lobby.LobbySlots[i].PlayerStats.TotalGames, totalGamesRestriction, lobby.LobbySlots[i].IsHuman)
                .CenteredString(5)
                .PadRight(5) + " | "

                + GetAvgStay(lobby.LobbySlots[i].PlayerStats.AvgStayPercent, lobby.LobbySlots[i].IsHuman)
                .CenteredString(3)
                .PadRight(3) + " | " 

                + GetAvgLoadString(lobby.LobbySlots[i].PlayerStats.AvgLoadingTime, avgLoadRestriction, lobby.LobbySlots[i].IsHuman)
                .CenteredString(avgLoadRestriction);

                sb.Append(field);
            }

            sb.Append("```");

            return sb.ToString();
        }
        
        private string GetAvgStay(string rawStr, bool isHuman)
        {
            return isHuman ? rawStr : "-";
        }
        private string GetAvgLoadString(string rawStr, int avgLoadRestriction, bool isHuman)
        {
            return isHuman ? rawStr.Length > avgLoadRestriction ? "999+" : rawStr : "-";
        }
        private string GetTotalGamesString(string rawStr, int totalGamesRestriction, bool isHuman)
        {
            return isHuman ? rawStr.Length > totalGamesRestriction ? rawStr.Substring(0, totalGamesRestriction) : rawStr : "-";
        }
        private string GetGameNameString(string rawStr, int gameNameLengthRestriction, GameStatus gs)
        {
            string res = "```" + (rawStr.Length > gameNameLengthRestriction ? rawStr.Substring(0, gameNameLengthRestriction) : rawStr) + "```";

            if (gs == GameStatus.Finished || gs == GameStatus.InProgress)
            {
                return "~~" + res + "~~";
            }
            else
            {
                return res;
            }
        }
        private string GetCurrentPlayersString(string rawStr, int currentPlayerLengthRestriction)
        {
            return "```" + String.Format("{0," + ((currentPlayerLengthRestriction / 2) + (rawStr.Length / 2)) + "}", rawStr) + "```";
        }
        private string GetPlayerName(string rawName, int playerNameLengthRestriction)
        {
            return rawName.Length > playerNameLengthRestriction ? rawName.Substring(0, playerNameLengthRestriction) : rawName;
        }
        private string GetPlayerPing(string rawStr, int pingLengthRestriction, bool isHuman)
        {
            return isHuman ? rawStr.Length > pingLengthRestriction ? "999+" : rawStr : "-";
        }
        private string GetFooterString(GameStatus gs, DateTime dt, int lobbyId)
        {
            string resultStr = string.Empty;

            if (embedMessages.ContainsKey(lobbyId))
            {
                if (embedMessages[lobbyId].Item2 != gs)
                {
                    // Change status
                    logger.Info("[Discord bot] New status for " + lobbyId + ".");
                    resultStr = GetDateTimeStringForFooter(gs, dt);
                }
                else
                {
                    // Return existing status string
                    logger.Info("[Discord bot] Using existing status for " + lobbyId + ".");
                    resultStr = embedMessages[lobbyId].Item1.Embeds.FirstOrDefault().Footer.Value.Text;

                    int index = resultStr.IndexOf(" - Updated");
                    resultStr = resultStr.Substring(0, index);
                }
            }
            else
            {
                // New message
                logger.Info("[Discord bot] New status for new message " + lobbyId + ".");
                resultStr = GetDateTimeStringForFooter(gs, dt);
            }

            return resultStr + " - Updated on " + DateTime.Now.ToString("dd'-'MM'-'yyyy 'at' HH:mm:ss");
            /*
            // TODO - Wtf?
            if(resultStr.Contains("Updated"))
            {
                return resultStr.Substring(resultStr.IndexOf("Updated")) + " - Updated at: " + DateTime.Now.ToString("dd'-'MM'-'yyyy HH:mm:ss");
            }
            else
            {
                return resultStr + " - Updated at: " + DateTime.Now.ToString("dd'-'MM'-'yyyy HH:mm:ss");
            }
            */
        }
        private string GetDateTimeStringForFooter(GameStatus gs, DateTime dt)
        {
            string dateTime = dt.ToString("dd'-'MM'-'yyyy 'at' HH:mm:ss");

            switch (gs)
            {
                case GameStatus.Open:
                    return "Opened on " + dateTime;
                case GameStatus.InProgress:
                    return "Started on " + dateTime;
                case GameStatus.Finished:
                    return "Finished on " + dateTime;
                default:
                    return "Opened on " + dateTime;
            }
        }
        private string GetDescription(string lobbyName, string currentPlayers, GameStatus gs, int descriptionPadding)
        {
            // Check lenght
            lobbyName = lobbyName.Length > 26 ? lobbyName.Substring(0, 26) : lobbyName;

            if (gs == GameStatus.Finished || gs == GameStatus.InProgress)
            {
                return "~~" + lobbyName + currentPlayers.PadLeft(descriptionPadding) + "~~";
            }
            else
            {
                return lobbyName + currentPlayers.PadLeft(descriptionPadding);
            }
        }
        private Color GetStatusColor(GameStatus status)
        {
            switch (status)
            {
                case GameStatus.Open:
                    return new Color(126, 211, 33);
                case GameStatus.InProgress:
                    return new Color(245, 166, 35);
                case GameStatus.Finished:
                    return new Color(208, 2, 27);
                default:
                    return new Color(255, 255, 255);
            }
        }
    }
}