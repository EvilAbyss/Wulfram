using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MBS;

public class WUUTabGroup : MonoBehaviour {

	public ScrollRect
		content;

	public RectTransform
		content_area;

	public Button[]
		tab_buttons;

	public void SelectTab(int index)
	{
		if (index >= tab_buttons.Length) return;
		content.horizontalNormalizedPosition = (float)index / tab_buttons.Length;
		int counter = 0;
		foreach(Button b in tab_buttons)
			b.interactable = index != counter++;
	}

	// Use this for initialization
	void Start () {
		int runner = 0;
		foreach(Button b in tab_buttons)
		{
			int tempval = runner;
			b.onClick.AddListener(() => SelectTab(tempval) );
			runner++;
		}
		SelectTab(0);
	}
}
