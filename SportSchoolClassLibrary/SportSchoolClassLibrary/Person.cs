using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SportSchoolClassLibrary
{
    [DataContract]
    public class Person
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int SectionID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string TelephoneNumber { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
