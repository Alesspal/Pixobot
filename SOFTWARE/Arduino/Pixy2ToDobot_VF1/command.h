// command.h

#ifndef _COMMAND_h
#define _COMMAND_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

// ----------------------------------------------------------------------------
// Modifier ou compléter, si nécéssaire, le code ci-dessous
// ----------------------------------------------------------------------------

// #include du projet ...
// ----------------------------------------------------------------------------
#include "arduino.h"
#include "_cZordBase.h"
#include "global.h"
#include "pix2Cam.h"


// Définitions diverses utilisées dans plusieurs fichiers ...
// define, extern, enum, etc.
// ----------------------------------------------------------------------------

// Pour les commandes

const char GET = 'g';
const char BLOCK = 'b';
const char NUMBER_OF = 'n';
const char STATE_OF_TRAIN = 't';
const char ALL = 'a';
const String NON_EXISTENT_BLOCK = "ebi";
const String NO_BLOCK_DETECTED = "enb0";
const int ARRIVED = 1;
const int NOT_ARRIVED = 0;
const String INCOMPREHENSIBLE_COMMAND = "command error";
const String COMMAND_ERROR_PROCESSING = "commande error processing";

enum Command
{
	NUMBER_OF_BLOCK_DETECTED,
	ZERO_BLOCK_DETECTED,
	COORD_OF_BLOCK,
	REQUESTED_BLOCK_DOES_NOT_EXIST,
	COORD_OF_ALL_BLOCKS,
	TRAIN_ARE_ARRIVED,
	TRAIN_ARE_NOT_ARRIVED,
	COMMAND_ERROR
};

extern SerialCommandRead scr;

// Prototypes des méthodes (de projet.cpp)
// ----------------------------------------------------------------------------

Command AnalysisCommand(String dataReceived);
void processingCommand(Command command, String dataReceived);

#endif

