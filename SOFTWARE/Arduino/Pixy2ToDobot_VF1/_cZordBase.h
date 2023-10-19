//============================================================================//
// _cZordBase                                                      Version 4e //
//                                                                 17.03.2022 //
// Fonctions de base pour les projets _cZord (complément à Arduino)           //
//                                                                            //
// (_cZordBase.h et _cZordBase.cpp doivent être dans votre projet)            //
//                                                                            //
//                                                   ©cZord-IbouX 2019 - 2022 //
//============================================================================//
//                                                                            //
// - L'important n'est pas l'efficacité du code, mais sa lisibilité           // 
// - La façon de coder doit permettre d'éviter au maximum les erreurs         //
//                                                                            //
//============================================================================//
//  Remarques - La manière d'écrire et l'organisation de certaines parties    //
//              du code dans ce projet sont volontairement adaptées aux       //
//              objectifs de l'apprentissage de la programmation dans le      //
//              cadre des labos et projets d'atelier.                         //
//============================================================================//
//                                                                            //
//           N E   P A S   M O D I F I E R   C E   F I C H I E R              //
//                                                                            //
//============================================================================//
// Les classes fournies dans ce fichier permettent de :                       //
//                                                                            //
//      - D_Out        : Gérer les sorties                                    //
//      - D_In         : Gérer les entrées                                    //
//      - Timer        : Gérer des temps non-bloquant                         //
//      - StateMachine : Gérer des machines d'état (avec Entry / Do / Exit)   //
//      - SerialCommandRead  : Gestion du mode "COMMAND" pour la réception    //
//                             des données d'un port série                    //
//                                                                            //
//============================================================================//

#pragma region Versions
//============================================================================//
// v4e                                                                        //
// 17.3.22 - Modification de SerialCommandRead                                //
//           => Correction du paramètre de setTimeOut (de uint_8t à int)      //
//           => Suppression de la vitesse speed du constructeur et ajout dans //
//              begin (pour être similaire à Serial.begin de Arduino)         //
//         - Ajout d'un cast pour suppression d'un warning dans D_Out::write  //
// ---------------------------------------------------------------------------//
// v4d                                                                        //
// 1.11.21 - Ajout du fichier project.h                                       //
//============================================================================//
// v4c                                                                        //
// 29.6.21 - La classe SerialCommandRead est entièrement remaniée             //
//           et n'est plus compatible avec la version v4b                     //
//----------------------------------------------------------------------------//
// v4b                                                                        //
// 9.11.20 - Modification des commentaires interactifs (xml)                  //
//         - Pas de modifications de code et de fonctionnement sauf pour      // 
//           SerialCommandRead (modification du nom des méthodes)             //
//----------------------------------------------------------------------------//
// v4a                                                                        //
// 20.9.20 - Modification des enum dans les classes D_In & D_Out              //
//           => plus de "mélange" de type, par exemple D_In::ON au lieu de    //
//              D_Out::ON                                                     //
//           => D_Out::OFF devient D_Out::state::OFF, etc.                    //
// ATTENTION,   les anciens projets doivent êtres adaptés                     //
//              pour D_Out::State, D_In::state & D_In::pullup                 //
//         - Modification de la classe Timer.                                 //
//           => reset est remplacé par restart                                //
//           => ajout de stopAndReset (arrête de compter et met le cpt à 0    //
// ATTENTION,   les anciens projets doivent êtres adaptés                     //
//              en remplaçant reset par restart                               // 
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
//           stateMachine.entry() n'est pas appelé au début de l'état         //
//----------------------------------------------------------------------------//
// - La compilation de ce projet vide contenant uniquement setup() et         //
//   loop() vides avec la carte Mega2560 utilise (version 2b)                 //
//   -    656 bytes de code et   9 bytes de ram si Debug:Off                  //
//   -   2138                   272             si Debug:Serial               //
//   - 253952                  8192             total de la carte Mega2560    //
//----------------------------------------------------------------------------//
// Testé avec l'environnement suivant :                                       //
// - Windows 10 Professionnel version 1909 (18363.693)                        //
// - Visual Studio Community 2019, version 16.4.5                             //
// - Visual Micro (Arduino IDE for Visual Studio v1912.28.2)                  //
// - IDE Arduino 1.8.12                                                       //
//----------------------------------------------------------------------------//
// Anciennes versions                                                         //
//----------------------------------------------------------------------------//
// v2a                                                                        //
// - Nouvelles versions de D_Out, D_In, Timer, StateMachine,                  //
//   et SerialCommandRead (béta)                                              //
//   (attention, ces fonctions ne sont pas compatible avec les versions       //
//    précédentes v1.x, soit Digital_Out, etc.)                               //
//============================================================================//
#pragma endregion

