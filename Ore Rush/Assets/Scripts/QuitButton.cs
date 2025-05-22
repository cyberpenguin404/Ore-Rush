using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuitButton : MonoBehaviour
{
    // Reference to the Button component
    public Button exitButton;

    void Start()
    {
        // Ensure the Button component is assigned
        if (exitButton != null)
        {
            // Add a listener to the button to call the ExitGame method when clicked
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    // Method to exit the game
    void ExitGame()
    {
        // If we are running in the Unity editor
#if UNITY_EDITOR
        // Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the application
        Application.Quit();
#endif
    }
}


