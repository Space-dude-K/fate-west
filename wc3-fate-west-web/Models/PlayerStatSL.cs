using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_data_access_layer.Data;
using wc3_fate_west_web.Extensions;

namespace wc3_fate_west_web.Models
{
    public class PlayerStatSL
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly FateWestDbContext fateDbContext;

        public PlayerStatSL(FateWestDbContext fateDbContext)
        {
            this.fateDbContext = fateDbContext;
        }
        public PlayerStatsPageViewModel GetPlayerStatSummary(string playerName, string serverName, int lastGameId)
        {
            PlayerStatsPageViewModel vm = new PlayerStatsPageViewModel
            {
                UserName = playerName,
                Server = serverName
            };

            // Get player total summary first.
            PlayerStatSummaryData summaryData = GetPlayerSummary(playerName, serverName);
            if (summaryData == null)
            {
                vm.HasFoundUser = false;
                return vm;
            }

            vm.UserName = summaryData.PlayerName;
            vm.HasFoundUser = true;
            vm.Wins = summaryData.Win;
            vm.Losses = summaryData.Loss;
            vm.WLPercent = $"{(vm.Wins * 1.0 / (vm.Wins + vm.Losses)):0.0%}";
            vm.LastPlayedDateTime = summaryData.LastGamePlayed;
            double avgPlayerKills = (summaryData.TotalPlayerKills * 1.0 / summaryData.TotalPlayerGameCount);
            double avgPlayerDeaths = (summaryData.TotalPlayerDeaths * 1.0 / summaryData.TotalPlayerGameCount);
            double avgPlayerAssists = (summaryData.TotalPlayerAssists * 1.0 / summaryData.TotalPlayerGameCount);
            vm.AveragePlayerKills = avgPlayerKills.ToString("0.0");
            vm.AveragePlayerDeaths = avgPlayerDeaths.ToString("0.0");
            vm.AveragePlayerAssists = avgPlayerAssists.ToString("0.0");
            vm.AveragePlayerKDA =
                ((avgPlayerKills + avgPlayerAssists) / avgPlayerDeaths)
                    .ToString("0.00");


            // Get player hero summary.
            List<PlayerHeroStatSummaryData> heroStatSummaryData = GetPlayerHeroSummary(summaryData.PlayerId);
            List<PlayerHeroStatsViewModel> vmHeroStats = new List<PlayerHeroStatsViewModel>();
            foreach (PlayerHeroStatSummaryData heroSummary in heroStatSummaryData)
            {
                PlayerHeroStatsViewModel vmHero = new PlayerHeroStatsViewModel
                {
                    HeroImageURL = ContentURL.GetHeroIconURL(heroSummary.HeroUnitTypeID),
                    HeroName = heroSummary.HeroName,
                    HeroWins = heroSummary.Wins,
                    HeroLosses = heroSummary.Losses,
                    HeroWLPercent = $"{(heroSummary.Wins * 1.0 / (heroSummary.Wins + heroSummary.Losses)):0.0%}"
                };
                double avgHeroKills = (heroSummary.HeroTotalKills * 1.0 / heroSummary.HeroTotalPlayCount);
                double avgHeroDeaths = (heroSummary.HeroTotalDeaths * 1.0 / heroSummary.HeroTotalPlayCount);
                double avgHeroAssists = (heroSummary.HeroTotalAssists * 1.0 / heroSummary.HeroTotalPlayCount);
                vmHero.HeroAverageKills = avgHeroKills.ToString("0.0");
                vmHero.HeroAverageDeaths = avgHeroDeaths.ToString("0.0");
                vmHero.HeroAverageAssists = avgHeroAssists.ToString("0.0");
                vmHero.HeroAverageKDA = ((avgHeroKills + avgHeroAssists) / avgHeroDeaths).ToString("0.00");

                double heroKDA = ((avgHeroKills + avgHeroAssists) / avgHeroDeaths);
                vmHero.HeroAverageKDA = heroKDA.ToString("0.00");
                vmHero.HeroKDAColor = heroKDA.GetKDAColor();

                vmHeroStats.Add(vmHero);
            }

            vm.PlayerHeroStatSummaryData = vmHeroStats;
            vm.PlayerGameSummaryData = GetPlayerGameSummary(summaryData.PlayerId, lastGameId, "");
            vm.LastGameID = vm.PlayerGameSummaryData.Min(x => x.GameID);

            vm.SearchableServantData = GetSearchableServants();
            return vm;
        }
        private List<SearchableServantData> GetSearchableServants()
        {
            return (
                        from heroType in fateDbContext.herotype
                        join herotypename in fateDbContext.herotypename on heroType.HeroTypeID equals herotypename.FK_HeroTypeID
                        select new SearchableServantData()
                        {
                            HeroUnitTypeID = heroType.HeroUnitTypeID,
                            HeroNameTitle = herotypename.HeroName + " - " + herotypename.HeroTitle
                        }
                     ).ToList();
        }
        private PlayerStatSummaryData GetPlayerSummary(string playerName, string serverName)
        {
            PlayerStatSummaryData summaryData = (
                    from playerStat in fateDbContext.playerstat
                    join player in fateDbContext.player on playerStat.FK_PlayerID equals player.PlayerID
                    join server in fateDbContext.server on player.FK_ServerID equals server.ServerID
                    join playerHeroStat in fateDbContext.playerherostat on player.PlayerID equals playerHeroStat.FK_PlayerID
                    where playerName == player.PlayerName &&
                          serverName == server.ServerName
                    group playerHeroStat by new
                    {
                        player.PlayerID,
                        player.PlayerName,
                        playerStat.Win,
                        playerStat.Loss
                    }
                    into g
                    select new PlayerStatSummaryData()
                    {
                        PlayerId = (int)g.Key.PlayerID,
                        PlayerName = g.Key.PlayerName,
                        Win = g.Key.Win,
                        Loss = g.Key.Loss,
                        TotalPlayerKills = g.Sum(x => x.TotalHeroKills),
                        TotalPlayerDeaths = g.Sum(x => x.TotalHeroDeaths),
                        TotalPlayerAssists = g.Sum(x => x.TotalHeroAssists),
                        TotalPlayerGameCount = g.Sum(x => x.HeroPlayCount)
                    }).FirstOrDefault();

            if (summaryData == null)
                return null;

            DateTime lastGamePlayed = (
                from game in fateDbContext.game
                join gameDetail in fateDbContext.gameplayerdetail on game.GameID equals gameDetail.FK_GameID
                where gameDetail.FK_PlayerID == summaryData.PlayerId
                orderby game.PlayedDate descending
                select game.PlayedDate
                ).First();

            summaryData.LastGamePlayed = lastGamePlayed;
            return summaryData;
        }
        private List<PlayerHeroStatSummaryData> GetPlayerHeroSummary(int playerId)
        {
            var heroSummaryQuery = (
                    from gameDetail in fateDbContext.gameplayerdetail
                    join heroType in fateDbContext.herotype on gameDetail.FK_HeroTypeID equals heroType.HeroTypeID
                    where gameDetail.FK_PlayerID == playerId
                    group gameDetail by new
                    {
                        gameDetail.Result,
                        heroType.HeroUnitTypeID
                    }
                    into g
                    select new
                    {
                        g.Key.HeroUnitTypeID,
                        g.Key.Result,
                        ResultCount = g.Count()
                    });
            List<PlayerHeroStatSummaryData> heroStatSummaryList = new List<PlayerHeroStatSummaryData>();
            foreach (var summaryData in heroSummaryQuery)
            {
                PlayerHeroStatSummaryData heroStatSummary =
                    heroStatSummaryList.FirstOrDefault(x => x.HeroUnitTypeID == summaryData.HeroUnitTypeID);
                if (heroStatSummary == null)
                {
                    heroStatSummary = new PlayerHeroStatSummaryData { HeroUnitTypeID = summaryData.HeroUnitTypeID };
                    heroStatSummaryList.Add(heroStatSummary);
                }
                if (summaryData.Result == "WIN")
                {
                    heroStatSummary.Wins = summaryData.ResultCount;
                }
                else if (summaryData.Result == "LOSS")
                {
                    heroStatSummary.Losses = summaryData.ResultCount;
                }
            }

            var heroSummaryKDAQuery = (
                from playerHeroStat in fateDbContext.playerherostat
                join heroType in fateDbContext.herotype on playerHeroStat.FK_HeroTypeID equals heroType.HeroTypeID
                join heroName in fateDbContext.herotypename on heroType.HeroTypeID equals heroName.FK_HeroTypeID
                where playerHeroStat.FK_PlayerID == playerId
                select new
                {
                    heroName.HeroName,
                    heroType.HeroUnitTypeID,
                    playerHeroStat.HeroPlayCount,
                    playerHeroStat.TotalHeroKills,
                    playerHeroStat.TotalHeroDeaths,
                    playerHeroStat.TotalHeroAssists
                });

            foreach (var kdaData in heroSummaryKDAQuery)
            {
                PlayerHeroStatSummaryData heroStatSummary =
                    heroStatSummaryList.First(x => x.HeroUnitTypeID == kdaData.HeroUnitTypeID);
                heroStatSummary.HeroTotalPlayCount = kdaData.HeroPlayCount;
                heroStatSummary.HeroTotalKills = kdaData.TotalHeroKills;
                heroStatSummary.HeroTotalDeaths = kdaData.TotalHeroDeaths;
                heroStatSummary.HeroTotalAssists = kdaData.TotalHeroAssists;
                heroStatSummary.HeroName = kdaData.HeroName;
            }

            heroStatSummaryList = heroStatSummaryList.OrderByDescending(x => x.HeroTotalPlayCount).ToList();
            return heroStatSummaryList;
        }
        private List<PlayerGameSummaryData> GetPlayerGameSummaryData(int playerId, int gameId, string heroUnitTypeId)
        {
            List<PlayerGameSummaryData> gameSummaryData = (
                    from game in fateDbContext.game
                    join gameDetail in fateDbContext.gameplayerdetail on game.GameID equals gameDetail.FK_GameID
                    join heroType in fateDbContext.herotype on gameDetail.FK_HeroTypeID equals heroType.HeroTypeID
                    where gameDetail.FK_PlayerID == playerId && game.GameID < gameId && heroType.HeroUnitTypeID.Contains(heroUnitTypeId)
                    orderby game.GameID descending
                    select new PlayerGameSummaryData
                    {
                        GameID = (int)game.GameID,
                        GoldSpent = gameDetail.GoldSpent,
                        PlayedDate = game.PlayedDate,
                        GameResult = gameDetail.Result,
                        HeroKills = gameDetail.Kills,
                        HeroDeaths = gameDetail.Deaths,
                        HeroAssists = gameDetail.Assists,
                        HeroUnitTypeID = heroType.HeroUnitTypeID,
                        HeroLevel = gameDetail.HeroLevel,
                        Team = gameDetail.Team,
                        TeamOneWinCount = game.TeamOneWinCount,
                        TeamTwoWinCount = game.TeamTwoWinCount,
                        DamageDealt = gameDetail.DamageDealt,
                        DamageTaken = gameDetail.DamageTaken
                    }).Take(8).ToList();

            foreach (PlayerGameSummaryData data in gameSummaryData)
            {
                var teamQuery = (
                    from gameDetail in fateDbContext.gameplayerdetail
                    join player in fateDbContext.player on gameDetail.FK_PlayerID equals player.PlayerID
                    join heroType in fateDbContext.herotype on gameDetail.FK_HeroTypeID equals heroType.HeroTypeID
                    where gameDetail.FK_GameID == data.GameID
                    select new
                    {
                        player.PlayerName,
                        heroType.HeroUnitTypeID,
                        gameDetail.Team
                    }
                    );
                foreach (var teamData in teamQuery)
                {
                    PlayerGameTeamPlayerData teamPlayer = new PlayerGameTeamPlayerData
                    {
                        PlayerName = teamData.PlayerName,
                        HeroUnitTypeID = teamData.HeroUnitTypeID,
                        Team = teamData.Team
                    };
                    data.TeamList.Add(teamPlayer);
                }
            }

            return gameSummaryData;
        }
        public List<PlayerGameSummaryViewModel> GetPlayerGameSummary(string playerName, string serverName, int lastGameId, string heroUnitTypeId)
        {
            if (heroUnitTypeId == "NONE")
            {
                heroUnitTypeId = "";
            }
            //Get player total summary first
            PlayerStatSummaryData summaryData = GetPlayerSummary(playerName, serverName);
            if (summaryData == null)
            {
                return null;
            }
            return GetPlayerGameSummary(summaryData.PlayerId, lastGameId, heroUnitTypeId);
        }
        private List<PlayerGameSummaryViewModel> GetPlayerGameSummary(int playerId, int lastGameId, string heroUnitTypeId)
        {
            // Get player game summary.
            List<PlayerGameSummaryData> playerGameSummaryData = GetPlayerGameSummaryData(playerId, lastGameId, heroUnitTypeId);
            List<PlayerGameSummaryViewModel> vmGameSummary = new List<PlayerGameSummaryViewModel>();
            foreach (PlayerGameSummaryData gameSummary in playerGameSummaryData)
            {
                PlayerGameSummaryViewModel vmGame = new PlayerGameSummaryViewModel
                {
                    GameID = gameSummary.GameID,
                    PlayedDate = gameSummary.PlayedDate.ToShortDateString(),
                    GoldSpent = $"{gameSummary.GoldSpent:n0}",
                    GameResult = gameSummary.GameResult,
                    HeroKills = gameSummary.HeroKills,
                    HeroDeaths = gameSummary.HeroDeaths,
                    HeroAssists = gameSummary.HeroAssists,
                    HeroLevel = gameSummary.HeroLevel,
                    DamageDealt = $"{((int)gameSummary.DamageDealt):n0}",
                    DamageTaken = $"{((int)gameSummary.DamageTaken):n0}",

                    HeroImageURL = ContentURL.GetHeroIconURL(gameSummary.HeroUnitTypeID),
                };

                double heroKDA =
                    (gameSummary.HeroKills * 1.0 + gameSummary.HeroAssists) / gameSummary.HeroDeaths;
                vmGame.HeroKDA = heroKDA.ToString("0.00");
                vmGame.HeroKDAColor = heroKDA.GetKDAColor();

                //Flip the wins if player is in team 2
                if (gameSummary.Team == "2")
                {
                    vmGame.TeamOneWinCount = gameSummary.TeamTwoWinCount;
                    vmGame.TeamTwoWinCount = gameSummary.TeamOneWinCount;
                }
                else
                {
                    vmGame.TeamOneWinCount = gameSummary.TeamOneWinCount;
                    vmGame.TeamTwoWinCount = gameSummary.TeamTwoWinCount;
                }
                foreach (PlayerGameTeamPlayerData gameTeamPlayerData in gameSummary.TeamList)
                {
                    gameTeamPlayerData.HeroImageURL = ContentURL.GetHeroIconURL(gameTeamPlayerData.HeroUnitTypeID);
                    if (gameTeamPlayerData.Team == "1")
                        vmGame.Team1List.Add(gameTeamPlayerData);
                    else
                        vmGame.Team2List.Add(gameTeamPlayerData);
                }
                vmGameSummary.Add(vmGame);
            }
            return vmGameSummary;
        }
    }
}