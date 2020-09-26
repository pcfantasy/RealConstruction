using System.Reflection;

namespace RealConstruction.Util
{
    public class RealCityUtil
    {
        public delegate bool RealCityGetRealCityV10();
        public static RealCityGetRealCityV10 GetRealCityV10;

        public delegate float RealCityGetAverageSalary();
        public static RealCityGetAverageSalary GetAverageSalary;

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
                if (GetAverageSalary != null)
                    if (GetReduceCargoDiv != null)
                        if (GetResourcePrice != null)
                            if (GetOutsideTouristMoney != null)
                                if (GetOutsideGovermentMoney != null)
                                    if (SetOutsideTouristMoney != null)
                                        if (SetOutsideGovermentMoney != null)
                                            return;

            DebugLog.LogToFileOnly("Try to InitDelegate in RealCityUtil");
            GetRealCityV10 = FastDelegateFactory.Create<RealCityGetRealCityV10>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "GetRealCityV10", instanceMethod: false);
            GetAverageSalary = FastDelegateFactory.Create<RealCityGetAverageSalary>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "GetAverageSalary", instanceMethod: false);
            GetReduceCargoDiv = FastDelegateFactory.Create<RealCityGetReduceCargoDiv>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "GetReduceCargoDiv", instanceMethod: false);
            GetResourcePrice = FastDelegateFactory.Create<RealCityGetResourcePrice>(Assembly.Load("RealCity").GetType("RealCity.CustomAI.RealCityIndustryBuildingAI"), "GetResourcePrice", instanceMethod: false);
            GetOutsideTouristMoney = FastDelegateFactory.Create<RealCityGetOutsideTouristMoney>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "GetOutsideTouristMoney", instanceMethod: false);
            GetOutsideGovermentMoney = FastDelegateFactory.Create<RealCityGetOutsideGovermentMoney>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "GetOutsideGovermentMoney", instanceMethod: false);
            SetOutsideTouristMoney = FastDelegateFactory.Create<RealCitySetOutsideTouristMoney>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "SetOutsideTouristMoney", instanceMethod: false);
            SetOutsideGovermentMoney = FastDelegateFactory.Create<RealCitySetOutsideGovermentMoney>(Assembly.Load("RealCity").GetType("RealCity.RealCity"), "SetOutsideGovermentMoney", instanceMethod: false);
        }
    }
}
