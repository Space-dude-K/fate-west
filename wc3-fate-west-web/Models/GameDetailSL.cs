using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using wc3_fate_west_data_access_layer.Data;

namespace wc3_fate_west_web.Models
{
    public class GameDetailSL
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly wc3_fate_west_data_access_layer.FateWestDbContext fateDbContext;

        public GameDetailSL(wc3_fate_west_data_access_layer.FateWestDbContext fateDbContext)
        {
            this.fateDbContext = fateDbContext;
        }
        // TODO
        private List<GamePlayerDetailData> GetGameDetails(int gameId)
        {
            System.Data.Common.DbConnection conn = null;
            List<GamePlayerDetailData> gamePlayerDetailData = null;

            try
            {
                conn = fateDbContext.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();

                command.CommandText = $@"SELECT A.GameID, 
	                                   A.TeamOneWinCount, 
                                       A.TeamTwoWinCount, 
                                       D.PlayerName, 
                                       B.Kills, 
                                       B.Deaths, 
                                       B.Assists, 
                                       B.HeroLevel AS Level, 
                                       B.Team,
                                       B.DamageDealt,
                                       B.DamageTaken,
                                       B.GoldSpent,
                                       C.HeroUnitTypeID, 
                                       group_concat(F.GodsHelpAbilID, ',') AS GodsHelpAbilIDConcat
                                FROM Game A
                                JOIN GamePlayerDetail B
	                                ON A.GameID = B.FK_GameID
                                JOIN HeroType C
	                                ON B.FK_HeroTypeID = C.HeroTypeID
                                JOIN Player D
	                                ON B.FK_PlayerID = D.PlayerID
                                LEFT OUTER JOIN GodsHelpUse E
	                                ON B.GamePlayerDetailID = E.FK_GamePlayerDetailID
                                LEFT OUTER JOIN GodsHelpInfo F
	                                ON E.FK_GodsHelpInfoID = F.GodsHelpInfoID
                                WHERE A.GameID = '{gameId + "'"}
                                GROUP BY A.GameID, A.TeamOneWinCount, A.TeamTwoWinCount, D.PlayerName, B.Kills, B.Deaths, B.Assists, B.HeroLevel, B.Team, B.DamageDealt, B.DamageTaken, B.GoldSpent, C.HeroUnitTypeID;";

                var r = command.ExecuteReader();

                gamePlayerDetailData = new List<GamePlayerDetailData>();

                while (r.Read())
                {
                    GamePlayerDetailData gdd = new GamePlayerDetailData();

                    gdd.GameID = gameId;
                    gdd.TeamOneWinCount = Convert.ToInt32(r["TeamOneWinCount"]);
                    gdd.TeamTwoWinCount = Convert.ToInt32(r["TeamTwoWinCount"]);
                    gdd.PlayerName = Convert.ToString(r["PlayerName"]);
                    gdd.Kills = Convert.ToInt32(r["Kills"]);
                    gdd.Deaths = Convert.ToInt32(r["Deaths"]);
                    gdd.Assists = Convert.ToInt32(r["Assists"]);
                    gdd.Level = Convert.ToInt32(r["Level"]);
                    gdd.Team = Convert.ToString(r["Team"]);
                    gdd.DamageDealt = Convert.ToDouble(r["DamageDealt"]);
                    gdd.DamageTaken = Convert.ToDouble(r["DamageTaken"]);
                    gdd.GoldSpent = Convert.ToInt32(r["GoldSpent"]);
                    gdd.HeroUnitTypeID = Convert.ToString(r["HeroUnitTypeID"]);
                    gdd.GodsHelpAbilIDConcat = Convert.ToString(r["GodsHelpAbilIDConcat"]);

                    gamePlayerDetailData.Add(gdd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                conn.Close();
            }

            return gamePlayerDetailData;
        }
        private PlayerGameBuildData GetPlayerGameBuildDetail(string playerName, int gameId)
        {
            //Find gamePlayerDetailId first
            int gamePlayerDetailId = (int)(
                from game in fateDbContext.game
                join gameDetail in fateDbContext.gameplayerdetail on game.GameID equals gameDetail.FK_GameID
                join player in fateDbContext.player on gameDetail.FK_PlayerID equals player.PlayerID
                where game.GameID == gameId && player.PlayerName == playerName
                select gameDetail.GamePlayerDetailID).FirstOrDefault();

            if (gamePlayerDetailId == 0)
                return null;

            //Find HeroUnitTypeId
            string heroTypeUnitId = (
                from herotype in fateDbContext.herotype
                join gameDetail in fateDbContext.gameplayerdetail on herotype.HeroTypeID equals gameDetail.FK_HeroTypeID
                where gameDetail.GamePlayerDetailID == gamePlayerDetailId
                select herotype.HeroUnitTypeID).First();

            PlayerGameBuildData buildData = new PlayerGameBuildData { HeroUnitTypeId = heroTypeUnitId };

            //Find stat learn information
            var heroStatLearnQuery = (
                from herostatinfo in fateDbContext.herostatinfo
                join herostatlearn in fateDbContext.herostatlearn on herostatinfo.HeroStatInfoID equals
                    herostatlearn.FK_HeroStatInfoID
                where herostatlearn.FK_GamePlayerDetailID == gamePlayerDetailId
                select new
                {
                    herostatinfo.HeroStatAbilID,
                    herostatlearn.LearnCount
                });

            buildData.StatBuildDic = new Dictionary<string, int>();
            foreach (var heroStat in heroStatLearnQuery)
            {
                buildData.StatBuildDic.Add(heroStat.HeroStatAbilID, heroStat.LearnCount);
            }

            //Find attribute information
            var heroAttributeQuery = (
                from attributeinfo in fateDbContext.attributeinfo
                join attributelearn in fateDbContext.attributelearn on attributeinfo.AttributeInfoID equals attributelearn.FK_AttributeInfoID
                where attributelearn.FK_GamePlayerDetailID == gamePlayerDetailId
                select new
                {
                    attributeinfo.AttributeAbilID,
                    attributeinfo.AttributeName
                });

            buildData.LearnedAttributeList = new List<Tuple<string, string>>();
            foreach (var attribute in heroAttributeQuery)
            {
                Tuple<string, string> attrTuple = new Tuple<string, string>(attribute.AttributeAbilID, attribute.AttributeName);
                buildData.LearnedAttributeList.Add(attrTuple);
            }

            //Find Ward/Familiar Information
            var itemKeys = new[] { "I003", "I00N", "SWAR", "I002", "I005", "I00R", "I00G" };

            var itemsQuery = (
                from iteminfo in fateDbContext.iteminfo
                join itembuy in fateDbContext.gameitempurchase on iteminfo.ItemID equals itembuy.FK_ItemID
                where itembuy.FK_GamePlayerDetailID == gamePlayerDetailId && itemKeys.Contains(iteminfo.ItemTypeID)
                select new
                {
                    iteminfo.ItemTypeID,
                    itembuy.ItemPurchaseCount
                });

            buildData.PurchasedItemsDic = new Dictionary<string, int>();
            foreach (var item in itemsQuery)
            {
                buildData.PurchasedItemsDic.Add(item.ItemTypeID, item.ItemPurchaseCount);
            }

            //Find CommandSeal Info
            var commandSealKeys = new[] { "A05Q", "A094", "A043", "A044" };

            var commandSealQuery = (
                from commandseal in fateDbContext.commandsealuses
                where commandseal.FK_GamePlayerDetailID == gamePlayerDetailId && commandSealKeys.Contains(commandseal.CommandSealAbilID)
                select new
                {
                    commandseal.CommandSealAbilID,
                    commandseal.UseCount
                });

            buildData.CommandSealDic = new Dictionary<string, int>();
            foreach (var cs in commandSealQuery)
            {
                buildData.CommandSealDic.Add(cs.CommandSealAbilID, cs.UseCount);
            }

            return buildData;
        }
        private void PopulateImageURL(GamePlayerDetailData data)
        {
            data.HeroImageURL = ContentURL.GetHeroIconURL(data.HeroUnitTypeID);
            data.GodsHelpImageURLList = new List<string>();
            foreach (string url in data.GodsHelpAbilIDList.Where(x => !String.IsNullOrEmpty(x))
                                                          .Select(ContentURL.GetGodsHelpIconURL)
                                                          .Where(url => !String.IsNullOrEmpty(url)))
            {
                data.GodsHelpImageURLList.Add(url);
            }
        }
        public GameDetailData GetGameDetail(int gameId)
        {
            GameDetailData dataModel = new GameDetailData();
            List<GamePlayerDetailData> gameDetailData = GetGameDetails(gameId);
            //Populate godshelp information first
            foreach (GamePlayerDetailData data in gameDetailData)
            {
                data.GodsHelpAbilIDList = data.GodsHelpAbilIDConcat?.Split(',').ToList() ?? new List<string>();
            }

            dataModel.Team1Data = gameDetailData.Where(x => x.Team == "1").ToList();
            dataModel.Team2Data = gameDetailData.Where(x => x.Team == "2").ToList();
            dataModel.Team1WinCount = gameDetailData.First(x => x.Team == "1").TeamOneWinCount;
            dataModel.Team2WinCount = gameDetailData.First(x => x.Team == "2").TeamTwoWinCount;
            dataModel.GameID = gameDetailData.First().GameID;
            foreach (GamePlayerDetailData data in dataModel.Team1Data)
            {
                dataModel.Team1Kills += data.Kills;
                dataModel.Team1Deaths += data.Deaths;
                dataModel.Team1Assists += data.Assists;
                dataModel.Team1Gold += data.GoldSpent;
                dataModel.Team1DamageDealt += data.DamageDealt;
                PopulateImageURL(data);
            }
            foreach (GamePlayerDetailData data in dataModel.Team2Data)
            {
                dataModel.Team2Kills += data.Kills;
                dataModel.Team2Deaths += data.Deaths;
                dataModel.Team2Assists += data.Assists;
                dataModel.Team2Gold += data.GoldSpent;
                dataModel.Team2DamageDealt += data.DamageDealt;
                PopulateImageURL(data);
            }
            return dataModel;
        }
        public PlayerGameBuildViewModel GetPlayerGameBuild(int gameId, string playerName)
        {
            PlayerGameBuildViewModel vm = new PlayerGameBuildViewModel();
            PlayerGameBuildData data = GetPlayerGameBuildDetail(playerName, gameId);

            //TO DO: Is there any cleaner way of doing this?
            vm.PlayerName = playerName;
            vm.HeroIconURL = ContentURL.GetHeroIconURL(data.HeroUnitTypeId);
            vm.Strength = data.StatBuildDic.GetValueOrDefault("A02W");
            vm.Agility = data.StatBuildDic.GetValueOrDefault("A03D");
            vm.Intelligence = data.StatBuildDic.GetValueOrDefault("A03E");
            vm.Attack = data.StatBuildDic.GetValueOrDefault("A03W");
            vm.Armor = data.StatBuildDic.GetValueOrDefault("A03X");
            vm.HealthRegen = data.StatBuildDic.GetValueOrDefault("A03Y");
            vm.ManaRegen = data.StatBuildDic.GetValueOrDefault("A03Z");
            vm.MoveSpeed = data.StatBuildDic.GetValueOrDefault("A04Y");
            vm.GoldRegen = data.StatBuildDic.GetValueOrDefault("A0A9");
            vm.PrelatiMana = data.StatBuildDic.GetValueOrDefault("A0CJ");
            vm.WardCount += data.PurchasedItemsDic.GetValueOrDefault("I003");
            vm.WardCount += data.PurchasedItemsDic.GetValueOrDefault("I00N");
            vm.WardCount += data.PurchasedItemsDic.GetValueOrDefault("SWAR");
            vm.FamiliarCount += data.PurchasedItemsDic.GetValueOrDefault("I002");
            vm.FamiliarCount += data.PurchasedItemsDic.GetValueOrDefault("I005");
            vm.SpiritLinkCount += data.PurchasedItemsDic.GetValueOrDefault("I00G");
            vm.SpiritLinkCount += data.PurchasedItemsDic.GetValueOrDefault("I00R");
            vm.FirstCS = data.CommandSealDic.GetValueOrDefault("A094");
            vm.SecondCS = data.CommandSealDic.GetValueOrDefault("A043");
            vm.ThirdCS = data.CommandSealDic.GetValueOrDefault("A044");
            vm.FourthCS = data.CommandSealDic.GetValueOrDefault("A05Q");

            vm.AttributeList = new List<PlayerGameBuildAttribute>();
            foreach (Tuple<string, string> attributeData in data.LearnedAttributeList)
            {
                PlayerGameBuildAttribute attr = new PlayerGameBuildAttribute
                {
                    AttributeImgUrl = ContentURL.GetAttributeIconURL(attributeData.Item1),
                    AttributeName = attributeData.Item2
                };
                vm.AttributeList.Add(attr);
            }
            return vm;
        }
    }
}