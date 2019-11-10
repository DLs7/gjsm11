using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioManager audioManager;

    private GameObject particleSystem;

    private void Awake()
    {
        particleSystem = GameObject.Find("Particle System");
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(particleSystem);
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame

    public void PlayGame()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        foreach (GameObject gos in GameObject.FindGameObjectsWithTag("Snow"))
        {
            if (gos.name == "Snow(Clone)")
            {
                Destroy(gos);
            }
        }
        SceneManager.LoadScene(1);
        audioManager.Play("battle_song");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayGame();
        }
    }
}
