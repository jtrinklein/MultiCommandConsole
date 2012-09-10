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

namespace Tests.Mono.Options
{
	static class Utils
	{
		public static void AssertArgumentOutOfRangeException<T>(string argName, T a, Action<T> action)
		{
			AssertException(typeof(ArgumentOutOfRangeException), string.Format("Specified argument was out of the range of valid values.{0}Parameter name: {1}", Environment.NewLine, argName), a, action);
		}
		public static void AssertArgumentNullException<T>(string argName, T a, Action<T> action)
		{
			AssertException(typeof(ArgumentNullException), string.Format("Value cannot be null.{0}Parameter name: {1}", Environment.NewLine, argName), a, action);
		}

		public static void AssertException<T>(Type exceptionType, string message, T a, Action<T> action)
		{
			try
			{
				action(a);
			}
			catch (Exception e)
			{
				e.GetType().Should().Be(exceptionType);
				e.Message.Should().Be(message);
			}
		}

	}
}

