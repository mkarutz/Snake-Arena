
/***********************************************************************************************************
 * Produced by App Advisory	- http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/


using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System;

namespace AppAdvisory.SharingSystem
{
	[InitializeOnLoad]
	public class WelcomeVerySimpleShare : EditorWindow 
	{
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		private const string ONLINE_DOC_URL = "https://dl.dropboxusercontent.com/u/8289407/VerySimpleShare/Documentation/__VSShare%20Documentation%20Setup.pdf";
		private const string RATEUS_URL = "http://u3d.as/u3N";

		private const string NAME_OF_THE_GAME = "VERY SIMPLE SHARE";
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/
		/******* TO MODIFY **********/

		private const string VERYSIMPLEAD_URL = "http://u3d.as/oWD";
		private const string VERYSIMPLELEADERBOARD_URL = "http://u3d.as/qxf";
		private const string VERYSIMPLESHARE_URL = "http://u3d.as/u3N";

		private const string FACEBOOK_URL = "https://facebook.com/appadvisory";
		private const string REQUEST_URL = "https://appadvisory.zendesk.com/hc/en-us/requests/new";
		private const string APPADVISORY_UNITY_CATALOG_URL = "http://u3d.as/9cs";
		private const string COMMUNITY_URL = "https://appadvisory.zendesk.com/hc/en-us/community/topics";

		private const float width = 600;
		private const float height = 760;

		private const string PREFSHOWATSTARTUP = "AppAdvisory" + NAME_OF_THE_GAME + ".PREFSHOWATSTARTUP";

		private static bool showAtStartup;
		private static GUIStyle imgHeader;
		private static bool interfaceInitialized;
		private static Texture adsIcon;
		private static Texture leaderboardIcon;
		private static Texture shareIcon;
		private static Texture onlineDocIcon;
		private static Texture moreGamesIcon;
		private static Texture rateIcon;
		private static Texture communityIcon;
		private static Texture topicIcon;
		private static Texture questionIcon;
		private static Texture facebookIcon;

		[MenuItem("Tools/APP ADVISORY/"+NAME_OF_THE_GAME+"/Welcome Screen", false, 0)]
		[MenuItem("Window/APP ADVISORY/"+NAME_OF_THE_GAME+"/Welcome Screen", false, 0)]
		public static void OpenWelcomeWindow(){
			GetWindow<WelcomeVerySimpleShare>(true);
		}

		static WelcomeVerySimpleShare()
		{
			EditorApplication.playmodeStateChanged -= OnPlayModeChanged;
			EditorApplication.playmodeStateChanged += OnPlayModeChanged;

			showAtStartup = EditorPrefs.GetInt(PREFSHOWATSTARTUP, 1) == 1;

			if (showAtStartup){
				EditorApplication.update -= OpenAtStartup;
				EditorApplication.update += OpenAtStartup;
			}

			DOScriptingSymbol();

		}
