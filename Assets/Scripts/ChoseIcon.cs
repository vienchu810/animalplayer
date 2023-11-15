using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoseIcon : MonoBehaviour
{
    public GameObject imageContainerLayer0;
    public GameObject imageContainerLayer1;

    private SpriteRenderer spriteRendererLayer0;
    private SpriteRenderer spriteRendererLayer1;

    private bool isCheckingMatch = false;
    private GameObject firstSelectedImage;

    void Start()
    {
        // Lấy các SpriteRenderer từ các GameObject
        spriteRendererLayer0 = imageContainerLayer0.GetComponent<SpriteRenderer>();
        spriteRendererLayer1 = imageContainerLayer1.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Kiểm tra sự kiện click chuột
        if (Input.GetMouseButtonDown(0) && !isCheckingMatch)
        {
            // Lấy vị trí chuột trong thế giới
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Kiểm tra xem chuột có trỏ vào GameObject ảnh không
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            if (hitCollider != null)
            {
                // Kiểm tra xem chuột có trỏ vào GameObject ảnh chứa Order in Layer 0 hay 1 không
                if (hitCollider.gameObject == imageContainerLayer0)
                {
                    // Thay đổi Order in Layer của GameObject ảnh chứa Order in Layer 0 thành 1
                    spriteRendererLayer0.sortingOrder = 1;

                    // Lưu lại thông tin về ảnh được chọn để so sánh sau này
                    firstSelectedImage = hitCollider.gameObject;

                    // Không kiểm tra sự trùng khớp ngay lúc này
                }
                else if (hitCollider.gameObject == imageContainerLayer1)
                {
                    // Thay đổi Order in Layer của GameObject ảnh chứa Order in Layer 1 thành 1
                    spriteRendererLayer1.sortingOrder = 1;

                    // Lưu lại thông tin về ảnh được chọn để so sánh sau này
                    firstSelectedImage = hitCollider.gameObject;

                    // Không kiểm tra sự trùng khớp ngay lúc này
                }

                // Kiểm tra sự trùng khớp sau một khoảng thời gian
                StartCoroutine(CheckMatchingImages());
            }
        }
    }

    IEnumerator CheckMatchingImages()
    {
        isCheckingMatch = true;

        // Đợi một khoảng thời gian để đảm bảo người chơi có thời gian nhìn thấy hình ảnh
        yield return new WaitForSeconds(1.0f);

        // Kiểm tra sự trùng khớp ở đây
        if (firstSelectedImage != null)
        {
            // Lấy thông tin về SpriteRenderer của ảnh thứ nhất
            SpriteRenderer firstSpriteRenderer = firstSelectedImage.GetComponent<SpriteRenderer>();
            SpriteRenderer secondSpriteRenderer = null;

            // Lấy thông tin về SpriteRenderer của ảnh thứ hai
            if (firstSelectedImage == imageContainerLayer0)
            {
                secondSpriteRenderer = imageContainerLayer1.GetComponent<SpriteRenderer>();
            }
            else if (firstSelectedImage == imageContainerLayer1)
            {
                secondSpriteRenderer = imageContainerLayer0.GetComponent<SpriteRenderer>();
            }

            // Kiểm tra sự trùng khớp
            if (firstSpriteRenderer.sprite == secondSpriteRenderer.sprite)
            {
                Debug.Log("Match!");
                // Destroy hoặc thực hiện hành động khi có sự trùng khớp

                // Đặt lại Order in Layer và vị trí ban đầu của các hình ảnh
                spriteRendererLayer0.sortingOrder = 1;
                spriteRendererLayer1.sortingOrder = 2;
            }
            else
            {
                Debug.Log("No match!");
                // Đặt lại Order in Layer và vị trí ban đầu của các hình ảnh
                spriteRendererLayer0.sortingOrder = 1;
                spriteRendererLayer1.sortingOrder = 2;
            }

            // Đặt lại trạng thái kiểm tra sự trùng khớp
            isCheckingMatch = false;
        }
    }
}
