using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MinigamesDemo
{
    public class Demo : MonoBehaviour
    {
        [System.Serializable]
        public class DemoItem
        {
            public Button ItemButton;
            public GameObject GameObject;
        }

        [SerializeField]
        private List<DemoItem> demoItems = new List<DemoItem>();

        public void Start()
        {
            foreach (var item in demoItems)
            {
                var capturedItem = item;
                capturedItem.ItemButton.onClick.AddListener(() =>
                    {
                        foreach (var itemToChange in demoItems)
                            itemToChange.GameObject.SetActive(capturedItem == itemToChange);
                    });
            }
        }
    }
}