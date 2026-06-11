using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Referencias")]
    public TreeData treeData;

    [Header("Config")]
    public Transform spawnPoint;
    public float spawnForceMin = 2f;
    public float spawnForceMax = 5f;

    public void SpawnFruit()
    {
        FruitData baseData = GetWeightedRandom();
        if (baseData == null || baseData.prefab == null) return;

        // �AQU� EST� EL TRUCO!: Creamos un clon �nico con estad�sticas de velocidad aleatorias
        FruitData uniqueFruitData = baseData.CreateUniqueClone();

        Vector3 randomOffset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(0f, 0.5f), Random.Range(-1.5f, 1.5f));
        Vector3 spawnPos = spawnPoint.position + randomOffset;

        GameObject fruitObj = Instantiate(uniqueFruitData.prefab, spawnPos, Random.rotation);

        // Aplicamos el comportamiento f�sico en base a su nueva estad�stica
        if (fruitObj.TryGetComponent(out Rigidbody rb))
        {
            rb.angularDamping = uniqueFruitData.angularDrag;
            Vector3 force = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(spawnForceMin, spawnForceMax);
            rb.AddForce(force, ForceMode.Impulse);
        }

        // Le pasamos el clon de datos a su FruitObject para que sepa qu� estad�sticas guardar al tocarla
        FruitObject fo = fruitObj.GetComponent<FruitObject>();
        if (fo != null) fo.data = uniqueFruitData;
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