using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

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
   private bool isWatchingAd = false;
    private bool isFlipped = false;

     private bool isHandlingImage1 = false;
    
  public float gameTime = 180.0f; // Thời gian chơi là 3 phút
    private bool isGameOver = false;

private Sprite selectedImage; 

    public Text countdownText;

    private int remainingChecks = 3;
public Text remainingChecksText;
public GameObject LostDialogPrefab; 
public GameObject WinDialogPrefab;
private bool isGameWon = false;
    void Start()
    {
        //  Advertisement.Initialize("id_game_của_bạn", testMode: true);
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
         if (!isGameOver && !isGameWon)
        {
            // Đếm ngược thời gian
            gameTime -= Time.deltaTime;

            // Kiểm tra nếu thời gian hết
            if (gameTime <= 0.0f)
            {
                GameOver();
            }
            UpdateCountdownText();
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
    }
        
 void UpdateCountdownText()
    {
        // Định dạng thời gian để hiển thị trên UI Text
        int minutes = Mathf.FloorToInt(gameTime / 60.0f);
        int seconds = Mathf.FloorToInt(gameTime % 60.0f);

        // Cập nhật nội dung của UI Text
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void GameOver()
    {
        isGameOver = true;

          GameObject dialog = Instantiate(LostDialogPrefab);
              dialog.SetActive(true);
                gameObject.SetActive(true);
                                        UnityEngine.Debug.LogError("Thua cuộc");

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

    if (selectedTiles.Count == 2)
    {
        SpriteRenderer firstSpriteRenderer = selectedTiles[0].GetComponent<SpriteRenderer>();
        SpriteRenderer secondSpriteRenderer = selectedTiles[1].GetComponent<SpriteRenderer>();

        if (firstSpriteRenderer != null && secondSpriteRenderer != null)
        {
            if (firstSpriteRenderer.sprite == secondSpriteRenderer.sprite)
            {
                Destroy(selectedTiles[0]);
                Destroy(selectedTiles[1]);
                tiles.Remove(selectedTiles[0]);
                tiles.Remove(selectedTiles[1]);
                 if (tiles.Count == 0)
                {
                   
                    YouWin();
                }
            }
            else
            {
                StartCoroutine(FlipImage(selectedTiles[0]));
                StartCoroutine(FlipImage(selectedTiles[1]));
            }
        }
    }

    isCheckingMatch = false;
    selectedTiles.Clear();
    
    }
    void YouWin(){
           isGameWon = true;
      GameObject dialog = Instantiate(WinDialogPrefab);
              dialog.SetActive(true);
                gameObject.SetActive(true);
                                        UnityEngine.Debug.LogError("Thắng cuộc");    
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

        public void CheckPair()
    {
          if (!isCheckingMatch && remainingChecks > 0){
            // Lọc ra các ô còn lại có hình để chọn
            List<GameObject> remainingTiles = tiles.FindAll(tile => tile.activeSelf);

            if (remainingTiles.Count >= 2)
            {
                // Chọn một ô ngẫu nhiên từ danh sách còn lại
                int randomIndex1 = Random.Range(0, remainingTiles.Count);
                GameObject tile1 = remainingTiles[randomIndex1];

                // Xác định ảnh của ô đã chọn
                selectedImage = tile1.GetComponent<SpriteRenderer>().sprite;

                // Chọn ngẫu nhiên một ô khác có hình giống với ô đã chọn
                int randomIndex2;
                do
                {
                    randomIndex2 = Random.Range(0, remainingTiles.Count);
                } while (randomIndex2 == randomIndex1 || remainingTiles[randomIndex2].GetComponent<SpriteRenderer>().sprite != selectedImage);

                GameObject tile2 = remainingTiles[randomIndex2];

                // Xoay và kiểm tra cặp ảnh
                StartCoroutine(FlipImage(tile1));
                StartCoroutine(FlipImage(tile2));

             StartCoroutine(CheckMatchingImages());

        // Decrement the remaining checks
        remainingChecks--;

        // Check if there are no more checks and trigger ad if needed
        if (remainingChecks == 0)
        {
             ShowAd();
      UnityEngine.Debug.Log("Xem quảng cáo");

        }

        // Update the Text object with the remaining checks
        UpdateRemainingChecksText();
    }
    else
    {
        UnityEngine.Debug.Log("Out of checks. Watch an ad to get more.");
        }
    
          }
    }
  private void UpdateRemainingChecksText()
{
    // Update the Text object with the remaining checks
    if (remainingChecksText != null)
    {
        remainingChecksText.text = "" + remainingChecks.ToString();
    }
}

 private void ShowAd()
    {
        // if (!isWatchingAd && Advertisement.IsReady("rewardedVideo"))
        // {
        //     Advertisement.Show("rewardedVideo", new ShowOptions
        //     {
        //         // resultCallback = HandleAdResult
        //     });
        // }
        // else
        // {
        //     // Tuỳ chọn xử lý trường hợp khi quảng cáo không sẵn sàng
        //     Debug.Log("Quảng cáo không sẵn sàng.");
        // }
    }

    // private void HandleAdResult(ShowResult result)
    // {
    //     isWatchingAd = false;

    //     switch (result)
    //     {
    //         case ShowResult.Finished:
    //             // Người chơi xem quảng cáo, cung cấp thêm lượt kiểm tra
    //             remainingChecks = 3;
    //             Debug.Log("Quảng cáo đã xem. Bạn đã nhận thêm lượt kiểm tra!");
    //             break;
    //         case ShowResult.Skipped:
    //             // Người chơi bỏ qua quảng cáo
    //             Debug.Log("Quảng cáo bị bỏ qua. Bạn không nhận thêm lượt kiểm tra.");
    //             break;
    //         case ShowResult.Failed:
    //             // Quảng cáo không thể chơi
    //             Debug.Log("Quảng cáo thất bại. Bạn không nhận thêm lượt kiểm tra.");
    //             break;
    //     }
    // }
} 