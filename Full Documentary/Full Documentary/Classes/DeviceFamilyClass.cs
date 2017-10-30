using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;

namespace Full_Documentary
{
    class DeviceFamilyClass
    {
        public double GridViewItemWidth { get; set; }
        public double GridViewItemHeight { get; set; }
        public double TextFontSize { get; set; }


        public DeviceFamilyClass()
        {
            if(AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
            {
                GridViewItemHeight = 250;
                GridViewItemWidth = 250;
                TextFontSize = 19;
            }
            else if(AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                GridViewItemHeight = 150;
                GridViewItemWidth = 150;
                TextFontSize = 14;
            }
            else
            {

            }
           
        }

    }
}
