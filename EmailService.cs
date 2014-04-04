using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

namespace Pet360.Service.Email
{
    public class EmailService
    {
        private string _endpoint;
        private HttpClient _client;
        private HttpContent _content;
        private Task<HttpResponseMessage> _response;
        private string _body;

        public Task<HttpResponseMessage> AsyncSendEmail(string endPoint, string body)
        {
            try
            {
                _endpoint = endPoint;
                _client = new HttpClient();
                _body = body;
                _content = new StringContent(_body);
                _response = _client.PostAsync(_endpoint, _content);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred sending email to endpoint " + _endpoint + Environment.NewLine + ex);
            }

            return _response;
        }

        public void SyncSendEmail(string endPoint, string body)
        {
            bool result = true;
            WebRequest webRequest = WebRequest.Create(endPoint);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";

            byte[] bytes = Encoding.UTF8.GetBytes(body.ToString());
            Stream _stream = null;

            try
            { // send the Post
                webRequest.ContentLength = bytes.Length;   //Count bytes to send
                _stream = webRequest.GetRequestStream();
                _stream.Write(bytes, 0, bytes.Length);     //Post method
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                if (_stream != null) _stream.Close();
            }
        }
    }
}
