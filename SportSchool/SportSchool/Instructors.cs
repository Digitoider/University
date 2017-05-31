using SportSchoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSchool
{
    public class Instructors
    {
        private List<SportSchoolClassLibrary.Instructor> instructors = new List<SportSchoolClassLibrary.Instructor>();
        public Instructors(int sectionID)
        {
            AdminServiceReference.AdminServiceClient adminClient = new AdminServiceReference.AdminServiceClient();
            instructors = adminClient.GetInstructorsBy(sectionID).ToList();
            adminClient.Close();
        }
        public List<string> GetInstructorsNamePlusID()
        {
            List<string> instructorsNames = new List<string>();
            foreach (SportSchoolClassLibrary.Instructor instructor in instructors)
            {
                instructorsNames.Add(instructor.Surname + " " + instructor.Name + " " + instructor.MiddleName+instructor.ID);
            }
            return instructorsNames;
        }
        public SportSchoolClassLibrary.Instructor GetInstructor(string instractorName)
        {
            foreach (var i in instructors)
            {
                if (i.Surname + " " + i.Name + " " + i.MiddleName == instractorName) //return i.Clone() as SportSchoolClassLibrary.Instructor;
                 return new Instructor()
                 {
                     ID = i.ID,
                     SectionID = i.SectionID,
                     Name = i.Name,
                     Surname = i.Surname,
                     MiddleName = i.MiddleName,
                     TelephoneNumber = i.TelephoneNumber,
                     Email = i.Email,
                     Password = i.Password
                 };
            }
            return null;
        }
        public void Clear()
        {
            instructors.Clear();
        }
    }
}
