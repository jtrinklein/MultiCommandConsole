//
// Utils.cs
//
// Authors:
//  Jonathan Pryor <jpryor@novell.com>
//
// Copyright (C) 2008 Novell (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using FluentAssertions;
using Mono.Options;

namespace Tests.Mono.Options
{
	public static class Utils
	{
		public static void ShouldHaveOnlyTheseSeparators(this Option option, params string[] expectedSeparators)
		{
			var actualSeparators = option.GetValueSeparators();

			actualSeparators.Length.Should().Be(expectedSeparators.Length);
			actualSeparators.Should().ContainInOrder(expectedSeparators);
		}

		public static void AssertArgumentOutOfRangeException(string argName, Action action)
		{
			AssertArg<ArgumentOutOfRangeException>(argName, "Specified argument was out of the range of valid values.", action);
		}
		public static void AssertArgumentNullException(string argName, Action action)
		{
			AssertArg<ArgumentNullException>(argName, "Value cannot be null.", action);
		}
		public static void AssertArgumentException(string argName, string message, Action action)
		{
			AssertArg<ArgumentException>(argName, message, action);
		}
		private static void AssertArg<TException>(string argName, string message, Action action) where TException : ArgumentException
		{
			Assert<TException>(string.Format("{0}{1}Parameter name: {2}", message, Environment.NewLine, argName), action);
		}

		public static void Assert<TException>(string message, Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				e.GetType().Should().Be(typeof(TException));
				e.Message.Should().Be(message);
			}
		}

	}
}

