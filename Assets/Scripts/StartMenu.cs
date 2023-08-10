using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class StartMenu : MonoBehaviour
{
    public InputField playerNameInput;

    public void StartGame()
    {
        string playerName = playerNameInput.text;
        PlayerPrefs.SetString("PlayerName", playerName); // Save the player's name
        SceneManager.LoadScene(1); // Load the Main Game scene

    }
}