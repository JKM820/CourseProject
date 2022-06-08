using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPConnectionAPI_C_sharp_
{
    static public class ReportCreator
    {
        public static string CreateReportAboutVehicles()
        {
            using (IDataViewPermision db = new DatabaseContext())
            {
                var buf = db.FindDetailsWhere(c => c != null);
                string str = ("Отчет состояния деталей на " + DateTime.Now.ToString() + '\n');
                foreach (var item in buf)
                {
                    str += '\n';
                    str += "Название: " + item.Name + '\n';
                    str += "Артикул: " + item.VendorCode + '\n';
                    str += "Цена: " + item.Cost + ".руб" + '\n';
                }
                str += "\n\n\n";
                str += ("Отчет состояния кредитных организаций на " + DateTime.Now.ToString() + '\n');
                var creditors = db.FindCreditorWhere(c => c != null);
                foreach (var item in creditors)
                {
                    str += '\n';
                    str += "Название организации: " + item.Name + '\n';
                    str += "УНП организации: " + item.UNP + '\n';
                    str += "Потенциальная сумма займа: " + item.SumToLoan + ".руб" + '\n';
                }

                return str;
            }
        }
    }
}
