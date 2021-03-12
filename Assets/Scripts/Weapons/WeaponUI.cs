using TMPro;
using UnityEngine;

namespace Weapons
{
    public class WeaponUI : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI ammoText;

        public void UpdateAmmo(int currentAmmo, int totalAmmo)
        {
            ammoText.SetText($"{currentAmmo} / {totalAmmo}");
        }
    }
}