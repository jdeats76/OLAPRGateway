/* 
*************************************************************************************************
OLAPRGateway by Jeremy Deats, Copyright(c) 2015
*************************************************************************************************
This source code is "free" software. By using this software you agree to assume all liability for events that may occur as a result of its use.
You may use. modify and redistribute this source code according to the terms of the GNU General Public License
which can be found here http://www.gnu.org/licenses/gpl.html

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace OLAPRGatewayExt
{
    // provides a common abstract base class that can be used by plug-in developers and invoked by OLAPRGateway.exe
    public abstract class OLAPRGatewayPlugin
    {
        public abstract DataTable ReturnDataTable(string param);
    
    }
}
