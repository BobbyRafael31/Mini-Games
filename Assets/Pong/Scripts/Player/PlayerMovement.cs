using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float playerSpeed = 10f;

    [Header("Input Settings")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;

    float limit;
    private bool frozen = false;

    private void Start()
    {
        float halfHeight = Camera.main.orthographicSize;
        float playerHalf = GetComponent<BoxCollider2D>().bounds.extents.y;
        limit = halfHeight - playerHalf;
    }

    private void Update()
    {

        if (frozen) return;

        float direction = 0f;

        if (Input.GetKey(upKey)) direction += 1f;
        if (Input.GetKey(downKey)) direction -= 1f;

        Vector3 position = transform.position;
        position.y += direction * playerSpeed * Time.deltaTime;

        position.y = Mathf.Clamp(position.y, -limit, limit);

        transform.position = position;
    }

    public void Freeze(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        frozen = true;
        yield return new WaitForSeconds(duration);
        frozen = false;
    }

    public void Reduce(float duration)
    {
        StartCoroutine(ReduceRoutine(duration));
    }

    private IEnumerator ReduceRoutine(float duration)
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z);

        yield return new WaitForSeconds(duration);

        transform.localScale = originalScale;
    }

}
