using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using wc3_fate_west_parser_replay_parser.Data;
using static wc3_fate_west_data_access_layer.Db.FateWestDataAccess;

namespace wc3_fate_west_data_access_layer
{
    public class ReplayDataWriter : IReplayDataWriter
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly FateWestDbContext fateWestDbContext;
        public ReplayDataWriter(FateWestDbContext fateWestDbContext)
        {
            this.fateWestDbContext = fateWestDbContext;
        }
        public void WriteReplayToDb(ReplayData replayData, string serverName)
        {
            logger.Info("Writing to fateWestDbContext.");

            using (var trans = fateWestDbContext.Database.BeginTransaction())
            {
                try
                {

                    if (!fateWestDbContext.server.Any(x => x.ServerName.Equals(serverName) && x.IsServiced))
                        throw new Exception(
                            String.Format("fateWestDbContext Error: Either server name doesn't exist or it is not serviced: {0}",
                                          serverName));

                    server dbServer = fateWestDbContext.server.FirstOrDefault(x => (string)x.ServerName == serverName);
                    // Player collection based on server
                    var dbPlayers = fateWestDbContext.Set<player>().Where(x => x.FK_ServerID == dbServer.ServerID);

                    List<player> fatePlayerList = AddPlayerList(replayData, dbPlayers, dbServer, fateWestDbContext);
                    //AddPlayerList(replayData, dbPlayers, dbServer, fateWestDbContext);


                    fateWestDbContext.SaveChanges();

                    //logger.Info("Add players: " + fatePlayerList.Count);


                    game fateGame = GetNewGame(replayData, dbServer, fateWestDbContext);
                    fateWestDbContext.game.Add(fateGame);

                    List<gameplayerdetail> fateGamePlayerDetailList = GetGamePlayerDetailList(replayData,
                                                                                              fatePlayerList,
                                                                                              fateGame, dbServer, fateWestDbContext);
                    fateWestDbContext.gameplayerdetail.AddRange(fateGamePlayerDetailList);

                    AddPlayerStatToDatabase(replayData, fatePlayerList, fateWestDbContext, dbServer, fateGame);
                    //fateWestDbContext.SaveChanges();
                    AddPlayerHeroStatToDatabase(replayData, fatePlayerList, fateWestDbContext, dbServer);
                    fateWestDbContext.SaveChanges(); //Save changes at this point to assign IDs to tables
                    AddItemPurchaseDetailToDatabase(replayData, fateWestDbContext, fateGamePlayerDetailList, fatePlayerList);
                    AddHeroStatLearnDetailToDatabase(replayData, fateWestDbContext, fateGamePlayerDetailList, fatePlayerList);
                    AddGodsHelpUseToDatabase(replayData, fateWestDbContext, fateGamePlayerDetailList, fatePlayerList);
                    AddAttributeLearnToDatabase(replayData, fateWestDbContext, fateGamePlayerDetailList, fatePlayerList);
                    AddCommandSealUseToDatabase(replayData, fateWestDbContext, fateGamePlayerDetailList, fatePlayerList);

                    fateWestDbContext.SaveChanges();


                    trans.Commit();

                    logger.Info("[DB Writer] Complete successfully.");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    logger.Error(ex.ToString());
                    throw;
                }
            }
        }
        private static void AddItemPurchaseDetailToDatabase(ReplayData replayData, FateWestDbContext fateWestDbContext,
            List<gameplayerdetail> dbPlayerDetailList, List<player> dbPlayerList)
        {
            int maxGameItemPurchaseId = 0;
            if (fateWestDbContext.gameitempurchase.Any())
            {
                maxGameItemPurchaseId = (int)(fateWestDbContext.gameitempurchase.Max(x => x.GameItemPurchaseID) + 1);
            }
            foreach (player player in dbPlayerList)
            {
                PlayerInfo playerInfo = replayData.GetPlayerInfoByPlayerName((string)player.PlayerName);
                if (playerInfo == null)
                    throw new InvalidOperationException("Player Name could not be found from replay data (AIPDTD): " + player.PlayerName);
                var purchasedItemGroup = playerInfo.ItemPurchaseList.GroupBy(x => x)
                    .Select(group => new
                    {
                        ItemTypeID = group.Key,
                        PurchaseCount = group.Count()
                    });

                gameplayerdetail gpDetail = dbPlayerDetailList.First(x => x.FK_PlayerID == player.PlayerID);

                int spentGold = 0;
                foreach (var item in purchasedItemGroup)
                {
                    iteminfo itemInfo = fateWestDbContext.iteminfo.First(x => (string)x.ItemTypeID == item.ItemTypeID);
                    gameitempurchase itemPurchaseRow = new gameitempurchase
                    {
                        GameItemPurchaseID = maxGameItemPurchaseId,
                        FK_GamePlayerDetailID = (int)gpDetail.GamePlayerDetailID,
                        FK_ItemID = itemInfo.ItemID,
                        ItemPurchaseCount = item.PurchaseCount
                    };
                    spentGold += itemInfo.ItemCost * item.PurchaseCount;
                    fateWestDbContext.gameitempurchase.Add(itemPurchaseRow);
                    maxGameItemPurchaseId++;
                }

                gpDetail.GoldSpent = spentGold;
            }
        }

