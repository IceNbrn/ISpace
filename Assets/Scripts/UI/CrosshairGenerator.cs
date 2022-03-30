using System;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CrosshairGenerator : MonoBehaviour
    {
        [SerializeField] private RectTransform topRect;
        [SerializeField] private RectTransform bottomRect;
        [SerializeField] private RectTransform leftRect;
        [SerializeField] private RectTransform rightRect;

        [SerializeField] private Image[] rectangles;

        private CrosshairSettings _crosshairSettings;
        private PlayerSettings _playerSettings;
        private void Start()
        {
            GameManager.Singleton.PlayerSettings.OnCrosshairUpdated += SetCrosshair;
            
            SetCrosshair();
        }


        private void SetCrosshair()
        {
            _crosshairSettings = GameManager.Singleton.PlayerSettings.CrosshairSettings;
            SetThickAndHeight(_crosshairSettings.Thickness, _crosshairSettings.Size);
            SetGap(_crosshairSettings.Gap);
            SetType(_crosshairSettings.Type, _crosshairSettings.Gap);
            SetColor(_crosshairSettings.Color);
        }

        private void SetThickAndHeight(float thickness, float size)
        {
            topRect.sizeDelta = new Vector2(thickness, size);
            bottomRect.sizeDelta = new Vector2(thickness, size);
            leftRect.sizeDelta = new Vector2(thickness, size);
            rightRect.sizeDelta = new Vector2(thickness, size);
        }

        private void SetGap(float gap)
        {
            topRect.anchoredPosition = new Vector2(0.0f, gap);
            bottomRect.anchoredPosition = new Vector2(0.0f, -gap);
            leftRect.anchoredPosition = new Vector2(-gap, 0.0f);
            rightRect.anchoredPosition = new Vector2(gap, 0.0f);
        }

        private void SetType(ECrosshairType type, float gap)
        {
            GameObject topGameObject = topRect.gameObject;
            switch (type)
            {
                case ECrosshairType.Cross:

                    topGameObject.SetActive(true);
                    topRect.anchoredPosition = new Vector2(0.0f, gap);
                    bottomRect.anchoredPosition = new Vector2(0.0f, -gap);
                    leftRect.anchoredPosition = new Vector2(-gap, 0.0f);
                    rightRect.anchoredPosition = new Vector2(gap, 0.0f);
                    
                    {
                        const float rotation1 = 0.0f;
                        const float rotation2 = -90.0f;
                        ApplyRotation(topRect, rotation1);
                        ApplyRotation(bottomRect, rotation1);
                        ApplyRotation(leftRect, rotation2);
                        ApplyRotation(rightRect, rotation2);
                    }

                    break;
                case ECrosshairType.T:
                    SetType(ECrosshairType.Cross, gap);
                    topGameObject.SetActive(false);
                    break;
                case ECrosshairType.X:

                    topGameObject.SetActive(true);
                    topRect.anchoredPosition = new Vector2(gap, gap);
                    bottomRect.anchoredPosition = new Vector2(-gap, -gap);
                    leftRect.anchoredPosition = new Vector2(-gap, gap);
                    rightRect.anchoredPosition = new Vector2(gap, -gap);

                    {
                        const float rotation1 = -45.0f;
                        const float rotation2 = -135.0f;
                        ApplyRotation(topRect, rotation1);
                        ApplyRotation(bottomRect, rotation1);
                        ApplyRotation(leftRect, rotation2);
                        ApplyRotation(rightRect, rotation2);
                    }
                    
                    break;
                default:
                    break;
            }
        }

        private void SetColor(Color color)
        {
            foreach (Image rectangle in rectangles)
            {
                rectangle.color = color;
            }
        }
        
        private void ApplyRotation(RectTransform rectTransform, float rotation)
        {
            rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
        }
    }
}