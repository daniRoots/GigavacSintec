//Mario A. Dominguez Guerrero 
//June - 2023

#region System Libraries
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

#region Project Libraries

#endregion

namespace SensataSoftware_DEMO
{
    class SQLDatabase
    {
        #region Variables

        #region SQL parameters
        //Station ID
        private static Int64 stationID;
        public Int64 StationID
        {
            get
            {
                return stationID;
            }

            set
            {
                stationID = value;
            }
        }
        //Product: Serial Number of the Machine (MachineConfig)
        private static Int64 equipmentID;
        public Int64 EquipmentID
        {
            get
            {
                return equipmentID;
            }

            set
            {
                equipmentID = value;
            }
        }
        //Product: Station name register on the DB (MachineConfig)
        private static string station;
        public string Station
        {
            get
            {
                return station;
            }

            set
            {
                station = value;
            }
        }
        //Product: Models availables into the DB
        private static string deviceID;
        public string DeviceID
        {
            get
            {
                return deviceID;
            }

            set
            {
                deviceID = value;
            }
        }
        //Next Station
        private static string nextStation;
        public string NextStation
        {
            get
            {
                return nextStation;
            }

            set
            {
                nextStation = value;
            }
        }
        //Product: Serial Number
        private static Int64 serial;
        public Int64 Serial
        {
            get
            {
                return serial;
            }

            set
            {
                serial = value;
            }
        }
        //Device Family
        private static string deviceFamily;
        public string DeviceFamily
        {
            get
            {
                return deviceFamily;
            }

            set
            {
                deviceFamily = value;
            }
        }
        #endregion

        #region SQL Settings
        /// Server & Database
        /// SQL Server 2008
        ///
        /*----------------------------------------------------------------------------*/
        //Cloud SQL Server
        //Data source: SAGPDBV02 (MachineConfig)
        private static string server;
        public string Server
        {
            get
            {
                return server;
            }

            set
            {
                server = value;
            }
        }
        //Initial Catalog: APTProduction (MachineConfig)
        private static string dataBase;
        public string DataBase
        {
            get
            {
                return dataBase;
            }

            set
            {
                dataBase = value;
            }
        }
        // User and Password (permission for connection to DB) (MachineConfig)
        private static string sQLServerUser;
        public string SQLServerUser
        {
            get
            {
                return sQLServerUser;
            }

            set
            {
                sQLServerUser = value;
            }
        }
        private string SQLServerPassword = "*.mm,Tt,Y-r~*cICZ&ACcY,)M4%rJj";
        // User and Password (permission for query) Applicatin Role (MachineConfig)
        private static string sQLqueryUser;
        public string SQLqueryUser
        {
            get
            {
                return sQLqueryUser;
            }

            set
            {
                sQLqueryUser = value;
            }
        }
        private string SQLqueryPassword = "]3~Eht,fXWJciykQ)K?T%@zd*]p4)T";
        private string MachineRoleQuery_set = "sys.sp_setapprole ";
        private string MachineRoleQuery_unset = "sys.sp_unsetapprole ";
        #endregion

        #region SQL Queries
        //Programmability
        //OFM: Prior op Check (*.odb) (MachineConfig)
        private static string tblDeviceIDs;
        public string TblDeviceIDs
        {
            get
            {
                return tblDeviceIDs;
            }

            set
            {
                tblDeviceIDs = value;
            }
        }
        //Queries
        SqlCommand DeviceIDsQuery;
        //Master Parts Sequence View
        private static string tblMastersPartsSeqView;
        public string TblMastersPartsSeqView
        {
            get
            {
                return tblMastersPartsSeqView;
            }

            set
            {
                tblMastersPartsSeqView = value;
            }
        }
        public SqlCommand MastersPartsSeqViewQuery;
        //App Role @cookie
        private byte[] _appRoleEnableCookie;

        #region GFPA: Station Queries (Limits & Results History)
        public class CStationsQueries
        {
            //Result History
            string tblResultsHistory;
            public string TblResultsHistory
            {
                get
                {
                    return tblResultsHistory;
                }

                set
                {
                    tblResultsHistory = value;
                }
            }
            public SqlCommand ResultsHistoryQuery;
            //Limits
            string tblLimits;
            public string TblLimits
            {
                get
                {
                    return tblLimits;
                }

                set
                {
                    tblLimits = value;
                }
            }
            //Return
            int limits_Return;
            public int Limits_Return
            {
                get
                {
                    return limits_Return;
                }

                set
                {
                    limits_Return = value;
                }
            }
            //Limits Messages
            string limits_Msg;
            public string Limits_Msg
            {
                get
                {
                    return limits_Msg;
                }

                set
                {
                    limits_Msg = value;
                }
            }
            public SqlCommand LimitsQuery;
            //Station ID
            Int64 stationID;
            public Int64 StationID
            {
                get
                {
                    return stationID;
                }

                set
                {
                    stationID = value;
                }
            }
            //Product: Serial Number of the Machine (MachineConfig)
            Int64 equipmentID;
            public Int64 EquipmentID
            {
                get
                {
                    return equipmentID;
                }

                set
                {
                    equipmentID = value;
                }
            }
            //Product: Station name register on the DB (MachineConfig)
            string station;
            public string Station
            {
                get
                {
                    return station;
                }

                set
                {
                    station = value;
                }
            }

            public CStationsQueries()
            {

            }
        }
        #endregion

        #region GFPA: Prior op Check (*.odb) (MachineConfig)
        string tblPriorOpCheck;
        public string TblPriorOpCheck
        {
            get
            {
                return tblPriorOpCheck;
            }

            set
            {
                tblPriorOpCheck = value;
            }
        }
        int priorOpChk_Return;
        public int PriorOpChk_Return
        {
            get
            {
                return priorOpChk_Return;
            }

            set
            {
                priorOpChk_Return = value;
            }
        }
        //Prior Op Check Messages
        string pOChkMsg;
        public string POChkMsg
        {
            get
            {
                return pOChkMsg;
            }

            set
            {
                pOChkMsg = value;
            }
        }
        string pOChkMsg2;
        public string POChkMsg2
        {
            get
            {
                return pOChkMsg2;
            }

            set
            {
                pOChkMsg2 = value;
            }
        }
        string pOChkMsg3;
        public string POChkMsg3
        {
            get
            {
                return pOChkMsg3;
            }

            set
            {
                pOChkMsg3 = value;
            }
        }
        //Queries
        public SqlCommand PriorOpChkQuery;
        #endregion

        #region GFPA: Check Components
        string tblCheckComponents;
        public string TblCheckComponents
        {
            get
            {
                return tblCheckComponents;
            }

            set
            {
                tblCheckComponents = value;
            }
        }
        int checkComponents_Return;
        public int CheckComponents_Return
        {
            get
            {
                return checkComponents_Return;
            }

            set
            {
                checkComponents_Return = value;
            }
        }
        //Update Device Status Messages
        string checkComponents_Msg;
        public string CheckComponents_Msg
        {
            get
            {
                return checkComponents_Msg;
            }

            set
            {
                checkComponents_Msg = value;
            }
        }

        //Queries
        public SqlCommand CheckComponentsQuery;
        #endregion

        #region GFPA: Assign Components to Serial ID
        string tblAssignComponentsToSerial;
        public string TblAssignComponentsToSerial
        {
            get
            {
                return tblAssignComponentsToSerial;
            }

            set
            {
                tblAssignComponentsToSerial = value;
            }
        }
        int assignComponentsToSerial_Return;
        public int AssignComponentsToSerial_Return
        {
            get
            {
                return assignComponentsToSerial_Return;
            }

            set
            {
                assignComponentsToSerial_Return = value;
            }
        }
        //Update Device Status Messages
        string assignComponentsToSerial_Msg;
        public string AssignComponentsToSerial_Msg
        {
            get
            {
                return assignComponentsToSerial_Msg;
            }

            set
            {
                assignComponentsToSerial_Msg = value;
            }
        }

        //Queries
        public SqlCommand AssignComponentsToSerialQuery;
        //Assign & Check Components ID
        static string assignChkCmpID;
        public string AssignChkCmpID
        {
            get
            {
                return assignChkCmpID;
            }

            set
            {
                assignChkCmpID = value;
            }
        }
        #endregion

