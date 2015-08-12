/* 
*************************************************************************************************
OLAPRGateway by Jeremy Deats, Copyright(c) 2015
*************************************************************************************************
This source code is "free" software. By using this software you agree to assume all liability for events that may occur as a result of its use.
You may use. modify and redistribute this source code according to the terms of the GNU General Public License
which can be found here http://www.gnu.org/licenses/gpl.html

*/
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AnalysisServices.AdomdClient;
using log4net;
using log4net.Config;

namespace OLAPRGateway
{
    public class Program
    {
        // static instance of logger
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static int _port = -1;
        private static string _connStr = "";
        private static string _token = "";
        private static string _mode = "";
        private static string _host = "localhost";
        private static bool _logdataset = false;
        private static List<string> _plugInList = new List<string>();

        // validate that string can be converted to numeric value
        static bool IsNum(string str)
        {
            bool result = false;
            int i = 0;
            if (int.TryParse(str, out i))
            {
                result = true;
            }
            return result;
        }

        // add escape characters to make string R complaint.
        static string RStrEscape(string str)
        {
            str = str.Replace("\\", "\\\\");   // convert \ to \\
            str = str.Replace("\"", "\\\"");   // convert " to \"

            return str;
        }


        // If string is not numeric add leading and trailing double quotes
        static string Coat(string str)
        {
            StringBuilder sb = new StringBuilder();
            if (!(IsNum(str)))
            {
                sb.Append("\"");
                sb.Append(RStrEscape(str));
                sb.Append("\"");

            }
            else
            {
                sb.Append(str);
            }

            return sb.ToString();
        }

        // remove malformed formatting from columnName
        static string CleanColumnName(string columnName)
        {
            columnName = columnName.Replace("[", "");
            columnName = columnName.Replace("]", "");
            columnName = columnName.Replace(" ", "");
            columnName = columnName.Replace(".", "_");
            columnName = columnName.Replace("&", "");
            return columnName;
        }

        // iterates through an ado.net DataTable object then builds and returns an R language compliant dataframe script 
        static string DataTableToRDataFrame(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            List<string> columnNames = new List<string>();
            foreach (DataColumn c in dt.Columns)
            {
                sb.Append(CleanColumnName(c.ColumnName.ToString()));
                columnNames.Add(CleanColumnName(c.ColumnName));
                sb.Append(" <- c(");

                //string sparts = "";
                StringBuilder sparts = new StringBuilder();
                foreach (DataRow r in dt.Rows)
                {
                    sparts.Append(Coat(r[c].ToString()));
                    sparts.Append(",");
                }

                string strSparts = sparts.ToString().Substring(0, sparts.Length - 1); // remove trailing comma
                sb.Append(strSparts);
                sb.Append(");");
            }

            sb.Append("sqltemp <- data.frame(");
            StringBuilder cnames = new StringBuilder();
            foreach (string c in columnNames)
            {
                cnames.Append(c);
                cnames.Append(",");
            }
            String strCnames = cnames.ToString().Substring(0, cnames.Length - 1);
            sb.Append(strCnames);
            sb.Append(")\r\n");

            return sb.ToString();
        }


        // validate token prefix from request
        static bool ValidateToken(ref string str)
        {
            bool validated = false;
            // confirm token was received
            if (str.Contains(_token))            
            {
                // remove token from string
                str = str.Replace(_token, "");    
                validated = true;
            }

            return validated;
        }

        

        // execute SSAS/OLAP command and return a R language compliant dataframe script representing the result data.
        static string SSASConnectAndExec(string mdx)
        {
            AdomdConnection conn = new AdomdConnection(_connStr);
            //  string mdx = "SELECT {[Measures].[Page Views],[Measures].[Daily Unique Visitors]} ON 0,{[Dim Date].[Month].[Month]} ON ROWS FROM [Hearst POC];";
            AdomdCommand cmd = new AdomdCommand(mdx, conn);
            AdomdDataAdapter da = new AdomdDataAdapter(cmd);
            DataTable dt = new DataTable();

            try
            {
                conn.Open();
            }
            catch (AdomdException e)
            {
                Console.WriteLine(">> Failed to open SSAS Connection");
                log.Error("Failed to open SSAS connection for " + _connStr, e);
                return null;
            }

            try
            {
                da.Fill(dt);
            }
            catch (AdomdException e)
            {
                Console.WriteLine(">> Failed to fill the datatable");
                log.Error("Failed to fill datatable for query " + mdx, e);
                return null;
            }

            // Clean up ado.net objects
            da.Dispose();
            conn.Close();

            // process datatable and create R language compliant dataframe object as script
            return DataTableToRDataFrame(dt);

        }

