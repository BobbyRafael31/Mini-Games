using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float playerSpeed = 10f;

    [Header("Input Settings")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;

    float limit;

    private void Start()
    {
        float halfHeight = Camera.main.orthographicSize;
        float playerHalf = GetComponent<BoxCollider2D>().bounds.extents.y;
        limit = halfHeight - playerHalf;
    }

    private void Update()
    {
        float direction = 0f;

        if (Input.GetKey(upKey)) direction += 1f;
        if (Input.GetKey(downKey)) direction -= 1f;

        Vector3 position = transform.position;
        position.y += direction * playerSpeed * Time.deltaTime;

        position.y = Mathf.Clamp(position.y, -limit, limit);

        transform.position = position;
    }


}
