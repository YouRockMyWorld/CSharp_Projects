using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Windows.Media.Imaging;
using AutoCreateFloor;

namespace AutoCreateFloor
{
    class Ribbon : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel myPanel = application.CreateRibbonPanel("SC-Tools");
            PushButtonData pushButtonData_AutoCreateFloor = new PushButtonData("RoomAutoCreateFloor", "房间生成楼板", System.Reflection.Assembly.GetExecutingAssembly().Location, "AutoCreateFloor.CreateFloor");
            PushButton pushButton_AutoCreateFloor = myPanel.AddItem(pushButtonData_AutoCreateFloor) as PushButton;
            pushButton_AutoCreateFloor.LargeImage = new BitmapImage(new Uri("pack://application:,,,/AutoCreateFloor;component/image/CreateFloor.png"));
            pushButton_AutoCreateFloor.ToolTip = "过滤项目中所有房间并根据房间轮廓生成楼板";

            return Result.Succeeded;
        }
    }
}
