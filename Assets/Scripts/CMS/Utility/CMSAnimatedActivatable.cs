using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CMSAnimatedActivatable : MonoBehaviour {
	[SerializeField] string activeParam;
	[SerializeField] float deactivateTime;
	[SerializeField] bool startActive = true, hasDeactivateEvent = false;

	public bool IsActive { get; private set; }

	Animator anim;

	private void Awake() {
		anim = GetComponent<Animator>();
		IsActive = startActive;
		anim.SetBool(activeParam, IsActive);
		gameObject.SetActive(IsActive);
	}

	public void SetActive(bool isActive) {
		if (isActive == IsActive) return;
		IsActive = isActive;
		if (IsActive) gameObject.SetActive(true);
		else if (!hasDeactivateEvent) Deactivate();
		anim.SetBool(activeParam, IsActive);
	}

	void Deactivate() {
		StartCoroutine(DeactivateCR());
	}

	IEnumerator DeactivateCR() {
		yield return new WaitForSeconds(deactivateTime);
		gameObject.SetActive(false);
	}
}