//============================================================================//
// _cZordBase                                                      Version 4e //
//                                                                 17.03.2022 //
// Fonctions de base pour les projets _cZord (compl�ment � Arduino)           //
//                                                                            //
// (_cZordBase.h et _cZordBase.cpp doivent �tre dans votre projet)            //
//                                                                            //
//                                                   �cZord-IbouX 2019 - 2022 //
//============================================================================//
//                                                                            //
// - L'important n'est pas l'efficacit� du code, mais sa lisibilit�           // 
// - La fa�on de coder doit permettre d'�viter au maximum les erreurs         //
//                                                                            //
//============================================================================//
//  Remarques - La mani�re d'�crire et l'organisation de certaines parties    //
//              du code dans ce projet sont volontairement adapt�es aux       //
//              objectifs de l'apprentissage de la programmation dans le      //
//              cadre des labos et projets d'atelier.                         //
//============================================================================//
//                                                                            //
//           N E   P A S   M O D I F I E R   C E   F I C H I E R              //
//                                                                            //
//============================================================================//
// Les classes fournies dans ce fichier permettent de :                       //
//                                                                            //
//      - D_Out        : G�rer les sorties                                    //
//      - D_In         : G�rer les entr�es                                    //
//      - Timer        : G�rer des temps non-bloquant                         //
//      - StateMachine : G�rer des machines d'�tat (avec Entry / Do / Exit)   //
//      - SerialCommandRead  : Gestion du mode "COMMAND" pour la r�ception    //
//                             des donn�es d'un port s�rie                    //
//                                                                            //
//============================================================================//

#pragma region Versions
//============================================================================//
// v4e                                                                        //
// 17.3.22 - Modification de SerialCommandRead                                //
//           => Correction du param�tre de setTimeOut (de uint_8t � int)      //
//           => Suppression de la vitesse speed du constructeur et ajout dans //
//              begin (pour �tre similaire � Serial.begin de Arduino)         //
//         - Ajout d'un cast pour suppression d'un warning dans D_Out::write  //
// ---------------------------------------------------------------------------//
// v4d                                                                        //
// 1.11.21 - Ajout du fichier project.h                                       //
//============================================================================//
// v4c                                                                        //
// 29.6.21 - La classe SerialCommandRead est enti�rement remani�e             //
//           et n'est plus compatible avec la version v4b                     //
//----------------------------------------------------------------------------//
// v4b                                                                        //
// 9.11.20 - Modification des commentaires interactifs (xml)                  //
//         - Pas de modifications de code et de fonctionnement sauf pour      // 
//           SerialCommandRead (modification du nom des m�thodes)             //
//----------------------------------------------------------------------------//
// v4a                                                                        //
// 20.9.20 - Modification des enum dans les classes D_In & D_Out              //
//           => plus de "m�lange" de type, par exemple D_In::ON au lieu de    //
//              D_Out::ON                                                     //
//           => D_Out::OFF devient D_Out::state::OFF, etc.                    //
// ATTENTION,   les anciens projets doivent �tres adapt�s                     //
//              pour D_Out::State, D_In::state & D_In::pullup                 //
//         - Modification de la classe Timer.                                 //
//           => reset est remplac� par restart                                //
//           => ajout de stopAndReset (arr�te de compter et met le cpt � 0    //
// ATTENTION,   les anciens projets doivent �tres adapt�s                     //
//              en rempla�ant reset par restart                               // 
//----------------------------------------------------------------------------//
// v3a                                                                        //
// 26.8.20 - Maj des commentaires XML (pour affichage lors de l'utilisation   //
//           des classes dans le code                                         //
//           (config. environnement et mise en forme ds cZordBase.h)          //
//----------------------------------------------------------------------------//
// v2c                                                                        //
//  3.4.20 - Modifications mineures                                           //
//         - Commentaires ds global.h                                         //
//----------------------------------------------------------------------------//
// v2b                                                                        //
// 25.2.20 - Modifications mineures                                           //
// 25.2.20 - stateMachine.exit()                                              //
//           - suppression de l'envoi de Serial.
// ln("ERROR") si           //
//           stateMachine.entry() n'est pas appel� au d�but de l'�tat         //
//----------------------------------------------------------------------------//
// - La compilation de ce projet vide contenant uniquement setup() et         //
//   loop() vides avec la carte Mega2560 utilise (version 2b)                 //
//   -    656 bytes de code et   9 bytes de ram si Debug:Off                  //
//   -   2138                   272             si Debug:Serial               //
//   - 253952                  8192             total de la carte Mega2560    //
//----------------------------------------------------------------------------//
// Test� avec l'environnement suivant :                                       //
// - Windows 10 Professionnel version 1909 (18363.693)                        //
// - Visual Studio Community 2019, version 16.4.5                             //
// - Visual Micro (Arduino IDE for Visual Studio v1912.28.2)                  //
// - IDE Arduino 1.8.12                                                       //
//----------------------------------------------------------------------------//
// Anciennes versions                                                         //
//----------------------------------------------------------------------------//
// v2a                                                                        //
// - Nouvelles versions de D_Out, D_In, Timer, StateMachine,                  //
//   et SerialCommandRead (b�ta)                                              //
//   (attention, ces fonctions ne sont pas compatible avec les versions       //
//    pr�c�dentes v1.x, soit Digital_Out, etc.)                               //
//============================================================================//
#pragma endregion

