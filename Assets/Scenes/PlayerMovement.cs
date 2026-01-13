using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 7f;

    [Header("Clamp")]
    public float padding = 0.5f; 

    private Camera cam;
    private float minX, maxX;

    void Awake()
    {
        cam = Camera.main;
        RecalcBounds();
    }

    void Update()
    {
        if (!GameManager.Instance || !GameManager.Instance.IsPlaying) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        transform.Translate(Vector2.right * moveX * speed * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX + padding, maxX - padding);
        transform.position = pos;
    }

    public void RecalcBounds()
    {
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        minX = left.x;
        maxX = right.x;
    }
}


