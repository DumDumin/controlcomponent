﻿using System.Collections;
using System.Collections.Generic;

internal class Occupation
{
    public const string OCCUPIER_NONE = "NONE";
    public string OCCUPIER { get; private set; } = OCCUPIER_NONE;

    public void Free(string sender)
    {
        if (OCCUPIER == sender)
        {
            OCCUPIER = OCCUPIER_NONE;
        }
    }

    public void Occupy(string sender)
    {
        if(IsFree())
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

}