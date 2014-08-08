using Newtonsoft.Json;
using StringResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using WindDataLib;

namespace WindInfo.Code
{
    public class LocalizedStrings
    {
        public static AppResources Resources
        {
            get
            {
                return r;
            }


        }

        public static CreditInfo FakeModel
        {
            get
            {
                return JsonConvert.DeserializeObject<CreditInfo>("{\"Username\":\"3405386652\",\"Password\":\"01030507\",\"NumberInfos\":[{\"clShowed\":false,\"smsShowed\":false,\"gigaShowed\":false,\"minShowed\":false,\"Number\":\"3471282301\",\"Credit\":19.46,\"CreditLimit\":0.0,\"CreditLimitReached\":false,\"Minutes\":150,\"MinutesLimit\":0,\"MinutesLimitReached\":false,\"MinutesTotal\":399,\"SMS\":0,\"SMSLimit\":0,\"SMSTotal\":400,\"SMSLimitReached\":false,\"Gigabytes\":98,\"GigabytesLimit\":0.0,\"GigabytesTotal\":100.0,\"GigabytesLimitReached\":false,\"NotifyEnabled\":false,\"Brush\":null,\"FriendlyName\":null,\"LastUpdate\":\"2014-01-04T09:19:43.5582003+01:00\",\"ExpirationDate\":\"2014-01-29T00:00:00\"},{\"clShowed\":false,\"smsShowed\":false,\"gigaShowed\":false,\"minShowed\":false,\"Number\":\"3405386652\",\"Credit\":9.68,\"CreditLimit\":0.0,\"CreditLimitReached\":false,\"Minutes\":73,\"MinutesLimit\":0,\"MinutesLimitReached\":false,\"MinutesTotal\":119,\"SMS\":78,\"SMSLimit\":0,\"SMSTotal\":120,\"SMSLimitReached\":false,\"Gigabytes\":77,\"GigabytesLimit\":0.0,\"GigabytesTotal\":100.0,\"GigabytesLimitReached\":false,\"NotifyEnabled\":false,\"Brush\":null,\"FriendlyName\":null,\"LastUpdate\":\"2014-01-04T09:19:44.3348915+01:00\",\"ExpirationDate\":\"2014-01-13T00:00:00\"}]}");
            }
        }

        private static AppResources r = new AppResources();
    }

}