        private static void AddHeroStatLearnDetailToDatabase(ReplayData replayData, FateWestDbContext fateWestDbContext,
            List<gameplayerdetail> dbPlayerDetailList, List<player> dbPlayerList)
        {
            foreach (player player in dbPlayerList)
            {
                PlayerInfo playerInfo = replayData.GetPlayerInfoByPlayerName((string)player.PlayerName);
                if (playerInfo == null)
                    throw new InvalidOperationException("Player Name could not be found from replay data (AddHeroStatLearnDetailToDatabase): " + player.PlayerName);
                var statList = playerInfo.StatList.GroupBy(x => x)
                    .Select(group => new
                    {
                        StatAbilID = group.Key,
                        StatLearnCount = group.Count()
                    });

                gameplayerdetail gpDetail = dbPlayerDetailList.First(x => x.FK_PlayerID == player.PlayerID);

                foreach (var stat in statList)
                {
                    herostatinfo statInfo = fateWestDbContext.herostatinfo.First(x => (string)x.HeroStatAbilID == stat.StatAbilID);
                    herostatlearn statLearned = new herostatlearn()
                    {
                        FK_HeroStatInfoID = (int)statInfo.HeroStatInfoID,
                        FK_GamePlayerDetailID = (int)gpDetail.GamePlayerDetailID,
                        LearnCount = stat.StatLearnCount
                    };
                    fateWestDbContext.herostatlearn.Add(statLearned);
                }
            }
        }

        private static void AddGodsHelpUseToDatabase(ReplayData replayData, FateWestDbContext fateWestDbContext,
            List<gameplayerdetail> dbPlayerDetailList, List<player> dbPlayerList)
        {
            foreach (player player in dbPlayerList)
            {
                PlayerInfo playerInfo = replayData.GetPlayerInfoByPlayerName((string)player.PlayerName);
                if (playerInfo == null)
                    throw new InvalidOperationException("Player Name could not be found from replay data (AddGodsHelpUseToDatabase): " + player.PlayerName);

                gameplayerdetail gpDetail = dbPlayerDetailList.First(x => x.FK_PlayerID == player.PlayerID);

                foreach (var godsHelpAbilId in playerInfo.GodsHelpList)
                {
                    godshelpinfo godsHelpInfo = fateWestDbContext.godshelpinfo.First(x => (string)x.GodsHelpAbilID == godsHelpAbilId);
                    godshelpuse godsHelpUsed = new godshelpuse()
                    {
                        FK_GodsHelpInfoID = (int)godsHelpInfo.GodsHelpInfoID,
                        FK_GamePlayerDetailID = (int)gpDetail.GamePlayerDetailID
                    };
                    fateWestDbContext.godshelpuse.Add(godsHelpUsed);
                }
            }
        }