        #region GFPA: Update Device Status (*.odb) (MachineConfig)
        string tblUpdateDeviceStatus;
        public string TblUpdateDeviceStatus
        {
            get
            {
                return tblUpdateDeviceStatus;
            }

            set
            {
                tblUpdateDeviceStatus = value;
            }
        }
        int updateDevice_Return;
        public int UpdateDevice_Return
        {
            get
            {
                return updateDevice_Return;
            }

            set
            {
                updateDevice_Return = value;
            }
        }
        //Update Device Status Messages
        string upDevStatMsg;
        public string UpDevStatMsg
        {
            get
            {
                return upDevStatMsg;
            }

            set
            {
                upDevStatMsg = value;
            }
        }
        //Status of the process finished
        string statusFinished;
        public string StatusFinished
        {
            get
            {
                return statusFinished;
            }

            set
            {
                statusFinished = value;
            }
        }
        //Queries
        public SqlCommand UpdateDeviceStatusQuery;
        #endregion

        #region GFPA: Components Traceability
        //Scan Components table
        string tblScanComponents;
        public string TblScanComponents
        {
            get
            {
                return tblScanComponents;
            }

            set
            {
                tblScanComponents = value;
            }
        }
        //Scan Components return
        int scanComponents_Return;
        public int ScanComponents_Return
        {
            get
            {
                return scanComponents_Return;
            }

            set
            {
                scanComponents_Return = value;
            }
        }
        //Scan Components Messages
        string scanComponents_Msg;
        public string ScanComponents_Msg
        {
            get
            {
                return scanComponents_Msg;
            }

            set
            {
                scanComponents_Msg = value;
            }
        }
        //Queries
        public SqlCommand ScanComponentsQuery;

        //Required Components table
        string tblRequiredComponents;
        public string TblRequiredComponents
        {
            get
            {
                return tblRequiredComponents;
            }

            set
            {
                tblRequiredComponents = value;
            }
        }
        //Required Components return
        int reqComponents_Return;
        public int ReqComponents_Return
        {
            get
            {
                return reqComponents_Return;
            }

            set
            {
                reqComponents_Return = value;
            }
        }
        //Required Components Messages
        string reqComponents_Msg;
        public string ReqComponents_Msg
        {
            get
            {
                return reqComponents_Msg;
            }

            set
            {
                reqComponents_Msg = value;
            }
        }
        //Queries
        public SqlCommand RequiredComponentsQuery;
        #endregion

        #endregion

        #region Limits
        // Measurements Limits
        public string pathHeatingLimitsCSVData = "ProductionData/HeatingLimits.csv";
        public string pathPICLimitsCSVData = "ProductionData/PICLimits.csv";
        public string pathGLTLimitsCSVData = "ProductionData/GLTLimits.csv";
        //Device ID List
        public string pathDeviceIDData = "ProductionData/DeviceID_List.txt";
        //Limits for Heating, Press inner Can and Gross Leak Test Stations
        public class CLimits
        {
            public int Limits_SIZE; //A16955-CP, DOGMA, Setting from DB
            public int Master_SIZE;
            public int MasterParam_SIZE;
            public int MasterStatus_SIZE;
            int masterSampleQty;
            public int MasterSampleQty
            {
                get
                {
                    return masterSampleQty;
                }

                set
                {
                    masterSampleQty = value;
                }
            }

            /* [0] = Device ID
            .
            .
            .
               [N] = Settings Configuration, //A16955-CP, DOGMA, Setting from DB
            */
            string[] limits;
            public string[] Limits
            {
                get
                {
                    return limits;
                }

                set
                {
                    limits = value;
                }
            }

            ///Masters Parameters
            /// [0] = Serial
            /// [1] = Device ID
            /// [2] = Station
            /// [3] = Status
            /// [4] = Expected Value
            /// [5] = Sequence Number
            /// [6] = Nest
            //Target
            string[,] masterTarget;
            public string[,] MasterTarget
            {
                get
                {
                    return masterTarget;
                }

                set
                {
                    masterTarget = value;
                }
            }
            //Results
            string[,] masterResult;
            public string[,] MasterResult
            {
                get
                {
                    return masterResult;
                }

                set
                {
                    masterResult = value;
                }
            }
            /// Masters Status
            /// [0] = Omitted, Waiting, Done, Good, Fail
            /// [1] = Counter
            int[] masterProcessStatus;
            public int[] MasterProcessStatus
            {
                get
                {
                    return masterProcessStatus;
                }

                set
                {
                    masterProcessStatus = value;
                }
            }

            //Initialization
            public CLimits(int LimitsSize, int MasterSize, int MasterParamSIZE, int MasterStatusSIZE)
            {
                this.Limits_SIZE = LimitsSize;
                this.Master_SIZE = MasterSize;
                this.MasterParam_SIZE = MasterParamSIZE;
                this.MasterStatus_SIZE = MasterStatusSIZE;

                limits = new string[Limits_SIZE];
                masterTarget = new string[Master_SIZE, MasterParam_SIZE];
                masterResult = new string[Master_SIZE, MasterParam_SIZE];
                masterProcessStatus = new int[MasterStatus_SIZE];

                //default values
                for (int i = 0; i < Limits_SIZE; i++)
                    limits[i] = "0";
                for (int i = 0; i < Master_SIZE; i++)
                    for (int j = 0; j < MasterParam_SIZE; j++)
                        masterTarget[i, j] = "0";
                for (int i = 0; i < Master_SIZE; i++)
                    for (int j = 0; j < MasterParam_SIZE; j++)
                        masterResult[i, j] = "0";
                for (int i = 0; i < MasterStatus_SIZE; i++)
                    masterProcessStatus[i] = 0;
            }
        }
        #endregion

        #region Production and Lot Data
        public const int ProdLotParam_SIZE = 4;
        public const int Stations_SIZE = 4;
        /// [0] = Device ID
        /// [1] = Lot ID
        /// [2] = 
        /// [3] = 
        private static string[,] prodLot_Param = new string[Stations_SIZE, ProdLotParam_SIZE];
        public string[,] ProdLot_Param
        {
            get
            {
                return prodLot_Param;
            }

            set
            {
                prodLot_Param = value;
            }
        }
        #endregion

        #endregion

        #region Callbacks

        #endregion

        #region Objects 
        //Scan Components
        static CStationsQueries oScanCompQueries;
        public CStationsQueries OScanCompQueries { get { return oScanCompQueries; } set { oScanCompQueries = value; } }
        //Install Terminal Shield Station Limits
        static CLimits oInstallTermShldLimits;
        public CLimits OInstallTermShldLimits { get { return oInstallTermShldLimits; } set { oInstallTermShldLimits = value; } }
        static CStationsQueries oInstallTermShldQueries;
        public CStationsQueries OInstallTermShldQueries { get { return oInstallTermShldQueries; } set { oInstallTermShldQueries = value; } }
        //Install M8 Inserts Station Limits
        static CLimits oInstallM8InsrLimits;
        public CLimits OInstallM8InsrLimits { get { return oInstallM8InsrLimits; } set { oInstallM8InsrLimits = value; } }
        static CStationsQueries oInstallM8InsrQueries;
        public CStationsQueries OInstallM8InsrQueries { get { return oInstallM8InsrQueries; } set { oInstallM8InsrQueries = value; } }
        //Plunger & Armature Assy Station Limits
        static CLimits oPlugrArmAssyLimits;
        public CLimits OPlugrArmAssyLimits { get { return oPlugrArmAssyLimits; } set { oPlugrArmAssyLimits = value; } }
        static CStationsQueries oPlugrArmAssyQueries;
        public CStationsQueries OPlugrArmAssyQueries { get { return oPlugrArmAssyQueries; } set { oPlugrArmAssyQueries = value; } }
        //Latch Assy Station Limits
        static CLimits oLatchAssyLimits;
        public CLimits OLatchAssyLimits { get { return oLatchAssyLimits; } set { oLatchAssyLimits = value; } }
        static CStationsQueries oLatchAssyQueries;
        public CStationsQueries OLatchAssyQueries { get { return oLatchAssyQueries; } set { oLatchAssyQueries = value; } }
        #endregion

        public SQLDatabase()
        {
            //Scan Components
            oScanCompQueries = new CStationsQueries();
            //Install Terminal Shield Station Limits
            oInstallTermShldLimits = new CLimits(6, 10, 7, 2);
            oInstallTermShldQueries = new CStationsQueries();
            //Install M8 Inserts Station Limits
            oInstallM8InsrLimits = new CLimits(7, 10, 7, 2);
            oInstallM8InsrQueries = new CStationsQueries();
            //Plunger & Armature Assy Station Limits
            oPlugrArmAssyLimits = new CLimits(34, 10, 7, 2);
            oPlugrArmAssyQueries = new CStationsQueries();
            //Latch Assy Station Limits
            oLatchAssyLimits = new CLimits(12, 10, 7, 2);
            oLatchAssyQueries = new CStationsQueries();
        }

