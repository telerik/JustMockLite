﻿/*
 JustMock Lite
 Copyright © 2023 Progress Software Corporation

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

namespace Telerik.JustMock.Core.Internal
{
    using System.Threading;

    internal class MonitorLockHolder : ILockHolder
    {
        private readonly object locker;
        private bool lockAcquired;

        public MonitorLockHolder(object locker, bool waitForLock)
        {
            this.locker = locker;
            if(waitForLock)
            {
                Monitor.Enter(locker);
                lockAcquired = true;
                return;
            }

            lockAcquired = Monitor.TryEnter(locker, 0);
        }

        public void Dispose()
        {
            if (!LockAcquired) return;
            Monitor.Exit(locker);
            lockAcquired = false;
        }

        public bool LockAcquired
        {
            get { return lockAcquired; }
        }
    }
}
