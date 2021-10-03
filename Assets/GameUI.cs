using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
	public Canvas canvas;

	public GameObject levelEnd;

	public Image comboImg;

	[SerializeField]
	Button playAgainButton;

	[SerializeField]
	TextMeshProUGUI scoreText;

	[SerializeField]
	TextMeshProUGUI multiplierText;

	[SerializeField]
	TextMeshProUGUI timerText;

	public Action onPlayAgainClick;

	void Awake () {
		playAgainButton.onClick.AddListener( OnPlayAgain );
	}

	private void Start () {
		ScoreManager.instance.onScoreChange += OnScoreChange;
		ScoreManager.instance.onMultiplierChange += OnMultiplierChange;
	}

	private void Update () {
		var p = ScoreManager.instance.scoreSinceLastMultiplier / ( ScoreManager.instance.multiplier * 100 );

		comboImg.fillAmount = p;

		var timer = ScrollManager.instance.timer;

		float minutes = Mathf.Floor( timer / 60 );
		float seconds = Mathf.RoundToInt( timer % 60 );

		timerText.text = minutes.ToString().PadLeft( 2, '0' ) + ":" + seconds.ToString().PadLeft( 2, '0' );
	}

	private void OnMultiplierChange ( int multiplier ) {
		multiplierText.text = $"x{multiplier}";
	}

	private void OnScoreChange ( float score, float oldScore ) {
		scoreText.text = $"{Mathf.FloorToInt( score ).ToString().PadLeft( 10, '0' )}";
	}

	void OnPlayAgain () {
		onPlayAgainClick?.Invoke();
		playAgainButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play Again";
	}

	public void EnableGameEnd () {
		levelEnd.SetActive( true );
	}

	public void DisableGameEnd () {
		levelEnd.SetActive( false );
	}
}