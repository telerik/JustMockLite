/*
 JustMock Lite
 Copyright © 2010-2014 Telerik AD

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustMock.DemoLib.Objects;

namespace Telerik.JustMock.DemoLib
{
    public class TransactionService
    {
        public static void RecalculateTicket(TransactionHeaderViewModel model)
        {

        }

        public static void SaveTransaction(TransactionHeaderViewModel ticket, bool recalculate)
        {
            if (UserService.GetLoggedInUser() == null)
            {
                throw new ArgumentException();
            }

            if (ConfigurationService.GetConfiguration() == null)
            {
                throw new InvalidOperationException("no valid configuration");
            }

            ticket.ReservationNumber = GetReservationNumber(UserService.Username);
            //ticket.OperatorId = Guid.NewGuid().ToString();
            ticket.SetLineNumbers();
            ticket.SetPaidAmount();
            ticket.SaveTicket();
        }

        public static int GetReservationNumber(string s)
        {
            return 0;
        }
    }
}
