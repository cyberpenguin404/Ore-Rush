using UnityEngine;
using UnityEngine.SceneManagement;
public class BackButton : MonoBehaviour
{
  
    public void LoadScene()
    {
        SceneManager.LoadScene("Title");
    }
}