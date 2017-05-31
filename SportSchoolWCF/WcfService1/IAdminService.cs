using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SportSchoolClassLibrary;

namespace SportSchoolWCF
{
    [ServiceContract]
    public interface IAdminService
    {
        [OperationContract]
        List<Learner> GetLeranersBy(string section);

        [OperationContract]
        List<Instructor> GetInstructorsBy(string section);

        [OperationContract]
        void InsertNewInstructor(Instructor instructor);

        [OperationContract]
        void InsertNewLearner(Learner learner);

        [OperationContract]
        int GetSectionID(string section);
    }
}
