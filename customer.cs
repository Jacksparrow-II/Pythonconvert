using System;
using System.Collections.Generic;
using System.Text;

namespace Pythonconvert
{
    public class Customer
    {
        public string CustomerNo { get; set; }
        public string InvoiceDate { get; set; }
        public string PaymentDate { get; set; }
        public int OverDueDays { get; set; }
        public int IsExist { get; set; }

        public int PredictedDays { get; set; }
        
    }
}


