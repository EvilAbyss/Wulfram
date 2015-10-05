#pragma strict
#pragma implicit
#pragma downcast

enum MainMenuState
{
	IDLE,
	OPTIONS,
	GRAPHICS,
	ABOUT,
	CONECTION,
}

// The MainMenuScreen deals with all 
// menus throughout the demo. 
class MainMenuScreen extends MonoBehaviour
{
 	public var isUnityDebug : boolean;
    private var nameRoom : String = "Default Room";
    public var _usePhoton : GameObject;
    public var ShowMainConect : boolean;
    public var gameManager : GameManager;
	public var menuBackground : Texture2D;
	private var menuBackgroundRect : Rect;
	private var limit : int = 8;
	public var windowBackground : Texture2D;
	private var windowBackgroundRect : Rect;
	
	public var playGame : Texture2D;
	public var playGameOver : Texture2D;
	private var playGameRect : Rect;
	
	public var resume : Texture2D;
	public var resumeOver : Texture2D;
	private var resumeRect : Rect;
	
	public var toLobby : Texture2D;
	public var toLobbyOver : Texture2D;
	private var toLobbyRect : Rect;
	
	public var options : Texture2D;
	public var optionsOver : Texture2D;
	private var optionsRect : Rect;
	
	public var graphics : Texture2D;
	public var graphicsOver : Texture2D;
	private var graphicsRect : Rect;
	
	public var about : Texture2D;
	public var aboutOver : Texture2D;
	private var aboutRect : Rect;
	
	public var hudSkin : GUISkin;
	
	private var panelLeft : GUIStyle;
	private var panelLeftRect : Rect;
	
	private var panelRight : GUIStyle;
	private var panelRightRect : Rect;
	
	private var descriptionStyle : GUIStyle;
	private var titleStyle : GUIStyle;
	private var customBox : GUIStyle;
	
	private var mousePos : Vector2;
	private var screenSize : Vector2;
	
	private var evt : Event;
	
	public var state : MainMenuState;
	private var lastMouseTime : float;
	
	public var receiveDamageOn : Texture2D;
	public var receiveDamageOff : Texture2D;
	public var dontReceiveDamageOn : Texture2D;
	public var dontReceiveDamageOff : Texture2D;
	private var damageRect : Rect;
	
	private var scrollPosition : Rect;
	private var scrollView : Rect;
	private var scroll : Vector2;
	
	public var black : Texture2D;
	private var alpha : float;
	static public var goingToGame : boolean;
    static public var showProgress : boolean;
	
	private var aboutScroll : Vector2;
    private var connectScroll : Vector2;
	private var graphicsScroll : Vector2;
	private var aboutStyle : GUIStyle;
	private var conectStyle: GUIStyle;
	private var resumeGame : boolean;
	public var visible : boolean;
	
	private var sliderBackground : GUIStyle;
	private var sliderButton : GUIStyle;
	
	public var greenBar : Texture2D;
	public var checkOn : Texture2D;
	public var checkOff : Texture2D;
	public var whiteMarker : Texture2D;
	
	private var margin : float  = 30;

	private var questionRect : Rect;
	private var greenRect : Rect;
	private var tooltipStyle : GUIStyle;
	private var questionButtonStyle : GUIStyle;
	
	private var aquirisLogo : GUIStyle;
	private var unityLogo : GUIStyle;
	private var textFieldStyle : GUIStyle;
	public var overSound : AudioClip;
	public var overVolume : float = 0.4;

	public var clickSound : AudioClip;
	public var clickVolume : float = 0.7;
	
	private var over : boolean;
	private var hideOptions : boolean;
    private var loadingIndustry : boolean;

    public var textStyle : GUIStyle;
	private var angle : float;
    public var loadingBackground : Texture2D;
    
    var GameList : Hashtable = new Hashtable();

    function PushList(_gameList : Hashtable)
    {
        if(_gameList != GameList)
        {
            GameList.Clear();
            GameList = _gameList;
        }
    }
    function ShowMainConection()
    {
        ShowMainConect = true;
    }