        private static void AddCommandSealUseToDatabase(ReplayData replayData, FateWestDbContext fateWestDbContext,
            List<gameplayerdetail> dbPlayerDetailList, List<player> dbPlayerList)
        {
            foreach (player player in dbPlayerList)
            {
                PlayerInfo playerInfo = replayData.GetPlayerInfoByPlayerName((string)player.PlayerName);
                if (playerInfo == null)
                    throw new InvalidOperationException("Player Name could not be found from replay data (AddCommandSealUseToDatabase): " + player.PlayerName);

                gameplayerdetail gpDetail = dbPlayerDetailList.First(x => x.FK_PlayerID == player.PlayerID);

                var commandSealGroup = playerInfo.CommandSealList.GroupBy(x => x)
                .Select(group => new
                {
                    CommandSealAbilID = group.Key,
                    UseCount = group.Count()
                });

                foreach (var commandSeal in commandSealGroup)
                {
                    commandsealuses commandSealUsed = new commandsealuses()
                    {
                        FK_GamePlayerDetailID = (int)gpDetail.GamePlayerDetailID,
                        CommandSealAbilID = commandSeal.CommandSealAbilID,
                        UseCount = commandSeal.UseCount

                    };
                    fateWestDbContext.commandsealuses.Add(commandSealUsed);
                }
            }
        }

        private static void AddAttributeLearnToDatabase(ReplayData replayData, FateWestDbContext fateWestDbContext,
            List<gameplayerdetail> dbPlayerDetailList, List<player> dbPlayerList)
        {
            foreach (player player in dbPlayerList)
            {
                PlayerInfo playerInfo = replayData.GetPlayerInfoByPlayerName((string)player.PlayerName);
                if (playerInfo == null)
                    throw new InvalidOperationException("Player Name could not be found from replay data (AddAttributeLearnToDatabase): " + player.PlayerName);

                gameplayerdetail gpDetail = dbPlayerDetailList.First(x => x.FK_PlayerID == player.PlayerID);

                foreach (var attributeAbilId in playerInfo.AttributeList)
                {
                    attributeinfo attributeInfo = fateWestDbContext.attributeinfo.FirstOrDefault(x => x.AttributeAbilID == attributeAbilId);
                    if (attributeInfo == null)
                    {
                        int maxId = 0;
                        if (fateWestDbContext.attributeinfo.Any())
                        {
                            maxId = (int)fateWestDbContext.attributeinfo.Max(x => x.AttributeInfoID);
                        }
                        attributeInfo = new attributeinfo
                        {
                            AttributeInfoID = maxId,
                            AttributeAbilID = attributeAbilId,
                            AttributeName = ""
                        };
                        fateWestDbContext.attributeinfo.Add(attributeInfo);
                    }
                    attributelearn attributeLearned = new attributelearn()
                    {
                        FK_AttributeInfoID = (int)attributeInfo.AttributeInfoID,
                        FK_GamePlayerDetailID = (int)gpDetail.GamePlayerDetailID
                    };
                    fateWestDbContext.attributelearn.Add(attributeLearned);
                }
            }
        }


