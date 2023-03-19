using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Error : MonoBehaviour
{
	protected bool quitToMenu;
	public GameObject confirmQuitMenu;
	public void Quit(bool toMenu)
	{
		quitToMenu = toMenu;
		confirmQuitMenu.SetActive(true);
	}
	public void Confirm()
	{
		if (quitToMenu)
		{
			Destroy(transform.parent.gameObject);
			SceneManager.LoadScene("Menu");
		}
		else
		{
			Application.Quit();
		}
	}

	public void Nevermind()
	{
		confirmQuitMenu.SetActive(false);
	}
}
