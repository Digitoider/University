using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SportSchoolClassLibrary
{
    [DataContract]
    public class Section: ICloneable
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        public object Clone()
        {
            return new Section() { ID = this.ID, Name = this.Name };
        }
    }
}
