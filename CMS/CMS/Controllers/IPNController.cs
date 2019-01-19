using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Net;

namespace CMS.Controllers
{
    public class IPNController : Controller
    {
        private class IPNContext
        {
            public HttpRequest IPNRequest { get; set; }

            public string RequestBody { get; set; }

            public string Verification { get; set; } = String.Empty;
        }

        [HttpPost]
        public IActionResult Receive()
        {
            IPNContext ipnContext = new IPNContext()
            {
                IPNRequest = Request
            };

            using (StreamReader reader = new StreamReader(ipnContext.IPNRequest.Body, Encoding.ASCII))
            {
                ipnContext.RequestBody = reader.ReadToEnd();
            }

            //Store the IPN received from PayPal
            LogRequest(ipnContext);

            //Fire and forget verification task
            Task.Run(() => VerifyTask(ipnContext));

            //Reply back a 200 code
            return Ok();
        }

        private void VerifyTask(IPNContext ipnContext)
        {
            try
            {
                var verificationRequest = WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr");

                //Set values for the verification request
                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";

                //Add cmd=_notify-validate to the payload
                string strRequest = "cmd=_notify-validate&" + ipnContext.RequestBody;
                verificationRequest.ContentLength = strRequest.Length;

                //Attach payload to the verification request
                using (StreamWriter writer = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII))
                {
                    writer.Write(strRequest);
                }

                //Send the request to PayPal and get the response
                using (StreamReader reader = new StreamReader(verificationRequest.GetResponse().GetResponseStream()))
                {
                    ipnContext.Verification = reader.ReadToEnd();
                }
            }
            catch (Exception exception)
            {
                //Capture exception for manual investigation
            }

            ProcessVerificationResponse(ipnContext);
        }


        private void LogRequest(IPNContext ipnContext)
        {
            // Persist the request values into a database or temporary data store
        }

        private void ProcessVerificationResponse(IPNContext ipnContext)
        {
            if (ipnContext.Verification.Equals("VERIFIED"))
            {
                // check that Payment_status=Completed
                // check that Txn_id has not been previously processed
                // check that Receiver_email is your Primary PayPal email
                // check that Payment_amount/Payment_currency are correct
                // process payment


            }
            else if (ipnContext.Verification.Equals("INVALID"))
            {
                //Log for manual investigation
            }
            else
            {
                //Log error
            }
        }

        protected void IpnTest()
        {
            string ipn = "mc_gross=19.00&protection_eligibility=Eligible&address_status=unconfirmed&payer_id=ZFZ82G4TVHDR6&tax=0.00&address_street=Flat no. 507 Wing A Raheja Residency Film City Road, Goregaon East&payment_date = 03:56:05 Jan 08, 2016 PST & payment_status = Completed & charset = windows - 1252 & address_zip = 400097 & first_name = test & mc_fee = 1.04 & address_country_code = IN & address_name = test buyer & notify_version = 3.8 & custom = 4,3 & payer_status = verified & business = yogeshcs2003@gmail.com & address_country = India & address_city = Mumbai & quantity = 1 & verify_sign = A21kmBLyRcQpDSjYvmswlm3ckjahAsKHT1mADo3BTr8z77LT1FFrQHfw & payer_email = yogeshcs2003 - buyer@gmail.com & txn_id = 8R896852UL426531A & payment_type = instant & last_name = buyer & address_state = Maharashtra & receiver_email = yogeshcs2003@gmail.com & payment_fee = 1.04 & receiver_id = TTWAPEJ224V3S & txn_type = web_accept & item_name = Bronze & mc_currency = USD & item_number = &residence_country = IN & test_ipn = 1 & handling_amount = 0.00 & transaction_subject = 4,3 & payment_gross = 19.00 & shipping = 0.00 & ipn_track_id = 99c888eb37c1b";

            int partnerId = Convert.ToInt32(GetValueFromString(ipn, "custom").Split(',')[0]);
            int membershipId = Convert.ToInt32(GetValueFromString(ipn, "custom").Split(',')[1]);
        }

        protected string GetValueFromString(string FullString, string ValueToGet)
        {
            int valueToGetIndex, equalSignIndex, ampersandSignIndex;
            valueToGetIndex = FullString.IndexOf(ValueToGet);
            equalSignIndex = FullString.IndexOf("=", valueToGetIndex);
            ampersandSignIndex = FullString.IndexOf("&", equalSignIndex);
            string returnValue = FullString.Substring(equalSignIndex + 1, ampersandSignIndex - equalSignIndex - 1);
            return returnValue;
        }
    }
}