        static string PlugInExec(string param)
        {
            string lib = "";
            string method = "";
            string className = "";
            DataTable refResult = null;
         
            // Expected format param = "Lib=MyDLL.DLL;Class=Namespace.Class;Param=Data to pass to method input string here" 
            string[] plist = param.Split(new char[] { ';' });
           
            lib = plist[0].Split(new char[] { '=' })[1];
            className = plist[1].Split(new char[] { '=' })[1];
            param = plist[2].Split(new char[] { '=' })[1];

            foreach (string dll in _plugInList)
            {
                if (dll.Contains(lib))
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(dll);
                        Type type = assembly.GetType(className);
                        if (type != null)
                        {
                            MethodInfo methodInfo = type.GetMethod("ReturnDataTable");
                            if (methodInfo != null)
                            {
                                ParameterInfo[] parameters = methodInfo.GetParameters();
                                object classInstance = Activator.CreateInstance(type, null);
                                refResult = (DataTable)methodInfo.Invoke(classInstance, new object[] { param });
                                return DataTableToRDataFrame(refResult);
                            }
                        }
                    } catch (Exception e)
                    {
                        log.Error("Failed to invoke class " + className + " in " + lib, e);
                    }
                    break;
                }

            }
            log.Error("Failed to find library " + lib);
            throw new Exception("Failed to invoke class " + className);

