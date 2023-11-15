using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryGame : MonoBehaviour
{
   public GameObject tilePrefab; // Prefab cho ô vuông
    public List<Sprite> images;  
    
    public int gridSize = 5;

    public float spacing = 2f;
   private List<GameObject> tiles = new List<GameObject>();
    private List<Sprite> shuffledImages = new List<Sprite>();

    private bool isCheckingMatch = false;
    private List<GameObject> selectedTiles = new List<GameObject>();

    private Color highlightedColor = new Color(1f, 1f, 1f, 0.5f); // Màu sắc khi được chọn
    private Color defaultColor = new Color(1f, 1f, 1f, 1f); // Màu sắc mặc định

   public SpriteRenderer image1;
    public SpriteRenderer image2;

    private bool isFlipped = false;

     private bool isHandlingImage1 = false;
    
 

    void Start()
    {
        image1.sortingOrder = 1;
        image2.sortingOrder = 2;
        if (images.Count < gridSize * gridSize / 2)
        {
            return;
        }
      
        InitializeGame();
        ShuffleAndSelectImages();
        SetImagesOnTiles();
    }

     void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isCheckingMatch)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            if (hitCollider != null)
            {
                GameObject selectedTile = hitCollider.gameObject;

                if (selectedTile.GetComponent<SpriteRenderer>().sprite != null && !selectedTiles.Contains(selectedTile))
                {
                    selectedTiles.Add(selectedTile);

                    if (hitCollider.gameObject == image1.gameObject || hitCollider.gameObject == image2.gameObject)
                    {
                        // Xoay chỉ ảnh được chọn
                        StartCoroutine(FlipImage(selectedTile));
                                UnityEngine.Debug.LogError("Update sortingOrder: " + selectedTile.GetComponent<SpriteRenderer>().sortingOrder);

                    }

                    if (selectedTiles.Count == 2)
                    {
                        StartCoroutine(CheckMatchingImages());
                    }
                }
            }
        }
    }

    void InitializeGame()
    {
        GameObject gameController = GameObject.Find("GameController");
        if (gameController == null)
        {
            return;
        }

        Vector3 gameControllerPosition = gameController.transform.position;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(i * spacing - gridSize / 2f + gameControllerPosition.x, j * spacing - gridSize / 2f + gameControllerPosition.y, 0), Quaternion.identity);
                tile.transform.SetParent(gameController.transform);
                tiles.Add(tile);
            }
        }
    }
 IEnumerator FlipImage(GameObject tile)
    {
       float duration = 0.5f;
        float elapsed = 0f;

        Vector3 startRotation = tile.transform.rotation.eulerAngles;
        Vector3 targetRotation = new Vector3(startRotation.x, startRotation.y + 180f, startRotation.z);

        while (elapsed < duration)
        {
            tile.transform.rotation = Quaternion.Euler(Vector3.Lerp(startRotation, targetRotation, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Đảo ngược orderLayer để cả hai ảnh đều có sortingOrder là 1
        tile.GetComponent<SpriteRenderer>().sortingOrder = 1;
        UnityEngine.Debug.LogError("sortingOrder  " + tile.GetComponent<SpriteRenderer>().sortingOrder);
        
        isHandlingImage1 = !isHandlingImage1; // Đảo ngược trạng thái của isFlipped
    }
    IEnumerator CheckMatchingImages()
    {
       isCheckingMatch = true;
        yield return new WaitForSeconds(1.0f);

        SpriteRenderer firstSpriteRenderer = selectedTiles[0].GetComponent<SpriteRenderer>();
        SpriteRenderer secondSpriteRenderer = selectedTiles[1].GetComponent<SpriteRenderer>();

        if (firstSpriteRenderer.sprite == secondSpriteRenderer.sprite)
        {
            Destroy(selectedTiles[0]);
            Destroy(selectedTiles[1]);
            tiles.Remove(selectedTiles[0]);
            tiles.Remove(selectedTiles[1]);
        }
        else
        {
            // Xác định xem đang xử lý ảnh 1 hay ảnh 2 để cập nhật sortingOrder
            if (isHandlingImage1)
            {
                firstSpriteRenderer.sortingOrder = 1;
                secondSpriteRenderer.sortingOrder = 2;
            }
            else
            {
                firstSpriteRenderer.sortingOrder = 2;
                secondSpriteRenderer.sortingOrder = 1;
            }

            StartCoroutine(FlipImage(selectedTiles[0]));
            StartCoroutine(FlipImage(selectedTiles[1]));
        }

        isCheckingMatch = false;
        selectedTiles.Clear();
    
    }

    void ShuffleAndSelectImages()
    {
        List<Sprite> availableImages = new List<Sprite>(images);

        List<Sprite> imagePairs = new List<Sprite>();
        for (int i = 0; i < gridSize * gridSize / 2; i++)
        {
            int randomIndex = Random.Range(0, availableImages.Count);
            Sprite selectedImage = availableImages[randomIndex];
            imagePairs.Add(selectedImage);
            imagePairs.Add(selectedImage);
            availableImages.RemoveAt(randomIndex);
        }

        int n = imagePairs.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Sprite temp = imagePairs[k];
            imagePairs[k] = imagePairs[n];
            imagePairs[n] = temp;
        }

        shuffledImages.AddRange(imagePairs);
    }

    void SetImagesOnTiles()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].GetComponent<SpriteRenderer>().sprite = shuffledImages[i % shuffledImages.Count];
        }
    }
} 