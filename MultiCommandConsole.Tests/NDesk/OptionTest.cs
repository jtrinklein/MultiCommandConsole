//
// OptionTest.cs
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

using NUnit.Framework;

namespace Tests.Mono.Options
{
	[TestFixture]
	public class OptionTest
	{
		[Test]
		public void Exceptions()
		{
			Utils.AssertArgumentNullException("prototype", () => new DefaultOption(null, null));
			Utils.AssertArgumentException("prototype", "Cannot be the empty string.", () => new DefaultOption("", null));
			Utils.AssertArgumentException("prototype", "Empty option names are not supported.", () => new DefaultOption("a|b||c=", null));
			Utils.AssertArgumentException("prototype", "Conflicting option types: '=' vs. ':'.", () => new DefaultOption("a=|b:", null));

			Utils.AssertArgumentException("prototype", "The default option handler '<>' cannot require values.", () => new DefaultOption("<>=", null));
			Utils.AssertArgumentException("prototype", "The default option handler '<>' cannot require values.", () => new DefaultOption("<>:", null));
			new DefaultOption("t|<>=", null, 1); // should work
			Utils.AssertArgumentException("prototype", "The default option handler '<>' cannot require values.", () => new DefaultOption("t|<>=", null, 2));

			new DefaultOption("a|b=", null, 2); // should work

			Utils.AssertArgumentOutOfRangeException("maxValueCount", () => new DefaultOption("a", null, -1));
			Utils.AssertArgumentException("maxValueCount",
			                              "Cannot provide maxValueCount of 0 for OptionValueType.Required or OptionValueType.Optional.", 
			                              () => new DefaultOption("a=", null, 0));

			Utils.AssertArgumentException("prototype", "Ill-formed name/value separator found in \"a={\".", () => new DefaultOption("a={", null));
			Utils.AssertArgumentException("prototype", "Ill-formed name/value separator found in \"a=}\".", () => new DefaultOption("a=}", null));
			Utils.AssertArgumentException("prototype", "Ill-formed name/value separator found in \"a={{}}\".", () => new DefaultOption("a={{}}", null));
			Utils.AssertArgumentException("prototype", "Ill-formed name/value separator found in \"a={}}\".", () => new DefaultOption("a={}}", null));
			Utils.AssertArgumentException("prototype", "Ill-formed name/value separator found in \"a={}{\".", () => new DefaultOption("a={}{", null));

			Utils.AssertArgumentException("prototype", "Cannot provide key/value separators for Options taking 1 value(s).", () => new DefaultOption("a==", null));
			Utils.AssertArgumentException("prototype", "Cannot provide key/value separators for Options taking 1 value(s).", () => new DefaultOption("a={}", null));
			Utils.AssertArgumentException("prototype", "Cannot provide key/value separators for Options taking 1 value(s).", () => new DefaultOption("a=+-*/", null));
			
			new DefaultOption("a", null, 0); // should work

			var emptySeparators = new string[0];
			new DefaultOption("a", null).ShouldHaveOnlyTheseSeparators(emptySeparators);
			new DefaultOption("a=", null).ShouldHaveOnlyTheseSeparators(emptySeparators);
			new DefaultOption("a=", null, 2).ShouldHaveOnlyTheseSeparators(new[] {":", "="});
			new DefaultOption("a={}", null, 2).ShouldHaveOnlyTheseSeparators(emptySeparators);
			new DefaultOption("a={-->}{=>}", null, 2).ShouldHaveOnlyTheseSeparators(new[] {"-->", "=>"});
			new DefaultOption("a=+-*/", null, 2).ShouldHaveOnlyTheseSeparators(new[] {"+", "-", "*", "/"});
		}
	}
}


