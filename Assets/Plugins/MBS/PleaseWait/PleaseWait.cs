using UnityEngine;
using System.Collections;

namespace MBS {
	public class PleaseWait : MonoBehaviour {

		static PleaseWait _instance = null;
		static public PleaseWait Instance 
		{ 
			get 
			{ 
				if (null == _instance)
				{
					PleaseWait[] objects = GameObject.FindObjectsOfType<PleaseWait>();
					if (null != objects && objects.Length > 0)
					{
						_instance = objects[0];
					}
					if (null == _instance)
					{
						GameObject obj = new GameObject("PleaseWait");
						_instance = obj.AddComponent<PleaseWait>();
						DontDestroyOnLoad(obj);
					}
				}
				return _instance; 
			} 
		}

		static float		rotAngle = 0;
		static Vector2		pivotPoint = Vector2.zero;
		static Rect			pos = new Rect(0,0,0,0);

		static Vector2 PivotPoint
		{
			get 
			{
				if (pivotPoint == Vector2.zero)
					PivotPoint	= new Vector2(Screen.width / 2f, Screen.height / 2f);
				return pivotPoint;
			}

			set
			{
				pivotPoint = value;
				pos = new Rect(0,0,0,0);
			}
		}

		static Rect	Pos
		{ 
			get 
			{
				if (pos == new Rect(0,0,0,0))
					if (null == PleaseWaitSpinner)
						return new Rect(0,0,0,0);
					else
					pos	= new Rect(	pivotPoint.x - (Instance.pleaseWaitSpinner.width / 2),
					               pivotPoint.y - (Instance.pleaseWaitSpinner.height / 2),
					               Instance.pleaseWaitSpinner.width,
					               Instance.pleaseWaitSpinner.height);

				return pos;
			} 
		}

		static public float	SpinnerSpeed	
		{ 
			get 
			{ 
				return Instance.spinnerSpeed; 
			} 
			set 
			{ 
				Instance.spinnerSpeed = value;
			} 
		} 

		static public Texture2D	PleaseWaitSpinner
		{
			get
			{
				if (null == Instance.pleaseWaitSpinner)
				{
					Instance.pleaseWaitSpinner = (Texture2D)Resources.Load("Spinner", typeof(Texture2D));
					PivotPoint = Vector2.zero;
				}

				return Instance.pleaseWaitSpinner;
			}

			set
			{
				if (value != null)
				{
					Instance.pleaseWaitSpinner = value;
					pos = new Rect();
				}
			}
		}

		bool				IsInstanceCopy { get { return this == _instance; } }

		public float		spinnerSpeed = -300f;
		public Texture2D	pleaseWaitSpinner;

		void Update() {
			rotAngle -= SpinnerSpeed * Time.deltaTime;
		}
	
		void Start() {
			//we only want one in the scene so keep the first one and delete all
			//other objects with this component attached.
			if (null == _instance) {
				_instance = this;
				DontDestroyOnLoad(gameObject);
			} else {
				PleaseWait[] ob = GameObject.FindObjectsOfType<PleaseWait>();
				foreach (Object o in ob)
					if ( !((PleaseWait)o).IsInstanceCopy) {
						print( "Deleting duplicate object " + ((PleaseWait)o).transform.name);
						Destroy(((PleaseWait)o).gameObject);
					}
			}
		}
	
		static public void Draw() {
			if (null == PleaseWaitSpinner) {
				print("You don't have a PleaseWait spinner graphic defined! i.e. Assets/Resources/Spinner.psd");
				return;
			}
			
			GUI.matrix	= Matrix4x4.identity;
		    GUIUtility.RotateAroundPivot (rotAngle, PivotPoint); 
			GUI.DrawTexture( Pos, PleaseWaitSpinner);
			GUIUtility.RotateAroundPivot (-rotAngle, PivotPoint); 
		}
	}

}