	// Initialization
	function Awake()
	{
        angle = 0.0;
		over = false;
        loadingIndustry = false;
        showProgress = false;
		hideOptions = Application.loadedLevelName != "demo_industry";
		GameManager.receiveDamage = true;
		questionButtonStyle = hudSkin.GetStyle("QuestionButton");
		tooltipStyle = hudSkin.GetStyle("TooltipStyle");
		aquirisLogo = hudSkin.GetStyle("AquirisLogo");
		unityLogo = hudSkin.GetStyle("UnityLogo");
		questionRect = new Rect(0, 0, 11, 11);
		textFieldStyle = hudSkin.GetStyle("textField");
		alpha = 1.0;
		goingToGame = false;
		resumeGame = false;
		
		state = MainMenuState.IDLE;
		
		descriptionStyle = hudSkin.GetStyle("Description");
		titleStyle = hudSkin.GetStyle("Titles");
		customBox = hudSkin.GetStyle("CustomBox");
		panelLeft = hudSkin.GetStyle("PanelLeft");
		panelRight = hudSkin.GetStyle("PanelRight");
		aboutStyle = hudSkin.GetStyle("About");
		conectStyle = hudSkin.GetStyle("Conect");
		
		menuBackgroundRect = new Rect(0, 0, menuBackground.width, menuBackground.height);
		windowBackgroundRect = new Rect(0, 0, windowBackground.width, windowBackground.height);

		panelLeftRect = new Rect(0, 0, 235, 240);
		panelRightRect = new Rect(0, 0, 235, 240);
		playGameRect = new Rect(0, 0, playGame.width * 0.75, playGame.height * 0.75);
		optionsRect = new Rect(0, 0, options.width * 0.75, options.height * 0.75);
		graphicsRect = new Rect(0, 0, graphics.width * 0.75, graphics.height * 0.75);
		aboutRect = new Rect(0, 0, about.width * 0.75, about.height * 0.75);
		toLobbyRect = new Rect(0, 0, toLobby.width * 0.75, toLobby.height * 0.75);
		resumeRect = new Rect(0, 0, resume.width * 0.75, resume.height * 0.75);
		damageRect = new Rect(0, 0, receiveDamageOn.width * 0.7, receiveDamageOn.height * 0.7);
	}
	
	function Update()
	{
		if(goingToGame)
		{
			alpha += Time.deltaTime;
			
			if(alpha >= 1.0)
			{
                if(!loadingIndustry)
                {
                    loadingIndustry = true;
				    Application.LoadLevelAsync("demo_industry");
                }
			}
		}
		else
		{
		
			if(alpha > 0 && gameManager.showMenu == false)
			{
				alpha -= Time.deltaTime * 0.5;
			}
		}
		
        if(Time.timeScale == 0.0 || GameManager.showMenu)
        {
		    lastMouseTime -= 0.01;
        }

        if(showProgress)
        {
            angle -= Time.deltaTime * 360;

            if(angle < -360.0)
            {
                angle += 360.0;
            }
        }
	}
	
	// Decide which MainMenuState 
	// is to be shown. 
	function DrawGUI(event : Event)
	{
		evt = event;
		screenSize = new Vector2(Screen.width, Screen.height);
		mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		
		if(visible)
		{
			if(state != MainMenuState.IDLE)
			{
				windowBackgroundRect.x = menuBackgroundRect.x + menuBackgroundRect.width;
				windowBackgroundRect.y = (screenSize.y - windowBackgroundRect.height) * 0.5;
			
				GUI.DrawTexture(windowBackgroundRect, windowBackground);
				
				if(state == MainMenuState.GRAPHICS || state == MainMenuState.ABOUT || state == MainMenuState.CONECTION)
				{
					panelLeftRect.width = 475;
					panelLeftRect.x = windowBackgroundRect.x + 15;
					panelLeftRect.y = windowBackgroundRect.y + 55;
			
					GUI.Box(panelLeftRect, "", panelLeft);
				}
			}
			
			if(state == MainMenuState.OPTIONS)
			{
				DrawGameOptions();
			}
			else if(state == MainMenuState.GRAPHICS)
			{
				DrawGraphicOptions();
			}
			else if(state == MainMenuState.ABOUT)
			{
				DrawAbout();
			}
			else if(state == MainMenuState.CONECTION)
			{
			    DrawConection();
			}
			
			// Draws the small side-Menu where the 
			// above MainMenuStates can be choosen. 
			DrawMenu();
		}
        
      		
        if(alpha > 0.0)
        {
		    var c : Color = GUI.color;
		    var d : Color = c;
		    d.a = alpha;
		    GUI.color = d;
		
		    GUI.DrawTexture(new Rect(0, 0, screenSize.x, screenSize.y), black);

            if(goingToGame)
            {
		        GUI.Label(new Rect(Screen.width - 120, Screen.height - 40, 90, 20), "Loading...", textStyle);
            }
		    GUI.color = c;
        }
        if(ShowMainConect)
            MainConection();
	}
	