#pragma region USE {} for constructor
//============================================================================//
// Instanciation d'une classe, appel du constructeur avec {}                  //
//                                                                            //
// L'appel d'un constructeur sans param�tre pose un probl�me �pineux au       //
// complilateur, en effet:                                                    //
// Timer tmr();   - Peut �tre l'instanciation de tmr appelant le constructeur //
//                  par d�faut (sans param�tre)                               //
//                - La d�claration (prototype) d'une m�thode tmr retournant   //
//                  un Timer                                                  //
//                Cette syntaxe g�n�re donc une erreur                        //
// Le C++ peut maintenant utiliser l'initialisation uniforme en rempla�ant,   //
// dans ce cas, les () par des {}                                             //
// Timer tmr{};    - Est donc accept�                                         //
// Timer tmr;      - A �viter, ok mais pas clair pour l'utilisateur           //
//                             (n'est pas une variable de type Timer!)        //
//                                                                            //
// CONSEIL :  Utiliser syst�matiquement les {} lors de l'instanciation d'une  //
//            classe (appel du constructeur avec ou sans param�tre)           //
//============================================================================//
#pragma endregion

#pragma region Types de donn�es � pr�f�rer
//============================================================================//
// TYPES de donn�es � utiliser de pr�f�rences                                 //
// ATTENTION aux conversions, le C/C++ est typ� mais tol�rant                 //
//----------------------------------------------------------------------------//
// uint8_t, repr�sente un octet (8 bit, unsigned) d'une donn�e pouvant �tre   //
//       une partie d'une trame �chang�e entre 2 appareils, etc.              //
// int,  repr�sente un nombre entier sign�, par exemple un nombre de chaises, //
//       un nombre de personnes entrants (+) ou sortant (-) d'une salle, etc. //
//       (ATTENTION, UNO, MEGA, ... = 16 bits, DUE = 32 bits)                 // 
// long, unsigned long, 32 bits, pour de grands nombres entiers               //
// float, nombre � virgule flottante, 32 bits                                 // 
//       ATTENTION aux limites de l'utilisation d'un r�el                     //
// bool, true/false, utilise 8 bits                                           //
// cha�ne de caract�res, soit tableau avec '\0', char myString[] = "Hello";   //
//       aussi not� string dans documentation Arduino)                        //
//----------------------------------------------------------------------------//
// short => tous les Arduino => signed, 16 bits                               //
//          devrait �tre pr�f�r� � int, mais est peu utilis�                  //
// double, idem � float (sauf sur DUE, double = 64 bits)                      //
// String, object                                                             //
//============================================================================//
#pragma endregion

