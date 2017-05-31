using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SportSchoolClassLibrary
{
    [DataContract]
    public class Learner:Person, ICloneable
    {
        [DataMember]
        public DateTime BirthDate { get;  set; }

        [DataMember]
        public DateTime RegistrationDate { get; set; }

        public object Clone()
        {
            return new Learner()
            {
                ID = this.ID,
                SectionID = this.SectionID,
                Name = this.Name,
                Surname = this.Surname,
                MiddleName = this.MiddleName,
                TelephoneNumber = this.TelephoneNumber,
                Email = this.Email,
                BirthDate = new DateTime(this.BirthDate.Year, this.BirthDate.Month, this.BirthDate.Day),
                RegistrationDate = new DateTime(this.RegistrationDate.Year, this.RegistrationDate.Month, this.RegistrationDate.Day),
            };
        }
    }
}