	// Shows the gameOptions menu
	function DrawGameOptions()
	{
		panelLeftRect.width = 235;
		panelLeftRect.x = windowBackgroundRect.x + 15;
		panelLeftRect.y = windowBackgroundRect.y + 55;

		panelRightRect.x = panelLeftRect.x + 5 + panelLeftRect.width;
		panelRightRect.y = panelLeftRect.y;
		
		damageRect.x = panelLeftRect.x + ((panelLeftRect.width - damageRect.width) * 0.5);
		damageRect.y = panelLeftRect.y + ((panelLeftRect.height - damageRect.height) * 0.5);
		
		var dRect = damageRect;
		dRect.x = panelRightRect.x + ((panelRightRect.width - damageRect.width) * 0.5);
		
		if(evt.current.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
		{
			if(damageRect.Contains(mousePos))
			{
				if(!GameManager.receiveDamage)
				{
					GetComponent.<AudioSource>().volume = clickVolume;
					GetComponent.<AudioSource>().PlayOneShot(clickSound);
					GameManager.receiveDamage = true;	
					lastMouseTime = Time.time;
				}
			}
			else if(dRect.Contains(mousePos))
			{
				if(GameManager.receiveDamage)
				{
					GetComponent.<AudioSource>().volume = clickVolume;
					GetComponent.<AudioSource>().PlayOneShot(clickSound);
					GameManager.receiveDamage = false;
					lastMouseTime = Time.time;
				}
			}
		}
		
		if(GameManager.receiveDamage)
		{
			GUI.DrawTexture(damageRect, receiveDamageOn);
			GUI.DrawTexture(dRect, dontReceiveDamageOff);
		}
		else
		{
			GUI.DrawTexture(damageRect, receiveDamageOff);
			GUI.DrawTexture(dRect, dontReceiveDamageOn);
		}
		
		GUI.Label(new Rect(windowBackgroundRect.x + 20, windowBackgroundRect.y + 15, 200, 20), "GAME OPTIONS", titleStyle);	
	}
	
	
	private var sceneConf : SceneSettings;
	function GetSceneRef()
	{
		//var currentScene : int = Application.loadedLevel;
		var currentScene : int;// = Application.loadedLevel;
		
        if(Application.loadedLevelName == "demo_start_cutscene")
        {
            currentScene = 0;
        }
        else if(Application.loadedLevelName == "demo_forest")
        {
            currentScene = 1;
        }
        else if(Application.loadedLevelName == "demo_industry")
        {
            currentScene = 2;
        }

		if(GameQualitySettings.scenes != null)
		{
			if(currentScene >= 0 && currentScene < GameQualitySettings.scenes.Length)
			{
				sceneConf = GameQualitySettings.scenes[currentScene];		
			}
			else
			{
				currentScene = -1;
			}
		}
		else
		{
			currentScene = -1;
		}
	}
	
	private function DrawSliderOverlay (rect : Rect, val : float)
	{
		rect.width = Mathf.Clamp(val * 199.0, 0.0, 199.0);
		GUI.DrawTexture (rect, greenBar);
	}
		
	private function SettingsSlider (name : String, nameLen : int, tooltip : String, v : float, vmin : float, vmax : float, dispname : String, dispmul : float, dispadd : float)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.BeginVertical();
		GUILayout.Label (name);
				
		questionRect.x = margin + nameLen;
		questionRect.y += 39;
		GUI.Button(questionRect, GUIContent(String.Empty, tooltip), questionButtonStyle);
		
		v = GUILayout.HorizontalSlider(v, vmin, vmax, GUILayout.Width(210));
		greenRect.y += 39;
		DrawSliderOverlay (greenRect, Mathf.InverseLerp (vmin, vmax, v));
		
		var disp = v * dispmul + dispadd;
		GUI.Label(new Rect(greenRect.x + 220, greenRect.y - 7, 200, 20), dispname + disp.ToString("0.00"));
		
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		return v;
	}
	
	private function SettingsIntSlider (name : String, nameLen : int, tooltip : String, v : int, vmin : int, vmax : int, dispnames : String[])
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.BeginVertical();
		GUILayout.Label (name);
				
		questionRect.x = margin + nameLen;
		questionRect.y += 39;
		GUI.Button(questionRect, GUIContent(String.Empty, tooltip), questionButtonStyle);
		
		v = GUILayout.HorizontalSlider(v, vmin, vmax, GUILayout.Width(210));
		greenRect.y += 39;
		DrawSliderOverlay (greenRect, Mathf.InverseLerp (vmin, vmax, v));
		
		GUI.Label(new Rect(greenRect.x + 220, greenRect.y - 7, 200, 20), dispnames == null ? v.ToString() : dispnames[v]);
		
