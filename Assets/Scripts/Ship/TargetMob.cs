using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TargetMob : MonoBehaviour {
	// Use this for initialization
	public List<Transform> targets;
	public Transform SelectTarget;
	
	private Transform myTransform;
	
	void Start () {
		targets = new List<Transform>();
		addallenemys();
		SelectTarget = null;
		myTransform = transform;
	}
	
	public void addallenemys()
	{
		GameObject[] go = GameObject.FindGameObjectsWithTag("FlakCargo");  
		foreach(GameObject enemy in go)
			AddTarget(enemy.transform);
	}
	
	
	public void AddTarget(Transform enemy)
	{
		targets.Add(enemy);
	}
	
	
	private void SortTargetsByDsitance()
	{
		targets.Sort(delegate(Transform t1, Transform t2)
		             { return (Vector3.Distance(t1.position, myTransform.position).CompareTo(Vector3.Distance(t2.position, myTransform.position)));  
		});
	}
	private void targetenemy()
	{
		if(SelectTarget == null)
		{
			SortTargetsByDsitance();
			SelectTarget = targets[0];
		}
		else
		{
			int index = targets.IndexOf(SelectTarget);
			if(index < targets.Count - 1)
			{
				index++;
			}
			else
			{
				index = 0;
			}
			SelectTarget = targets[index];
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.E))
			targetenemy();
	}
}