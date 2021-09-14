using System;
using System.Collections;
using System.Collections.Generic;

namespace ControlComponents.Core
{
    internal class Occupation
    {
        private string _occupier = OCCUPIER_NONE;

        public const string OCCUPIER_NONE = "NONE";
        public string OCCUPIER
        { 
            get => _occupier; 
            private set
            {
                _occupier = value;
                OccupierChanged?.Invoke(this, new OccupationEventArgs(_occupier));
            }
        }

        public event OccupationEventHandler OccupierChanged;

        public void Free(string sender)
        {
            if (OCCUPIER == sender)
            {
                OCCUPIER = OCCUPIER_NONE;
            }
        }

        public void Occupy(string sender)
        {
            if (IsFree())
            {
                OCCUPIER = sender;
            }
        }

        public bool IsFree()
        {
            return OCCUPIER == OCCUPIER_NONE;
        }

        public bool IsOccupied()
        {
            return !IsFree();
        }

        public bool IsUsableBy(string id) => IsFree() || OCCUPIER == id;

        public void Prio(string sender)
        {
            OCCUPIER = sender;
        }
    }
}