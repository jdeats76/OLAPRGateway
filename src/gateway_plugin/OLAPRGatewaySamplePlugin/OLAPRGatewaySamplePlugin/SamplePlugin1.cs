/* 
*************************************************************************************************
OLAPRGatewaySamplePlugin by Jeremy Deats, Copyright(c) 2015
*************************************************************************************************
This source code is "free" software. By using this software you agree to assume all liability for events that may occur as a result of its use.
You may use. modify and redistribute this source code according to the terms of the GNU General Public License
which can be found here http://www.gnu.org/licenses/gpl.html

*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using OLAPRGatewayExt;

namespace OLAPRGatewaySamplePlugin
{
    public class SimplePlugin1 : OLAPRGatewayPlugin
    {
        public override DataTable ReturnDataTable(string param)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ID"));
            dt.Columns.Add(new DataColumn("Name"));
            dt.Columns.Add(new DataColumn("Age"));

            DataRow r = dt.NewRow();
            r[0] = "100";
            r[1] = "John";
            r[2] = "23";

            dt.Rows.Add(r);

            DataRow r2 = dt.NewRow();
            r2[0] = "200";
            r2[1] = "Sarah";
            r2[2] = "18";

            dt.Rows.Add(r2);

            DataRow r3 = dt.NewRow();
            r3[0] = "300";
            r3[1] = "Jackie";
            r3[2] = "43";

            dt.Rows.Add(r3);

            return dt;
        }
    }
}
