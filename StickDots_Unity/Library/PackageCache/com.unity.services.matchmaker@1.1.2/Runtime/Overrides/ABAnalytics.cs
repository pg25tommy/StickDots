#if UGS_MATCHMAKER_ANALYTICS
using System;
using Unity.Services.Analytics;
using UnityEngine;
using Event = Unity.Services.Analytics.Internal.Event;

namespace Unity.Services.Matchmaker.Overrides
{
    internal class ABAnalytics : IABAnalytics
    {
        private readonly string _environmentId;
        private readonly IAnalyticsService _analyticsService;

        public ABAnalytics(string environmentId, IAnalyticsService analyticsService = default)
        {
            _environmentId = environmentId;
            _analyticsService = analyticsService;
        }

        public void SubmitUserAssignmentConfirmedEvent(string rcVariantID, string rcAssignmentID)
        {
            var evt = new Event("userAssignmentConfirmed", 1);
            evt.Parameters.Set("sdkMethod", "Matchmaker.ABTests.ABAnalytics.userAssignmentConfirmed");
            evt.Parameters.Set("projectID", Application.cloudProjectId);
            evt.Parameters.Set("rcVariantID", rcVariantID);
            evt.Parameters.Set("rcAssignmentID", rcAssignmentID);
            evt.Parameters.Set("rcEnvironmentID", _environmentId);
            evt.Parameters.Set("assignmentSource", "matchmaker");
            (_analyticsService?? AnalyticsService.Instance).RecordInternalEvent(evt);
        }
    }
}
#endif
