﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using NUnit.Framework;

namespace CloudinaryDotNet.IntegrationTest.AdminApi
{
    public class UsageReportTest: IntegrationTestBase
    {
        [Test]
        public void TestUsage()
        {
            UploadTestImageResource(); // making sure at least one resource exists

            var result = m_cloudinary.GetUsage();

            AssertUsageResult(result);
        }

        [Test]
        public async Task TestUsageAsync()
        {
            await UploadTestImageResourceAsync(); // making sure at least one resource exists

            var result = await m_cloudinary.GetUsageAsync();

            AssertUsageResult(result);
        }

        private void AssertUsageResult(UsageResult result)
        {
            Assert.NotNull(result.LastUpdated);
        }
    }
}
