#pragma strict
#pragma implicit
#pragma downcast

class Destructable extends MonoBehaviour
{
	function Destruct()
	{
		if(transform.childCount > 0)
		{
			var t : Transform;

			for(var i : int = 0; i < transform.childCount; i++)
			{
				t = transform.GetChild(i);
				t.parent = null;
				#if UNITY_3_5
					t.gameObject.active = true;
				#endif
				#if UNITY_4
					t.gameObject.SetActive(true);
				#endif
				if(t.GetComponent.<Renderer>() != null)
				{
					t.GetComponent.<Renderer>().enabled = true;
				}
				
				if(t.GetComponent.<Rigidbody>() != null)
				{
					t.GetComponent.<Rigidbody>().isKinematic = false;
				}
				
				if(t.gameObject.GetComponent("TrainingDummyPartDestructor") == null)
				{
					t.gameObject.AddComponent.<TrainingDummyPartDestructor>();
				}
			}
			
			if(transform.parent != null)
			{
				Destroy(transform.parent.gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}