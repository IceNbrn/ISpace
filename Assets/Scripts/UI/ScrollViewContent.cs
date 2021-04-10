using System;
using System.Collections.Generic;
using UI.ScoreBoard;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScrollViewContent : MonoBehaviour
    {
        [SerializeField] 
        private float contentHeightOffset;
        [SerializeField]
        private Transform spawnPosition;

        public bool IsScrollActive;

        private void OnGUI()
        {
            if(IsScrollActive)
                UpdateScrollView();
        }

        private void UpdateScrollView()
        {
            /*
            Vector3 lastPosition = 
            Vector3 position = lastPosition;
            position.y -= contentHeightOffset;

            instantiatedObject = Instantiate(prefab, position, Quaternion.identity);*/
        }

        public GameObject AddContent(GameObject prefab)
        {
        
            GameObject instantiatedObject = null;
            /*if (_objectsSpawned.Count == 0)
            {
                instantiatedObject = Instantiate(prefab, spawnPosition.position, Quaternion.identity);
                _objectsSpawned.Add(instantiatedObject.transform);
            }
            else
            {
                
                
            }*/
            return instantiatedObject;
        }
    }
}