        private static void AddPlayerHeroStatToDatabase(ReplayData replayData, IEnumerable<player> dbPlayers,
                                                        FateWestDbContext fateWestDbContext, server dbServer)
        {
            int heroStatId = 0;
            heroStatId = fateWestDbContext.playerherostat.AsNoTracking().Any() ? (int)fateWestDbContext.playerherostat.AsNoTracking().Max(g => g.PlayerHeroStatID) + 1 : heroStatId + 1;

            foreach (player player in dbPlayers)
            {


                PlayerInfo playerInfo = replayData.GetPlayerInfoByPlayerName((string)player.PlayerName);
                if (playerInfo == null)
                    throw new Exception(String.Format("Player Name not found during PlayerHeroStat module. Input: {0}",
                                                      player.PlayerName));

                herotype playerHeroType = fateWestDbContext.herotype.First(x => (string)x.HeroUnitTypeID == playerInfo.ServantId);


                playerherostat playerHeroStat =
                    fateWestDbContext.playerherostat.FirstOrDefault(
                        x =>
                        x.FK_ServerID == dbServer.ServerID && x.FK_PlayerID == player.PlayerID &&
                        x.FK_HeroTypeID == playerHeroType.HeroTypeID);

                bool isNewHeroStat = false;



                if (playerHeroStat == null)
                {
                    playerHeroStat = new playerherostat
                    {
                        PlayerHeroStatID = heroStatId,
                        FK_PlayerID = (int)player.PlayerID,
                        FK_ServerID = (int)dbServer.ServerID,
                        FK_HeroTypeID = (int)playerHeroType.HeroTypeID
                    };

                    logger.Info("Player h stat id: " + playerHeroStat.PlayerHeroStatID);

                    heroStatId++;

                    fateWestDbContext.playerherostat.Add(playerHeroStat);
                    isNewHeroStat = true;
                }
                playerHeroStat.HeroPlayCount++;
                playerHeroStat.TotalHeroKills += playerInfo.Kills;
                playerHeroStat.TotalHeroDeaths += playerInfo.Deaths;
                playerHeroStat.TotalHeroAssists += playerInfo.Assists;

                //Upsert
                fateWestDbContext.playerherostat.Attach(playerHeroStat);
                var playerHeroStatEntry = fateWestDbContext.Entry(playerHeroStat);
                playerHeroStatEntry.State = isNewHeroStat ? EntityState.Added : EntityState.Modified;
            }
        }

        private void AddPlayerStatToDatabase(ReplayData replayData, IEnumerable<player> dbPlayers, FateWestDbContext fateWestDbContext,
                                             server dbServer, game fateGame)
        {
            int statId = 1;
            statId = (int)fateWestDbContext.playerstat.AsNoTracking().Max(g => g.PlayerStatID) + 1;

            foreach (player player in dbPlayers)
            {
                //statId = fateWestDbContext.playerstat.AsNoTracking().Any() ? (int)fateWestDbContext.playerstat.AsNoTracking().Max(g => g.PlayerStatID) + 1 : statId;
                bool isNewPlayerStat = false;

                playerstat playerStat =
                    fateWestDbContext.playerstat.FirstOrDefault(
                        x => x.FK_ServerID == dbServer.ServerID && x.FK_PlayerID == player.PlayerID);

                if (playerStat == null)
                {
                    //int statId = fateWestDbContext.playerstat.AsNoTracking().Any() ? (int)fateWestDbContext.playerstat.AsNoTracking().Max(g => g.PlayerStatID) + 1 : 1;
                    //playerId = (int)(fateWestDbContext.player.AsNoTracking().Max(g => g.PlayerID) + 1); //Generate Max GameID + 1
                    playerStat = new playerstat();


                    logger.Info("New stat ID = " + statId);

                    playerStat.PlayerStatID = statId;
                    playerStat.FK_PlayerID = (int)player.PlayerID;
                    playerStat.FK_ServerID = (int)dbServer.ServerID;
                    fateWestDbContext.playerstat.Add(playerStat);
                    isNewPlayerStat = true;

                    statId++;
                }
                playerStat.PlayCount++;

                PlayerInfo playerInfo = replayData.GetPlayerInfoByPlayerName((string)player.PlayerName);
                if (playerInfo == null)
                    throw new Exception(String.Format("Player Name not found during PlayerStatList module. Input: {0}",
                                                      player.PlayerName));

                if ((string)fateGame.Result == GameResult.NONE.ToString())
                    continue;

                if ((string)fateGame.Result == GameResult.T1W.ToString())
                {
                    if (playerInfo.Team == 0)
                        playerStat.Win++;
                    else if (playerInfo.Team == 1)
                        playerStat.Loss++;
                    else
                        throw new Exception(String.Format("Unexpected playerInfo team at PlayerStatListModule. Input: {0}",
                                                          playerInfo.Team));
                }
                else if ((string)fateGame.Result == GameResult.T2W.ToString())
                {
                    if (playerInfo.Team == 0)
                        playerStat.Loss++;
                    else if (playerInfo.Team == 1)
                        playerStat.Win++;
                    else
                        throw new Exception(String.Format("Unexpected playerInfo team at PlayerStatListModule. Input: {0}",
                                                          playerInfo.Team));
                }
                else
                {
                    throw new Exception(String.Format("Unexpected GameResult enumeration at PlayerStatListModule. Input: {0}",
                                                      fateGame.Result));
                }

                //Upsert
                fateWestDbContext.playerstat.Attach(playerStat);
                var playerStatEntry = fateWestDbContext.Entry(playerStat);
                playerStatEntry.State = isNewPlayerStat ? EntityState.Added : EntityState.Modified;

            }
        }

