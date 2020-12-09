using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DrawIt.Uml
{
    [Serializable]
    public class UmlMemberDescription
    {
        public string Name
        {
            get;
            set;
        }

        public string DataType
        {
            get;
            set;
        }

        public bool IsPublic
        {
            get;
            set;
        }
    }
}
