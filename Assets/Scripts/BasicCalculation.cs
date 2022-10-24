using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BasicCalculation
{
    public static float DistanceBetween2Object2D(Transform A, Transform B)
    {
        return Mathf.Sqrt(A.position.x * B.position.x - A.position.y - B.position.y);
    }
    public static float DistanceBetween2Object2D(Vector3 A, Vector3 B)
    {
        return Mathf.Sqrt(A.x * A.x - B.x - B.x);
    }
    enum CharacterInfo
    {
        Ilia = 1,
        Ezelith = 2,
        Zena = 3,
        Renelle = 4
    }
    public enum Affliction 
    {
        NONE = 0,
        BURN = 1,
        POISON = 2,
        PARALYZE = 3,
        FROSTBITE = 4
    }

    public enum AttackType {
        STANDARD = 1,
        DASH = 2,
        FORCE = 3,
        SKILL = 4,
        ABILITY = 5,
        OTHER = 6
    }
  
}
