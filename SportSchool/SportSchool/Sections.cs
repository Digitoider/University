using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSchool
{
    public class Sections
    {
        private List<SportSchoolClassLibrary.Section> sections;
        public Sections()
        {
            AdminServiceReference.AdminServiceClient adminClient = new AdminServiceReference.AdminServiceClient();
            sections = adminClient.GetSections().ToList();
            adminClient.Close();
        }
        public List<string> GetSectionNames()
        {
            List<string> sectionNames = new List<string>();
            foreach (SportSchoolClassLibrary.Section section in sections)
            {
                sectionNames.Add(section.Name);
            }
            return sectionNames;
        }
        public int GetSectionID(string sectionName)
        {
            foreach (var section in sections)
            {
                if (section.Name == sectionName) return section.ID;
            }
            return 1;
        }
        public SportSchoolClassLibrary.Section GetSection(string sectionName)
        {
            foreach (var section in sections)
            {
                if (section.Name == sectionName) return section.Clone() as SportSchoolClassLibrary.Section;
            }
            return null;
        }
    }
}
