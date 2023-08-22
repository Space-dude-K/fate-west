#ifndef __DISCORD_H__
#define __DISCORD_H__

#include "includes.h"
#include "util.h"
#include "gameplayer.h"

using namespace std;

class Lobby
{
private:
    static atomic_bool cancelTokenForLobbyUpdater;
    static string GetNameForNonHumanSlot(CGameSlot m_Slot);
public:
    static void RunLobbyUpdater(CGHost* ghost);
    static void InitCancelTokenForLobbyUpdater();
    static void RequestCancelForLobbyUpdater();
    static string GetDateForLobbyStatus();
    static void UpdateGameStatusForLobby(uint32_t lobbyId, int status, CGHostDB* m_DB);
    static void InitLobby(uint32_t lobbyId, uint32_t gameCounter, string realm, string gameName, int gameStatus, int lobbyType, CGHostDB* m_DB);
    static void InitSlotsForLobby(uint32_t lobbyId, std::vector<CGameSlot> m_Slots, CGHostDB* m_DB);
    static void UpdateAllSlots(uint32_t lobbyId, std::vector<CGameSlot> m_Slots, std::vector<CGamePlayer*> m_Players, CGHostDB* m_DB);
    static void DeleteAllLobbies(CGHostDB* m_DB);
    static void DeleteAllSlots(CGHostDB* m_DB);
};

#endif