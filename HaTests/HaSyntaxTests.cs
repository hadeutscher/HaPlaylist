/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using HaSyntaxLib;

namespace HaTests
{
    [TestClass]
    public class HaSyntaxTests
    {
        [TestMethod]
        public void Test()
        {
            Assert.IsTrue(new HaSyntax("a or b").Validate(new string[] { "a" }) == true);
            Assert.IsTrue(new HaSyntax("a or b").Validate(new string[] { "c" }) == false);
            Assert.IsTrue(new HaSyntax("a or b || !c").Validate(new string[] { "c" }) == false);
            Assert.IsTrue(new HaSyntax("a or b || !c").Validate(new string[] { "A", "c" }) == true);
            Assert.IsTrue(new HaSyntax("a or b || not !c").Validate(new string[] { "c" }) == true);
            Assert.IsTrue(new HaSyntax("a b not c || d e f").Validate(new string[] { "d", "e", "a" }) == false);
            Assert.IsTrue(new HaSyntax("a b not c || d e f").Validate(new string[] { "d", "e", "a", "b", "c" }) == false);
            Assert.IsTrue(new HaSyntax("a b not c || d e f").Validate(new string[] { "d", "e", "f", "a", "b", "c" }) == true);
            Assert.IsTrue(new HaSyntax("a b not c || d e f").Validate(new string[] { "d", "e", "a", "b" }) == true);
            Assert.IsTrue(new HaSyntax("a b not c d || d e f").Validate(new string[] { "a", "b" }) == false);
            Assert.IsTrue(new HaSyntax("a b not c d || d e f").Validate(new string[] { "a", "b", "d" }) == true);
        }
    }
}