#pragma region Known issues (probl�mes connus)
//============================================================================//
// PROBLEMES CONNUS (Visual Studio, Vmicro, Arduino)                          //
//----------------------------------------------------------------------------//
// Vos modifications ne sont pas prisent en compte 1 -------------------------//
//----------------------------------------------------------------------------//
//   - Apr�s avoir renomm� un dossier et le fichier .ino, toujours ouvrir     //
//     le projet la 1�re fois depuis l'environnement Visual Studio            //
//   - Depuis le menu vMicro ... Open Existing Arduino Project et             //
//     s�lectionner le fichier .ino du dossier de votre projet                //
//     (Apr�s avoir ferm� VS, vous pouvez supprimer les 4 fichiers commencant //
//     par le nom de l'ancien projet)                                         //
//   - Vous pouvez ensuite ouvrir votre projet par double-clic sur *.sln      //
//----------------------------------------------------------------------------//
// Vos modifications ne sont pas prisent en compte 2 -------------------------//
//----------------------------------------------------------------------------//
//   Vous travaillez peut-�tre sur des fichiers d'un autre dossier            //
//   - Fermer tous les onglets des fichiers ouverts                           //
//     puis les ouvrir � nouveau depuis la fen�tre "Explorateur de solutions" //
//----------------------------------------------------------------------------//
// Le contenu d'une variable n'est pas pris en compte ------------------------//
//----------------------------------------------------------------------------//
//   La valeur d'une variable d�clar�e � l'int�rieur de votre code n'est pas  //
//   prise en compte (pas d'erreur de compil., mais probl�me � l'ex�cution)   //
//   (par ex. d�claration dans un case ... d'une machine d'�tat)              //
//   - TOUJOURS d�clarer vos variables au d�but d'une fonction, par ex. loop  //
//     ou en "global" (� �viter), donc imm�diatement apr�s {                  //
//   - Aussi ok si ajout d'un bloc {} avant la d�claration                    //
//   Remarque, la d�claration � l'int�rieur d'un for(int i = 1; ...) est OK   //
//   Ce probl�me est ind�pendant du type utilis�                              //
//----------------------------------------------------------------------------//
// ATTENTION aux conversions -------------------------------------------------//
//----------------------------------------------------------------------------//
//   Le C/C++ est typ� mais tol�rant, des erreurs d'ex�cution sont possibles! //
//   - Par exemple :                                                          //
//     if (byte1 + byte2 > 300) est diff�rent de if (byteRes > 300) !!        //
//     En effet, byteRes = byte1 + byte2;                                     //
//                         <----------->  calcul en int (16 bits)             //
//     mais le r�sultat tronqu� sur 8 bits, il ne peut �tre > 300 !           //
//     (conversion avec perte de donn�es SANS g�n�rer d'ERREUR!)              //
//     DONC if (byteRes > 300) sera toujours FAUX !!                          //
//     MAIS if (byte1 + byte2 > 300) pourra �tre VRAI ou FAUX                 //
//          en effet le calcul puis la comparaison se fait en int (16 bits)   //
//============================================================================//
#pragma endregion

#pragma region Dossier .vs
//============================================================================//
// Remarque concernant le dossier .vs (25.02.20, VS 2019 16.4.5)              //
//----------------------------------------------------------------------------//
// - Le dossier d'un projet contient un sous-dossier cach� nomm� .vs,         //
//   celui-ci contient des informations g�n�r�es par l'IDE Visual Studio      //
//   lors des diff�rents build du projet                                      //
// - Si un nouveau projet est cr�� � partir d'un existant (en renommant le    //
//   dossier et le fichier .ino) le .vs "archivera" l'ancien projet, chaque   //
//   "archive" prennant env. 100Mb, la taille de .vs devient imposante !!     //
// - Les essais montrent que la suppression de .vs ne pose pas de probl�me    //
//   pour le projet, il est recr�� lors du prochain build du projet           //
// - Toutefois cela "reset" les onglets des fichiers ouverts, d�veloppe       //
//   toutes les r�gions, r�duit les fichiers dans l'explorateur de solutions, //
//   etc.                                                                     //
//============================================================================//
#pragma endregion

#pragma region Taille du code et de la ram - Essais divers
//============================================================================//
// TAILLE DU CODE ET DE LA RAM                                                //
//----------------------------------------------------------------------------//
// Essais divers CLASSIQUE vs OBJET, char[] vs String                         //
// Comparaison version classique vs objet                                     //
// Projet minuteur, lcd, 6 I/O, 2 timers, 1 statemachine, lib _cZord          //
//                      code    ram                                           //
// Ancienne librairie   8068    763                                           //
// Version objet        7610    768                                           //
//                              (la ram augmente plus vite avec plus d'objets)//
// Projet modifi� avec SerialCommandRead - tableau de char vs String          //
// L'utilisation du type String augmente directement le code d'environ 2k,    //
// (pour la librairie) mais la ram utilis�e varie de moins de 10%             //
//============================================================================//
#pragma endregion

