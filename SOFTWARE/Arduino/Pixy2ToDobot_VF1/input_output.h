#ifndef _INPUT_OUTPUT_h
#define _INPUT_OUTPUT_h

// ----------------------------------------------------------------------------
// Modifier ou compl�ter, si n�c�ssaire, le code ci-dessous
// ----------------------------------------------------------------------------

// #include du projet ...
// ----------------------------------------------------------------------------
#include "arduino.h"
#include "_cZordBase.h"
#include "global.h"
#include "pix2Cam.h"

// D�finitions diverses utilis�es dans plusieurs fichiers ...
// define, extern, enum, etc.
// ----------------------------------------------------------------------------

const int nbOfBtn = 1;	// Donner le nombre des entr�es
typedef enum
{
	SW_TRAIN,
}
DigitalInputEnum;		// Mettre le nom des entr�es

// Output
typedef enum
{
	L1,
}
DigitalOutputEnum;		// Mettre le nom des sorties

// Prototypes des m�thodes (de projet.cpp)
// ----------------------------------------------------------------------------

void StateD_In();
bool IsUp(DigitalInputEnum);
bool IsOn(DigitalInputEnum);
bool IsDown(DigitalInputEnum);
bool IsOff(DigitalInputEnum);

void SetOn(DigitalOutputEnum eD_Out);
void SetOff(DigitalOutputEnum eD_Out);
void SetToggle(DigitalOutputEnum eD_Out);

#endif

