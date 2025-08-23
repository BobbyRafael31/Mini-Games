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

    [Header("Spawn Settings")] // based on camera
    [Range(0.1f, 1f)] public float centerStripWidthPercent = 0.35f;
    [Range(0f, 0.45f)] public float verticalMarginPercent = 0.1f;

    [Header("Limits")]
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
        Camera cam = Camera.main;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float stripHalfW = halfW * centerStripWidthPercent * 0.5f;
        float x = Random.Range(-stripHalfW, stripHalfW);

        float vMargin = halfH * verticalMarginPercent;
        float y = Random.Range(-halfH + vMargin, halfH - vMargin);

        return new Vector3(x, y, 0f);
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        var cam = Camera.main;
        if (!cam) return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float stripHalfW = halfW * centerStripWidthPercent * 0.5f;
        float vMargin = halfH * verticalMarginPercent;

        Gizmos.color = Color.yellow;
        Vector3 center = Vector3.zero;
        Vector3 size = new Vector3(stripHalfW * 2f, (halfH - vMargin) * 2f, 0f);
        Gizmos.DrawWireCube(center, size);
    }

#endif
}
