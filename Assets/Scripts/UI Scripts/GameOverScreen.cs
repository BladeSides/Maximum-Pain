using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] float _timer = 0f;
    [SerializeField] float _timeToEnd = 3f;
    [SerializeField] TextMeshProUGUI _gameoverText;
    // Start is called before the first frame update
    void Start()
    {
        _timer = 0f;
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameWon)
        {
            _gameoverText.text = "You\nWin";
        }
        else
        {
            _gameoverText.text = "Game\nOver";
        }
        _timer += Time.deltaTime;
        if (_timer > _timeToEnd)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
