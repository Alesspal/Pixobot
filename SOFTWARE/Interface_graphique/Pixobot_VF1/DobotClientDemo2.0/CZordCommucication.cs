using System;
using System.IO.Ports;
using System.Windows.Controls;
using System.Windows.Threading;
using TermiZordLib;

namespace DobotClientDemo
{
    class CZordCommucication
    {
        #region Attributs
        private static CZordCommucication instance;

        // Pour la gestion du port série (connexion, communication, COMMAND)
        private readonly SerialPort _serialPort;
        private readonly CZord_GuiSerialConnect _guiSerialConnect;
        private readonly CZord_SerialCommand _serialCommand;

        // Indique si un port série est connecté
        public bool _flagSerialPortConnected;

        // Pour le polling des commandes reçues
        private readonly DispatcherTimer _tmr_PollingRx;

        // Pour les paramètres nécessaires à l'utilisation de la librairie
        public struct Settings
        {
            public string startOfCommand;
            public string endOfCommand;
            public int timeout;
            public int pollingInterval;
            public int scanPortInterval;
        }
        public Settings _settings;

        public Canvas _cnv_SerialConnect;

        // constante string erreur
        private const string WAITING = "waiting";   //7
        private const string PROCESSING = "processing"; //10
        private const string ERROR_NO_START = "error_no_start"; //14
        private const string ERROR_TIME_OUT = "error_time_out"; //14
        private const string ERROR_PORT = "error_port"; //10
        private const string ERROR_NOT_DEFINED = "error_not_defined";   //17

        public enum Command
        {
            COORD_BLOC = 5,
            NB_OF_BLOCS_DETECTED = 4,
            TRAIN_HERE = 3,
            TRAIN_NOT_HERE = 2,
            TRAIN_STATE_UNKNOWN = 1,
            WAITING = 0,
            PROCESSING = -1,
            ERROR_NO_START = -2,
            ERROR_TIME_OUT = -3,
            ERROR_PORT = -4,
            UNKNOWN_COMMAND = -5,
            ERROR_NOT_DEFINED = -6,
        }

        const char STATE_OF_TRAIN = 't';
        const char TRAIN_ARIVED = '1';
        const char TRAIN_NOT_ARIVED = '0';
        const char NUMBER = 'n';
        const char BLOCK = 'b';

        #endregion

        #region Initialisation

        /// <summary>
        /// Toujours construire le premier objet CZordCommunication
        /// avec son paramètre. Si besoin de créer plusieurs objets
        /// à d'autres endroits, pas besoin de remettre son paramètre
        /// </summary>
        public static CZordCommucication GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// Construction de l'objet si l'instance est null,
        /// sinon reprend l'instance de l'objet déjà créé
        /// </summary>
        /// <param name="cnv_SerialConnect"></param>
        /// <returns> l'instance de l'objet </returns>
        public static CZordCommucication GetInstance(Canvas cnv_SerialConnect)
        {
            if (instance == null)   // si aucune instance de cette objet a été créé
            {
                instance = new CZordCommucication(cnv_SerialConnect);
            }
            return instance;        // sinon reprend l'instance de l'objet créé
        }


