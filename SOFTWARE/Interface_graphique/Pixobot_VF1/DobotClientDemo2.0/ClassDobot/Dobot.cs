using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using DobotClientDemo.CPlusDll;
using ObjDobot;

namespace DobotAPI
{
    sealed class Dobot
    {

        #region INSTANCE

        private static Dobot instance;

        private Arm arm;
        private SuctionCup suc;
        //private Gripper grippe;
        private List<ConveyorBelt> conveyorBeltLst;
        private List<ColorSensor> colorSensorLst;
        private List<LaserSensor> laserSensorLst;
        private FileDobot file;

        private const byte BYTE_ERREUR = 255;
        private const float FLOAT_ERREUR = 0.0F;

        public string[] tabCommands;
        private int NextExecuted;
        private Dobot()
        {
            conveyorBeltLst = new List<ConveyorBelt>();
            colorSensorLst = new List<ColorSensor>();
            laserSensorLst = new List<LaserSensor>();
        }

        public static Dobot GetInstance() // Crée une instance dans le cas ou il y en à pas
        {
            if (instance == null)
            {
                instance = new Dobot();
            }
            return instance;
        }

        public void CreateRobotArm() // Check la connexion du bras juste apres avoir crée l'instance
        {
            if (arm == null)
            {
                arm = new Arm();
            }
        }

        public bool TurnOffALL()
        {
            bool arretOk = true;

            if (arm != null)
            {
                if (!arm.Move(JogCmdType.Idle))
                {
                    arretOk = false;
                }
            }

            if (suc != null)
            {
                if (suc.IsEnable)
                {
                    if (!suc.Release())
                    {
                        arretOk = false;
                    }
                }
            }

            //if (grippe != null)
            //{
            //    if (grippe.IsEnable)
            //    {
            //        if (!grippe.Release())
            //        {
            //            arretOk = false;
            //        }
            //    }
            //}

            for (int i = 0; i < conveyorBeltLst.Count; i++)
            {
                if (conveyorBeltLst[i].IsEnable)
                {
                    if (!conveyorBeltLst[i].TurnOff())
                    {
                        arretOk = false;
                    }
                }
            }

            for (int i = 0; i < colorSensorLst.Count; i++)
            {
                if (colorSensorLst[i].IsEnable)
                {
                    if (!colorSensorLst[i].TurnOff())
                    {
                        arretOk = false;
                    }
                }
            }

            for (int i = 0; i < laserSensorLst.Count; i++)
            {
                if (laserSensorLst[i].IsEnable)
                {
                    if (!laserSensorLst[i].TurnOff())
                    {
                        arretOk = false;
                    }
                }
            }

            return arretOk;
        } // Eteint les objets mais ne les deconnects pas

        private void DisconnectObj()
        {

            if (arm != null)
            {
                arm = null;
            }

            if (suc != null)
            {
                suc = null;
            }

            //if (grippe != null)
            //{
            //    grippe = null;
            //}

            for (int i = 0; i < conveyorBeltLst.Count; i++)
            {
                conveyorBeltLst.RemoveAt(i);
            }

            for (int i = 0; i < colorSensorLst.Count; i++)
            {
                colorSensorLst.RemoveAt(i);
            }

            for (int i = 0; i < laserSensorLst.Count; i++)
            {
                laserSensorLst.RemoveAt(i);
            }

        } // Faire ça avant de déconnecter le bras sinon on les objets allumer ne pourrons plus être éteints (éteint et déconnect tout les objets)

        public bool DisconnectALL()
        {
            bool ret = false;

            if (TurnOffALL())
            {
                ret = true;
            }

            DobotDll.SetQueuedCmdClear();
            DisconnectObj();
            DobotDll.DisconnectDobot();

            IsConnected = false;

            return ret;
        }

        public void CreateSuctionCup()
        {
            if (suc == null)
            {
                suc = new SuctionCup();
            }
        }

        public void SuctionCupDisconnect()
        {
            suc = null;
        }

        //public void CreateGripper()
        //{
        //    if (grippe == null)
        //    {
        //        grippe = new Gripper();
        //    }
        //}

        //public void GripperDisconnect()
        //{
        //    grippe = null;
        //}

