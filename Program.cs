using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace Pythonconvert
{
    class Program
    {
        public void SelectData()
        {
            SqlConnection cn = new SqlConnection(@"Data Source=10.61.18.12;Initial Catalog=AR_Tool_NEW;User ID=msdba;Password=dba@123;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            SqlCommand cmd = new SqlCommand("Select C.CustomerNo, A.InvoiceDate, D.PaymentDate,DATEDIFF(day,A.InvoiceDate, getdate()) as 'OverDueDays', Case when E.CustomerNo Case when E.CustomerNo = C.CustomerNo then 1 else 0 end as 'IsExist' then 1 else 0 end as 'IsExist' from invoice A join Customer B on B.customerNo = A.CustomerNo Join ParentCustomerInfo C on C.DunsNo = B.DunsNo Left join Payment D on D.InvoiceNo = A.invoiceno LEFT join Predicted_Data E on E.CustomerNo = C.CustomerNo", cn);
            //SqlCommand cmd = new SqlCommand("Pythonselectcustomer", cn);

            cn.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                // Print employee details 
                Console.WriteLine("CustomerNo\tInvoiceDate\tPaymentDate\tOverDueDays\tIsExist\t");
                Console.WriteLine(rdr[0] + "  " + rdr[1] + "  " + rdr[2] + "  " + rdr[3] + "  " + rdr[4]);
            }

            rdr.Close();
            cn.Close();

        }


        public List<Customer> GetCustomer()
        {
            SqlConnection sc = new SqlConnection("Data Source=10.61.18.12;Initial Catalog=AR_Tool_NEW;User ID=msdba;Password=dba@123;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            List<Customer> s2 = new List<Customer>();
            SqlDataAdapter sa = new SqlDataAdapter("Pythonselectcustomer", sc);
            DataTable dt = new DataTable();
            sa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sc.Open();
            sa.Fill(dt);
            sc.Close();
            foreach (DataRow dr in dt.Rows)
            {
                s2.Add(
                    new Customer
                    {
                        CustomerNo = Convert.ToString(dr["CustomerNo"]),
                        InvoiceDate = Convert.ToString(dr["InvoiceDate"]),
                        PaymentDate = Convert.ToString(dr["PaymentDate"]),
                        OverDueDays = Convert.ToInt32(dr["OverDueDays"]),
                        IsExist = Convert.ToInt32(dr["IsExist"]),
                    }
                    );
            }

            //foreach (var cust in s2)
            //{
            //    Console.WriteLine("CustomerNo: {0}" , cust.CustomerNo + "\tInvoiceDate:" + cust.InvoiceDate + "\tPaymentDate:" + cust.PaymentDate + "\tOverDueDays:" + cust.OverDueDays + "\tIsExist:" + cust.IsExist);
            //}

            var groupedResult = (from c in s2
                                 group c by c.CustomerNo into x
                                 select new { customerNo = x.Key, predictDays = x.Average(y => y.OverDueDays) }).ToList();

            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = sc;
            sqlCommand.CommandType = CommandType.Text;
            sc.Open();


            foreach (var result in groupedResult)
            {
                Console.WriteLine("{0}    {1}", result.customerNo, result.predictDays.ToString());

                foreach (var sat in s2)
                {
                    if (sat.IsExist == 0)
                    {
                        var test = "INSERT into Predicted_Data(CustomerNo, PredictedDays) values('" + result.customerNo + "'," + result.predictDays + ") ";
                        sqlCommand.CommandText = test;
                    }
                    else
                    {
                        var test = "UPDATE Predicted_Data SET PredictedDays = " + result.predictDays + " where CustomerNo = '" + result.customerNo + "' ";
                        sqlCommand.CommandText = test;
                    }
                }


                int output = sqlCommand.ExecuteNonQuery();
                Console.WriteLine(output);
            }


            //List<CustomerwiseOverdue> lstCustomerwiseOverdue = new List<CustomerwiseOverdue>();
            //CustomerwiseOverdue customerwiseOverdue = null;
            //foreach (var custGroup in groupedResult)
            //{
            //    customerwiseOverdue = new CustomerwiseOverdue();
            //    Console.WriteLine("CustomerNo: {0}", custGroup.Key); //Each group has a key 
            //    int count=0;
            //    int Totaloverduedays = 0;
            //    foreach (Customer s in custGroup) // Each group has inner collection
            //    {
            //        Console.WriteLine("OverDueDays: {0}", s.OverDueDays);
            //        count++;
            //        Totaloverduedays += s.OverDueDays; 
            //    }

            //    //Console.WriteLine("Count {0}", count);
            //    Console.WriteLine("Total of overduedays {0}", Totaloverduedays);
            //    float avg;
            //    avg = Totaloverduedays / count;
            //    Console.WriteLine("Mean {0}",avg );

            //    customerwiseOverdue.CustomerNo = custGroup.Key;
            //    customerwiseOverdue.PrdicationOverdueDays = custGroup.

            //    lstCustomerwiseOverdue.Add()

            //}


            return s2;

        }


        static void Main(string[] args)
        {
            Program p = new Program();
            //p.SelectData();
            //p.getgroupby();
            p.GetCustomer();
            //p.Addcust();
            Console.ReadLine();
        }

    }


    public class CustomerwiseOverdue
    {
        public string CustomerNo { get; set; }
        public int PrdicationOverdueDays { get; set; }
    }
}



//public string UploadInvoiceFile(int userid, DataTable table, DatabaseInfo tenant)
//{
//    try
//    {
//        using (var context = new TenantDbContext(tenant))
//        using (var command = context.Database.GetDbConnection().CreateCommand())
//        {
//            command.CommandText = "ImportExcelDataToImportInvoice";
//            command.CommandType = CommandType.StoredProcedure;
//            command.Parameters.Add(new SqlParameter("@tblInvoice", table));
//            command.Parameters.Add(new SqlParameter("@UserId", userid));
//            context.Database.OpenConnection();
//            DbDataAdapter da = APIHelperMethods.CreateDataAdapter(command);
//            DataSet result = new DataSet();
//            da.Fill(result);
//            string JSONString = string.Empty;
//            JSONString = JsonConvert.SerializeObject(result);
//            return JSONString;
//        }
//    }
//    catch (Exception ex)
//    {
//        _logger.LogError(ex.Message);
//        return "FAIL";
//    }
//}


//public string UploadFile(int UserId = 0)
//{
//    var httpRequest = HttpContext.Request;
//    string JSONString = string.Empty;
//    try
//    {
//        if (httpRequest.Form.Files.Count > 0)
//        {
//            var filePath = "";

//            foreach (var file in httpRequest.Form.Files)
//            {
//                if (file.Length > 0)
//                {
//                    var path = Path.GetTempFileName();
//                    var myfile = System.IO.File.Create(path);
//                    FileStream fs = myfile;
//                    file.CopyTo(fs);
//                    fs.Flush();
//                    byte[] array = new byte[fs.Length];
//                    fs.Seek(0, SeekOrigin.Begin);
//                    fs.Read(array, 0, array.Length);
//                    myfile.Close();
//                    System.IO.File.WriteAllBytes(path, array);
//                    filePath = path;
//                    FileInfo fileInfo = new FileInfo(filePath);
//                    string pathToExcelFile = filePath;

//                    //var connectionString = "";

//                    //if (file.FileName.EndsWith(".xls"))
//                    //{
//                    //    connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
//                    //}
//                    //else 
//                    if (file.FileName.EndsWith(".xlsx"))
//                    {
//                        ExcelPackage p = new ExcelPackage(fileInfo);
//                        ExcelWorksheet myWorksheet = p.Workbook.Worksheets["Sheet1"];
//                        DataTable table = new DataTable();
//                        //22 is number od Column to select from EXCEL
//                        foreach (var firstRowCell in myWorksheet.Cells[1, 1, 1, 22])
//                        {
//                            table.Columns.Add(firstRowCell.Text);
//                        }

//                        for (var rowNumber = 2; rowNumber <= myWorksheet.Dimension.End.Row; rowNumber++)
//                        {
//                            var row = myWorksheet.Cells[rowNumber, 1, rowNumber, 22];
//                            var newRow = table.NewRow();
//                            foreach (var cell in row)
//                            {
//                                newRow[cell.Start.Column - 1] = cell.Text;
//                            }
//                            table.Rows.Add(newRow);
//                        }

//                        DataTable tbl = ADOExtensions.RemoveEmptyRowsFromDataTable(table);

//                        if (IsTenantExist)
//                            return _fileUpload.UploadFile(UserId, tbl, string.Empty, tid);
//                        return string.Empty;
//                    }
//                }
//            }
//        }
//    }