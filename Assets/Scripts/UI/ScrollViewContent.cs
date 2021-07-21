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
        [SerializeField] 
        private Transform parent;
        [SerializeField] 
        private List<GameObject> listPrefabs;

        public bool IsScrollViewActive;

        private void OnGUI()
        {
            if (IsScrollViewActive)
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
            GameObject instantiatedObject = Instantiate(prefab, spawnPosition.transform.parent, false);
            RectTransform rectTransform = instantiatedObject.GetComponent<RectTransform>();
            RectTransform spawnRectTransform = spawnPosition.GetComponent<RectTransform>();
            Rect rect = spawnRectTransform.rect;
            Vector3 position = rectTransform.localPosition;
            
            rectTransform.sizeDelta = new Vector2(0.0f, rect.height);
            position.y -= spawnRectTransform.sizeDelta.y * listPrefabs.Count;
            
            rectTransform.localPosition = position;
            listPrefabs.Add(instantiatedObject);
            
            return instantiatedObject;
        }

        public void DeleteAllContent()
        {
            foreach (GameObject prefab in listPrefabs)
                Destroy(prefab);
            listPrefabs.Clear();
        }
    }
}