        public enum BeltPort { STEPPERPORT_1, STEPPERPORT_2, PAS_DE_PORT = 255 };
        public ConveyorBelt CreateConveyorBelt(BeltPort stepperPort)
        {
            if (conveyorBeltLst.Count < 2)
            {
                ConveyorBelt belt = new ConveyorBelt((ConveyorBelt.StepperPort)stepperPort);

                conveyorBeltLst.Add(belt);

                return belt;
            }
            return null;
        }

        public void BeltDisconnect(int id = 0) // Déconnonecté les appereils dans l'ordre sinon Belt1 passe en Belt0
        {
            if (conveyorBeltLst.Count <= id)
            {
                return;
            }
            conveyorBeltLst.RemoveAt(id);
        }

        public enum GPort { GP1, GP2, GP4, GP5, PAS_DE_PORT = 255 };
        public ColorSensor CreateColorSensor(GPort port)
        {
            if (colorSensorLst.Count < 2)
            {
                ColorSensor colorSensor = new ColorSensor((Sensor.GPort)port);
                colorSensorLst.Add(colorSensor);

                return colorSensor;
            }
            return null;
        }

        public void ColorSensorDisconnect(int id = 0) // Déconnonecté les appereils dans l'ordre sinon Color1 passe en Color0
        {
            if (colorSensorLst.Count <= id)
            {
                return;
            }
            colorSensorLst.RemoveAt(id);
        }

        public LaserSensor CreateLaserSensor(GPort port)
        {
            if (laserSensorLst.Count < 2)
            {
                LaserSensor laserSensor = new LaserSensor((Sensor.GPort)port);
                laserSensorLst.Add(laserSensor); // .Add ajoute une fonction mais les positions change (peut-etre résolu avec tableau ou autre(jsp))

                return laserSensor;
            }
            return null;
        }

        public void LaserSensorDisconnect(int id = 0) // Déconnonecté les appereils dans l'ordre sinon Laser1 passe en Laser0
        {
            if (laserSensorLst.Count <= id)
            {
                return;
            }
            laserSensorLst.RemoveAt(id);
        }

        //public void CreateFile(string fileName)
        //{
        //    if (file == null)
        //    {
        //        file = new FileDobot(fileName);
        //    }
        //}

        #endregion

        #region BRAS

        #region CONNEXTION

        public bool IsConnected {
            get;
            private set;
        }

        public void Connect()
        {

            if (CheckConnection())
            {
                IsConnected = true;
                return;
            }

            StringBuilder fwType = new StringBuilder(60);
            StringBuilder version = new StringBuilder(60);

            int ret = DobotDll.ConnectDobot("", 115200, fwType, version);

            if (ret != (int)DobotConnect.DobotConnect_NoError)
            {
                IsConnected = false;
                return;
            }

            DobotDll.SetCmdTimeout(3000);

            ///get device name and device Serial Number
            string deviceName = "Dobot Magician";
            DobotDll.SetDeviceName(deviceName);
            StringBuilder deviceSN = new StringBuilder(64);
            DobotDll.GetDeviceName(deviceSN, 64);

            ///clear queue and start executing queue
            DobotDll.SetQueuedCmdClear();
            DobotDll.SetQueuedCmdStartExec();

            IsConnected = true;
            return;
        }

        public bool CheckConnection() // à mettre dans une boucle pour tester si le Dobot est toujours connecté
        {
            if (arm == null || !IsConnected)
            {
                return false;
            }

            IsConnected = DobotDll.SearchDobot(new StringBuilder("COM1"), 1000) != 0;
            return IsConnected;
        }

        #endregion

        #region CALIBRAGE

        public bool ArmSetCoordCalibrage(float x, float y, float z, float r)
        {
            return CheckConnection() && arm.SetHOMEParams(x, y, z, r);
        }

        /// <summary></summary>
        public bool ArmCalibrage()
        {
            return CheckConnection() && arm.SetHOMECmd();
        }

        /// <summary></summary>
        public bool ArmAutoLeveling() // Calibrage spéciale... S'informer encore
        {
            return CheckConnection() && arm.SetAutoLevelingCmd();
        }

        public bool Reboot()
        {
            return arm.AlarmClear();
        }

        #endregion

        #region GET/SET

        public float ArmGetJump()
        {
            return !CheckConnection() ? FLOAT_ERREUR : arm.Jump;
        }