        #region Controls

        #endregion

        #region Information

        #endregion

        #region Functions

        #region Public
        //SQL Server 2008: Connection management
        public string Connect()
        {
            string ConnectionParam = @"Data Source=" + server + ";Initial Catalog=" + dataBase + ";User ID=" + sQLServerUser + ";Password=" + SQLServerPassword;
            return ConnectionParam;
        }
        public void Disconnect(SqlConnection Connection)
        {
            //Close the Connection      
            Connection.Close();
        }
        //SQL Server 2008: Application Role management
        private Boolean ExecuteSetAppRole(SqlConnection Connection)
        {
            bool Succ = true;
            //Application Role: Enable
            try
            {
                //SQL command: Query permission
                SqlCommand cmd = new SqlCommand(MachineRoleQuery_set);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = Connection;
                SqlParameter paramAppRoleName = new SqlParameter();
                paramAppRoleName.Direction = ParameterDirection.Input;
                paramAppRoleName.ParameterName = "@rolename";
                paramAppRoleName.Value = sQLqueryUser;
                cmd.Parameters.Add(paramAppRoleName);

                SqlParameter paramAppRolePwd = new SqlParameter();
                paramAppRolePwd.Direction = ParameterDirection.Input;
                paramAppRolePwd.ParameterName = "@password";
                paramAppRolePwd.Value = SQLqueryPassword;
                cmd.Parameters.Add(paramAppRolePwd);

                SqlParameter paramCreateCookie = new SqlParameter();
                paramCreateCookie.Direction = ParameterDirection.Input;
                paramCreateCookie.ParameterName = "@fCreateCookie";
                paramCreateCookie.DbType = DbType.Boolean;
                paramCreateCookie.Value = 1;
                cmd.Parameters.Add(paramCreateCookie);

                SqlParameter paramEncrypt = new SqlParameter();
                paramEncrypt.Direction = ParameterDirection.Input;
                paramEncrypt.ParameterName = "@encrypt";
                paramEncrypt.Value = "none";
                cmd.Parameters.Add(paramEncrypt);

                SqlParameter paramEnableCookie = new SqlParameter();
                paramEnableCookie.ParameterName = "@cookie";
                paramEnableCookie.DbType = DbType.Binary;
                paramEnableCookie.Direction = ParameterDirection.Output;
                paramEnableCookie.Size = 1000;
                cmd.Parameters.Add(paramEnableCookie);

                //Execute the query
                cmd.ExecuteNonQuery();
                SqlParameter outCookie = cmd.Parameters["@cookie"];
                // Store the enabled cookie so that approle  can be disabled with the cookie.
                _appRoleEnableCookie = (byte[])outCookie.Value;

            }
            catch (Exception)
            {
                Succ = false;
                //HMI.OForm.SystemMessages("SQL: Set App Role failed\n", "Error");
            }

            return Succ;
        }
        private Boolean ExecuteUnsetAppRole(SqlConnection Connection)
        {
            bool Succ = true;
            //Application Role: Disable
            try
            {
                SqlCommand cmd = new SqlCommand(MachineRoleQuery_unset);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = Connection;
                SqlParameter paramEnableCookie = new SqlParameter();
                paramEnableCookie.Direction = ParameterDirection.Input;
                paramEnableCookie.ParameterName = "@cookie";
                paramEnableCookie.Value = this._appRoleEnableCookie;
                cmd.Parameters.Add(paramEnableCookie);

                //Execute the query
                cmd.ExecuteNonQuery();
                _appRoleEnableCookie = null;
            }
            catch (Exception)
            {
                Succ = false;
                //HMI.OForm.SystemMessages("SQL: Unset App Role failed\n", "Error");
            }

            return Succ;
        }
        //SQL Server 2008: Queries & Store Procedures

