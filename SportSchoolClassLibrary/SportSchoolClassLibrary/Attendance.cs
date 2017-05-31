using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SportSchoolClassLibrary
{
    [DataContract]
    public class Attendance
    {
        [DataMember]
        public int Learner_ID { get; set; }

        [DataMember]
        public DateTime AttendanceDate { get; set; }
    }
}
