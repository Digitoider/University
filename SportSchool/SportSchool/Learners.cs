using SportSchoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSchool
{
    public class Learners//:ICloneable
    {
        private List<SportSchoolClassLibrary.Learner> learners;
        public List<SportSchoolClassLibrary.Learner> Items
        {
            get
            {
                return learners;
            }
            set
            {
                learners = value;
            }
        }
        public Learners(int sectionID)
        {
            var adminClient = new AdminServiceReference.AdminServiceClient();
            learners = adminClient.GetLeranersBy(sectionID).ToList();
            adminClient.Close();
        }
        public Learners()
        {
            learners = new List<SportSchoolClassLibrary.Learner>();
        }

        /*public object Clone()
        {
            List<Learner> list = new List<Learner>();
            foreach (Learner l in learners)
            {
                list.Add(new Learner()
                {
                    ID = l.ID,
                    SectionID = l.SectionID,
                    Name = l.Name,
                    Surname = l.Surname,
                    MiddleName = l.MiddleName,
                    TelephoneNumber = l.TelephoneNumber,
                    Email = l.Email,
                    BirthDate = new DateTime(l.BirthDate.Year, l.BirthDate.Month, l.BirthDate.Day)
                });
            }
            return list;
        }*/
    }
}