#pragma region USE {} for constructor
//============================================================================//
// Instanciation d'une classe, appel du constructeur avec {}                  //
//                                                                            //
// L'appel d'un constructeur sans paramètre pose un problème épineux au       //
// complilateur, en effet:                                                    //
// Timer tmr();   - Peut être l'instanciation de tmr appelant le constructeur //
//                  par défaut (sans paramètre)                               //
//                - La déclaration (prototype) d'une méthode tmr retournant   //
//                  un Timer                                                  //
//                Cette syntaxe génère donc une erreur                        //
// Le C++ peut maintenant utiliser l'initialisation uniforme en remplaçant,   //
// dans ce cas, les () par des {}                                             //
// Timer tmr{};    - Est donc accepté                                         //
// Timer tmr;      - A éviter, ok mais pas clair pour l'utilisateur           //
//                             (n'est pas une variable de type Timer!)        //
//                                                                            //
// CONSEIL :  Utiliser systématiquement les {} lors de l'instanciation d'une  //
//            classe (appel du constructeur avec ou sans paramètre)           //
//============================================================================//
#pragma endregion

#pragma region Types de données à préférer
//============================================================================//
// TYPES de données à utiliser de préférences                                 //
// ATTENTION aux conversions, le C/C++ est typé mais tolérant                 //
//----------------------------------------------------------------------------//
// uint8_t, représente un octet (8 bit, unsigned) d'une donnée pouvant être   //
//       une partie d'une trame échangée entre 2 appareils, etc.              //
// int,  représente un nombre entier signé, par exemple un nombre de chaises, //
//       un nombre de personnes entrants (+) ou sortant (-) d'une salle, etc. //
//       (ATTENTION, UNO, MEGA, ... = 16 bits, DUE = 32 bits)                 // 
// long, unsigned long, 32 bits, pour de grands nombres entiers               //
// float, nombre à virgule flottante, 32 bits                                 // 
//       ATTENTION aux limites de l'utilisation d'un réel                     //
// bool, true/false, utilise 8 bits                                           //
// chaîne de caractères, soit tableau avec '\0', char myString[] = "Hello";   //
//       aussi noté string dans documentation Arduino)                        //
//----------------------------------------------------------------------------//
// short => tous les Arduino => signed, 16 bits                               //
//          devrait être préféré à int, mais est peu utilisé                  //
// double, idem à float (sauf sur DUE, double = 64 bits)                      //
// String, object                                                             //
//============================================================================//
#pragma endregion

#pragma region Known issues (problèmes connus)
//============================================================================//
// PROBLEMES CONNUS (Visual Studio, Vmicro, Arduino)                          //
//----------------------------------------------------------------------------//
// Vos modifications ne sont pas prisent en compte 1 -------------------------//
//----------------------------------------------------------------------------//
//   - Après avoir renommé un dossier et le fichier .ino, toujours ouvrir     //
//     le projet la 1ère fois depuis l'environnement Visual Studio            //
//   - Depuis le menu vMicro ... Open Existing Arduino Project et             //
//     sélectionner le fichier .ino du dossier de votre projet                //
//     (Après avoir fermé VS, vous pouvez supprimer les 4 fichiers commencant //
//     par le nom de l'ancien projet)                                         //
//   - Vous pouvez ensuite ouvrir votre projet par double-clic sur *.sln      //
//----------------------------------------------------------------------------//
// Vos modifications ne sont pas prisent en compte 2 -------------------------//
//----------------------------------------------------------------------------//
//   Vous travaillez peut-être sur des fichiers d'un autre dossier            //
//   - Fermer tous les onglets des fichiers ouverts                           //
//     puis les ouvrir à nouveau depuis la fenêtre "Explorateur de solutions" //
//----------------------------------------------------------------------------//
// Le contenu d'une variable n'est pas pris en compte ------------------------//
//----------------------------------------------------------------------------//
//   La valeur d'une variable déclarée à l'intérieur de votre code n'est pas  //
//   prise en compte (pas d'erreur de compil., mais problème à l'exécution)   //
//   (par ex. déclaration dans un case ... d'une machine d'état)              //
//   - TOUJOURS déclarer vos variables au début d'une fonction, par ex. loop  //
//     ou en "global" (à éviter), donc immédiatement après {                  //
//   - Aussi ok si ajout d'un bloc {} avant la déclaration                    //
//   Remarque, la déclaration à l'intérieur d'un for(int i = 1; ...) est OK   //
//   Ce problème est indépendant du type utilisé                              //
//----------------------------------------------------------------------------//
// ATTENTION aux conversions -------------------------------------------------//
//----------------------------------------------------------------------------//
//   Le C/C++ est typé mais tolérant, des erreurs d'exécution sont possibles! //
//   - Par exemple :                                                          //
//     if (byte1 + byte2 > 300) est différent de if (byteRes > 300) !!        //
//     En effet, byteRes = byte1 + byte2;                                     //
//                         <----------->  calcul en int (16 bits)             //
//     mais le résultat tronqué sur 8 bits, il ne peut être > 300 !           //
//     (conversion avec perte de données SANS générer d'ERREUR!)              //
//     DONC if (byteRes > 300) sera toujours FAUX !!                          //
//     MAIS if (byte1 + byte2 > 300) pourra être VRAI ou FAUX                 //
//          en effet le calcul puis la comparaison se fait en int (16 bits)   //
//============================================================================//
#pragma endregion

