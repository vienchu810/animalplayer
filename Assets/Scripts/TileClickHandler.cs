using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileClickHandler : MonoBehaviour
{
   public SpriteRenderer image1;
    public SpriteRenderer image2;

    private bool isFlipped = false;

    void Start()
    {
        // Đặt orderLayer cho ảnh 1 và 2
        image1.sortingOrder = 1;
        image2.sortingOrder = 2;
    }

    void Update()
    {
        // Kiểm tra sự kiện click
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            if (hitCollider != null)
            {
                // Kiểm tra xem ảnh 1 hay ảnh 2 được click
                if (hitCollider.gameObject == image1.gameObject || hitCollider.gameObject == image2.gameObject)
                {
                    // Thực hiện xoay lật 180 độ
                    StartCoroutine(FlipImages());
                }
            }
        }
    }

    IEnumerator FlipImages()
    {
        // Xoay lật 180 độ
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startRotation = transform.rotation.eulerAngles;
        Vector3 targetRotation = new Vector3(startRotation.x, startRotation.y + 180f, startRotation.z);

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Euler(Vector3.Lerp(startRotation, targetRotation, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Đảo ngược orderLayer để ảnh 2 lên trước ảnh 1
        image1.sortingOrder = isFlipped ? 1 : 2;
        image2.sortingOrder = isFlipped ? 2 : 1;

        // Đảo ngược trạng thái của isFlipped
        isFlipped = !isFlipped;
    }
}