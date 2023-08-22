using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using wc3_fate_west_parser_replay_parser.Data;
using static wc3_fate_west_data_access_layer.Db.FateWestDataAccess;

namespace wc3_fate_west_data_access_layer
{
    public class DbWriter
    {
        private readonly FateWestDbContext dbContext;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public DbWriter(FateWestDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void WriteToDb(ReplayData replayData, string serverName)
        {
            logger.Info("Writing to dbContext.");

            using (var trans = dbContext.Database.BeginTransaction())
            {
                try
                {

                    if (!dbContext.server.Any(x => x.ServerName.Equals(serverName) && x.IsServiced))
                        throw new Exception(
                            String.Format("dbContext Error: Either server name doesn't exist or it is not serviced: {0}",
                                          serverName));

                    server dbServer = dbContext.server.FirstOrDefault(x => (string)x.ServerName == serverName);
                    // Player collection based on server
                    var dbPlayers = dbContext.Set<player>().Where(x => x.FK_ServerID == dbServer.ServerID);

                    List<player> fatePlayerList = AddPlayerList(replayData, dbPlayers, dbServer, dbContext);
                    //AddPlayerList(replayData, dbPlayers, dbServer, dbContext);


                    dbContext.SaveChanges();

                    //logger.Info("Add players: " + fatePlayerList.Count);

                    
                    game fateGame = GetNewGame(replayData, dbServer, dbContext);
                    dbContext.game.Add(fateGame);

                    List<gameplayerdetail> fateGamePlayerDetailList = GetGamePlayerDetailList(replayData,
                                                                                              fatePlayerList,
                                                                                              fateGame, dbServer, dbContext);
                    dbContext.gameplayerdetail.AddRange(fateGamePlayerDetailList);

                    AddPlayerStatToDatabase(replayData, fatePlayerList, dbContext, dbServer, fateGame);
                    //dbContext.SaveChanges();
                    AddPlayerHeroStatToDatabase(replayData, fatePlayerList, dbContext, dbServer);
                    dbContext.SaveChanges(); //Save changes at this point to assign IDs to tables
                    AddItemPurchaseDetailToDatabase(replayData, dbContext, fateGamePlayerDetailList, fatePlayerList);
                    AddHeroStatLearnDetailToDatabase(replayData, dbContext, fateGamePlayerDetailList, fatePlayerList);
                    AddGodsHelpUseToDatabase(replayData, dbContext, fateGamePlayerDetailList, fatePlayerList);
                    AddAttributeLearnToDatabase(replayData, dbContext, fateGamePlayerDetailList, fatePlayerList);
                    AddCommandSealUseToDatabase(replayData, dbContext, fateGamePlayerDetailList, fatePlayerList);

                    dbContext.SaveChanges();

                    
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
        private static void AddItemPurchaseDetailToDatabase(ReplayData replayData, FateWestDbContext dbContext,
            List<gameplayerdetail> dbPlayerDetailList, List<player> dbPlayerList)
        {
            int maxGameItemPurchaseId = 0;
            if (dbContext.gameitempurchase.Any())
            {
                maxGameItemPurchaseId = (int)(dbContext.gameitempurchase.Max(x => x.GameItemPurchaseID) + 1);
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
                    iteminfo itemInfo = dbContext.iteminfo.First(x => (string)x.ItemTypeID == item.ItemTypeID);
                    gameitempurchase itemPurchaseRow = new gameitempurchase
                    {
                        GameItemPurchaseID = maxGameItemPurchaseId,
                        FK_GamePlayerDetailID = (int)gpDetail.GamePlayerDetailID,
                        FK_ItemID = itemInfo.ItemID,
                        ItemPurchaseCount = item.PurchaseCount
                    };
                    spentGold += itemInfo.ItemCost * item.PurchaseCount;
                    dbContext.gameitempurchase.Add(itemPurchaseRow);
                    maxGameItemPurchaseId++;
                }

                gpDetail.GoldSpent = spentGold;
            }
        }

        private static void AddHeroStatLearnDetailToDatabase(ReplayData replayData, FateWestDbContext dbContext,
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
                    herostatinfo statInfo = dbContext.herostatinfo.First(x => (string)x.HeroStatAbilID == stat.StatAbilID);
                    herostatlearn statLearned = new herostatlearn()
                    {
                        FK_HeroStatInfoID = (int)statInfo.HeroStatInfoID,
                        FK_GamePlayerDetailID = (int)gpDetail.GamePlayerDetailID,
                        LearnCount = stat.StatLearnCount
                    };
                    dbContext.herostatlearn.Add(statLearned);
                }
            }
        }

        private static void AddGodsHelpUseToDatabase(ReplayData replayData, FateWestDbContext dbContext,
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
                    godshelpinfo godsHelpInfo = dbContext.godshelpinfo.First(x => (string)x.GodsHelpAbilID == godsHelpAbilId);
                    godshelpuse godsHelpUsed = new godshelpuse()
                    {
                        FK_GodsHelpInfoID = (int)godsHelpInfo.GodsHelpInfoID,
                        FK_GamePlayerDetailID = (int)gpDetail.GamePlayerDetailID
                    };
                    dbContext.godshelpuse.Add(godsHelpUsed);
                }
            }
        }

