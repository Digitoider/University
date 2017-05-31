using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SportSchoolClassLibrary
{
    [DataContract]
    public class Instructor : Person, ICloneable
    {
        public object Clone()
        {
            return new Instructor()
            {
                ID = this.ID,
                SectionID = this.SectionID,
                Name = this.Name,
                Surname = this.Surname,
                MiddleName = this.MiddleName,
                TelephoneNumber = this.TelephoneNumber,
                Email = this.Email,
                Password = this.Password
            };
        }
    }
}
