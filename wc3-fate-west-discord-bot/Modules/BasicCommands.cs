using Discord;
using Discord.Commands;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_data_access_layer.Data.Wc3_Ghost;
using static wc3_fate_west_data_access_layer.Data.Wc3_Ghost.Lobby;
using Microsoft.Extensions.DependencyInjection;
using wc3_fate_west_discord_bot.Extensions;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace wc3_fate_west_discord_bot.Modules
{
    public class BasicCommands : ModuleBase<SocketCommandContext>
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IServiceProvider services;
        private readonly IConfigurationRoot cfg;

        public BasicCommands(IServiceProvider services, IConfigurationRoot cfg)
        {
            this.services = services;
            this.cfg = cfg;
        }
        [Command("say"), Alias("ss")]
        [Summary("Make the bot say something")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public Task Say([Remainder] string text)
            => ReplyAsync(text);
        // Upload replay
        [Command("upload"), Alias("u")]
        [Summary("Upload fate replay.")]
        public async Task UploadCommand()
        {
            var attachment = Context.Message.Attachments.FirstOrDefault();
            var extension = Path.GetExtension(attachment.Filename).ToUpper();

            // Discord size limit for non nitro - 8mb.
            int sizeLimit = 8388608;

            logger.Info(attachment.Size);

            if (attachment.Size > sizeLimit)
            {
                logger.Info("[Discord bot] File to large. " + attachment.Size + " 8 mb limit.");
                await Context.Message.AddReactionAsync(new Emoji("\u274C"));
            }

            if (extension == ".W3G")
            {
                // Parse replay.
                var client = new HttpClient();
                var response = await client.GetAsync(Context.Message.Attachments.First().Url);

                string savePath = cfg.GetSection("DiscordBotSection")["PathForReplay"] + attachment.Filename;

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var fileInfo = new FileInfo(savePath);
                    using (var fileStream = fileInfo.OpenWrite())
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                } 

                await Context.Message.AddReactionAsync(new Emoji("\u2705"));
            }
            else
            {
                logger.Info("[Discord bot] Wrong extension.");
                await Context.Message.AddReactionAsync(new Emoji("\u274C"));
            }
        }
        // Get stats for current user.
        [Command("stats"), Alias("s")]
        [Summary("Get stats for player.")]
        public async Task StatsCommand()
        {
            await StatsCommand(Context.User.Username);
        }
        // Get stats for specific player.
        [Command("stats"), Alias("s")]
        [Summary("Get stats for player.")]
        public async Task StatsCommand(string playerName)
        {
            if(string.IsNullOrWhiteSpace(playerName))
            {
                await ReplyAsync("Player not found");
                return;
            }

            Stats stat = services.GetService<IGhostDataReceiver>().GetGhostStatsForPlayer(playerName);

            if (stat == null || !stat.IsReal)
            {
                await ReplyAsync("Player not found");
                return;
            }
            else
            {
                await ReplyAsync("", false, CreateEmbedForStatCmd(stat, playerName));
            } 
        }
        private Embed CreateEmbedForStatCmd(Stats stat, string playerName)
        {
            int fgRestriction = 20;
            int lgRestriction = 3;

            int totalGamesRestriction = 6;
            int avgLoadRestriction = 5;

            var builder = new EmbedBuilder()
                    .WithTitle("Stats for " + playerName)
                    .WithUrl("https://www.google.com/")
                    .WithColor(Discord.Color.Gold)
                    .AddField(
                "First game".PadRight(49) +
                "Last game".PadRight(49), GetStringForTimeField(stat, fgRestriction, lgRestriction))
                    .AddField(
                "Total games".PadRight(18) +
                "Avg stay".PadRight(13) +
                "Average load".PadRight(14), GetStringForStatField(stat, playerName, totalGamesRestriction, avgLoadRestriction));

            var embed = builder.Build();

            return embed;
        }
        private string GetStringForTimeField(
            Stats stat,
            int fgRestriction,
            int lgRestriction
            )
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("```");

            string field =
                "\n"
                + stat.FirstGameTime
                .CenteredString(fgRestriction - 1) + " | "

                + stat.LastGameTime
                .CenteredString(stat.LastGameTime.Length);

            sb.Append(field);

            sb.Append("```");

            return sb.ToString();
        }
        private string GetStringForStatField(
            Stats stat, string playerName,
            int totalGamesRestriction, 
            int avgLoadRestriction)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("```");

            string field =
                "\n"

                + stat.TotalGames.CenteredString(9) + " | "

                + stat.AvgStayPercent
                .CenteredString(5) + " | "

                + stat.AvgLoadingTime
                .CenteredString(avgLoadRestriction);

            sb.Append(field);

            sb.Append("```");

            return sb.ToString();
        }
    }
}