// ---------------------------------------------------------------------------------------------
// NOTE IMPORTANTE concernant la stabilit� de l'environnement                             8.3.22
// 
// ~99% des "plant�es" de Arduino avec VS2019 & Visual Micro sont dues ...
// ... � celui qui lit ce message :-))))
// 
// Pour �viter cela
// ----------------
// Dans le menu Visual STUDIO, s�lectionner "Release" pour la "configurations de solutions"
// et sutout pas Debug.
//
// ATTENTION, la suppression du dossier cach� .vs ou le transfert du projet sur un autre PC
//            peut remettre cette config sur "Debug"
//      
// UTILISATION du moniteur s�rie de Arduino
// ----------------------------------------
// ==> Menu vMicro...Debugger...Debug...Debug:Serial
//     Permet d'utiliser le moniteur s�rie de Arduino
//     -> Pour afficher le moniteur s�rie (et voir les donn�es du port Serial)
//        - VMicro ... View Port Monitor
//        - Dans le menu en bas, v�rifier que les bauds correspondent � ceux de votre application
//          (si le menu n'appara�t pas, DEPLACER la fen�tre et (re)positionnez-la en bas ou � droite)
// ==> Menu vMicro...Debugger...Debug...Debug:Off
//     Pas d'utilisation du moniteur s�rie de Arduino, MAIS utilisation possible d'un autre terminal,
//     par exemple Termite, TermiZord, ...
//
// UNE BONNE HABITUDE
// ------------------
// TOUJOURS travailler avec les menus de Visual MICRO (vMicro) et pas ceux de Visual STUDIO
// (par exemple utiliser vMicro...Buil & Upload et pas "D�marrer"
// 
// ---------------------------------------------------------------------------------------------


#ifndef _GLOBAL_h
#define _GLOBAL_h

#include <Arduino.h>
// ----------------------------------------------------------------------------
// DEBUG conditionnel cZarduiBase
// Il suffit de mettre la ligne ci-dessous (#define DEBUG_SERIAL) en commentaire
// pour "supprimer" (ne pas compiler et ex�cuter) les lignes DEBUG_... dans votre code
#define DEBUG_SERIAL


#pragma region DO_NOT_EDIT
// Ne PAS modifier le code de la region DO_NOT_EDIT
// ****************************************************************************
// Activation / d�sactivation rapide du debug par le port Serial
// ----------------------------------------------------------------------------
// La ligne ci-dessous permet de remplacer Serial.begin/print/println 
//                                      par DEBUG_BEGIN/PRINT/PRINTLN
// Par exemple
// DEBUG_BEGIN(9600);					// au lieu de Serial.begin(9600);
// DEBUG_PRINTLN("Debug ON");		    // au lieu de Serial.println("Debug ON");

#ifdef DEBUG_SERIAL
#define DEBUG_BEGIN(bds) Serial.begin(bds)
#define DEBUG_PRINT(x) Serial.print(x)
#define DEBUG_PRINTLN(x) Serial.println(x)
#else
#define DEBUG_BEGIN(bds)
#define DEBUG_PRINT(x)
#define DEBUG_PRINTLN(x)
#endif
// ****************************************************************************
#pragma endregion (DO_NOT_EDIT)


// ----------------------------------------------------------------------------
// EXEMPLE de code pour les objets utilis�s dans plusieurs fichiers
// ----------------------------------------------------------------------------
// Dans le fichier source o� l'objet est principalement utilis�s (x.cpp ou x.ino)
// - D�clarer l'objet (instanciation)
// Dans le/les fichiers d'ent�tes dans lequel vous d�sirez utilser l'objet (x.h)
// - Si n�cessaire, ajouter le #include ad�quat (par ex. <LiquidCrystal.h>)
// - Ajouter la r�f�rence � l'objet avec le mot cl� extern
//   cela permet � l'objet d'�tre accessible dans les autres fichiers 
//   sans le cr�er � nouveau
// ----------------------------------------------------------------------------
// Proc�der de m�me pour des variables "globales", cependant elles sont � EVITER !
// ----------------------------------------------------------------------------
//
// Exemple pour un affichage lcd utilis� dans *.ino et projet.cpp
//
// #include <LiquidCrystal.h>    - ajouter cette ligne dans ce fichier
// LiquidCrystal monLcd( ... );	 - D�clarer l'objet dans le fichier projet.cpp
// extern LiquidCrystal monLcd;  - Ajouter sa ref�rence "extern" dans ce fichier
//
// Les m�thodes lcd.xxx seront alors utilisables dans les fichiers 
// contenant #include "global.h"
// ----------------------------------------------------------------------------

// --------------------------------------------------------------------------------------
// Modifier ou compl�ter, si n�c�ssaire, le code ci-dessous
// --------------------------------------------------------------------------------------
// D�finitions diverses utiles pour tout le projet
// define, extern, enum, etc.
// Ne pas oublier d'ajouter #include "global.h" dans les fichiers concern�s
// ----------------------------------------------------------------------------




#endif