        public bool ArmSetJump(float Height) // Jump de toute façon PTP, PTP = mode commande pour mettre des coordonnées
        {
            if (!CheckConnection())
            {
                return false;
            }
            arm.Jump = Height;
            return true;

        }

        public float ArmGetVelocity()
        {
            return !CheckConnection() ? FLOAT_ERREUR : arm.Velocity;
        }

        public bool ArmSetVelocity(float velocity) // Sans PTP = mode normale sans mettre de coordonnées
        {
            if (!CheckConnection())
            {
                return false;
            }
            arm.Velocity = velocity;
            return true;

        }

        // NE CHANGE PAS LA VITESSE DES COORDONNÉES. IL FAUT LE DEBUG AVANT D'UTILISER
        //public float ArmGetVelocityPTP()
        //{
        //    return !CheckConnection() ? FLOAT_ERREUR : arm.PtpVelocity;
        //}

        //public bool ArmSetVelocityPTP(float velocityPTP)
        //{
        //    if (!CheckConnection())
        //    {
        //        return false;
        //    }
        //    arm.PtpVelocity = velocityPTP;
        //    return true;
        //}

        public float ArmGetAcceleration()
        {
            return !CheckConnection() ? FLOAT_ERREUR : arm.Acceleration;
        }

        public bool ArmSetAcceleration(float acceleration)
        {
            if (!CheckConnection())
            {
                return false;
            }
            arm.Acceleration = acceleration;
            return true;
        }

        // NE CHANGE PAS L'ACCELERATION DES COORDONNÉES. IL FAUT LE DEBUG AVANT D'UTILISER
        //public float ArmGetAccelerationPTP()
        //{
        //    return !CheckConnection() ? FLOAT_ERREUR : arm.PtpAcceleration;
        //}

        //public bool ArmSetAccelerationPTP(float accelerationPTP)
        //{
        //    if (!CheckConnection())
        //    {
        //        return false;
        //    }
        //    arm.PtpAcceleration = accelerationPTP;
        //    return true;
        //}

        public enum ArmModeCmd { Jump_Arc, Arc, Line };
        public bool ArmSetMode(ArmModeCmd modeCmd)
        {
            if (!CheckConnection())
            {
                return false;
            }
            arm.CoordinateMode = (Arm.Mode)modeCmd;
            return true;
        }

        // Faire des Set si besoin

        public float ArmGetX()
        {
            return !CheckConnection() ? FLOAT_ERREUR : arm.X;
        }

        public float ArmGetY()
        {
            return !CheckConnection() ? FLOAT_ERREUR : arm.Y;
        }

        public float ArmGetZ()
        {
            return !CheckConnection() ? FLOAT_ERREUR : arm.Z;
        }

        public float ArmGetRHead()
        {
            return !CheckConnection() ? FLOAT_ERREUR : arm.RHead;
        }

        #endregion

        #region MOUVEMENT

        public enum Axe
        {
            Idle,
            X_Plus,
            X_Minus,
            Y_Plus,
            Y_Minus,
            Z_Plus,
            Z_Minus,
            R_Plus,
            R_Minus,
            L_Plus,
            L_Minus
        };
        public bool ArmMove(Axe axe)
        {
            return CheckConnection() && arm.Move((JogCmdType)axe);
        }

        public void GoCoordinateXYZR(float x, float y, float z, float rHead)
        {
            if (CheckConnection())
            {
                arm.CoordinateXYZR(x, y, z, rHead);
            }
        }

        #endregion

        #endregion

        #region MAIN

        public bool SuctionCupCatch()
        {
            return suc.Catch();
        }

        //public bool GripperCatch() // ALLER DANS LA CLASS HAND => GRIPPER VOUS INFORMER POUR L'UTILISATION
        //{
        //    return grippe.Catch();
        //}

        public bool SuctionCupRelease()
        {
            return suc.Release();
        }

        //public bool GripperRelease()
        //{
        //    return grippe.Release();
        //}

        public bool SuctionCupGetStatus()
        {
            return suc.IsEnable;
        }

        //public bool GripperGetStatus()
        //{
        //    return grippe.IsEnable;
        //}

        #endregion

        #region TAPIS

        #region GET/SET

