using System;
using System.Collections.Generic;
using System.Linq;
using wc3_fate_west_data_access_layer;
using wc3_fate_west_data_access_layer.Data;

namespace wc3_fate_west_web.Models
{
    public class GameSL
    {
        private readonly FateWestDbContext fateDbContext;

        public GameSL(FateWestDbContext fateDbContext)
        {
            this.fateDbContext = fateDbContext;
        }
        private IEnumerable<GameData> GetRecentGameDataListFromDb(int count)
        {
            var gameDataList = new List<GameData>();

            var recentGameQueryData = (
                    from game in fateDbContext.game
                    join server in fateDbContext.server on game.FK_ServerID equals server.ServerID
                    join gameDetail in fateDbContext.gameplayerdetail on game.GameID equals gameDetail.FK_GameID
                    group game by new
                    {
                        game.GameID,
                        game.GameName,
                        game.PlayedDate,
                        game.Duration,
                        game.Result,
                        game.MapVersion,
                        game.ReplayUrl,
                        game.MatchType,
                        server.ServerName,
                        game.TeamOneWinCount,
                        game.TeamTwoWinCount
                    } into g
                    orderby g.Key.GameID descending
                    select new
                    {
                        g.Key.GameID,
                        g.Key.GameName,
                        g.Key.Duration,
                        g.Key.MapVersion,
                        g.Key.MatchType,
                        g.Key.PlayedDate,
                        g.Key.ReplayUrl,
                        g.Key.Result,
                        g.Key.ServerName,
                        g.Key.TeamOneWinCount,
                        g.Key.TeamTwoWinCount,
                        PlayerCount = g.Count()
                    }).Take(count);

            foreach (var data in recentGameQueryData)
            {
                var gameData = new GameData
                {
                    GameID = (int)data.GameID,
                    GameName = data.GameName,
                    Duration = new TimeSpan(0, 0, 0, 0, data.Duration),
                    MapVersion = data.MapVersion,
                    MatchType = data.MatchType,
                    PlayedDate = data.PlayedDate,
                    ReplayUrl = data.ReplayUrl,
                    Result = data.Result,
                    ServerName = data.ServerName,
                    PlayerCount = data.PlayerCount,
                    TeamOneWinCount = data.TeamOneWinCount,
                    TeamTwoWinCount = data.TeamTwoWinCount
                };
                gameDataList.Add(gameData);
            }

            return gameDataList;
        }
        public IEnumerable<GameData> GetRecentGames(int count)
        {

            List<GameData> recentGameList = GetRecentGameDataListFromDb(count).ToList();
            foreach (GameData data in recentGameList)
            {
                if (data.TeamOneWinCount > data.TeamTwoWinCount)
                {
                    data.Result = $@"Team 1 Victory ({data.TeamOneWinCount}-{data.TeamTwoWinCount})";
                }
                else if (data.TeamOneWinCount < data.TeamTwoWinCount)
                {
                    data.Result = $@"Team 2 Victory ({data.TeamOneWinCount}-{data.TeamTwoWinCount})";
                }
                else
                {
                    data.Result = $@"Draw ({data.TeamOneWinCount}-{data.TeamTwoWinCount})";
                }
            }
            return recentGameList;
        }
        /// <summary>
        /// Retrieves the chat log from the game
        /// </summary>
        /// <param name="gameId">Game ID of the game</param>
        /// <returns>Log (string)</returns>
        public string GetGameLogFromDb(int gameId)
        {
            return fateDbContext.game.FirstOrDefault(x => x.GameID == gameId)?.Log;
        }
        public string GetGameLog(int gameId)
        {
            return GetGameLogFromDb(gameId);
        }
        private GameReplayData GetReplayDataFromDb(int gameId)
        {
            GameReplayData data = new GameReplayData();
            var game = fateDbContext.game.FirstOrDefault(x => x.GameID == gameId);
            if (game == null)
                return null;
            data.PlayedDateTime = game.PlayedDate;
            data.ReplayPath = game.ReplayUrl;
            return data;
        }
        public GameReplayData GetReplayData(int gameId)
        {
            GameReplayData data = GetReplayData(gameId);
            if (data == null)
                return null;
            if (string.IsNullOrEmpty(data.ReplayPath))
                return null;
            // TODO data.ReplayPath = Path.Combine(ConfigHandler.ReplayFileLocation, Path.GetFileName(data.ReplayPath));
            return data;
        }
    }
}