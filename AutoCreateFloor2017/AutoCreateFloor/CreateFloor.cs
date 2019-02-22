using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;

namespace AutoCreateFloor
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class CreateFloor : IExternalCommand
    {
        private ExternalEvent externalEvent;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiApplication = commandData.Application;
                UIDocument uiDocument = uiApplication.ActiveUIDocument;
                Document document = uiDocument.Document;
                //过滤项目中是否存在房间，标高和楼板类型等必要信息
                List<Element> rooms = FilterRoom(document);
                if(rooms.Count == 0)
                {
                    TaskDialog.Show("Error", "没有发现房间！");
                    return Result.Failed;
                }

                List<Element> levels = FilterLevel(document);
                if(levels.Count == 0)
                {
                    TaskDialog.Show("Error", "没有发现标高！");
                    return Result.Failed;
                }

                List<Element> floorType = FilterFloorType(document);
                if(floorType.Count == 0)
                {
                    TaskDialog.Show("Error", "没有发现楼板类型!");
                    return Result.Failed;
                }

                //存在则创建外部事件同时弹出交互对话框，输入相关数据信息
                CreateFloorEventHandler createFloorEventHandler = new CreateFloorEventHandler();
                externalEvent = ExternalEvent.Create(createFloorEventHandler);

                FloorOption floorOption = new FloorOption(rooms, floorType, levels);
                floorOption.ExEvent = externalEvent;
                floorOption.EventHandler = createFloorEventHandler;
                //floorOption.b_Apply.Click += Click_b_Apply;
                floorOption.Show();
 
                return Result.Succeeded;
            }

            catch (Exception e)
            {
                message = e.Message +"\nTargetSite:"+e.TargetSite.ToString()+"\nStackTrace:"+e.StackTrace;
                return Result.Failed;
            }
        }

        //private void Click_b_Apply(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    try
        //    {
        //    }
        //}

        private List<Element> FilterRoom(Document doc)
        {
            ElementCategoryFilter roomCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Rooms);
            FilteredElementCollector roomCollector = new FilteredElementCollector(doc);
            roomCollector.WherePasses(roomCategoryFilter);

            return roomCollector.ToElements().ToList();
        }

        private List<Element> FilterLevel(Document doc)
        {
            FilteredElementCollector levelCollector = new FilteredElementCollector(doc);
            levelCollector.OfClass(typeof(Level));

            return levelCollector.ToElements().ToList();
        }

        private List<Element> FilterFloorType(Document doc)
        {
            FilteredElementCollector floorTypeCollector = new FilteredElementCollector(doc);
            floorTypeCollector.OfCategory(BuiltInCategory.OST_Floors).OfClass(typeof(FloorType));

            return floorTypeCollector.ToElements().ToList();
        }
    }
}
