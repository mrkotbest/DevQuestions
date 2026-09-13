namespace DevQuestions.Domain.Reports;

public enum ReportStatus
{
    /// <summary> The report is open and has not been addressed yet. </summary>
    Open = 0,

    /// <summary> The report is currently being worked on. </summary>
    InProgress = 1,

    /// <summary> The report has been resolved. </summary>
    Resolved = 2,

    /// <summary> The report has been dismissed and will not be addressed. </summary>
    Dismissed = 3
}
