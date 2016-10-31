/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Runtime.Serialization;

namespace HaTagLib
{
    [Serializable]
    public class HaException : Exception
    {
        public HaException()
        {
        }

        public HaException(string message) : base(message)
        {
        }

        public HaException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
