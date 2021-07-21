using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class DeathUIManager : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI textKilledBy;

        public void SetTextKilledBy(string killerName, string weaponName)
        {
            string text = $"You have been killed by {killerName} with {weaponName}";
            textKilledBy.SetText(text);
        }

        public void SetKilledTextEmpty() => textKilledBy.SetText(String.Empty);
    }
}