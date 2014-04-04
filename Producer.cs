using System;
using System.Text;
using RabbitMQ.Client;

namespace Services.Queue
{
    public class Producer : IProducer
    {
        public QueueFactory Factory;
        private string _exchangeName;
        private byte[] _message;
        private string _queueName;

        public Producer()
        {
            Factory = new QueueFactory();

            if (Factory != null)
            {
                Factory.CreateInstanceOfRabittMqChannel();
            }
        }

        #region IProducer Members

        public bool SendMessage(string message, string queue)
        {
            bool retVal = false;
            try
            {
                if (message == null) return retVal;
                _queueName = queue + ".queue";
                _exchangeName = queue + ".exchange";
                _message = Encoding.UTF8.GetBytes(message);

                if (Factory != null)
                {
                    Factory.Channel.QueueBind(_queueName, _exchangeName, _queueName);

                    byte[] messageBodyBytes = _message;
                    IBasicProperties props = Factory.Channel.CreateBasicProperties();
                    props.ContentType = "text/plain";
                    props.DeliveryMode = 2;
                    Factory.Channel.BasicPublish(_exchangeName, _queueName, props, messageBodyBytes);

                    Factory.Dispose();

                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured: " + ex);
            }
            return retVal;
        }

        #endregion
    }
}