using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMaster : MonoBehaviour

{
    public GameObject mainMenuGroup;
    public GameObject multiplayerMenuGroup;

    public enum Scenes
    {
        GameScene,
        Menu,
        Loading
    }

    public void Load(Scenes scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadHotseat()
    {
        Load(Scenes.GameScene);
        Debug.Log("Loading the hotseat mode.");
    }

    public void ToggleMultiplayerMenu()
    {
        mainMenuGroup.SetActive(!mainMenuGroup.activeSelf);
        multiplayerMenuGroup.SetActive(!multiplayerMenuGroup.activeSelf);
    }
}