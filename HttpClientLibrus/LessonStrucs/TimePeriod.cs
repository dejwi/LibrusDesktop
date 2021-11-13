using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientLibrus.LessonStrucs
{
    public struct TimePeriod
    {
        public DateTime start, end;
        public int mark { get; set; }
        public TimePeriod(DateTime start, DateTime end, int mark)
        {
            this.start = start;
            this.end = end;
            this.mark = mark;
        }

        public override string ToString()
        {
            return $"{start:HH:mm} - {end:HH:mm}";
        }
    }
}
