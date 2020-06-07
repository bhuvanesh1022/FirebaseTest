using DecisionFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;
using System;

public class ReignslikeGameManager : MonoBehaviour {

    public playerscore scoreObj;
    public user u;
    public string localID;

	[SerializeField] DecisionsHolder decHolder;
	[SerializeField] int turnsTotal, startingStat, minStat, maxStat;
	[SerializeField] List<string> statNames;
	[SerializeField] string endgameMessage = "THAT'S ALL FOLKS";
	[SerializeField] DecisionDisplay decisionDisplay;
	[SerializeField] TMP_Text welcomeText, statusText, outcomeText, lastTraitsText;
	[SerializeField] TextButton startButton, resetButton;
	[SerializeField] List<GameObject> showDuringGameplay, showDuringEndgame;

	List<Decision> poolAvailable = new List<Decision>(),
				   poolLocked = new List<Decision>(),
				   poolUsed = new List<Decision>();
	int turnsLeft;
	Dictionary<string, int> statMap = new Dictionary<string, int>(),
							traitMap = new Dictionary<string, int>();

	public delegate void SessionEndEvent(UserGameData traitResults);
	public static SessionEndEvent OnSessionEnded;

	private void Awake() {
        scoreObj = GameObject.FindWithTag("GameController").GetComponent<playerscore>();
		poolAvailable = new List<Decision>(decHolder.decisions);
		decisionDisplay.OnDecisionTaken += OnDecisionTaken;
		decisionDisplay.OnDecisionEnd += NextDecision;
		if (startButton) startButton.OnButtonPressed += _ => NextDecision();
        if (resetButton) resetButton.OnButtonPressed += _ => ReloadApp();
        statNames.ForEach(statName => statMap[statName] = startingStat);
	}

	private void Start() {
        u = scoreObj.user;
        welcomeText.text = "<line-height=150%>" + "Hi! " + u.userName.ToUpper();
        lastTraitsText.text += scoreObj.lastTraits != "" ?  "\n" + scoreObj.lastTraits : "";

        //localID = scoreObj.localid;
		turnsLeft = turnsTotal;
		Endgame(false);
		UpdateStatus();
		if (startButton) decisionDisplay.gameObject.SetActive(false);
		else NextDecision();
	}

    void ReloadApp()
    {
        Destroy(scoreObj.gameObject);
        SceneManager.LoadScene(0);
    }

    void UpdateStatus() {
		string newText = "<line-height=150%>" + (turnsLeft > 0 ? "Turns left: " + turnsLeft : endgameMessage);
		foreach (var kvp in statMap) newText += string.Format("\n<line-height=100%>{0}: {1}", kvp.Key, kvp.Value.ToString());
		statusText.text = newText;
	}

    void NextDecision() 
    {
        lastTraitsText.enabled = false;

		if (!decisionDisplay.gameObject.activeSelf) {
			if (startButton) startButton.gameObject.SetActive(false);
			decisionDisplay.gameObject.SetActive(true);
		}

		UpdateStatus();
		if (turnsLeft <= 0) 
        {
            scoreObj.onsubmit();
            Endgame(true);
			return;
		}

		bool IsDecisionAvailable(Decision dec) {
			return dec.requirements.TrueForAll(req => {
				int checkVal = 0;
				switch (req.property.type) {
				case Property.Type.STAT:
					if (!statMap.ContainsKey(req.property.name)) return false;
					checkVal = statMap[req.property.name];
					break;
				case Property.Type.TRAIT:
					if (!traitMap.ContainsKey(req.property.name)) return false;
					checkVal = traitMap[req.property.name];
					break;
				default:
					break;
				}
				return (!req.checkMin || checkVal >= req.minValue) && (!req.checkMin || checkVal <= req.maxValue);
			});
		}

		List<Decision> newlyAvailable = poolLocked.FindAll(dec => IsDecisionAvailable(dec));
		poolAvailable.AddRange(newlyAvailable);
		newlyAvailable.ForEach(dec => poolLocked.Remove(dec));

		if (poolAvailable.Count == 0) {
			poolAvailable.AddRange(poolUsed);
			poolUsed.Clear();
		}

		List<Decision> newlyLocked = poolAvailable.FindAll(dec => !IsDecisionAvailable(dec));
		poolLocked.AddRange(newlyAvailable);
		newlyLocked.ForEach(dec => poolAvailable.Remove(dec));

		Decision curDec = poolAvailable[UnityEngine.Random.Range(0, poolAvailable.Count)];
		poolUsed.Add(curDec);
		poolAvailable.Remove(curDec);
		decisionDisplay.ShowDecision(curDec);
	}

	void OnDecisionTaken(Choice choice) {
		foreach (var effect in choice.effects) {
			switch (effect.property.type) {
			case Property.Type.STAT:
				if (!statMap.ContainsKey(effect.property.name)) break;
				statMap[effect.property.name] = Mathf.Clamp(statMap[effect.property.name] + effect.amount, minStat, maxStat);
				break;
			case Property.Type.TRAIT:
				if (!traitMap.ContainsKey(effect.property.name)) traitMap[effect.property.name] = 0;
				traitMap[effect.property.name] += effect.amount;
				break;
			default:
				break;
			}
		}
		UpdateStatus();
		turnsLeft--;
	}

	void Endgame(bool show) 
    {
		showDuringEndgame.ForEach(go => go.SetActive(show));
		showDuringGameplay.ForEach(go => go.SetActive(!show));
		if (show) {
			string newText = string.Empty;
			foreach (var kvp in traitMap)
            {
                newText += (string.IsNullOrEmpty(newText) ? "" : "\n") + string.Format("{0}: {1}", kvp.Key, kvp.Value.ToString());
            }
            newText += "\n" + "\n" + "Traits Submitted.";
            outcomeText.text = newText;
		}
		OnSessionEnded?.Invoke(new UserGameData(traitMap));
	}
}