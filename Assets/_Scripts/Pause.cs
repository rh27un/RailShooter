using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Pause : MonoBehaviour
{
	public GameObject pauseMenu;
	public GameObject confirmQuitMenu;
	public GameObject optionsCanvas;
	[TextArea]
	public string defaultConfirmText;
	[TextArea]
	public string noCreditsConfirmText;
	[TextArea]
	public string infiniteCreditsConfirmText;
    public bool isPause;
	PlayerHealth health;
	public TMP_Text confirmText;
	public bool quitToMenu;
	private void Awake()
	{
		health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
	}
	void Update()
    {
		if (Input.GetButtonDown("Cancel"))
		{
            isPause = !isPause;
			if (isPause)
			{
				Time.timeScale = 0;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				pauseMenu.SetActive(true);
			}
			else
			{
				Time.timeScale = 1;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				pauseMenu.SetActive(false);
				confirmQuitMenu.SetActive(false);
				optionsCanvas.SetActive(false);
			}
		}
    }

	public void Resume()
	{
		isPause = false;
		Time.timeScale = 1;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		pauseMenu.SetActive(false);
		confirmQuitMenu.SetActive(false);
	}
	public void Quit(bool toMenu)
	{
		quitToMenu = toMenu;
		confirmQuitMenu.SetActive(true);
		if(health.continues == -1)
		{
			confirmText.text = infiniteCreditsConfirmText;
		}
		else if(health.continues == 0)
		{
			confirmText.text = noCreditsConfirmText;
		}
		else
		{
			confirmText.text = defaultConfirmText;
		}
	}
	public void Confirm()
	{
		if (quitToMenu)
		{
			Destroy(transform.parent.gameObject);
			SceneManager.LoadScene("Menu");
		} else
		{
			Application.Quit();
		}
	}

	public void Nevermind()
	{
		confirmQuitMenu.SetActive(false);
	}

	public void Options()
	{
		optionsCanvas.SetActive(true);
		pauseMenu.SetActive(false);
	}
	public void Back()
	{
		optionsCanvas.SetActive(false);
		pauseMenu.SetActive(true);
	}
}