#ifndef __CZORDBASE_h
#define __CZORDBASE_h

#include "arduino.h"

#pragma region D�clarations des classes
//==============================================================================

/// <summary> Initialise la patte d�sir�e en sortie digitale (cr�e une instance de D_Out)
/// <para>=> D_Out objectName (refPinArduino, physicalValueForOn, initStateOut);</para></summary>
/// <param name = 'refPinArduino'> - R�f�rence Arduino de la patte � initialiser (ex. 3 ou A5)</param>
/// <param name = 'physicalValueForOn'> - Etat physique de la patte pour �tre 'ON' (0 ou 1)</param>
/// <param name = 'initStateOut'> - D�fini l'�tat initial de la sortie (D_Out::state::ON ou OFF)</param>
class D_Out
{
public:
    enum  class state { OFF, ON, TOGGLE };

    D_Out(uint8_t refPinArduino, uint8_t physicalValueForOn, state initState);

    /// <summary>Lit l'�tat actuel de la sortie
    /// <para>(�tat m�moris�, pas �tat physique de la patte)</para></summary>
    /// <returns>Etat de la sortie (de type D_Out::state)</returns>
    state read();

    /// <summary>Met la sortie � l'�tat d�sir�</summary>
    /// <param name = 'sOut'>Etat de la sortie (D_Out::state::ON ou OFF)</param>
    void write(state stateOut);

    /// <summary>Met la sortie � l'�tat D_Out::state::ON</summary>
    void setOn();

    /// <summary>Met la sortie � l'�tat D_Out::state::OFF</summary>
    void setOff();

    /// <summary>Permute l'�tat de la sortie</summary>
    void setToggle();

private:
    uint8_t _numOutToArduino;       // Correspondance entre numOut et numArduino
    uint8_t _physicalValueForOn;    // Etat physique (0/1) � mettre sur la patte pour D_Out::state::ON
    state _currentState;            // M�mo de l'�tat actuel de la sortie (ON / OFF, pour toggle)
};


/// <summary> Initialise la patte d�sir�e en entr�e digitale (cr�e une instance de D_In)
/// <para>=> D_In objectName (refPinArduino, physicalValueForOn, debounceTime, resistance);</para></summary>
/// <param name = 'refPinArduino'> - R�f�rence Arduino de la patte � initialiser (ex. 3 ou A5)</param>
/// <param name = 'physicalValueForOn'> - Etat physique de la patte pour �tre lue comme 'ON' (0 ou 1)</param>
/// <param name = 'debounceTime'> - D�fini le temps de l'anti-rebonds (0 = pas d'anti-rebond, ou 1 � 255 fois 5ms)</param>
/// <param name = 'resistance'> - Active ou non la pullup interne (D_IN::pullup::ON ou OFF)</param>
class D_In
{
public:
    enum  class state { OFF, ON, UP, DOWN };
    enum  class pullup { OFF, ON };

    D_In(uint8_t refPinArduino, uint8_t physicalValueForOn, uint8_t debounceTime, pullup resistance);

    /// <summary>Retourne l'�tat de l'entr�e digitale
    /// <para>- Retourne le changement d'�tat imm�diatement, puis attends le temps de l'anti-rebonds</para>
    /// <para>- Doit �tre appel�e au minimum toutes les 65535 ms pour fonctionner</para></summary>
    /// <returns>Etat de l'entr�e (de type D_In::state, valeurs possibles D_In::state::OFF, UP, ON, DOWN)</returns>
    state read();

private:
    uint8_t _numOutToArduino;       // Correspondance entre numOut et numArduino
    uint8_t _physicalValueForOn;    // Etat physique (0/1) de la patte pour D_In::state::ON
    state _oldState;                // M�mo de l'�tat pr�c�dent pour d�terminer les flancs (ON ou OFF)
    uint16_t _debounceTime;         // Temps de debounce pour l'entr�e
    uint16_t _startDebounceTime;    // Temps du d�but du debounce en cours
    bool _flagDebounce;             // true si debounce en cours
};