        public bool BeltGetStatus(int id = 0)
        {
            return conveyorBeltLst.Count > id && conveyorBeltLst[id].IsEnable;
        }

        public bool BeltGetDistanceStatus(int id = 0)
        {
            return conveyorBeltLst.Count > id && conveyorBeltLst[id].DistanceIsEnable;
        }

        public byte BeltGetStepperPort(int id = 0)
        {
            return conveyorBeltLst.Count <= id ? BYTE_ERREUR : (byte)conveyorBeltLst[id].Port;
        }

        public bool BeltSetStepperPort(BeltPort port, int id = 0)
        {
            if (conveyorBeltLst.Count <= id)
            {
                return false;
            }
            conveyorBeltLst[id].Port = (ConveyorBelt.StepperPort)port;
            return true;
        }

        public float BeltGetSpeed(int id = 0)
        {
            return conveyorBeltLst.Count <= id ? FLOAT_ERREUR : conveyorBeltLst[id].Speed;
        }

        public bool BeltSetSpeed(int speed, int id = 0)
        {
            if (conveyorBeltLst.Count <= id)
            {
                return false;
            }
            conveyorBeltLst[id].Speed = speed;
            return true;
        }

        public float BeltGetDistanceSBS(int id = 0) // Distance en Step Bye Step
        {
            return conveyorBeltLst.Count <= id ? FLOAT_ERREUR : conveyorBeltLst[id].DistanceSbS;
        }

        public bool BeltSetDistanceSBS(int distanceSBS, int id = 0)
        {
            if (conveyorBeltLst.Count <= id)
            {
                return false;
            }
            conveyorBeltLst[id].DistanceSbS = distanceSBS;
            return true;
        }

        public float BeltGetDistanceMM(int id = 0) // Disance en milimètre
        {
            return conveyorBeltLst.Count <= id ? FLOAT_ERREUR : conveyorBeltLst[id].DistanceMM;
        }

        public bool BeltSetDistanceMM(int distanceMM, int id = 0)
        {
            if (conveyorBeltLst.Count <= id)
            {
                return false;
            }
            conveyorBeltLst[id].DistanceMM = distanceMM;
            return true;
        }

        public bool BeltGetDirectionSTD(int id = 0) // Direction Standard
        {
            return conveyorBeltLst[id].DirectionStd;
        }

        public bool BeltSetDirectionSTD(bool directionSTD, int id = 0)
        {
            if (conveyorBeltLst.Count <= id)
            {
                return false;
            }
            conveyorBeltLst[id].DirectionStd = directionSTD;
            return true;
        }


        #endregion

        #region ENCLECHEMENT

        public bool BeltTurnON(int id = 0)
        {
            return conveyorBeltLst.Count > id && conveyorBeltLst[id].TurnOn();
        }

        public bool BeltTurnOFF(int id = 0)
        {

            return conveyorBeltLst.Count > id && conveyorBeltLst[id].TurnOff();
        }

        public bool BeltDistanceON(int id = 0)
        {
            return conveyorBeltLst.Count > id && conveyorBeltLst[id].DistanceOn();
        }

        #endregion

        #endregion

        #region CAPTEURCOULEUR

        #region GET

        public bool ColorSensorGetStatus(int id = 0)
        {
            return colorSensorLst.Count > id && colorSensorLst[id].IsEnable;
        }

        public byte ColorSensorGetPort(int id = 0)
        {
            return colorSensorLst.Count <= id ? BYTE_ERREUR : (byte)colorSensorLst[id].Port;
        }

        public byte ColorSensorGetSensorInByte(int id = 0)
        {
            return colorSensorLst.Count <= id ? BYTE_ERREUR : colorSensorLst[id].GetSensor();
        }

        //A mettre dans Capteur Couleur
        public string ColorSensorGetSensorInString(int id = 0) // Si on veut un fonction qui retourne la couleur en text du capteur de couleur. id == 0 : capt1, id = 1 : capt2
        {
            Dictionary<int, string> colorDic = new Dictionary<int, string>() { { 1, "red" }, { 2, "green" }, { 3, "blue" }, { 255, "ERREUR" } };

            return colorSensorLst.Count <= id ? "ERREUR" : colorDic[colorSensorLst[id].GetSensor()];
        }

        #endregion

        #region ENCLENCHEMENT

