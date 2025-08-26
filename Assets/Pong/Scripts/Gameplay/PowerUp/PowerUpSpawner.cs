using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public PowerUp[] powerUpPrefabs;

    [Header("Settings")]
    public float minInterval = 4f;
    public float maxInterval = 8f;

    [Header("PowerUp Spawn Settings")]
    [Range(0.1f, 1f)] public float centerStripWidthPercent = 0.35f;
    [Range(0f, 0.45f)] public float verticalMarginPercent = 0.1f;


    [Header("PowerUp Limits")]
    public int maxActive = 3;

    private List<PowerUp> activePowerUps = new List<PowerUp>();
    private Coroutine loop;

    public void Begin()
    {
        if (loop == null) loop = StartCoroutine(SpawnLoop());
    }

    public void StopSpwanning()
    {
        if (loop != null) { StopCoroutine(loop); loop = null; }

        foreach (var p in activePowerUps)
        {
            if (p != null) Destroy(p.gameObject);
        }
        activePowerUps.Clear();
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            if (activePowerUps.Count >= maxActive) continue;

            PowerUp prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
            Vector3 pos = RandomSpawnPosition();

            PowerUp newPU = Instantiate(prefab, pos, Quaternion.identity);
            newPU.spawner = this;
            activePowerUps.Add(newPU);
        }
    }

    public void NotifyTaken(PowerUp instance)
    {
        activePowerUps.Remove(instance);
    }

    Vector3 RandomSpawnPosition()
    {
        Bounds camBounds = CameraBoundsUtility.GetCameraBounds(Camera.main);
        Vector3 camCenter = camBounds.center;

        float stripHalfWidth = camBounds.size.x * centerStripWidthPercent * 0.5f;
        float verticalMargin = camBounds.size.y * verticalMarginPercent;

        float x = Random.Range(camCenter.x - stripHalfWidth, camCenter.x + stripHalfWidth);
        float y = Random.Range(camBounds.min.y + verticalMargin, camBounds.max.y - verticalMargin);

        return new Vector3(x, y, 0f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!Camera.main) return;

        Bounds camBounds = CameraBoundsUtility.GetCameraBounds(Camera.main);
        Vector3 camCenter = camBounds.center;

        float stripHalfWidth = camBounds.size.x * centerStripWidthPercent * 0.5f;
        float verticalMargin = camBounds.size.y * verticalMarginPercent;

        Vector3 center = new Vector3(camCenter.x, camCenter.y, 0f);
        Vector3 size = new Vector3(stripHalfWidth * 2f, camBounds.size.y - verticalMargin * 2f, 0f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
#endif

}
