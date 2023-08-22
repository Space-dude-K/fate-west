#include "includes.h"
#include "lobby.h"
#include "util.h"
#include "gameplayer.h"
#include "gameslot.h"
#include "ghostdb.h"
#include <chrono>
#include <thread>
#include <iostream>
#include <ctime>

using namespace std::chrono_literals;

atomic_bool Lobby::cancelTokenForLobbyUpdater = ATOMIC_VAR_INIT(false);

string Lobby::GetDateForLobbyStatus()
{
	time_t rawtime;
	struct tm* timeinfo;
	char buffer[80];

	time(&rawtime);
	timeinfo = localtime(&rawtime);

	strftime(buffer, sizeof(buffer), "%d-%m-%Y %H:%M:%S", timeinfo);
	std::string str(buffer);

	return str;
}
void Lobby::UpdateGameStatusForLobby(uint32_t lobbyId, int status, CGHostDB* m_DB)
{
	try
	{
		CONSOLE_Print("[LOBBY EXPORT] Update game status for lobby: " + to_string(lobbyId) + " " + GetDateForLobbyStatus());
		m_DB->ThreadedLobbyStatusUpdate(lobbyId, status, GetDateForLobbyStatus());
	}
	catch (const std::exception& err)
	{
		string str = err.what();
		CONSOLE_Print("[LOBBY EXPORT] UpdateGameStatusForLobby error. " + str);
	}
}
void Lobby::InitLobby(uint32_t lobbyId, uint32_t gameCounter, string realm, string gameName, int gameStatus, int lobbyType, CGHostDB* m_DB)
{
	try
	{
		CONSOLE_Print("[LOBBY EXPORT] Init " + to_string(lobbyType) + " lobby: " + to_string(lobbyId) + " " + GetDateForLobbyStatus());

		// Private game = 17
		m_DB->ThreadedLobbyAdd(lobbyId, gameCounter, realm, gameName, gameStatus, GetDateForLobbyStatus(), lobbyType == 17 ? 1 : 0);
	}
	catch (const std::exception& err)
	{
		string str = err.what();
		CONSOLE_Print("[LOBBY EXPORT] InitLobby error. " + str);
	}
}
void Lobby::InitSlotsForLobby(uint32_t lobbyId, std::vector<CGameSlot> m_Slots, CGHostDB* m_DB)
{
	try
	{
		CONSOLE_Print("[LOBBY EXPORT] Init slots for lobby: " + to_string(lobbyId));

		for (int i = 0; i < m_Slots.size(); i++)
		{
			m_DB->ThreadedLobbySlotsInit(lobbyId, i, 0, static_cast<int>(m_Slots[i].GetTeam()), "Open", i, 0);
		}
	}
	catch (const std::exception& err)
	{
		string str = err.what();
		CONSOLE_Print("[LOBBY EXPORT] InitSlotsForLobby error. " + str);
	}
}
void Lobby::UpdateAllSlots(uint32_t lobbyId, std::vector<CGameSlot> m_Slots, std::vector<CGamePlayer*> m_Players, CGHostDB* m_DB)
{
	try
	{
		std::thread::id threadID = std::this_thread::get_id();
		stringstream ss;
		ss << threadID;
		string mystring = ss.str();

		//CONSOLE_Print("[LOBBY EXPORT] - Update slot data for lobby: " + to_string(lobbyId) + " from thread: " + mystring);

		int sId = 255;
		int pID = 255;
		int sTeam = 255;
		string pName = "Default name";
		int sColour = 255;
		int pPing = 255;

		for (int i = 0; i < m_Slots.size(); i++)
		{
			sId = i;
			// 0 - for non-human, 1 - reserved
			pID = m_Slots[i].GetPID();
			sTeam = m_Slots[i].GetTeam();
			pName = pID > 1 && pID != 255 ? m_Players[pID - 2]->GetName()
				: m_Slots[i].GetSlotStatus() == SLOTSTATUS_CLOSED ? "Closed"
				: m_Slots[i].GetComputer() == 1 ? GetNameForNonHumanSlot(m_Slots[i]) : "Open";
			sColour = m_Slots[i].GetColour();
			pPing = pID > 1 && pID != 255 ? m_Players[pID - 2]->GetPing(false) : 0;

			//CONSOLE_Print("PID: " + to_string(m_Slots[i].GetPID()) + " for slot ID: " + to_string(sId) + " name: " + pName + " is comp: " + to_string(m_Slots[i].GetComputer()));

			m_DB->ThreadedLobbySlotUpdate(lobbyId, sId, pID, sTeam, pName, sColour, pPing);
		}
	}
	catch (const std::exception& err)
	{
		string str = err.what();
		CONSOLE_Print("[LOBBY EXPORT] UpdateAllSlots error. " + str);
	}
}
string Lobby::GetNameForNonHumanSlot(CGameSlot m_Slot)
{
	string name = "Open";

	if (m_Slot.GetComputerType() == 0)
	{
		name = "Comp Easy";
	}
	else if (m_Slot.GetComputerType() == 1)
	{
		name = "Comp Normal";
	}
	else
	{
		name = "Comp Hard";
	}
	
	return name;
}
void Lobby::DeleteAllLobbies(CGHostDB* m_DB)
{
	try
	{
		CONSOLE_Print("[LOBBY EXPORT] Deleting all lobbies.");
		m_DB->ThreadedLobbyDeleteAllLobbies();
	}
	catch (const std::exception& err)
	{
		string str = err.what();
		CONSOLE_Print("[LOBBY EXPORT] DeleteAllLobbies error. " + str);
	}
}
void Lobby::DeleteAllSlots(CGHostDB* m_DB)
{
	try
	{
		CONSOLE_Print("[LOBBY EXPORT] Deleting all slots.");
		m_DB->ThreadedLobbyDeleteAllSlots();
	}
	catch (const std::exception& err)
	{
		string str = err.what();
		CONSOLE_Print("[LOBBY EXPORT] DeleteAllSlots error. " + str);
	}
}
void Lobby::RunLobbyUpdater(CGHost* ghost)
{
	try
	{
		CONSOLE_Print("[LOBBY EXPORT] Starting lobby updater with " + to_string(ghost->m_lobbyExportDelay) + " delay.");

		while (true)
		{
			if (cancelTokenForLobbyUpdater)
			{
				CONSOLE_Print("[LOBBY EXPORT] Closing DB writer for lobby updater.");
				break;
			}

			if (ghost->m_CurrentGame)
			{
				UpdateAllSlots(ghost->m_CurrentGame->m_RandomSeed, ghost->m_CurrentGame->m_Slots, ghost->m_CurrentGame->m_Players, ghost->m_CurrentGame->m_GHost->m_DB);
			}
			else
			{
				CONSOLE_Print("[LOBBY EXPORT] Nothing to update.");
			}

			std::this_thread::sleep_for(std::chrono::seconds(ghost->m_lobbyExportDelay));
		}
	}
	catch (const std::exception& err)
	{
		string str = err.what();
		CONSOLE_Print("[LOBBY EXPORT] RunLobbyUpdater error. " + str);
	}
};
void Lobby::InitCancelTokenForLobbyUpdater()
{
	try
	{
		CONSOLE_Print("[LOBBY EXPORT] Init cancel token for lobby updater.");
		cancelTokenForLobbyUpdater = false;
	}
	catch (const std::exception& err)
	{
		string str = err.what();
		CONSOLE_Print("[LOBBY EXPORT] InitCancelTokenForLobbyUpdater error. " + str);
	}
};
void Lobby::RequestCancelForLobbyUpdater()
{
	try
	{
		CONSOLE_Print("[LOBBY EXPORT] Requesting cancelation lobby updater.");
		cancelTokenForLobbyUpdater = true;
	}
	catch (const std::exception& err)
	{
		string str = err.what();
		CONSOLE_Print("[LOBBY EXPORT] RequestCancelForLobbyUpdater error. " + str);
	}
};