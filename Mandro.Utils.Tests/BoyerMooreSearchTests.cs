using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using NUnit.Framework;

namespace Mandro.Utils.Tests
{
    public class BoyerMooreSearchTests
    {
        [TestCase('P', ExpectedResult = 0)]
        [TestCase('A', ExpectedResult = 1)]
        [TestCase('N', ExpectedResult = 2)]
        [TestCase('M', ExpectedResult = 3)]
        public int SingleLetterTest(char letter)
        {
            // given
            string text = "PANMANAPANAMANAPMAN";
            string pattern = letter.ToString();

            // when
            int index = text.ToCharArray().IndexOf(pattern.ToCharArray());

            // then
            return index;
        }

        [TestCase("PA", ExpectedResult = 0)]
        [TestCase("AN", ExpectedResult = 1)]
        [TestCase("PANAMA", ExpectedResult = 7)]
        public int MultiLetterMatch(string pattern)
        {
            // given
            string text = "PANMANAPANAMANAPMAN";

            // when
            int index = text.ToCharArray().IndexOf(pattern.ToCharArray());

            text.IndexOf("test");

            // then
            return index;
        }

        [Test]
        public void IntMatchTest()
        {
            // given
            var source = new[] { 1, 2, 2, 3, 1, 3, 2 };
            var pattern = new[] { 1, 3, 2 };

            // when
            var index = source.IndexOf(pattern);

            // then
            Assert.AreEqual(4, index);
        }

        [Test]
        public void BadCharacterRuleShift()
        {
            // given
            string text = "PANMANAPANAMANAPMAN";
            string pattern = "PANAMA";

            // when
            var shifts = new List<int>();
            int index = text.ToCharArray().IndexOf(pattern.ToCharArray());

            // then
            Assert.That(shifts.Count, Is.LessThan(3));

        }

        [TestCase("BNAMCNAMANAMPNAMDNAM", "ANAMPNAM", 8, 3)]
        [TestCase("WHICH FINALLY HALTS.  AT THAT POINT...", "AT THAT", 22, 5)]
        public void GoodSuffixRuleShift(string text, string pattern, int expectedIndex, int expectedShiftsLimit)
        {
            // given
            var shifts = new List<int>();

            // when
            int index = text.ToCharArray().IndexOf(pattern.ToCharArray());

            // then
            Assert.AreEqual(expectedIndex, index);

            //   01234567890123456789
            //   BNAMCNAMANAMPNAMDNAM
            // 0 ANAMPNAM
            // 1      ANAMPNAM               (5)
            // 2         ANAMPNAM            (3)

            Assert.That(shifts.Count, Is.LessThan(expectedShiftsLimit));
        }

	    public class OtherType
	    {
		    public string S { get; set; }

		    protected bool Equals(OtherType other)
		    {
			    return string.Equals(S, other.S);
		    }

		    public override bool Equals(object obj)
		    {
			    if (ReferenceEquals(null, obj))
			    {
				    return false;
			    }
			    if (ReferenceEquals(this, obj))
			    {
				    return true;
			    }
			    if (obj.GetType() != this.GetType())
			    {
				    return false;
			    }
			    return Equals((OtherType)obj);
		    }

		    public override int GetHashCode()
		    {
			    return (S != null ? S.GetHashCode() : 0);
		    }

		    public static bool operator ==(OtherType left, OtherType right)
		    {
			    return Equals(left, right);
		    }

		    public static bool operator !=(OtherType left, OtherType right)
		    {
			    return !Equals(left, right);
		    }
	    }

		[Test]
	    public void OtherTypeTests()
	    {
			// given



			var text = new[] { new OtherType { S = "O" }, new OtherType() { S = "N" } };
			var pattern = new[] { new OtherType { S = "O" }, new OtherType() { S = "N" } };

			// when
			int index = text.IndexOf(pattern);

			// then
			Assert.That(index, Is.EqualTo(0));
	    }
        
        [Test, Ignore]
        public void TestPerformance()
        {
            const string patternString = "CloseHandle(17cMDR)";
            var patternChars = patternString.ToCharArray();

            for (int size = 128; size <= 1048576; size *= 2)
            {
                var testString = GenerateString(size, patternString);
                var testStringChars = testString.ToCharArray();

                int result1 = 0;
                int result2 = 0;

                var builtInTime = MeasureTimeInMs(() => result1 = testString.IndexOf(patternString));
                var myImplementation = MeasureTimeInMs(() => result2 = testStringChars.IndexOf(patternChars));

                Console.Write("{0}\t{1}\t{2}\r\n", size, builtInTime.ToString("F"), myImplementation.ToString("F"));

                Assert.AreEqual(result2, result1);
            }
        }

        private static double MeasureTimeInMs(Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var repeats = 100;
            for (int i = 0; i < repeats; ++i)
            {
                action();
            }

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds / (double)repeats;
        }

        private static string GenerateString(int size, string patternString)
        {
            var random = new Random((int)DateTime.Now.Ticks);

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < size; i++)
            {
                stringBuilder.Append(random.Next('A', 'Z'));
            }

            stringBuilder.Append(patternString);

            return stringBuilder.ToString();
        }

        //[Test]
        //public void TestZ()
        //{
        //    // given
        //    string pattern = "PANAMA";

        //    // when
        //    var Z = ParsePatternIntoZ(pattern.ToCharArray());

        //    // then
        //    var expectedResults = new Dictionary<char, int[]>
        //                          { { 'A', new[] { -1, -1, 1, 1, 3, 3 } }, { 'M', new[] { -1, -1, -1, -1, -1, 4 } }, { 'N', new[] { -1, -1, -1, 2, 2, 2 } }, { 'P', new[] { -1, 0, 0, 0, 0, 0 } } };

        //    foreach (var expectedResult in expectedResults)
        //    {
        //        CollectionAssert.AreEqual(expectedResult.Value, Z[expectedResult.Key]);
        //    }

        //}

        //[TestCase("PANAMA", new[] { 3, -1, -1, -1, -1, })]
        //[TestCase("ANAMPNAM", new[] { -1, -1, 1, -1, -1, -1, -1 })]
        //public void TestL(string patternString, int[] expectedL)
        //{
        //    // given
        //    var pattern = patternString.ToCharArray();
        //    var Z = ParsePatternIntoZ(pattern);

        //    // when
        //    int[] L = GetL(pattern, Z);

        //    // then
        //    CollectionAssert.AreEqual(expectedL, L);

        //}
    }
}