#pragma region Dossier .vs
//============================================================================//
// Remarque concernant le dossier .vs (25.02.20, VS 2019 16.4.5)              //
//----------------------------------------------------------------------------//
// - Le dossier d'un projet contient un sous-dossier caché nommé .vs,         //
//   celui-ci contient des informations générées par l'IDE Visual Studio      //
//   lors des différents build du projet                                      //
// - Si un nouveau projet est créé à partir d'un existant (en renommant le    //
//   dossier et le fichier .ino) le .vs "archivera" l'ancien projet, chaque   //
//   "archive" prennant env. 100Mb, la taille de .vs devient imposante !!     //
// - Les essais montrent que la suppression de .vs ne pose pas de problème    //
//   pour le projet, il est recréé lors du prochain build du projet           //
// - Toutefois cela "reset" les onglets des fichiers ouverts, développe       //
//   toutes les régions, réduit les fichiers dans l'explorateur de solutions, //
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
// Projet modifié avec SerialCommandRead - tableau de char vs String          //
// L'utilisation du type String augmente directement le code d'environ 2k,    //
// (pour la librairie) mais la ram utilisée varie de moins de 10%             //
//============================================================================//
#pragma endregion

#ifndef __CZORDBASE_h
#define __CZORDBASE_h

#include "arduino.h"

#pragma region Déclarations des classes
//==============================================================================

/// <summary> Initialise la patte désirée en sortie digitale (crée une instance de D_Out)
/// <para>=> D_Out objectName (refPinArduino, physicalValueForOn, initStateOut);</para></summary>
/// <param name = 'refPinArduino'> - Référence Arduino de la patte à initialiser (ex. 3 ou A5)</param>
/// <param name = 'physicalValueForOn'> - Etat physique de la patte pour être 'ON' (0 ou 1)</param>
/// <param name = 'initStateOut'> - Défini l'état initial de la sortie (D_Out::state::ON ou OFF)</param>
class D_Out
{
public:
    enum  class state { OFF, ON, TOGGLE };

    D_Out(uint8_t refPinArduino, uint8_t physicalValueForOn, state initState);

    /// <summary>Lit l'état actuel de la sortie
    /// <para>(état mémorisé, pas état physique de la patte)</para></summary>
    /// <returns>Etat de la sortie (de type D_Out::state)</returns>
    state read();

    /// <summary>Met la sortie à l'état désiré</summary>
    /// <param name = 'sOut'>Etat de la sortie (D_Out::state::ON ou OFF)</param>
    void write(state stateOut);

    /// <summary>Met la sortie à l'état D_Out::state::ON</summary>
    void setOn();

    /// <summary>Met la sortie à l'état D_Out::state::OFF</summary>
    void setOff();

    /// <summary>Permute l'état de la sortie</summary>
    void setToggle();

private:
    uint8_t _numOutToArduino;       // Correspondance entre numOut et numArduino
    uint8_t _physicalValueForOn;    // Etat physique (0/1) à mettre sur la patte pour D_Out::state::ON
    state _currentState;            // Mémo de l'état actuel de la sortie (ON / OFF, pour toggle)
};


