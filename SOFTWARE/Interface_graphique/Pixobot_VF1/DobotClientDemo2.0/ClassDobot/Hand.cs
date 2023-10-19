using System;
using DobotClientDemo.CPlusDll;

namespace ObjDobot
{
    abstract class Hand
    {
        protected UInt64 _cmdIndex;

        public bool IsEnable {
            get {
                return GetStatus();
            }
        }

        protected Hand()
        {
            _cmdIndex = 0;
        }

        public abstract bool Catch(); // Prend l'objet
        public abstract bool Release(); // Relache l'objet
        protected abstract bool GetStatus(); // Retourne si il est entrain de prendre ou pas l'objet

    }

    class SuctionCup : Hand
    {
        private bool _isEnabled;
        private bool _isCatch;

        public override bool Catch()
        {
            _isEnabled = true;
            _isCatch = true;
            return DobotDll.SetEndEffectorSuctionCup(_isEnabled, _isCatch, false, ref _cmdIndex) == (int)DobotCommunicate.DobotCommunicate_NoError;
        }

        public override bool Release()
        {
            _isEnabled = false;
            _isCatch = false;
            return DobotDll.SetEndEffectorSuctionCup(_isEnabled, _isCatch, false, ref _cmdIndex) == (int)DobotCommunicate.DobotCommunicate_NoError;
        }

        protected override bool GetStatus()
        {
            DobotDll.GetEndEffectorSuctionCup(ref _isEnabled, ref _isCatch);

            return _isEnabled;
        }

    }

    class Gripper : Hand
    {
        // LES API DU GRIPPER PEUX FAIRE FONCTIONNER LE SUCTION CUP
        // S'INFORMER SUR LE PDF DES API POUR VOIR COMMENT LE GRIPPER FONCTIONNE
        private bool _isEnabled;
        private bool _isCatch;

        public override bool Catch()
        {
            _isEnabled = true;
            _isCatch = true;
            return DobotDll.SetEndEffectorGripper(true, true, false, ref _cmdIndex) == (int)DobotCommunicate.DobotCommunicate_NoError;
        }

        public override bool Release()
        {
            _isEnabled = false;
            _isCatch = false;
            return DobotDll.SetEndEffectorGripper(false, false, false, ref _cmdIndex) == (int)DobotCommunicate.DobotCommunicate_NoError;
        }

        protected override bool GetStatus()
        {
            DobotDll.GetEndEffectorGripper(ref _isEnabled, ref _isCatch);

            return _isEnabled;
        }
    }
}