using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MyTimeTracker
{
  public static class ReportGenerator
  {
    public static void ClearBlock(string open, string close, ref string result)
    {
      Regex regex = new Regex(string.Format("{0}.*{1}", open, close), RegexOptions.Singleline);
      Match match = regex.Match(result);
      if (match.Success)
      {
        result = regex.Replace(result, string.Empty);
      }
    }


    public static string ExtractBlock(string open, string close, ref string result)
    {
      Regex regex = new Regex(string.Format("{0}(?<value>.*){1}", open, close), RegexOptions.Singleline);
      Match match = regex.Match(result);
      if (match.Success)
      {
        result = regex.Replace(result, string.Format("{0}{1}", open, close));
        return match.Groups["value"].Value;
      }

      return "";
    }

    public static string GenerateReport(ReportSettings settings, ObservableCollection<Task>tasks)
    {
      //read template;
      var template = File.ReadAllText(settings.TemplateFileName.FullName);

      //get Project Template (project)
      string projectTempl = ExtractBlock(@"<#Project>", @"</#Project>", ref template);

      string report = GenerateHeaderReport(template, settings.StartDate, settings.EndDate);

      int reportVolume = 0;
      int reportResult = 0;
      DateTime reportDuration = DateTime.MinValue;
      int reportProjectsCount = 0;

      foreach (var item in tasks)
      {
        report = report.Replace(@"<#Project></#Project>", string.Format("{0}{1}",
            GenerateProjectReport(projectTempl, item, settings.StartDate, settings.EndDate), @"<#Project></#Project>"));
        reportVolume += item.TotalVolume;
        reportResult += item.TotalResult;
        reportDuration = reportDuration.AddTicks(item.TotalSpan(settings.StartDate, settings.EndDate).Ticks);
        reportProjectsCount++;
      }

      report = report.Replace(@"<#ReportTotalVolume>", reportVolume.ToString());
      report = report.Replace(@"<#ReportTotalResult>", reportResult.ToString());
      report = report.Replace(@"<#ReportTotalTime>", reportDuration.ToShortTimeString());
      report = report.Replace(@"<#ReportTotalProjects>", reportProjectsCount.ToString());
      report = report.Replace(@"<#ReportCommonToday>", DateTime.Today.ToShortDateString());
      report = report.Replace(@"<#ReportCommonTime>", DateTime.Now.ToShortTimeString());
      report = report.Replace(@"<#ReportCommonNow>", DateTime.Now.ToString());

      ClearBlock("<#ReportName>", "</#ReportName>", ref report);
      ClearBlock("<#ReportDescription>", "</#ReportDescription>", ref report);
      ClearBlock("<#ReportExtention>", "</#ReportExtention>", ref report);
      ClearBlock("<#ReportDefaultFileName>", "</#ReportDefaultFileName>", ref report);


      report = report.Replace(@"<#Project></#Project>", string.Empty);
      
      return report;
    }

    private static string GenerateProjectReport(string projectTempl, Task item, DateTime start, DateTime end)
    {
      //get SubTasks Template (subtasks)
      string subTasksTempl = ExtractBlock(@"<#SubTasks>", @"</#SubTasks>", ref projectTempl);

      //get projectsPeriods Template (projPeriods)
      string projPeriodsTempl = ExtractBlock(@"<#Periods>", @"</#Periods>", ref projectTempl);

      projectTempl = projectTempl.Replace(@"<#ProjectName>", item.Name);
      projectTempl = projectTempl.Replace(@"<#ProjectDescription>", item.Description);
      projectTempl = projectTempl.Replace(@"<#ProjectVolume>", item.TotalVolume.ToString());
      projectTempl = projectTempl.Replace(@"<#ProjectResult>", item.TotalResult.ToString());
      projectTempl = projectTempl.Replace(@"<#ProjectDuration>", item.TotalSpan(start, end).ToShortTimeString());
      projectTempl = projectTempl.Replace(@"<#ProjectStartedAt>", item.TaskStartedAt.ToString());
      projectTempl = projectTempl.Replace(@"<#ProjectEndedAt>", item.TaskEndedAt.ToString());
      projectTempl = projectTempl.Replace(@"<#ProjectStartedAtCut>", item.TaskStartedAtCut(start).ToString());
      projectTempl = projectTempl.Replace(@"<#ProjectEndedAtCut>", item.TaskEndedAtCut(end).ToString());

      foreach (var period in item.Periods)
      {
        projectTempl = projectTempl.Replace(@"<#Periods></#Periods>", string.Format("{0}{1}",
            GeneratePeriodsReport(projPeriodsTempl, period, start, end), @"<#Periods></#Periods>"));
      }
      projectTempl = projectTempl.Replace(@"<#Periods></#Periods>", string.Empty);

      foreach (var subTask in item.SubTasks)
      {
        projectTempl = projectTempl.Replace(@"<#SubTasks></#SubTasks>", string.Format("{0}{1}",
            GenerateSubTasksReport(subTasksTempl, subTask, start, end), @"<#SubTasks></#SubTasks>"));
      }
      projectTempl = projectTempl.Replace(@"<#SubTasks></#SubTasks>", string.Empty);

      return projectTempl;
    }

    private static string GenerateSubTasksReport(string subTasksTempl, Task subTask, DateTime start, DateTime end)
    {
      //get SubTasksPeriods Template (subTasksPeriods)
      string subTasksPeriodsTempl = ExtractBlock(@"<#Periods>", @"</#Periods>", ref subTasksTempl);

      subTasksTempl = subTasksTempl.Replace(@"<#TaskName>", subTask.Name);
      subTasksTempl = subTasksTempl.Replace(@"<#TaskDescription>", subTask.Description);
      subTasksTempl = subTasksTempl.Replace(@"<#TaskVolume>", subTask.TotalVolume.ToString());
      subTasksTempl = subTasksTempl.Replace(@"<#TaskResult>", subTask.TotalResult.ToString());
      subTasksTempl = subTasksTempl.Replace(@"<#TaskDuration>", subTask.TotalSpan(start, end).ToShortTimeString());
      subTasksTempl = subTasksTempl.Replace(@"<#TaskStartedAt>", subTask.TaskStartedAt.ToString());
      subTasksTempl = subTasksTempl.Replace(@"<#TaskEndedAt>", subTask.TaskEndedAt.ToString());
      subTasksTempl = subTasksTempl.Replace(@"<#TaskStartedAtCut>", subTask.TaskStartedAtCut(start).ToString());
      subTasksTempl = subTasksTempl.Replace(@"<#TaskEndedAtCut>", subTask.TaskEndedAtCut(end).ToString());

      foreach (var period in subTask.Periods)
      {
        subTasksTempl = subTasksTempl.Replace(@"<#Periods></#Periods>", string.Format("{0}{1}",
            GeneratePeriodsReport(subTasksPeriodsTempl, period, start, end), @"<#Periods></#Periods>"));
      }
      subTasksTempl = subTasksTempl.Replace(@"<#Periods></#Periods>", string.Empty);
      return subTasksTempl;
    }

    private static string GeneratePeriodsReport(string projPeriodsTempl, Period period, DateTime start, DateTime end)
    {
      projPeriodsTempl = projPeriodsTempl.Replace(@"<#PeriodStart>", period.Start.ToString());
      projPeriodsTempl = projPeriodsTempl.Replace(@"<#PeriodEnd>", period.Finish.ToString());
      projPeriodsTempl = projPeriodsTempl.Replace(@"<#PeriodStartCut>", period.StartCut(start).ToString());
      projPeriodsTempl = projPeriodsTempl.Replace(@"<#PeriodEndCut>", period.FinishCut(end).ToString());
      projPeriodsTempl = projPeriodsTempl.Replace(@"<#PeriodDuration>", period.Span(start, end).ToShortTimeString());
      return projPeriodsTempl;
    }

    private static string GenerateHeaderReport(string template, DateTime start, DateTime end)
    {
      template = template.Replace(@"<#ReportStartDate>", start.ToShortDateString());
      template = template.Replace(@"<#ReportEndDate>", end.ToShortDateString());
      return template;
    }
  }
}
