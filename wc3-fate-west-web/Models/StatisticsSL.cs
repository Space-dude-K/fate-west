using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_data_access_layer.Data;
using wc3_fate_west_web.Extensions;

namespace wc3_fate_west_web.Models
{
    public class StatisticsSL
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly FateWestDbContext fateDbContext;

        public StatisticsSL(FateWestDbContext fateDbContext)
        {
            this.fateDbContext = fateDbContext;
        }
        public List<ServantStatisticsViewModel> GetServantStatistics()
        {
            List<ServantStatisticsData> statisticsData = GetServantStatisticsFromDb();
            List<ServantStatisticsViewModel> vm = new List<ServantStatisticsViewModel>();
            foreach (ServantStatisticsData data in statisticsData)
            {
                Debug.WriteLine("ServantStatisticsData: " + data.HeroTypeId);

                ServantStatisticsViewModel vmInner = new ServantStatisticsViewModel
                {
                    HeroTypeId = $"{data.HeroTypeId}",
                    HeroImageURL = ContentURL.GetHeroIconURL(data.HeroUnitTypeID),
                    AvgDamageDealt = $"{data.AvgDamageDealt:n0}",
                    AvgDamageTaken = $"{data.AvgDamageTaken:n0}",
                    AvgGoldSpent = $"{data.AvgGoldSpent:n0}",
                    HeroName = data.HeroName,
                    HeroTitle = data.HeroTitle,
                    PlayCount = data.WinCount + data.LossCount,
                    WinRatioVal = data.WinCount * 1.0 / (data.WinCount + data.LossCount),
                    ProgressBarColor = data.WinCount < data.LossCount || data.WinCount == 0 ? "progressbar red" : "progressbar blue",
                    AvgKDA = ((data.AvgKills + data.AvgAssists) / data.AvgDeaths).ToString("0.00")
                };

                vmInner.WinRatio = $"{vmInner.WinRatioVal:0.0%}".Replace(',', '.');
                vm.Add(vmInner);
            }
            vm = vm.OrderByDescending(x => x.WinRatioVal).ThenByDescending(y => y.PlayCount).ToList();
            return vm;
        }
        private PStatOrderType GetPlayerStatOrderType(int orderType)
        {
            switch (orderType)
            {
                case 1:
                    return PStatOrderType.WinRate;
                case 2:
                    return PStatOrderType.KDA;
                case 3:
                    return PStatOrderType.DamageDealt;
                case 4:
                    return PStatOrderType.DamageTaken;
                default:
                    throw new ArgumentOutOfRangeException("Invalid order type given: " + orderType);
            }
        }
        private PStatTime GetPlayerStatTimeFilter(int time)
        {
            switch (time)
            {
                case 1:
                    return PStatTime.LastSevenDays;
                case 2:
                    return PStatTime.LastThirtyDays;
                case 3:
                    return PStatTime.AllTime;
                default:
                    throw new ArgumentOutOfRangeException("Invalid time filter given: " + time);
            }
        }
        public List<PlayerStatisticsViewModel> GetPlayerStatistics(int orderType, int time)
        {
            List<PlayerStatisticsData> playerStatisticsData = GetPlayerStatisticsFromDb(GetPlayerStatOrderType(orderType), GetPlayerStatTimeFilter(time));
            List<PlayerStatisticsViewModel> vm = new List<PlayerStatisticsViewModel>();
            foreach (PlayerStatisticsData data in playerStatisticsData)
            {
                PlayerStatisticsViewModel vmInner = new PlayerStatisticsViewModel
                {
                    PlayerName = data.PlayerName,
                    WinRatio = $"{data.WinRate:0.0%}".Replace(',','.'),
                    WinRatioPBColor = data.Wins < data.Losses || data.Wins == 0 ? "progressbar red" : "progressbar blue",
                    AvgKDA = data.KDA.ToString("0.00"),
                    AvgDamageDealt = $"{data.AvgDamageDealt:n0}",
                    AvgDamageTaken = $"{data.AvgDamageTaken:n0}",
                    AvgGoldSpent = $"{data.AvgGoldSpent:n0}",
                    KDAColor = data.KDA.GetKDAColor()
                };

                vm.Add(vmInner);
            }
            return vm;
        }
        public Dictionary<string, int> GetTotalGamesPlayedVersion()
        {
            var gamePlayCountDict = new Dictionary<string, int>();

            var gamesPlayedByVersion = (
                    from game in fateDbContext.game
                    group game by new { game.MapVersion } into g
                    select new
                    {
                        g.Key.MapVersion,
                        PlayedTotal = g.Count()
                    });

            foreach (var version in gamesPlayedByVersion)
            {
                gamePlayCountDict[version.MapVersion] = version.PlayedTotal;
            }

            return gamePlayCountDict;
        }
        public List<ServantDetailStatData> GetServantDetailStatPoints(int typeId = 0)
        {
            System.Data.Common.DbConnection conn = null;
            List<ServantDetailStatData> servantStatPointDetails = null;

            try
            {
                conn = fateDbContext.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();

                command.CommandText = $@"select 
		                            g.MapVersion,
                                    stf.HeroStatAbilId as StatTypeId,
		                            stf.HeroStatName as StatTypeName,
		                            sum(stl.LearnCount) as Points
                                from HeroTypeName h
                                inner join GamePlayerDetail gpd on (h.FK_HeroTypeId = gpd.FK_HeroTypeId)
                                inner join Game g on (gpd.FK_GameID = g.GameId)
                                inner join HeroStatLearn stl on (gpd.GamePlayerDetailID = stl.FK_GamePlayerDetailID)
                                inner join HeroStatInfo stf on (stl.FK_HeroStatInfoID = stf.HeroStatInfoID)
                                WHERE h.FK_HeroTypeId = '{typeId + "'"}
                                group by g.MapVersion, stf.HeroStatName;";

                var r = command.ExecuteReader();

                servantStatPointDetails = new List<ServantDetailStatData>();

                while (r.Read())
                {
                    ServantDetailStatData sds = new ServantDetailStatData();

                    sds.MapVersion = Convert.ToString(r["MapVersion"]);
                    sds.StatTypeId = Convert.ToString(r["StatTypeId"]);
                    sds.StatTypeName = Convert.ToString(r["StatTypeName"]);
                    sds.Points = Convert.ToInt32(r["Points"]);

                    servantStatPointDetails.Add(sds);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return servantStatPointDetails;
        }
        public List<ServantDetailData> GetServantDetailStatistics(int typeId = 0)
        {
            System.Data.Common.DbConnection conn = null;
            List<ServantDetailData> servantDetails = null;

            try
            {
                conn = fateDbContext.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();

                command.CommandText = $@"SELECT ht.HeroUnitTypeID AS HeroUnitTypeID,
                       h.HeroTitle AS HeroTitle,
                       g.MapVersion,
                       SUM(gpd.DamageDealt) AS DamageDealt,
                       SUM(gpd.DamageTaken) AS DamageTaken,
                       SUM(gpd.GoldSpent) AS GoldSpent,
                       SUM(gpd.Kills) AS KillCount,
                       SUM(gpd.Assists) AS AssistCount,
                       SUM(gpd.Deaths) AS DeathCount,
                       COUNT( * ) AS PlayCount,
                       SUM(CASE WHEN gpd.Result = 'Win' THEN 1 ELSE 0 END) AS WinCount,
                       SUM(time(g.Duration, 'unixepoch') ) AS GameDuration,
                       SUM(CASE WHEN gpd.result = 'WIN' THEN (CASE WHEN gpd.team = '1' THEN g.TeamOneWinCount - g.TeamTwoWinCount ELSE g.TeamTwoWinCount - g.TeamOneWinCount END) ELSE 0 END) AS WinScoreDifference,
                       SUM(CASE WHEN gpd.result = 'LOSS' THEN (CASE WHEN gpd.team = '1' THEN g.TeamTwoWinCount - g.TeamOneWinCount ELSE g.TeamOneWinCount - g.TeamTwoWinCount END) ELSE 0 END) AS LossScoreDifference
                       FROM HeroTypeName h
                        INNER JOIN
                        HeroType ht ON (h.FK_HeroTypeId = ht.HeroTypeId) 
                        INNER JOIN
                        gameplayerdetail gpd ON (h.FK_HeroTypeId = gpd.FK_HeroTypeId) 
                        INNER JOIN
                        game g ON (gpd.FK_GameID = g.GameId) 
                            WHERE h.FK_HeroTypeId = '{typeId + "'"}
                            GROUP BY h.herotitle, g.MapVersion;";

                var r = command.ExecuteReader();

                servantDetails = new List<ServantDetailData>();

                while (r.Read())
                {
                    ServantDetailData sdd = new ServantDetailData();

                    sdd.HeroUnitTypeID = Convert.ToString(r["HeroUnitTypeID"]);
                    sdd.HeroTitle = Convert.ToString(r["HeroTitle"]);
                    sdd.MapVersion = Convert.ToString(r["MapVersion"]);
                    sdd.DamageDealt = Convert.ToDouble(r["DamageDealt"]);
                    sdd.DamageTaken = Convert.ToDouble(r["DamageTaken"]);
                    sdd.GoldSpent = Convert.ToDouble(r["GoldSpent"]);
                    sdd.KillCount = Convert.ToInt32(r["KillCount"]);
                    sdd.AssistCount = Convert.ToInt32(r["AssistCount"]);
                    sdd.DeathCount = Convert.ToInt32(r["DeathCount"]);
                    sdd.PlayCount = Convert.ToInt32(r["PlayCount"]);
                    sdd.WinCount = Convert.ToInt32(r["WinCount"]);
                    sdd.GameDuration = Convert.ToInt32(r["GameDuration"]);
                    sdd.WinScoreDifference = Convert.ToDouble(r["WinScoreDifference"]);
                    sdd.LossScoreDifference = Convert.ToDouble(r["LossScoreDifference"]);

                    servantDetails.Add(sdd);

                    Debug.WriteLine("GetServantDetailStatistics " + sdd.HeroTitle);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                conn.Close();
            }

            if (servantDetails.Count == 0)
                logger.Error("Servant details not found! " + typeId);

            Debug.WriteLine("GetServantDetailStatistics count: " + servantDetails.Count);

            return servantDetails;
        }
        private List<ServantStatisticsData> GetServantStatisticsFromDb()
        {
            System.Data.Common.DbConnection conn = null;
            List<ServantStatisticsData> servantStatistics = null;

            try
            {
                conn = fateDbContext.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();

                command.CommandText = @"SELECT B.HeroUnitTypeID AS HeroUnitTypeID,
                                B.HeroTypeId AS HeroTypeId,
	                            C.HeroName AS HeroName,
                                C.HeroTitle AS HeroTitle,
	                            SUM(CASE WHEN Result='WIN' THEN 1 ELSE 0 END) WinCount, 
                                SUM(CASE WHEN Result='LOSS' THEN 1 ELSE 0 END) LossCount, 
                                AVG(Kills) AS AvgKills, 
                                AVG(Deaths) AS AvgDeaths, 
                                AVG(Assists) AS AvgAssists,
                                AVG(GoldSpent) AS AvgGoldSpent, 
                                AVG(DamageDealt) AS AvgDamageDealt, 
                                AVG(DamageTaken) AS AvgDamageTaken
                        FROM GAMEPLAYERDETAIL A
                        JOIN HEROTYPE B
	                        ON A.FK_HeroTypeID = B.HeroTypeID
                        JOIN HEROTYPENAME C
	                        ON C.FK_HeroTypeID = B.HeroTypeID
                        GROUP BY A.FK_HeroTypeID;";

                var r = command.ExecuteReader();

                servantStatistics = new List<ServantStatisticsData>();

                while (r.Read())
                {
                    ServantStatisticsData ssd = new ServantStatisticsData();

                    ssd.HeroUnitTypeID = Convert.ToString(r["HeroUnitTypeID"]);
                    ssd.HeroTypeId = Convert.ToString(r["HeroTypeId"]);
                    ssd.HeroName = Convert.ToString(r["HeroName"]);
                    ssd.HeroTitle = Convert.ToString(r["HeroTitle"]);
                    ssd.WinCount = Convert.ToInt32(r["WinCount"]);
                    ssd.LossCount = Convert.ToInt32(r["LossCount"]);
                    ssd.AvgKills = Convert.ToDouble(r["AvgKills"]);
                    ssd.AvgDeaths = Convert.ToDouble(r["AvgDeaths"]);
                    ssd.AvgAssists = Convert.ToDouble(r["AvgAssists"]);
                    ssd.AvgGoldSpent = Convert.ToDouble(r["AvgGoldSpent"]);
                    ssd.AvgDamageDealt = Convert.ToDouble(r["AvgDamageDealt"]);
                    ssd.AvgDamageTaken = Convert.ToDouble(r["AvgDamageTaken"]);

                    servantStatistics.Add(ssd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                servantStatistics = null;
                throw;
            }
            finally
            {
                conn.Close();
            }

            return servantStatistics;
        }
        public enum PStatOrderType
        {
            WinRate,
            KDA,
            DamageDealt,
            DamageTaken
        };
        public enum PStatTime
        {
            LastSevenDays,
            LastThirtyDays,
            AllTime
        }
        public List<PlayerStatisticsData> GetPlayerStatisticsFromDb(PStatOrderType orderType, PStatTime time)
        {
            System.Data.Common.DbConnection conn = null;
            List<PlayerStatisticsData> playerStatistics = null;

            string orderByClause;

            switch (orderType)
            {
                case PStatOrderType.WinRate:
                    orderByClause = "ORDER BY WinRate DESC";
                    break;
                case PStatOrderType.KDA:
                    orderByClause = "ORDER BY KDA DESC";
                    break;
                case PStatOrderType.DamageDealt:
                    orderByClause = "ORDER BY AvgDamageDealt DESC";
                    break;
                case PStatOrderType.DamageTaken:
                    orderByClause = "ORDER BY AvgDamageTaken DESC";
                    break;
                default:
                    throw new InvalidEnumArgumentException("PStatOrderType Invalid Enum");
            }

            string timeClause;

            switch (time)
            {
                case PStatTime.LastSevenDays:
                    timeClause = "WHERE PlayedDate BETWEEN datetime('now', '-7 day') AND datetime('now')";
                    break;
                case PStatTime.LastThirtyDays:
                    timeClause = "WHERE PlayedDate BETWEEN datetime('now', '-30 day') AND datetime('now')";
                    break;
                case PStatTime.AllTime:
                    timeClause = "";
                    break;
                default:
                    throw new InvalidEnumArgumentException("PStatTime Invalid Enum");
            }

            try
            {
                conn = fateDbContext.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();

                command.CommandText = $@"SELECT PlayerName, Wins, Losses, Wins * 1.0 / (Wins + Losses) AS WinRate,
                                    (Kills + Assists) / (Deaths) AS KDA,
                                    AvgDamageDealt, AvgDamageTaken, AvgGoldSpent
                        FROM (
                        SELECT 
	                        C.PlayerName AS PlayerName, 
                            SUM(CASE WHEN B.Result = 'LOSS' THEN 1 ELSE 0 END) Losses, 
                            SUM(CASE WHEN B.Result = 'WIN' THEN 1 ELSE 0 END) Wins,
                            SUM(B.Kills) AS Kills,
                            SUM(B.Deaths) AS Deaths,
                            SUM(B.Assists) AS Assists,
                            SUM(B.DamageDealt) / COUNT(B.DamageDealt) AS AvgDamageDealt,
                            SUM(B.DamageTaken) / COUNT(B.DamageTaken) AS AvgDamageTaken,
                            SUM(B.GoldSpent) / COUNT(B.GoldSpent) AS AvgGoldSpent
                        FROM Game A
                        JOIN GamePlayerDetail B
	                        ON A.GameID = B.FK_GameID
                        JOIN Player C
	                        ON B.FK_PlayerID = C.PlayerID
                        {timeClause}
                        GROUP BY C.PlayerName
                        HAVING COUNT(C.PlayerID) >= 10
                        ) Result
                        {orderByClause}";

                var r = command.ExecuteReader();

                playerStatistics = new List<PlayerStatisticsData>();

                while (r.Read())
                {
                    PlayerStatisticsData psd = new PlayerStatisticsData();

                    psd.PlayerName = Convert.ToString(r["PlayerName"]);
                    psd.Wins = Convert.ToInt32(r["Wins"]);
                    psd.Losses = Convert.ToInt32(r["Losses"]);
                    psd.WinRate = Convert.ToDouble(r["WinRate"]);
                    psd.KDA = Convert.ToDouble(r["KDA"]);
                    psd.AvgDamageDealt = Convert.ToDouble(r["AvgDamageDealt"]);
                    psd.AvgDamageTaken = Convert.ToDouble(r["AvgDamageDealt"]);
                    psd.AvgGoldSpent = Convert.ToDouble(r["AvgGoldSpent"]);

                    playerStatistics.Add(psd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return playerStatistics;
        }
    }
}