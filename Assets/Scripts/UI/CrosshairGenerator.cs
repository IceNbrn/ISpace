using System;
using Player;
using UnityEngine;

namespace UI
{
    public class CrosshairGenerator : MonoBehaviour
    {
        [SerializeField] private RectTransform topRect;
        [SerializeField] private RectTransform bottomRect;
        [SerializeField] private RectTransform leftRect;
        [SerializeField] private RectTransform rightRect;

        private CrosshairSettings _crosshairSettings;
        private PlayerSettings _playerSettings;
        private void Start()
        {
            _playerSettings = GameManager.Singleton.GetPlayerSettings();
            _playerSettings.OnCrosshairUpdated += SetCrosshair;
            SetCrosshair();
        }


        private void SetCrosshair()
        {
            _crosshairSettings = _playerSettings.CrosshairSettings;
            SetThickAndHeight(_crosshairSettings.Thickness, _crosshairSettings.Height);
            SetGap(_crosshairSettings.Gap);
        }

        private void SetThickAndHeight(float thickness, float height)
        {
            topRect.sizeDelta = new Vector2(thickness, height);
            bottomRect.sizeDelta = new Vector2(thickness, height);
            leftRect.sizeDelta = new Vector2(thickness, height);
            rightRect.sizeDelta = new Vector2(thickness, height);
        }

        private void SetGap(float gap)
        {
            topRect.anchoredPosition = new Vector2(0.0f, gap);
            bottomRect.anchoredPosition = new Vector2(0.0f, -gap);
            leftRect.anchoredPosition = new Vector2(-gap, 0.0f);
            rightRect.anchoredPosition = new Vector2(gap, 0.0f);
        }
    }
}