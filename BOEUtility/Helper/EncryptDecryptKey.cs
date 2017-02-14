using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOEUtility.Helper
{
    public class EncryptDecryptKey
    {
        private int _keyIndex;
        private string _keyValue;

        public int KeyIndex
        {
            get
            {
                return _keyIndex;
            }
            set
            {
                _keyIndex = value;
            }
        }

        public string KeyValue
        {
            get
            {
                return _keyValue;
            }
            set
            {
                _keyValue = value;
            }
        }
    }
}
