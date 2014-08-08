using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WindDataLib
{
    [ImplementPropertyChanged]
    public class CreditInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string Type { get; set; }


        public ObservableCollection<NumberInfo> NumberInfos { get; set; }

        public void Merge(CreditInfo newData)
        {
            if (newData == null)
                return;

            if (NumberInfos == null)
                NumberInfos = new ObservableCollection<NumberInfo>();

            foreach (var item in newData.NumberInfos)
            {
                var it = NumberInfos.Where(n => n.Number == item.Number);

                if (!it.Any())
                    NumberInfos.Add(item);

                var nmb = it.Single();

                nmb.ExpirationDate = item.ExpirationDate;
                nmb.Minutes = item.Minutes;
                nmb.Gigabytes = item.Gigabytes;
                nmb.SMS = item.SMS;
                nmb.Credit = item.Credit;
                nmb.LastUpdate = item.LastUpdate;
            }

            //Remove what is not in the new data.
            foreach (var item in NumberInfos.Where(x => !newData.NumberInfos.Select(y => y.Number).Contains(x.Number)))
                NumberInfos.Remove(item);
        }
    }


}

