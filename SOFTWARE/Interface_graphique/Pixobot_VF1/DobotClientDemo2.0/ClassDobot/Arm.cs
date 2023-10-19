using System;
using DobotClientDemo.CPlusDll;

namespace ObjDobot
{
    sealed class Arm
    {

        #region ATTRIBUTS       

        // Structures de l'api du site Dobot Magicien
        private HOMEParams homeParams;
        private PTPJumpParams jumpParams;
        private JOGCommonParams commonParams;
        private PTPCoordinateParams ptpCoordParams;
        private JogCmd jogCmd;
        private PTPCmd ptpCmd;
        private UInt64 cmdIndex;
        private UInt64 queuedCmdIndex;

        //Gère pas les erreurs de Set pour les property 
        public float Jump {
            get {
                return Get_JumpParams().jumpHeight; // return la variable de la hauteur du saut dans la structure jumpParams
            }

            set {
                SetJumpHeight(value);   // attention à mettre une valeur que le dobot puisse accepter de monter
            }
        }

        public float Velocity {
            get {
                return Get_CommonParams().velocityRatio;
            }
            set {
                SetVelocity(value); // valeur TESTER max = 100
            }
        }
        public float PtpVelocity {
            get {
                return Get_PtpCoordinateParams().xyzVelocity;
            }
            set {
                SetVelocityPTP(value); // valeur TESTER max = 100
            }
        }

        public float Acceleration {
            get {
                return Get_CommonParams().accelerationRatio;
            }
            set {
                SetAcceleration(value); // A tester
            }
        }
        public float PtpAcceleration {
            get {
                return Get_PtpCoordinateParams().xyzAcceleration;
            }
            set {
                SetAccelerationPTP(value);  // A tester
            }
        }

        public enum Mode { Jump_Arc, Arc, Line, }; // Meilleur mode : Arc, Jump_Arc / le mode line ne peux pas toujours aller à une coordonnée. Il se deplace en ligne droite.
        public Mode CoordinateMode {
            get {
                return CoordinateMode;
            }
            set {
                SetMode(value);
            }
        }

        public float X {
            get {
                return (float)Math.Round(Get_Coordinate().x, 1);
            }
        }
        public float Y {
            get {
                return (float)Math.Round(Get_Coordinate().y, 1);
            }
        }
        public float Z {
            get {
                return (float)Math.Round(Get_Coordinate().z, 1);
            }
        }
        public float RHead {
            get {
                return (float)Math.Round(Get_Coordinate().rHead, 1);
            }
        }

        #endregion

        public Arm()
        {

        }

        #region Calibrage

        // Regle la postion de la fin du calibrage
        // Sauvegarde la position dans le dobot
        // Même apres avoir éteint le dobot la position est sauvegarder jusqu'au prochain changement du SetHOMEParams
        public bool SetHOMEParams(float x, float y, float z, float r)
        {
            homeParams.x = x;
            homeParams.y = y;
            homeParams.z = z;
            homeParams.r = r;

            return DobotDll.SetHOMEParams(ref homeParams, false, ref queuedCmdIndex) == (int)DobotCommunicate.DobotCommunicate_NoError;
        }

        public HOMEParams GetHOMEParams()   // Donne la position mise avec le SetHOMEParams
        {
            DobotDll.GetHOMEParams(ref homeParams);
            return homeParams;
        }

        public bool SetHOMECmd() // Execute le calibrage automatique du bras et va à la position donnée au SetHOMEParams
        {
            HOMECmd homeCmd;
            homeCmd.temp = 0; // reserved futur use

            return DobotDll.SetHOMECmd(ref homeCmd, false, ref queuedCmdIndex) == (int)DobotCommunicate.DobotCommunicate_NoError;
        }

        public bool SetAutoLevelingCmd() // Calibrage spéciale... S'informer encore...
        {
            tagAutoLevelingCmd autoLeveling;
            autoLeveling.controlFlag = 1;
            autoLeveling.precision = 0.2F;

            return DobotDll.SetAutoLevelingCmd(ref autoLeveling, false, ref queuedCmdIndex) == (int)DobotCommunicate.DobotCommunicate_NoError;
        }


        // À faire pour avoir la status de l'alarm du robot (se renseigner dans la doc des API du Dobot du site Dobot Magician)
        //public void GetAlarmStatus()
        //{

        //}

        // Mets à jour l'etat de l'alarme
        public bool AlarmClear()
        {
            return DobotDll.ClearAllAlarmsState() == (int)DobotCommunicate.DobotCommunicate_NoError;
        }