        private static void AddCommandSealUseToDatabase(ReplayData replayData, FateWestDbContext dbContext,
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
                    dbContext.commandsealuses.Add(commandSealUsed);
                }
            }
        }

        private static void AddAttributeLearnToDatabase(ReplayData replayData, FateWestDbContext dbContext,
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
                    attributeinfo attributeInfo = dbContext.attributeinfo.FirstOrDefault(x => x.AttributeAbilID == attributeAbilId);
                    if (attributeInfo == null)
                    {
                        int maxId = 0;
                        if (dbContext.attributeinfo.Any())
                        {
                            maxId = (int)dbContext.attributeinfo.Max(x => x.AttributeInfoID);
                        }
                        attributeInfo = new attributeinfo
                        {
                            AttributeInfoID = maxId,
                            AttributeAbilID = attributeAbilId,
                            AttributeName = ""
                        };
                        dbContext.attributeinfo.Add(attributeInfo);
                    }
                    attributelearn attributeLearned = new attributelearn()
                    {
                        FK_AttributeInfoID = (int)attributeInfo.AttributeInfoID,
                        FK_GamePlayerDetailID = (int)gpDetail.GamePlayerDetailID
                    };
                    dbContext.attributelearn.Add(attributeLearned);
                }
            }
        }


        private static void AddPlayerHeroStatToDatabase(ReplayData replayData, IEnumerable<player> dbPlayers,
                                                        FateWestDbContext dbContext, server dbServer)
        {
            int heroStatId = 0;
            heroStatId = dbContext.playerherostat.AsNoTracking().Any() ? (int)dbContext.playerherostat.AsNoTracking().Max(g => g.PlayerHeroStatID) + 1 : heroStatId + 1;

            foreach (player player in dbPlayers)
            {
                

                PlayerInfo playerInfo = replayData.GetPlayerInfoByPlayerName((string)player.PlayerName);
                if (playerInfo == null)
                    throw new Exception(String.Format("Player Name not found during PlayerHeroStat module. Input: {0}",
                                                      player.PlayerName));

                herotype playerHeroType = dbContext.herotype.First(x => (string)x.HeroUnitTypeID == playerInfo.ServantId);

                
                playerherostat playerHeroStat =
                    dbContext.playerherostat.FirstOrDefault(
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

                    dbContext.playerherostat.Add(playerHeroStat);
                    isNewHeroStat = true;
                }
                playerHeroStat.HeroPlayCount++;
                playerHeroStat.TotalHeroKills += playerInfo.Kills;
                playerHeroStat.TotalHeroDeaths += playerInfo.Deaths;
                playerHeroStat.TotalHeroAssists += playerInfo.Assists;

                //Upsert
                dbContext.playerherostat.Attach(playerHeroStat);
                var playerHeroStatEntry = dbContext.Entry(playerHeroStat);
                playerHeroStatEntry.State = isNewHeroStat ? EntityState.Added : EntityState.Modified;
            }
        }

        private void AddPlayerStatToDatabase(ReplayData replayData, IEnumerable<player> dbPlayers, FateWestDbContext dbContext,
                                             server dbServer, game fateGame)
        {
            int statId = 1;
            statId = (int)dbContext.playerstat.AsNoTracking().Max(g => g.PlayerStatID) + 1;

            foreach (player player in dbPlayers)
            {
                //statId = dbContext.playerstat.AsNoTracking().Any() ? (int)dbContext.playerstat.AsNoTracking().Max(g => g.PlayerStatID) + 1 : statId;
                bool isNewPlayerStat = false;

                playerstat playerStat =
                    dbContext.playerstat.FirstOrDefault(
                        x => x.FK_ServerID == dbServer.ServerID && x.FK_PlayerID == player.PlayerID);

                if (playerStat == null)
                {
                    //int statId = dbContext.playerstat.AsNoTracking().Any() ? (int)dbContext.playerstat.AsNoTracking().Max(g => g.PlayerStatID) + 1 : 1;
                    //playerId = (int)(dbContext.player.AsNoTracking().Max(g => g.PlayerID) + 1); //Generate Max GameID + 1
                    playerStat = new playerstat();


                    logger.Info("New stat ID = " + statId);

                    playerStat.PlayerStatID = statId;
                    playerStat.FK_PlayerID = (int)player.PlayerID;
                    playerStat.FK_ServerID = (int)dbServer.ServerID;
                    dbContext.playerstat.Add(playerStat);
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
                dbContext.playerstat.Attach(playerStat);
                var playerStatEntry = dbContext.Entry(playerStat);
                playerStatEntry.State = isNewPlayerStat ? EntityState.Added : EntityState.Modified;

            }
        }

        private List<gameplayerdetail> GetGamePlayerDetailList(ReplayData replayData, IEnumerable<player> dbPlayers, game fateGame, server dbServer,
                                                  FateWestDbContext dbContext)
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
                //gamePlayerDetailId = dbContext.gameplayerdetail.AsNoTracking().Any() ? (int)dbContext.gameplayerdetail.AsNoTracking().Max(g => g.GamePlayerDetailID) + 1 : gamePlayerDetailId + 1;

                
                //fateGamePlayerDetail.GamePlayerDetailID = gamePlayerDetailId;
                fateGamePlayerDetail.FK_GameID = (int)fateGame.GameID;
                fateGamePlayerDetail.FK_PlayerID = (int)player.PlayerID;
                fateGamePlayerDetail.FK_ServerID = (int)dbServer.ServerID;

                logger.Info("Game FK_GameID: " + fateGamePlayerDetail.FK_GameID + " FK_PlayerID: " + fateGamePlayerDetail.FK_PlayerID + " FK_ServerID: " + fateGamePlayerDetail.FK_ServerID);

                herotype playerHeroType = dbContext.herotype.FirstOrDefault(x => (string)x.HeroUnitTypeID == playerInfo.ServantId);
                if (playerHeroType == null)
                    throw new Exception(String.Format("dbContext Error: Unknown hero type id: {0}", playerInfo.ServantId));
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

        private game GetNewGame(ReplayData replayData, server dbServer, FateWestDbContext dbContext)
        {
            int gameId = 1;
            if (dbContext.game.Any())
                gameId = (int)(dbContext.game.Max(g => g.GameID) + 1); //Generate Max GameID + 1

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
                                             FateWestDbContext dbContext)
        {
            List<player> playerList = new List<player>();
            int playerId = 1;
            //playerId = (int)(dbContext.player.AsNoTracking().Max(g => g.PlayerID) + 1);
            playerId = (int)(dbContext.player.AsNoTracking().Max(g => g.PlayerID));

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
                if (dbContext.player.AsNoTracking().Any())
                {
                    //playerId = (int)(dbContext.player.AsNoTracking().Max(g => g.PlayerID) + 1); //Generate Max GameID + 1
                    playerId = (int)(dbContext.player.AsNoTracking().Max(g => g.PlayerID) + 1); //Generate Max GameID + 1
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
                //var deb = dbContext.player.Add(player).DebugView;
                dbContext.player.Add(player);
                //logger.Info(deb.LongView);
            }
            return playerList;
        }
    }
}