        private List<gameplayerdetail> GetGamePlayerDetailList(ReplayData replayData, IEnumerable<player> dbPlayers, game fateGame, server dbServer,
                                                  FateWestDbContext fateWestDbContext)
        {
            //int gamePlayerDetailId = 0;

            List<gameplayerdetail> fateGamePlayerDetailList = new List<gameplayerdetail>();
            foreach (player player in dbPlayers)
            {
                PlayerInfo playerInfo = replayData.GetPlayerInfoByPlayerName((string)player.PlayerName);
                if (playerInfo == null)
                    throw new Exception(String.Format("Player Name not found during GamePlayerDetailList module. Input: {0}",
                                                      player.PlayerName));

                gameplayerdetail fateGamePlayerDetail = new gameplayerdetail();
                //gamePlayerDetailId = fateWestDbContext.gameplayerdetail.AsNoTracking().Any() ? (int)fateWestDbContext.gameplayerdetail.AsNoTracking().Max(g => g.GamePlayerDetailID) + 1 : gamePlayerDetailId + 1;


                //fateGamePlayerDetail.GamePlayerDetailID = gamePlayerDetailId;
                fateGamePlayerDetail.FK_GameID = (int)fateGame.GameID;
                fateGamePlayerDetail.FK_PlayerID = (int)player.PlayerID;
                fateGamePlayerDetail.FK_ServerID = (int)dbServer.ServerID;

                logger.Info("Game FK_GameID: " + fateGamePlayerDetail.FK_GameID + " FK_PlayerID: " + fateGamePlayerDetail.FK_PlayerID + " FK_ServerID: " + fateGamePlayerDetail.FK_ServerID);

                herotype playerHeroType = fateWestDbContext.herotype.FirstOrDefault(x => (string)x.HeroUnitTypeID == playerInfo.ServantId);
                if (playerHeroType == null)
                    throw new Exception(String.Format("fateWestDbContext Error: Unknown hero type id: {0}", playerInfo.ServantId));
                fateGamePlayerDetail.FK_HeroTypeID = (int)playerHeroType.HeroTypeID;
                fateGamePlayerDetail.Kills = playerInfo.Kills;
                fateGamePlayerDetail.Deaths = playerInfo.Deaths;
                fateGamePlayerDetail.Assists = playerInfo.Assists;
                fateGamePlayerDetail.Team = (playerInfo.Team + 1).ToString();
                fateGamePlayerDetail.DamageTaken = playerInfo.DamageTaken;
                fateGamePlayerDetail.DamageDealt = playerInfo.DamageDealt;
                fateGamePlayerDetail.HeroLevel = playerInfo.ServantLevel;
                if ((string)fateGame.Result == GameResult.NONE.ToString())
                    fateGamePlayerDetail.Result = GamePlayerResult.NONE.ToString();
                else if ((string)fateGame.Result == GameResult.T1W.ToString())
                {
                    fateGamePlayerDetail.Result = playerInfo.Team == 0
                                                      ? GamePlayerResult.WIN.ToString()
                                                      : GamePlayerResult.LOSS.ToString();
                }
                else if ((string)fateGame.Result == GameResult.T2W.ToString())
                {
                    fateGamePlayerDetail.Result = playerInfo.Team == 1
                                                      ? GamePlayerResult.WIN.ToString()
                                                      : GamePlayerResult.LOSS.ToString();
                }
                else
                {
                    throw new Exception(String.Format("Unexpected GameResult enumeration. Input: {0}",
                                                      fateGame.Result));
                }
                fateGamePlayerDetailList.Add(fateGamePlayerDetail);
            }
            return fateGamePlayerDetailList;
        }