            return null;
        }


        // execute SQLServer command and return a R language compliant dataframe script representing the result data.
        static string SQLConnectAndExec(string sql)
        {

            // connect to SQL Server and execute query. 
            StringBuilder sb = new StringBuilder();
            SqlConnection conn = new SqlConnection(_connStr);
            try
            {
                conn.Open();
            }
            catch (SqlException e) {
                Console.WriteLine(">> Failed to open SQL Connection");
                log.Error("Failed to open SQL connection for " + _connStr, e);
                return null; 
            }

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();

            // store query results in DataTable
            try
            {
                da.Fill(dt);
            } catch (SqlException e)
            {
                Console.WriteLine(">> Failed to fill the datatable");
                log.Error("Failed to fill datatable for query " + sql, e);
                return null;
            }

            // Clean up ado.net objects
            da.Dispose();
            conn.Close();

            // process datatable and create R language compliant dataframe object as script
            return DataTableToRDataFrame(dt);
        }

        // Encode string as ASCII Byte array
        static Byte[] E(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        // Cleans/removes ASCII null characters "\0" from string
        static string C(string str)
        {
            return str.Replace("\0", "");
        }


        // load data from app.config
        static bool LoadConfig()
        {
            bool loadSuccess = true;
            try {
                _mode = ConfigurationSettings.AppSettings["Mode"];
            } catch (Exception e) { log.Error("Failed to find Mode in app.confg");loadSuccess = false; }

            try {
                _port = Convert.ToInt32(ConfigurationSettings.AppSettings["Port"]);
            } catch (Exception e) { log.Error("Failed to find Port in app.config");loadSuccess = false; }

           
            try {
                _token = ConfigurationSettings.AppSettings["Token"];
            } catch (Exception e) { log.Error("Failed to find Token in app.config");loadSuccess = false; }

            try
            {
                string tmp = ConfigurationSettings.AppSettings["LogResultDataSet"].ToLower();
                if (tmp == "true") { _logdataset = true;  }
            }
            catch (Exception e) { log.Error("Failed to find LogResultDataSet in app.config"); loadSuccess = false; }

            if (_mode == "SQLServer")
            {
                try {
                    _connStr = System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ToString();
                } catch (Exception e) { log.Error("Failed to find connection string for SQLServer value in app.config");loadSuccess = false; }
            }
            else if(_mode == "SSAS")
            {
                try
                {
                    _connStr = System.Configuration.ConfigurationManager.ConnectionStrings["SSAServer"].ToString();
                }
                catch (Exception e) { log.Error("Failed to find connection string for SSAServer value in app.config"); loadSuccess = false; }
            }

            return loadSuccess;
        }

        // load available plugins
        static void LoadPlugins()
        {
            string dir = System.Environment.CurrentDirectory + "\\Lib";
            if (System.IO.Directory.Exists(dir))
            {
                string[] files= System.IO.Directory.GetFiles(dir);
                if (files.Length > 0)
                {
                    Console.WriteLine(">> Loading plug-ins....");
                }

                foreach (string file in files)
                {
                   
                    _plugInList.Add(file);
                    Console.WriteLine(".... " + file);
                }
            }
        }

   
        // main module
        static void Main(string[] args)
        {
            string serverResponse = "";

            // log config
            if (!(LoadConfig()))
            {
                Console.WriteLine("Missing expected configuration infomration in app.config. Please see logs for details");
                Console.WriteLine(">> Server did not start");
                log.Error("Server failed to start");
                return;
            }

            TcpListener serverSocket = null;
            int requestCount = 0;
            TcpClient clientSocket = null;

            Console.WriteLine(">>------------------------------------------------------------------------");
            Console.WriteLine(">> OLAPRGateway (c)2015 Jeremy Deats");
            Console.WriteLine(">>------------------------------------------------------------------------");
            Console.WriteLine(">> This software is licensed under the GNU GPL (General Public License)");
            Console.WriteLine(">> Terms and conditions of the GNU GPL can be found at this location");
            Console.WriteLine(">> http://www.gnu.org/licenses/gpl-3.0.en.html");
            Console.WriteLine(">> ");
            Console.WriteLine(">> You may freely modify and redistribute this software following the terms ");
            Console.WriteLine(">> outlined inthe GNU GPL.");  
            Console.WriteLine(">> ");
            Console.WriteLine(">> ABSOLUTELY NO WARRANTY is provided for this software.");
            Console.WriteLine(">> By continuing to use this software you agree to assume all liability ");
            Console.WriteLine(">> for any and all events that may result from its use");
            Console.WriteLine(">> ");
            Console.WriteLine(">> log4net is licensed under the Apache License.");
            Console.WriteLine(">> https://logging.apache.org/log4net/license.html");
            Console.WriteLine(">>------------------------------------------------------------------------");

            LoadPlugins();

        // bind port and start server
        InitServer:
            try
            {
                serverSocket = new TcpListener(_port);
                clientSocket = default(TcpClient);
                serverSocket.Start();
                requestCount = 0;
                Console.WriteLine(">> Gateway successfully started");
                Console.WriteLine(">> Waiting for client connection on port " + _port);
                clientSocket = serverSocket.AcceptTcpClient();


                IPEndPoint ipep = (IPEndPoint)clientSocket.Client.RemoteEndPoint;
                IPAddress ipa = ipep.Address;

                Console.WriteLine(" >> Client connected from " + ipa.ToString());
                log.Info("Connection from " + ipa.ToString());

            }
            catch (IOException e)
            {
                log.Error("Failed to bind port and start server", e);
                log.Error("Server failed to start");
                Console.Write(" >> Failed to bind port and start server");
                Console.Write(" >> Server did not start");
                return;
            }
               
            bool continueStream = true;

            try {
                while (continueStream)
                {
                    try
                    {
                        requestCount = requestCount + 1;
                        NetworkStream networkStream = null;
                        try
                        {
                            networkStream = clientSocket.GetStream();
                        }
                        catch (Exception e)
                        {
                            continueStream = false;
                            log.Error("Stream was closed unexpectedly", e);
                            break;
                        }

                        //networkStream.Write(E("HELLO C#"), 0, "HELLO C#".Length);
                        byte[] bytesFrom = new byte[10025];
                        try
                        {
                            networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                        }
                        catch (Exception exc)
                        {
                            continueStream = false;
                            break;
                        }
                        string dataFromClient = C(System.Text.Encoding.ASCII.GetString(bytesFrom));
                        if (dataFromClient.Trim().Length > 0)
                        {
                            if (!(ValidateToken(ref dataFromClient)))
                            {
                                Console.WriteLine(" >> Client validation token was invalid. Terminating session");
                                log.Error("Invalid client token provided with request: " + dataFromClient);
                                log.Error("Terminated session");
                                break;
                            }
                        }
                        else
                        {
                            // exit if next line from stream is empty
                            break;
                        }

                        Console.WriteLine(" >> Received: " + dataFromClient);
                        log.Info("Received:" + dataFromClient);

                        if (dataFromClient.Contains("Lib="))
                        {
                            serverResponse = PlugInExec(dataFromClient);
                        }
                        else
                        {
                            switch (_mode)
                            {
                                case "SQLServer":
                                    serverResponse = SQLConnectAndExec(dataFromClient);
                                    break;

                                case "SSAS":
                                    serverResponse = SSASConnectAndExec(dataFromClient);
                                    break;

                            }
                        }
                        
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                        Console.WriteLine(" >> " + serverResponse);
                        if (_logdataset)
                        {
                            log.Info(serverResponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        continueStream = false;
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine(">> Undefined exception occured while trying to stream to or from client");
                log.Error("Undefined exception occured while streaming ", e);
            }

            try
            {
                clientSocket.Close();
                serverSocket.Stop();
            }
            catch (Exception ex) { }
            Console.WriteLine(" >> flushing and restarting");
            goto InitServer;
        }


    }
}
