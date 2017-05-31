using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SportSchoolClassLibrary;
using System.Data;

namespace SportSchoolWcfService
{
    [ServiceContract]
    public interface IAdminService
    {
        [OperationContract]
        List<Learner> GetLeranersBy(int section);

        [OperationContract]
        SportSchoolClassLibrary.Instructor GetInstructorBy(string email);

        [OperationContract]
        List<Instructor> GetInstructorsBy(int section);

        [OperationContract]
        void InsertNewInstructor(Instructor instructor);

        [OperationContract]
        void InsertNewLearner(Learner learner);

        [OperationContract]
        int GetSectionID(string section);

        [OperationContract]
        List<Section> GetSections();

        [OperationContract]
        bool EmailExists(string email);

        [OperationContract]
        bool SectionExists(string section);

        [OperationContract]
        void InsertSection(Section section);

        [OperationContract]
        List<TimeTableRow> GetLearnesTimeTable(int sectionID, int instructorID);

        [OperationContract]
        List<TimeTableRow> GetOtherLearnesTimeTable(int sectionID, int instructorID);

        [OperationContract]
        Learner GetLearner(int learnerID);

        [OperationContract]
        void ChangeLearnersTimeTable(List<TimeTableRow> rows, int sectionID);

        [OperationContract]
        void FreeLearners(List<int> list);

        [OperationContract]
        void InsertIntoInstructorsTimeTable(List<TimeTableRow> rows);

        [OperationContract]
        void DeleteFromInstructorsTimeTable(List<TimeTableRow> rows);

        [OperationContract]
        List<Learner> FindLearners(Person person, string Person);

        [OperationContract]
        List<Instructor> FindInstructors(Person person, string Person);

        [OperationContract]
        void ModifyLearner(Learner learner);

        [OperationContract]
        void ModifyInstructor(Instructor instructor);
    }
}
