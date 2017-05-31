using System;
using MySql.Data;
using MySql.Data.SqlClient;

public class Learner
{
    public int ID { get; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string MiddleName { get; private set; }
    public string Email { get; private set; }
    public string TelephoneNumber { get; private set; }

    /// <summary>
    /// даты в MySQL хранятся в формате "ГГГГ-ММ-ДД ЧЧ:ММ:СС"
    /// маска формата - "%Y-%m-%d %H:%i:%s" (стандарт ISO)
    /// DATETIME
    ///Хранит время в виде целого числа вида YYYYMMDDHHMMSS, 
    ///используя для этого 8 байтов.Это время не зависит от временной зоны.
    ///Оно всегда отображается при выборке точно так же, как было сохранено, 
    ///независимо от того какой часовой пояс установлен в MySQL.
    /// </summary>
    public MySqlDateTime BirthDate { get; private set; }
    public Learner()
    {
    }
}