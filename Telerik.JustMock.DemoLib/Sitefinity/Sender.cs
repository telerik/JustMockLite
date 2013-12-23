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
using System.Net;
using System.Net.Mail;

namespace Telerik.Sitefinity.Modules.Newsletters.Communication
{
    /// <summary>
    /// This class provides functionality for sending the messages through Newsletter module.
    /// </summary>
    public class Sender : IDisposable
    {
        #region Public methods

        /// <summary>
        /// Sends a mail message.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(MailMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var smtpClient = this.GetSmtpClient();
            smtpClient.Send(message);
        }

        /// <summary>
        /// Gets the configured instance of the <see cref="SmtpClient"/>.
        /// </summary>
        /// <returns>An instance of the <see cref="SmtpClient"/>.</returns>
        public SmtpClient GetSmtpClient()
        {
            return new SmtpClient("localhost", 25);
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.smtpClient != null)
                {
                    (this.smtpClient as IDisposable).Dispose();
                }
                this.smtpClient = null;
            }
        }

        #endregion

        #region Private fields and constants

        private SmtpClient smtpClient;

        #endregion
    }
}
