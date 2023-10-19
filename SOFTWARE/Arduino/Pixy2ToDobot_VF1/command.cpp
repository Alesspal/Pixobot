#include "command.h"
#include "input_output.h"

SerialCommandRead scr{ &Serial1, "@", "\r\n" };

/// <summary>
/// Analyse et traite la commande
/// </summary>
/// <param name="dataReceived"></param>
Command AnalysisCommand(String dataReceived)
{
	bool digitValidated;
	int numBlock;
	String tmp;

	if (dataReceived[0] == GET)
	{
		DEBUG_PRINTLN(GET);
		if (dataReceived.length() > 2)
		{
			if (dataReceived[1] == BLOCK)
			{
				DEBUG_PRINTLN(BLOCK);
				if (NumberOfBlocksDetected() > 0)
				{
					if (dataReceived[2] == ALL)
					{
						DEBUG_PRINTLN(ALL);
						return COORD_OF_ALL_BLOCKS;
					}
					else
					{
						digitValidated = true;
						tmp = dataReceived.substring(2);
						DEBUG_PRINTLN(tmp);
						for (int i = 0; i < tmp.length(); i++) // Contrôle si tous les car. peuvent se convertir en chiffre
						{
							if (!isDigit(tmp[i]))
							{
								digitValidated = false;
								i = tmp.length(); // sort de la boucle for
							}
						}

						if (digitValidated)
						{
							numBlock = tmp.toInt();
							if (numBlock >= 0 && numBlock < NumberOfBlocksDetected()) // de 0 au nombre de bloc - 1 car le premier bloc est 0
							{
								return COORD_OF_BLOCK;
							}
							else
							{
								return REQUESTED_BLOCK_DOES_NOT_EXIST;
							}
						}
					}
				}
				else
				{
					DEBUG_PRINTLN(NO_BLOCK_DETECTED);
					return ZERO_BLOCK_DETECTED;
				}
			}
			else if (dataReceived[1] == NUMBER_OF)
			{
				DEBUG_PRINTLN(NUMBER_OF);
				if (dataReceived.length() == 3)
				{
					if (dataReceived[2] == BLOCK)
					{
						DEBUG_PRINTLN(BLOCK);
						return NUMBER_OF_BLOCK_DETECTED;
					}
				}
			}
		}
		else
		{
			if (dataReceived[1] == STATE_OF_TRAIN)
			{
				DEBUG_PRINTLN(STATE_OF_TRAIN);
				if (IsOn(SW_TRAIN) || IsUp(SW_TRAIN))
				{
					return TRAIN_ARE_ARRIVED;
				}
				else
				{
					return TRAIN_ARE_NOT_ARRIVED;
				}
			}
		}
	}
	return COMMAND_ERROR;
}

void processingCommand(Command command, String dataReceived)
{
	int numBlock;
	switch (command)
	{
	case NUMBER_OF_BLOCK_DETECTED:
		DEBUG_PRINTLN(NumberOfBlocksDetected());
		scr.print("nb" + (String)NumberOfBlocksDetected());
		break;

	case ZERO_BLOCK_DETECTED:
		DEBUG_PRINTLN("enb0");
		scr.print("enb0");
		break;

	case COORD_OF_BLOCK:
		numBlock = dataReceived.substring(2).toInt();
		DEBUG_PRINTLN(ValuesOfBlock(numBlock));
		scr.print(ValuesOfBlock(numBlock));
		break;

	case REQUESTED_BLOCK_DOES_NOT_EXIST:
		DEBUG_PRINTLN("ebi");
		scr.print("ebi");
		break;

	case COORD_OF_ALL_BLOCKS:
		for (int i = 0; i < NumberOfBlocksDetected(); i++) // Envoie tout les objets detectés
		{
			DEBUG_PRINTLN(ValuesOfBlock(i));
			scr.print(ValuesOfBlock(i));
		}
		break;

	case TRAIN_ARE_ARRIVED:
		DEBUG_PRINTLN("t1");
		scr.print("t1"); // t : train, 1 : arrivé
		break;

	case TRAIN_ARE_NOT_ARRIVED:
		DEBUG_PRINTLN("t0");
		scr.print("t0"); // t : train, 0 : pas encore arrivé
		break;

	case COMMAND_ERROR:
	default:
		DEBUG_PRINTLN("command error");
		scr.print("command error");
		break;
	}
}


