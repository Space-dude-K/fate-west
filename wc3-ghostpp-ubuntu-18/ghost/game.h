/*

   Copyright [2008] [Trevor Hogan]

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   CODE PORTED FROM THE ORIGINAL GHOST PROJECT: http://ghost.pwner.org/

*/

#ifndef GAME_H
#define GAME_H

//
// CGame
//

class CDBBan;
class CDBGame;
class CDBGamePlayer;
class CStats;
class CCallableBanCheck;
class CCallableBanAdd;
class CCallableGameAdd;
class CCallableGamePlayerSummaryCheck;
class CCallableDotAPlayerSummaryCheck;

typedef pair<string,CCallableBanCheck *> PairedBanCheck;
typedef pair<string,CCallableBanAdd *> PairedBanAdd;
typedef pair<string,CCallableGamePlayerSummaryCheck *> PairedGPSCheck;
typedef pair<string,CCallableDotAPlayerSummaryCheck *> PairedDPSCheck;

class CGame : public CBaseGame
{
protected:
	CDBBan *m_DBBanLast;						// last ban for the !banlast command - this is a pointer to one of the items in m_DBBans
	vector<CDBBan *> m_DBBans;					// vector of potential ban data for the database (see the Update function for more info, it's not as straightforward as you might think)
	CDBGame *m_DBGame;							// potential game data for the database
	vector<CDBGamePlayer *> m_DBGamePlayers;	// vector of potential gameplayer data for the database
	CStats *m_Stats;							// class to keep track of game stats such as kills/deaths/assists in dota
	CCallableGameAdd *m_CallableGameAdd;		// threaded database game addition in progress
	vector<PairedBanCheck> m_PairedBanChecks;	// vector of paired threaded database ban checks in progress
	vector<PairedBanAdd> m_PairedBanAdds;		// vector of paired threaded database ban adds in progress
	vector<PairedGPSCheck> m_PairedGPSChecks;	// vector of paired threaded database game player summary checks in progress
	vector<PairedDPSCheck> m_PairedDPSChecks;	// vector of paired threaded database DotA player summary checks in progress

public:
	CGame( CGHost *nGHost, CMap *nMap, CSaveGame *nSaveGame, uint16_t nHostPort, unsigned char nGameState, string nGameName, string nOwnerName, string nCreatorName, string nCreatorServer );
	virtual ~CGame( );

