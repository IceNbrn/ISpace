using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "WeaponInfo", menuName = "Weapons/Weapon")]
    public class WeaponInfo : ScriptableObject
    {
        public float Damage = 10.0f;
        public float FireRate = 25.0f;
        public byte Capacity = 30;
        public float Range = 200.0f;

        public ParticleSystem MuzzleFlash;
    }
}