		if (Mathf.Abs(vmin-vmax) < 8)
			DrawMarker (greenRect.y + 5, Mathf.Abs(vmin-vmax));
		
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		return v;
	}
	
	private function SettingsSpace (pix:int)
	{
		GUILayout.Space (pix);
		questionRect.y += pix;
		greenRect.y += pix;
	}
	
	private function SettingsToggle (v : boolean, name : String, nameLen : int, tooltip : String) : boolean
	{
		GUILayout.BeginVertical();
		GUILayout.Space(7);
		v = GUILayout.Toggle (v, v ? checkOn : checkOff, GUILayout.Width(14), GUILayout.Height(14));
		GUILayout.EndVertical();
		GUILayout.Space(5);
		GUILayout.Label(name);
		questionRect.x = margin + nameLen;
		GUI.Button (questionRect, GUIContent(String.Empty, tooltip), questionButtonStyle);
		return v;
	}
	
	private function BeginToggleRow ()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal(GUILayout.Width(400));
		questionRect.y += 21;
	}
	
	private function EndToggleRow (pix : int)
	{
		GUILayout.Space (pix);		
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	// Shows the graphicOptions menu
	function DrawGraphicOptions()
	{
		GetSceneRef();
//        var currentQualityLevel : int = QualitySettings.currentLevel;

        var originalColor : Color = GUI.color;

		if(sceneConf == null) return;
		
		GUI.Label(new Rect(windowBackgroundRect.x + 20, windowBackgroundRect.y + 15, 200, 20), "GRAPHICS OPTIONS", titleStyle);	
		
		var graphicRect : Rect = new Rect(panelLeftRect.x + 7, panelLeftRect.y + 30, panelLeftRect.width - 9, panelLeftRect.height - 35);
		
		var cSkin : GUISkin = GUI.skin;
		GUI.skin = hudSkin;
		
		greenRect = new Rect(margin, 0, 210, 5);

		GUILayout.BeginArea(graphicRect);
		graphicsScroll = GUILayout.BeginScrollView(graphicsScroll, GUILayout.Width(graphicRect.width));
		
		var boxRect = new Rect(17, 0, 430, 0);
		// overall level
		boxRect.height = 18 + 39;
		GUI.Box(boxRect, "", customBox);
		// post-fx
		boxRect.y += 10 + boxRect.height;
		boxRect.height = 93;
		GUI.Box(boxRect, "", customBox);
		// distances
		boxRect.y += 10 + boxRect.height;
		boxRect.height = 18 + 39;
		GUI.Box(boxRect, "", customBox);
		// shadow distance
		boxRect.y += 10 + boxRect.height;
		boxRect.height = 18 + 39;
		GUI.Box(boxRect, "", customBox);
		// texture limit
		boxRect.y += 10 + boxRect.height;
		boxRect.height = 18 + 39;
		GUI.Box(boxRect, "", customBox);
		// terrain
		boxRect.y += 10 + boxRect.height;
		boxRect.height = 18 + 39 * 7;
		GUI.Box(boxRect, "", customBox);
		
		GUILayout.BeginVertical();
		questionRect.y = -31;
		greenRect.y = -9;
		
		GameQualitySettings.overallQuality = SettingsIntSlider (
			"Overall Quality Level", 125, "Overall quality level of the game.",
			GameQualitySettings.overallQuality, 0, 5,
			["QUALITY: FASTEST", "QUALITY: FAST", "QUALITY: SIMPLE", "QUALITY: GOOD", "QUALITY: BEAUTIFUL", "QUALITY: FANTASTIC"]);
		
		GUILayout.Space(29);
		questionRect.y += 47;
		
		BeginToggleRow ();
		GameQualitySettings.anisotropicFiltering = SettingsToggle (GameQualitySettings.anisotropicFiltering, "Anisotropic Filtering", 153, "Anisotropic filtering for the textures.");
		GUILayout.FlexibleSpace();
		GameQualitySettings.ambientParticles = SettingsToggle (GameQualitySettings.ambientParticles, "Ambient Particles", 355, "Smoke & dust particles.");
		EndToggleRow (50);

		BeginToggleRow ();
		GameQualitySettings.colorCorrection = SettingsToggle (GameQualitySettings.colorCorrection, "Color Correction", 128, "Color correction image effect.");
        GUILayout.FlexibleSpace();
		GameQualitySettings.bloomAndFlares = SettingsToggle (GameQualitySettings.bloomAndFlares, "Bloom & Flares", 336, "Bloom & Lens Flares image effect.");
		EndToggleRow (71);

		BeginToggleRow ();
		GameQualitySettings.sunShafts = SettingsToggle (GameQualitySettings.sunShafts, "Sun Shafts", 100, "Sun Shafts image effect.");
		GUILayout.FlexibleSpace();
		GameQualitySettings.depthOfFieldAvailable = SettingsToggle (GameQualitySettings.depthOfFieldAvailable, "Depth Of Field", 336, "Depth Of Field image effect while aiming.");
		EndToggleRow (73);

		BeginToggleRow ();	
		var ssaoEnable : boolean = SettingsToggle (GameQualitySettings.ssao, "SSAO", 60, "Screen Space Ambient Ccclusion image effect.");
		if(GameQualitySettings.overallQuality > 3)	
			 GameQualitySettings.ssao = ssaoEnable;
		GUILayout.FlexibleSpace();
		EndToggleRow (0);
		
        greenRect.y += 113;
		questionRect.y -= 18;
		
		SettingsSpace (25);
		
		GameQualitySettings.dynamicObjectsFarClip = SettingsSlider (
			"Dynamic Objects Far Distance", 180, "Drawing distance of dynamic objects.",
			GameQualitySettings.dynamicObjectsFarClip, 0.0, 1.0, "DYNAMIC: ",
			GameQualitySettings._dynamicLayersRange.y - GameQualitySettings._dynamicLayersRange.x, GameQualitySettings._dynamicLayersRange.x);
		
		SettingsSpace (27);
		
		GameQualitySettings.shadowDistance = SettingsSlider (
			"Shadow Distance", 108, "Realtime shadows drawing distance.",
			GameQualitySettings.shadowDistance, 0.0, 30.0, "",
			1.0, 0.0);
			
		SettingsSpace (28);
		
		GameQualitySettings.masterTextureLimit = SettingsIntSlider (
			"Texture Quality", 100, "Overall texture detail.",
			GameQualitySettings.masterTextureLimit, 3, 0,
			["FULL RESOLUTION", "HALF RESOLUTION", "QUARTER RESOLUTION", "1/8 RESOLUTION"]);
		
		SettingsSpace (27);
		
		sceneConf.detailObjectDensity = SettingsSlider (
			"Terrain Grass Density", 136, "Grass density.",
			sceneConf.detailObjectDensity, 0.0, 1.0, "",
			1.0, 0.0);
			
		sceneConf.detailObjectDistance = SettingsSlider (
			"Terrain Grass Distance", 141, "Grass drawing distance.",
			sceneConf.detailObjectDistance, 0.0, 50.0, "",
			1.0, 0.0);
			
		sceneConf.nearTerrainPixelError = SettingsSlider (
			"Terrain Pixel Error", 146, "Set terrain pixel error.",
			sceneConf.nearTerrainPixelError, 50.0, 5.0, "",
			1.0, 0.0);
		
		sceneConf.treeDistance = SettingsSlider (
			"Terrain Tree Distance", 137, "Tree drawing distance.",
			sceneConf.treeDistance, 200.0, 400.0, "",
			1.0, 0.0);
		
		sceneConf.heightmapMaximumLOD = SettingsIntSlider (
			"Terrain Level of Detail", 139, "Overall terrain Level of Detail.",
			sceneConf.heightmapMaximumLOD, 2, 0,
			["FULL RESOLUTION", "QUARTER RESOLUTION", "1/16 RESOLUTION"]);
		
		sceneConf.terrainTreesBillboardStart = SettingsSlider (
			"Terrain Billboard Start", 140, "Distance from the camera where trees will be rendered as billboards.",
			sceneConf.terrainTreesBillboardStart, 10.0, 70.0, "",
			1.0, 0.0);
			
		sceneConf.maxMeshTrees = SettingsIntSlider (
			"Max Mesh Trees", 100, "Set the maximum number of trees rendered at full LOD.",
			sceneConf.maxMeshTrees, 5, 60,
			null);
		
		GUILayout.Space(20);
			
		GUILayout.EndVertical();
	
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		
		if(GUI.tooltip != "")
		{
			GUI.Label(new Rect(mousePos.x + 15, mousePos.y - 60, 150, 70), GUI.tooltip, tooltipStyle);
		}
		
		GUI.skin = cSkin;
	}
	
	function DrawMarker(y : float, steps : int)
	{
		var markerRect : Rect = new Rect(margin, y + 2, 1, 5);
		var aux : float;
		var s : float = steps;
		
		for(var i : int = 0; i <= steps; i++)
		{
			aux = i;
			aux 	/= s;
			markerRect.x = margin + 5 + aux * 196;
			
			GUI.DrawTexture(markerRect, whiteMarker);
		}
	}
	
	// Shows the about menu
	function DrawAbout()
	{
		GUI.Label(new Rect(windowBackgroundRect.x + 20, windowBackgroundRect.y + 15, 200, 20), "ABOUT", titleStyle);	
		
		var abRect : Rect = new Rect(panelLeftRect.x + 7, panelLeftRect.y + 30, panelLeftRect.width - 12, panelLeftRect.height - 40);

		var cSkin : GUISkin = GUI.skin;
		GUI.skin = hudSkin;

		GUILayout.BeginArea(abRect);
		aboutScroll = GUILayout.BeginScrollView(aboutScroll, GUILayout.Width(abRect.width));
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(17);
		GUILayout.BeginVertical();
		GUILayout.Label("Developed by the Wulfram Community", aboutStyle, GUILayout.Width(423));
		GUILayout.Space(5);
		var names : String;
		names = "Not avaliable yet, ";
		names += "Not avaliable yet, ";
		GUILayout.Label(names, GUILayout.Width(400));
		GUILayout.Space(20);
		GUILayout.Label("Special thanks to:", aboutStyle, GUILayout.Width(423));
		GUILayout.Space(5);
		names = "Not avaliable yet, ";
		names += "Not avaliable yetl, ";
		names += "Not avaliable yet, ";
		names += "Not avaliable yet, ";
		names += "Not avaliable yet, ";
		names += "Not avaliable yet.";
		GUILayout.Label(names, GUILayout.Width(400));

		GUILayout.Space(70);
		;
		
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		GUILayout.EndScrollView();
		
		GUILayout.EndArea();

		GUI.skin = cSkin;
	}
	
	// Shows the available connections in the lobby after game start
	// And a opportunity to Create a Game/Room
	function MainConection()
	{
	    GUI.DrawTexture(new Rect((Screen.width / 2) - (windowBackground.width / 2), (Screen.height / 2) - (windowBackground.height / 2), windowBackground.width, windowBackground.height), windowBackground);
    	GUI.Box(new Rect(((Screen.width / 2) - (windowBackground.width / 2))+8, ((Screen.height / 2) - (windowBackground.height / 2))+49, windowBackground.width-50, windowBackground.height-69), "", panelLeft);
		GUI.Label(new Rect(((Screen.width / 2) - (windowBackground.width / 2))+20, ((Screen.height / 2) - (windowBackground.height / 2))+10, windowBackground.width-12, windowBackground.height-80), "Rooms", titleStyle);	
		
		var coRect : Rect =new Rect(((Screen.width / 2) - (windowBackground.width / 2))+9, ((Screen.height / 2) - (windowBackground.height / 2))+50, windowBackground.width-58, windowBackground.height-70);// new Rect(247f + 7f-100, 194.50f + 10f, 475f - 12f, 240f - 20f);

		var cSkin : GUISkin = GUI.skin;
		GUI.skin = hudSkin;
		GUILayout.BeginArea(coRect);
        GUILayout.BeginHorizontal( );
            GUILayout.Space(8);
            GUILayout.Label("Room Name",conectStyle,GUILayout.Width(coRect.width/3));
            GUILayout.Space(80);
            GUILayout.Label("Connected Players " ,conectStyle,GUILayout.Width(coRect.width/3));
        GUILayout.EndHorizontal();
            GUILayout.Space(5);
		connectScroll = GUILayout.BeginScrollView(connectScroll, GUILayout.Width(coRect.width));
		
		for(var key : DictionaryEntry  in GameList)
        {
			var room = key.Value;
			var roomDetails = (room as String).Split(";"[0]);
            var numofPlayers : int = int.Parse(roomDetails[4]);
			var maxPlayers : int = int.Parse(roomDetails[3]);
			
            if(numofPlayers != 0) // only listing room where some action is 
            {	
                GUILayout.BeginHorizontal(GUI.skin.GetStyle("box"));
                GUILayout.Label(key.Key+"",conectStyle,GUILayout.Width(coRect.width/3));
                GUILayout.Space(80);
                GUILayout.Label("Players: " + numofPlayers,conectStyle,GUILayout.Width(coRect.width/3));
                
                if(numofPlayers < maxPlayers)
                {
                    if (GUILayout.Button("Go",conectStyle,GUILayout.Width(coRect.width/10)) )
                    {
                        _usePhoton.SendMessage("JoinRoom", key.Key);
                        ShowMainConect =false;
                    }
                }
                else
                {
                    GUILayout.Label("Full",conectStyle,GUILayout.Width(coRect.width/10)); 
                }
                GUILayout.EndHorizontal();
            }
        }
		//GameList
 
		GUILayout.EndScrollView();
		GUILayout.Space(0);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Create New Room:",hudSkin.GetStyle("Create"),GUILayout.Width(coRect.width/3));
		nameRoom = GUILayout.TextField(nameRoom,textFieldStyle,GUILayout.Width(coRect.width/3));
        if (GUILayout.Button("OK",hudSkin.GetStyle("Create"),GUILayout.Width(coRect.width/8)) && nameRoom.length > 0 || Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return))
        { 
            if(nameRoom == "")
            {
            	nameRoom = "DefaultRoom";
           	}
            	
           	_usePhoton.SendMessage("CreateGame", nameRoom);
            nameRoom = "";
            ShowMainConect = false;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
		GUILayout.EndArea();
		GUI.skin = cSkin;
	}
	
    // Shows the available connections in the lobby when in game
    // And a opportunity to Create a Game/Room
    function DrawConection()
	{
		GUI.Label(new Rect(windowBackgroundRect.x + 20, windowBackgroundRect.y + 15, 200, 20), "Rooms", titleStyle);	
		
		var coRect : Rect = new Rect(panelLeftRect.x + 7, panelLeftRect.y + 10, panelLeftRect.width - 12, panelLeftRect.height - 20);

		var cSkin : GUISkin = GUI.skin;
		GUI.skin = hudSkin;
		GUILayout.BeginArea(coRect);
        GUILayout.BeginHorizontal( );
            GUILayout.Space(8);
            GUILayout.Label("Room Name",conectStyle,GUILayout.Width(coRect.width/3));
            GUILayout.Space(80);
            GUILayout.Label("Connected Players " ,conectStyle,GUILayout.Width(coRect.width/3));
        GUILayout.EndHorizontal();
            GUILayout.Space(5);
		connectScroll = GUILayout.BeginScrollView(connectScroll, GUILayout.Width(coRect.width));
		
		for(var key : DictionaryEntry  in GameList)
        {
			var room = key.Value;
			var roomDetails = (room as String).Split(";"[0]);
            var numofPlayers : int = int.Parse(roomDetails[4]);
			var maxPlayers : int = int.Parse(roomDetails[3]);

            if(numofPlayers != 0)
            {
                GUILayout.BeginHorizontal(GUI.skin.GetStyle("box"));
                    GUILayout.Label(key.Key+"",conectStyle,GUILayout.Width(coRect.width/3));
                    GUILayout.Space(80);
                    GUILayout.Label("Players: " + numofPlayers,conectStyle,GUILayout.Width(coRect.width/3));
                    
                    if(numofPlayers < maxPlayers)
                    {
                        if (GUILayout.Button("Go",conectStyle,GUILayout.Width(coRect.width/10)) )
                        { 
                            _usePhoton.SendMessage("JoinRoom",key.Key); 
                            state = MainMenuState.IDLE;
                        }
                    }
                    else
                    {
                        GUILayout.Label("Full",conectStyle,GUILayout.Width(coRect.width/10)); 
                    }
                GUILayout.EndHorizontal();
            }
        }
		//GameList
		
                
		GUILayout.EndScrollView();
		GUILayout.Space(0);
		GUILayout.BeginHorizontal();
		//GUILayout.Label("Create New Room:",hudSkin.GetStyle("Create"),GUILayout.Width(coRect.width/3));
		//nameRoom = GUILayout.TextField(nameRoom,textFieldStyle,GUILayout.Width(coRect.width/3));
		        /*if (GUI.Button(Rect(330,180,50,50),"OK",hudSkin.GetStyle("Create")) && nameRoom.length > 0 || Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return))
                { 
                    if(nameRoom != "")
                    {
                        _usePhoton.SendMessage("CreateGame", nameRoom); 
                        nameRoom = "";
                        state = MainMenuState.IDLE;
                    }
                }*/
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
		GUILayout.EndArea();
		GUI.skin = cSkin;
	}
	
    // Shows the small side menu when Esc was pressed. 
	function DrawMenu()
	{
		if (state == MainMenuState.CONECTION) {
			return;
		}
		
		menuBackgroundRect.x = 0;
		menuBackgroundRect.y = (screenSize.y - menuBackgroundRect.height) * 0.5 - 50;
		
		playGameRect.x = menuBackgroundRect.x + 93;
		
		if(hideOptions)
		{
			playGameRect.y = menuBackgroundRect.y + 80;
		}
		else
		{
			playGameRect.y = menuBackgroundRect.y + 62;
		}
		
		resumeRect.x = playGameRect.x;
		resumeRect.y = playGameRect.y;
		
		optionsRect.x = playGameRect.x;
		optionsRect.y = playGameRect.y + playGameRect.height + 15;
		
		graphicsRect.x = playGameRect.x;
		
		if(hideOptions)
		{
			graphicsRect.y = playGameRect.y + playGameRect.height + 15;
		}
		else
		{
			graphicsRect.y = optionsRect.y + optionsRect.height + 15;
		}
		
		aboutRect.x = playGameRect.x;
		aboutRect.y = graphicsRect.y + graphicsRect.height + 15;
		
//		conectRect.x = playGameRect.x;
//		conectRect.y = aboutRect.y + graphicsRect.height + 15;
		toLobbyRect.x = playGameRect.x;
		toLobbyRect.y = aboutRect.y + graphicsRect.height + 15;
				
		GUI.DrawTexture(menuBackgroundRect, menuBackground);
		
		var overButton = false;
		
		if(resumeRect.Contains(mousePos))
		{
			overButton = true;
			
			if(!over)
			{
				over = true;
				GetComponent.<AudioSource>().volume = overVolume;
				GetComponent.<AudioSource>().PlayOneShot(overSound);
			}
			
			GUI.DrawTexture(resumeRect, resumeOver);
			
			if(alpha <= 0.0 && GameManager.showMenu)
			{
				if(evt.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
				{
					GetComponent.<AudioSource>().volume = clickVolume;
					GetComponent.<AudioSource>().PlayOneShot(clickSound);
					
					GameManager.showMenu = false;						
					_usePhoton.SendMessage("StatusPause",false);
					
                    Time.timeScale = 1.0;
					visible = false;
					lastMouseTime = Time.time;
				}
			}
		}
		else
		{
			GUI.DrawTexture(resumeRect, resume);
		}
		
		if(!hideOptions)
		{
			if(optionsRect.Contains(mousePos))
			{
				overButton = true;
				
				if(!over)
				{
					over = true;
					GetComponent.<AudioSource>().volume = overVolume;
					GetComponent.<AudioSource>().PlayOneShot(overSound);
				}
				
				GUI.DrawTexture(optionsRect, optionsOver);
				
				if(alpha <= 0.0 && !goingToGame)
				{
					if(evt.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
					{
						GetComponent.<AudioSource>().volume = clickVolume;
						GetComponent.<AudioSource>().PlayOneShot(clickSound);
						
						if(state != MainMenuState.OPTIONS)
						{
							state = MainMenuState.OPTIONS;
						}
						else
						{
							state = MainMenuState.IDLE;
						}
						
						lastMouseTime = Time.time;
					}
				}
			}
			else
			{
				GUI.DrawTexture(optionsRect, options);
			}
		}
		
		if(graphicsRect.Contains(mousePos))
		{
			overButton = true;
			
			if(!over)
			{
				over = true;
				GetComponent.<AudioSource>().volume = overVolume;
				GetComponent.<AudioSource>().PlayOneShot(overSound);
			}
			
			GUI.DrawTexture(graphicsRect, graphicsOver);
			
			if(alpha <= 0.0 && !goingToGame)
			{
				if(evt.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
				{
					GetComponent.<AudioSource>().volume = clickVolume;
					GetComponent.<AudioSource>().PlayOneShot(clickSound);
					
					if(state != MainMenuState.GRAPHICS)
					{
						state = MainMenuState.GRAPHICS;
					}
					else
					{
						state = MainMenuState.IDLE;
					}
					
					lastMouseTime = Time.time;
				}
			}
		}
		else
		{
			GUI.DrawTexture(graphicsRect, graphics);
		}
		
		if(aboutRect.Contains(mousePos))
		{
			overButton = true;
			
			if(!over)
			{
				over = true;
				GetComponent.<AudioSource>().volume = overVolume;
				GetComponent.<AudioSource>().PlayOneShot(overSound);
			}
			
			GUI.DrawTexture(aboutRect, aboutOver);
			
			if(alpha <= 0.0 && !goingToGame)
			{
				if(evt.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
				{
					GetComponent.<AudioSource>().volume = clickVolume;
					GetComponent.<AudioSource>().PlayOneShot(clickSound);
					
					if(state != MainMenuState.ABOUT)
					{
						state = MainMenuState.ABOUT;
					}
					else
					{
						state = MainMenuState.IDLE;
					}
					
					lastMouseTime = Time.time;
				}
			}
		}
		else
		{
			GUI.DrawTexture(aboutRect, about);
		}
		
		if(toLobbyRect.Contains(mousePos))
		{
			overButton = true;
			
			if(!over)
			{
				over = true;
				GetComponent.<AudioSource>().volume = overVolume;
				GetComponent.<AudioSource>().PlayOneShot(overSound);
			}
			
			GUI.DrawTexture(toLobbyRect, toLobbyOver);
						
			if(alpha <= 0.0 && !goingToGame)
			{
				if(evt.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
				{
					GetComponent.<AudioSource>().volume = clickVolume;
					GetComponent.<AudioSource>().PlayOneShot(clickSound);

					state = MainMenuState.CONECTION;

					_usePhoton.SendMessage("ToLobby");
			
					lastMouseTime = Time.time;
				}
			}
		}
		else
		{
			GUI.DrawTexture(toLobbyRect, toLobby);
		}
		
		if(!overButton)
		{
			over = false;
		}
	}
}