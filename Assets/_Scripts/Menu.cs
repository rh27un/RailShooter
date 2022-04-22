using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum MenuState
{
	Main = 0,
	Play = 1,
	Options = 2
}
public class Menu : MonoBehaviour
{
	MenuState state;
	[SerializeField]
	Transform main;
	[SerializeField]
	Transform play;
	[SerializeField]
	Transform options;

	[SerializeField]
	float speed;

	[SerializeField]
	string fileName;

	Camera cam;

	[SerializeField]
	GameObject contBtn;

	[SerializeField]
	Difficulty difficulty;



	[SerializeField]
	GameObject mainCanvas;
	[SerializeField]
	GameObject playCanvas;
	[SerializeField]
	GameObject optionsCanvas;

	protected Vector3 start;
	protected Vector3 end;
	[SerializeField]
	protected float t;

	protected float startTime;

	private void Awake()
	{
		if (!File.Exists(fileName))
		{
			contBtn.SetActive(false);
		}
		state = MenuState.Main;
		cam = Camera.main;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Time.timeScale = 1;
		start = main.position;
		end = main.position;
	}
	private void Update()
	{
		cam.transform.position = Vector3.Lerp(start, end, t);
		if(t < 1)
			t = Mathf.Clamp01((Mathf.Sin((Time.time - startTime) * Mathf.PI))) * 1.0001f;
	}
	public void Play()
	{
		mainCanvas.SetActive(false);
		playCanvas.SetActive(true);
		t = 0;
		startTime = Time.time;
		start = cam.transform.position;
		end = play.position;
		state = MenuState.Play;
	}
	public void Options()
	{
		mainCanvas.SetActive(false);
		optionsCanvas.SetActive(true);
		t = 0;
		startTime = Time.time;
		start = cam.transform.position;
		end = options.position;
		state = MenuState.Options;
	}
	public void NewGame()
	{
		if(File.Exists(fileName))
			File.Delete(fileName);

		SceneManager.LoadScene("Init");
	}
	public void Back()
	{
		mainCanvas.SetActive(true);
		playCanvas.SetActive(false);
		optionsCanvas.SetActive(false);
		t = 0;
		startTime = Time.time;
		start = cam.transform.position;
		end = main.position;
		state = MenuState.Main;
	}
	public void Continue()
	{
		SceneManager.LoadScene("Init");
	}

	public void SetLives(int num)
	{
		difficulty.lives = num;
	}
	public void SetContinues(int num)
	{
		difficulty.continues = num;
		difficulty.infiniteContinues = false;
	}
	public void InfiniteContinues()
	{
		difficulty.infiniteContinues = true;
	}


	public void Quit()
	{
		Application.Quit();
	}
}
