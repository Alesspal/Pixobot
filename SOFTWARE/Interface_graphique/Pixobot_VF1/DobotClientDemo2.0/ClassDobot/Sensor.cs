using DobotClientDemo.CPlusDll;

namespace ObjDobot
{
    abstract class Sensor
    {
        protected const byte _VERSION = 1;  // Si la Version de l'accessoire est V1.0 : Vesrion = 0, si la version est V2.0 : Version = 1

        protected bool _status;

        protected const byte ERREUR = 255;

        public enum GPort
        {
            IF_PORT_GP1,    // Pour API DobotDLL
            IF_PORT_GP2,
            IF_PORT_GP4,
            IF_PORT_GP5,
            PAS_DE_PORT = 255,  // Pas pour API juste un nb pour dire qu'il n'y pas de port sélectionnée
        };

        protected GPort _Port = 0;

        public GPort Port {
            get {
                return _Port;
            }
            set {
                ChangePort(value);
            }
        }

        public bool IsEnable {
            get {
                return GetStatus(); // retourne si il est actif ou pas
            }
        }

        protected Sensor(GPort port)
        {
            _status = false;
            _Port = port;
        }

        public abstract bool TurnOn();      // Active le capteur
        public abstract bool TurnOff();     // Desactive le capteur
        public abstract byte GetSensor();   // Retourne ce que le capteur à devant lui

        private bool GetStatus()            // Retourne le status du capteur
        {
            return _status;
        }

        private void ChangePort(GPort port)
        {
            _Port = port;
        }
    }

    class ColorSensor : Sensor
    {

        public ColorSensor(GPort port) : base(port)
        {
        }

        public override bool TurnOn()
        {
            if (DobotDll.SetColorSensor(true, (ColorPort)_Port, _VERSION) == (int)DobotCommunicate.DobotCommunicate_NoError)
            {
                _status = true;
                return true;
            }
            return false;
        }

        public override bool TurnOff()
        {
            if (DobotDll.SetColorSensor(false, (ColorPort)_Port, _VERSION) == (int)DobotCommunicate.DobotCommunicate_NoError)
            {
                _status = false;
                return true;
            }
            return false;
        }

        public override byte GetSensor() // affiche toujours un valeur meme si enable false
        {
            byte value = ERREUR;

            byte r = 0, g = 0, b = 0;

            if (DobotDll.GetColorSensor(ref r, ref g, ref b) != (int)DobotCommunicate.DobotCommunicate_NoError)
            {
                return ERREUR;
            }
            else if (r == 1) { value = 1; }
            else if (g == 1) { value = 2; }
            else if (b == 1) { value = 3; }

            return value;
        }

    }

    class LaserSensor : Sensor
    {
        // Si la Version de l'accessoire est V1.0 : Vesrion = 0, si la version est V2.0 : Version = 1 (changez dans l'abstract class Sensor dans le fichier ColorSensor)
        public LaserSensor(GPort port) : base(port)
        {
        }

        public override bool TurnOn()
        {
            if (DobotDll.SetInfraredSensor(true, (InfraredPort)_Port, _VERSION) == (int)DobotCommunicate.DobotCommunicate_NoError)
            {
                _status = true;
                return true;
            }
            return false;
        }

        public override bool TurnOff()
        {
            if (DobotDll.SetInfraredSensor(false, (InfraredPort)_Port, _VERSION) == (int)DobotCommunicate.DobotCommunicate_NoError)
            {
                _status = false;
                return true;
            }
            return false;
        }

        public override byte GetSensor() // affiche toujours un valeur meme si enable false
        {
            byte value = ERREUR;

            return DobotDll.GetInfraredSensor((InfraredPort)_Port, ref value) == (int)DobotCommunicate.DobotCommunicate_NoError ? value : ERREUR;
        }

    }

}