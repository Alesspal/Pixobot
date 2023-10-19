using System;
using DobotClientDemo.CPlusDll;

namespace ObjDobot
{
    class ConveyorBelt
    {

        #region ATTRIBUTS

        private const int DISTANCE_MIN_SBS = 1481;      // 1cm
        private const int LONGUEUR_TAPIS_SBS = 93240;   // Environ 63cm
        private const int DISTANCE_MIN_MM = 10;
        private const int LONGUEUR_TAPIS_MM = 630;
        private const byte ACTIF = 1;

        private UInt64 queuedCmdIndex = 0;

        // Bien faire la différence entre structure eMotor et eMotorS
        private tagEMotor eMotor;
        private tagEMotorS eMotorS; // Structure des moteurs avec une distance est sélectionnée

        private StepperPort _StepperPort = StepperPort.PAS_DE_PORT;
        public enum StepperPort
        {
            STEPPER_1 = 0,      // Pour API DobotDLL
            STEPPER_2 = 1,
            PAS_DE_PORT = 255,  // Pas pour API juste un nb pour dire que c'est pas de port
        };

        public StepperPort Port {
            get {
                return _StepperPort; // retourne le port sélectionné
            }
            set {
                SetPort(value);
            }
        }

        public bool IsEnable {
            get {
                return GetEmotor().isEnable == ACTIF;   // Actif ne veux pas forcement dire qu'il est en marche mais veux dire que dans la structure on l'a mis à 1
            }                                           // Pour le mettre en marche il doit être à 1 dans la structure mais faut ensuite appeler l'api pour vraiment le mettre en marche
        }
        public bool DistanceIsEnable {
            get {
                return GetEmotorS().isEnable == ACTIF;  // Même chose que le IsEnable
            }
        }
        public int Speed {
            get {
                return GetEmotor().speed;
            }
            set {
                SetSpeed(value);    // 5000 est une bonne vitesse pour être précis, plus de vitesse peut crée des pertes de pas
            }
        }
        public int DistanceSbS {
            get {
                return GetEmotorS().distance;
            }
            set {
                SetDistanceSBS(value);  // Distance max
            }
        }
        public int DistanceMM {
            get {
                return GetEmotorS().distance / 148;
            }
            set {
                SetDistanceMM(value);
            }
        }

        private bool _direction = true;
        public bool DirectionStd {
            get {
                return _direction;
            }
            set {
                SetDirection(value);
            }
        } // Mettre à true pour la faire aller dans le sens positif dès le debut, à appeler avant le SetSpeed si vous voulez changer de direction

        #endregion

        #region CONSTRUCTEUR

        //public ConveyorBelt()
        //{
        //    Speed = new int();
        //    DistanceSbS = new int();
        //    DistanceMM = new int();
        //    DirectionStd = true;
        //}

        public ConveyorBelt(StepperPort stepperPort)
        {
            SetPort(stepperPort);
            Speed = new int();
            DistanceSbS = new int();
            DistanceMM = new int();
        }

        #endregion

        #region SET

        private void SetDirection(bool direction)
        {
            _direction = direction;
            SetSpeed(eMotor.speed);
        }

        private void SetSpeed(int speed)
        {
            if (_direction)
            {
                if (speed < 0)
                {
                    eMotor.speed = -speed;
                    eMotorS.speed = -speed;
                }
                else
                {
                    eMotor.speed = speed;
                    eMotorS.speed = speed;
                }
            }
            else
            {
                if (speed > 0)
                {
                    eMotor.speed = -speed;
                    eMotorS.speed = -speed;
                }
                else
                {
                    eMotor.speed = speed;
                    eMotorS.speed = speed;
                }
            }
        }

        private void SetDistanceSBS(int distanceSBS)    // SbS = Step by Step
        {
            if (distanceSBS == 0)
            {
                distanceSBS = 0;
            }
            else if (distanceSBS < DISTANCE_MIN_SBS)
            {
                distanceSBS = DISTANCE_MIN_SBS;
            }
            else if (distanceSBS > LONGUEUR_TAPIS_SBS)
            {
                distanceSBS = LONGUEUR_TAPIS_SBS;
            }
            eMotorS.distance = distanceSBS;
        }

        private void SetDistanceMM(int distanceMM)
        {
            if (distanceMM == 0)
            {
                distanceMM = 0;
            }
            else if (distanceMM < DISTANCE_MIN_MM) // Ne pas faire un distance plus petite que le centimetre (pas précis et inutile)
            {
                distanceMM = DISTANCE_MIN_MM;
            }
            else if (distanceMM > LONGUEUR_TAPIS_MM)
            {
                distanceMM = LONGUEUR_TAPIS_MM;
            }

            DistanceSbS = distanceMM * 148; //  1481 pas = 1cm
            eMotorS.distance = DistanceSbS;
        }

        private void SetPort(StepperPort stepperPort)
        {
            _StepperPort = stepperPort;
            eMotor.index = (byte)stepperPort; // 0 : stepper1, 1 : stepper2
            eMotorS.index = (byte)stepperPort;
        }

        #endregion

        #region Enclenchement

        public bool TurnOn() // A appeler apres avoir set une vitesse
        {
            eMotor.isEnable = 1;

            if (_StepperPort == StepperPort.PAS_DE_PORT || DobotDll.SetEMotor(ref eMotor, false, ref queuedCmdIndex) != (int)DobotCommunicate.DobotCommunicate_NoError)
            {
                eMotor.isEnable = 0;
                return false;
            }
            return true;

        }

        public bool TurnOff() // A appeler apres avoir set une vitesse
        {
            eMotor.isEnable = 0;

            if (_StepperPort == StepperPort.PAS_DE_PORT || DobotDll.SetEMotor(ref eMotor, false, ref queuedCmdIndex) != (int)DobotCommunicate.DobotCommunicate_NoError)
            {
                eMotor.isEnable = 1;
                return false;
            }
            return true;

        }

        public bool DistanceOn()  // A appeler apres avoir set une distance pour l'appliquer
        {
            eMotorS.isEnable = 1;

            if (DobotDll.SetEMotorS(ref eMotorS, false, ref queuedCmdIndex) != (int)DobotCommunicate.DobotCommunicate_NoError)
            {
                eMotorS.isEnable = 0;
                return false;
            }
            return true;
        }

        #endregion

        #region GET

        private tagEMotor GetEmotor()
        {
            return eMotor;
        }

        private tagEMotorS GetEmotorS()
        {
            return eMotorS;
        }

        #endregion       

    }
}