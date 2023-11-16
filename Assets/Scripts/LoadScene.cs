using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
public GameObject DialogPrefab;
 private GameObject instantiatedDialog;
    public string sceneName;
    // Start is called before the first frame update
    public void ChangeScene()
    {
      

        SceneManager.LoadScene(sceneName);
    }

     public void Menu()
    {
        instantiatedDialog = Instantiate(DialogPrefab);
        instantiatedDialog.SetActive(true);
        gameObject.SetActive(true);
    }
   public void CloseMenu()
    {
        if (instantiatedDialog != null)
        {
            instantiatedDialog.SetActive(false);
        }
    }
}
