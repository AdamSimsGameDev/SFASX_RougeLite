using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MenuManager
{
    [Header("Main Menu")]
    public MenuElement[] saves;

    private void Start()
    {
        SetCurrentMenu("mainMenu");
    }
    private void Update ()
    {
        LoadSaveStates();
    }

    private void LoadSaveStates ()
    {
        for (int i = 0; i < 5; i++)
        {
            bool doesExist = File.Exists(Application.persistentDataPath + "/slot" + i.ToString() + ".save");

            int j = i;

            TextMeshProUGUI textUI = saves[i].transform.Find("Text").GetComponent<TextMeshProUGUI>();
            if (doesExist)
            {
                saves[i].onUse.AddListener(() => Global.instance.LoadGame(j));

                textUI.text = ("Save " + (i + 1).ToString()).ToUpper();
            }
            else
            {
                saves[i].onUse.AddListener(() => Global.instance.CreateNewGame(j));

                textUI.text = ("New Save").ToUpper();
            }
        }
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