        #endregion

        #region MOUVEMENT

        //const float ZLIMIT = 100;
        private bool SetJumpHeight(float height) // Saute à la hauteur donnée et redescend à la coordonnée de base
        {
            jumpParams.jumpHeight = height;
            //jumpParams.zLimit = ZLIMIT; // Voir à quoi ça sert, surement à fixer une hauteur maximum à ne pas dépasser
            return DobotDll.SetPTPJumpParams(ref jumpParams, false, ref queuedCmdIndex) == 0;
        }

        private bool SetVelocity(float speed) // Vitesse du bras quand on le bouge sans coordonnée, testé jusqu'à 100 max mais pas testé plus
        {
            commonParams.velocityRatio = speed;
            return DobotDll.SetJOGCommonParams(ref commonParams, false, ref cmdIndex) == 0;
        }

        private bool SetVelocityPTP(float speed) // Vitesse du bras quand il se déplace de coordonnée en coordonnée, testé jusqu'à 100 max mais pas testé plus
        {
            ptpCoordParams.xyzVelocity = speed;
            ptpCoordParams.rVelocity = speed;
            return DobotDll.SetPTPCoordinateParams(ref ptpCoordParams, false, ref queuedCmdIndex) == 0;
        }

        // Les Accelerations ne marche pas forcement (pas de changment visible a l'oeil). Se renseigner...
        private bool SetAcceleration(float accel) // Acceleration du bras quand on le bouge sans coordonnée (voir la valeur qu'il faut mettre)
        {
            commonParams.accelerationRatio = accel;
            return DobotDll.SetJOGCommonParams(ref commonParams, false, ref cmdIndex) == 0;
        }

        private bool SetAccelerationPTP(float accel) // Acceleration du bras quand il se déplace de coordonnée en coordonnée
        {
            ptpCoordParams.xyzAcceleration = accel;
            ptpCoordParams.rAcceleration = accel;
            return DobotDll.SetPTPCoordinateParams(ref ptpCoordParams, false, ref queuedCmdIndex) == 0;
        }

        private PTPJumpParams Get_JumpParams() // Retourne la structure jumpParams
        {
            DobotDll.GetPTPJumpParams(ref jumpParams);
            return jumpParams;
        }

        private JOGCommonParams Get_CommonParams() // Retourne la structure commonParams (structure en des vitesses/accelerations en mode jog)
        {
            DobotDll.GetJOGCommonParams(ref commonParams);
            return commonParams;
        }

        private PTPCoordinateParams Get_PtpCoordinateParams() // Retourne la structure ptpCoordParams (structure en des vitesses/accelerations en mode ptp (en coordonnée))
        {
            DobotDll.GetPTPCoordinateParams(ref ptpCoordParams);
            return ptpCoordParams;
        }

        public bool Move(JogCmdType move) // 0 à 10 = un mouvement défini dans JogCmdType, Set les vitesses avant d'utiliser cette methode
        {
            jogCmd.cmd = (byte)move;

            return DobotDll.SetJOGCmd(ref jogCmd, false, ref cmdIndex) == 0;
        }

        #endregion

        #region Coordinate

        private void SetMode(Mode mode) // Choix du mode de déplacement pour les coordonnées
        {
            ptpCmd.ptpMode = (byte)mode;
        }

        private ulong Ptp(float x, float y, float z, float r) // Enregistre les Axes pour les mettres dans le cmdIndex pour pouvoir l'utiliser dans SetCordinateXYZR
        {
            ptpCmd.x = x;
            ptpCmd.y = y;
            ptpCmd.z = z;
            ptpCmd.rHead = r;
            while (true)
            {
                int ret = DobotDll.SetPTPCmd(ref ptpCmd, true, ref cmdIndex);
                if (ret == 0)
                {
                    break;
                }
            }
            return cmdIndex;
        }

        public void CoordinateXYZR(float x, float y, float z, float r) // Va aux coordonnées misent en parametre avec le mode sauvegarder dans la structure (choisit grâce au setMode)
        {
            cmdIndex = Ptp(x, y, z, r);
            while (true)
            {
                ulong retIndex = 0;
                int ind = DobotDll.GetQueuedCmdCurrentIndex(ref retIndex);
                if (ind == 0 && cmdIndex <= retIndex)
                {
                    break;
                }
            }
        }

        private Pose Get_Coordinate() // Retourne la structure des positions actuelles du bras
        {
            Pose pose = new Pose();
            DobotDll.GetPose(ref pose);
            return pose;
        }

        #endregion

    }
}