using SportSchoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSchool
{
    public class TimeTable
    {
        private List<TimeTableRow> learnersOfSelectedInstructor;
        private List<TimeTableRow> learnersOfOtherInstructors;

        public List<TimeTableRow> LearnersOfSelectedInstructor
        {
            get
            {
                return learnersOfSelectedInstructor;
            }
        }
        public List<TimeTableRow> LearnersOfOtherInstructors
        {
            get
            {
                return learnersOfOtherInstructors;
            }
        }

        public TimeTable(int sectionID, int instructorID)
        {
            var adminClient = new AdminServiceReference.AdminServiceClient();
            learnersOfSelectedInstructor = adminClient.GetLearnesTimeTable(sectionID, instructorID).ToList();
            learnersOfOtherInstructors = adminClient.GetOtherLearnesTimeTable(sectionID, instructorID).ToList();
            adminClient.Close();
        }
        public TimeTable(List<TimeTableRow> learnersOfSelectedInstructor, List<TimeTableRow> learnersOfOtherInstructors)
        {
            this.learnersOfSelectedInstructor = new List<TimeTableRow>();
            foreach(var item in learnersOfSelectedInstructor)
            {
                this.learnersOfSelectedInstructor.Add(item.Clone() as TimeTableRow);
            }

            this.learnersOfOtherInstructors = new List<TimeTableRow>();
            foreach (var item in learnersOfOtherInstructors)
            {
                this.learnersOfOtherInstructors.Add(item.Clone() as TimeTableRow);
            }
        }
        public TimeTable(TimeTable table)
        {
            this.learnersOfSelectedInstructor = new List<TimeTableRow>();
            foreach (var item in table.LearnersOfSelectedInstructor)
            {
                this.learnersOfSelectedInstructor.Add(item.Clone() as TimeTableRow);
            }

            this.learnersOfOtherInstructors = new List<TimeTableRow>();
            foreach (var item in table.LearnersOfOtherInstructors)
            {
                this.learnersOfOtherInstructors.Add(item.Clone() as TimeTableRow);
            }
        }
    }
}
