using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Tester
{
    public class TestEffects : MonoBehaviour
    {
        public static TestEffects Instance;
        public Text UIText;
        public GameObject[] effects;
        int index = 0;
        public GameObject activeeffect;
        void Start()
        {
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < effects.Length; i++)
                {
                    effects[i].SetActive(false);
                }
                activeeffect = effects[index];
                activeeffect.SetActive(true);
                //UIText.text = (index + 1).ToString() + ". " + effects[index].name;
                index++;
                if (index == effects.Length)
                {
                    index = 0;
                }
            }
        }
    }
}