	virtual bool Update(void* fd, void* send_fd)
	{
		// update callables

		for (vector<PairedBanCheck> ::iterator i = m_PairedBanChecks.begin(); i != m_PairedBanChecks.end(); )
		{
			if (i->second->GetReady())
			{
				CDBBan* Ban = i->second->GetResult();

				if (Ban)
					SendAllChat(m_GHost->m_Language->UserWasBannedOnByBecause(i->second->GetServer(), i->second->GetUser(), Ban->GetDate(), Ban->GetAdmin(), Ban->GetReason()));
				else
					SendAllChat(m_GHost->m_Language->UserIsNotBanned(i->second->GetServer(), i->second->GetUser()));

				m_GHost->m_DB->RecoverCallable(i->second);
				delete i->second;
				i = m_PairedBanChecks.erase(i);
			}
			else
				++i;
		}

		for (vector<PairedBanAdd> ::iterator i = m_PairedBanAdds.begin(); i != m_PairedBanAdds.end(); )
		{
			if (i->second->GetReady())
			{
				if (i->second->GetResult())
				{
					for (vector<CBNET*> ::iterator j = m_GHost->m_BNETs.begin(); j != m_GHost->m_BNETs.end(); ++j)
					{
						if ((*j)->GetServer() == i->second->GetServer())
							(*j)->AddBan(i->second->GetUser(), i->second->GetIP(), i->second->GetGameName(), i->second->GetAdmin(), i->second->GetReason());
					}

					SendAllChat(m_GHost->m_Language->PlayerWasBannedByPlayer(i->second->GetServer(), i->second->GetUser(), i->first));
				}

				m_GHost->m_DB->RecoverCallable(i->second);
				delete i->second;
				i = m_PairedBanAdds.erase(i);
			}
			else
				++i;
		}

		for (vector<PairedGPSCheck> ::iterator i = m_PairedGPSChecks.begin(); i != m_PairedGPSChecks.end(); )
		{
			if (i->second->GetReady())
			{
				CDBGamePlayerSummary* GamePlayerSummary = i->second->GetResult();

				if (GamePlayerSummary)
				{
					if (i->first.empty())
						SendAllChat(m_GHost->m_Language->HasPlayedGamesWithThisBot(i->second->GetName(), GamePlayerSummary->GetFirstGameDateTime(), GamePlayerSummary->GetLastGameDateTime(), UTIL_ToString(GamePlayerSummary->GetTotalGames()), UTIL_ToString((float)GamePlayerSummary->GetAvgLoadingTime() / 1000, 2), UTIL_ToString(GamePlayerSummary->GetAvgLeftPercent())));
					else
					{
						CGamePlayer* Player = GetPlayerFromName(i->first, true);

						if (Player)
							SendChat(Player, m_GHost->m_Language->HasPlayedGamesWithThisBot(i->second->GetName(), GamePlayerSummary->GetFirstGameDateTime(), GamePlayerSummary->GetLastGameDateTime(), UTIL_ToString(GamePlayerSummary->GetTotalGames()), UTIL_ToString((float)GamePlayerSummary->GetAvgLoadingTime() / 1000, 2), UTIL_ToString(GamePlayerSummary->GetAvgLeftPercent())));
					}
				}
				else
				{
					if (i->first.empty())
						SendAllChat(m_GHost->m_Language->HasntPlayedGamesWithThisBot(i->second->GetName()));
					else
					{
						CGamePlayer* Player = GetPlayerFromName(i->first, true);

						if (Player)
							SendChat(Player, m_GHost->m_Language->HasntPlayedGamesWithThisBot(i->second->GetName()));
					}
				}

				m_GHost->m_DB->RecoverCallable(i->second);
				delete i->second;
				i = m_PairedGPSChecks.erase(i);
			}
			else
				++i;
		}

		for (vector<PairedDPSCheck> ::iterator i = m_PairedDPSChecks.begin(); i != m_PairedDPSChecks.end(); )
		{
			if (i->second->GetReady())
			{
				CDBDotAPlayerSummary* DotAPlayerSummary = i->second->GetResult();

				if (DotAPlayerSummary)
				{
					string Summary = m_GHost->m_Language->HasPlayedDotAGamesWithThisBot(i->second->GetName(),
						UTIL_ToString(DotAPlayerSummary->GetTotalGames()),
						UTIL_ToString(DotAPlayerSummary->GetTotalWins()),
						UTIL_ToString(DotAPlayerSummary->GetTotalLosses()),
						UTIL_ToString(DotAPlayerSummary->GetTotalKills()),
						UTIL_ToString(DotAPlayerSummary->GetTotalDeaths()),
						UTIL_ToString(DotAPlayerSummary->GetTotalCreepKills()),
						UTIL_ToString(DotAPlayerSummary->GetTotalCreepDenies()),
						UTIL_ToString(DotAPlayerSummary->GetTotalAssists()),
						UTIL_ToString(DotAPlayerSummary->GetTotalNeutralKills()),
						UTIL_ToString(DotAPlayerSummary->GetTotalTowerKills()),
						UTIL_ToString(DotAPlayerSummary->GetTotalRaxKills()),
						UTIL_ToString(DotAPlayerSummary->GetTotalCourierKills()),
						UTIL_ToString(DotAPlayerSummary->GetAvgKills(), 2),
						UTIL_ToString(DotAPlayerSummary->GetAvgDeaths(), 2),
						UTIL_ToString(DotAPlayerSummary->GetAvgCreepKills(), 2),
						UTIL_ToString(DotAPlayerSummary->GetAvgCreepDenies(), 2),
						UTIL_ToString(DotAPlayerSummary->GetAvgAssists(), 2),
						UTIL_ToString(DotAPlayerSummary->GetAvgNeutralKills(), 2),
						UTIL_ToString(DotAPlayerSummary->GetAvgTowerKills(), 2),
						UTIL_ToString(DotAPlayerSummary->GetAvgRaxKills(), 2),
						UTIL_ToString(DotAPlayerSummary->GetAvgCourierKills(), 2));

					if (i->first.empty())
						SendAllChat(Summary);
					else
					{
						CGamePlayer* Player = GetPlayerFromName(i->first, true);

						if (Player)
							SendChat(Player, Summary);
					}
				}
				else
				{
					if (i->first.empty())
						SendAllChat(m_GHost->m_Language->HasntPlayedDotAGamesWithThisBot(i->second->GetName()));
					else
					{
						CGamePlayer* Player = GetPlayerFromName(i->first, true);

						if (Player)
							SendChat(Player, m_GHost->m_Language->HasntPlayedDotAGamesWithThisBot(i->second->GetName()));
					}
				}

				m_GHost->m_DB->RecoverCallable(i->second);
				delete i->second;
				i = m_PairedDPSChecks.erase(i);
			}
			else
				++i;
		}

		return CBaseGame::Update(fd, send_fd);
	}
	virtual void EventPlayerDeleted( CGamePlayer *player );
	virtual bool EventPlayerAction( CGamePlayer *player, CIncomingAction *action );
	virtual bool EventPlayerBotCommand( CGamePlayer *player, string command, string payload );
	virtual void EventGameStarted( );
	virtual bool IsGameDataSaved( );
	virtual void SaveGameData( );
};

#endif
