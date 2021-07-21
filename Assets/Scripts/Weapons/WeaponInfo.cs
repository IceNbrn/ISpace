using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "WeaponInfo", menuName = "Weapons/Weapon")]
    public class WeaponInfo : ScriptableObject
    {
        public int Damage = 10;
        public float FireRate = 25.0f;
        public int Capacity = 120;
        public int MagazineCapacity = 30;
        public float Range = 200.0f;
        public float CoolOffTime = 3.0f;
        public float RecoilForce = 5.0f;
    }
}