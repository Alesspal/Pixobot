#include "_cZordBase.h"
#include "input_output.h"

D_In sw1{ 49, 0, 8,D_In::pullup::OFF };
D_In* tabInp[] = { &sw1}; // mettre chaque bouton dans le tableau, dans le même ordre de l'enum
D_In::state tabState[nbOfBtn];

D_Out l1{ A0, 0, D_Out::state::OFF };
D_Out* tabOut[] = { &l1 }; // mettre chaque bouton dans le tableau, dans le même ordre de l'enum

/// <summary>
/// Mets les états de tout les boutons dans un tableau.
/// Lire cette fonstion à chaque entré du loop
/// </summary>
void StateD_In()
{
	for (int i = 0; i < nbOfBtn; i++)
	{
		tabState[i] = tabInp[i]->read();
	}
}

/// <summary>
/// Contrôl si le bouton est en flanc montant
/// </summary>
/// <param name="eD_inp"></param>
/// <returns> true : si l'état de l'objet est en flanc montant (UP) </returns>
bool IsUp(DigitalInputEnum eD_Inp)
{
	return tabState[(int)eD_Inp] == D_In::state::UP;
}

/// <summary>
/// Contrôl si le bouton est en flanc actif
/// </summary>
/// <param name="eD_inp"></param>
/// <returns> true : si l'état de l'objet est en flanc actif (ON) </returns>
bool IsOn(DigitalInputEnum eD_Inp)
{
	return tabState[(int)eD_Inp] == D_In::state::ON;
}

/// <summary>
/// Contrôl si le bouton est en flanc descendant
/// </summary>
/// <param name="eD_inp"></param>
/// <returns> true : si l'état de l'objet est en flanc descendant (DOWN) </returns>
bool IsDown(DigitalInputEnum eD_Inp)
{
	return tabState[(int)eD_Inp] == D_In::state::DOWN;
}

/// <summary>
/// Contrôl si le bouton est en flanc inactif
/// </summary>
/// <param name="eD_inp"></param>
/// <returns> true : si l'état de l'objet est en flanc inactif (OFF) </returns>
bool IsOff(DigitalInputEnum eD_Inp)
{
	return tabState[(int)eD_Inp] == D_In::state::OFF;
}

/// <summary>
/// Allume la led mise en paramètre
/// </summary>
/// <param name="eD_Out"></param>
void SetOn(DigitalOutputEnum eD_Out)
{
	tabOut[(int)eD_Out]->setOn();
}

/// <summary>
/// Permute la led mise en paramètre
/// </summary>
/// <param name="eD_Out"></param>
void SetOff(DigitalOutputEnum eD_Out)
{
	tabOut[(int)eD_Out]->setOff();
}

/// <summary>
/// Peermute la led mise en paramètre
/// </summary>
/// <param name="eD_Out"></param>
void SetToggle(DigitalOutputEnum eD_Out)
{
	tabOut[(int)eD_Out]->setToggle();
}