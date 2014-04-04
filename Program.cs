using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Queue;
using Pet360.Service.Email;

namespace BHWorker
{
    public class Program
    {
        private static Consumer _consumer;
        private static string _queueName = ConfigurationManager.AppSettings.Get("BHQueueName");
        private static string _endpoint = ConfigurationManager.AppSettings.Get("BHEndpoint");
        private delegate void _consumeDelegate();
        private static EmailService _sendEmail;
        private static int _counter;

        static void Main(string[] args)
        {
            StartConsuming();
        }

        private static void StartConsuming()
        {
            try
            {
                _counter = 0;
                Console.WriteLine("NOW READING FROM QUEUE" + Environment.NewLine);
                _consumer = new Consumer(_queueName + ".queue");

                var _consumeDelegate = new _consumeDelegate(_consumer.Consume);
                _consumeDelegate.BeginInvoke(null, null);
                _consumer.onMessageReceived += HandleMessage;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occcurred with running the BH worker application: " + ex);
            }
        }

        private static void HandleMessage(byte[] body)
        {
            _counter++;
            var _body = Encoding.UTF8.GetString(body);
            _sendEmail = new EmailService();
            var response = _sendEmail.AsyncSendEmail(_endpoint, _body);
            //_sendEmail.SyncSendEmail(_endpoint, _body);

            Console.WriteLine("***********************************************");
            Console.WriteLine("MESSAGE #" + _counter + " FROM QUEUE:" + Environment.NewLine);
            Console.WriteLine(_body + Environment.NewLine);
            Console.WriteLine("SENT EMAIL RESPONSE CODE : " + response.Result + Environment.NewLine);
            Console.WriteLine("***********************************************");
        }
    }
}
