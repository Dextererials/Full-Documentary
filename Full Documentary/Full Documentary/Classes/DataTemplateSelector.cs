using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Full_Documentary
{
    class DesktopDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DesktopAdTemplate { get; set; }
        public DataTemplate DocumentaryItemTemplate { get; set; }
    

        protected override DataTemplate SelectTemplateCore(object item,
                                                   DependencyObject container)
        {
            if (item is MovieDataItem && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
                return DocumentaryItemTemplate;
          
            if ((bool)item == true && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
                return DesktopAdTemplate;
          
            return base.SelectTemplateCore(item, container);
        }
    }

    class MobileDataTemplateSelector:DataTemplateSelector
    {

        public DataTemplate MobileAdTemplate { get; set; }
        public DataTemplate MobileItemTemplate { get; set; }


        protected override DataTemplate SelectTemplateCore(object item,
                                                   DependencyObject container)
        {
            if (item is MovieDataItem && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                return MobileItemTemplate;

            if ((bool)item == true && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                return MobileAdTemplate;

            return base.SelectTemplateCore(item, container);
        }
    }

}