//		void OnEnable()
//		{
//			DOScriptingSymbol();
//		}
//
		static void OpenAtStartup(){
			OpenWelcomeWindow();
			EditorApplication.update -= OpenAtStartup;

		}

		static void DOScriptingSymbol()
		{
			Debug.Log("VS SHARE - DOScriptingSymbol");

			SetScriptingSymbol("VS_SHARE", BuildTargetGroup.iOS, true);
			SetScriptingSymbol("VS_SHARE", BuildTargetGroup.Android, true);
		}

		static void OnPlayModeChanged(){
			EditorApplication.update -= OpenAtStartup;
			EditorApplication.playmodeStateChanged -= OnPlayModeChanged;
		}


		static void SetScriptingSymbol(string symbol, BuildTargetGroup target, bool isActivate)
		{
			if(target == BuildTargetGroup.Unknown)
				return;

			var type = target.GetType();
			var memInfo = type.GetMember(target.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(BuildTargetGroup), false);
			bool continueBTG = attributes.Length > 0;

			if(!continueBTG)
				return;
			
				var s = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

				string sTemp = symbol;

				if(!s.Contains(sTemp))
				{

					s = s.Replace(symbol + ";","");

					s = s.Replace(symbol,"");

					if(isActivate)
						s = symbol + ";" + s;

					PlayerSettings.SetScriptingDefineSymbolsForGroup(target,s);
				}
//			}
		}


		void OnEnable()
		{


			//			SetScriptingSymbol("VS_SHARE", BuildTargetGroup.Android, true);
			//			SetScriptingSymbol("VS_SHARE", BuildTargetGroup.iOS, true);

			#if UNITY_5_3_OR_NEWER
			titleContent = new GUIContent("Welcome To " + NAME_OF_THE_GAME); 
			#endif

			maxSize = new Vector2(width, height);
			minSize = maxSize;
		}	

		public void OnGUI()
		{
			InitInterface();
			GUI.Box(new Rect(0, 0, width, 60), "", imgHeader);
			GUILayoutUtility.GetRect(position.width, 50);
			GUILayout.Space(30);
			GUILayout.BeginVertical();

			if (Button(adsIcon,"WANT TO MONETIZE THIS ASSET?","Get 'Very Simple Ads' on the Asset Store and earn money in a minute!")){
				Application.OpenURL(VERYSIMPLEAD_URL);
			}

			if (Button(leaderboardIcon,"WANT TO ADD A LEADERBOARD?","Get 'Very Simple Leaderboard' on the Asset Store!")){
				Application.OpenURL(VERYSIMPLELEADERBOARD_URL);
			}

			if (Button(communityIcon,"Join the community and get access to direct download","Be informed of the latest updates.")){
				Application.OpenURL(COMMUNITY_URL);
			}

			if (Button(rateIcon,"Rate this asset","Write us a review on the asset store.")){
				Application.OpenURL(RATEUS_URL);
			}

			if (Button(moreGamesIcon,"More Unity assets from us","Have a look to our Unity's Asset Store catalog!")){
				Application.OpenURL(APPADVISORY_UNITY_CATALOG_URL);
			}

			if (Button(questionIcon,"A request? LOOKING FOR A DEVELOPER? Need a reskin for this game?","Don't hesitate to contact us.")){
				Application.OpenURL(REQUEST_URL);
			}

			if (Button(facebookIcon,"Facebook page","Follow us on Facebook.")){
				Application.OpenURL(FACEBOOK_URL);
			}

			if (Button(onlineDocIcon,"Online documentation","Read the full documentation.")){
				Application.OpenURL(ONLINE_DOC_URL);
			}

			GUILayout.Space(3);

			bool show = GUILayout.Toggle(showAtStartup, "Show at startup");
			if (show != showAtStartup){
				showAtStartup = show;
				int i = GetInt(showAtStartup);
				Debug.Log("toggle i = " + i);
				EditorPrefs.SetInt(PREFSHOWATSTARTUP, i);
			}

			GUILayout.EndVertical();
		}

		int GetInt(bool b)
		{
			if(b)
				return 1;
			else
				return 0;
		}

		void InitInterface(){

			if (!interfaceInitialized){

				imgHeader = new GUIStyle();
				imgHeader.normal.background = (Texture2D)Resources.Load("vss_appadvisoryBanner");
				imgHeader.normal.textColor = Color.white;

				adsIcon = (Texture)Resources.Load("vss_btn_monetization") as Texture;
				leaderboardIcon = (Texture)Resources.Load("vss_btn_leaderboard") as Texture;
				shareIcon = (Texture)Resources.Load("vss_btn_share") as Texture;
				onlineDocIcon = (Texture)Resources.Load("vss_btn_onlinedoc") as Texture;
				communityIcon = (Texture)Resources.Load("vss_btn_community") as Texture;
				moreGamesIcon = (Texture)Resources.Load("vss_btn_moregames") as Texture;
				rateIcon = (Texture)Resources.Load("vss_btn_rate") as Texture;
				questionIcon = (Texture)Resources.Load("vss_btn_question") as Texture;
				facebookIcon = (Texture)Resources.Load("vss_btn_facebook") as Texture;

				interfaceInitialized = true;
			}
		}

		bool Button(Texture texture, string heading, string body, int space=10){

			GUILayout.BeginHorizontal();

			GUILayout.Space(54);
			GUILayout.Box(texture, GUIStyle.none, GUILayout.MaxWidth(48));
			GUILayout.Space(10);

			GUILayout.BeginVertical();
			GUILayout.Space(1);
			GUILayout.Label(heading, EditorStyles.boldLabel);
			GUILayout.Label(body);
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();

			var rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

			bool returnValue = false;
			if (Event.current.type == EventType.mouseDown && rect.Contains(Event.current.mousePosition)){
				returnValue = true;
			}

			GUILayout.Space(space);

			return returnValue;
		}
	}
}