        public bool ColorSensorTurnON(int id = 0)
        {
            if (colorSensorLst.Count <= id)
            {
                return false;
            }
            return colorSensorLst[id].TurnOn();
        }

        public bool ColorSensorTurnOFF(int id = 0)
        {
            if (colorSensorLst.Count <= id)
            {
                return false;
            }
            return colorSensorLst[id].TurnOff();
        }

        #endregion

        #endregion

        #region CAPTEURLASER

        #region GET

        public bool LaserSensorGetStatus(int id = 0)
        {
            return laserSensorLst.Count > id && laserSensorLst[id].IsEnable;
        }

        public byte LaserSensorGetPort(int id = 0)
        {
            return laserSensorLst.Count <= id ? BYTE_ERREUR : (byte)laserSensorLst[id].Port;
        }

        public byte LaserSensorGetSensor(int id = 0)
        {
            return laserSensorLst.Count <= id ? BYTE_ERREUR : laserSensorLst[id].GetSensor();
        }

        #endregion

        #region ENCLENCHEMENT

        public bool LaserSensorTurnON(int id = 0)
        {
            if (laserSensorLst.Count <= id)
            {
                return false;
            }
            return laserSensorLst[id].TurnOn();
        }

        public bool LaserSensorTurnOFF(int id = 0)
        {
            if (laserSensorLst.Count <= id)
            {
                return false;
            }
            return laserSensorLst[id].TurnOff();
        }

        #endregion

        #endregion

        #region FILE

        public void FileSave(string name, RichTextBox richTextBox)
        {
            FileDobot.Create(name, richTextBox);
        }

        public void OpenFileWindow() // Ouvre l'explorateur de fichier, enregistre le dossier selectionner
        {
            file = FileDobot.OpenFileWindow();
        }

        public bool LoadFile()
        {
            //Prendre les commands qui sont dans le fichier
            if (file == null || file.Name == null)
            {
                return false;
            }

            tabCommands = file.GetCommands();

            for (int i = 0; i < tabCommands.Length; i++)
            {
                tabCommands[i] = tabCommands[i].ToLower();
            }

            //demarrer notre compteur pour savoir quelle est la prochaine commande à executer
            NextExecuted = 0;
            return true;
        }

        public string FileName()
        {
            return file == null || file.Name == null ? "" : file.Name;
        }

        private bool enter = false;
        private bool infini = false;
        private int boucle = 0;
        private int fin = 0;

        public void CommandeRestart()
        {
            NextExecuted = 0;
            boucle = 0;
            fin = 0;
        }

        public enum Commande // TOUTES LES COMMANDES POSSIBLES
        {
            ERREUR,
            EMPTY,
            ARM_MOVE,
            ARM_UP_DEFAULT,
            ARM_UP_PARAM,
            ARM_DOWN_DEFAULT,
            ARM_DOWN_PARAM,
            ARM_MODE,
            HAND_CATCH,
            HAND_RELEASE,
            BELT_STOP_AT_SENSROR,
            BELT_DISTANCE_DEFAULT,
            BELT_DISTANCE_PARAM,
            BELT_DIRECTION_DEFAULT,
            BELT_DIRECTION_PARAM,
            COLORSENSOR_ALLUMER,
            COLORSENSOR_ETEINT,
            IF_COLOR,
            END_IF,
            ELSE_COLOR,
            END_ELSE,
            WAIT_DEFAULT,
            WAIT_PARAM,
            WHILE_INFINI,
            WHILE_PARAM, // METTRE AU DEBUT DU FICHIER UNIQUEMENT
            END_WHILE   // METTRE A LA FIN DU FICHIER UNIQUEMENT
        }

