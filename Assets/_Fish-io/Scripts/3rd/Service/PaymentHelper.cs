using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

[RequireComponent(typeof(GamePayment))]
    public class PaymentHelper : MonoBehaviour
    {
        #region Members

        private static PaymentHelper mInstance;
        public static PaymentHelper Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = FindObjectOfType<PaymentHelper>();
                return mInstance;
            }
        }

        #endregion

        //=============================================

        #region MonoBehaviour

        private void Awake()
        {
            if (mInstance == null)
                mInstance = this;
            else if (mInstance != this)
                Destroy(gameObject);
        }

        #endregion

        //=============================================

        #region Public

        public void InitProducts(List<string> consumableSkus, List<string> subscriptionSkus, Action<bool> pOnFinished)
        {
            // if (GameUnityData.instance.GameRemoteConfig.freeMode_showStore)
            GamePayment.Instance.Init(consumableSkus, subscriptionSkus, true, pOnFinished);
        }

        private void PurchaseSku(string sku, Action<bool> pAction)
        {
#if UNITY_WEBGL
            return;
#endif

#if DEVELOPMENT
            if (pAction != null)
                pAction(true); return;
#endif

            GamePayment.Instance.Purchase(sku, pAction);
        }

        private void Restore(Action<bool> pOnRestored)
        {
#if UNITY_WEBGL
            return;
#endif
            GamePayment.Instance.Restore(pOnRestored);
        }

        public bool IsSubscribed(string pPackageId)
        {
#if UNITY_WEBGL
            return false;
#endif
#if UNITY_IAP
            var subscriptionInfo = GamePayment.Instance.GetSubscriptionInfo(pPackageId);
            if (subscriptionInfo == null)
                return false;

            return (subscriptionInfo.isCancelled() == Result.False &&
                    subscriptionInfo.isExpired() == Result.False &&
                    (DateTime.UtcNow - subscriptionInfo.getExpireDate()).TotalSeconds < 0);
#else
            return false;
#endif
        }

        public DateTime? SubscriptionExpireDate(string pPackageId)
        {
#if UNITY_IAP
            var subscriptionInfo = GamePayment.Instance.GetSubscriptionInfo(pPackageId);
            if (subscriptionInfo == null)
                return null;

            return subscriptionInfo.getExpireDate();
#else
            return null;
#endif
        }

        public static bool IsProductOwned(string Sku)
        {
#if UNITY_WEBGL
            return false;
#endif
            return GamePayment.Instance.IsProductOwned(Sku);
        }

        private decimal GetLocalizedPrice(string pPackageId)
        {
            return GamePayment.Instance.GetLocalizedPrice(pPackageId);
        }

        public string GetLocalizedPriceString(string pPackageId)
        {
            return GamePayment.Instance.GetLocalizedPriceString(pPackageId);
        }

        private string GetIsoCurrencyCode(string pPackageId)
        {
            return GamePayment.Instance.GetIsoCurrencyCode(pPackageId);
        }


        //fix CRASH
        public static void SetTextLocalizedPriceString(TextMeshProUGUI txtCost, string sku)
        {
            if (txtCost != null)
            {
                txtCost.text = "1.99$";
            }

            try
            {
                if (Instance != null
                    && txtCost != null
                    && sku != null
                    && !sku.Equals(""))
                {
                    var s = Instance.GetLocalizedPriceString(sku);
                    if (s != null) txtCost.text = s;
                }
            }
            catch (System.Exception)
            {

            }
        }

        //public static void SetTextLocalizedPriceString(TextMeshProUGUI txtCost, PackageData packageData)
        //{
        //    if (txtCost != null)
        //    {
        //        txtCost.text = packageData.Usd;
        //    }

        //    try
        //    {
        //        if (Instance != null
        //            && txtCost != null
        //            && packageData.Sku != null
        //            && !packageData.Sku.Equals(""))
        //        {
        //            var s = Instance.GetLocalizedPriceString(packageData.Sku);
        //            if (s != null) txtCost.text = s;
        //        }
        //    }
        //    catch (System.Exception)
        //    {

        //    }
        //}

        //public static void SetTextLocalizedPriceString(TextMeshProUGUI txtCost, PremiumPackageData premiumPackageData)
        //{
        //    if (txtCost != null)
        //    {
        //        txtCost.text = premiumPackageData.Usd;
        //    }

        //    try
        //    {
        //        if (Instance != null
        //            && txtCost != null
        //            && premiumPackageData.Sku != null
        //            && !premiumPackageData.Sku.Equals(""))
        //        {
        //            var s = Instance.GetLocalizedPriceString(premiumPackageData.Sku);
        //            if (s != null) txtCost.text = s;
        //        }
        //    }
        //    catch (System.Exception)
        //    {

        //    }
        //}

        public static void Purchase(string sku, Action<bool> pAction)
        {
            Debug.Log(sku);
            try
            {
                if (Instance != null
                    && sku != null
                    && !sku.Equals(""))
                {
                    Instance.PurchaseSku(sku, pAction);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                if (pAction != null)
                    pAction(false);
            }
        }



        //public static void LogPurchase(string sku)
        //{
        //    try
        //    {
        //        if (Instance != null
        //            && sku != null
        //            && !sku.Equals(""))
        //        {
        //            var price = (float)(Instance.GetLocalizedPrice(sku));
        //            var currency = Instance.GetIsoCurrencyCode(sku);
        //            Config.LogPurchaseEvent(price, currency);
        //        }
        //    }
        //    catch (System.Exception)
        //    {

        //    }
        //}

        //public static void LogPurchase(PackageData packageData)
        //{
        //    try
        //    {
        //        if (Instance != null
        //            && packageData.Sku != null
        //            && !packageData.Sku.Equals(""))
        //        {
        //            var price = (float)(Instance.GetLocalizedPrice(packageData.Sku));
        //            var currency = Instance.GetIsoCurrencyCode(packageData.Sku);
        //            Config.LogPurchaseEvent(price, currency);
        //        }
        //    }
        //    catch (System.Exception)
        //    {

        //    }
        //}

        //public static void LogPurchase(PremiumPackageData premiumPackageData)
        //{
        //    try
        //    {
        //        if (Instance != null
        //            && premiumPackageData.Sku != null
        //            && !premiumPackageData.Sku.Equals(""))
        //        {
        //            var price = (float)(Instance.GetLocalizedPrice(premiumPackageData.Sku));
        //            var currency = Instance.GetIsoCurrencyCode(premiumPackageData.Sku);
        //            Config.LogPurchaseEvent(price, currency);
        //        }
        //    }
        //    catch (System.Exception)
        //    {

        //    }
        //}
        public static void RestorePurchase(Action<bool> pOnRestored)
        {
            try
            {
                if (Instance != null)
                {
                    Instance.Restore(pOnRestored);
                }
            }
            catch (System.Exception)
            {

            }
        }

        //public static bool IsSubscribed(string Sku)
        //{
        //    try
        //    {
        //        if (Instance != null
        //            && Sku != null
        //            && !Sku.Equals(""))
        //        {
        //            return Instance.IsSubscribed(Sku);
        //        }
        //    }
        //    catch (System.Exception)
        //    {

        //    }

        //    return false;
        //}

        #endregion
    }
