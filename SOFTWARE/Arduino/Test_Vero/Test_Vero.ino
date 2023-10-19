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
#include "arduino.h"
#include "global.h"
#include "_cZordBase.h"
#include "project.h"

SerialCommandRead scr{ &Serial1, "@", "\r\n" };

Timer tmr_Send{};

D_In sw1{ 49, 0, 8, D_In::pullup::OFF };
D_Out l1{ A0, 0, D_Out::state::OFF };

void setup()
{
	Serial.begin(9600); // Pour le moniteur
	scr.begin(9600); // Pour l'envoie au PC
}

void loop()
{
	String dataReceived;
	SerialCommandRead::state stateData;
	stateData = scr.available();

	D_In::state state_Sw1 = sw1.read();

	if (stateData < SerialCommandRead::state::WAITING)
	{
		dataReceived = scr.read();
		Serial.println(dataReceived); // Envoie au moniteur ce qu'a re�u le Serial1
	}
	else if (stateData > SerialCommandRead::state::PROCESSING)
	{
		scr.restart(); // ingore les carct. re�u et recommence une nouvelle analyse pour la commande suivante
		scr.print("command error");
		Serial.println("command error");
	}

	if (state_Sw1 == D_In::state::ON) 
	{
		if (tmr_Send.get_ms() >= 500)
		{			
			l1.setToggle();
			scr.print("abcde");
			tmr_Send.restart();
		}		
	}
	else if (state_Sw1 == D_In::state::DOWN)
	{
		l1.setOff();
	}
}
