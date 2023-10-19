// pix2Cam.h

#ifndef _PIX2CAM_h
#define _PIX2CAM_h

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

#include <ZumoMotors.h>
#include <ZumoBuzzer.h>
#include <TPixy2.h>
#include <Pixy2Video.h>
#include <Pixy2UART.h>
#include <Pixy2SPI_SS.h>
#include <Pixy2Line.h>
#include <Pixy2I2C.h>
#include <Pixy2CCC.h>
#include <Pixy2.h>
#include <PIDLoop.h>

// Définitions diverses utilisées dans plusieurs fichiers ...
// define, extern, enum, etc.
// ----------------------------------------------------------------------------



// Prototypes des méthodes (de projet.cpp)
// ----------------------------------------------------------------------------

void Pixy_Init();
void Scan_Blocks();
int NumberOfBlocksDetected();
String ValuesOfBlock(int block);

#endif