        private CZordCommucication(Canvas cnv_SerialConnect)
        {
            // Paramètres nécessaires pour la gestion de la connexion série et du mode COMMAND
            // ------------------------------------------------------------------------------------------------------------
            // ATTENTION, pollingInterval est utilisé pour mettre à jour l'affichage de l'application (commande reçue) et
            //            demande du temps au système, donc éviter les valeurs trop faibles
            _settings.startOfCommand = "@";             // Chaîne start, peut être ""                            
            _settings.endOfCommand = "\r\n";            // Chaîne end, doit être <> "" (mode spécial non décrit ici si "")
            _settings.timeout = 100;                    // timeout de réception, de 100 à 10000 ms, sinon prend 1000
            _settings.pollingInterval = 200;            // interval d'interrogation du port série en mode auto (timer)
            _settings.scanPortInterval = 2000;          // interval pour la vérification de l'ajout/retrait d'un port série

            // Création du timer pour le polling de la réception
            // ------------------------------------------------------------------------------------------------------------
            // ATTENTION, doit être fait avant l'instanciation de _guiSerialConnect
            // En effet le constructeur de _guiSerialConnect appelle la méthode TzdSerialState qui accède à ce timer
            _tmr_PollingRx = new DispatcherTimer();
            _tmr_PollingRx.Stop();
            _tmr_PollingRx.Interval = new TimeSpan(0, 0, 0, 0, _settings.pollingInterval);     // Interval d'appel de l'événement Tick
            _tmr_PollingRx.Tick += new EventHandler(TimerPolling_Tick);                        // Abonnement à l'événement du timer

            _tmr_PollingRx.Start();

            // Gestion du port série et initialisation du mode COMMAND
            // ------------------------------------------------------------------------------------------------------------
            _serialPort = new SerialPort();
            // Création, dans le canvas, des objets nécessaires pour la gestion du port COM
            _cnv_SerialConnect = cnv_SerialConnect;
            _guiSerialConnect = new CZord_GuiSerialConnect(_cnv_SerialConnect, _serialPort, TzdSerialState);
            _serialCommand = new CZord_SerialCommand(_serialPort, _settings.startOfCommand, _settings.endOfCommand, _settings.timeout);
            _serialCommand.FlushSerialInAndRestart();
            // Temps de scan du port série (pour détecter ajout/retrait de port COM)
            _guiSerialConnect.SetScanPortInterval(_settings.scanPortInterval);
        }

        #endregion

        #region TimerPolling, RECEPTION & AFFICHAGE des données reçues, ENVOI

        private void TimerPolling_Tick(object sender, EventArgs e)
        {
            _tmr_PollingRx.Stop();          // Peut-il se rappeler lui-même si traitement ici  plus long que l'interval ?
                                            // Si port déconnecté (retrait), n'est pas réactivé
            if (_flagSerialPortConnected)   // Port toujours connecté (flag maj par TzdSerialState)
            {
                SerialCheckAvailableAndRead();
                _tmr_PollingRx.Start();
            }
        }

        // RECEPTION DES DONNEES
        // ------------------------------------------------------------------------------------------------------------
        // - Test si Available, lecture et affichage des données
        // - Appelée par timer ou bouton (manuel)
        // - Démonstration de la gestion de toutes les erreurs
        // - Affichage de la commande sans traitement
        //   ATTENTION aux remarques concernant les problèmes de polling et maj de l'affichage décris ailleurs
        public string SerialCheckAvailableAndRead()
        {
            switch (_serialCommand.Available())
            {

                case CZord_SerialCommand.CommandStates.TO_READ:
                    // Affiche la commande reçue => |command|
                    return _serialCommand.Read();

                case CZord_SerialCommand.CommandStates.CHAR_BEFORE_START:
                    // Affiche la commande reçue sans les caractères avant le start
                    return _serialCommand.Read(); ;

                case CZord_SerialCommand.CommandStates.WAITING:
                    // Attente d'une commande
                    return WAITING;

                case CZord_SerialCommand.CommandStates.PROCESSING:
                    // Commande en cours de réception
                    return PROCESSING;

                case CZord_SerialCommand.CommandStates.ERROR_NO_START:
                    // Restart lance une nouvelle analyse (Read fait la même chose)
                    _serialCommand.Restart();
                    return ERROR_NO_START;

                case CZord_SerialCommand.CommandStates.ERROR_TIMEOUT:
                    _serialCommand.Restart();
                    return ERROR_TIME_OUT;

                case CZord_SerialCommand.CommandStates.ERROR_SERIAL_PORT:
                    // Erreur du port série (retiré?)
                    _serialCommand.Restart();   // Vide le buffer si une commande était en cours
                    return ERROR_PORT;
                default:
                    return ERROR_NOT_DEFINED;
            }
        }

