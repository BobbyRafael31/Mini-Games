using UnityEngine;

public static class CameraBoundsUtility
{
    public static Bounds GetCameraBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / Screen.height;
        float cameraHeight = camera.orthographicSize * 2;

        Vector3 center = camera.transform.position;
        Vector3 size = new Vector3(cameraHeight * screenAspect, cameraHeight, 0);

        return new Bounds(center, size);
    }

    public static Vector3 ClampToBounds(Vector3 position, Bounds bounds)
    {
        float clampedX = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        float clampedY = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);

        return new Vector3(clampedX, clampedY, position.z);
    }

    public static Vector3 GetRandomPointInside(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector3(x, y, 0);
    }

    public static Vector3 GetRandomPointOutside(Bounds bounds, float offset = 1f)
    {
        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: // Left
                return new Vector3(bounds.min.x - offset, Random.Range(bounds.min.y, bounds.max.y), 0);
            case 1: // Right
                return new Vector3(bounds.max.x + offset, Random.Range(bounds.min.y, bounds.max.y), 0);
            case 2: // Top
                return new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y + offset, 0);
            case 3: // Bottom
                return new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.min.y - offset, 0);
            default:
                return bounds.center;
        }
    }
}
