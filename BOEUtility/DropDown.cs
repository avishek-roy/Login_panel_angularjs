using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BOEUtility
{
    public class DropDown
    {
        public static SelectList ClientType()
        {
            List<KeyValuePair<string, string>> statusKeyValuePairs = new List<KeyValuePair<string, string>>();
            statusKeyValuePairs.Add(new KeyValuePair<string, string>("NewClient", "New Client"));
            statusKeyValuePairs.Add(new KeyValuePair<string, string>("ExistingClient", "Existing Client"));
            SelectList statusSelectList = new SelectList(statusKeyValuePairs, "Key", "Value");
            return statusSelectList;
        }

        public static SelectList ComuncationType()
        {
            List<KeyValuePair<string, string>> statusKeyValuePairs = new List<KeyValuePair<string, string>>();
            statusKeyValuePairs.Add(new KeyValuePair<string, string>("Phone", "Over Phone"));
            statusKeyValuePairs.Add(new KeyValuePair<string, string>("Email", "E-Mail"));
            statusKeyValuePairs.Add(new KeyValuePair<string, string>("Meeting", "Face To Face Meet"));
            SelectList statusSelectList = new SelectList(statusKeyValuePairs, "Key", "Value");
            return statusSelectList;
        }

    }
}
