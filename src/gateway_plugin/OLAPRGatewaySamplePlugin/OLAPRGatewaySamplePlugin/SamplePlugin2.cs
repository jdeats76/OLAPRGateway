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
using System.Linq;
using System.Text;
using OLAPRGatewayExt;
using System.Data;

namespace OLAPRGatewaySamplePlugin
{
    public class SimplePlugin2 : OLAPRGatewayPlugin
    {
        public override DataTable ReturnDataTable(string param)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ID"));
            dt.Columns.Add(new DataColumn("State"));
            dt.Columns.Add(new DataColumn("Population"));

            DataRow r = dt.NewRow();
            r[0] = "1";
            r[1] = "NY";
            r[2] = "8002";

            dt.Rows.Add(r);

            DataRow r2 = dt.NewRow();
            r2[0] = "2";
            r2[1] = "CO";
            r2[2] = "6003";

            dt.Rows.Add(r2);

            DataRow r3 = dt.NewRow();
            r3[0] = "3";
            r3[1] = "CA";
            r3[2] = "7200";

            dt.Rows.Add(r3);

            return dt;
        }
    }
}
