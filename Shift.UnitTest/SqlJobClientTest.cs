﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shift.Entities;
using Shift.DataLayer;

namespace Shift.UnitTest
{
    [TestClass]
    public class SqlJobClientTest
    {
        JobClient jobClient;
        const string appID = "TestAppID";

        public SqlJobClientTest()
        {
            //Configure storage connection
            var config = new ClientConfig();

            //config.DBConnectionString = "Data Source=localhost\\SQL2014;Initial Catalog=ShiftJobsDB;Integrated Security=SSPI;";
            string cs = SqlDBHelpers.GetLocalDB("testdatabase");
            config.DBConnectionString = cs;

            config.StorageMode = "mssql";
            jobClient = new JobClient(config);
        }

        [TestMethod]
        public void GetJobInvalidTest()
        {
            var job = jobClient.GetJob("-123");

            Assert.IsNull(job);
        }

        [TestMethod]
        public void GetJobValidTest()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            var job = jobClient.GetJob(jobID);
            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(job);
            Assert.AreEqual(jobID, job.JobID);
        }

        [TestMethod]
        public void AddJobTest1()
        {
            var jobID = jobClient.Add(() => Console.WriteLine("Hello Test"));
            var job = jobClient.GetJob(jobID);
            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(job);
            Assert.AreEqual(jobID, job.JobID);
        }

        [TestMethod]
        public void AddJobTest2()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            var job = jobClient.GetJob(jobID);
            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(job);
            Assert.AreEqual(jobID, job.JobID);
            Assert.AreEqual(appID, job.AppID);
        }

        [TestMethod]
        public void AddJobTest3()
        {
            var jobID = jobClient.Add(appID, "-123", "TestJobType", () => Console.WriteLine("Hello Test"));
            var job = jobClient.GetJob(jobID);
            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(job);
            Assert.AreEqual(jobID, job.JobID);
            Assert.AreEqual(appID, job.AppID);
            Assert.AreEqual("-123", job.UserID);
            Assert.AreEqual("TestJobType", job.JobType);
        }

        [TestMethod]
        public void AddJobTest4()
        {
            var jobID = jobClient.Add(appID, "-123", "TestJobType", "Test.JobName", () => Console.WriteLine("Hello Test"));
            var job = jobClient.GetJob(jobID);
            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(job);
            Assert.AreEqual(jobID, job.JobID);
            Assert.AreEqual(appID, job.AppID);
            Assert.AreEqual("-123", job.UserID);
            Assert.AreEqual("TestJobType", job.JobType);
            Assert.AreEqual("Test.JobName", job.JobName);
        }

        [TestMethod]
        public void UpdateJobTest1()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            jobClient.Update(jobID, () => Console.WriteLine("Hello Test Updated"));
            var job = jobClient.GetJob(jobID);
            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(job);
            Assert.AreEqual("[\"\\\"Hello Test Updated\\\"\"]", job.Parameters);
        }

        [TestMethod]
        public void UpdateJobTest2()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            jobClient.Update(jobID, "TestAppIDUpdated", () => Console.WriteLine("Hello Test Updated"));
            var job = jobClient.GetJob(jobID);
            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(job);
            Assert.AreEqual("TestAppIDUpdated", job.AppID);
            Assert.AreEqual("[\"\\\"Hello Test Updated\\\"\"]", job.Parameters);
        }

        [TestMethod]
        public void UpdateJobTest3()
        {
            var jobID = jobClient.Add(appID, "-123", "TestJobType", "Test.JobName", () => Console.WriteLine("Hello Test"));
            jobClient.Update(jobID, "TestAppIDUpdated", "-222", "TestJobTypeUpdated", "Test.JobNameUpdated", () => Console.WriteLine("Hello Test Updated"));
            var job = jobClient.GetJob(jobID);
            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(job);
            Assert.AreEqual("TestAppIDUpdated", job.AppID);
            Assert.AreEqual("-222", job.UserID);
            Assert.AreEqual("TestJobTypeUpdated", job.JobType);
            Assert.AreEqual("Test.JobNameUpdated", job.JobName);
            Assert.AreEqual("[\"\\\"Hello Test Updated\\\"\"]", job.Parameters);
        }

        [TestMethod]
        public void SetCommandStopTest()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            jobClient.SetCommandStop(new List<string> { jobID });
            var job = jobClient.GetJob(jobID);
            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(job);
            Assert.AreEqual(JobCommand.Stop, job.Command);
        }

        [TestMethod]
        public void SetCommandRunNowTest()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            jobClient.SetCommandRunNow(new List<string> { jobID });
            var job = jobClient.GetJob(jobID);
            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(job);
            Assert.AreEqual(JobCommand.RunNow, job.Command);
        }

        [TestMethod]
        public void GetJobViewTest()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            var jobView = jobClient.GetJobView(jobID);
            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(jobView);
            Assert.AreEqual(jobID, jobView.JobID);
            Assert.IsInstanceOfType(jobView, typeof(JobView));
        }

        [TestMethod]
        public void GetJobViewsTest()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            var jobID2 = jobClient.Add(appID, () => Console.WriteLine("Hello Test2"));
            var jobViews = jobClient.GetJobViews(0, 10);
            jobClient.DeleteJobs(new List<string>() { jobID, jobID2 });

            Assert.IsInstanceOfType(jobViews, typeof(JobViewList));
            Assert.IsNotNull(jobViews);
            Assert.IsNotNull(jobViews.Items);
            Assert.IsTrue(jobViews.Items.Count > 0);
            Assert.IsTrue(jobViews.Total > 0);
        }

        [TestMethod]
        public void ResetJobsTest()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            jobClient.SetCommandStop(new List<string> { jobID });
            var job = jobClient.GetJob(jobID);
            Assert.IsNotNull(job);
            Assert.AreEqual(JobCommand.Stop, job.Command); //ensure it is set to 'stop' command

            //try to reset
            jobClient.ResetJobs(new List<string> { jobID });
            job = jobClient.GetJob(jobID);

            jobClient.DeleteJobs(new List<string>() { jobID });

            Assert.IsNotNull(job);
            Assert.IsTrue(string.IsNullOrWhiteSpace(job.Command));
        }

        [TestMethod]
        public void DeleteJobsTest()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            var job = jobClient.GetJob(jobID);
            Assert.IsNotNull(job); //ensure it exists

            //try to delete
            jobClient.DeleteJobs(new List<string> { jobID });
            job = jobClient.GetJob(jobID);

            Assert.IsNull(job);
        }

        [TestMethod]
        public void GetJobStatusCountTest()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            var statusCount = jobClient.GetJobStatusCount(appID, null);
            jobClient.DeleteJobs(new List<string> { jobID });

            Assert.IsNotNull(statusCount); 
            Assert.IsTrue(statusCount.Count > 0);
        }

        [TestMethod]
        public void GetProgressTest()
        {
            var jobID = jobClient.Add(appID, () => Console.WriteLine("Hello Test"));
            var progress = jobClient.GetProgress(jobID);
            jobClient.DeleteJobs(new List<string> { jobID });

            Assert.IsNotNull(progress);
            Assert.IsInstanceOfType(progress, typeof(JobStatusProgress));
            Assert.AreEqual(jobID, progress.JobID);
        }

        public void GetCachedProgressTest()
        {
            //Note: Unable to test, no progress data in Redis, since no running job.
        }
    }
}