/// <summary> Initialise la patte désirée en entrée digitale (crée une instance de D_In)
/// <para>=> D_In objectName (refPinArduino, physicalValueForOn, debounceTime, resistance);</para></summary>
/// <param name = 'refPinArduino'> - Référence Arduino de la patte à initialiser (ex. 3 ou A5)</param>
/// <param name = 'physicalValueForOn'> - Etat physique de la patte pour être lue comme 'ON' (0 ou 1)</param>
/// <param name = 'debounceTime'> - Défini le temps de l'anti-rebonds (0 = pas d'anti-rebond, ou 1 à 255 fois 5ms)</param>
/// <param name = 'resistance'> - Active ou non la pullup interne (D_IN::pullup::ON ou OFF)</param>
class D_In
{
public:
    enum  class state { OFF, ON, UP, DOWN };
    enum  class pullup { OFF, ON };

    D_In(uint8_t refPinArduino, uint8_t physicalValueForOn, uint8_t debounceTime, pullup resistance);

    /// <summary>Retourne l'état de l'entrée digitale
    /// <para>- Retourne le changement d'état immédiatement, puis attends le temps de l'anti-rebonds</para>
    /// <para>- Doit être appelée au minimum toutes les 65535 ms pour fonctionner</para></summary>
    /// <returns>Etat de l'entrée (de type D_In::state, valeurs possibles D_In::state::OFF, UP, ON, DOWN)</returns>
    state read();

private:
    uint8_t _numOutToArduino;       // Correspondance entre numOut et numArduino
    uint8_t _physicalValueForOn;    // Etat physique (0/1) de la patte pour D_In::state::ON
    state _oldState;                // Mémo de l'état précédent pour déterminer les flancs (ON ou OFF)
    uint16_t _debounceTime;         // Temps de debounce pour l'entrée
    uint16_t _startDebounceTime;    // Temps du début du debounce en cours
    bool _flagDebounce;             // true si debounce en cours
};


/// <summary> Initialise un nouveau timer et commence à compter depuis 0 (crée une instance de Timer)
/// <para>=> Timer objectName {};</para>
/// <para>Remarque : utiliser {} et pas ();</para></summary>
class Timer
{
public:
    Timer();

    /// <summary>Redémarre le comptage du temps depuis 0</summary>
    void restart();

    /// <summary>Arrête le comptage du temps et met le timer à 0</summary>
    void stopAnReset();

    /// <summary>Retourne le nombre de ms écoulées pour le timer depuis son dernier restart()
    /// <para>- Retourne toujours 0 après l'appel de stopAnReset()</para>
    /// <para>- Utilise millis() pour mesurer le temps</para></summary>
    /// <returns>Temps écoulé en ms</returns>
    unsigned long get_ms();

private:
    bool _counting;                 // Indique s'il est actif (compte)
    unsigned long _old_ms;          // Pour mémoriser la valeur de millis() précédente
};


/// <summary>Initialise une nouvelle machine d'état (crée une instance de StateMachine)
/// <para>=> StateMachine objectName (initState);</para></summary>
/// <param name = 'initState'> - Valeur de l'état initial désiré (de type uint8)</param>
class StateMachine
{
public:
    StateMachine(uint8_t initState);

    /// <summary>Défini un nouvel état de la machine d'état
    /// <para>- Peut être appelée partout SAUF dans 'entry()' et 'exit()'</para>
    /// <para>- Force le passage dans 'exit()' de l'état en cours puis 'entry()' de newState</para>
    /// <para>  (aussi si newState est identique au précédent!)</para></summary>
    /// <param name = 'newState'> - Valeur du nouvel état désiré</param>
    void StateMachine::change(uint8_t newState);

    /// <summary>Retourne l'état actuel de la machine d'état
    /// <para>- Ne modifie pas 'entry()' ou 'exit()'</para></summary>
    /// <returns>Etat actuel</returns>
    uint8_t StateMachine::get(void);

    /// <summary>Indique si c'est le premier passage dans cet état
    /// <para>- Doit être appelée immédiatement au DEBUT du traitement de l'état</para></summary>
    /// <returns>true si 1er passage dans l'état, sinon false</returns>
    bool StateMachine::entry(void);

    /// <summary>Indique si la méthode change vient d'être appelée et que l'on va donc quitter l'état actuel
    /// <para>- Doit être appelée tout à la FIN du traitement de l'état</para>
    /// <para>- 'Entry()' doit OBLIGATOIREMENT être appelée (avec ou sans code), sinon exit retourne toujours false</para></summary>
    /// <returns>true si on quitte l'état actuel, sinon false</returns>
    bool StateMachine::exit(void);

private:
    uint8_t _state;             // Valeur actuelle de la machine d'état
    bool _flagEntry;            // Pour déterminer entry()
    bool _callEntry;            // Pour déterminer si appel de entry() avant exit()
    bool _flagExit;             // Pour déterminer exit()
};


