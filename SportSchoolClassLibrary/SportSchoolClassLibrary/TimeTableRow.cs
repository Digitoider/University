using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SportSchoolClassLibrary
{
    [DataContract]
    public class TimeTableRow : IComparable<TimeTableRow>, ICloneable
    {
        [DataMember]
        public int Holder_ID { get; set; }

        [DataMember]
        public int Weekday_ID { get; set; }

        [DataMember]
        public DateTime TableTime { get; set; }

        
        [DataMember]
        public TimeTableRow prevValue { get; set; }
        /*
        [DataMember]
        public int OldHolder_ID { get; set; }

        [DataMember]
        public int OldWeekday_ID { get; set; }
        */
        public int CompareTo(TimeTableRow other)
        {
            if (Weekday_ID == other.Weekday_ID && TableTime.Hour == other.TableTime.Hour)
            {
                return 0;
            }
            if (Weekday_ID < other.Weekday_ID) return -1;
            return 1;
        }

        public object Clone()
        {
            return new TimeTableRow() { Holder_ID = this.Holder_ID, Weekday_ID = this.Weekday_ID, TableTime = this.TableTime };
        }

        public bool HolderID_IsIn(List<int> list)
        {
            foreach(int elem in list)
            {
                if (elem == Holder_ID)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
