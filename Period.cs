using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace MyTimeTracker
{
    [Serializable]
    public class Period : INotifyPropertyChanged
    {
        [XmlElement("PeriodID")]
        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                Notify("PeriodID");
            }
        }
        private int _id;


        [XmlElement("Start")]
        public DateTime Start
        {
            get { return _start; }
            set
            {
                _start = value;
                Notify("Start");
            }
        }
        private DateTime _start;


        [XmlElement("Finish")]
        public DateTime Finish
        {
            get { return _finish; }
            set
            {
                _finish = value;
                Notify("Finish");
            }
        }
        private DateTime _finish;

        public DateTime StartCut(DateTime cut)
        {
            if (cut > Start) return cut;
            else return Start;
        }

        public DateTime FinishCut(DateTime cut)
        {
            DateTime fin = Finish;
            if (Finish.Ticks == 0) fin = DateTime.Now;

            if (cut < fin) return cut;
            else return fin;
        }

        [XmlElement("State")]
        public States State
        {
            get { return _state; }
            set
            {
                _state = value;
                Notify("State");
            }
        }
        private States _state;

        public Period()
        {
            State = States.Unknown;
        }

        public Period(int id)
            : this()
        {
            ID = id;

        }

        public void Open()
        {
            State = States.Opened;
            Start = DateTime.Now;
        }

        public void Close()
        {
            State = States.Closed;
            Finish = DateTime.Now;
        }

        [XmlIgnore]
        public DateTime Duration
        {
            get
            {
                if (Finish.Ticks == 0)
                {
                    return new DateTime(DateTime.Now.Ticks - Start.Ticks);
                } return new DateTime(Finish.Ticks - Start.Ticks);
            }
        }

        [XmlIgnore]
        public DateTime DurationToday
        {
            get
            {
                return Span(DateTime.Today, DateTime.Now);
            }
        }

        public DateTime Span(DateTime start, DateTime end)
        {
            DateTime startTime = Start;
            DateTime finishTime = Finish;
            if (startTime < start) startTime = start;
            if (finishTime.Ticks == 0) finishTime = DateTime.Now;
            if (finishTime > end) finishTime = end;
            if (finishTime < startTime) return DateTime.MinValue;
            return new DateTime(finishTime.Ticks - startTime.Ticks);
        }

        public void UpdateDuration()
        {
            Notify("Duration");
            Notify("DurationToday");
        }

        public override string ToString()
        {
            if (Finish.Ticks == 0)
            {
                return String.Format("begin: {0}", Start.ToLongTimeString());
            } return String.Format("begin: {0}, end: {1}, length: {2}",
              Start.ToLongTimeString(), Finish.ToLongTimeString(), Duration.ToLongTimeString());
        }

        #region INotifyPropertyChanged

        private void Notify(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
