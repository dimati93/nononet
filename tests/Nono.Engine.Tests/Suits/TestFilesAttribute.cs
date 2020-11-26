﻿using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Nono.Engine.Tests.Suits
{
    public class TestFilesAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var testFiles = TestResources.GetAllFiles();

            foreach (var testFile in testFiles)
            {
                using (var reader = new NonFileReader(testFile))
                {
                    var (rows, columns) = reader.Read();

                    yield return new object[] { rows, columns };
                }
            }
        }
    }
}