using System;
using System.Collections.Generic;
using System.Configuration;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace Services.Queue
{
    public class Consumer
    {
        #region Delegates

        public delegate void onReceiveMessage(byte[] message);

        #endregion

        private readonly string _queueName;

        public QueueFactory Factory;
        private List<string> _queueData;
        private Subscription _subscription;
        private bool isConsuming;
        // used to pass messages back to client

        public Consumer(string queueName)
        {
            _queueName = queueName;
            _queueData = new List<string>();

            Factory = new QueueFactory();
            Factory.CreateInstanceOfRabittMqChannel();
        }

        public event onReceiveMessage onMessageReceived;

        public void Consume()
        {
            isConsuming = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("BHIsConsuming"));

            try
            {
                if (Factory.Channel != null)
                {
                    Factory.Channel.BasicQos(0, 1, false);

                    //create new subscription
                    _subscription = new Subscription(Factory.Channel, _queueName, false);

                    while (isConsuming)
                    {
                        if (_subscription != null)
                        {
                            BasicDeliverEventArgs e = _subscription.Next();
                            if (e.Body != null)
                            {
                                onMessageReceived(e.Body);
                                _subscription.Ack(e);
                            }
                        }
                    }

                    Factory.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred consuming email messages from the queue : " + ex);
            }
        }
    }
}