        /// <summary>
        /// Analyse la donnée reçu
        /// </summary>
        /// <param name="dataReceived"></param>
        /// <returns> la commande </returns>
        public Command AnalyzeCommand(string dataReceived)
        {
            if (dataReceived.Length == 2)
            {
                if (dataReceived[0] == STATE_OF_TRAIN)    // A TESTER
                {
                    if (dataReceived[1] == TRAIN_NOT_ARIVED)
                    {
                        return Command.TRAIN_NOT_HERE;
                    }
                    else if (dataReceived[1] == TRAIN_ARIVED)
                    {
                        return Command.TRAIN_HERE;
                    }
                }
            }
            else if (dataReceived.Length > 2)
            {
                if (dataReceived[0] == NUMBER && dataReceived[1] == BLOCK)
                {
                    if (int.TryParse(dataReceived.Substring(2), out _))
                    {
                        return Command.NB_OF_BLOCS_DETECTED;
                    }
                }
                else if (dataReceived.Length == 54)
                {
                    if (dataReceived.Substring(0, 5) == "sig: ")
                    {
                        if (int.TryParse(dataReceived.Substring(5, 3), out _))
                        {
                            if (dataReceived.Substring(9, 3) == "x: ")
                            {
                                if (int.TryParse(dataReceived.Substring(12, 3), out _))
                                {
                                    if (dataReceived.Substring(16, 3) == "y: ")
                                    {
                                        if (int.TryParse(dataReceived.Substring(19, 3), out _))
                                        {
                                            if (dataReceived.Substring(23, 3) == "w: ")
                                            {
                                                if (int.TryParse(dataReceived.Substring(26, 3), out _))
                                                {
                                                    if (dataReceived.Substring(30, 3) == "h: ")
                                                    {
                                                        if (int.TryParse(dataReceived.Substring(33, 3), out _))
                                                        {
                                                            if (dataReceived.Substring(37, 5) == "ind: ")
                                                            {
                                                                if (int.TryParse(dataReceived.Substring(42, 3), out _))
                                                                {
                                                                    if (dataReceived.Substring(46, 5) == "age: ")
                                                                    {
                                                                        if (int.TryParse(dataReceived.Substring(51, 3), out _))
                                                                        {
                                                                            return Command.COORD_BLOC;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (dataReceived.Substring(0) == WAITING)
                {
                    return Command.WAITING;
                }
                else if (dataReceived.Substring(0) == PROCESSING)
                {
                    return Command.PROCESSING;
                }
                else if (dataReceived.Substring(0) == ERROR_NO_START)
                {
                    return Command.ERROR_NO_START;
                }
                else if (dataReceived.Substring(0) == ERROR_TIME_OUT)
                {
                    return Command.ERROR_TIME_OUT;
                }
                else if (dataReceived.Substring(0) == ERROR_PORT)
                {
                    return Command.ERROR_PORT;
                }
                else if (dataReceived.Substring(0) == ERROR_NOT_DEFINED)
                {
                    return Command.ERROR_NOT_DEFINED;
                }
            }
            return Command.UNKNOWN_COMMAND;
        }

        /// <summary>
        /// Envoie la donnée mise en paramètre au port série connecté
        /// </summary>
        /// <param name="data"></param>
        public void WriteCommand(string data)
        {
            _serialCommand.WriteCommand(data);
        }

        #endregion

        #region Modification de l'état du port série

        //  Méthode appelée automatiquement par _guiSerialConnect lors de la modification de l'état du port série
        private void TzdSerialState(CZord_GuiSerialConnect.GuiPortState comPortState)
        {
            // comPortState vaut 
            //  - twzRemoteEventState.CONNECTED                 ==> port connecté
            //  - twzRemoteEventState.DISCONNECTED              ==> port déconnecté
            //                                                      (aussi appelé lors de l'instanciation
            //                                                       de CZord_GuiSerialConnect, au lancement de l'aplication)
            //  - twzRemoteEventState.REMOVED_AND_DISCONNECTED  ==> port retité par un kroumir alors qu'il est connecté 

            if (comPortState == CZord_GuiSerialConnect.GuiPortState.CONNECTED)
            {
                // Port connecté
                _serialCommand.FlushSerialInAndRestart();       // On vide les buffers
                _flagSerialPortConnected = true;                // Pour autoriser la réception de commande (par le timer ou manuelle) 
            }
            else
            {
                // Port déconnecté
                _flagSerialPortConnected = false;               // Indique que la réception n'est plus autorisée
            }
            // Dans tous les cas
            _tmr_PollingRx.IsEnabled = _flagSerialPortConnected;        // Equivalent à Start ou Stop
        }

        #endregion

        #region Fermeture du port série

        /// <summary>
        /// Ferme le port
        /// </summary>
        public void SerialPortClose()
        {
            _guiSerialConnect.Close();
        }

        #endregion
    }
}