using System;

namespace ControlComponents.Core
{
    public delegate void OccupationEventHandler(object sender, OccupationEventArgs e);

    public class OccupationEventArgs : EventArgs
    {
        public string Occupier { get; }

        public OccupationEventArgs(string occupier)
        {
            Occupier = occupier;
        }
    }
}