using System.Reflection;

namespace RealConstruction.Util
{
    public class RealCityUtil
    {
        public delegate bool RealCityGetRealCityV10();
        public static RealCityGetRealCityV10 GetRealCityV10;

        public delegate int RealCityGetReduceCargoDiv();
        public static RealCityGetReduceCargoDiv GetReduceCargoDiv;

        public delegate float RealCityGetResourcePrice(TransferManager.TransferReason material);
        public static RealCityGetResourcePrice GetResourcePrice;

        public delegate float RealCityGetOutsideTouristMoney();
        public static RealCityGetOutsideTouristMoney GetOutsideTouristMoney;

        public delegate float RealCityGetOutsideGovermentMoney();
        public static RealCityGetOutsideGovermentMoney GetOutsideGovermentMoney;

        public delegate void RealCitySetOutsideTouristMoney(float value);
        public static RealCitySetOutsideTouristMoney SetOutsideTouristMoney;

        public delegate void RealCitySetOutsideGovermentMoney(float value);
        public static RealCitySetOutsideGovermentMoney SetOutsideGovermentMoney;


        public static void InitDelegate()
        {
            if (GetRealCityV10 != null)
                return;
            if (GetReduceCargoDiv != null)
                return;
            if (GetResourcePrice != null)
                return;
            if (GetOutsideTouristMoney != null)
                return;
            if (GetOutsideGovermentMoney != null)
                return;
            if (SetOutsideTouristMoney != null)
                return;
            if (SetOutsideGovermentMoney != null)
                return;
            GetRealCityV10 = FastDelegateFactory.Create<RealCityGetRealCityV10>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "GetRealCityV10", instanceMethod: false);
            GetReduceCargoDiv = FastDelegateFactory.Create<RealCityGetReduceCargoDiv>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "GetReduceCargoDiv", instanceMethod: false);
            GetResourcePrice = FastDelegateFactory.Create<RealCityGetResourcePrice>(Assembly.Load("RealCity").GetType("RealCity.CustomAI.RealCityIndustryBuildingAI"), "GetResourcePrice", instanceMethod: false);
            GetOutsideTouristMoney = FastDelegateFactory.Create<RealCityGetOutsideTouristMoney>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "GetOutsideTouristMoney", instanceMethod: false);
            GetOutsideGovermentMoney = FastDelegateFactory.Create<RealCityGetOutsideGovermentMoney>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "GetOutsideGovermentMoney", instanceMethod: false);
            SetOutsideTouristMoney = FastDelegateFactory.Create<RealCitySetOutsideTouristMoney>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "SetOutsideTouristMoney", instanceMethod: false);
            SetOutsideGovermentMoney = FastDelegateFactory.Create<RealCitySetOutsideGovermentMoney>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "SetOutsideGovermentMoney", instanceMethod: false);
        }
    }
}
