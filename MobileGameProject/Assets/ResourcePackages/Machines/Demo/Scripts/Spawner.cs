using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649

namespace MinigamesDemo
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject spawnPrototype;

        [SerializeField]
        private float spawnDelay;

        [SerializeField]
        private int spawnCount;

        private List<GameObject> spawnedObjects = new List<GameObject>();

        private void OnEnable()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            foreach (var spawned in spawnedObjects)
                Destroy(spawned);

            yield return null;

            spawnedObjects.Clear();

            spawnPrototype.SetActive(false);

            var siblingIndex = spawnPrototype.transform.GetSiblingIndex();
            var delay = new WaitForSeconds(spawnDelay);
            for (var index = 0; index < spawnCount; index++)
            {
                var instance = Instantiate(spawnPrototype, spawnPrototype.transform.parent);
                spawnedObjects.Add(instance);
                instance.SetActive(true);
                instance.transform.SetSiblingIndex(siblingIndex);

                yield return delay;
            }
        }
    }
}