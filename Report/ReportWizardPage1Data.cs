using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MyTimeTracker
{
  public class ReportWizardPage1Data : INotifyPropertyChanged
  {
    public readonly int tpvDay = 0;
    public readonly int tpvWeek = 1;
    public readonly int tpvMonth = 2;

    private bool allTime = true;
    public bool AllTime
    {
      get { return allTime; }
      set
      {
        allTime = value;
        Notify("AllTime");
      }
    }

    private bool selectedPeriod = false;
    public bool SelectedPeriod
    {
      get { return selectedPeriod; }
      set
      {
        selectedPeriod = value;
        Notify("SelectedPeriod");
      }
    }

    private bool freePeriod = false;
    public bool FreePeriod
    {
      get { return freePeriod; }
      set
      {
        freePeriod = value;
        Notify("FreePeriod");
      }
    }

    private int periods;
    public int Periods
    {
      get { return periods; }
      set
      {
        periods = value; 
        Notify("Periods");
      }
    }

    private DateTime startDate = DateTime.Today;
    public DateTime StartDate
    {
      get { return startDate; }
      set
      {
        startDate = value;
        Notify("StartDate");
      }
    }

    private DateTime endDate = DateTime.Today;
    public DateTime EndDate
    {
      get { return endDate; }
      set
      {
        endDate = value;
        Notify("EndDate");
      }
    }

    public override string ToString()
    {
      return String.Format("AllTime: {0}{1}SelectedPeriod: {2} -> Periods: {3}{1}FreePeriod: {4} -> StartDate: {5} EndDate: {6}",
        AllTime, Environment.NewLine, SelectedPeriod, Periods, FreePeriod, StartDate, EndDate);
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
