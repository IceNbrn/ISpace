using TMPro;
using UnityEngine;

namespace DevConsole
{
    public class CommandOutput : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleTxt;
        [SerializeField] private TextMeshProUGUI valueTxt;
        [SerializeField] private TextMeshProUGUI descriptionTxt;

        public void SetTexts(string title, string value, string description)
        {
            titleTxt.SetText(title);
            valueTxt.SetText(value);
            descriptionTxt.SetText(description);
        }
    }
}