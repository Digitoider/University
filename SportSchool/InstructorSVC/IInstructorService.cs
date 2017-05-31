using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace InstructorSVC
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract]
    public interface IInstructorService
    {

        [OperationContract]
        List<SportSchoolClassLibrary.Learner> GetLearners(string instructorsEmail, int dayOfWeek, int hour);

        [OperationContract]
        void SaveAttendance(List<SportSchoolClassLibrary.Learner> learners);

        [OperationContract]
        SportSchoolClassLibrary.Instructor GetInstructor(string email);

        [OperationContract]
        void AddCompetition(SportSchoolClassLibrary.Competition competition, List<int> learnersIDs);

        [OperationContract]
        List<SportSchoolClassLibrary.Competition> GetCompetitions();

        [OperationContract]
        List<SportSchoolClassLibrary.Learner> GetCompetitiors(SportSchoolClassLibrary.Competition competition);

        [OperationContract]
        List<SportSchoolClassLibrary.Attendance> GetAttendance(int learnerID);
    }
}
