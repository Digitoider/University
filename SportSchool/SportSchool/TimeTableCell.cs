using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SportSchool
{
    class TimeTableCell:ICloneable, IComparable<TimeTableCell>
    {
        public bool Locked { get; set; }
        public bool Changed { get; set; }
        public ListBox Item { get; set; }
        /// <summary>
        /// X описывает время проведения занятия 
        /// Например 1 - 8:30, 2 - 10:10 и т.д.
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// X описывает день проведения занятия 
        /// Например 1 - Понедельник, 2 - Вторник и т.д.
        /// </summary>
        public int Y { get; set; } 

        public TimeTableCell()
        {
            Item = new ListBox();
            Locked = false;
            Changed = false;
        }
        public static int GetXbyHour(int hour)
        {
            switch (hour)
            {
                case  8: return 1;
                case 10: return 2;
                case 11: return 3;
                case 14: return 4;
                case 15: return 5;
                case 17: return 6;
            }
            return 1;
        }
        public static int GetYbyWeekday(string weekday)
        {
            switch (weekday)
            {
                case "Понедельник": return 1;
                case "Вторник": return 2;
                case "Среда": return 3;
                case "Четверг": return 4;
                case "Пятница": return 5;
                case "Суббота": return 6;
            }
            return 1;
        }
        public static DateTime GetDateTimeByID(int timeID)
        {
            switch (timeID)
            {
                case 1: return new DateTime(2000, 12, 24, 8, 30, 0);
                case 2: return new DateTime(2000, 12, 24, 10, 10, 0);
                case 3: return new DateTime(2000, 12, 24, 11, 50, 0);
                case 4: return new DateTime(2000, 12, 24, 14, 0, 0);
                case 5: return new DateTime(2000, 12, 24, 15, 40, 0);
                case 6: return new DateTime(2000, 12, 24, 17, 20, 0);
            }
            return new DateTime(2000, 12, 24, 8, 30, 0);
        }

        public TimeTableCell Intersect(TimeTableCell other)
        {
            TimeTableCell intersection = new TimeTableCell();
            foreach(ListBoxItem l in this.Item.Items)
            {
                bool exists = false;
                foreach(ListBoxItem t in other.Item.Items)
                {
                    if(l.Name == t.Name)
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists)
                {
                    intersection.Item.Items.Add(new ListBoxItem()
                    {
                        Name = l.Name
                    });
                }
            }
            intersection.X = X;
            intersection.Y = Y;
            return intersection;
        }

        public TimeTableCell Minus(TimeTableCell other)
        {
            TimeTableCell difference = new TimeTableCell();
            if (this.Item == null || this.Item.Items.Count == 0)
            {
                return difference;
            }
            if (other==null || other.Item == null)
            {
                /*
                foreach (ListBoxItem i in this.Item.Items)
                {
                    difference.Item.Items.Add(new ListBoxItem()
                    {
                        Name = i.Name
                    });
                }*/
                return difference;
            }
            foreach (ListBoxItem l in this.Item.Items)
            {
                bool exists = false;
                foreach (ListBoxItem t in other.Item.Items)
                {
                    if (l.Name == t.Name)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    difference.Item.Items.Add(new ListBoxItem()
                    {
                        Name = l.Name
                    });
                }
            }
            difference.X = X;
            difference.Y = Y;
            return difference;
        }

        public object Clone()
        {
            return new TimeTableCell()
            {
                Locked = this.Locked,
                Item = new ListBox() { Name = this.Item.Name },
                X = this.X,
                Y = this.Y
            };
        }

        public int CompareTo(TimeTableCell other)
        {
            if(Locked == other.Locked && Item.Name == other.Item.Name && X == other.X && Y == other.Y)
            {
                return 0;
            }
            return 1;
        }
    }
}