/// <summary> Initialise un nouveau timer et commence � compter depuis 0 (cr�e une instance de Timer)
/// <para>=> Timer objectName {};</para>
/// <para>Remarque : utiliser {} et pas ();</para></summary>
class Timer
{
public:
    Timer();

    /// <summary>Red�marre le comptage du temps depuis 0</summary>
    void restart();

    /// <summary>Arr�te le comptage du temps et met le timer � 0</summary>
    void stopAnReset();

    /// <summary>Retourne le nombre de ms �coul�es pour le timer depuis son dernier restart()
    /// <para>- Retourne toujours 0 apr�s l'appel de stopAnReset()</para>
    /// <para>- Utilise millis() pour mesurer le temps</para></summary>
    /// <returns>Temps �coul� en ms</returns>
    unsigned long get_ms();

private:
    bool _counting;                 // Indique s'il est actif (compte)
    unsigned long _old_ms;          // Pour m�moriser la valeur de millis() pr�c�dente
};


/// <summary>Initialise une nouvelle machine d'�tat (cr�e une instance de StateMachine)
/// <para>=> StateMachine objectName (initState);</para></summary>
/// <param name = 'initState'> - Valeur de l'�tat initial d�sir� (de type uint8)</param>
class StateMachine
{
public:
    StateMachine(uint8_t initState);

    /// <summary>D�fini un nouvel �tat de la machine d'�tat
    /// <para>- Peut �tre appel�e partout SAUF dans 'entry()' et 'exit()'</para>
    /// <para>- Force le passage dans 'exit()' de l'�tat en cours puis 'entry()' de newState</para>
    /// <para>  (aussi si newState est identique au pr�c�dent!)</para></summary>
    /// <param name = 'newState'> - Valeur du nouvel �tat d�sir�</param>
    void StateMachine::change(uint8_t newState);

    /// <summary>Retourne l'�tat actuel de la machine d'�tat
    /// <para>- Ne modifie pas 'entry()' ou 'exit()'</para></summary>
    /// <returns>Etat actuel</returns>
    uint8_t StateMachine::get(void);

    /// <summary>Indique si c'est le premier passage dans cet �tat
    /// <para>- Doit �tre appel�e imm�diatement au DEBUT du traitement de l'�tat</para></summary>
    /// <returns>true si 1er passage dans l'�tat, sinon false</returns>
    bool StateMachine::entry(void);

    /// <summary>Indique si la m�thode change vient d'�tre appel�e et que l'on va donc quitter l'�tat actuel
    /// <para>- Doit �tre appel�e tout � la FIN du traitement de l'�tat</para>
    /// <para>- 'Entry()' doit OBLIGATOIREMENT �tre appel�e (avec ou sans code), sinon exit retourne toujours false</para></summary>
    /// <returns>true si on quitte l'�tat actuel, sinon false</returns>
    bool StateMachine::exit(void);

private:
    uint8_t _state;             // Valeur actuelle de la machine d'�tat
    bool _flagEntry;            // Pour d�terminer entry()
    bool _callEntry;            // Pour d�terminer si appel de entry() avant exit()
    bool _flagExit;             // Pour d�terminer exit()
};


/// <summary>Instancie un objet SerialCommandRead pour utiliser le mode 'COMMAND'
/// <para>=> SerialCommandRead objectName (*pSerial, start, end, timeout = 100, commandSize = 32);</para>
/// <para>REMARQUES</para>
/// <para>- NE PAS UTILISER les m�thodes de Serialx mais celles de cet objet</para>
/// <para>- INITIALISER ce port avec objectName.begin() avant l'appel des m�thodes de cette classe</para>
/// <para>- "start" et "end" sont case sensitive et accepte les s�quences d'�chappement</para>
/// <para>- Consulter l'aide des m�thodes de cette classe pour plus de d�tails</para></summary>
/// <param name = 'pSerial'> - Adresse du port s�rie � utiliser, par ex. &amp;Serial1</param>
/// <param name = 'start'> - String contenant la s�quence de d�part, peut �tre vide ""</param>
/// <param name = 'end'> - String contenant la s�quence de fin, min. 1 caract�re, si "" prend "\r"</param>
/// <param name = 'timeout'> - timeout en ms (10 � 10000)</param>
/// <param name = 'commandSize'> - code optimis� pour une commande jusqu'� cette taille (mais ok pour des commandes plus grandes)</param>
class SerialCommandRead
{
public:
    enum  class state { OK, CHAR_BEFORE_START, WAITING, PROCESSING, ERROR_NO_START, ERROR_TIMEOUT };

