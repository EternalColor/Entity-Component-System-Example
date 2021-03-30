using System;
using System.Collections;
using System.Collections.Generic;
using FindTheIdol.Utilities.Constants;
using FindTheIdol.Utilities.Cursor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject startGameButtonGameObject;

    [SerializeField]
    private GameObject continueGameButtonGameObject;

    [SerializeField]
    private GameObject optionsButtonGameObject;

    [SerializeField]
    private GameObject aboutButtonGameObject;

    [SerializeField]
    private GameObject aboutPanelGameObject;

    [SerializeField]
    private GameObject quitAboutButtonGameObject;
    
    [SerializeField]
    private GameObject quitButtonGameObject;

    private Button startGameButton;  

    private Button continueGameButton;  

    private Button optionsButton;  

    private Button aboutButton;

    private Button quitAboutButton;

    private Button quitButton;  


    private void Start()
    {   
        CursorStateModifier.ModifyCursorState(false);

        this.startGameButton = this.startGameButtonGameObject.GetComponent<Button>();
        this.startGameButton.onClick.AddListener(StartGame);

        this.continueGameButton = this.continueGameButtonGameObject.GetComponent<Button>();
        this.continueGameButton.onClick.AddListener(ContinueGame);

        this.optionsButton = this.optionsButtonGameObject.GetComponent<Button>();
        this.optionsButton.onClick.AddListener(OpenOptionsMenu);

        //Hide panel until about button is pressed
        this.aboutPanelGameObject.SetActive(false);    

        this.aboutButton = this.aboutButtonGameObject.GetComponent<Button>();
        this.aboutButton.onClick.AddListener(delegate { DisplayAboutPanel(true); });

        this.quitAboutButton = this.quitAboutButtonGameObject.GetComponent<Button>();
        this.quitAboutButton.onClick.AddListener(delegate { DisplayAboutPanel(false); });

        this.quitButton = this.quitButtonGameObject.GetComponent<Button>();
        this.quitButton.onClick.AddListener(QuitApplication);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("CharacterCreator");
    }

    private void ContinueGame()
    {
        throw new NotImplementedException("I said not implemented :)");
    }

    private void OpenOptionsMenu()
    {
        throw new NotImplementedException("I said not implemented :)");
    }

    private void DisplayAboutPanel(bool setActive)
    {
        this.aboutPanelGameObject.SetActive(setActive);
    }

    private void QuitApplication()
    {
        Application.Quit();
    }
}
