// ---------------------------------------------------------------------------------------------
// cZarduiBase v4e
// ---------------------------------------------------------------------------------------------
// Projet de base pour l'utilisation des classes :
// - D_Out              : Gestion de sorties digitales
// - D_In               : Gestion d'entr�es digitales avec anti-rebonds
// - Timer              : Gestion de d�lais non-bloquants
// - StateMachine       : Gestion de machine d'�tat
// - SerialCommandRead  : Gestion du mode "COMMAND" pour la r�ception d'un port s�rie
//                        (version b�ta, non compatible avec cZarduiBase pr�c�dents)
// 
// �cZord 2019-2022
// ---------------------------------------------------------------------------------------------
#include "command.h"
#include "pix2Cam.h"
#include "arduino.h"
#include "global.h"
#include "_cZordBase.h"
#include "input_output.h"

void setup()
{
	DEBUG_BEGIN(115200);
	scr.begin(9600);

	Pixy_Init();
}

void loop()
{
	String dataReceived;
	SerialCommandRead::state stateData;
	Command command;
	stateData = scr.available();

	StateD_In(); // Lis tout les �tats des boutons instanci�s

	// Grab blocks
	Scan_Blocks();

	// Signale si le train est arriv�
	static bool ledOn = false;

	// si bouton enclench�
	if (IsOn(SW_TRAIN))	// SW_TRAIN : bouton d�di�, 
	{					// � la simulation du train
		if (!ledOn)
		{
			SetOn(L1);
			ledOn = true;
		}
	}
	else if (IsOff(SW_TRAIN)) // si bouton d�clench�
	{
		if (ledOn)
		{
			SetOff(L1);
			ledOn = false;
		}
	}

	if (stateData < SerialCommandRead::state::WAITING)
	{
		dataReceived = scr.read();

		// analyse et traitement de la commande
		command = AnalysisCommand(dataReceived);
		processingCommand(command, dataReceived);

	}
	else if (stateData > SerialCommandRead::state::PROCESSING)
	{
		scr.restart();	// vide le buffer et d�marre l'analyse de la commande suivante
		scr.print("commande error processing");
		DEBUG_PRINTLN(COMMAND_ERROR_PROCESSING);
	}
}