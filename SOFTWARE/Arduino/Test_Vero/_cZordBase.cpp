//============================================================================//
// _cZordBase                                                      Version 4e //
//                                                                            //
// Fonctions de base pour les projets _cZord (complément à Arduino)           //
//                                                                            //
// (_cZordBase.h et _cZordBase.cpp doivent être dans votre projet)            //
//                                                                            //
//                                                   ©cZord-IbouX 2019 - 2021 //
//============================================================================//
//                                                                            //
//           N E   P A S   M O D I F I E R   C E   F I C H I E R              //
//                                                                            //
//============================================================================//
#include "_cZordBase.h"


#pragma region Gestion des sorties digitales   -   Class D_Out
//################################################################################################
//                                                 Gestion des sorties digitales   -   Class D_Out
//================================================================================================


//==============================================================================
// © cZord-IbouX 2019-2020 
// 8.11.20
//==============================================================================
D_Out::D_Out(uint8_t refPinArduino, uint8_t physicalValueForOn, state initStateOut) {

    _numOutToArduino = refPinArduino;       // Récupère la référence de la pin "Arduino"
    // Etat physique (0/1) à mettre sur la patte pour l'état ON (check, 0 reste, autre valeur devient 1)
    _physicalValueForOn = physicalValueForOn ? 1 : 0;
    _currentState = initStateOut;           // Mémo de l'état courant, voir D_Out::read
    // Défini l'état et configure la patte en sortie
    write(initStateOut);
    pinMode(_numOutToArduino, OUTPUT);
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
// 15.3.22 - cast pour suppression d'un warning
//==============================================================================
void D_Out::write(state stateOut) {

    if (stateOut == state::TOGGLE)
    {
        // stateOut : récupère l'état physique de la patte dans _currentState et l'inverse
        stateOut = state::OFF;
        if (_currentState == state::OFF) stateOut = state::ON;
    }
    // Convertir stateOut en 0 ou 1 selon _physicalValueForOn
    uint8_t physicalValue = _physicalValueForOn;
    // 15.3.22 physicalValue = !physicalValue génère un warning => cast
    if (stateOut == state::OFF) { physicalValue = (uint8_t)(!(int)physicalValue); }
    // Maj de la sortie et de _currentState
    digitalWrite(_numOutToArduino, physicalValue);
    _currentState = stateOut;
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
void D_Out::setOn() {
    write(state::ON);
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
void D_Out::setOff() {
    write(state::OFF);
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
void D_Out::setToggle() {
    write(state::TOGGLE);
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
D_Out::state D_Out::read() {
    // On n'utilise pas digitalRead car ne fonctionne pas avec tous les Arduino
    // Par exemple KO avec Due
    return _currentState;
}
#pragma endregion

#pragma region Gestion des entrées digitales   -   Class D_In
//################################################################################################
//                                                  Gestion des entrées digitales   -   Class D_In
//================================================================================================

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
D_In::D_In(uint8_t refPinArduino, uint8_t physicalValueForOn, uint8_t debounceTime, pullup resistance) {

    _numOutToArduino = refPinArduino;       // Récupère la référence de la pin "Arduino"
    // Configure la patte en entrée (ici, donc petit délai avant read plus bas)
    if (resistance == pullup::OFF) {
        pinMode(_numOutToArduino, INPUT);
    }
    else {
        pinMode(_numOutToArduino, INPUT_PULLUP);
    }
    // Etat physique (0/1) à mettre sur la patte pour l'état ON (check, 0 reste, autre valeur devient 1)
    _physicalValueForOn = physicalValueForOn ? 1 : 0;
    _debounceTime = (uint16_t)debounceTime * 5;
    _flagDebounce = false;                  // Pas de debounce en cours
    // Memorisation de l'état physique actuel (pour UP / DOWN)
    // Code entre pinMode et digitalRead => délai pour stabilité (en principe déjà IN)
    _oldState = state::OFF;
    if (digitalRead(_numOutToArduino) == _physicalValueForOn) _oldState = state::ON;
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
D_In::state D_In::read() {
    // Remarque : On prend 16 bits de millis() => doit être appelée au min
    //            toutes les 65535 ms pour fonctionner

    if (_flagDebounce) {          // Debounce en cours
        // cast uint16_t, aussi pour Due (calcule en 32 bits)
        if ((uint16_t)((uint16_t)millis() - (uint16_t)_startDebounceTime) <= (uint16_t)_debounceTime) {
            // Debounce en cours => conserver l'état précédent
            return _oldState;
        }
        _flagDebounce = false;             // Arrêt du debounce
    }

    // Ici, donc pas ou fin du debounce en cours
    // Lecture de l'état actuel de la patte
    state stateNow = state::OFF;
    if (digitalRead(_numOutToArduino) == _physicalValueForOn) stateNow = state::ON;

    if (stateNow != _oldState) {            // Chagement d'état => flanc
        // Si _debounceTime est défini => démarrer le compteur
        if (_debounceTime != 0) {
            _startDebounceTime = (uint16_t)millis();
            _flagDebounce = true;
        }
        _oldState = stateNow;               // Mémorise l'état actuel
        if (stateNow == state::ON) {        // UP ou DOWN ?
            stateNow = state::UP;
        }
        else {
            stateNow = state::DOWN;
        }
    }
    return stateNow;
}
#pragma endregion

#pragma region Gestion des temps, utilisation de millis()   -   Class Timer
//################################################################################################
//                                    Gestion des temps, utilisation de millis()   -   Class Timer
//================================================================================================

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
Timer::Timer() {
    Timer::restart();
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
void Timer::restart() {
    _counting = true;
    _old_ms = millis();
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
void Timer::stopAnReset() {
    _counting = false;
    _old_ms = millis();
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
unsigned long Timer::get_ms() {
    if (_counting) return millis() - _old_ms;
    else return 0ul;
}

// QUE SE PASSE-T'IL LOSRQUE millis() repart à 0 ?
// -----------------------------------------------------------------------------
// millis() fournit le nombre de ms depuis le lancement de votre programme
// Il arrivera à son maximum après ~49.7 jours
// Il repart ensuite à 0
// En cas de dépassement pour un unsigned (rollover), le calcul suivant reste
// juste : if ((currentTime - oldTime) >= delayTime)
// Il n'y a donc aucune précaution à prendre, la librairie TIME fonctionnera
// toujours après 49 jours, pour autant que TIME_Reset soit appelé au moins 
// une fois tous les 49 jours
// -----------------------------------------------------------------------------
// Voici un programme prouvant cela, pour simplifier on travaille avec un uint8_t
// Port open
// 240  240  0
// 241  240  1
// 242  ...
// ...
// 250  240  10
// Delai OK
// 251  250  1
// ...
// 255  250  5
// 0  250  6
// ...
// 4  250  10
// Delai OK
// 5  4  1
// ...
// uint8_t currentTime = 240, oldTime = 240, delayTime = 10;
// void loop() {
//     Serial.print(currentTime);
//     Serial.print("  ");
//     Serial.print(oldTime);

//     uint8_t tempsEcoule = currentTime - oldTime;
//     Serial.print("  ");
//     Serial.println(tempsEcoule);

//     // ATTENTION, si le calcul est fait dans le if, il faut caster 
//     // pour simuler le rollover du uint8_t
//     // if ((uint8_t)(currentTime - oldTime) >= delayTime)
//     if (tempsEcoule >= delayTime)
//     {
//         // Temps écoulé, reset du timer
//         oldTime = currentTime;
//         Serial.println("Delai OK");
//     }
//     currentTime++;
// }
#pragma endregion

#pragma region Gestion de la machine d'état   -   Class StateMachine
//################################################################################################
//                                           Gestion de la machine d'état   -   Class StateMachine
//================================================================================================
// Réflexion pour gestion de entry() et exit()
// entry : change met un flag à true
//         1er appel => retourne true et met flag à false
// exit :  - change dans état puis exit() => doit retourner true
//         - change dans un autre état (ou hors état) => doit retourner false
//           le SEUL moyen de différencier ces cas est d'utiliser le passage dans entry()
// Il est donc OBLIGATOIRE d'appeler entry() si on veut utiliser exit()
// (utilisation d'un flag pour vérification, si pas fait exit() retourne tjs false;
//------------------------------------------------------------------------------------------------

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
StateMachine::StateMachine(uint8_t initState) {
    _state = initState;
    _flagEntry = true;
    _callEntry = false;
    _flagExit = true;
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
void StateMachine::change(uint8_t newState) {
    _state = newState;
    _flagEntry = true;
    // _callEntry ne doit pas être modifié ici!
    _flagExit = true;
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
uint8_t StateMachine::get(void) {
    return _state;
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
bool StateMachine::entry(void) {
    bool tmp = _flagEntry;
    _flagEntry = false;
    // Si on passe ici, exit() doit retourner false car change() a été appelé
    // hors de "cet" état (hors de l'état ou depuis un autre état)
    // (voir commentaire dans l'analyse)
    _flagExit = false;
    _callEntry = true;
    return tmp;
}

//==============================================================================
// © cZord-IbouX 2019-2020
// 8.11.20
//==============================================================================
bool StateMachine::exit(void) {
    bool tmp = _flagExit;
    // Si pas de passage par entry(), retourne false
    // Sinon retourne _flagExit
    if (!_callEntry) {
        tmp = false;
        // v2b, suppression de la ligne ci-dessous
        // Serial.println("ERROR");
    }
    _callEntry = false;
    return tmp;
}
#pragma endregion


#pragma region Serial - Gestion du mode COMMAND pour la réception  -   Class SerialCommandRead
//################################################################################################
//                           Serial - Gestion du mode COMMAND   -   Class SerialCommandRead (béta)
//================================================================================================

//==============================================================================
// © cZord-IbouX 2019-2021
// 29.6.21
// 15.3.22 - "Transfert" du paramètre speed à begin
//==============================================================================
SerialCommandRead::SerialCommandRead(HardwareSerial* pSerial, String start, String end,
    int timeout = 100, int commandSize = 32) {

    _pSerial = pSerial;         // Port série à utiliser
    _command.reserve(commandSize >= 16 && commandSize <= 128 ? commandSize : 64);
    _command = "";              // Réinitialise la commande
    _start = start;             // Utilisation d'un String, permet déclaration taille "dynamique"
    _end = end;
    if (_end.length() == 0) _end = "\r";
    setTimeout(timeout);
    _initMillisForTimeout = false;
}

//==============================================================================
// © cZord-IbouX 2019-2021
//  1.7.21
// 15.3.22 - Ajout du paramètre speed
//==============================================================================
void SerialCommandRead::begin(uint32_t speed) {
    _speed = speed;
    _pSerial->begin(_speed);
    flushSerialAndRestart();
}

//==============================================================================
// © cZord-IbouX 2019-2021
// 1.7.21
//==============================================================================
void SerialCommandRead::end() {
    _pSerial->end();
}

//==============================================================================
// © cZord-IbouX 2019-2021
// 29.6.21
//==============================================================================
void SerialCommandRead::setTimeout(int delay) {
    _timeout = delay >= 10 && delay <= 10000 ? delay : 100;
    restart();
}

//==============================================================================
// © cZord-IbouX 2019-2021
// 29.6.21
//==============================================================================
void SerialCommandRead::flushSerialAndRestart() {
    while (_pSerial->available() > 0) _pSerial->read(); // Vider les car. dans le buffer Serialx
    sm.change(eSm::WAIT_CHAR);                         // Redémarre l'analyse
}

//==============================================================================
// © cZord-IbouX 2019-2021
// 29.6.21
//==============================================================================
void SerialCommandRead::restart() {
    sm.change(eSm::WAIT_CHAR);                         // Redémarre l'analyse
}

//==============================================================================
// © cZord-IbouX 2019-2021
// 27.6.21
//==============================================================================
SerialCommandRead::state SerialCommandRead::available() {
    bool cont, receive = false;
    // Utilisation de if et pas if else ou switch, passe immédiatement à l'état suivant (ici séquentiel)
    // et continue de lire les car tq available et pas au prochain appel de available() 

    // TIMEOUT
    // - En arrivant ici, si des car. sont reçus, on traite ces car. avant le timeout
    //   En effet, dû à l'application, le temps entre 2 appels de cette fonction peut
    //   provoquer un timeout alors que les car. étaient déjà dans le buffer Arduino
    //   auparavant
    // - DONC, on vérifie les car. reçus et si "end" n'est pas reçu et que le 
    //   délai timeout est dépassé, alors ERROR_TIMEOUT est retourné
    //   Sinon timeout redémarre

    //-- WAIT_CHAR ------------------------------------------------------------
    if (sm.get() == eSm::WAIT_CHAR)
    {
        if (sm.entry()) {
            //Serial.println("command::WAIT_START entry");
            _command = "";
            _state = state::WAITING;
            _initMillisForTimeout = false;
        }

        if (_pSerial->available() > 0) {
            sm.change(eSm::WAIT_END);           // car. reçu => on analyse
        }
    }

    //-- WAIT END ------------------------------------------------------------
    if (sm.get() == eSm::WAIT_END)
    {
        if (sm.entry()) {
            _state = state::PROCESSING;
        }

        // Recherche du "start" si != ""
        //           du "end", aussi en cas d'absence du "start"
        // On lit tous les caractères bufferisés jusqu'au "end" si présent
        // Lire les car. les uns après les autres et tester "end", afin de ne pas
        // vider le buffer Arduino avec un/des car. de la commande suivante
        cont = true;
        while (cont) {
            if (_pSerial->available() > 0) {
                _command += (char)_pSerial->read();     // cast obligatoire
                receive = true;                         // pour timeout
                if (_command.endsWith(_end)) {
                    cont = false;                       // commande ok termine l'analyse
                                                        // le buffer Arduino peut contenir d'autres car.
                                                        // qui seront pris en compte pour la prochaine commande
                    sm.change(eSm::DATA_READY_TO_READ);
                }
            }
            else {
                cont = false;                           // plus de car. ds le buffer
            }
        }
    }
    // -- DATA_READY_TO_READ ----------------------------------------------------
    // On arrive ici si "end" est reçu
    // Attente de la lecture de la commande par read
    // Si read() n'est pas appelé, alors utiliser restart()pour commencer l'analyse
    // de la commande suivante
    // Pendan ce temps, les car. reçus mémorisés par buffer Arduino
    if (sm.get() == eSm::DATA_READY_TO_READ)
    {
        if (sm.entry()) {
            _state = state::OK;
            // Supprimer "end"
            _command.remove(_command.length() - _end.length());
            if (_start.length() > 0) {
                int pos = _command.indexOf(_start);
                if (pos == -1) { _state = state::ERROR_NO_START; }
                else {
                    // Start => supprimer tous les car. avant et le "start"
                    _command.remove(0, pos + _start.length());
                    // != 0 => car. avant
                    if (pos != 0) { _state = state::CHAR_BEFORE_START; }
                }
            }
        }
    }

    if (sm.get() == eSm::TIMEOUT) {
        // On ne fait plus rien, jusqu'à read() ou autre
    }

    // Ici on peut avoir _state
    // - OK                         : _command contient la commande à lire (sans "start" ni "end")
    // - WAITING                    : attente de réception du 1er caractère de la commande
    // - PROCESSING                 : commande en cours de réception
    // - ERROR_NO_START             : erreur de la séquence "start", pensez à vider le "buffer" pour commencer une nouvelle analyse
    // - ERROR_CHAR_BEFORE_START    : les car. avant "start", "start et "end" sont supprimés
    // - ERROR_TIMEOUT              : pas de "end" (ou partiel) et délai du timeout écoulé depuis le l'appel de available() précédent

    // Vérification du timeout uniquement si PROCESSING en cours
    if (_state == state::PROCESSING) {
        if (!_initMillisForTimeout) {
            // premiers car. reçus pour la commande en cours
            _initMillisForTimeout = true;
            _oldMillis = millis();              // init. _oldMillis
        }
        else {
            // Contrôle du timeout
            if (millis() > (_oldMillis + (unsigned long)_timeout)) {
                _state = state::ERROR_TIMEOUT;  // Entre autre, ne test plus timeout
                sm.change(eSm::TIMEOUT);
            }
            else {
                if (receive) {                  // car. reçu dans cet appel => maj _oldMillis
                    _oldMillis = millis();
                }
            }
        }
    }
    return _state;
}

//==============================================================================
// © cZord-IbouX 2019-2021
// 29.6.21
//==============================================================================
String SerialCommandRead::read() {
    sm.change(eSm::WAIT_CHAR);   // Redémarre l'analyse au prochain available()
    return _command;
}

//==============================================================================
// © cZord-IbouX 2019-2021
// 29.6.21
//==============================================================================
void SerialCommandRead::print(String command) {
    _pSerial->print(_start);
    _pSerial->print(command);
    _pSerial->print(_end);
}



#pragma endregion


