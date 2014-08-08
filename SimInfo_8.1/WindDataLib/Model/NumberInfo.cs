using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindDataLib
{
    [ImplementPropertyChanged]
    public class NumberInfo
    {

        public NumberInfo()
        {
            SMS = SMSLimit = Minutes = MinutesLimit = Gigabytes = 0;
            GigabytesLimit = 0;
        }
        public static bool operator ==(NumberInfo one, NumberInfo other)
        {
            return one.Number == other.Number;
        }

        public static bool operator !=(NumberInfo one, NumberInfo other)
        {
            return !(one == other);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(NumberInfo))
                return false;

            return (NumberInfo)obj == this;
        }

        [DoNotNotify]
        public bool IsCompletelyInvalid
        {
            get
            {
                return Credit == 0 && SMS == 0 && Minutes == 0 & Gigabytes == 0;
            }
        }

        public override int GetHashCode()
        {
            return Credit.GetHashCode() ^ SMS.GetHashCode() ^ Minutes.GetHashCode() ^ Gigabytes.GetHashCode() ^ Number.GetHashCode();
        }

        public string Number { get; set; }

        [AlsoNotifyFor("CreditLimitReached")]
        public float Credit { get; set; }

        [AlsoNotifyFor("CreditLimitReached")]
        public float CreditLimit { get; set; }

        public bool CreditLimitReached { get { return Credit >= 0 && Credit < CreditLimit; } }

        [AlsoNotifyFor("MinutesLimitReached")]
        public int Minutes { get; set; }

        [AlsoNotifyFor("MinutesLimitReached")]
        public int MinutesLimit { get; set; }

        public bool MinutesLimitReached { get { return Minutes >= 0 && Minutes < MinutesLimit; } }
        public int MinutesTotal { get; set; }

        [AlsoNotifyFor("SMSLimitReached")]
        public int SMS { get; set; }

        [AlsoNotifyFor("SMSLimitReached")]
        public int SMSLimit { get; set; }

        public int SMSTotal { get; set; }

        public bool SMSLimitReached { get { return SMS >= 0 && SMS < SMSLimit; } }

        [AlsoNotifyFor("GigabytesLimitReached")]
        public int Gigabytes { get; set; }

        [AlsoNotifyFor("GigabytesLimitReached")]
        public float GigabytesLimit { get; set; }
        public float GigabytesTotal { get; set; }
        public bool GigabytesLimitReached { get { return Gigabytes >= 0 && Gigabytes < GigabytesLimit; } }
        public bool NotifyEnabled { get; set; }
        public string Brush { get; set; }
        public string FriendlyName { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public bool clShowed;

        public bool smsShowed;

        public bool gigaShowed;

        public bool minShowed;


    }
}
