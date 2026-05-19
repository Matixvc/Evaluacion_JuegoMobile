using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Referencias")]
    public TreeData treeData;

    [Header("Config")]
    public Transform spawnPoint;    // Empty GameObject en la copa del árbol
    public float spawnForceMin = 2f;
    public float spawnForceMax = 5f;

    public void SpawnFruit()
    {
        FruitData selected = GetWeightedRandom();
        if (selected == null || selected.prefab == null) return;

        // Offset aleatorio alrededor del spawnPoint
        Vector3 randomOffset = new Vector3(
            Random.Range(-1.5f, 1.5f),
            Random.Range(0f, 0.5f),
            Random.Range(-1.5f, 1.5f)
        );

        Vector3 spawnPos = spawnPoint.position + randomOffset;

        GameObject fruit = Instantiate(selected.prefab, spawnPos, Random.rotation);

        if (fruit.TryGetComponent(out Rigidbody rb))
        {
            Vector3 force = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1f),
                Random.Range(-1f, 1f)
            ).normalized * Random.Range(spawnForceMin, spawnForceMax);

            rb.AddForce(force, ForceMode.Impulse);
        }

        FruitObject fo = fruit.GetComponent<FruitObject>();
        if (fo != null) fo.data = selected;
    }

    FruitData GetWeightedRandom()
    {
        if (treeData == null || treeData.possibleFruits.Length == 0) return null;

        float total = 0f;
        foreach (var f in treeData.possibleFruits) total += f.spawnWeight;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var f in treeData.possibleFruits)
        {
            cumulative += f.spawnWeight;
            if (roll <= cumulative) return f;
        }

        return treeData.possibleFruits[0];
    }
}