        public Commande AnalyseCommand(string TabCommands) // EN COURS DE PRODUICTION...
        {
            string[] tabAnalyseCommande = TabCommands.Split(' ');

            switch (tabAnalyseCommande[0])
            {
                case "arm":

                    if (tabAnalyseCommande.Length > 1)
                    {
                        if (tabAnalyseCommande[1] == "move")
                        {
                            if (tabAnalyseCommande.Length == 6)
                            {
                                if (float.TryParse(tabAnalyseCommande[2], out _) && float.TryParse(tabAnalyseCommande[3], out _) && float.TryParse(tabAnalyseCommande[4], out _) && float.TryParse(tabAnalyseCommande[5], out _))
                                {
                                    if (double.Parse(tabAnalyseCommande[2]) >= -120 && double.Parse(tabAnalyseCommande[2]) <= 300 && double.Parse(tabAnalyseCommande[3]) >= -300 && double.Parse(tabAnalyseCommande[3]) <= 300) // limide coordonée a rentrer pour l'axe X et Y
                                    {
                                        if (Math.Sqrt(Math.Pow(double.Parse(tabAnalyseCommande[2]), 2) + Math.Pow(double.Parse(tabAnalyseCommande[3]), 2)) >= 200) // minimum du résultat de l'équation
                                        {
                                            if (Math.Sqrt(Math.Pow(double.Parse(tabAnalyseCommande[2]), 2) + Math.Pow(double.Parse(tabAnalyseCommande[3]), 2)) <= 300) // maximum du résultat de l'équation
                                            {
                                                if (double.Parse(tabAnalyseCommande[4]) >= -60 && double.Parse(tabAnalyseCommande[4]) <= 120) // limide coordonée a rentrer pour l'axe Z
                                                {
                                                    if (double.Parse(tabAnalyseCommande[5]) >= -140 && double.Parse(tabAnalyseCommande[5]) <= 140)  // limide coordonée a rentrer pour l'axe R
                                                    {
                                                        return Commande.ARM_MOVE;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (tabAnalyseCommande[1] == "up")
                        {
                            if (tabAnalyseCommande.Length == 2) // limite de taille de la commande
                            {
                                return Commande.ARM_UP_DEFAULT;
                            }
                            else if (tabAnalyseCommande.Length == 3)
                            {
                                if (float.TryParse(tabAnalyseCommande[2], out _)) // LETTRE EN PARAMETRE PAS ACCEPTÉ
                                {
                                    if (float.Parse(tabAnalyseCommande[2]) > 0 && float.Parse(tabAnalyseCommande[2]) < 60)
                                    {
                                        return Commande.ARM_UP_PARAM;
                                    }
                                }
                            }
                        }
                        else if (tabAnalyseCommande[1] == "down")
                        {
                            if (tabAnalyseCommande.Length == 2)
                            {
                                return Commande.ARM_DOWN_DEFAULT;
                            }
                            else if (tabAnalyseCommande.Length == 3)
                            {
                                if (float.TryParse(tabAnalyseCommande[2], out _)) // LETTRE EN PARAMETRE PAS ACCEPTÉ GERER ERREUR
                                {
                                    if (float.Parse(tabAnalyseCommande[2]) > 0 && float.Parse(tabAnalyseCommande[2]) < 60)
                                    {
                                        return Commande.ARM_DOWN_PARAM;
                                    }
                                }
                            }
                        }
                        else if (tabAnalyseCommande[1] == "mode")
                        {
                            if (tabAnalyseCommande.Length == 3) // limite de taille de la commande
                            {
                                if (tabAnalyseCommande[2] == "arc" || tabAnalyseCommande[2] == "jump_arc" || tabAnalyseCommande[2] == "line")
                                {
                                    return Commande.ARM_MODE;
                                }
                            }
                        }
                    }
                    break;

                case "hand":

                    if (tabAnalyseCommande.Length == 2)  // limite de taille de la commande
                    {
                        if (tabAnalyseCommande[1] == "catch")
                        {
                            return Commande.HAND_CATCH;
                        }
                        else if (tabAnalyseCommande[1] == "release")
                        {
                            return Commande.HAND_RELEASE;
                        }
                    }
                    break;

                case "belt":

                    if (tabAnalyseCommande.Length > 1)
                    {
                        if (tabAnalyseCommande[1] == "stop_at_sensor")
                        {
                            if (tabAnalyseCommande.Length == 2)
                            {
                                return Commande.BELT_STOP_AT_SENSROR;
                            }
                        }
                        else if (tabAnalyseCommande[1] == "distance")
                        {

                            if (tabAnalyseCommande.Length == 2)
                            {
                                return Commande.BELT_DISTANCE_DEFAULT;
                            }
                            else if (tabAnalyseCommande.Length == 3)
                            {
                                if (float.TryParse(tabAnalyseCommande[2], out _))
                                {
                                    if (float.Parse(tabAnalyseCommande[2]) > 10 && float.Parse(tabAnalyseCommande[2]) < 600) // Min distance = 10, Max distance = 600
                                    {
                                        return Commande.BELT_DISTANCE_PARAM;
                                    }
                                }
                            }
                        }
                        else if (tabAnalyseCommande[1] == "direction")
                        {
                            if (tabAnalyseCommande.Length == 2)
                            {
                                return Commande.BELT_DIRECTION_DEFAULT;
                            }
                            else if (tabAnalyseCommande.Length == 3)
                            {
                                if (tabAnalyseCommande[2] == "left" || tabAnalyseCommande[2] == "right")
                                {
                                    return Commande.BELT_DIRECTION_PARAM;
                                }
                            }
                        }
                    }
                    break;

                case "colorsensor":

                    if (tabAnalyseCommande.Length == 2)
                    {
                        if (tabAnalyseCommande[1] == "on")
                        {
                            return Commande.COLORSENSOR_ALLUMER;
                        }
                        else if (tabAnalyseCommande[1] == "off")
                        {
                            return Commande.COLORSENSOR_ETEINT;
                        }
                    }

                    break;

                case "if_color":

                    if (tabAnalyseCommande.Length == 2)
                    {
                        if (tabAnalyseCommande[1] == "red" || tabAnalyseCommande[1] == "green" || tabAnalyseCommande[1] == "blue")
                        {
                            return Commande.IF_COLOR;
                        }
                    }

                    break;

                case "end_if":

                    if (tabAnalyseCommande.Length == 1)
                    {
                        return Commande.END_IF;
                    }
                    break;

                case "else_color":

                    if (tabAnalyseCommande.Length == 1)
                    {
                        return Commande.ELSE_COLOR;
                    }
                    break;

                case "end_else":

                    if (tabAnalyseCommande.Length == 1)
                    {
                        return Commande.END_ELSE;
                    }
                    break;

                case "wait":

                    if (tabAnalyseCommande.Length == 1)
                    {
                        return Commande.WAIT_DEFAULT;
                    }
                    else if (tabAnalyseCommande.Length == 2)
                    {
                        if (float.TryParse(tabAnalyseCommande[1], out _))
                        {
                            if (int.Parse(tabAnalyseCommande[1]) > 0) // Si il peut pas convertir en int : paramètre faux
                            {
                                return Commande.WAIT_PARAM;
                            }
                        }
                    }
                    break;

                case "while":

                    if (tabAnalyseCommande.Length == 2)
                    {
                        if (tabAnalyseCommande[1] == "infinite")
                        {
                            return Commande.WHILE_INFINI;
                        }
                        else if (float.TryParse(tabAnalyseCommande[1], out _))
                        {
                            if (int.Parse(tabAnalyseCommande[1]) > 0) // Si il peut pas convertir en int : paramètre faux
                            {
                                return Commande.WHILE_PARAM;
                            }
                        }
                    }
                    break;

                case "end_while":

                    if (tabAnalyseCommande.Length == 1)
                    {
                        return Commande.END_WHILE;
                    }
                    break;

                case "":

                    return Commande.EMPTY;

                default:
                    return Commande.ERREUR;
            }
            return Commande.ERREUR;
        }

        public bool ExecuteNextCommand() // VOIR POUR TOUT RENDRE EN MINUSCULE ET SANS ACCENT
        {
            if (file == null || file.Name == null || NextExecuted >= tabCommands.Length)
            {
                return false;
            }

            //Prendre la commande à executer et la spliter
            string[] Commands = tabCommands[NextExecuted].Split(' ');

            switch (AnalyseCommand(tabCommands[NextExecuted]))
            {
                case Commande.ARM_MOVE:

                    arm.CoordinateXYZR(float.Parse(Commands[2]), float.Parse(Commands[3]), float.Parse(Commands[4]), float.Parse(Commands[5]));

                    break;

                case Commande.ARM_UP_DEFAULT:

                    arm.CoordinateXYZR(arm.X, arm.Y, arm.Z + 5, arm.RHead);

                    break;

                case Commande.ARM_UP_PARAM:

                    arm.CoordinateXYZR(arm.X, arm.Y, arm.Z + float.Parse(Commands[2]), arm.RHead);

                    break;

                case Commande.ARM_DOWN_DEFAULT:

                    arm.CoordinateXYZR(arm.X, arm.Y, arm.Z - 5, arm.RHead);

                    break;

                case Commande.ARM_DOWN_PARAM:

                    arm.CoordinateXYZR(arm.X, arm.Y, arm.Z + -float.Parse(Commands[2]), arm.RHead);

                    break;

                case Commande.ARM_MODE:

                    Dictionary<string, Arm.Mode> modeArmDic = new Dictionary<string, Arm.Mode>() { { "jump_arc", Arm.Mode.Jump_Arc }, { "arc", Arm.Mode.Arc }, { "line", Arm.Mode.Line } };

                    arm.CoordinateMode = modeArmDic[Commands[2]];

                    break;

                case Commande.HAND_CATCH:

                    if (!suc.Catch())
                    {
                        return false;
                    }

                    break;

                case Commande.HAND_RELEASE:

                    if (!suc.Release())
                    {
                        return false;
                    }

                    break;

                case Commande.BELT_STOP_AT_SENSROR:

                    bool state = false;

                    if (laserSensorLst[0].GetSensor() == 0)
                    {
                        if (!conveyorBeltLst[0].TurnOn())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        state = true;
                    }

                    while (!state)
                    {
                        if (laserSensorLst[0].GetSensor() != 0)
                        {
                            if (!conveyorBeltLst[0].TurnOff())
                            {
                                return false;
                            }
                            state = true;
                        }
                    }

                    break;

                case Commande.BELT_DISTANCE_DEFAULT:

                    conveyorBeltLst[0].DistanceMM = 100;

                    break;

                case Commande.BELT_DISTANCE_PARAM:

                    conveyorBeltLst[0].DistanceMM = int.Parse(Commands[2]);

                    break;

                case Commande.BELT_DIRECTION_DEFAULT:

                    conveyorBeltLst[0].DirectionStd = true;

                    break;

                case Commande.BELT_DIRECTION_PARAM:
                    Dictionary<string, bool> directionBeltDic = new Dictionary<string, bool>() { { "droite", true }, { "gauche", false } };

                    conveyorBeltLst[0].DirectionStd = directionBeltDic[Commands[2]];

                    break;

                case Commande.COLORSENSOR_ALLUMER:

                    if (!colorSensorLst[0].TurnOn())
                    {
                        return false;
                    }
                    Thread.Sleep(1000); // Laisser 1000ms minimum allumer pour lire la couleur (A TESTER LE TEMPS)

                    break;

                case Commande.COLORSENSOR_ETEINT:

                    if (!colorSensorLst[0].TurnOff())
                    {
                        return false;
                    }

                    break;

                case Commande.IF_COLOR:

                    if (Commands[1] == ColorSensorGetSensorInString())
                    {
                        enter = true;
                    }
                    else
                    {
                        while (Commands[0] != "else_color" && Commands[0] != "end_if")
                        {
                            NextExecuted++; // lire la ligne suivante du tableau
                            Commands = tabCommands[NextExecuted].Split(' '); // crée une fonction car répétition de code
                        }
                    }

                    break;

                case Commande.ELSE_COLOR:

                    if (enter)
                    {
                        while (Commands[0] != "end_else")
                        {
                            NextExecuted++;
                            Commands = tabCommands[NextExecuted].Split(' '); // A TESTER
                        }
                    }
                    enter = false;

                    break;

                case Commande.WAIT_DEFAULT:

                    Thread.Sleep(1000);

                    break;

                case Commande.WAIT_PARAM:

                    Thread.Sleep(int.Parse(Commands[1]));

                    break;

                case Commande.WHILE_INFINI:

                    infini = true;

                    break;

                case Commande.WHILE_PARAM:

                    fin = int.Parse(Commands[1]);

                    boucle++;

                    break;

                case Commande.END_WHILE:

                    if (fin <= boucle && !infini)
                    {
                        return false; // met fin à la lecture
                    }

                    NextExecuted = -1;

                    break;

                default:
                    //return false;
                    break;
            }

            //Mettre a jour la variable pour savoir quelle est la prochaine commande a executer
            NextExecuted++;

            //si nous sommes arrivés à la fin de la liste de commandes, retourner false
            return NextExecuted != tabCommands.Length;
        }

        #endregion

    }
}