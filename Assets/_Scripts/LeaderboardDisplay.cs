using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class LeaderboardDisplay : MonoBehaviour
{
	public TMP_Text namesText;
	public TMP_Text scoresText;
	public GameObject newScorePanel;
	public TMP_Text newScoreText;
	public TMP_InputField newScoreNameField;

	protected Leaderboard leaderboardObject;
	protected string newScoreName;

	protected List<KeyValuePair<string, int>> leaderboard;
	protected int newScore;

	private void Awake()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		leaderboardObject = GameObject.FindGameObjectWithTag("GameController").GetComponent<Leaderboard>();

		ShowLeaderboard();

		var scoreObject = leaderboardObject.gameObject.GetComponent<Score>();
		newScore = Mathf.FloorToInt(scoreObject.GetScore());

		//check if score is on leaderboard
		if (newScore >= leaderboard.Last().Value)
		{
			newScorePanel.SetActive(true);
			newScoreText.text = newScore.ToString();
		}
	}

	public void Menu()
	{
		SceneManager.LoadScene("Menu");
		Destroy(leaderboardObject.transform.parent.gameObject);
	}

	public void SubmitNewScore()
	{
		newScoreName = newScoreNameField.text.ToUpper();
		leaderboard.Add(new KeyValuePair<string, int>(newScoreName, newScore));
		leaderboard = leaderboard.OrderByDescending(kv => kv.Value).Take(10).ToList();
		newScorePanel.SetActive(false);
		leaderboardObject.UpdateLeaderboard(leaderboard);
		ShowLeaderboard();
	}

	protected void ShowLeaderboard()
	{
		leaderboard = leaderboardObject.GetLeaderBoard();
		string names = string.Empty;
		string scores = string.Empty;
		foreach (var pair in leaderboard)
		{
			names += pair.Key + "\n";
			scores += pair.Value + "\n";
		}
		names.TrimEnd('\n');
		scores.TrimEnd('\n');

		namesText.text = names;
		scoresText.text = scores;
	}
}
