using System;
using System.Configuration;
using RabbitMQ.Client;

namespace Services.Queue
{
    public class QueueFactory : IQueueFactory
    {
        public IModel Channel;
        public IConnection Connection;

        private ConnectionFactory _rabittMqConnectionFactory;

        #region IQueueFactory Members

        public void CreateInstanceOfRabittMqChannel()
        {
            try
            {
                _rabittMqConnectionFactory = new ConnectionFactory
                                                 {
                                                     HostName = ConfigurationManager.AppSettings.Get("QueueHostName"),
                                                     Port =
                                                         Convert.ToInt32(
                                                             ConfigurationManager.AppSettings.Get("QueuePort")),
                                                     UserName = ConfigurationManager.AppSettings.Get("QueueUserName"),
                                                     Password = ConfigurationManager.AppSettings.Get("QueuePassword"),
                                                     VirtualHost =
                                                         ConfigurationManager.AppSettings.Get("QueueVirtualHost"),
                                                     Protocol = RabbitMQ.Client.Protocols.AMQP_0_8

                                                 };
                Connection = _rabittMqConnectionFactory.CreateConnection();
                if (Connection != null)
                {
                    Channel = Connection.CreateModel();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred connecting to the queue: " + ex);
            }
        }

        #endregion

        public void LogConnClose(ShutdownEventArgs reason)
        {
            Console.Error.WriteLine("Closing connection " + Connection + " with reason " + reason);
        }

        public void Dispose()
        {
            if (Connection != null)
                Connection.Close();
            if (Channel != null)
                Channel.Abort();
        }
    }
}