using UnityEngine;
using TMPro;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class TextPopUpScript : MonoBehaviour
{
    public ParticleSystem celebrateFlare;
    public TMP_Text textPrefab;
    public float lifetime = 5f;
    public void SpawnText(string text, Vector3 position)
    {
        Vector3 variation = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
        TMP_Text instantiatedText = Instantiate(textPrefab, position+variation, Quaternion.identity);
        instantiatedText.text = text;
        instantiatedText.transform.LookAt(Camera.main.transform);
        instantiatedText.transform.rotation = Quaternion.LookRotation(instantiatedText.transform.position - Camera.main.transform.position);
        SpawnFlare(position);
        Destroy(instantiatedText.gameObject, lifetime);
    }

    private void SpawnFlare(Vector3 position)
    {
        ParticleSystem flareInstance = Instantiate(celebrateFlare, position, quaternion.identity);
        Destroy(flareInstance.gameObject, lifetime);
    }

}