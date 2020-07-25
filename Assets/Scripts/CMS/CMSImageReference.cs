using DecisionFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CMSImageReference : MonoBehaviour {
    [SerializeField] CMSInputText description;

    public ImageReference ImageReference { get; private set; }

    public void Initialize(ImageReference imageRef = null) {
        ImageReference = imageRef ?? new ImageReference();
        description.Initialize(ImageReference.description);
    }

    public void Refresh() {
        ImageReference.description = description.Text;
    }
}