    SerialCommandRead(HardwareSerial* pSerial, String start, String end, int timeout = 100, int commandSize = 32);

    /// <summary>Initialise Serialx et d�fini la vitesse</summary>
    /// <param name = 'speed'> - Vitesse du port s�rie en bits par seconde (bauds)</param>
    void begin(uint32_t speed);

    /// <summary>D�sactive le port Serialx</summary>
    void end();

    /// <summary>D�fini le d�lai d'attente pour la r�ception de "end" 
    /// <para>- Ce d�lai est le temps d'attente maximun entre chaque caract�re re�u</para>
    /// <para>- Apr�s ce d�lai, sans r�ception de "end", available() retourne ERROR_TIMEOUT</para>
    /// <para>- G�n�ralement ce d�lai doit �tre plus grand que le temps max. �coul� entre 2 appels de available()</para>
    /// <para>Cette m�thode REINITIALISE l'analyse par restart()</para></summary>
    /// <param name = 'delay'> - d�lai du timeout en ms, 10 - 10000, sinon prends 100</param>
    void setTimeout(int delay);

    /// <summary>Vide le buffer de SerialCommandRead et d�marre l'analyse de la commande suivante
    /// <para>(ne vide pas le buffer de Arduino)</para></summary>
    void restart();

    /// <summary>Vide les buffers de SerialCommandRead et Serialx de Arduino puis
    /// <para>d�marre l'analyse de la commande suivante</para></summary>
    void flushSerialAndRestart();

    /// <summary>Lit les car. re�us par Serialx (Arduino) jusqu'� r�ception de "end" ou timeout, non bloquante, elle retourne state::...
    /// <para>- OK, "start" et "end" ont �t� re�us. La commande peut �tre lue par read()</para>
    /// <para>- CHAR_BEFORE_START, commande OK, mais "start" est pr�c�d� par d'autres caract�res (supprim�s)</para>
    /// <para>- WAITING (attente du 1er car.) ou PROCESSING (attente de "end")</para>
    /// <para>- ERROR_NO_START, "end" est re�u, mais "start" est absent</para>
    /// <para>- ERROR_TIMEOUT, le d�lai timeout � �t� d�pass� depuis le dernier caract�re re�u et "end" est absent</para>
    /// <para>=> si state &lt; WAITING, alors la commande peut �tre trat�e</para>
    /// <para>sinon si &gt; PROCESSING, vider la commande avec restart() ou flushSerialAndRestart()</para></summary>
    /// <returns>SerialCommandRead::state selon liste ci-dessus</returns>
    state available();

    /// <summary>Retourne la commande re�ue puis d�marre l'analyse de la suivante
    /// <para>- Les caract�res avant "start" ainsi que "start" et "end" sont supprim�s</para>
    /// <para>- ATTENTION, appeler available() avant</para>
    /// <para>- read() n'efface pas le buffer contenant la commande, cela est fait au prochain appel de available()</para></summary>
    /// <returns>Commande re�ue, elle peut �tre ""</returns>
    String read();

    /// <summary>Envoie la commande d�sir�e pr�c�d�e de "start" et termin�e par "end"
    /// <para>- Accepte aussi une cha�ne de caracat�res (conversion implicite)</para></summary>
    /// <param name = 'command'>Commande � envoyer par le port Serialx</param>
    void print(String command);

private:
    HardwareSerial* _pSerial;
    uint32_t _speed;
    String _start;
    String _end;
    String _command;
    enum eSm { WAIT_CHAR, WAIT_END, DATA_READY_TO_READ, TIMEOUT };
    StateMachine sm{ eSm::WAIT_CHAR };
    state _state;
    int _timeout;
    bool _initMillisForTimeout;
    unsigned long _oldMillis;
};

#pragma endregion

#endif