        private game GetNewGame(ReplayData replayData, server dbServer, FateWestDbContext fateWestDbContext)
        {
            int gameId = 1;
            if (fateWestDbContext.game.Any())
                gameId = (int)(fateWestDbContext.game.Max(g => g.GameID) + 1); //Generate Max GameID + 1

            game fateGame = new game
            {
                GameID = gameId,
                GameName = replayData.GameName,
                Log = String.Join("\n", replayData.GameChatMessage.ToArray()),
                MatchType = replayData.GameMode.ToString(),
                MapVersion = ReplayData.MapVersion,
                //Duration = new TimeSpan(0, 0, 0, 0, (int)replayData.ReplayHeader.ReplayLength),
                Duration = (int)replayData.ReplayHeader.ReplayLength,
                PlayedDate = replayData.GameDateTime,
                ReplayUrl = replayData.ReplayUrl,
                FK_ServerID = (int)dbServer.ServerID,
                TeamOneWinCount = replayData.TeamOneVictoryCount,
                TeamTwoWinCount = replayData.TeamTwoVictoryCount
            };
            if (replayData.TeamOneVictoryCount > replayData.TeamTwoVictoryCount)
                fateGame.Result = GameResult.T1W.ToString();
            else if (replayData.TeamOneVictoryCount < replayData.TeamTwoVictoryCount)
                fateGame.Result = GameResult.T2W.ToString();
            else
                fateGame.Result = GameResult.NONE.ToString();

            return fateGame;
        }

        private List<player> AddPlayerList(ReplayData replayData, IQueryable<player> dbPlayers, server dbServer,
                                             FateWestDbContext fateWestDbContext)
        {
            List<player> playerList = new List<player>();
            int playerId = 1;
            //playerId = (int)(fateWestDbContext.player.AsNoTracking().Max(g => g.PlayerID) + 1);
            playerId = (int)(fateWestDbContext.player.AsNoTracking().Max(g => g.PlayerID));

            foreach (PlayerInfo playerInfo in replayData.PlayerInfoList)
            {
                //Ignore observer
                if (playerInfo.IsObserver)
                    continue;

                player player = dbPlayers.FirstOrDefault(x => x.PlayerName == playerInfo.PlayerName);



                if (player != null)
                {
                    logger.Info("Player alrdy exist: " + player.PlayerName);
                    playerList.Add(player);
                    continue;
                }
                else
                {

                    logger.Info("Player NOT exist: " + playerInfo.PlayerName + " max id: " + playerId);
                    playerId++;
                }

                /*
                if (fateWestDbContext.player.AsNoTracking().Any())
                {
                    //playerId = (int)(fateWestDbContext.player.AsNoTracking().Max(g => g.PlayerID) + 1); //Generate Max GameID + 1
                    playerId = (int)(fateWestDbContext.player.AsNoTracking().Max(g => g.PlayerID) + 1); //Generate Max GameID + 1
                }
                */


                player = new player
                {
                    FK_ServerID = (int)dbServer.ServerID,
                    IsBanned = false,
                    LastUpdatedBy = "FateRankingSystem",
                    PlayerName = playerInfo.PlayerName,
                    RegDate = DateTime.Now,
                    LastUpdatedOn = DateTime.Now,
                    PlayerID = playerId
                };

                logger.Info("Trying add player id: " + player.PlayerID);

                playerId++;
                playerList.Add(player);
                //var deb = fateWestDbContext.player.Add(player).DebugView;
                fateWestDbContext.player.Add(player);
                //logger.Info(deb.LongView);
            }
            return playerList;
        }
    }

    public interface IReplayDataWriter
    {
        void WriteReplayToDb(ReplayData replayData, string serverName);
    }
}