        //Device IDs, Device Family is the Production Line (Ceramic, GVe, GFPA)
        public Boolean DeviceIDs(string DeviceFamily)
        {
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Device IDs
                    #region Device IDs
                    try
                    {
                        //SQL command (Table)
                        using (DeviceIDsQuery = new SqlCommand("SELECT * FROM " + tblDeviceIDs + " WHERE DeviceFamily = @p1", SQLport))
                        {
                            //Parameters
                            DeviceIDsQuery.Parameters.Add(new SqlParameter("p1", DeviceFamily));

                            List<object[]> rowList = new List<object[]>();
                            using (SqlDataReader reader = DeviceIDsQuery.ExecuteReader())
                            {
                                // while there is another record present
                                while (reader.Read())
                                {
                                    //Get the Device ID List from Device Family DB
                                    //Add all data
                                    object[] values = new object[reader.FieldCount];
                                    reader.GetValues(values);
                                    rowList.Add(values);
                                }

                                #region Device IDs List    
                                //Clear file
                                File.WriteAllText(pathDeviceIDData, String.Empty);
                                //Open File with the current date
                                StreamWriter StartUpSoft = File.AppendText(pathDeviceIDData);
                                for (int i = 0; i < rowList.Count; i++)
                                {
                                    //Write the file
                                    StartUpSoft.WriteLine(rowList[i][0].ToString());
                                }
                                //Close File
                                StartUpSoft.Close();
                                #endregion
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //HMI.OForm.SystemMessages("SQL: Device IDs downloading failed\n", "Error");
                        return false;
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
                return true;
            }
            catch (Exception)
            {
                //HMI.OForm.SystemMessages("SQL: Server connection failed\n", "Error");
                return false;
            }
        }

        #region Common
        //Prior Operation Check
        public Boolean PriorOpCheck(CStationsQueries OStation, Int64 SerialNumber, string DeviceID)
        {
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Prior Op Check
                    #region Prior Op Check
                    try
                    {
                        //SQL command (Stored procedure)
                        using (PriorOpChkQuery = new SqlCommand(TblPriorOpCheck, SQLport))
                        {
                            //Specify the stored procedure command
                            PriorOpChkQuery.CommandType = CommandType.StoredProcedure;
                            //Add Parameters INPUT to the command
                            PriorOpChkQuery.Parameters.AddWithValue("@SerialNo", SerialNumber);
                            PriorOpChkQuery.Parameters.AddWithValue("@DeviceID", DeviceID);
                            PriorOpChkQuery.Parameters.AddWithValue("@StationName", OStation.Station);
                            PriorOpChkQuery.Parameters.AddWithValue("@EquipmentID", OStation.EquipmentID);

                            //Prepare the return value
                            PriorOpChkQuery.Parameters.Add("@return_value", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;

                            //Add Parameters OUTPUT to the command, for the Prior Op Check Messages
                            SqlParameter parameter1 = new SqlParameter();
                            parameter1.ParameterName = "@MessageText";
                            parameter1.SqlDbType = SqlDbType.VarChar;
                            parameter1.Direction = ParameterDirection.Output;
                            parameter1.Size = 500;
                            // Add the parameter to the Parameters collection. 
                            PriorOpChkQuery.Parameters.Add(parameter1);

                            //Last Station Name
                            SqlParameter parameter2 = new SqlParameter();
                            parameter2.ParameterName = "@LastStationName";
                            parameter2.SqlDbType = SqlDbType.VarChar;
                            parameter2.Direction = ParameterDirection.Output;
                            parameter2.Size = 200;

                            // Add the parameter to the Parameters collection. 
                            PriorOpChkQuery.Parameters.Add(parameter2);

                            //Part Status
                            SqlParameter parameter3 = new SqlParameter();
                            parameter3.ParameterName = "@PartStatus";
                            parameter3.SqlDbType = SqlDbType.VarChar;
                            parameter3.Direction = ParameterDirection.Output;
                            parameter3.Size = 200;

                            // Add the parameter to the Parameters collection. 
                            PriorOpChkQuery.Parameters.Add(parameter3);

                            //Execute the query            
                            PriorOpChkQuery.ExecuteNonQuery();
                            //Read the Status of the Serial number
                            PriorOpChk_Return = (int)PriorOpChkQuery.Parameters["@return_value"].Value;
                            if (PriorOpChk_Return == 0)
                            {
                                //Read the Message from the Prior Operation Check procedure
                                POChkMsg = PriorOpChkQuery.Parameters["@MessageText"].Value.ToString();
                                POChkMsg2 = PriorOpChkQuery.Parameters["@LastStationName"].Value.ToString();
                                POChkMsg3 = PriorOpChkQuery.Parameters["@PartStatus"].Value.ToString();
                            }
                            else
                            {
                                POChkMsg = PriorOpChkQuery.Parameters["@MessageText"].Value.ToString();
                                POChkMsg2 = PriorOpChkQuery.Parameters["@LastStationName"].Value.ToString();
                                POChkMsg3 = PriorOpChkQuery.Parameters["@PartStatus"].Value.ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        PriorOpChk_Return = 0;
                        POChkMsg = "SQL Exception Error";
                        POChkMsg2 = "SQL Exception Error";
                        POChkMsg3 = "SQL Exception Error";
                        //HMI.OForm.SystemMessages("SQL: Prior Op Check failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();
                }
                return true;
            }
            catch (Exception)
            {
                //HMI.OForm.SystemMessages("SQL: Server connection failed\n", "Error");
                return false;
            }
        }
        //Update Device Status
        public Boolean UpdateDeviceStatus(CStationsQueries OStation, Int64 SerialNumber, string DeviceID, string Status)
        {
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Update Device Status
                    #region Update Device Status
                    try
                    {
                        //SQL command (Stored procedure)
                        using (UpdateDeviceStatusQuery = new SqlCommand(TblUpdateDeviceStatus, SQLport))
                        {
                            //Specify the stored procedure command
                            UpdateDeviceStatusQuery.CommandType = CommandType.StoredProcedure;
                            //Add Parameters to the command
                            UpdateDeviceStatusQuery.Parameters.AddWithValue("@SerialNo", SerialNumber);
                            UpdateDeviceStatusQuery.Parameters.AddWithValue("@DeviceID", DeviceID);
                            UpdateDeviceStatusQuery.Parameters.AddWithValue("@StationName", OStation.Station);
                            UpdateDeviceStatusQuery.Parameters.AddWithValue("@EquipmentID", OStation.EquipmentID);
                            UpdateDeviceStatusQuery.Parameters.AddWithValue("@Status", Status);
                            //PriorOpChkQuery.Parameters.AddWithValue("@MessageText", pOChkMsg);
                            UpdateDeviceStatusQuery.Parameters.Add("@return_value", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;
                            //Add Parameters to the command, for the Prior Op Check Messages
                            SqlParameter parameter1 = new SqlParameter();
                            //Messages text
                            parameter1.ParameterName = "@MessageText";
                            parameter1.SqlDbType = SqlDbType.VarChar;
                            parameter1.Direction = ParameterDirection.InputOutput;
                            parameter1.Size = 100;
                            parameter1.Value = UpDevStatMsg;
                            // Add the parameter to the Parameters collection. 
                            UpdateDeviceStatusQuery.Parameters.Add(parameter1);
                            //Execute the query            
                            UpdateDeviceStatusQuery.ExecuteNonQuery();
                            //Read the Status of the Serial number
                            UpdateDevice_Return = (int)UpdateDeviceStatusQuery.Parameters["@return_value"].Value;
                            if (UpdateDevice_Return == 0)
                                //Read the Message from the Prior Operation Check procedure
                                UpDevStatMsg = UpdateDeviceStatusQuery.Parameters["@MessageText"].Value.ToString();
                            else
                                UpDevStatMsg = UpdateDeviceStatusQuery.Parameters["@MessageText"].Value.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        UpdateDevice_Return = 0;
                        UpDevStatMsg = "SQL Exception Error";
                        //HMI.OForm.SystemMessages("SQL: Update Device Status failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
                return true;
            }
            catch (Exception)
            {
                //HMI.OForm.SystemMessages("SQL: Server connection failed\n", "Error");
                return false;
            }
        }
        //Components Lot Traceability
        //Check Components
        public Boolean CheckComponents(CStationsQueries OStation, string DeviceID)
        {
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Check Components
                    #region Check Components
                    try
                    {
                        //SQL command (Stored procedure)
                        using (CheckComponentsQuery = new SqlCommand(TblCheckComponents, SQLport))
                        {
                            //Specify the stored procedure command
                            CheckComponentsQuery.CommandType = CommandType.StoredProcedure;
                            //Add Parameters INPUT to the command
                            CheckComponentsQuery.Parameters.AddWithValue("@DeviceID", DeviceID);
                            CheckComponentsQuery.Parameters.AddWithValue("@StationID", OStation.StationID);
                            CheckComponentsQuery.Parameters.AddWithValue("@EquipmentID", Convert.ToInt32(AssignChkCmpID));    //Check Components query it doesn't use the Equipment ID of the machine or station

                            //Prepare the return value
                            CheckComponentsQuery.Parameters.Add("@return_value", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;

                            //Add Parameters OUTPUT to the command, for the Check Components Messages
                            SqlParameter parameter1 = new SqlParameter();
                            parameter1.ParameterName = "@MessageText";
                            parameter1.SqlDbType = SqlDbType.VarChar;
                            parameter1.Direction = ParameterDirection.Output;
                            parameter1.Size = 500;
                            // Add the parameter to the Parameters collection. 
                            CheckComponentsQuery.Parameters.Add(parameter1);

                            //Execute the query            
                            CheckComponentsQuery.ExecuteNonQuery();
                            //Read the Status of the Serial number
                            CheckComponents_Return = (int)CheckComponentsQuery.Parameters["@return_value"].Value;
                            if (CheckComponents_Return == 0)
                                //Read the Message from the Prior Operation Check procedure
                                CheckComponents_Msg = CheckComponentsQuery.Parameters["@MessageText"].Value.ToString();
                            else
                                CheckComponents_Msg = CheckComponentsQuery.Parameters["@MessageText"].Value.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        CheckComponents_Return = 0;
                        CheckComponents_Msg = "SQL Exception Error";
                        //HMI.OForm.SystemMessages("SQL: Check Component failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();
                }
                return true;
            }
            catch (Exception)
            {
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
                return false;
            }
        }
        //Assign Components to Serial ID
        public Boolean AssignComponentsToSerial(CStationsQueries OStation, Int64 SerialNumber, string DeviceID, string LotID)
        {
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Assign Components To Serial
                    #region AssignComponentsToSerial
                    try
                    {
                        //SQL command (Stored procedure)
                        using (AssignComponentsToSerialQuery = new SqlCommand(TblAssignComponentsToSerial, SQLport))
                        {
                            //Specify the stored procedure command
                            AssignComponentsToSerialQuery.CommandType = CommandType.StoredProcedure;
                            //Add Parameters to the command
                            AssignComponentsToSerialQuery.Parameters.AddWithValue("@Station", OStation.StationID);
                            AssignComponentsToSerialQuery.Parameters.AddWithValue("@EquipmentID", Convert.ToInt32(AssignChkCmpID)); //Check Components query it doesn't use the Equipment ID of the machine or station
                            AssignComponentsToSerialQuery.Parameters.AddWithValue("@DeviceID", DeviceID);
                            AssignComponentsToSerialQuery.Parameters.AddWithValue("@SerialNo", SerialNumber);
                            AssignComponentsToSerialQuery.Parameters.AddWithValue("@LotID", LotID);
                            AssignComponentsToSerialQuery.Parameters.AddWithValue("@AssignEquipmentID", OStation.EquipmentID);
                            //PriorOpChkQuery.Parameters.AddWithValue("@MessageText", pOChkMsg);
                            AssignComponentsToSerialQuery.Parameters.Add("@return_value", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;
                            //Add Parameters to the command, for the Prior Op Check Messages
                            SqlParameter parameter1 = new SqlParameter();
                            //Messages text
                            parameter1.ParameterName = "@MessageText";
                            parameter1.SqlDbType = SqlDbType.VarChar;
                            parameter1.Direction = ParameterDirection.Output;
                            parameter1.Size = 100;
                            // Add the parameter to the Parameters collection. 
                            AssignComponentsToSerialQuery.Parameters.Add(parameter1);
                            //Execute the query            
                            AssignComponentsToSerialQuery.ExecuteNonQuery();
                            //Query's results
                            AssignComponentsToSerial_Return = (int)AssignComponentsToSerialQuery.Parameters["@return_value"].Value;
                            //Query's messages
                            AssignComponentsToSerial_Msg = AssignComponentsToSerialQuery.Parameters["@MessageText"].Value.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        AssignComponentsToSerial_Return = 0;
                        AssignComponentsToSerial_Msg = "SQL Exception Error";
                        //HMI.OForm.SystemMessages("SQL: Assign Components To Serial failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
                return true;
            }
            catch (Exception)
            {
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
                return false;
            }
        }
        #endregion

        #region Components Traceability

        #endregion

        #region Install Terminal Shields
        //Limits 
        public Boolean InstallTermShldLimitsDB(string DeviceID)
        {
            int bSucc = 0;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Limits
                    #region Get the Limits
                    try
                    {
                        //SQL command (Table)
                        using (oInstallTermShldQueries.LimitsQuery = new SqlCommand(oInstallTermShldQueries.TblLimits, SQLport))
                        {
                            //Specify the stored procedure command
                            oInstallTermShldQueries.LimitsQuery.CommandType = CommandType.StoredProcedure;

                            //Add Parameters to the command
                            oInstallTermShldQueries.LimitsQuery.Parameters.AddWithValue("@DeviceID", DeviceID);
                            oInstallTermShldQueries.LimitsQuery.Parameters.AddWithValue("@Category", oInstallTermShldQueries.Station);
                            oInstallTermShldQueries.LimitsQuery.Parameters.Add("@return_value", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;

                            List<string[]> rowList = new List<string[]>();
                            //Execute the query            
                            using (SqlDataReader reader = oInstallTermShldQueries.LimitsQuery.ExecuteReader())
                            {

                                // while there is another record present
                                while (reader.Read())
                                {
                                    //Save all data
                                    string[] values = new string[reader.FieldCount];
                                    reader.GetValues(values);
                                    rowList.Add(values);
                                }
                                /* Index	ParamType	ParamName	                        ParamOrd	ParamValue	ParamDescr 

                                */
                                //oInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.Displacement_Max]   = rowList[0][1];
                                //oInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.Displacement_Min]   = rowList[1][1];
                                //oInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.Force_Max]          = rowList[2][1];
                                //oInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.Force_Min]          = rowList[3][1];
                                oInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.ThresholdX_Max]     = rowList[0][1];
                                oInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.ThresholdX_Min]     = rowList[1][1];
                                oInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.ThresholdY_Max]     = rowList[2][1];
                                oInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.ThresholdY_Min]     = rowList[3][1];
                                //A16955-CP, DOGMA, Setting from DB
                                //oInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.Settings] = rowList[8][1];
                                oInstallTermShldLimits.Limits[(int)eInstallTermShld_Prod_Limits.Settings] = "65535";
                                //Fill out Limits
                                //HMI.OForm.GetLimits_InstallTermShld();
                            }

                            //Read the Status of the query
                            oInstallTermShldQueries.Limits_Return = (int)oInstallTermShldQueries.LimitsQuery.Parameters["@return_value"].Value;
                        }
                    }
                    catch (Exception)
                    {
                        bSucc++;
                        //HMI.OForm.SystemMessages("SQL: Limits downloadings failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
            }
            catch (Exception)
            {
                bSucc++;
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
            }

            //There was any error?
            if (bSucc > 0)
            {
                return false;
            }
            else
                return true;

        }
        //Results History
        public Boolean InstallTermShldResultsDB(Int64 SerialNumber, string DeviceID, string LotID, string PartStatus,
                                                string MaxForceLeft,
                                                string MaxDistanceLeft,
                                                string MaxForceRight,
                                                string MaxDistanceRight)
        {
            bool bSucc = false;
            int bSuccx = 0;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Spring Force test history
                    #region Save the Results
                    try
                    {
                        //Attemps (3 times) to Insert the results WO A16913-CP
                        for (int i = 0; i < 3; i++)
                        {
                            //SQL command (Table)
                            using (oInstallTermShldQueries.ResultsHistoryQuery = new SqlCommand("INSERT INTO " + oInstallTermShldQueries.TblResultsHistory + " (SerialNo, DeviceID, TestTime, EquipmentID , LotID,  PartStatus ,MaxForceLeft,MaxDistanceLeft,MaxForceRight, MaxDistanceRight) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10)", SQLport))
                            {
                                //Parameters
                                oInstallTermShldQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p1", SerialNumber));
                                oInstallTermShldQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p2", DeviceID));
                                oInstallTermShldQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p3", DateTime.Now));
                                oInstallTermShldQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p4", oInstallTermShldQueries.EquipmentID));
                                oInstallTermShldQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p5", LotID));
                                oInstallTermShldQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p6", PartStatus));
                                oInstallTermShldQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p7", Convert.ToDouble(MaxForceLeft)));
                                oInstallTermShldQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p8", Convert.ToDouble(MaxDistanceLeft)));
                                oInstallTermShldQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p9", Convert.ToDouble(MaxForceRight)));
                                oInstallTermShldQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p10", Convert.ToDouble(MaxDistanceRight)));
                                //Execute Query
                                bSuccx = oInstallTermShldQueries.ResultsHistoryQuery.ExecuteNonQuery();
                            }

                            if (bSuccx > 0)
                            {
                                bSucc = true;
                                break;
                            }
                            else
                                bSucc = false;
                        }

                    }
                    catch (Exception)
                    {
                        bSucc = false;
                        //HMI.OForm.SystemMessages("SQL: Store results History failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }
            }
            catch (Exception)
            {
                bSucc = false;
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
            }

            return bSucc;
        }
        //Master Parts Sequence View
        public Boolean InstallTermShldMasterPartsSequence(string DeviceID)
        {
            bool bSucc = false;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query the Master Part Sequence
                    #region Master Parts Sequence
                    try
                    {
                        //SQL command (Table)
                        using (MastersPartsSeqViewQuery = new SqlCommand("SELECT * FROM " + tblMastersPartsSeqView + " WHERE DeviceID = @p1 AND Station = @p2", SQLport))
                        {
                            //Parameters
                            MastersPartsSeqViewQuery.Parameters.Add(new SqlParameter("p1", DeviceID));
                            MastersPartsSeqViewQuery.Parameters.Add(new SqlParameter("p2", oInstallTermShldQueries.Station));

                            List<object[]> rowList = new List<object[]>();
                            using (SqlDataReader reader = MastersPartsSeqViewQuery.ExecuteReader())
                            {

                                // while there is another record present
                                while (reader.Read())
                                {
                                    //Add all data
                                    object[] values = new object[reader.FieldCount];
                                    reader.GetValues(values);
                                    rowList.Add(values);
                                }

                                //HMI.OForm.OTblMasters_InstallTermShld.Clear();

                                #region Save all the Masters Parameters
                                if (rowList.Count > 0)
                                {
                                    for (int i = 0; i < rowList.Count; i++)
                                    {
                                        for (int k = 0; k < oInstallTermShldLimits.MasterParam_SIZE; k++)
                                        {
                                            oInstallTermShldLimits.MasterTarget[i, k] = rowList[i][k].ToString();
                                        }
                                        ////HMI.OForm.OTblMasters_InstallTermShld.Rows.Add(oInstallTermShldLimits.MasterTarget[i, (int)MastersParameters.Serial],
                                        //                                                oInstallTermShldLimits.MasterTarget[i, (int)MastersParameters.DeviceID],
                                        //                                                oInstallTermShldLimits.MasterTarget[i, (int)MastersParameters.Station],
                                        //                                                oInstallTermShldLimits.MasterTarget[i, (int)MastersParameters.Status],
                                        //                                                oInstallTermShldLimits.MasterTarget[i, (int)MastersParameters.ExpecValue],
                                        //                                                oInstallTermShldLimits.MasterTarget[i, (int)MastersParameters.SequenceNo],
                                        //                                                oInstallTermShldLimits.MasterTarget[i, (int)MastersParameters.Nest]);
                                    }
                                    bSucc = true;
                                }
                                else
                                    bSucc = false;
                                #endregion

                            }
                        }
                    }
                    catch (Exception)
                    {
                        //HMI.OForm.SystemMessages("SQL: Master parts sequence failed\n", "Error");
                        bSucc = false;
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
            }
            catch (Exception)
            {
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
                bSucc = false;
            }
            return bSucc;
        }
        #endregion

        #region Install M8 Inserts
        //Limits 
        public Boolean InstallM8InsrLimitsDB(string DeviceID)
        {
            int bSucc = 0;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Limits
                    #region Get the Limits
                    try
                    {
                        //SQL command (Table)
                        using (oInstallM8InsrQueries.LimitsQuery = new SqlCommand(oInstallM8InsrQueries.TblLimits, SQLport))
                        {
                            //Specify the stored procedure command
                            oInstallM8InsrQueries.LimitsQuery.CommandType = CommandType.StoredProcedure;

                            //Add Parameters to the command
                            oInstallM8InsrQueries.LimitsQuery.Parameters.AddWithValue("@DeviceID", DeviceID);
                            oInstallM8InsrQueries.LimitsQuery.Parameters.AddWithValue("@Category", oInstallM8InsrQueries.Station);
                            oInstallM8InsrQueries.LimitsQuery.Parameters.Add("@return_value", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;

                            List<string[]> rowList = new List<string[]>();
                            //Execute the query            
                            using (SqlDataReader reader = oInstallM8InsrQueries.LimitsQuery.ExecuteReader())
                            {

                                // while there is another record present
                                while (reader.Read())
                                {
                                    //Save all data
                                    string[] values = new string[reader.FieldCount];
                                    reader.GetValues(values);
                                    rowList.Add(values);
                                }
                                /* Index	ParamType	ParamName	                        ParamOrd	ParamValue	ParamDescr 

                                */
                                //oInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.Displacement_Max]   = rowList[0][1];
                                //oInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.Displacement_Min]   = rowList[1][1];
                                //oInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.Force_Max]          = rowList[2][1];
                                //oInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.Force_Min]          = rowList[3][1];
                                oInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdX_Max]     = rowList[0][1];
                                oInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdX_Min]     = rowList[1][1];
                                oInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdY_Max]     = rowList[2][1];
                                oInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.ThresholdY_Min]     = rowList[3][1];
                                oInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.VisionRecipe]       = rowList[4][1];
                                //oInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.VisionSystemRecipe] = rowList[9][1];
                                //----------------------------------------------------------------------
                                //A16955-CP, DOGMA, Setting from DB
                                oInstallM8InsrLimits.Limits[(int)eInstallM8Insr_Prod_Limits.Settings] = "65535";
                                //Fill out Limits
                                //HMI.OForm.GetLimits_InstallM8Ins();
                            }

                            //Read the Status of the query
                            oInstallM8InsrQueries.Limits_Return = (int)oInstallM8InsrQueries.LimitsQuery.Parameters["@return_value"].Value;
                        }
                    }
                    catch (Exception)
                    {
                        bSucc++;
                        //HMI.OForm.SystemMessages("SQL: Limits downloading failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
            }
            catch (Exception)
            {
                bSucc++;
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
            }

            //There was any error?
            if (bSucc > 0)
            {
                return false;
            }
            else
                return true;

        }
        //Result History
        public Boolean InstallM8InsrResultsDB(Int64 SerialNumber, string DeviceID, string LotID, string PartStatus,
                                             string MaxForceinsert1,
                                             string MaxDistanceInsert1,
                                             string MaxForceInsert2,
                                             string MaxDistanceInsert2)
        {
            bool bSucc = false;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Spring Force test history
                    #region Save the Results
                    try
                    {
                        //SQL command (Table)
                        using (oInstallM8InsrQueries.ResultsHistoryQuery = new SqlCommand("INSERT INTO " + oInstallM8InsrQueries.TblResultsHistory + " (SerialNo, DeviceID, TestTime, EquipmentID , LotID, PartStatus , MaxForceinsert1, MaxDistanceInsert1, MaxForceInsert2, MaxDistanceInsert2) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10)", SQLport))
                        {
                            //Parameters
                            oInstallM8InsrQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p1", SerialNumber));
                            oInstallM8InsrQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p2", DeviceID));
                            oInstallM8InsrQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p3", DateTime.Now));
                            oInstallM8InsrQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p4", oInstallM8InsrQueries.EquipmentID));
                            oInstallM8InsrQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p5", LotID));
                            oInstallM8InsrQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p6", PartStatus));
                            oInstallM8InsrQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p7", Convert.ToDouble(MaxForceinsert1)));
                            oInstallM8InsrQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p8", Convert.ToDouble(MaxDistanceInsert1)));
                            oInstallM8InsrQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p9", Convert.ToDouble(MaxForceInsert2)));
                            oInstallM8InsrQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p10", Convert.ToDouble(MaxDistanceInsert2)));
                            //Execute Query
                            oInstallM8InsrQueries.ResultsHistoryQuery.ExecuteNonQuery();
                        }
                        bSucc = true;
                    }
                    catch (Exception)
                    {
                        bSucc = false;
                        //HMI.OForm.SystemMessages("SQL: Store Result History failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
                bSucc = true;
            }
            catch (Exception)
            {
                bSucc = false;
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
            }

            return bSucc;
        }
        //Master Parts Sequence View
        public Boolean InstallM8InsrMasterPartsSequence(string DeviceID)
        {
            bool bSucc = false;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query the Master Part Sequence
                    #region Master Parts Sequence
                    try
                    {
                        //SQL command (Table)
                        using (MastersPartsSeqViewQuery = new SqlCommand("SELECT * FROM " + tblMastersPartsSeqView + " WHERE DeviceID = @p1 AND Station = @p2", SQLport))
                        {
                            //Parameters
                            MastersPartsSeqViewQuery.Parameters.Add(new SqlParameter("p1", DeviceID));
                            MastersPartsSeqViewQuery.Parameters.Add(new SqlParameter("p2", oInstallM8InsrQueries.Station));

                            List<object[]> rowList = new List<object[]>();
                            using (SqlDataReader reader = MastersPartsSeqViewQuery.ExecuteReader())
                            {

                                // while there is another record present
                                while (reader.Read())
                                {
                                    //Add all data
                                    object[] values = new object[reader.FieldCount];
                                    reader.GetValues(values);
                                    rowList.Add(values);
                                }

                                //HMI.OForm.OTblMasters_InstallM8Insr.Clear();

                                #region Save all the Masters Parameters
                                if (rowList.Count > 0)
                                {
                                    for (int i = 0; i < rowList.Count; i++)
                                    {
                                        for (int k = 0; k < oInstallM8InsrLimits.MasterParam_SIZE; k++)
                                        {
                                            oInstallM8InsrLimits.MasterTarget[i, k] = rowList[i][k].ToString();
                                        }
                                        //HMI.OForm.OTblMasters_InstallM8Insr.Rows.Add(oInstallM8InsrLimits.MasterTarget[i, (int)MastersParameters.Serial],
                                                                                        //oInstallM8InsrLimits.MasterTarget[i, (int)MastersParameters.DeviceID],
                                                                                        //oInstallM8InsrLimits.MasterTarget[i, (int)MastersParameters.Station],
                                                                                        //oInstallM8InsrLimits.MasterTarget[i, (int)MastersParameters.Status],
                                                                                        //oInstallM8InsrLimits.MasterTarget[i, (int)MastersParameters.ExpecValue],
                                                                                        //oInstallM8InsrLimits.MasterTarget[i, (int)MastersParameters.SequenceNo],
                                                                                        //oInstallM8InsrLimits.MasterTarget[i, (int)MastersParameters.Nest]);
                                    }
                                    bSucc = true;
                                }
                                else
                                    bSucc = false;
                                #endregion
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //HMI.OForm.SystemMessages("SQL: Master parts sequence failed\n", "Error");
                        bSucc = false;
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
            }
            catch (Exception)
            {
                //HMI.OForm.SystemMessages("SQL: Server connection failed\n", "Error");
                bSucc = false;
            }

            return bSucc;
        }
        #endregion

        #region Plunger & Armature Assy
        //Limits 
        public Boolean PlugrArmAssyLimitsDB(string DeviceID)
        {
            int bSucc = 0;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Limits
                    #region Get the Limits
                    try
                    {
                        //SQL command (Table)
                        using (oPlugrArmAssyQueries.LimitsQuery = new SqlCommand(oPlugrArmAssyQueries.TblLimits, SQLport))
                        {
                            //Specify the stored procedure command
                            oPlugrArmAssyQueries.LimitsQuery.CommandType = CommandType.StoredProcedure;

                            //Add Parameters to the command
                            oPlugrArmAssyQueries.LimitsQuery.Parameters.AddWithValue("@DeviceID", DeviceID);
                            oPlugrArmAssyQueries.LimitsQuery.Parameters.AddWithValue("@Category", oPlugrArmAssyQueries.Station);
                            oPlugrArmAssyQueries.LimitsQuery.Parameters.Add("@return_value", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;

                            List<string[]> rowList = new List<string[]>();
                            //Execute the query            
                            using (SqlDataReader reader = oPlugrArmAssyQueries.LimitsQuery.ExecuteReader())
                            {

                                // while there is another record present
                                while (reader.Read())
                                {
                                    //Save all data
                                    string[] values = new string[reader.FieldCount];
                                    reader.GetValues(values);
                                    rowList.Add(values);

                                }

                                /* Index	ParamType	ParamName	                        ParamOrd	ParamValue	ParamDescr 
                                ///[0]   PSI_Max    float       Pressure upper limit
                                ///[1]   PSI_Min    float       pressure lower limit
                                ///[2]   Lmn        float       Leak upper Limit
                                ///[3]   Lmn        float       Leak lower limit

                                //23  int Spring Force Test 3 / zSettings - 1         1021         Settings Configuration
                                */
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Angle_Max]            = rowList[0][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Angle_Min]            = rowList[1][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ArmatureInspection]   = rowList[2][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ClampAngle_Max]       = rowList[3][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ClampAngle_Min]       = rowList[4][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ClampTorque_Max]      = rowList[5][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ClampTorque_Min]      = rowList[6][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ConstantK_max]        = rowList[7][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ConstantK_min]        = rowList[8][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ReadyPos]             = rowList[9][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ReadySpeed]           = rowList[10][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ScrewdrivingPos]      = rowList[11][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.ScrewdrivingSpeed]    = rowList[12][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.SP_Max]               = rowList[13][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.SP_Min]               = rowList[14][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.SpringInspection]     = rowList[15][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.StandbyPos]           = rowList[16][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.StandbySpeed]         = rowList[17][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChXmax]          = rowList[18][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChXmin]          = rowList[19][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChYmax]          = rowList[20][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1ChYmin]          = rowList[21][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1Pos]             = rowList[22][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test1Speed]           = rowList[23][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChXmax]          = rowList[24][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChXmin]          = rowList[25][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChYmax]          = rowList[26][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2ChYmin]          = rowList[27][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2Pos]             = rowList[28][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Test2Speed]           = rowList[29][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Torque_Max]           = rowList[30][1];
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Torque_Min]           = rowList[31][1];
                                //A16955-CP, DOGMA, Setting from DB
                                oPlugrArmAssyLimits.Limits[(int)ePlugrArmAssy_Prod_Limits.Settings] = "65535";
                                //Fill out limits
                                //HMI.OForm.GetLimits_PlugrArmAssy();
                            }

                            //Read the Status of the query
                            oPlugrArmAssyQueries.Limits_Return = (int)oPlugrArmAssyQueries.LimitsQuery.Parameters["@return_value"].Value;
                        }
                    }
                    catch (Exception)
                    {
                        bSucc++;
                        //HMI.OForm.SystemMessages("SQL: Limits downloadings failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
            }
            catch (Exception)
            {
                bSucc++;
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
            }

            //There was any error?
            if (bSucc > 0)
            {
                return false;
            }
            else
                return true;

        }
        //Results History
        public Boolean PlugrArmAssyResultsDB(Int64 SerialNumber, string DeviceID, string LotID, string PartStatus,
                                            string Torque, 
                                            string Angle,
                                            string Clamp,
                                            string SP,
                                            string ClampAngle,
                                            string Test1_Force,
                                            string Test1_Displacement,
                                            string Test2_Force,
                                            string Test2_Displacement,
                                            string ConstK)
        {
            bool bSucc = false;
            int bSuccx = 0;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Spring Force test history
                    #region Save the Results
                    try
                    {
                        //Attemps (3 times) to Insert the results WO A16913-CP
                        for (int i = 0; i < 3; i++)
                        {
                            //SQL command (Table)
                            using (oPlugrArmAssyQueries.ResultsHistoryQuery = new SqlCommand("INSERT INTO " + oPlugrArmAssyQueries.TblResultsHistory + "(SerialNo, DeviceID, TestTime, EquipmentID, LotID, PartStatus, Torque, Angle, Clamp, SeatingPoint, ClampAngle, Test1_Force, Test1_Displacement, Test2_Force, Test2_Displacement, ConstK) VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16)", SQLport))
                            {
                                //Parameters
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p1", SerialNumber));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p2", DeviceID));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p3", DateTime.Now));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p4", oPlugrArmAssyQueries.EquipmentID));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p5", LotID));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p6", PartStatus));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p7", Convert.ToDouble(Torque)));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p8", Convert.ToDouble(Angle)));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p9", Convert.ToDouble(Clamp)));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p10", Convert.ToDouble(SP)));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p11", Convert.ToDouble(ClampAngle)));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p12", Convert.ToDouble(Test1_Force)));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p13", Convert.ToDouble(Test1_Displacement)));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p14", Convert.ToDouble(Test2_Force)));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p15", Convert.ToDouble(Test2_Displacement)));
                                oPlugrArmAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p16", Convert.ToDouble(ConstK)));

                                //Execute Query
                                bSuccx = oPlugrArmAssyQueries.ResultsHistoryQuery.ExecuteNonQuery();
                            }

                            if (bSuccx > 0)
                            {
                                bSucc = true;
                                break;
                            }
                            else
                                bSucc = false;
                        }

                    }
                    catch (Exception)
                    {
                        bSucc = false;
                        //HMI.OForm.SystemMessages("SQL: Store results History failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }
            }
            catch (Exception)
            {
                bSucc = false;
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
            }

            return bSucc;
        }
        //Master Parts Sequence View
        public Boolean PlugrArmAssyMasterPartsSequence(string DeviceID)
        {
            bool bSucc = false;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query the Master Part Sequence
                    #region Master Parts Sequence
                    try
                    {
                        //SQL command (Table)
                        using (MastersPartsSeqViewQuery = new SqlCommand("SELECT * FROM " + tblMastersPartsSeqView + " WHERE DeviceID = @p1 AND Station = @p2", SQLport))
                        {
                            //Parameters
                            MastersPartsSeqViewQuery.Parameters.Add(new SqlParameter("p1", DeviceID));
                            MastersPartsSeqViewQuery.Parameters.Add(new SqlParameter("p2", oPlugrArmAssyQueries.Station));

                            List<object[]> rowList = new List<object[]>();
                            using (SqlDataReader reader = MastersPartsSeqViewQuery.ExecuteReader())
                            {

                                // while there is another record present
                                while (reader.Read())
                                {
                                    //Add all data
                                    object[] values = new object[reader.FieldCount];
                                    reader.GetValues(values);
                                    rowList.Add(values);
                                }

                                //HMI.OForm.OTblMasters_PlungrArmAssy.Clear();

                                #region Save all the Masters Parameters
                                if (rowList.Count > 0)
                                {
                                    for (int i = 0; i < rowList.Count; i++)
                                    {
                                        for (int k = 0; k < oPlugrArmAssyLimits.MasterParam_SIZE; k++)
                                        {
                                            oPlugrArmAssyLimits.MasterTarget[i, k] = rowList[i][k].ToString();
                                        }
                                        //HMI.OForm.OTblMasters_PlungrArmAssy.Rows.Add(oPlugrArmAssyLimits.MasterTarget[i, (int)MastersParameters.Serial],
                                                                                        //oPlugrArmAssyLimits.MasterTarget[i, (int)MastersParameters.DeviceID],
                                                                                        //oPlugrArmAssyLimits.MasterTarget[i, (int)MastersParameters.Station],
                                                                                        //oPlugrArmAssyLimits.MasterTarget[i, (int)MastersParameters.Status],
                                                                                        //oPlugrArmAssyLimits.MasterTarget[i, (int)MastersParameters.ExpecValue],
                                                                                        //oPlugrArmAssyLimits.MasterTarget[i, (int)MastersParameters.SequenceNo],
                                                                                        //oPlugrArmAssyLimits.MasterTarget[i, (int)MastersParameters.Nest]);
                                    }
                                    bSucc = true;
                                }
                                else
                                    bSucc = false;
                                #endregion
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //HMI.OForm.SystemMessages("SQL: Master parts sequence failed\n", "Error");
                        bSucc = false;
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
            }
            catch (Exception)
            {
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
                bSucc = false;
            }
            return bSucc;
        }
        #endregion

        #region Latch Assy
        //Limits 
        public Boolean LatchAssyLimitsDB(string DeviceID)
        {
            int bSucc = 0;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Limits
                    #region Get the Limits
                    try
                    {
                        //SQL command (Table)
                        using (oLatchAssyQueries.LimitsQuery = new SqlCommand(oLatchAssyQueries.TblLimits, SQLport))
                        {
                            //Specify the stored procedure command
                            oLatchAssyQueries.LimitsQuery.CommandType = CommandType.StoredProcedure;

                            //Add Parameters to the command
                            oLatchAssyQueries.LimitsQuery.Parameters.AddWithValue("@DeviceID", DeviceID);
                            oLatchAssyQueries.LimitsQuery.Parameters.AddWithValue("@Category", oLatchAssyQueries.Station);
                            oLatchAssyQueries.LimitsQuery.Parameters.Add("@return_value", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.ReturnValue;

                            List<string[]> rowList = new List<string[]>();
                            //Execute the query            
                            using (SqlDataReader reader = oLatchAssyQueries.LimitsQuery.ExecuteReader())
                            {

                                // while there is another record present
                                while (reader.Read())
                                {
                                    //Save all data
                                    string[] values = new string[reader.FieldCount];
                                    reader.GetValues(values);
                                    rowList.Add(values);
                                }
                                /* Index	ParamType	ParamName	                        ParamOrd	ParamValue	ParamDescr 

                                */
                                oLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Angle_Max]          = rowList[0][1];
                                oLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Angle_Min]          = rowList[1][1];
                                oLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.ClampAngle_Max]     = rowList[2][1];
                                oLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.ClampAngle_Min]     = rowList[3][1];
                                oLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.ClampTorque_Max]    = rowList[4][1];
                                oLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.ClampTorque_Min]    = rowList[5][1];
                                oLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.SP_Max]             = rowList[6][1];
                                oLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.SP_Min]             = rowList[7][1];
                                oLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Torque_Max]         = rowList[8][1];
                                oLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Torque_Min]         = rowList[9][1];
                                //A16955-CP, DOGMA, Setting from DB
                                oLatchAssyLimits.Limits[(int)eLatchAssy_Prod_Limits.Settings] = "65535";
                                //HMI.OForm.GetLimits_LatchAssys();
                            }

                            //Read the Status of the query
                            oLatchAssyQueries.Limits_Return = (int)oLatchAssyQueries.LimitsQuery.Parameters["@return_value"].Value;
                        }
                    }
                    catch (Exception)
                    {
                        bSucc++;
                        //HMI.OForm.SystemMessages("SQL: Limits downloadings failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
            }
            catch (Exception)
            {
                bSucc++;
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
            }

            //There was any error?
            if (bSucc > 0)
            {
                return false;
            }
            else
                return true;

        }
        //Results History
        public Boolean LatchAssyResultsDB(Int64 SerialNumber, string DeviceID, string LotID, string PartStatus,
                                        string Torque,
                                        string Angle,
                                        string Clamp,
                                        string SP,
                                        string ClampAngle)
        {
            bool bSucc = false;
            int bSuccx = 0;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query to Spring Force test history
                    #region Save the Results
                    try
                    {
                        //Attemps (3 times) to Insert the results WO A16913-CP
                        for (int i = 0; i < 3; i++)
                        {
                            //SQL command (Table)
                            using (oLatchAssyQueries.ResultsHistoryQuery = new SqlCommand("INSERT INTO " + oLatchAssyQueries.TblResultsHistory + " (SerialNo, DeviceID, TestTime, EquipmentID , LotID,  PartStatus , Torque , Angle, Clamp ,SeatingPoint, ClampAngle) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11)", SQLport))
                            {
                                //Parameters
                                oLatchAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p1", SerialNumber));
                                oLatchAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p2", DeviceID));
                                oLatchAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p3", DateTime.Now));
                                oLatchAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p4", oLatchAssyQueries.EquipmentID));
                                oLatchAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p5", LotID));
                                oLatchAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p6", PartStatus));
                                oLatchAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p7", Convert.ToDouble(Torque)));
                                oLatchAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p8", Convert.ToDouble(Angle)));
                                oLatchAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p9", Convert.ToDouble(Clamp)));
                                oLatchAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p10", Convert.ToDouble(SP)));
                                oLatchAssyQueries.ResultsHistoryQuery.Parameters.Add(new SqlParameter("p11", Convert.ToDouble(ClampAngle)));
                                //Execute Query
                                bSuccx = oLatchAssyQueries.ResultsHistoryQuery.ExecuteNonQuery();
                            }

                            if (bSuccx > 0)
                            {
                                bSucc = true;
                                break;
                            }
                            else
                                bSucc = false;
                        }

                    }
                    catch (Exception)
                    {
                        bSucc = false;
                        //HMI.OForm.SystemMessages("SQL: Store results History failed\n", "Error");
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }
            }
            catch (Exception)
            {
                bSucc = false;
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
            }

            return bSucc;
        }
        //Master Parts Sequence View
        public Boolean LatchAssyMasterPartsSequence(string DeviceID)
        {
            bool bSucc = false;
            string ConnectionParam = Connect();
            try
            {
                //Connection to Server & DB
                using (SqlConnection SQLport = new SqlConnection(ConnectionParam))
                {
                    //Open the Conexion
                    SQLport.Open();
                    //Application Role Permission: Enable
                    ExecuteSetAppRole(SQLport);

                    //Query the Master Part Sequence
                    #region Master Parts Sequence
                    try
                    {
                        //SQL command (Table)
                        using (MastersPartsSeqViewQuery = new SqlCommand("SELECT * FROM " + tblMastersPartsSeqView + " WHERE DeviceID = @p1 AND Station = @p2", SQLport))
                        {
                            //Parameters
                            MastersPartsSeqViewQuery.Parameters.Add(new SqlParameter("p1", DeviceID));
                            MastersPartsSeqViewQuery.Parameters.Add(new SqlParameter("p2", oLatchAssyQueries.Station));

                            List<object[]> rowList = new List<object[]>();
                            using (SqlDataReader reader = MastersPartsSeqViewQuery.ExecuteReader())
                            {

                                // while there is another record present
                                while (reader.Read())
                                {
                                    //Add all data
                                    object[] values = new object[reader.FieldCount];
                                    reader.GetValues(values);
                                    rowList.Add(values);
                                }

                                //HMI.OForm.OTblMasters_LatchAssy.Clear();

                                #region Save all the Masters Parameters
                                if (rowList.Count > 0)
                                {
                                    for (int i = 0; i < rowList.Count; i++)
                                    {
                                        for (int k = 0; k < oLatchAssyLimits.MasterParam_SIZE; k++)
                                        {
                                            oLatchAssyLimits.MasterTarget[i, k] = rowList[i][k].ToString();
                                        }
                                        //HMI.OForm.OTblMasters_LatchAssy.Rows.Add(oLatchAssyLimits.MasterTarget[i, (int)MastersParameters.Serial],
                                                                                        //oLatchAssyLimits.MasterTarget[i, (int)MastersParameters.DeviceID],
                                                                                        //oLatchAssyLimits.MasterTarget[i, (int)MastersParameters.Station],
                                                                                        //oLatchAssyLimits.MasterTarget[i, (int)MastersParameters.Status],
                                                                                        //oLatchAssyLimits.MasterTarget[i, (int)MastersParameters.ExpecValue],
                                                                                        //oLatchAssyLimits.MasterTarget[i, (int)MastersParameters.SequenceNo],
                                                                                        //oLatchAssyLimits.MasterTarget[i, (int)MastersParameters.Nest]);
                                    }
                                    bSucc = true;
                                }
                                else
                                    bSucc = false;
                                #endregion
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //HMI.OForm.SystemMessages("SQL: Master parts sequence failed\n", "Error");
                        bSucc = false;
                    }
                    #endregion

                    //Application Role Permission: Disable
                    ExecuteUnsetAppRole(SQLport);
                    //Close the Connection
                    SQLport.Close();

                }//Close the Connection
            }
            catch (Exception)
            {
                //HMI.OForm.SystemMessages("SQL: Server Connection failed\n", "Error");
                bSucc = false;
            }
            return bSucc;
        }
        #endregion

        #endregion

        #region Public

        #region CSV file: Load Limits

        #endregion

        #endregion

        #endregion

        #region Threads

        #endregion
    }
}
