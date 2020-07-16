using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using InazumaSearch;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InazumaSearchUnitTest
{
    /// <summary>
    /// 無視設定のテスト
    /// </summary>
    [TestClass]
    public class IgnoreTest
    {
        private class PathTestDataSet
        {
            public string TargetPath { get; set; }
            public string BaseDirPath { get; set; }
            public bool TargetIsDirectory { get; set; }
            public List<TestPattern> TestPatterns { get; set; } = new List<TestPattern>();

            public void AddPattern(string @pattern, bool shouldBeMatched)
            {
                TestPatterns.Add(new TestPattern() { Pattern = pattern, ShouldBeMatched = shouldBeMatched });
            }

            public class TestPattern
            {
                public string Pattern { get; set; }
                public bool ShouldBeMatched { get; set; }
            }
        }

        private readonly List<PathTestDataSet> _dataSets = new List<PathTestDataSet>();

        [TestInitialize]
        public void Initialize()
        {
            _dataSets.Clear();


            // 通常のファイルパターン
            {
                var dataSet = new PathTestDataSet()
                {
                    BaseDirPath = @"c:\Document",
                    TargetPath = @"c:\document\dir1\dir2\file1",
                    TargetIsDirectory = false
                };
                dataSet.AddPattern("file1", true);
                dataSet.AddPattern("file*", true);
                dataSet.AddPattern("file?", true);
                dataSet.AddPattern("file1*", true);
                dataSet.AddPattern("file??", false);
                dataSet.AddPattern("FiLE1", true);
                dataSet.AddPattern("/file1", false);
                dataSet.AddPattern("/file*", false);
                dataSet.AddPattern("file1/", false);
                dataSet.AddPattern("file*/", false);

                dataSet.AddPattern("dir1", true);
                dataSet.AddPattern("dir2", true);
                dataSet.AddPattern("document", false);

                dataSet.AddPattern("/dir1/dir2/file1", true);
                dataSet.AddPattern("\\dir1\\dir2\\file1", true); // \マークパターン
                dataSet.AddPattern("dir1/dir2/file1", true);
                dataSet.AddPattern("/dir1/dir2/file*", true);
                dataSet.AddPattern("/dir1/dir2/file1/", false);
                dataSet.AddPattern("/dir2/file1", false);
                dataSet.AddPattern("/dir2/file1/", false);

                dataSet.AddPattern("dir1/dir2", true);
                dataSet.AddPattern("/dir1/dir2", true);


                _dataSets.Add(dataSet);
            }

            // 通常のフォルダパターン
            {
                var dataSet = new PathTestDataSet()
                {
                    BaseDirPath = @"c:\Document",
                    TargetPath = @"c:\document\dir1\dir2\dir3",
                    TargetIsDirectory = true
                };
                dataSet.AddPattern("dir3", true);
                dataSet.AddPattern("dir*", true);
                dataSet.AddPattern("Dir3", true);
                dataSet.AddPattern("/dir3", false);
                dataSet.AddPattern("/dir*", true); // dir1にマッチ
                dataSet.AddPattern("dir3/", true);
                dataSet.AddPattern("dir*/", true);

                dataSet.AddPattern("/dir*3", false); // ワイルドカードに \ が含まれていないかどうかの確認

                dataSet.AddPattern("dir1", true);
                dataSet.AddPattern("dir2", true);
                dataSet.AddPattern("document", false);

                dataSet.AddPattern("/dir1/dir2/dir3", true);
                dataSet.AddPattern("dir1/dir2/dir3", true);
                dataSet.AddPattern("/dir1/dir2/dir3", true);
                dataSet.AddPattern("/dir1/dir2/dir3/", true);
                dataSet.AddPattern("/dir2/dir3", false);
                dataSet.AddPattern("/dir2/dir3/", false);

                dataSet.AddPattern("dir1/dir2", true);
                dataSet.AddPattern("/dir1/dir2", true);

                _dataSets.Add(dataSet);
            }

            // 中間マッチパターン
            {
                var dataSet = new PathTestDataSet()
                {
                    BaseDirPath = @"c:\Document",
                    TargetPath = @"c:\document\dir1\dir2\dir3\file4",
                    TargetIsDirectory = false
                };
                dataSet.AddPattern("/dir2/dir3/", false);
                dataSet.AddPattern("dir2/dir3/", false);
                dataSet.AddPattern("/dir2/dir3", false);
                dataSet.AddPattern("dir2/dir3", false);

                _dataSets.Add(dataSet);
            }

            // ドライブルート
            {
                var dataSet = new PathTestDataSet()
                {
                    BaseDirPath = @"c:\",
                    TargetPath = @"c:\dir1\dir2\file1",
                    TargetIsDirectory = false
                };
                dataSet.AddPattern("file1", true);
                dataSet.AddPattern("filex", false);
                dataSet.AddPattern("dir2/", true);
                dataSet.AddPattern("/dir1/dir2/", true);
                dataSet.AddPattern("/dir1/dir2/file1", true);

                _dataSets.Add(dataSet);
            }

        }

        [TestMethod]
        public void BasicTest()
        {
            foreach (var dataSet in _dataSets)
            {
                foreach (var testPattern in dataSet.TestPatterns)
                {
                    var setting = new IgnoreSetting(dataSet.BaseDirPath);
                    setting.AddPattern(testPattern.Pattern);

                    if (testPattern.ShouldBeMatched)
                    {
                        Assert.IsTrue(setting.IsMatch(dataSet.TargetPath, dataSet.TargetIsDirectory));
                    }
                    else
                    {
                        Assert.IsFalse(setting.IsMatch(dataSet.TargetPath, dataSet.TargetIsDirectory));
                    }
                }
            }
        }
    }
}
