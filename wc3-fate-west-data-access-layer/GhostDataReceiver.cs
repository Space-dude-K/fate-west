using NLog;
using System;
using System.Collections.Generic;
using wc3_fate_west_data_access_layer.Db;
using System.Linq;
using wc3_fate_west_data_access_layer.Data.Wc3_Ghost;
using Microsoft.EntityFrameworkCore;

namespace wc3_fate_west_data_access_layer
{
    public class GhostDataReceiver : IGhostDataReceiver
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly GhostDbContext ghostDbContext;

        public GhostDataReceiver(GhostDbContext ghostDbContext)
        {
            this.ghostDbContext = ghostDbContext;
        }

        public List<Lobby> GetLobbiesFromDb()
        {
            List<Lobby> lobbyList = new List<Lobby>();

            try
            {
                var ghostLobbies = ghostDbContext.lobbies.AsNoTracking().Where(l => l.lobbyStatus != (long)Lobby.GameStatus.Processed);

                logger.Info("Lobby count: " + ghostLobbies.Count());

                foreach (var lob in ghostLobbies)
                {
                    Lobby lobby = new Lobby
                    {
                        LobbyId = (int)lob.lobbyId,
                        GameCounter = lob.gameCounter.ToString(),
                        Realm = lob.realm,
                        LobbyName = lob.lobbyName,
                        SetGameStatus = lob.lobbyStatus.ToString(),
                        SetLobbyDateTime = lob.lobbyDateTime,
                        SetLobbyType = lob.lobbyType.ToString()
                    };

                    logger.Info("Lobby ID: " + lobby.LobbyId);

                    if (lobby.LobbyId != 0)
                    {
                        lobby.LobbySlots = GetSlotsFromDb(lobby.LobbyId);
                    }

                    lobbyList.Add(lobby);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return lobbyList;
        }
        private List<Slot> GetSlotsFromDb(int lobbyId)
        {
            List<Slot> slots = new List<Slot>();

            var dbSlots = ghostDbContext.slots.AsNoTracking().AsQueryable().Where(s => s.lobbyId == lobbyId);

            foreach (var dbSlot in dbSlots)
            {
                Slot slot = new Slot()
                {
                    SlotId = dbSlot.slotId.ToString(),
                    PlayerId = (int)dbSlot.playerId,
                    PlayerTeam = (int)dbSlot.playerTeam,
                    PlayerName = dbSlot.playerName,
                    SetPlayerColor = dbSlot.playerColour.ToString(),
                    PlayerPing = dbSlot.playerPing.ToString()
                };

                bool isPlayerStatsReal = IsPlayerStatsExist(slot.PlayerName);

                if (slot.IsHuman && !string.IsNullOrWhiteSpace(slot.PlayerName) && isPlayerStatsReal)
                {
                    slot.PlayerStats = GetGhostStatsForPlayer(slot.PlayerName);
                }
                else if (slot.IsHuman && string.IsNullOrWhiteSpace(slot.PlayerName))
                {
                    logger.Error("Unable get stats for empty player.");
                }
                else
                {
                    slot.PlayerStats = new Stats(false);
                }

                slots.Add(slot);
            }

            return slots;
        }
        public Stats GetGhostStatsForPlayer(string playerName)
        {
            // TODO: Rewrite with proper linq (GroupJoin?)
            /*
            var query = from gp in ghostDbContext.gameplayers.AsNoTracking().AsQueryable()
                        join g in ghostDbContext.games.AsNoTracking().AsQueryable() on gp.gameid equals g.id into ggp
                        from y1 in ggp.DefaultIfEmpty()
                        join gp1 in ghostDbContext.gameplayers.AsNoTracking().AsQueryable() on y1.id equals gp1.gameid into ggp1
                        where gp.name == playerName
                        select new Stats
                        {
                            FirstGameTime = ggp.Min(fgt => fgt.datetime),
                            LastGameTime = ggp.Max(fgt => fgt.datetime),
                            TotalGames = ggp.Count().ToString(),
                            //MinDT = ggp.Min(dt => dt.datetime),
                            //MaxDT = ggp.Max(dt => dt.datetime),
                            MinLoadingTime = ggp1.Min(lt => lt.loadingtime).ToString(),
                            AvgLoadingTime = ggp1.Average(lt => lt.loadingtime).ToString(),
                            MaxLoadingTime = ggp1.Max(lt => lt.loadingtime).ToString(),
                            MinStayPercent = ((ggp1.Min(l => l.left) / ggp.Min(sp => sp.duration)) * 100).ToString(),
                            AvgStayPercent = ((ggp1.Average(l => l.left) / ggp.Average(sp => sp.duration)) * 100).ToString(),
                            MaxStayPercent = ((ggp1.Max(l => l.left) / ggp.Max(sp => sp.duration)) * 100).ToString(),
                            MinDurationTime = ggp.Min(d => d.duration).ToString(),
                            AvgDurationTime = ggp.Average(d => d.duration).ToString(),
                            MaxDurationTime = ggp.Max(d => d.duration).ToString(),
                        };
            */

            Stats ps = new Stats(IsPlayerStatsExist(playerName));

            try
            {
                var conn = ghostDbContext.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();

                command.CommandText =
                    $@"SELECT MIN(datetime), MAX(datetime), COUNT(*), MIN(loadingtime), AVG(loadingtime), MAX(loadingtime), MIN(left/CAST(duration AS REAL))*100, AVG(left/CAST(duration AS REAL))*100, MAX(left/CAST(duration AS REAL))*100, MIN(duration), AVG(duration), MAX(duration) FROM gameplayers LEFT JOIN games ON games.id=gameid WHERE name='{playerName.ToLower() + "'"}";

                var r = command.ExecuteReader();

                while (r.Read())
                {
                    ps.FirstGameTime = Convert.ToString(r["MIN(datetime)"]);
                    ps.LastGameTime = Convert.ToString(r["MAX(datetime)"]);

                    ps.TotalGames = Convert.ToString(r["COUNT(*)"]);

                    ps.MinLoadingTime = Convert.ToString(r["MIN(loadingtime)"]);
                    ps.AvgLoadingTime = Convert.ToString(r["AVG(loadingtime)"]);
                    ps.MaxLoadingTime = Convert.ToString(r["MAX(loadingtime)"]);

                    ps.MinStayPercent = Convert.ToString(r["MIN(left/CAST(duration AS REAL))*100"]);
                    ps.AvgStayPercent = Convert.ToString(r["AVG(left/CAST(duration AS REAL))*100"]);
                    ps.MaxStayPercent = Convert.ToString(r["MAX(left/CAST(duration AS REAL))*100"]);

                    ps.MinDurationTime = Convert.ToString(r["MIN(duration)"]);
                    ps.AvgDurationTime = Convert.ToString(r["AVG(duration)"]);
                    ps.MaxDurationTime = Convert.ToString(r["MAX(duration)"]);
                }

                if (ps == null)
                    throw new Exception(
                            String.Format("ghostDbContext Error: Empty stats for {0}",
                                          playerName));

                logger.Info(ps.FirstGameTime);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return ps;
        }
        private bool IsPlayerStatsExist(string playerName)
        {
            return ghostDbContext.gameplayers.AsNoTracking().AsQueryable().Where(p => p.name.Equals(playerName.ToLower())).Count() > 0;
        }
        public void ChangeGameStatusForLobby(int lobbyId, Lobby.GameStatus gs, bool deleteSlots)
        {
            using (var trans = ghostDbContext.Database.BeginTransaction())
            {
                try
                {
                    if (!ghostDbContext.lobbies.AsNoTracking().AsQueryable().Any(l => l.lobbyId == (long)lobbyId))
                        throw new Exception("[DAL] Unable change lobby status for " + lobbyId + " Lobby not found.");

                    logger.Info("[DAL] Changing status for lobby " + lobbyId + " to " + gs.ToString());

                    ghostDbContext.lobbies.Where(l => l.lobbyId == (long)lobbyId).FirstOrDefault().lobbyStatus = (long)gs;

                    ghostDbContext.SaveChanges();
                    trans.Commit();

                    if(deleteSlots)
                        RemoveSlotsFromDb(lobbyId);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    trans.Rollback();
                }
            }
        }
        private void RemoveSlotsFromDb(int lobbyId)
        {
            using (var trans = ghostDbContext.Database.BeginTransaction())
            {
                try
                {
                    if (!ghostDbContext.lobbies.AsNoTracking().AsQueryable().Any(l => l.lobbyId == (long)lobbyId))
                        throw new Exception("[DAL] Unable delete slots for " + lobbyId + " Slots not found.");

                    logger.Info("[DAL] Deleting sltos for lobby " + lobbyId);

                    ghostDbContext.RemoveRange(ghostDbContext.slots.Where(s => s.lobbyId == (long)lobbyId));

                    ghostDbContext.SaveChanges();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    trans.Rollback();
                }
            }
        }
    }
    public interface IGhostDataReceiver
    {
        void ChangeGameStatusForLobby(int lobbyId, Lobby.GameStatus gs, bool deleteSlots);
        Stats GetGhostStatsForPlayer(string playerName);
        List<Lobby> GetLobbiesFromDb();
    }
}