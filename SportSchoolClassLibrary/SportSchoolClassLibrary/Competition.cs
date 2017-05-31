using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SportSchoolClassLibrary
{
    [DataContract]
    public class Competition
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Description{ get; set; }
    }
}
