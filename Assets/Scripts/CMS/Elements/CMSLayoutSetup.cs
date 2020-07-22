using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CMSLayoutElement : MonoBehaviour {
	[SerializeField] protected Button removeButton;

	public delegate void RemoveEvent(CMSLayoutElement element);
	public RemoveEvent OnRemove;

	protected virtual void Awake() {
		if (removeButton) removeButton.onClick.AddListener(() => OnRemove?.Invoke(this));
	}
}

public class CMSLayoutSetup : MonoBehaviour {
	[SerializeField] LayoutGroup layout;
	[SerializeField] Button addButton;
	[SerializeField] Transform setAsLastCell;

	public delegate void AddButtonPressEvent();
	public AddButtonPressEvent OnAddButtonPressed;

	ContentSizeFitter fitter;
	int waitForLayoutRebuild = 0;

	static Dictionary<CMSLayoutSetup, int> allSetups = new Dictionary<CMSLayoutSetup, int>();
	static int maxChildDepth = 1;

	private void Awake() {
		if (addButton) addButton.onClick.AddListener(() => OnAddButtonPressed?.Invoke());
		if (layout) fitter = layout.GetComponent<ContentSizeFitter>();
		RegisterLayout(this);
	}

	private void OnDestroy() {
		DeregisterLayout(this);
	}

	public Transform InstantiateInLayout(Transform prefab) {
		Transform retval = Instantiate(prefab, transform);
		if (setAsLastCell && setAsLastCell.parent == layout.transform) setAsLastCell.SetAsLastSibling();
		return retval;
	}

	static void RegisterLayout(CMSLayoutSetup setup) {
		int childDepth;
		Transform curParent = setup.transform.parent;
		for (childDepth = 0; curParent != null; childDepth++) curParent = curParent.parent;
		maxChildDepth = Mathf.Max(maxChildDepth, childDepth);
		allSetups[setup] = childDepth;
	}

	static void DeregisterLayout(CMSLayoutSetup setup) {
		if (allSetups.ContainsKey(setup)) allSetups.Remove(setup);
	}

	public void UpdateLayout() {
		if (fitter) LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)fitter.transform);
	}

	public static void UpdateLayouts() {
		// Update layouts in reverse order of child depth
		foreach (var kvp in allSetups){
			if (!kvp.Key) continue;
			kvp.Key.waitForLayoutRebuild = maxChildDepth - kvp.Value;
			kvp.Key.enabled = true;
		}
	}

	private void LateUpdate() {
		if (--waitForLayoutRebuild >= 0) return;
		UpdateLayout();
		enabled = false;
		waitForLayoutRebuild = 0;
	}
}

public class CMSLayout<CellType> where CellType : CMSLayoutElement {
	public List<CellType> Elements { get; private set; } = new List<CellType>();

	public delegate void CellAddRemoveEvent(CellType cell, bool wasAdded);
	public CellAddRemoveEvent OnCellAddedOrRemoved;

	CMSLayoutSetup setup;
	CellType cellPrefab;

	public CMSLayout(CMSLayoutSetup setup, CellType cellPrefab) {
		this.setup = setup;
		this.cellPrefab = cellPrefab;

		setup.OnAddButtonPressed += () => {
			CellType newCell = Add();
			CMSLayoutSetup.UpdateLayouts();
			OnCellAddedOrRemoved?.Invoke(newCell, true);
		};
	}

	public void Initialize() {
		CMSLayoutSetup.UpdateLayouts();
	}

	public CellType Add() {
		CellType newCell = setup.InstantiateInLayout(cellPrefab.transform).GetComponent<CellType>();
		Elements.Add(newCell);
		newCell.OnRemove += cell => {
			Elements.Remove(newCell);
			OnCellAddedOrRemoved?.Invoke(newCell, false);
			Object.Destroy(newCell.gameObject);
			CMSLayoutSetup.UpdateLayouts();
		};
		return newCell;
	}

	public void Clear() {
		Elements.ForEach(cell => Object.Destroy(cell.gameObject));
		Elements.Clear();
	}
}