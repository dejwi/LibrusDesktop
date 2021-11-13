using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientLibrus.LessonStrucs
{
    public struct SchoolDay
    {
        public List<Lesson> lessons { get; set; }
        public DateTime day { get; set; }

        public SchoolDay(List<Lesson> lessons, DateTime day)
        {
            this.lessons = lessons;
            this.day = day;
        }
    }
}
