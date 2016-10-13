using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if VS_UI
using AppAdvisory.VSUI;
#endif

#if APPADVISORY_ADS
using AppAdvisory.Ads;
#endif

namespace AppAdvisory.SharingSystem
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class VSSHARE: MonoBehaviour  
	{

		public bool withText = true;

		#if APPADVISORY_ADS


		#if UNITY_ANDROID

		#if !VS_UI
		bool isAmazon
		{
			get
			{
				return AdsManager.instance.adIds.isAmazon;
			}
		}
		#else
		bool isAmazon
		{
			get
			{
				return FindObjectOfType<UIController>().isAmazon;
			}
		}
		#endif

		#else
		bool isAmazon = false;
		#endif



		#else
		#if !VS_UI
		public bool isAmazon = false;
		#else
		bool isAmazon
		{
			get
			{
				return FindObjectOfType<UIController>().isAmazon;
			}
		}
		#endif
		#endif

		
		#region options
		public bool showButtonShareWhenSceneRestartIfScreenshotAvailable = true;
		public GameObject[] ToHideOnTheScreenshot;
		public GameObject[] ToDisplayOnTheScreenshot;
		private string screenshotPath;

		#if !VS_UI
		public string shareTextBeforeUrl = "Get it here for free: "; 
		public string shareTextAfterUrl = " #appadvisory"; 
		#else
		public string shareTextBeforeUrl
		{
			get
			{
				return FindObjectOfType<UIController>().shareTextBeforeUrl;
			}
		}
		public string shareTextAfterUrl
		{
			get
			{
				return FindObjectOfType<UIController>().shareTextAfterUrl;
			}
		}
		#endif

		#if !VS_UI
		/// <summary>
		/// URL of the iOS game. Find it on iTunes Connect.
		/// </summary>
		public string iD_iOS = "1134939249";
		public string url_ios
		{
			get
			{
				return "https://itunes.apple.com/us/app/" + iD_iOS; //1134939249
			}
		}
		/// <summary>
		/// URL of the Android game. Find it on Google Play.
		/// </summary>
		public string bundleIdAndroid = "com.appadvisory.ab2";
		public string url_android
		{
			get
			{
				return "https://play.google.com/store/apps/details?id=" + bundleIdAndroid; //1134939249
			}
		}
		/// <summary>
		/// URL of the Amazon game. Find it on the Amazon Developer Console.
		/// </summary>
		public string amazonID = "B01DPBSF2A";
		public string url_amazon
		{
			get
			{
				return "https://www.amazon.fr/dp/" + amazonID; //1134939249
			}
		}

		public string URL_STORE
		{
			get
			{
				string URL = "";

		#if UNITY_IOS
		URL = url_ios;
		#else
				URL = url_android;
				if(isAmazon)
					URL = url_amazon;
		#endif

				return URL;
			}
		}

		#endif

		string ShareText
		{
			get
			{
				#if !VS_UI
				return shareTextBeforeUrl + URL_STORE + shareTextAfterUrl;
				#else
				return shareTextBeforeUrl + FindObjectOfType<UIController>().URL_STORE + shareTextAfterUrl;
				#endif
			}
		}

		#endregion

		#region variables
		float width
		{
			get
			{
				return Screen.width;
			}
		}

		float height
		{
			get
			{
				return Screen.height;
			}
		}


		bool isPortrait
		{
			get
			{
				return Screen.width < Screen.height;
			}
		}
		bool isAnimating
		{ 
			get
			{
				return m_isAnimating;
			}
			set
			{
				m_isAnimating = value;

				buttonOpenOrShareScreenshot.enabled = !value;
			}
		}
		private bool m_isAnimating = false;
		private bool takeFullScreen = true;
		[SerializeField] private ButtonShareState m_state;
		ButtonShareState state
		{
			get
			{
				return m_state;
			}

			set
			{
				if(value == ButtonShareState.isClosed)
				{
					if(OnButtonShareIsClosed != null)
						OnButtonShareIsClosed();
				}

				if(value == ButtonShareState.isIcon)
				{
					if(OnButtonShareisIcon != null)
						OnButtonShareisIcon();
				}

				if(value == ButtonShareState.isShareWindows)
				{
					if(OnButtonShareIsShareWindow != null)
						OnButtonShareIsShareWindow();
				}

				m_state = value;
			}
		}
		static public bool haveScreenshotAvailable
		{
			get
			{
				bool screenAvailable = false;
				if(self.spriteGO != null)
					screenAvailable = true;

				return screenAvailable;
			}
		}

		GameObject _spriteGO;
		GameObject spriteGO
		{
			get
			{
				if(_spriteGO == null)
					_spriteGO = GameObject.Find("__screenshot__");

				return _spriteGO;
			}
		}

		Texture2D screenshot
		{
			get
			{
				return spriteGO.GetComponent<TextureContainer>().texture;
			}
		}

		#endregion

		#region references
		static private VSSHARE self;
		public Button buttonOpenOrShareScreenshot;
		public Button buttonClose;
		public AudioClip photoSound;
		AudioSource audioSource;
		public ScreenshotElement shareElement;
		public Image flash;
		Vector2 VSSHARE_START_POSITION;
		#endregion

		#region delegate
		[System.Serializable] public delegate void OnButtonShareIsClosedHandler();
		[SerializeField] public static event OnButtonShareIsClosedHandler OnButtonShareIsClosed;

		[System.Serializable] public delegate void OnButtonShareisIconHandler();
		[SerializeField] public static event OnButtonShareisIconHandler OnButtonShareisIcon;

		[System.Serializable] public delegate void OnButtonShareIsShareWindowHandler();
		[SerializeField] public static event OnButtonShareIsShareWindowHandler OnButtonShareIsShareWindow;

		[System.Serializable] public delegate void OnScreenshotTakenHandler(Texture2D tex);
		[SerializeField] public static event OnScreenshotTakenHandler OnScreenshotTaken;
		#endregion

		#region class methods
		void SetVisibilityToHideOnScreenshot(bool _isVisible)
		{
			foreach(var go in ToHideOnTheScreenshot)
			{
				if(go != null)
					go.SetActive(_isVisible);
			}
		}

		void SetVisibilityToShowOnScreenshot(bool _isVisible)
		{
			foreach(var go in ToDisplayOnTheScreenshot)
			{
				if(go != null)
					go.SetActive(_isVisible);
			}
		}
		public bool DoAnimOpenScreenshotAsAButton()
		{
			if(!haveScreenshotAvailable)
			{
				shareElement.gameObject.SetActive(false);
				state = ButtonShareState.isClosed;
				return false;
			}

			shareElement.gameObject.SetActive(true);

			SetImage(shareElement.image, screenshot);

			shareElement.rect.gameObject.SetActive(true);

			shareElement.rect.localScale = Vector2.zero;

			self.StartCoroutine(LerpScale(shareElement.rect, 0, 1, 0.3f,
				() => {
					isAnimating = true;
				}, 
				() => {
					isAnimating = false;
					state = ButtonShareState.isIcon;
				}));

			return true;
		}
		void ResetFlash()
		{
			Color c = flash.color;
			c.a = 1;
			flash.color = c;
			flash.gameObject.SetActive(false);
		}

		void DoFlash(Action callback)
		{
			ResetFlash();

			flash.gameObject.SetActive(true);

			audioSource.PlayOneShot(photoSound);

			self.StartCoroutine(self.DOFade(flash, 1, 0, 0.3f, 0.1f,
				() => {
					isAnimating = true;
				}, 
				() => {
					isAnimating = true;

					if(callback != null)
						callback();
				}));
		}
		void ActivateButtonClose(bool _activate)
		{
			if(_activate)
			{
				if(buttonClose != null)
				{
					self.buttonClose.gameObject.SetActive(_activate);
					self.buttonClose.enabled = _activate;
					self.buttonClose.interactable = _activate;
					self.StartCoroutine(self.DOFade(self.buttonClose.image, 0f, 0.7f, 0.2f, 0.0f, null, null));
				}
			}
			else
			{
				if(buttonClose != null)
				{
					buttonClose.gameObject.SetActive(true);
					buttonClose.enabled = true;
					buttonClose.interactable = true;

					self.StartCoroutine(self.DOFade(buttonClose.image, 0.7f, 0.0f, 0.2f, 0.0f, null, () => {
						buttonClose.gameObject.SetActive(false);
						buttonClose.enabled = false;
						buttonClose.interactable = false;
					}));
				}
			}
		}
		public void OnClickedHideScreenshotIcon()
		{
			StartCoroutine(LerpScale(shareElement.rect, 1, 0, 0.3f, 
				() => {
					isAnimating = true;
				},
				() => {
					isAnimating = false;
					state = ButtonShareState.isClosed;
					shareElement.gameObject.SetActive(false);
				}));
		}

		public void OnClickedOnIconScreenshot()
		{
			if(isAnimating)
			{
				return;
			}
			else
			{
				if(state == ButtonShareState.isShareWindows)
				{
					ShareScreenshot(ShareText);
				}
				else if(state == ButtonShareState.isIcon)
				{
					ActivateButtonClose(true);

					AnimIconToWindow();
				}
			}
		}

		void AnimIconToWindow()
		{

			Vector2 sizeTo = Vector2.zero;

			if(takeFullScreen)
			{
				float decal = 0.30f * Mathf.Min(width, height);

//				old method...
//				sizeTo = new Vector2(width - decal, height - decal) / GetComponent<RectTransform>().lossyScale.x;

				sizeTo = new Vector2(width - decal, height - decal) * 2;

//				if(GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceCamera)
//					sizeTo = new Vector2(width - decal, height - decal);
			}
			else
			{
				if(isPortrait)
					sizeTo = new Vector2(width * 0.6f, width * 0.8f);
				else
					sizeTo = new Vector2(height * 0.6f, height * 0.8f);
			}
			//
			//			self.StartCoroutine(DORotate(shareElement.parentRectTransform, shareElement.parentRectTransform.rotation, Quaternion.identity, 0.3f));
			//			self.StartCoroutine(DOLocalMove(shareElement.parentRectTransform, shareElement.parentRectTransform.localPosition, Vector2.zero, 0.3f));

			self.StartCoroutine(DORotate(shareElement.rect, shareElement.rect.rotation, Quaternion.identity, 0.3f));

//			shareElement.defaultPosition = shareElement.rect.localPosition;



//			self.StartCoroutine(DOMove(shareElement.rect, shareElement.defaultPosition, new Vector2(Screen.width / 2f, Screen.height / 2f), 0.3f));
			VSSHARE_START_POSITION = GetComponent<RectTransform>().localPosition;
			self.StartCoroutine(DOMove(GetComponent<RectTransform>(), VSSHARE_START_POSITION, Vector2.zero, 0.3f));

//			GetComponent<RectTransform>().localPosition = Vector2.zero;


			self.StartCoroutine(DOSizeDelta(shareElement.rect, shareElement.rect.sizeDelta, sizeTo * 0.5f, 0.3f,
				() => {
					isAnimating = true;
				},
				() => {
					isAnimating = false;
					state = ButtonShareState.isShareWindows;
				}));
		}


		public void ShareScreenshot(string text)
		{
			if(withText)
				OpenShareImageMobileNativeDialog(text);
			else
				OpenShareImageOnlyMobileNativeDialog();
				
			return;
			#if UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID
//			OnClickedButtonCloseShareWindow();
			self.buttonClose.gameObject.SetActive(false);
			self.buttonClose.enabled = false;
			self.buttonClose.interactable = false;
			var c = self.buttonClose.image.color;
			c.a = 0;
			self.buttonClose.image.color = c;
			shareElement.rotation = shareElement.defaultRotation;
			shareElement.localPosition = shareElement.defaultPosition;
			shareElement.sizeDelta = shareElement.defaultSizeDelta;
			isAnimating = false;
			shareElement.localScale = Vector2.zero;
			shareElement.gameObject.SetActive(false);
			state = ButtonShareState.isClosed;
			#endif
		}

		public void OnClickedButtonCloseShareWindow()
		{
			if(isAnimating)
			{
				return;
			}

			ActivateButtonClose(false);

			self.StartCoroutine(self.DORotate(shareElement.rect, shareElement.rect.rotation, shareElement.defaultRotation, 0.3f));

//			self.StartCoroutine(self.DOMove(shareElement.rect, new Vector2(Screen.width / 2f, Screen.height / 2f), shareElement.defaultPosition, 0.3f));
			self.StartCoroutine(self.DOMove(GetComponent<RectTransform>(), Vector2.zero, VSSHARE_START_POSITION, 0.3f));


			self.StartCoroutine(self.DOSizeDelta(shareElement.rect, shareElement.rect.sizeDelta, shareElement.defaultSizeDelta, 0.3f,
				() => {
					isAnimating = true;
				},
				() => {
					isAnimating = false;
					state = ButtonShareState.isIcon;
				}));
		}
		#endregion

		#region static methods
		static public void DOOnclickedOnIconScreenshot()
		{
			self.OnClickedOnIconScreenshot();
		}
		static public ButtonShareState GetButtonShareState()
		{
			return self.m_state;
		}
		static public bool DOOpenScreenshotButton()
		{
			return self.DoAnimOpenScreenshotAsAButton();
		}
		static public void DOHideScreenshotIcon()
		{
			self.OnClickedHideScreenshotIcon();
		}
		static public void DOShareScreenshot(string text)
		{
			self.ShareScreenshot(text);
		}
		static public void DOCloseShareWindow()
		{
			self.OnClickedButtonCloseShareWindow();
		}
		static public void DODesactivate()
		{
			self.shareElement.gameObject.SetActive(false);
		}
		static public void DOTakeScreenShot()
		{
			self.TakeScreenshot();
		}
#if UNITY_EDITOR
		static VSSHARE()
		{
			PlayerSettings.Android.forceSDCardPermission = true; 
		}
#endif 
#if UNITY_IPHONE
		[DllImport ("__Internal")]	
		private static extern void presentActivitySheetWithImageAndString(string message,byte[] imgData,int _length);

		[DllImport ("__Internal")]	
		private static extern void presentActivitySheetWithImage(byte[] imgData,int _length);
#endif
		#endregion

		#region coroutines
		IEnumerator LerpScale(RectTransform rect, float _from, float _to, float time, Action OnUpdate, Action OnCompleted)
		{
			float elapsedTime = 0f;

			while (elapsedTime < time)
			{

				if(OnUpdate != null)
					OnUpdate();

				float f = Mathf.Lerp(_from, _to, (elapsedTime / time));
				Vector2 scale = new Vector2(f,f);
				rect.localScale = scale;
				elapsedTime += Time.deltaTime;
				yield return 0;
			}

			rect.localScale = new Vector2(_to, _to);

			if(OnCompleted != null)
				OnCompleted();
		}
		IEnumerator DORotate(RectTransform rect, Quaternion _quaternionStart, Quaternion _quaternionEnd, float time)
		{
			rect.rotation = _quaternionStart;

			float elapsedTime = 0f;

			while (elapsedTime < time)
			{
				Quaternion quart = Quaternion.Lerp(_quaternionStart, _quaternionEnd, (elapsedTime / time));
				rect.rotation = quart;
				elapsedTime += Time.deltaTime;
				elapsedTime = Mathf.Clamp01(elapsedTime);
				yield return 0;
			}

			rect.rotation = _quaternionEnd;
		}

		IEnumerator DOMove(RectTransform rect, Vector2 _start, Vector2 _end, float time)
		{
			rect.localPosition = _start;

			float elapsedTime = 0f;

			while (elapsedTime < time)
			{
				var f = Vector2.Lerp(_start, _end, (elapsedTime / time));
				rect.localPosition = f;
				elapsedTime += Time.deltaTime;
				elapsedTime = Mathf.Clamp01(elapsedTime);
				yield return 0;
			}

			rect.localPosition = _end;
		}

		IEnumerator DOSizeDelta(RectTransform rect, Vector2 _start, Vector2 _end, float time, Action onUpdate, Action callback)
		{
			rect.sizeDelta = _start;

			float elapsedTime = 0f;

			while (elapsedTime < time)
			{
				var f = Vector2.Lerp(_start, _end, (elapsedTime / time));
				rect.sizeDelta = f;
				elapsedTime += Time.deltaTime;
				elapsedTime = Mathf.Clamp01(elapsedTime);

				if(onUpdate != null)
					onUpdate();

				yield return 0;
			}

			yield return 0;

			if(callback != null)
				callback();
		}
		IEnumerator DOFade(Image im, float _from, float _to, float time, float delay, Action OnUpdate, Action OnCompleted)
		{
			float elapsedTime = 0f;

			while (elapsedTime < time)
			{

				Color c = im.color;

				float f = Mathf.Lerp(_from, _to, (elapsedTime / time));

				c.a = f;

				im.color = c;

				elapsedTime += Time.deltaTime;
				elapsedTime = Mathf.Clamp01(elapsedTime);

				if(OnUpdate != null)
					OnUpdate();

				yield return 0;
			}

			Color cc = im.color;

			cc.a = _to;

			im.color = cc;

			yield return 0;

			if(_to == 0)
				im.gameObject.SetActive(false);

			yield return 0;

			if(OnCompleted != null)
				OnCompleted();
		}
		IEnumerator getScreenshot()
		{
			SetVisibilityToShowOnScreenshot(true);
			SetVisibilityToHideOnScreenshot(false);

			yield return new WaitForEndOfFrame();

			Texture2D createdTexture = null;

			if(takeFullScreen)
			{
				createdTexture = new Texture2D((int)width, (int)height, TextureFormat.ARGB32, false);
				createdTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0,false);
			}
			else
			{
				if(isPortrait)
				{
					createdTexture = new Texture2D((int)width, (int)width, TextureFormat.ARGB32, false);
					createdTexture.ReadPixels(new Rect(0, (Screen.height - width) / 2f, width, width), 0, 0,false);
				}
				else
				{
					createdTexture = new Texture2D((int)height, (int)height, TextureFormat.ARGB32, false);
					createdTexture.ReadPixels(new Rect(0, (Screen.width - height) / 2f, height, height), 0, 0,false);
				}
			}

			createdTexture.Apply();

			SetVisibilityToShowOnScreenshot(false);
			SetVisibilityToHideOnScreenshot(true);

			DoFlash(() => {
				GameObject spriteGO = GameObject.Find("__screenshot__");

				TextureContainer tc = null;

				if(spriteGO == null)
				{
					spriteGO = new GameObject();

					tc = spriteGO.AddComponent<TextureContainer>();

					spriteGO.name = "__screenshot__";
				}
				else
				{
					tc = spriteGO.GetComponent<TextureContainer>();
				}

				DontDestroyOnLoad(spriteGO);

				tc.texture = createdTexture;

				if(OnScreenshotTaken != null)
					OnScreenshotTaken(createdTexture);

			});

			yield return 0;

		}
		#endregion

		#region initialization

		void Awake()
		{

//	#if APPADVISORY_ADS && UNITY_ANDROID
//	this.isAmazon = AdsManager.instance.adIds.isAmazon;
//	#endif
			audioSource = GetComponent<AudioSource>();

			ResetFlash();

//			shareElement.defaultRotation = shareElement.parentRectTransform.rotation;
//			shareElement.defaultPosition = shareElement.parentRectTransform.position;
//			shareElement.defaultSizeDelta = shareElement.parentRectTransform.sizeDelta;

			shareElement.gameObject.SetActive(false);

			shareElement.GetComponent<RectTransform>().localScale = Vector2.zero;

			if(buttonClose != null)
				buttonClose.gameObject.SetActive(false);

			state = ButtonShareState.isClosed;

			SetVisibilityToShowOnScreenshot(false);
		}


		void Start()
		{
			self = this;

			VSSHARE_START_POSITION = GetComponent<RectTransform>().position;

#if UNITY_ANDROID
AndroidJNIHelper.debug = true;
#endif

			if(haveScreenshotAvailable)
			{
				shareElement.gameObject.SetActive(true);

				SetImage(shareElement.image, screenshot);

				if(showButtonShareWhenSceneRestartIfScreenshotAvailable)
				{
					DoAnimOpenScreenshotAsAButton();
				}
			}
		}

		void SetImage(Image im, Texture2D tex)
		{
			im.sprite = null;

			if(takeFullScreen)
			{
				im.sprite =	Sprite.Create(tex, new Rect(0,0, width, height), new Vector2(0.5f,0.5f), 100f);
			}
			else
			{
				if(isPortrait)
					im.sprite =	Sprite.Create(tex, new Rect(0,0, width, width), new Vector2(0.5f,0.5f), 100f);
				else
					im.sprite =	Sprite.Create(tex, new Rect(0,0, height, height), new Vector2(0.5f,0.5f), 100f);
			}

		}

		public void TakeScreenshot()
		{
			self.StartCoroutine(self.getScreenshot());
		}

		public void ShareScreenshot()
		{
			if(withText)
				self.OpenShareImageMobileNativeDialog ( ShareText );
			else
				self.OpenShareImageOnlyMobileNativeDialog ();
		}

		void OpenShareImageOnlyMobileNativeDialog() 
		{
			#if UNITY_IPHONE
			byte[] imgData = screenshot.EncodeToPNG();
			string screenShotPath = Application.persistentDataPath + "/" + "ScreenshotVSSHARE.png";
			System.IO.File.WriteAllBytes(screenShotPath, screenshot.EncodeToPNG());
			presentActivitySheetWithImage(imgData,imgData.Length);
			#endif

			#if UNITY_ANDROID
			OpenShareImageMobileNativeDialog("");
			#endif
		}
			
		void OpenShareImageMobileNativeDialog(string shareText) 
		{
			if(Application.isEditor)
			{
				//#if UNITY_EDITOR
				//				if(UnityEditor.EditorUtility.DisplayDialog("********** Attention! **********","Very Simple Share doesn't work in the editor.","OK"))
				//				{
				//					Invoke("OnClickedButtonCloseShareWindow",1f);
				//				}
				//#endif
				Debug.LogWarning("********** Very Simple Share doesn't work in the editor. **********");
				return;
			}

#if UNITY_IPHONE
			byte[] imgData = screenshot.EncodeToPNG();
			string screenShotPath = Application.persistentDataPath + "/" + "ScreenshotVSSHARE.png";
			System.IO.File.WriteAllBytes(screenShotPath, screenshot.EncodeToPNG());
			presentActivitySheetWithImageAndString(shareText,imgData,imgData.Length);
#endif

#if UNITY_ANDROID

string screenShotPath = Application.persistentDataPath + "/" + "ScreenshotVSSHARE.png";
System.IO.File.WriteAllBytes(screenShotPath, screenshot.EncodeToPNG());

AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + screenShotPath);
intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
intentObject.Call<AndroidJavaObject>("setType", "image/png");

intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);

AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share");
currentActivity.Call("startActivity", jChooser);

#endif
		}
		#endregion
	}
}
 