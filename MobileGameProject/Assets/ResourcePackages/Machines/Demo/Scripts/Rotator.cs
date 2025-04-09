using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinigamesDemo
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField]
        private float rotationSpeed = 5.0f;

        private void Update()
        {
            transform.Rotate(transform.forward, rotationSpeed * Time.deltaTime);
        }
    }
}