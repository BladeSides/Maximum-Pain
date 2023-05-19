using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private float _defaultDeltaTime = 0.02f; //Unity's Default Fixed Delta Time
    [SerializeField] private GameObject _instructionsText;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = _defaultDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowInstructionsText()
    {
        _instructionsText.SetActive(!_instructionsText.activeSelf);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