/// <summary>Instancie un objet SerialCommandRead pour utiliser le mode 'COMMAND'
/// <para>=> SerialCommandRead objectName (*pSerial, start, end, timeout = 100, commandSize = 32);</para>
/// <para>REMARQUES</para>
/// <para>- NE PAS UTILISER les méthodes de Serialx mais celles de cet objet</para>
/// <para>- INITIALISER ce port avec objectName.begin() avant l'appel des méthodes de cette classe</para>
/// <para>- "start" et "end" sont case sensitive et accepte les séquences d'échappement</para>
/// <para>- Consulter l'aide des méthodes de cette classe pour plus de détails</para></summary>
/// <param name = 'pSerial'> - Adresse du port série à utiliser, par ex. &amp;Serial1</param>
/// <param name = 'start'> - String contenant la séquence de départ, peut être vide ""</param>
/// <param name = 'end'> - String contenant la séquence de fin, min. 1 caractère, si "" prend "\r"</param>
/// <param name = 'timeout'> - timeout en ms (10 à 10000)</param>
/// <param name = 'commandSize'> - code optimisé pour une commande jusqu'à cette taille (mais ok pour des commandes plus grandes)</param>
class SerialCommandRead
{
public:
    enum  class state { OK, CHAR_BEFORE_START, WAITING, PROCESSING, ERROR_NO_START, ERROR_TIMEOUT };

    SerialCommandRead(HardwareSerial* pSerial, String start, String end, int timeout = 100, int commandSize = 32);

    /// <summary>Initialise Serialx et défini la vitesse</summary>
    /// <param name = 'speed'> - Vitesse du port série en bits par seconde (bauds)</param>
    void begin(uint32_t speed);

    /// <summary>Désactive le port Serialx</summary>
    void end();

    /// <summary>Défini le délai d'attente pour la réception de "end" 
    /// <para>- Ce délai est le temps d'attente maximun entre chaque caractère reçu</para>
    /// <para>- Après ce délai, sans réception de "end", available() retourne ERROR_TIMEOUT</para>
    /// <para>- Généralement ce délai doit être plus grand que le temps max. écoulé entre 2 appels de available()</para>
    /// <para>Cette méthode REINITIALISE l'analyse par restart()</para></summary>
    /// <param name = 'delay'> - délai du timeout en ms, 10 - 10000, sinon prends 100</param>
    void setTimeout(int delay);

    /// <summary>Vide le buffer de SerialCommandRead et démarre l'analyse de la commande suivante
    /// <para>(ne vide pas le buffer de Arduino)</para></summary>
    void restart();

    /// <summary>Vide les buffers de SerialCommandRead et Serialx de Arduino puis
    /// <para>démarre l'analyse de la commande suivante</para></summary>
    void flushSerialAndRestart();

    /// <summary>Lit les car. reçus par Serialx (Arduino) jusqu'à réception de "end" ou timeout, non bloquante, elle retourne state::...
    /// <para>- OK, "start" et "end" ont été reçus. La commande peut être lue par read()</para>
    /// <para>- CHAR_BEFORE_START, commande OK, mais "start" est précédé par d'autres caractères (supprimés)</para>
    /// <para>- WAITING (attente du 1er car.) ou PROCESSING (attente de "end")</para>
    /// <para>- ERROR_NO_START, "end" est reçu, mais "start" est absent</para>
    /// <para>- ERROR_TIMEOUT, le délai timeout à été dépassé depuis le dernier caractère reçu et "end" est absent</para>
    /// <para>=> si state &lt; WAITING, alors la commande peut être tratée</para>
    /// <para>sinon si &gt; PROCESSING, vider la commande avec restart() ou flushSerialAndRestart()</para></summary>
    /// <returns>SerialCommandRead::state selon liste ci-dessus</returns>
    state available();

    /// <summary>Retourne la commande reçue puis démarre l'analyse de la suivante
    /// <para>- Les caractères avant "start" ainsi que "start" et "end" sont supprimés</para>
    /// <para>- ATTENTION, appeler available() avant</para>
    /// <para>- read() n'efface pas le buffer contenant la commande, cela est fait au prochain appel de available()</para></summary>
    /// <returns>Commande reçue, elle peut être ""</returns>
    String read();

    /// <summary>Envoie la commande désirée précédée de "start" et terminée par "end"
    /// <para>- Accepte aussi une chaîne de caracatères (conversion implicite)</para></summary>
    /// <param name = 'command'>Commande à envoyer par le port Serialx</param>
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

