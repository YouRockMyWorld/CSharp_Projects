using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordApp
{
    public class CurrentBookMark
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ReturnBookMarksEventArgs : EventArgs
    {
        public List<string> BookMarks { get; set; }
    }

    public class ModifyInfoEventArgs : EventArgs
    {
        //public int Id { get; set; }
        public ModifyInfoItem ModifyInfoItem { get; set; }
    }

    public class ReplaceSetting
    {
        public int Id { get; set; }
        public string BookMarkName { get; set; }
        public string ReplaceContent { get; set; }
    }

    public class ModifyInfoItem
    {
        public string TargetName { get; set; }
        public string MatchName { get; set; }
        public string ModifyContent { get; set; }
    }
}
