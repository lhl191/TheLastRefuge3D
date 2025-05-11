using UnityEngine;

public static class WeaponManager
{
    public enum WeaponType { NoWeapon, Axe, Bow }
    public static WeaponType CurrentWeapon = WeaponType.NoWeapon;

    public static string GetWeaponTypeString()
    {
        switch (CurrentWeapon)
        {
            case WeaponType.Axe:
                return "axe";
            case WeaponType.Bow:
                return "arrow";
            default:
                return "noWeapon";
        }
    }
}

