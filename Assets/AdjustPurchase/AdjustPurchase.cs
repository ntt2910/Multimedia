using System;
using System.Collections;
using System.Collections.Generic;
using com.adjust.sdk.purchase;
using UnityEngine;

public class AdjustPurchase : MonoBehaviour
	{
		#region AdjustPurchase fields
		private const string errorMessage = "AdjustPurchase: SDK not started. Start it manually using the 'start' method.";

		private static IAdjustPurchase instance = null;
		private static Action<ADJPVerificationInfo> verificationInfoCallback;

		public bool startManually = true;
		public string appToken = "{Your App Token}";

		public ADJPLogLevel logLevel = ADJPLogLevel.Info;
		public ADJPEnvironment environment = ADJPEnvironment.Sandbox;
		#endregion

		#region Unity lifecycle methods
		void Awake()
		{
			if (AdjustPurchase.instance != null)
			{
				return;
			}

			DontDestroyOnLoad(transform.gameObject);
#if DEVELOPMENT
			environment = ADJPEnvironment.Sandbox; logLevel = ADJPLogLevel.Verbose;

#else
            environment = ADJPEnvironment.Production;logLevel = ADJPLogLevel.Assert;
#endif

			if (!this.startManually)
			{
				ADJPConfig config = new ADJPConfig(this.appToken, this.environment);
				config.SetLogLevel(this.logLevel);
				AdjustPurchase.Init(config);
			}
		}
		#endregion

		#region AdjustPurchase methods
		public static void Init(ADJPConfig config)
		{
			if (AdjustPurchase.instance != null)
			{
				Debug.Log("AdjustPurchase: Error, purchase SDK already started.");
				return;
			}
			if (config == null)
			{
				Debug.Log("AdjustPurchase: Missing config to start.");
				return;
			}

#if UNITY_EDITOR
			AdjustPurchase.instance = null;
#elif UNITY_IOS
                AdjustPurchase.instance = new AdjustPurchaseiOS();
#elif UNITY_ANDROID
                AdjustPurchase.instance = new AdjustPurchaseAndroid();
#else
                AdjustPurchase.instance = null;
#endif

			if (AdjustPurchase.instance == null)
			{
				Debug.Log("AdjustPurchase: Purchase SDK can only be used in Android and iOS apps.");
				return;
			}

			AdjustPurchase.instance.Init(config);
		}

		public static void VerifyPurchaseiOS(string receipt, string transactionId, string productId, Action<ADJPVerificationInfo> verificationInfoCallback, string sceneName = "AdjustPurchase")
		{
			if (AdjustPurchase.instance == null)
			{
				Debug.Log(AdjustPurchase.errorMessage);
				return;
			}

			if (receipt == null || transactionId == null || verificationInfoCallback == null)
			{
				Debug.Log("AdjustPurchase: Invalid purchase parameters.");
				return;
			}

			AdjustPurchase.verificationInfoCallback = verificationInfoCallback;
			AdjustPurchase.instance.VerifyPurchaseiOS(receipt, transactionId, productId, sceneName);
		}

		public static void VerifyPurchaseAndroid(string itemSku, string itemToken, string developerPayload, Action<ADJPVerificationInfo> verificationInfoCallback, string sceneName = "AdjustPurchase")
		{
			if (AdjustPurchase.instance == null)
			{
				Debug.Log(AdjustPurchase.errorMessage);
				return;
			}

			if (itemSku == null || itemToken == null || developerPayload == null || verificationInfoCallback == null)
			{
				Debug.Log("AdjustPurchase: Invalid purchase parameters.");
				return;
			}

			AdjustPurchase.verificationInfoCallback = verificationInfoCallback;
			AdjustPurchase.instance.VerifyPurchaseAndroid(itemSku, itemToken, developerPayload, verificationInfoCallback);
		}
		#endregion

		#region Verification info callback
		public void GetNativeVerificationInfo(string stringVerificationInfo)
		{
			if (AdjustPurchase.instance == null)
			{
				Debug.Log(AdjustPurchase.errorMessage);
				return;
			}

			if (AdjustPurchase.verificationInfoCallback == null)
			{
				Debug.Log("AdjustPurchase: Attribution changed delegate was not set.");
				return;
			}

			ADJPVerificationInfo verificationInfo = new ADJPVerificationInfo(stringVerificationInfo);
			AdjustPurchase.verificationInfoCallback(verificationInfo);
		}
		#endregion
	}
