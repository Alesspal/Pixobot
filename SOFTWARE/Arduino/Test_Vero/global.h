// ---------------------------------------------------------------------------------------------
// NOTE IMPORTANTE concernant la stabilité de l'environnement                             8.3.22
// 
// ~99% des "plantées" de Arduino avec VS2019 & Visual Micro sont dues ...
// ... à celui qui lit ce message :-))))
// 
// Pour éviter cela
// ----------------
// Dans le menu Visual STUDIO, sélectionner "Release" pour la "configurations de solutions"
// et sutout pas Debug.
//
// ATTENTION, la suppression du dossier caché .vs ou le transfert du projet sur un autre PC
//            peut remettre cette config sur "Debug"
//      
// UTILISATION du moniteur série de Arduino
// ----------------------------------------
// ==> Menu vMicro...Debugger...Debug...Debug:Serial
//     Permet d'utiliser le moniteur série de Arduino
//     -> Pour afficher le moniteur série (et voir les données du port Serial)
//        - VMicro ... View Port Monitor
//        - Dans le menu en bas, vérifier que les bauds correspondent à ceux de votre application
//          (si le menu n'apparaît pas, DEPLACER la fenêtre et (re)positionnez-la en bas ou à droite)
// ==> Menu vMicro...Debugger...Debug...Debug:Off
//     Pas d'utilisation du moniteur série de Arduino, MAIS utilisation possible d'un autre terminal,
//     par exemple Termite, TermiZord, ...
//
// UNE BONNE HABITUDE
// ------------------
// TOUJOURS travailler avec les menus de Visual MICRO (vMicro) et pas ceux de Visual STUDIO
// (par exemple utiliser vMicro...Buil & Upload et pas "Démarrer"
// 
// ---------------------------------------------------------------------------------------------


#ifndef _GLOBAL_h
#define _GLOBAL_h

#include <Arduino.h>
// ----------------------------------------------------------------------------
// DEBUG conditionnel cZarduiBase
// Il suffit de mettre la ligne ci-dessous (#define DEBUG_SERIAL) en commentaire
// pour "supprimer" (ne pas compiler et exécuter) les lignes DEBUG_... dans votre code
#define DEBUG_SERIAL


#pragma region DO_NOT_EDIT
// Ne PAS modifier le code de la region DO_NOT_EDIT
// ****************************************************************************
// Activation / désactivation rapide du debug par le port Serial
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
// EXEMPLE de code pour les objets utilisés dans plusieurs fichiers
// ----------------------------------------------------------------------------
// Dans le fichier source où l'objet est principalement utilisés (x.cpp ou x.ino)
// - Déclarer l'objet (instanciation)
// Dans le/les fichiers d'entêtes dans lequel vous désirez utilser l'objet (x.h)
// - Si nécessaire, ajouter le #include adéquat (par ex. <LiquidCrystal.h>)
// - Ajouter la référence à l'objet avec le mot clé extern
//   cela permet à l'objet d'être accessible dans les autres fichiers 
//   sans le créer à nouveau
// ----------------------------------------------------------------------------
// Procéder de même pour des variables "globales", cependant elles sont à EVITER !
// ----------------------------------------------------------------------------
//
// Exemple pour un affichage lcd utilisé dans *.ino et projet.cpp
//
// #include <LiquidCrystal.h>    - ajouter cette ligne dans ce fichier
// LiquidCrystal monLcd( ... );	 - Déclarer l'objet dans le fichier projet.cpp
// extern LiquidCrystal monLcd;  - Ajouter sa reférence "extern" dans ce fichier
//
// Les méthodes lcd.xxx seront alors utilisables dans les fichiers 
// contenant #include "global.h"
// ----------------------------------------------------------------------------

// --------------------------------------------------------------------------------------
// Modifier ou compléter, si nécéssaire, le code ci-dessous
// --------------------------------------------------------------------------------------
// Définitions diverses utiles pour tout le projet
// define, extern, enum, etc.
// Ne pas oublier d'ajouter #include "global.h" dans les fichiers concernés
// ----------------------------------------------------------------------------




#endif

